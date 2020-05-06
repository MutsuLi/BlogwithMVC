using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using Blog.Common;
using Blog.Utils;
using Newtonsoft.Json.Linq;

namespace Blog.Login
{
    public class GitHub
    {
        //
        // 摘要:
        //     请求授权地址
        //
        // 参数:
        //   entity:
        public static string AuthorizeHref(GitHub_Authorize_RequestEntity entity)
        {
            if (!LoginBase.IsValid(entity))
            {
                return null;
            }
            return GitHubConfig.API_Authorize + "?client_id=" + entity.client_id + "&scope=" + entity.scope.ToEncode() + "&state=" + entity.state + "&redirect_uri=" + entity.redirect_uri.ToEncode();
        }

        //
        // 摘要:
        //     获取 access token
        //
        // 参数:
        //   entity:
        public static GitHub_AccessToken_ResultEntity AccessToken(GitHub_AccessToken_RequestEntity entity)
        {
            if (!LoginBase.IsValid(entity))
            {
                return null;
            }
            string data = LoginBase.EntityToPars(entity);
            HttpWebRequest httpWebRequest = HttpTo.HWRequest(GitHubConfig.API_AccessToken, "POST", data);
            httpWebRequest.Accept = "application/json";
            string result = HttpTo.Url(httpWebRequest);
            return LoginBase.ResultOutput<GitHub_AccessToken_ResultEntity>(result);
        }

        //
        // 摘要:
        //     获取 用户信息
        //
        // 参数:
        //   entity:
        public static GitHub_User_ResultEntity User(GitHub_User_RequestEntity entity)
        {
            if (!LoginBase.IsValid(entity))
            {
                return null;
            }
            string str = LoginBase.EntityToPars(entity);
            HttpWebRequest httpWebRequest = HttpTo.HWRequest(GitHubConfig.API_User + "?" + str);
            httpWebRequest.UserAgent = entity.ApplicationName;
            string result = HttpTo.Url(httpWebRequest);
            return LoginBase.ResultOutput<GitHub_User_ResultEntity>(result, new List<string>
            {
                "plan"
            });
        }
    }

    //
    // 摘要:
    //     user
    public class GitHub_User_RequestEntity
    {
        //
        // 摘要:
        //     access_token
        [Required]
        public string access_token
        {
            get;
            set;
        }

        //
        // 摘要:
        //     github 申请的应用名称
        [Required]
        public string ApplicationName
        {
            get;
            set;
        } = GitHubConfig.ApplicationName;

    }

    //
    // 摘要:
    //     用户信息
    public class GitHub_User_ResultEntity
    {
        public string login
        {
            get;
            set;
        }

        public int id
        {
            get;
            set;
        }

        public string avatar_url
        {
            get;
            set;
        }

        public string gravatar_id
        {
            get;
            set;
        }

        public string url
        {
            get;
            set;
        }

        public string html_url
        {
            get;
            set;
        }

        public string followers_url
        {
            get;
            set;
        }

        public string following_url
        {
            get;
            set;
        }

        public string gists_url
        {
            get;
            set;
        }

        public string starred_url
        {
            get;
            set;
        }

        public string subscriptions_url
        {
            get;
            set;
        }

        public string organizations_url
        {
            get;
            set;
        }

        public string repos_url
        {
            get;
            set;
        }

        public string events_url
        {
            get;
            set;
        }

        public string received_events_url
        {
            get;
            set;
        }

        public string type
        {
            get;
            set;
        }

        public bool site_admin
        {
            get;
            set;
        }

        public string name
        {
            get;
            set;
        }

        public string company
        {
            get;
            set;
        }

        public string blog
        {
            get;
            set;
        }

        public string location
        {
            get;
            set;
        }

        public string email
        {
            get;
            set;
        }

        public bool hireable
        {
            get;
            set;
        }

        public string bio
        {
            get;
            set;
        }

        public int public_repos
        {
            get;
            set;
        }

        public int public_gists
        {
            get;
            set;
        }

        public int followers
        {
            get;
            set;
        }

        public int following
        {
            get;
            set;
        }

        public string created_at
        {
            get;
            set;
        }

        public string updated_at
        {
            get;
            set;
        }

        public int private_gists
        {
            get;
            set;
        }

        public int total_private_repos
        {
            get;
            set;
        }

        public int owned_private_repos
        {
            get;
            set;
        }

        public int disk_usage
        {
            get;
            set;
        }

        public int collaborators
        {
            get;
            set;
        }

        public JObject plan
        {
            get;
            set;
        }
    }
    //
    // 摘要:
    //     Step1：获取authorize Code
    public class GitHub_Authorize_RequestEntity
    {
        //
        // 摘要:
        //     注册应用时的获取的client_id
        [Required]
        public string client_id
        {
            get;
            set;
        } = GitHubConfig.ClientID;


        //
        // 摘要:
        //     github鉴权成功之后，重定向到网站
        [Required]
        public string redirect_uri
        {
            get;
            set;
        } = GitHubConfig.Redirect_Uri;


        //
        // 摘要:
        //     自己设定，用于防止跨站请求伪造攻击
        [Required]
        public string state
        {
            get;
            set;
        } = Guid.NewGuid().ToString("N");


        //
        // 摘要:
        //     该参数可选。需要调用Github哪些信息，可以填写多个，以逗号分割，比如：scope=user public_repo。 如果不填写，那么你的应用程序将只能读取Github公开的信息，比如公开的用户信息，公开的库(repository)信息以及gists信息
        public string scope
        {
            get;
            set;
        } = "user,public_repo";


        public string allow_signup
        {
            get;
            set;
        }
    }

    //
    // 摘要:
    //     access token 请求参数
    public class GitHub_AccessToken_RequestEntity
    {
        //
        // 摘要:
        //     注册应用时的获取的client_id
        [Required]
        public string client_id
        {
            get;
            set;
        } = GitHubConfig.ClientID;


        //
        // 摘要:
        //     注册应用时的获取的client_secret。
        [Required]
        public string client_secret
        {
            get;
            set;
        } = GitHubConfig.ClientSecret;


        //
        // 摘要:
        //     调用authorize获得的code值。
        [Required]
        public string code
        {
            get;
            set;
        }

        //
        // 摘要:
        //     回调地址，需需与注册应用里的回调地址一致。
        [Required]
        public string redirect_uri
        {
            get;
            set;
        } = GitHubConfig.Redirect_Uri;


        //
        // 摘要:
        //     Step1 回传的值
        public string state
        {
            get;
            set;
        }
    }
    //
    // 摘要:
    //     配置 步骤：authorize => access_token => user
    public class GitHubConfig
    {
        //
        // 摘要:
        //     GET
        public static string API_Authorize = "https://github.com/login/oauth/authorize";

        //
        // 摘要:
        //     POST
        public static string API_AccessToken = "https://github.com/login/oauth/access_token";

        //
        // 摘要:
        //     GET
        public static string API_User = "https://api.github.com/user";

        //
        // 摘要:
        //     Client ID
        public static string ClientID = "";

        //
        // 摘要:
        //     Client Secret
        public static string ClientSecret = "";

        //
        // 摘要:
        //     回调
        public static string Redirect_Uri = "";

        //
        // 摘要:
        //     github 申请的应用名称
        public static string ApplicationName = "";
    }

    //
    // 摘要:
    //     access_token 信息
    public class GitHub_AccessToken_ResultEntity
    {
        //
        // 摘要:
        //     access_token
        public string access_token
        {
            get;
            set;
        }

        //
        // 摘要:
        //     类型
        public string token_type
        {
            get;
            set;
        }

        //
        // 摘要:
        //     授权的信息
        public string scope
        {
            get;
            set;
        }
    }
}
