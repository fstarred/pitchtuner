using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WpfPitchTuner.Provider
{
    public class SkinProvider
    {
        private SkinProvider()
        {
        }

        private static SkinProvider instance;

        public static SkinProvider Instance
        {
            get 
            { 
                return instance ?? (instance = new SkinProvider()); 
            }
        }

        static IList<string> skins;

        public static IList<string> GetSkins()
        {
            if (Helper.Helper.IsInDesignMode == false)
            {
                string[] fileEntries = Directory.GetFiles("Skins", "*.xaml", SearchOption.TopDirectoryOnly);

                skins = new List<string>();

                skins.Add("Default");

                foreach (string file in fileEntries)
                {
                    using (FileStream fs = new FileStream(file, FileMode.Open))
                    {
                        object xamlinput = System.Windows.Markup.XamlReader.Load(fs);

                        if (xamlinput is System.Windows.ResourceDictionary)
                        {
                            string filename = System.IO.Path.GetFileName(file);
                            string name = System.IO.Path.GetFileNameWithoutExtension(file);

                            skins.Add(name);
                        }
                    }                

                }
            }            

            return skins;
        }
    }

}
