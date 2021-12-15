public static class FileManager
{
    public static string CurrentPath = Directory.GetCurrentDirectory();
    public static string AppPath = Directory.GetCurrentDirectory() + @"\app\";
    public static bool Init()
    {
        List<bool> results = new List<bool>
        {
            fmDir.exist(""),
            fmDir.exist("Settings")
        };
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
                File.Create(path).Close();


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

