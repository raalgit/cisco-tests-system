using ICSharpCode.SharpZipLib.Checksum;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KAF304TESTS.CiscoTestEditor
{
    public class ZipManager
    {
        public static string CreateZip(string stDirToZip)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(stDirToZip);
                string stZipPath = di.Parent.FullName + "\\" + di.Name + ".zip";

                CreateZip(stZipPath, stDirToZip);

                return stZipPath;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void CreateZip(string stZipPath, string stDirToZip)
        {
            try
            {
                stDirToZip = Path.GetFullPath(stDirToZip);
                stZipPath = Path.GetFullPath(stZipPath);

                Stack<FileInfo> stackFiles = DirExplore(stDirToZip);
                ZipOutputStream zipOutput = null;

                if (File.Exists(stZipPath))
                    File.Delete(stZipPath);

                Crc32 crc = new Crc32();
                zipOutput = new ZipOutputStream(File.Create(stZipPath));
                zipOutput.SetLevel(6);
                zipOutput.Password = "test";

                int index = 0;
                foreach (FileInfo fi in stackFiles)
                {
                    ++index;
                    
                    FileStream fs = File.OpenRead(fi.FullName);

                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);

                    string stFileName = fi.FullName.Remove(0, stDirToZip.Length + 1);
                    ZipEntry entry = new ZipEntry(stFileName);

                    entry.DateTime = DateTime.Now;

                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);

                    entry.Crc = crc.Value;
                    zipOutput.PutNextEntry(entry);
                    zipOutput.Write(buffer, 0, buffer.Length);
                }
                zipOutput.Finish();
                zipOutput.Close();
                zipOutput = null;
            }
            catch (Exception er)
            {
                throw;
            }
        }

        private static Stack<FileInfo> DirExplore(string stSrcDirPath)
        {
            try
            {
                Stack<DirectoryInfo> stackDirs = new Stack<DirectoryInfo>();
                Stack<FileInfo> stackPaths = new Stack<FileInfo>();

                DirectoryInfo dd = new DirectoryInfo(Path.GetFullPath(stSrcDirPath));

                stackDirs.Push(dd);
                while (stackDirs.Count > 0)
                {
                    DirectoryInfo currentDir = (DirectoryInfo)stackDirs.Pop();

                    try
                    {
                        foreach (FileInfo fileInfo in currentDir.GetFiles())
                        {
                            stackPaths.Push(fileInfo);
                        }

                        foreach (DirectoryInfo diNext in currentDir.GetDirectories())
                            stackDirs.Push(diNext);
                    }
                    catch (Exception)
                    {

                    }
                }
                return stackPaths;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void ZipDirectoryKeepRelativeSubfolder(string zipFilePath, string directoryToZip)
        {
            var filenames = Directory.GetFiles(directoryToZip, "*.*", SearchOption.AllDirectories);
            using (var s = new ZipOutputStream(File.Create(zipFilePath)))
            {
                s.SetLevel(9);

                var buffer = new byte[4096];

                foreach (var file in filenames)
                {
                    var relativePath = file.Substring(directoryToZip.Length).TrimStart('\\');
                    var entry = new ZipEntry(relativePath);
                    entry.DateTime = DateTime.Now;
                    s.PutNextEntry(entry);

                    using (var fs = File.OpenRead(file))
                    {
                        int sourceBytes;
                        do
                        {
                            sourceBytes = fs.Read(buffer, 0, buffer.Length);
                            s.Write(buffer, 0, sourceBytes);
                        } while (sourceBytes > 0);
                    }
                }
                s.Finish();
                s.Close();
            }
        }

        private static char[] _stSchon = new char[] { '-', '\\', '|', '/' };
    }
}
