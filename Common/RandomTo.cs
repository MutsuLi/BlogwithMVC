using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Common
{
    //
    // 摘要:
    //     生成随机字符
    public class RandomTo
    {
        //
        // 摘要:
        //     随机字符 验证码
        //
        // 参数:
        //   strLen:
        //     长度 默认4个字符
        //
        //   source:
        //     自定义随机的字符源
        public static string StrCode(int strLen = 4, string source = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
        {
            string text = string.Empty;
            if (strLen > 0)
            {
                Random random = new Random(Guid.NewGuid().GetHashCode());
                for (int i = 0; i < strLen; i++)
                {
                    text += source[random.Next(source.Length)].ToString();
                }
            }
            return text;
        }

        //
        // 摘要:
        //     随机字符 纯数字
        //
        // 参数:
        //   strLen:
        //     长度 默认4
        //
        //   source:
        //     生成的源字符串 默认0-9
        public static string NumCode(int strLen = 4, string source = "0123456789")
        {
            return StrCode(strLen, source);
        }
    }
}
