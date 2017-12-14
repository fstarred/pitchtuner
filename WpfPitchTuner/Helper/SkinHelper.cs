using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using WpfPitchTuner.Provider;

namespace WpfPitchTuner.Helper
{
    public static class SkinHelper
    {       
 
        public static void SetSelectedSkin(UIElement element, string value)
        {
            element.SetValue(SelectedSkinProperty, value);
        }

        public static string GetSelectedSkin(UIElement element)
        {
            return (string)element.GetValue(SelectedSkinProperty);
        }

        public static readonly DependencyProperty SelectedSkinProperty = DependencyProperty.RegisterAttached(
              "SelectedSkin",
              typeof(string),
              typeof(SkinHelper),
              new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSelectedSkinChanged))
            );

        static void OnSelectedSkinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (Helper.IsInDesignMode == false)
            {
                string oldskin = (string)e.OldValue;
                string newskin = (string)e.NewValue;

                string uri = "Skins/" + newskin + ".xaml";

                UIElement element = d as UIElement;

                ResourceDictionary resource = null;

                string defaultskin = (string)App.Current.FindResource("defaultSkin") ?? "Default";

                if (newskin.Equals(defaultskin))
                {
                    resource = (ResourceDictionary)Application.LoadComponent(new Uri(uri, UriKind.Relative));
                }
                else
                {
                    using (FileStream fs = new FileStream(uri, FileMode.Open))
                    {
                        resource = (ResourceDictionary)System.Windows.Markup.XamlReader.Load(fs);
                    }
                }

                // first insert then remove !!!
                Application.Current.Resources.MergedDictionaries.Insert(0, resource);

                Application.Current.Resources.MergedDictionaries.RemoveAt(1);                

                

            }
            
        }
    }
}
