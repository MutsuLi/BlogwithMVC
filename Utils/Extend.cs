using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Blog.Utils
{
        //
        // 摘要:
        //     常用方法拓展
        public static class Extend
        {
            //
            // 摘要:
            //     object 转 JSON 字符串
            //
            // 参数:
            //   obj:
            //
            //   DateTimeFormat:
            //     时间格式化
            public static string ToJson(this object obj, string DateTimeFormat = "yyyy-MM-dd HH:mm:ss")
            {
                IsoDateTimeConverter isoDateTimeConverter = new IsoDateTimeConverter
                {
                    DateTimeFormat = DateTimeFormat
                };
                return JsonConvert.SerializeObject(obj, isoDateTimeConverter);
            }

            //
            // 摘要:
            //     解析 JSON字符串 为JObject对象
            //
            // 参数:
            //   json:
            //     JSON字符串
            //
            // 返回结果:
            //     JObject对象
            public static JObject ToJObject(this string json)
            {
                return JObject.Parse(json);
            }

            //
            // 摘要:
            //     解析 JSON字符串 为JArray对象
            //
            // 参数:
            //   json:
            //     JSON字符串
            //
            // 返回结果:
            //     JArray对象
            public static JArray ToJArray(this string json)
            {
                return JArray.Parse(json);
            }

            //
            // 摘要:
            //     JSON字符串 转 实体
            //
            // 参数:
            //   json:
            //     JSON字符串
            //
            // 类型参数:
            //   T:
            //     实体泛型
            public static T ToEntity<T>(this string json)
            {
                return JsonConvert.DeserializeObject<T>(json);
            }

            //
            // 摘要:
            //     JSON字符串 转 实体
            //
            // 参数:
            //   json:
            //     JSON字符串
            //
            // 类型参数:
            //   T:
            //     实体泛型
            public static List<T> ToEntitys<T>(this string json)
            {
                return JsonConvert.DeserializeObject<List<T>>(json);
            }

            //
            // 摘要:
            //     把jArray里面的json对象转为字符串
            //
            // 参数:
            //   jt:
            //     JToken对象
            public static string ToStringOrEmpty(this JToken jt)
            {
                try
                {
                    return (jt == null) ? "" : jt.ToString();
                }
                catch (Exception)
                {
                    return "";
                }
            }

            //
            // 摘要:
            //     字符串 JSON转义
            //
            // 参数:
            //   s:
            //     字符串
            public static string OfJson(this string s)
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < s.Length; i++)
                {
                    char c = s.ToCharArray()[i];
                    switch (c)
                    {
                        case '"':
                            stringBuilder.Append("\\\"");
                            break;
                        case '\\':
                            stringBuilder.Append("\\\\");
                            break;
                        case '/':
                            stringBuilder.Append("\\/");
                            break;
                        case '\b':
                            stringBuilder.Append("\\b");
                            break;
                        case '\f':
                            stringBuilder.Append("\\f");
                            break;
                        case '\n':
                            stringBuilder.Append("\\n");
                            break;
                        case '\r':
                            stringBuilder.Append("\\r");
                            break;
                        case '\t':
                            stringBuilder.Append("\\t");
                            break;
                        default:
                            stringBuilder.Append(c);
                            break;
                    }
                }
                return stringBuilder.ToString();
            }

            //
            // 摘要:
            //     SQL单引号转义
            //
            // 参数:
            //   s:
            //     字符串
            public static string OfSql(this string s)
            {
                return s.Replace("'", "''");
            }

            //
            // 摘要:
            //     实体转表
            //
            // 参数:
            //   list:
            //     对象
            //
            // 类型参数:
            //   T:
            //     泛型
            public static DataTable ToDataTable<T>(this IList<T> list)
            {
                Type typeFromHandle = typeof(T);
                DataTable t = new DataTable();
                typeFromHandle.GetProperties().ToList().ForEach((Action<PropertyInfo>)delegate (PropertyInfo propInfo)
                {
                    t.Columns.Add(propInfo.Name, Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType);
                });
                foreach (T item in list)
                {
                    DataRow row = t.NewRow();
                    typeFromHandle.GetProperties().ToList().ForEach((Action<PropertyInfo>)delegate (PropertyInfo propInfo)
                    {
                        row[propInfo.Name] = (propInfo.GetValue(item, null) ?? DBNull.Value);
                    });
                    t.Rows.Add(row);
                }
                return t;
            }

            //
            // 摘要:
            //     表转为实体
            //
            // 参数:
            //   table:
            //     表
            //
            // 类型参数:
            //   T:
            //     泛型
            public static List<T> ToModel<T>(this DataTable table) where T : class, new()
            {
                List<T> list = new List<T>();
                foreach (DataRow row in table.Rows)
                {
                    T val = new T();
                    foreach (DataColumn dc in row.Table.Columns)
                    {
                        object obj = row[dc.ColumnName];
                        PropertyInfo propertyInfo = ((IEnumerable<PropertyInfo>)val.GetType().GetProperties()).Where((Func<PropertyInfo, bool>)((PropertyInfo x) => x.Name.ToLower() == dc.ColumnName.ToLower())).FirstOrDefault();
                        Type conversionType = propertyInfo.PropertyType;
                        if (propertyInfo.PropertyType.FullName!.Contains("System.Nullable"))
                        {
                            conversionType = Type.GetType("System." + propertyInfo.PropertyType.FullName!.Split(',')[0].Split('.')[2]);
                        }
                        if (propertyInfo != null && propertyInfo.CanWrite && obj != null && !Convert.IsDBNull(obj))
                        {
                            try
                            {
                                obj = Convert.ChangeType(obj, conversionType);
                                propertyInfo.SetValue(val, obj, null);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    list.Add(val);
                }
                return list;
            }

            //
            // 摘要:
            //     编码
            //
            // 参数:
            //   uri:
            //     内容
            //
            //   charset:
            //     编码格式
            public static string ToEncode(this string uri, string charset = "utf-8")
            {
                string text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
                if (string.IsNullOrEmpty(uri))
                {
                    return string.Empty;
                }
                StringBuilder stringBuilder = new StringBuilder(uri.Length * 2);
                byte[] bytes = Encoding.GetEncoding(charset).GetBytes(uri);
                byte[] array = bytes;
                foreach (byte b in array)
                {
                    char value = (char)b;
                    if (text.IndexOf(value) != -1)
                    {
                        stringBuilder.Append(value);
                    }
                    else
                    {
                        stringBuilder.Append("%").Append(string.Format(CultureInfo.InstalledUICulture, "{0:X2}", (int)b));
                    }
                }
                return stringBuilder.ToString();
            }

            //
            // 摘要:
            //     解码
            //
            // 参数:
            //   uriToDecode:
            //     内容
            public static string ToDecode(this string uriToDecode)
            {
                if (!string.IsNullOrEmpty(uriToDecode))
                {
                    uriToDecode = uriToDecode.Replace("+", " ");
                    return Uri.UnescapeDataString(uriToDecode);
                }
                return string.Empty;
            }

            //
            // 摘要:
            //     将Datetime转换成时间戳，10位：秒 或 13位：毫秒
            //
            // 参数:
            //   datetime:
            //
            //   isms:
            //     毫秒，默认false为秒，设为true，返回13位，毫秒
            public static long ToTimestamp(this DateTime datetime, bool isms = false)
            {
                long num = datetime.ToUniversalTime().Ticks - 621355968000000000L;
                return num / (isms ? 10000 : 10000000);
            }
        }
    }
