
using System.Xml.Serialization;

namespace ProfileSwitcher
{

    public static class XmlFiles
    {
        /// <summary>
        /// Show last XML file read. For debug purposes.
        /// </summary>
        public static string LastReadFile { get; private set; }

        /// <summary>
        /// Save XML file.
        /// </summary>
        /// <param name="obj">Object to save.</param>
        /// <param name="filename">Output file name.</param>
        public static void SaveXml(object obj, string filename)
        {
            using (var writer = new StreamWriter(filename))
            {
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(writer, obj);
                writer.Flush();
            }
        }

        /// <summary>
        /// Read an XML file.
        /// </summary>
        /// <param name="filename">Input file name.</param>
        public static T LoadXml<T>(string filename)
        {
            LastReadFile = filename;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (var reader = File.OpenText(filename))
            {
                XmlDeserializationEvents eventsHandler = new XmlDeserializationEvents()
                {
                    OnUnknownAttribute = (object sender, XmlAttributeEventArgs e) => { throw new System.Exception("Error parsing file '" + filename + "': invalid attribute '" + e.Attr.Name + "' at line " + e.LineNumber); },
                    OnUnknownElement = (object sender, XmlElementEventArgs e) => { throw new System.Exception("Error parsing file '" + filename + "': invalid element '" + e.Element.Name + "' at line " + e.LineNumber); },
                    OnUnknownNode = (object sender, XmlNodeEventArgs e) => { throw new System.Exception("Error parsing file '" + filename + "': invalid element '" + e.Name + "' at line " + e.LineNumber); },
                    OnUnreferencedObject = (object sender, UnreferencedObjectEventArgs e) => { throw new System.Exception("Error parsing file '" + filename + "': unreferenced object '" + e.UnreferencedObject.ToString() + "'"); },
                };
                return (T)serializer.Deserialize(System.Xml.XmlReader.Create(reader), eventsHandler);
            }
        }

        /// <summary>
        /// Get an array with xml file names for a given folder.
        /// Return just file name, without the extension or path.
        /// </summary>
        public static string[] GetXmlFileNames(string folder)
        {
            var files = Directory.GetFiles(folder, "*.xml", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; ++i)
            {
                files[i] = Path.GetFileNameWithoutExtension(files[i]);
            }
            return files;
        }
    }



    public static class FileManager
    {
        public static string CurrentPath = Directory.GetCurrentDirectory();
        public static string AppPath = Directory.GetCurrentDirectory() + @"\app\";
        public static bool Init()
        {
            List<bool> results = new List<bool>();
            results.Add(fmDir.exist(""));
            results.Add(fmDir.exist(@"\contenu"));
            results.Add(fmFile.exist(@"addtext.json"));
            results.Add(fmFile.exist(@"replacetext.json"));
            return results.Find(x => x == false);
        }

        private static bool exist(int type, string _path, bool create = true)
        {
            string path = AppPath + _path;
            bool dirExists = Directory.Exists(path);
            bool fileExists = File.Exists(path);
            if (!dirExists && !fileExists && create)
            {
                if (type == 0)
                {
                    if (path[path.Length - 1] == '\\')
                    {
                        path = path.Substring(0, path.Length - 1) + "";

                    }
                    Directory.CreateDirectory(path);

                }
                else
                {
                    File.Create(path);
                    

                }
            }
            return dirExists || fileExists;
        }
        public static class fmFile
        {
            public static bool exist(string _path, bool create = true)
            {
                return FileManager.exist(1, _path, create);
            }
        }
        public static class fmDir
        {
            public static bool exist(string _path, bool create = true)
            {
                return FileManager.exist(0, _path, create);
            }
        }

    }

}
