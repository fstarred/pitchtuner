using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace WpfPitchTuner.Settings
{
    class PreferencesSettings : ApplicationSettingsBase
    {

        [UserScopedSettingAttribute()]
        [DefaultSettingValue("0")]
        public int SelectedDevice
        {
            get { return (int)(this["SelectedDevice"]); }
            set { this["SelectedDevice"] = value; }
        }

        [UserScopedSettingAttribute()]
        public string BassUser
        {
            get { return (string)(this["BassUser"]); }
            set { this["BassUser"] = value; }
        }

        [UserScopedSettingAttribute()]
        public string BassCode
        {
            get { return (string)(this["BassCode"]); }
            set { this["BassCode"] = value; }
        }

        [UserScopedSettingAttribute()]
        [DefaultSettingValue("false")]
        public bool EnableProxy
        {
            get { return (bool)(this["EnableProxy"]); }
            set { this["EnableProxy"] = value; }
        }

        [UserScopedSettingAttribute()]
        public string Host
        {
            get { return (string)(this["Host"]); }
            set { this["Host"] = value; }
        }

        [UserScopedSettingAttribute()]
        [DefaultSettingValue("8080")]
        public int Port
        {
            get { return (int)(this["Port"]); }
            set { this["Port"] = value; }
        }

        [UserScopedSettingAttribute()]
        public string ProxyUser
        {
            get { return (string)(this["ProxyUser"]); }
            set { this["ProxyUser"] = value; }
        }

        [UserScopedSettingAttribute()]
        public string ProxyPassword
        {
            get { return (string)(this["ProxyPassword"]); }
            set { this["ProxyPassword"] = value; }
        }

        [UserScopedSettingAttribute()]
        public string ProxyDomain
        {
            get { return (string)(this["ProxyDomain"]); }
            set { this["ProxyDomain"] = value; }
        }
    }
}
