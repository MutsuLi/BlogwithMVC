using System;
using System.Collections.Generic;
using System.Text;
using Blog.Utils;

namespace Blog.Common
{
    //
    // 摘要:
    //     输出
    public class ConsoleTo
    {
        //
        // 摘要:
        //     写入错误信息
        //
        // 参数:
        //   ex:
        //
        //   isFull:
        //     是否全部信息，默认false
        public static void Log(Exception ex, bool isFull = false)
        {
            string msg = ExceptionGet(ex, isFull);
            Log(msg);
        }

        //
        // 摘要:
        //     写入消息
        //
        // 参数:
        //   msg:
        public static void Log(object msg)
        {
            string content;
            try
            {
                switch (msg.GetType().Name)
                {
                    case "Enum":
                    case "Byte":
                    case "Char":
                    case "String":
                    case "Boolean":
                    case "UInt16":
                    case "Int16":
                    case "Int32":
                    case "Int64":
                    case "Single":
                    case "Double":
                    case "Decimal":
                        content = msg.ToString();
                        break;
                    default:
                        content = msg.ToJson();
                        break;
                }
            }
            catch (Exception)
            {
                content = msg.ToString();
            }
            DateTime now = DateTime.Now;
            string path = AppDomain.CurrentDomain.BaseDirectory!.Replace("\\", "/").TrimEnd('/') + "/logs/" + now.ToString("yyyyMM") + "/";
            FileTo.WriteText(content, path, "console_" + now.ToString("yyyyMMdd") + ".log");
        }

        //
        // 摘要:
        //     获取异常信息
        //
        // 参数:
        //   ex:
        //
        //   isFull:
        //     是否包含堆栈所有信息，默认 false
        private static string ExceptionGet(Exception ex, bool isFull = false)
        {
            string newLine = Environment.NewLine;
            string text = ex.StackTrace;
            if (!isFull)
            {
                text = text.Replace(newLine, "^").Split('^')[0];
            }
            string text2 = string.Join(newLine, new List<string>
            {
                "====日志记录时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                "消息内容：" + ex.Message,
                "引发异常的方法：" + text + newLine
            });
            if (ex.InnerException != null)
            {
                text2 += ExceptionGet(ex.InnerException, isFull);
            }
            return text2;
        }
    }
}
