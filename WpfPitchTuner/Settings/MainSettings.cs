using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace WpfPitchTuner.Settings
{
    class MainSettings : ApplicationSettingsBase
    {
        [UserScopedSettingAttribute()]
        [DefaultSettingValue("0.005")]
        public double Threshold
        {
            get { return (double)(this["Threshold"]); }
            set { this["Threshold"] = value; }
        }


        [UserScopedSettingAttribute()]
        [DefaultSettingValue("Default")]
        public string SelectedSkin
        {
            get { return (string)(this["SelectedSkin"]); }
            set { this["SelectedSkin"] = value; }
        }
    }
}
