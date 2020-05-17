using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Blog.Data;
using Newtonsoft.Json.Linq;
using Blog.Web.Filters;
using System.IO;
//using Blog.WeChat;
//using Blog.WeChat.Entities;
using Microsoft.Extensions.Hosting;
using System.Threading;
//using Qcloud.Shared.Api;
using System.ComponentModel;
using Blog.Func.ViewModel;
using Microsoft.AspNetCore.Http;
using Blog.Common;
using Blog.Data.Models;
using Blog.Utils;

namespace Blog.Web.Controllers
{
    /// <summary>
    /// 服务、对接
    /// </summary>
    public class ServicesController : Controller
    {
        
        #region WebHook

        /// <summary>
        /// WebHook
        /// </summary>
        /// <returns></returns>
        public ActionResultVM WebHook()
        {
            var vm = new ActionResultVM();

            try
            {
                if (Request.Method == "POST")
                {
                    using var ms = new MemoryStream();
                    Request.Body.CopyTo(ms);
                    string postStr = System.Text.Encoding.UTF8.GetString(ms.ToArray());

                    //new WebHookService(postStr);

                    vm.data = postStr;
                    vm.Set(ARTag.success);
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
                ConsoleTo.Log(ex);
            }

            return vm;
        }

        public class WebHookService
        {
            /// <summary>
            /// 推送的JSON包
            /// </summary>
            public JObject JoPush { get; set; }

            /// <summary>
            /// 邮箱
            /// </summary>
            public string Pemail { get; set; }

            /// <summary>
            /// 仓库名
            /// </summary>
            public string PrepositoryName { get; set; }

            /// <summary>
            /// 提交的信息
            /// </summary>
            public string PcommitMessage { get; set; }

            /// <summary>
            /// clone url
            /// </summary>
            public string PgitUrl { get; set; }

            /// <summary>
            /// 仓库主页链接
            /// </summary>
            public string PhomePage { get; set; }

            /// <summary>
            /// 构造 部署
            /// </summary>
            /// <param name="postStr">推送的JSON包</param>
            public WebHookService(string postStr)
            {
                JoPush = JObject.Parse(postStr);
                if (JoPush.ContainsKey("pusher"))
                {
                    Pemail = JoPush["pusher"]["email"].ToString();
                    PrepositoryName = JoPush["repository"]["name"].ToString();
                    PcommitMessage = JoPush["commits"][0]["message"].ToString();
                    PgitUrl = JoPush["repository"]["clone_url"].ToString();
                    PhomePage = JoPush["repository"]["homepage"].ToString();
                    string configEmail = GlobalTo.GetValue("WebHook:GitHub:Email");
                    string configNotDeploy = GlobalTo.GetValue("WebHook:GitHub:NotDeploy");
                    if (Pemail == configEmail && !PcommitMessage.ToLower().Contains(configNotDeploy))
                    {
                        Deploy();
                    }
                }
            }

            /// <summary>
            /// 部署
            /// </summary>
            public void Deploy()
            {
                //根目录
                string domainPath = GlobalTo.GetValue("WebHook:GitHub:DomainRootPath");

                //子域名&文件夹
                string subdomain = PrepositoryName;
                if (!string.IsNullOrWhiteSpace(PhomePage))
                {
                    subdomain = PhomePage.Replace("//", "^").Split('^')[1].Split('.')[0];
                }

                string path = domainPath + subdomain;

                //异步
                ThreadPool.QueueUserWorkItem(callBack =>
                {
                    if (!Directory.Exists(path))
                    {
                        string cmd = CmdFor.GitClone(PgitUrl, path);
                        CmdTo.Shell(cmd);
                    }
                    else
                    {
                        string cmd = CmdFor.GitPull(path);
                        CmdTo.Shell(cmd);
                    }
                });
            }

            /// <summary>
            /// 命令
            /// </summary>
            public class CmdFor
            {
                public static string GitClone(string giturl, string path)
                {
                    return $"git clone {giturl} {path}";
                }

                public static string GitPull(string path)
                {
                    return $"cd {path} && git pull origin master";
                }
            }
        }

        #endregion

        #region 百科字典

        /// <summary>
        /// 字典
        /// </summary>
        /// <returns></returns>
        [FilterConfigs.LocalAuth]
        public IActionResult KeyValues()
        {
            string cmd = RouteData.Values["id"]?.ToString();
            if (cmd != null)
            {
                string result = string.Empty;
                var rt = new List<object>
                {
                    0,
                    "fail"
                };

                try
                {
                    switch (cmd)
                    {
                        case "grab":
                            {
                                string key = Request.Form["Key"].ToString();
                                string api = $"https://baike.baidu.com/api/openapi/BaikeLemmaCardApi?scope=103&format=json&appid=379020&bk_key={key.ToEncode()}&bk_length=600";
                                string apirt = HttpTo.Get(api);
                                if (apirt.Length > 100)
                                {
                                    using var db = new ContextBase();
                                    var kvMo = db.KeyValues.Where(x => x.KeyName == key).FirstOrDefault();
                                    if (kvMo == null)
                                    {
                                        kvMo = new KeyValues
                                        {
                                            KeyId = Guid.NewGuid().ToString(),
                                            KeyName = key.ToLower(),
                                            KeyValue = apirt
                                        };
                                        db.KeyValues.Add(kvMo);
                                    }
                                    else
                                    {
                                        kvMo.KeyValue = apirt;
                                        db.KeyValues.Update(kvMo);
                                    }

                                    rt[0] = db.SaveChanges();
                                    rt[1] = kvMo;
                                }
                                else
                                {
                                    rt[0] = 0;
                                    rt[1] = apirt;
                                }
                            }
                            break;
                        case "synonym":
                            {
                                var keys = Request.Form["keys"].ToString().Split(',').ToList();

                                string mainKey = keys.First().ToLower();
                                keys.RemoveAt(0);

                                var listkvs = new List<KeyValueSynonym>();
                                foreach (var key in keys)
                                {
                                    var kvs = new KeyValueSynonym
                                    {
                                        KsId = Guid.NewGuid().ToString(),
                                        KeyName = mainKey,
                                        KsName = key.ToLower()
                                    };
                                    listkvs.Add(kvs);
                                }

                                using var db = new ContextBase();
                                var mo = db.KeyValueSynonym.Where(x => x.KeyName == mainKey).FirstOrDefault();
                                if (mo != null)
                                {
                                    db.KeyValueSynonym.Remove(mo);
                                }
                                db.KeyValueSynonym.AddRange(listkvs);
                                int oldrow = db.SaveChanges();
                                rt[0] = 1;
                                rt[1] = " 受影响 " + oldrow + " 行";
                            }
                            break;
                        case "addtag":
                            {
                                var tags = Request.Form["tags"].ToString().Split(',').ToList();

                                if (tags.Count > 0)
                                {
                                    using var db = new ContextBase();
                                    var mt = db.Tags.Where(x => tags.Contains(x.TagName)).ToList();
                                    if (mt.Count == 0)
                                    {
                                        var listMo = new List<Tags>();
                                        var tagHs = new HashSet<string>();
                                        foreach (var tag in tags)
                                        {
                                            if (tagHs.Add(tag))
                                            {
                                                var mo = new Tags
                                                {
                                                    TagName = tag.ToLower(),
                                                    TagStatus = 1,
                                                    TagHot = 0,
                                                    TagIcon = tag.ToLower() + ".svg"
                                                };
                                                listMo.Add(mo);
                                            }
                                        }
                                        tagHs.Clear();

                                        //新增&刷新缓存
                                        db.Tags.AddRange(listMo);
                                        rt[0] = db.SaveChanges();

                                        Func.Common.TagsQuery(false);

                                        rt[1] = "操作成功";
                                    }
                                    else
                                    {
                                        rt[0] = 0;
                                        rt[1] = "标签已存在：" + mt.ToJson();
                                    }
                                }
                                else
                                {
                                    rt[0] = 0;
                                    rt[1] = "新增标签不能为空";
                                }
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    rt[1] = ex.Message;
                    rt.Add(ex.StackTrace);
                }

                result = rt.ToJson();
                return Content(result);
            }
            return View();
        }

        #endregion

        #region 任务

        /// <summary>
        /// 任务项
        /// </summary>
        public enum TaskItem
        {
            /// <summary>
            /// 备份
            /// </summary>
            Backup,
            /// <summary>
            /// 代码片段同步
            /// </summary>
            GistSync,
            /// <summary>
            /// 链接替换
            /// </summary>
            ReplaceLink,
            /// <summary>
            /// 处理操作记录
            /// </summary>
            HOR
        }

        /// <summary>
        /// 需要处理的事情
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        [ResponseCache(Duration = 60)]
        [FilterConfigs.LocalAuth]
        public ActionResultVM ExecTask(TaskItem? ti)
        {
            var vm = new ActionResultVM();

            try
            {
                if (!ti.HasValue)
                {
                    ti = (TaskItem)Enum.Parse(typeof(TaskItem), RouteData.Values["id"]?.ToString(), true);
                }

                switch (ti)
                {
                    default:
                        vm.Set(ARTag.invalid);
                        break;

                    case TaskItem.Backup:
                        vm = Func.TaskAid.BackupDataBase();
                        break;

                    case TaskItem.GistSync:
                        vm = Func.TaskAid.GistSync();
                        break;

                    case TaskItem.ReplaceLink:
                        vm = Func.TaskAid.ReplaceLink();
                        break;

                    case TaskItem.HOR:
                        vm = Func.TaskAid.HandleOperationRecord();
                        break;
                }
            }
            catch (Exception ex)
            {
                vm.Set(ex);
            }

            return vm;
        }
        #endregion
    }
}