using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AppTestTool.Utils
{
    public class DirectionIO
    {
        private static string _path = "";
        public static string GetPath()
        {
            if (_path.Length == 0)
            {
                var systemPArth = System.Reflection.Assembly.GetExecutingAssembly().Location;
                _path = systemPArth.Remove(systemPArth.LastIndexOf("\\", System.StringComparison.Ordinal));
            }
            return _path.Clone() as string;
        }

        public static bool IsExistNewFolder(string folderName)
        {
            if (Directory.Exists(folderName))
                return true;
            return false;
        }

        public static string CreateNewFolder(string folderName)
        {

            if (!IsExistNewFolder(folderName))
                Directory.CreateDirectory(folderName);

            return folderName;
        }

        public static bool IsExistFile(string filePath)
        {
            if (File.Exists(filePath))
                return true;
            return false;
        }
        
        public static void WriteAllText(string filePath, string value)
        {
            File.WriteAllText(filePath, value);
        }
        public static string ReadAllText(string filePath)
        {
           return File.ReadAllText(filePath);
        }

        public static void RemoveFile(string s)
        {
            if (File.Exists(s))
            {
                File.Delete(s);
            }
        }
    }
}
