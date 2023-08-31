using Sandbox.Game.World;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using VRage.FileSystem;
using VRage.Game.ModAPI;
using VRage.Utils;

namespace SE_ResizableChat
{
    public class Config
    {
        [XmlIgnore]
        public static Config Static { get; set; }
        [XmlIgnore]
        public static Config Default { get; } = new Config
        {
            TextScale = ResizableChatConfigScreen.DEFAULT_TEXTSCALE,
            ChatWidth = ResizableChatConfigScreen.DEFAULT_CHATWIDTH,
            ChatHeight = ResizableChatConfigScreen.DEFAULT_CHATHEIGHT,
        };

        public float TextScale { get; set; }
        public float ChatWidth { get; set; }
        public float ChatHeight { get; set; }

        [XmlIgnore]
        private const string configName = "ResizableChatPlugin.cfg";

        public static bool Load()
        {
            bool loaded = false;
            string configPath = Path.Combine(MyFileSystem.UserDataPath, "Storage", configName);
            if (File.Exists(configPath))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(configPath))
                    {
                        Static = SerializeFromXML<Config>(sr.ReadToEnd());
                        sr.Close();
                    }

                    loaded = true;
                }
                catch (Exception e)
                {
                    MyLog.Default.WriteLine("ResizableChat threw an exception while loading config." + e);
                    Static = Default;
                }
            }
            else
            {
                Static = Default;
            }

            Save();

            return loaded;
        }

        public static void Save()
        {
            string configPath = Path.Combine(MyFileSystem.UserDataPath, "Storage", configName);
            try
            {
                using (StreamWriter sw = new StreamWriter(configPath))
                {
                    sw.Write(SerializeToXML(Static));
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                MyLog.Default.WriteLine("ResizableChat config could not be saved." + e);
            }
        }

        public static string SerializeToXML<T>(T objToSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(objToSerialize.GetType());
            StringWriter stringWriter1 = new StringWriter();
            StringWriter stringWriter2 = stringWriter1;
            xmlSerializer.Serialize((TextWriter)stringWriter2, objToSerialize);
            return stringWriter1.ToString();
        }

        public static T SerializeFromXML<T>(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return default(T);
            using (StringReader input = new StringReader(xml))
            {
                using (XmlReader xmlReader = XmlReader.Create((TextReader)input))
                    return (T)new XmlSerializer(typeof(T)).Deserialize(xmlReader);
            }
        }
    }
}
