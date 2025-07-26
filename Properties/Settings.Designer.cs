namespace Properties {
    internal sealed partial class Settings : System.Configuration.ApplicationSettingsBase {
        private static Settings defaultInstance = ((Settings)(System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        public static Settings Default => defaultInstance;

        [System.Configuration.UserScopedSetting()]
        [System.Configuration.DefaultSettingValue("Red")]
        public string CrosshairColor {
            get => ((string)this["CrosshairColor"]);
            set => this["CrosshairColor"] = value;
        }

        [System.Configuration.UserScopedSetting()]
        [System.Configuration.DefaultSettingValue("2")]
        public int CrosshairThickness {
            get => ((int)this["CrosshairThickness"]);
            set => this["CrosshairThickness"] = value;
        }

        [System.Configuration.UserScopedSetting()]
        [System.Configuration.DefaultSettingValue("Cruz")]
        public string CrosshairShape {
            get => ((string)this["CrosshairShape"]);
            set => this["CrosshairShape"] = value;
        }

        [System.Configuration.UserScopedSetting()]
        [System.Configuration.DefaultSettingValue("1")]
        public float CrosshairScale {
            get => ((float)this["CrosshairScale"]);
            set => this["CrosshairScale"] = value;
        }

        [System.Configuration.UserScopedSetting()]
        [System.Configuration.DefaultSettingValue("100")]
        public int CrosshairPositionX {
            get => ((int)this["CrosshairPositionX"]);
            set => this["CrosshairPositionX"] = value;
        }

        [System.Configuration.UserScopedSetting()]
        [System.Configuration.DefaultSettingValue("100")]
        public int CrosshairPositionY {
            get => ((int)this["CrosshairPositionY"]);
            set => this["CrosshairPositionY"] = value;
        }
    }
}
