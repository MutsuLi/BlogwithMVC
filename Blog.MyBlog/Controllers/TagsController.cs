using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Data;
using Blog.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Blog.Web.Controllers
{
    public class TagsController : Controller
    {
        private readonly ContextBase _context;
        public TagsController(ContextBase context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 查询记事本列表
        /// </summary>
        /// <param name="ivm"></param>
        /// <returns></returns>
        [Authorize]
        public QueryDataOutputVM QueryTagList(QueryDataInputVM ivm)
        {
            var ovm = new QueryDataOutputVM();

            var uinfo = new Func.UserAuthAid(HttpContext).Get();

            var query = from a in _context.Tags
                        orderby a.TagId descending
                        where string.IsNullOrEmpty(ivm.pe1) ? true : a.TagName.Contains(ivm.pe1)
                        select new Tags
                        {
                            TagId = a.TagId,
                            TagName = a.TagName,
                            TagOwner = a.TagOwner,
                            TagStatus = a.TagStatus,
                            createTime = a.createTime,
                            updateTime = a.updateTime,
                        };

            Func.Common.QueryJoin(query, ivm, ref ovm);

            return ovm;
        }

        /// <summary>
        /// 保存一条记事本
        /// </summary>
        /// <param name="mo"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResultVM AddTag(string tagName)
        {
            var vm = new ActionResultVM();

            if (string.IsNullOrWhiteSpace(tagName))
            {
                vm.msg = "标签名不可为空值";
                vm.code = -1;
                return vm;
            }
            else
            {
                var uinfo = new Func.UserAuthAid(HttpContext).Get();

                var now = DateTime.Now;
                Tags newtag = new Tags()
                {
                    createTime = now,
                    updateTime = now,
                    TagName = tagName,
                    TagOwner = uinfo.UserId,
                    TagStatus = 1
                };

                _context.Tags.Add(newtag);

                int num = _context.SaveChanges();
                vm.Set(num > 0);
                vm.data = newtag.TagName;
            }

            return vm;
        }

        /// <summary>
        /// 删除一条记事本
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResultVM DelTag(string TagName)
        {
            var vm = new ActionResultVM();

            var result = _context.Tags.FirstOrDefault(x=>x.TagName.Equals(TagName));
            if (null == result)
            {
                vm.msg = "请验证邮箱后再操作";
                vm.code = -1;
                return vm;
            }
            _context.Tags.Remove(result);
            int num = _context.SaveChanges();

            vm.msg = "删除成功";
            return vm;
        }
    }
}