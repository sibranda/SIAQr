using Newtonsoft.Json;

namespace SIAQr.Models
{
    internal class PROVISIONINGADMINEXTRASBUNDLE
    {
        [JsonProperty("com.google.android.apps.work.clouddpc.EXTRA_ENROLLMENT_TOKEN", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string EXTRA_ENROLLMENT_TOKEN { get; set; }
    }
    internal class AndroidQR
    {
        [JsonProperty("android.app.extra.PROVISIONING_DEVICE_ADMIN_COMPONENT_NAME", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string PROVISIONING_DEVICE_ADMIN_COMPONENT_NAME { get; set; }

        [JsonProperty("android.app.extra.PROVISIONING_DEVICE_ADMIN_SIGNATURE_CHECKSUM", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string PROVISIONING_DEVICE_ADMIN_SIGNATURE_CHECKSUM { get; set; }

        [JsonProperty("android.app.extra.PROVISIONING_DEVICE_ADMIN_PACKAGE_DOWNLOAD_LOCATION", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string PROVISIONING_DEVICE_ADMIN_PACKAGE_DOWNLOAD_LOCATION { get; set; }

        //  [JsonProperty("android.app.extra.PROVISIONING_ADMIN_EXTRAS_BUNDLE", DefaultValueHandling = DefaultValueHandling.Ignore)]
        // public string PROVISIONING_ADMIN_EXTRAS_BUNDLE { get; set; }

        [JsonProperty("android.app.extra.PROVISIONING_ADMIN_EXTRAS_BUNDLE", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public PROVISIONINGADMINEXTRASBUNDLE PROVISIONING_ADMIN_EXTRAS_BUNDLE { get; set; }

        [JsonProperty("android.app.extra.PROVISIONING_WIFI_SSID", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string PROVISIONING_WIFI_SSID { get; set; }

        [JsonProperty("android.app.extra.PROVISIONING_WIFI_PASSWORD", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string PROVISIONING_WIFI_PASSWORD { get; set; }

        [JsonProperty("android.app.extra.PROVISIONING_WIFI_SECURITY_TYPE", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string PROVISIONING_WIFI_SECURITY_TYPE { get; set; }

        [JsonProperty("android.app.extra.PROVISIONING_WIFI_HIDDEN", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool PROVISIONING_WIFI_HIDDEN { get; set; }

        [JsonProperty("android.app.extra.PROVISIONING_WIFI_PROXY_HOST", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string PROVISIONING_WIFI_PROXY_HOST { get; set; }

        [JsonProperty("android.app.extra.PROVISIONING_WIFI_PROXY_PORT", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string PROVISIONING_WIFI_PROXY_PORT { get; set; }

        [JsonProperty("android.app.extra.PROVISIONING_WIFI_PROXY_BYPASS", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string PROVISIONING_WIFI_PROXY_BYPASS { get; set; }

        [JsonProperty("android.app.extra.PROVISIONING_LEAVE_ALL_SYSTEM_APPS_ENABLED", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool PROVISIONING_LEAVE_ALL_SYSTEM_APPS_ENABLED { get; set; }
    }
}
