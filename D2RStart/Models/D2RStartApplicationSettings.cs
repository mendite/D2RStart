using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace D2RStart.Models
{
    internal class D2RStartApplicationSettings
    {
        #region Private fields        
        static readonly string ConfigurationFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "D2RStart\\D2RStartApplicationSettings.config");
        static readonly string OldConfigurationFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "D2RStart\\D2RStart.config");
        #endregion

        public List<D2RItem> Items { get; set; } = new List<D2RItem>();
        public WindowClosingSize WindowClosingSize { get; set; } = new WindowClosingSize();

        public void Save()
        {
            byte[] saveFileContent = JsonSerializer.SerializeToUtf8Bytes(this);
            if (!Directory.Exists(Path.GetDirectoryName(ConfigurationFile)))
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigurationFile));

            File.WriteAllBytes(ConfigurationFile, saveFileContent);
        }

        public static D2RStartApplicationSettings Load()
        {
            try
            {
                if (File.Exists(ConfigurationFile)) 
                {
                    if (JsonSerializer.Deserialize<D2RStartApplicationSettings>(File.ReadAllText(ConfigurationFile)) is D2RStartApplicationSettings settings)
                    {
                        return settings;
                    }
                }
                else if (File.Exists(OldConfigurationFile))
                {
                    if (JsonSerializer.Deserialize<List<D2RItem>>(File.ReadAllText(OldConfigurationFile)) is List<D2RItem> items)
                    {
                        D2RStartApplicationSettings settings = new D2RStartApplicationSettings() 
                        {
                            Items = items,
                        };

                        settings.Save();

                        File.Delete(OldConfigurationFile);

                        return settings;
                    }
                }
            }
            catch 
            {                
            }

            return new D2RStartApplicationSettings();
        }
    }
}
