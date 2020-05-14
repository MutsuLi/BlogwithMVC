using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Blog.Common
{
    //
    // 摘要:
    //     算法、加密、解密
    public class CalcTo
    {
        //
        // 摘要:
        //     异或算法
        //
        // 参数:
        //   s:
        //     字符串
        //
        //   key:
        //     异或因子 2-253
        //
        // 返回结果:
        //     返回异或后的字符串
        public static string XorKey(string s, int key)
        {
            byte b = byte.Parse(((key > 253) ? 253 : ((key < 2) ? 2 : key)).ToString());
            byte[] bytes = Encoding.Unicode.GetBytes(s);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(bytes[i] ^ b ^ (b + 7));
            }
            return Encoding.Unicode.GetString(bytes);
        }

        //
        // 摘要:
        //     MD5加密 小写
        //
        // 参数:
        //   s:
        //     需加密的字符串
        //
        //   len:
        //     长度 默认32 可选16
        public static string MD5(string s, int len = 32)
        {
            using (MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider())
            {
                byte[] array = mD5CryptoServiceProvider.ComputeHash(Encoding.Default.GetBytes(s));
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < array.Length; i++)
                {
                    stringBuilder.Append(array[i].ToString("x2"));
                }
                string text = stringBuilder.ToString();
                return (len == 32) ? text : text.Substring(8, 16);
            }
        }

        //
        // 摘要:
        //     DES 加密
        //
        // 参数:
        //   Text:
        //     内容
        //
        //   sKey:
        //     密钥
        public static string EnDES(string Text, string sKey)
        {
            DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
            byte[] bytes = Encoding.Default.GetBytes(Text);
            dESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(MD5(sKey).Substring(0, 8));
            dESCryptoServiceProvider.IV = Encoding.ASCII.GetBytes(MD5(sKey).Substring(0, 8));
            MemoryStream memoryStream = new MemoryStream();
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.Write(bytes, 0, bytes.Length);
                cryptoStream.FlushFinalBlock();
                StringBuilder stringBuilder = new StringBuilder();
                byte[] array = memoryStream.ToArray();
                foreach (byte b in array)
                {
                    stringBuilder.AppendFormat("{0:X2}", b);
                }
                return stringBuilder.ToString();
            }
        }

        //
        // 摘要:
        //     DES 解密
        //
        // 参数:
        //   Text:
        //     内容
        //
        //   sKey:
        //     密钥
        public static string DeDES(string Text, string sKey)
        {
            DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
            int num = Text.Length / 2;
            byte[] array = new byte[num];
            for (int i = 0; i < num; i++)
            {
                int num2 = Convert.ToInt32(Text.Substring(i * 2, 2), 16);
                array[i] = (byte)num2;
            }
            dESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(MD5(sKey).Substring(0, 8));
            dESCryptoServiceProvider.IV = Encoding.ASCII.GetBytes(MD5(sKey).Substring(0, 8));
            MemoryStream memoryStream = new MemoryStream();
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.Write(array, 0, array.Length);
                cryptoStream.FlushFinalBlock();
                return Encoding.Default.GetString(memoryStream.ToArray());
            }
        }

        //
        // 摘要:
        //     20字节,160位
        //
        // 参数:
        //   str:
        //     内容
        public static string SHA128(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            using (SHA1CryptoServiceProvider sHA1CryptoServiceProvider = new SHA1CryptoServiceProvider())
            {
                byte[] value = sHA1CryptoServiceProvider.ComputeHash(bytes);
                return BitConverter.ToString(value);
            }
        }

        //
        // 摘要:
        //     32字节,256位
        //
        // 参数:
        //   str:
        //     内容
        public static string SHA256(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            using (SHA256CryptoServiceProvider sHA256CryptoServiceProvider = new SHA256CryptoServiceProvider())
            {
                byte[] value = sHA256CryptoServiceProvider.ComputeHash(bytes);
                return BitConverter.ToString(value);
            }
        }

        //
        // 摘要:
        //     48字节,384位
        //
        // 参数:
        //   str:
        //     内容
        public static string SHA384(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            using (SHA384CryptoServiceProvider sHA384CryptoServiceProvider = new SHA384CryptoServiceProvider())
            {
                byte[] value = sHA384CryptoServiceProvider.ComputeHash(bytes);
                return BitConverter.ToString(value);
            }
        }

        //
        // 摘要:
        //     64字节,512位
        //
        // 参数:
        //   str:
        //     内容
        public static string SHA512(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            using (SHA512CryptoServiceProvider sHA512CryptoServiceProvider = new SHA512CryptoServiceProvider())
            {
                byte[] value = sHA512CryptoServiceProvider.ComputeHash(bytes);
                return BitConverter.ToString(value);
            }
        }

        //
        // 摘要:
        //     HMAC_SHA1 加密
        //
        // 参数:
        //   str:
        //     内容
        //
        //   key:
        //     密钥
        public static string HMAC_SHA1(string str, string key)
        {
            using (HMACSHA1 hMACSHA = new HMACSHA1
            {
                Key = Encoding.UTF8.GetBytes(key)
            })
            {
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                byte[] inArray = hMACSHA.ComputeHash(bytes);
                return Convert.ToBase64String(inArray);
            }
        }

        //
        // 摘要:
        //     HMAC_SHA256 加密
        //
        // 参数:
        //   str:
        //     内容
        //
        //   key:
        //     密钥
        public static string HMAC_SHA256(string str, string key)
        {
            using (HMACSHA256 hMACSHA = new HMACSHA256
            {
                Key = Encoding.UTF8.GetBytes(key)
            })
            {
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                byte[] inArray = hMACSHA.ComputeHash(bytes);
                return Convert.ToBase64String(inArray);
            }
        }

        //
        // 摘要:
        //     HMACSHA384 加密
        //
        // 参数:
        //   str:
        //     内容
        //
        //   key:
        //     密钥
        public static string HMACSHA384(string str, string key)
        {
            using (HMACSHA384 hMACSHA = new HMACSHA384
            {
                Key = Encoding.UTF8.GetBytes(key)
            })
            {
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                byte[] inArray = hMACSHA.ComputeHash(bytes);
                return Convert.ToBase64String(inArray);
            }
        }

        //
        // 摘要:
        //     HMACSHA512 加密
        //
        // 参数:
        //   str:
        //     内容
        //
        //   key:
        //     密钥
        public static string HMACSHA512(string str, string key)
        {
            using (HMACSHA512 hMACSHA = new HMACSHA512
            {
                Key = Encoding.UTF8.GetBytes(key)
            })
            {
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                byte[] inArray = hMACSHA.ComputeHash(bytes);
                return Convert.ToBase64String(inArray);
            }
        }
    }
}
