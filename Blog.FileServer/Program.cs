using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Blog.FileServer
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// dotnet Netnr.FileServer.dll "http://*:42"

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //����Kestrel�����ļ�
                    webBuilder.ConfigureKestrel((context, options) =>
                    {
                        options.Limits.MaxRequestBodySize = GlobalTo.GetValue<int>("StaticResource:MaxSize") * 1024 * 1024;
                    });

                    webBuilder.UseStartup<Startup>();
                    if (args.Length > 0)
                    {
                        webBuilder.UseUrls(args[0]);
                    }
                });
    }
}
