using System;
using System.IO;
using System.Text;

namespace Blog.Common
{
    //
    // 摘要:
    //     文件读写
    public class FileTo
    {
        //
        // 摘要:
        //     流写入
        //
        // 参数:
        //   content:
        //     内容
        //
        //   path:
        //     物理目录
        //
        //   fileName:
        //     文件名
        //
        //   e:
        //     编码
        //
        //   isAppend:
        //     默认追加，false覆盖
        public static void WriteText(string content, string path, string fileName, Encoding e, bool isAppend = true)
        {
            FileStream fileStream;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                fileStream = new FileStream(path + fileName, FileMode.Create);
            }
            else if (!File.Exists(path + fileName))
            {
                fileStream = new FileStream(path + fileName, FileMode.Create);
            }
            else
            {
                FileMode mode = isAppend ? FileMode.Append : FileMode.Truncate;
                fileStream = new FileStream(path + fileName, mode);
            }
            StreamWriter streamWriter = new StreamWriter(fileStream, e);
            streamWriter.WriteLine(content);
            streamWriter.Close();
            fileStream.Close();
        }

        //
        // 摘要:
        //     写入
        //
        // 参数:
        //   content:
        //
        //   path:
        //
        //   fileName:
        //
        //   isAppend:
        public static void WriteText(string content, string path, string fileName, bool isAppend = true)
        {
            WriteText(content, path, fileName, Encoding.UTF8, isAppend);
        }

        //
        // 摘要:
        //     读取
        //
        // 参数:
        //   path:
        //     物理目录
        //
        //   fileName:
        //     文件名
        //
        //   e:
        //     编码 默认UTF8
        public static string ReadText(string path, string fileName, Encoding e = null)
        {
            string result = string.Empty;
            try
            {
                if (e == null)
                {
                    e = Encoding.UTF8;
                }
                using (StreamReader streamReader = new StreamReader(path + fileName, Encoding.Default))
                {
                    result = streamReader.ReadToEnd();
                }
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}