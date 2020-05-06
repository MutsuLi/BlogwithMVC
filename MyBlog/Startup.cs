using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Blog.Common;
using Blog.Func;
using Microsoft.AspNetCore.Http;
using Blog.Web.Filters;
using Blog.Utils;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Linq;

namespace Blog.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            GlobalTo.Configuration = configuration;
            GlobalTo.HostEnvironment = env;

            using var db = new Data.ContextBase();
            if (db.Database.EnsureCreated())
            {
                var jodb = FileTo.ReadText(GlobalTo.WebRootPath + "/scripts/example/", "data.json").ToJObject();

                db.UserInfo.AddRange(jodb["UserInfo"].ToString().ToEntitys<Data.Models.UserInfo>());

                db.Tags.AddRange(jodb["Tags"].ToString().ToEntitys<Data.Models.Tags>());

                db.UserWriting.AddRange(jodb["UserWriting"].ToString().ToEntitys<Data.Models.UserWriting>());

                db.UserWritingTags.AddRange(jodb["UserWritingTags"].ToString().ToEntitys<Data.Models.UserWritingTags>());

                db.UserReply.AddRange(jodb["UserReply"].ToString().ToEntitys<Data.Models.UserReply>());

                db.Run.AddRange(jodb["Run"].ToString().ToEntitys<Data.Models.Run>());

                db.KeyValues.AddRange(jodb["KeyValues"].ToString().ToEntitys<Data.Models.KeyValues>());

                db.Gist.AddRange(jodb["Gist"].ToString().ToEntitys<Data.Models.Gist>());

                db.Draw.AddRange(jodb["Draw"].ToString().ToEntitys<Data.Models.Draw>());

                db.DocSet.AddRange(jodb["DocSet"].ToString().ToEntitys<Data.Models.DocSet>());

                db.DocSetDetail.AddRange(jodb["DocSetDetail"].ToString().ToEntitys<Data.Models.DocSetDetail>());

                db.SaveChanges();
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                //cookie存储需用户同意，欧盟新标准，暂且关闭，否则用户没同意无法写入
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddControllersWithViews(options =>
            {
                //注册全局错误过滤器
                options.Filters.Add(new Filters.FilterConfigs.ErrorActionFilter());

                //注册全局过滤器
                options.Filters.Add(new Filters.FilterConfigs.GlobalFilter());

                //注册全局授权访问时登录标记是否有效
                options.Filters.Add(new Filters.FilterConfigs.LoginSignValid());
            });

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                //Action原样输出JSON
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                //日期格式化
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });

            //配置swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Blog API",
                    Version = "v1"
                });

                "Web,Func,Common".Split(',').ToList().ForEach(x =>
                {
                    c.IncludeXmlComments(System.AppContext.BaseDirectory + "Blog." + x + ".xml", true);
                });
            });

            //路由小写
            services.AddRouting(options => options.LowercaseUrls = true);

            //授权访问信息
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                //允许其他站点携带授权Cookie访问，会出现伪造
                options.Cookie.SameSite = SameSiteMode.None;
                options.LoginPath = "/account/login";
            });

            //session
            services.AddSession();

            //定时任务
            FluentScheduler.JobManager.Initialize(new Func.TaskAid.TaskComponent.Reg());

            //配置上传文件大小限制（详细信息：FormOptions）
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = GlobalTo.GetValue<int>("StaticResource:MaxSize") * 1024 * 1024;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMemoryCache memoryCache)
        {
            //缓存
            CacheTo.memoryCache = memoryCache;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            //配置swagger
            app.UseSwagger().UseSwaggerUI(c =>
            {
                c.DocumentTitle = "Netnr API";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", c.DocumentTitle);
            });

            //默认起始页index.html
            DefaultFilesOptions options = new DefaultFilesOptions();
            options.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(options);

            //静态资源允许跨域
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = (x) =>
                {
                    x.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                }
            });

            app.UseRouting();

            //授权访问
            app.UseAuthentication();
            app.UseAuthorization();

            //session
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute("U", "{controller=U}/{id}", new { action = "index" });
                endpoints.MapControllerRoute("areas", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("Code", "{area:exists}/{controller=Code}/{id?}/{sid?}", new { action = "index" });
                endpoints.MapControllerRoute("Raw", "{area:exists}/{controller=Raw}/{id?}", new { action = "index" });
                endpoints.MapControllerRoute("User", "{area:exists}/{controller=User}/{id?}", new { action = "index" });
            });
        }
    }
}
