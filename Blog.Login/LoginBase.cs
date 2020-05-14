using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using Blog.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Blog.Login
{
    //
    // 摘要:
    //     基础支持
    public class LoginBase
    {
        //
        // 摘要:
        //     登录类型枚举
        public enum LoginType
        {
            //
            // 摘要:
            //     腾讯QQ
            QQ,
            //
            // 摘要:
            //     新浪微博
            WeiBo,
            //
            // 摘要:
            //     腾讯微信
            WeChat,
            //
            // 摘要:
            //     GitHub
            GitHub,
            //
            // 摘要:
            //     Gitee
            Gitee,
            //
            // 摘要:
            //     淘宝（天猫）
            TaoBao,
            //
            // 摘要:
            //     微软
            MicroSoft,
            //
            // 摘要:
            //     钉钉
            DingTalk,
            //
            // 摘要:
            //     谷歌
            Google,
            //
            // 摘要:
            //     支付宝
            AliPay,
            //
            // 摘要:
            //     Stack Overflow
            StackOverflow
        }

        //
        // 摘要:
        //     接收授权码、防伪标识
        public class AuthorizeResult
        {
            //
            // 摘要:
            //     授权码
            public string code
            {
                get;
                set;
            }

            //
            // 摘要:
            //     授权码，AliPay支付宝
            public string auth_code
            {
                get;
                set;
            }

            //
            // 摘要:
            //     防伪参数，如果传递参数，会回传该参数。
            public string state
            {
                get;
                set;
            }
        }

        //
        // 摘要:
        //     实体 转 Pars
        //
        // 参数:
        //   entity:
        //
        // 类型参数:
        //   T:
        public static string EntityToPars<T>(T entity)
        {
            string text = string.Empty;
            PropertyInfo[] properties = entity.GetType().GetProperties();
            PropertyInfo[] array = properties;
            foreach (PropertyInfo propertyInfo in array)
            {
                string text2 = propertyInfo.GetValue(entity, null)?.ToString();
                if (text2 != null)
                {
                    text = text + "&" + propertyInfo.Name + "=" + text2.ToEncode();
                }
            }
            return text.TrimStart('&');
        }

        //
        // 摘要:
        //     处理结果
        //
        // 参数:
        //   result:
        //     请求的结果
        //
        //   resultNeedJObject:
        //     处理的类型，默认JObject
        //
        // 类型参数:
        //   T:
        public static T ResultOutput<T>(string result, List<string> resultNeedJObject = null) where T : class, new()
        {
            T val = new T();
            PropertyInfo[] properties = val.GetType().GetProperties();
            JObject jObject = JObject.Parse(result);
            PropertyInfo[] array = properties;
            foreach (PropertyInfo propertyInfo in array)
            {
                object value;
                try
                {
                    value = Convert.ChangeType(conversionType: (!propertyInfo.PropertyType.FullName!.Contains("System.Nullable")) ? propertyInfo.PropertyType : Type.GetType("System." + propertyInfo.PropertyType.FullName!.Split(',')[0].Split('.')[2]), value: jObject[propertyInfo.Name]!.ToString());
                }
                catch (Exception)
                {
                    value = null;
                }
                if (resultNeedJObject != null && resultNeedJObject.Count > 0)
                {
                    try
                    {
                        if (resultNeedJObject.Contains(propertyInfo.Name))
                        {
                            value = JObject.Parse(jObject[propertyInfo.Name]!.ToString());
                        }
                    }
                    catch (Exception)
                    {
                        value = null;
                    }
                }
                propertyInfo.SetValue(val, value, null);
            }
            return val;
        }

        //
        // 摘要:
        //     验证对象是否有效
        //
        // 参数:
        //   entity:
        //
        // 类型参数:
        //   T:
        public static bool IsValid<T>(T entity) where T : new()
        {
            bool result = true;
            string name = typeof(Required).Name;
            PropertyInfo[] properties = entity.GetType().GetProperties();
            PropertyInfo[] array = properties;
            foreach (PropertyInfo propertyInfo in array)
            {
                bool flag = false;
                object[] customAttributes = propertyInfo.GetCustomAttributes(inherit: true);
                object[] array2 = customAttributes;
                foreach (object obj in array2)
                {
                    Type type = obj.GetType();
                    if (type.Name == name)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    object value = propertyInfo.GetValue(entity, null);
                    if (value == null || value.ToString() == "")
                    {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }
    }
}
