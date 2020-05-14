using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Blog.Utils;

namespace Blog.Common
{
    //
    // 摘要:
    //     Tree常用方法
    public class TreeTo
    {
        //
        // 摘要:
        //     数据集合转JSON
        //
        // 参数:
        //   list:
        //
        //   pidField:
        //     父ID键
        //
        //   idField:
        //     ID键
        //
        //   startPid:
        //     开始的PID
        //
        //   childrenNodeName:
        //     子节点名称，默认children
        //
        // 类型参数:
        //   T:
        public static string ListToTree<T>(List<T> list, string pidField, string idField, List<string> startPid, string childrenNodeName = "children")
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<T> list2 = ((IEnumerable<T>)list).Where((Func<T, bool>)((T x) => startPid.Contains(x.GetType().GetProperty(pidField)!.GetValue(x, null)!.ToString()))).ToList();
            for (int i = 0; i < list2.Count; i++)
            {
                if (i == 0)
                {
                    stringBuilder.Append("[");
                }
                else
                {
                    stringBuilder.Append(",");
                }
                stringBuilder.Append("{");
                T val = list2[i];
                string text = val.ToJson();
                stringBuilder.Append(text.TrimStart('{').TrimEnd('}'));
                PropertyInfo[] properties = val.GetType().GetProperties();
                PropertyInfo propertyInfo = ((IEnumerable<PropertyInfo>)properties).Where((Func<PropertyInfo, bool>)((PropertyInfo x) => x.Name == idField)).FirstOrDefault();
                startPid.Clear();
                string id = propertyInfo.GetValue(val, null)!.ToString();
                startPid.Add(id);
                List<T> list3 = ((IEnumerable<T>)list).Where((Func<T, bool>)((T x) => x.GetType().GetProperty(pidField)!.GetValue(x, null)!.ToString() == id.ToString())).ToList();
                if (list3.Count > 0)
                {
                    string text2 = ListToTree(list, pidField, idField, startPid, childrenNodeName);
                    stringBuilder.Append(",\"" + childrenNodeName + "\":" + text2 + "}");
                }
                else
                {
                    stringBuilder.Append("}");
                }
                if (i == list2.Count - 1)
                {
                    stringBuilder.Append("]");
                }
            }
            return stringBuilder.ToString();
        }

        //
        // 摘要:
        //     根据节点找到所有子节点（不包含自身节点）
        //
        // 参数:
        //   list:
        //
        //   pidField:
        //     父ID键
        //
        //   idField:
        //     ID键
        //
        //   startPid:
        //     开始的PID
        //
        // 类型参数:
        //   T:
        public static List<T> FindToTree<T>(List<T> list, string pidField, string idField, List<string> startPid)
        {
            List<T> list2 = new List<T>();
            List<T> list3 = ((IEnumerable<T>)list).Where((Func<T, bool>)((T x) => startPid.Contains(x.GetType().GetProperty(pidField)!.GetValue(x, null)!.ToString()))).ToList();
            for (int i = 0; i < list3.Count; i++)
            {
                T val = list3[i];
                list2.Add(val);
                PropertyInfo[] properties = val.GetType().GetProperties();
                PropertyInfo propertyInfo = ((IEnumerable<PropertyInfo>)properties).Where((Func<PropertyInfo, bool>)((PropertyInfo x) => x.Name == idField)).FirstOrDefault();
                startPid.Clear();
                string id = propertyInfo.GetValue(val, null)!.ToString();
                startPid.Add(id);
                List<T> list4 = ((IEnumerable<T>)list).Where((Func<T, bool>)((T x) => x.GetType().GetProperty(pidField)!.GetValue(x, null)!.ToString() == id.ToString())).ToList();
                if (list4.Count > 0)
                {
                    List<T> collection = FindToTree(list, pidField, idField, startPid);
                    list2.AddRange(collection);
                }
            }
            return list2;
        }
    }
}