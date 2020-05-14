using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Common
{
    //
    // 摘要:
    //     生成唯一标识
    public class UniqueTo
    {
        //
        // 摘要:
        //     根据Guid获取唯一数字序列，19位
        public static long LongId()
        {
            byte[] value = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(value, 0);
        }
    }
}
