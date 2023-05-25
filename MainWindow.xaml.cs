using QRCoder;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using SIAQr.Models;
using Microsoft.Win32;
using System.Diagnostics;
using System.Drawing;

namespace SIAQr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FunctionLoadCombos();
            FunctionCollapseAll();
        }

        private void FunctionLoadCombos()
        {
            cmbWifiSecurity.Items.Add("NONE");
            cmbWifiSecurity.Items.Add("WPA/WPA2-PSK");
            //cmbWifiSecurity.Items.Add("WPA/WPA2-EAP");
            cmbWifiSecurity.Items.Add("WEP");
        }

        private void FunctionMessageBox(string caption, string messageBoxText, string type)
        {
            //  string messageBoxText = "Export to CSV file failed";
            //  string caption = "Error: Export failed";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;
            if (type == "Error")
            {
                icon = MessageBoxImage.Error;
            }

            if (type == "Warning")
            {
                icon = MessageBoxImage.Warning;
            }
            MessageBoxResult result;
            result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.OK);
        }

        private void FunctionDev(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/sibranda",
                UseShellExecute = true
            });
        }

        private string qrCodeJSONData;
        private bool configWifi;
        private bool wifiHidden;
        private bool wifiProxy;
        private bool allSysApps;
        private bool customLogo;
        private string customLogoSrc;

        private string FunctionGenerateJSONData()
        {
            PROVISIONINGADMINEXTRASBUNDLE aQRChild = new PROVISIONINGADMINEXTRASBUNDLE();
            AndroidQR aQR = new AndroidQR();
            aQR.PROVISIONING_DEVICE_ADMIN_COMPONENT_NAME = "com.google.android.apps.work.clouddpc/.receivers.CloudDeviceAdminReceiver";
            aQR.PROVISIONING_DEVICE_ADMIN_SIGNATURE_CHECKSUM = "I5YvS0O5hXY46mb01BlRjq4oJJGs2kuUcHvVkAPEXlg";
            aQR.PROVISIONING_DEVICE_ADMIN_PACKAGE_DOWNLOAD_LOCATION = "https://play.google.com/managed/downloadManagingApp?identifier=setup";
            aQRChild.EXTRA_ENROLLMENT_TOKEN = txtBoxIntuneToken.Text;
            aQR.PROVISIONING_ADMIN_EXTRAS_BUNDLE = aQRChild;

            if (configWifi)
            {
                aQR.PROVISIONING_WIFI_SSID = txtBoxWifiSSID.Text;
                aQR.PROVISIONING_WIFI_PASSWORD = psdWifiPassword.Password;

                switch (cmbWifiSecurity.Text)
                {
                    case "WPA/WPA2-PSK":
                        aQR.PROVISIONING_WIFI_SECURITY_TYPE = "WPA";
                        break;
                    case "WPA/WPA2-EAP":
                        aQR.PROVISIONING_WIFI_SECURITY_TYPE = "EAP";
                        break;
                    default:
                        aQR.PROVISIONING_WIFI_SECURITY_TYPE = cmbWifiSecurity.Text;
                        break;
                }
                aQR.PROVISIONING_WIFI_HIDDEN = wifiHidden;
                if (wifiProxy)
                {
                    aQR.PROVISIONING_WIFI_PROXY_HOST = txtBoxProxyHost.Text;
                    aQR.PROVISIONING_WIFI_PROXY_PORT = txtBoxProxyPort.Text;
                    aQR.PROVISIONING_WIFI_PROXY_BYPASS = txtBoxProxyBypass.Text;
                }
            }
            if (allSysApps)
            {
                aQR.PROVISIONING_LEAVE_ALL_SYSTEM_APPS_ENABLED = allSysApps;
            }
            qrCodeJSONData = JsonConvert.SerializeObject(aQR);
            return qrCodeJSONData;
        }

        private void FunctionCheckData(object sender, EventArgs e)
        {
            imgQRCode.Source = null;
            bool allok = true;
            bool save = false;
            var btnClicked = ((Button)sender).Name;
            if (btnClicked == "SaveFile")
            {
                save = true;
            }

            if (txtBoxIntuneToken.Text == "")
            {
                allok = false;
                FunctionMessageBox("Information: Missing Intune Code", "You must to type an Intune Token code", "Info");
            }
            else if (configWifi)
            {
                if(txtBoxWifiSSID.Text == "")
                {
                    allok = false;
                    FunctionMessageBox("Information: Missing SSID", "You must to type a WiFi SSID Name", "Info");
                }
                else if(cmbWifiSecurity.SelectedValue == null)
                {
                    allok = false;
                    FunctionMessageBox("Information: WiFi Security", "You must select WiFi Security", "Info");
                }
            }

            if (allok)
            {
                FunctionGenerateQRCode(save);
            }
        }

        private void FunctionGenerateQRCode(bool save)
        {
            //BitmapImage qrIco = new BitmapImage(new Uri ("C:\\Scripts\\VS\\WPF\\SIAQr\\SIAQr.ico"));
            //https://github.com/codebude/QRCoder/wiki/Advanced-usage---QR-Code-renderers#21-qrcode-renderer-in-detail
            //https://github.com/codebude/QRCoder

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(FunctionGenerateJSONData(), QRCodeGenerator.ECCLevel.L); //ECCLevel.L its the same Intune uses

            BitmapImage bitmapimage = new BitmapImage();

            if (customLogo)
            {
                string srcImg;
                if (customLogoSrc != null)
                {
                    srcImg = customLogoSrc;
                }
                else
                {
                    srcImg = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "SIAQr.png");
                }

                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20, Color.Black, Color.White, (Bitmap)Bitmap.FromFile(srcImg));

                using (MemoryStream memory = new MemoryStream())
                {
                    qrCodeImage.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    memory.Position = 0;

                    bitmapimage.BeginInit();
                    bitmapimage.StreamSource = memory;
                    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapimage.EndInit();
                }
            }
            else
            {
                BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
                byte[] qrCodeAsBitmapByteArr = qrCode.GetGraphic(20);
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = new MemoryStream(qrCodeAsBitmapByteArr);
                bitmapimage.EndInit();
            }

            imgQRCode.Source = bitmapimage;
            if (save)
            {
                FunctionSavetoFile(bitmapimage);
            }
        }


        private void FunctionOpenCustomLogo(object sender, EventArgs e)
        {
            imgQRCode.Source = null;
            try
            {
                OpenFileDialog customLogoFile = new OpenFileDialog();
                customLogoFile.Title = "Save QR Code to File";
                customLogoFile.DefaultExt = "png";
                customLogoFile.Filter = "PNG files (*.png)|*.png|All files (*.*)|*.*";
                customLogoFile.FilterIndex = 1;
                customLogoFile.RestoreDirectory = true;
                customLogoFile.ShowDialog();
                if (customLogoFile.FileName != "")
                {
                    customLogoSrc = customLogoFile.FileName;
                }
            }
            catch (Exception)
            {
                FunctionMessageBox("Error: Opening File", "Error opening custom logo file", "Error");
            }
        }

        /*   private void FunctionSavetoJSON(object sender, EventArgs e)
           {
               string JSONresult = FunctionGenerateJSONData();
               string path = @"C:\Temp\AndroidQR.json";

               if (File.Exists(path))
               {
                   File.Delete(path);
                   using (var tw = new StreamWriter(path, true))
                   {
                       tw.WriteLine(JSONresult.ToString());
                       tw.Close();
                   }
               }
               else if (!File.Exists(path))
               {
                   using (var tw = new StreamWriter(path, true))
                   {
                       tw.WriteLine(JSONresult.ToString());
                       tw.Close();
                   }
               }

           }*/

        private void FunctionSavetoFile(BitmapImage image)
        {
            try
            {
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.Title = "Save QR Code to File";
                saveFile.DefaultExt = "png";
                saveFile.Filter = "PNG files (*.png)|*.png|All files (*.*)|*.*";
                saveFile.FilterIndex = 1;
                saveFile.RestoreDirectory = true;
                saveFile.ShowDialog();
                var filePath = saveFile.FileName;

                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));

                using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }
            catch (Exception)
            {
                FunctionMessageBox("Error: Saving File", "QRCode PNG File was not Saved", "Error");
            }
        }

        private void FunctionCollapseAll()
        {
            lblWifiSSID.Visibility = Visibility.Collapsed;
            lblWifiSecurity.Visibility = Visibility.Collapsed;
            txtBoxWifiSSID.Visibility = Visibility.Collapsed;
            chkWifiHidden.Visibility = Visibility.Collapsed;
            chkWifiHidden.Visibility = Visibility.Collapsed;
            cmbWifiSecurity.Visibility = Visibility.Collapsed;
            lblWifiPassword.Visibility = Visibility.Collapsed;
            psdWifiPassword.Visibility = Visibility.Collapsed;
            chkConfigProxy.Visibility = Visibility.Collapsed;
            lblProxyHost.Visibility = Visibility.Collapsed;
            txtBoxProxyHost.Visibility = Visibility.Collapsed;
            lblProxyPort.Visibility = Visibility.Collapsed;
            txtBoxProxyPort.Visibility = Visibility.Collapsed;
            lblProxyBypass.Visibility = Visibility.Collapsed;
            txtBoxProxyBypass.Visibility = Visibility.Collapsed;
        }

        private void FunctionConfigureWifi(object sender, RoutedEventArgs e)
        {
            imgQRCode.Source = null;
            if (chkConfigWifi.IsChecked == true)
            {
                lblWifiSSID.Visibility = Visibility.Visible;
                lblWifiSecurity.Visibility = Visibility.Visible;
                txtBoxWifiSSID.Visibility = Visibility.Visible;
                chkWifiHidden.Visibility = Visibility.Visible;
                chkWifiHidden.Visibility = Visibility.Visible;
                cmbWifiSecurity.Visibility = Visibility.Visible;
                lblWifiPassword.Visibility = Visibility.Visible;
                psdWifiPassword.Visibility = Visibility.Visible;
                chkConfigProxy.Visibility = Visibility.Visible;
                configWifi = true;
            }
            else
            {
                FunctionCollapseAll();
                configWifi = false;
                chkWifiHidden.IsChecked = false;
                wifiHidden = false;
                cmbWifiSecurity.SelectedItem = null;
                psdWifiPassword.Password = null;
                chkConfigProxy.IsChecked = false;
                wifiProxy = false;
                txtBoxProxyHost.Text = null;
                txtBoxProxyPort.Text = null;
                txtBoxProxyBypass.Text = null;
            }
        }

        private void FunctionWifiHidden(object sender, RoutedEventArgs e)
        {
            imgQRCode.Source = null;
            if (chkWifiHidden.IsChecked == true)
            {
                wifiHidden = true;
            }
            else
            {
                wifiHidden = false;
            }
        }

        private void FunctionConfigProxy(object sender, RoutedEventArgs e)
        {
            imgQRCode.Source = null;
            if (chkConfigProxy.IsChecked == true)
            {
                wifiProxy = true;
                lblProxyHost.Visibility = Visibility.Visible;
                txtBoxProxyHost.Visibility = Visibility.Visible;
                lblProxyPort.Visibility = Visibility.Visible;
                txtBoxProxyPort.Visibility = Visibility.Visible;
                lblProxyBypass.Visibility = Visibility.Visible;
                txtBoxProxyBypass.Visibility = Visibility.Visible;
            }
            else
            {
                wifiProxy = false;
                lblProxyHost.Visibility = Visibility.Collapsed;
                txtBoxProxyHost.Visibility = Visibility.Collapsed;
                lblProxyPort.Visibility = Visibility.Collapsed;
                txtBoxProxyPort.Visibility = Visibility.Collapsed;
                lblProxyBypass.Visibility = Visibility.Collapsed;
                txtBoxProxyBypass.Visibility = Visibility.Collapsed;
                txtBoxProxyHost.Text = null;
                txtBoxProxyPort.Text = null;
                txtBoxProxyBypass.Text = null;
            }
        }

        private void FunctionEnableSysApps(object sender, RoutedEventArgs e)
        {
            imgQRCode.Source = null;
            if (chkAllSystemApps.IsChecked == true)
            {
                allSysApps = true;
            }
            else
            {
                allSysApps = false;
            }
        }

        private void FunctionEnableCustomLogo(object sender, RoutedEventArgs e)
        {
            imgQRCode.Source = null;
            if (chkCustomLogo.IsChecked == true)
            {
                btnCustomLogo.Visibility = Visibility.Visible;
                customLogo = true;
            }
            else
            {
                btnCustomLogo.Visibility = Visibility.Collapsed;
                customLogo = false;
                customLogoSrc = null;
            }
        }

        private void FunctionNullQr (object sender, TextChangedEventArgs e)
        {
            imgQRCode.Source = null;
        }

        private void FunctionNullQrPsd(object sender, RoutedEventArgs e)
        {
            imgQRCode.Source = null;
        }
    }
}
