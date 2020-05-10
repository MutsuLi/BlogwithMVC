using System.ComponentModel;
using Blog.Func;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Areas.Draw.Controllers
{
    [Area("Draw")]
    public class DiscoverController : Controller
    {
        /// <summary>
        /// Draw列表
        /// </summary>
        /// <param name="q"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public IActionResult Index(string q, int page = 1)
        {
            var uinfo = new UserAuthAid(HttpContext).Get();

            var ps = Func.Common.DrawQuery(q, 0, uinfo.UserId, page);
            ps.Route = Request.Path;
            ViewData["q"] = q;
            return View("_PartialDrawList", ps);
        }
    }
}