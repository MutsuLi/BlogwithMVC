using System.ComponentModel;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Blog.Func;
using Blog.Web.Filters;
using Blog.Common;

namespace Blog.Web.Controllers
{
    /// <summary>
    /// 混合、综合、其它
    /// </summary>
    public class MixController : Controller
    {
        /// <summary>
        /// 关于页面
        /// </summary>
        /// <returns></returns>
        public IActionResult About()
        {
            return View();
        }

        /// <summary>
        /// 服务器状态
        /// </summary>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [ResponseCache(Duration = 10)]
        public ActionResultVM AboutServerStatus()
        {
            var vm = new ActionResultVM
            {
                data = new OSInfoTo()
            };
            return vm;
        }

        /// <summary>
        /// 条款
        /// </summary>
        /// <returns></returns>
        public IActionResult Terms()
        {
            return View();
        }

        /// <summary>
        /// FAQ
        /// </summary>
        /// <returns></returns>
        public IActionResult FAQ()
        {
            return View();
        }
    }
}