using System;
using System.Collections.Generic;
using System.IO;

namespace WB.ConfigUpdate
{
    public static partial class FileHelper
    {

        #region 复制文件夹
        /// <summary>
        /// 将一个文件夹下的所有东西复制到另一个文件夹
        /// </summary>
        public static void CopyDireToDire(string sourceDir, string destDir)
        {
            DirectoryInfo sourceDireInfo = new DirectoryInfo(sourceDir);
            List<FileInfo> fileList = new List<FileInfo>();
            GetFileList(sourceDireInfo, fileList);
            List<DirectoryInfo> dirList = new List<DirectoryInfo>();
            GetDirList(sourceDireInfo, dirList);

            foreach (DirectoryInfo dir in dirList)
            {
                string m = dir.FullName;
                string n = m.Replace(sourceDir, destDir);
                if (!Directory.Exists(n))
                {
                    Directory.CreateDirectory(n);
                }
            }
            foreach (FileInfo fileInfo in fileList)
            {
                string m = fileInfo.FullName;
                string n = m.Replace(sourceDir, destDir);
                System.IO.File.Copy(m, n, true);
            }
        }
        private static void GetFileList(DirectoryInfo dir, List<FileInfo> fileList)
        {
            fileList.AddRange(dir.GetFiles());
            foreach (DirectoryInfo directory in dir.GetDirectories()) GetFileList(directory, fileList);
        }
        private static void GetDirList(DirectoryInfo dir, List<DirectoryInfo> dirList)
        {
            dirList.AddRange(dir.GetDirectories());
            foreach (DirectoryInfo directory in dir.GetDirectories()) GetDirList(directory, dirList);
        }

        #endregion
    }
    public static partial class FileHelper
    {
        public static List<string> GetAllFiles(string dir, string searchPattern = "*")
        {
            List<string> list = new List<string>();
            GetAllFiles(list, dir, searchPattern);
            return list;
        }

        public static void GetAllFiles(List<string> files, string dir, string searchPattern = "*")
        {
            string[] fls = Directory.GetFiles(dir);
            foreach (string fl in fls)
            {
                files.Add(fl);
            }

            string[] subDirs = Directory.GetDirectories(dir);
            foreach (string subDir in subDirs)
            {
                GetAllFiles(files, subDir, searchPattern);
            }
        }

        public static void CleanDirectory(string dir)
        {
            if (!Directory.Exists(dir))
            {
                return;
            }
            foreach (string subdir in Directory.GetDirectories(dir))
            {
                Directory.Delete(subdir, true);
            }

            foreach (string subFile in Directory.GetFiles(dir))
            {
                File.Delete(subFile);
            }
        }

        public static void CopyDirectory(string srcDir, string tgtDir)
        {
            DirectoryInfo source = new DirectoryInfo(srcDir);
            DirectoryInfo target = new DirectoryInfo(tgtDir);

            if (target.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new Exception("父目录不能拷贝到子目录！");
            }

            if (!source.Exists)
            {
                return;
            }

            if (!target.Exists)
            {
                target.Create();
            }

            FileInfo[] files = source.GetFiles();

            for (int i = 0; i < files.Length; i++)
            {
                File.Copy(files[i].FullName, Path.Combine(target.FullName, files[i].Name), true);
            }

            DirectoryInfo[] dirs = source.GetDirectories();

            for (int j = 0; j < dirs.Length; j++)
            {
                CopyDirectory(dirs[j].FullName, Path.Combine(target.FullName, dirs[j].Name));
            }
        }

        public static void ReplaceExtensionName(string srcDir, string extensionName, string newExtensionName)
        {
            if (Directory.Exists(srcDir))
            {
                string[] fls = Directory.GetFiles(srcDir);

                foreach (string fl in fls)
                {
                    if (fl.EndsWith(extensionName))
                    {
                        File.Move(fl, fl.Substring(0, fl.IndexOf(extensionName)) + newExtensionName);
                        File.Delete(fl);
                    }
                }

                string[] subDirs = Directory.GetDirectories(srcDir);

                foreach (string subDir in subDirs)
                {
                    ReplaceExtensionName(subDir, extensionName, newExtensionName);
                }
            }
        }
    }
}
