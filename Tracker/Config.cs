using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tracker.QiTools;

namespace Tracker
{
    public class Config
    {
        public QIConnectorConfig QiConfig { get; set; } 
        
        public bool ConfigReady 
        { 
            get 
            {
                return QiConfig.CheckConfig();
            } 
        }

        private static string trackerPath = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), "QiTracker");

        public Config()
        {
            QiConfig = new QIConnectorConfig();
        }

        public void SaveToJson()
        {
            string json = JsonSerializer.Serialize(this, typeof(Config));
            File.WriteAllText(Path.Combine(trackerPath, "tracker-config.json"), json);
        }

        public static Config LoadFromFile()
        {
            if (!File.Exists(Path.Combine(trackerPath, "tracker-config.json"))) return new Config();
            return JsonSerializer.Deserialize<Config>(File.ReadAllText(Path.Combine(trackerPath, "tracker-config.json")));
        }
    }
}
