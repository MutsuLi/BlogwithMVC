using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace Blog.Common
{
    //
    // 摘要:
    //     HTTP请求
    public class HttpTo
    {
        //
        // 摘要:
        //     HttpWebRequest对象
        //
        // 参数:
        //   url:
        //     地址
        //
        //   type:
        //     请求类型，默认GET
        //
        //   data:
        //     发送数据，非GET、DELETE请求
        //
        //   charset:
        //     编码，默认utf-8
        public static HttpWebRequest HWRequest(string url, string type = "GET", string data = null, string charset = "utf-8")
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = type;
            httpWebRequest.KeepAlive = true;
            httpWebRequest.AllowAutoRedirect = true;
            httpWebRequest.MaximumAutomaticRedirections = 4;
            httpWebRequest.Timeout = 98301;
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            if (type != "GET" && type != "DELETE" && data != null)
            {
                byte[] bytes = Encoding.GetEncoding(charset).GetBytes(data);
                httpWebRequest.ContentLength = Encoding.GetEncoding(charset).GetBytes(data).Length;
                Stream requestStream = httpWebRequest.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }
            return httpWebRequest;
        }

        //
        // 摘要:
        //     HTTP请求
        //
        // 参数:
        //   request:
        //     HttpWebRequest对象
        //
        //   charset:
        //     编码，默认utf-8
        public static string Url(HttpWebRequest request, string charset = "utf-8")
        {
            HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();
            Stream stream = httpWebResponse.GetResponseStream();
            if (string.Compare(httpWebResponse.ContentEncoding, "gzip", ignoreCase: true) >= 0)
            {
                stream = new GZipStream(stream, CompressionMode.Decompress);
            }
            using (StreamReader streamReader = new StreamReader(stream, Encoding.GetEncoding(charset)))
            {
                return streamReader.ReadToEnd();
            }
        }

        //
        // 摘要:
        //     GET请求
        //
        // 参数:
        //   url:
        //     地址
        //
        //   charset:
        //     编码，默认utf-8
        public static string Get(string url, string charset = "utf-8")
        {
            HttpWebRequest request = HWRequest(url, "GET", null, charset);
            return Url(request, charset);
        }

        //
        // 摘要:
        //     POST请求
        //
        // 参数:
        //   url:
        //     地址
        //
        //   data:
        //     发送数据
        //
        //   charset:
        //     编码，默认utf-8
        public static string Post(string url, string data, string charset = "utf-8")
        {
            HttpWebRequest request = HWRequest(url, "POST", data, charset);
            return Url(request, charset);
        }
    }
}
