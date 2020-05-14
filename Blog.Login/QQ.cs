using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using Blog.Common;
using Blog.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Blog.Login
{
    public class QQ
    {
        //
        // 摘要:
        //     Step1：获取Authorization Code
        //
        // 参数:
        //   entity:
        public static string AuthorizationHref(QQ_Authorization_RequestEntity entity)
        {
            if (!LoginBase.IsValid(entity))
            {
                return null;
            }
            return QQConfig.API_Authorization_PC + "?client_id=" + entity.client_id + "&response_type=" + entity.response_type + "&state=" + entity.state + "&redirect_uri=" + entity.redirect_uri.ToEncode();
        }

        //
        // 摘要:
        //     Step2：通过Authorization Code获取Access Token
        //
        // 参数:
        //   entity:
        public static QQ_AccessToken_ResultEntity AccessToken(QQ_AccessToken_RequestEntity entity)
        {
            if (!LoginBase.IsValid(entity))
            {
                return null;
            }
            string str = LoginBase.EntityToPars(entity);
            string text = HttpTo.Get(QQConfig.API_AccessToken_PC + "?" + str);
            List<string> list = text.Split('&').ToList();
            JObject jObject = new JObject();
            foreach (string item in list)
            {
                List<string> source = item.Split('=').ToList();
                jObject[source.FirstOrDefault()] = (JToken)source.LastOrDefault();
            }
            return LoginBase.ResultOutput<QQ_AccessToken_ResultEntity>(JsonConvert.SerializeObject(jObject));
        }

        //
        // 摘要:
        //     Step3：获取用户OpenId
        //
        // 参数:
        //   entity:
        public static QQ_OpenId_ResultEntity OpenId(QQ_OpenId_RequestEntity entity)
        {
            QQ_OpenId_ResultEntity qQ_OpenId_ResultEntity = new QQ_OpenId_ResultEntity();
            PropertyInfo[] properties = qQ_OpenId_ResultEntity.GetType().GetProperties();
            if (!LoginBase.IsValid(entity))
            {
                return null;
            }
            string str = LoginBase.EntityToPars(entity);
            string text = HttpTo.Get(QQConfig.API_OpenID_PC + "?" + str);
            text = text.Replace("callback( ", "").Replace(" );", "");
            return LoginBase.ResultOutput<QQ_OpenId_ResultEntity>(text);
        }

        //
        // 摘要:
        //     Step4：获取用户信息
        //
        // 参数:
        //   entity:
        public static QQ_OpenId_get_user_info_ResultEntity OpenId_Get_User_Info(QQ_OpenAPI_RequestEntity entity)
        {
            if (!LoginBase.IsValid(entity))
            {
                return null;
            }
            string str = LoginBase.EntityToPars(entity);
            string text = HttpTo.Get(QQConfig.API_Get_User_Info + "?" + str);
            return LoginBase.ResultOutput<QQ_OpenId_get_user_info_ResultEntity>(text.Replace("\r\n", ""));
        }
    }
    //
    // 摘要:
    //     配置 步骤：Authorization => AccessToken => OpenId => OpenAPI（get_user_info）
    public class QQConfig
    {
        //
        // 摘要:
        //     PC网站，GET
        public static string API_Authorization_PC = "https://graph.qq.com/oauth2.0/authorize";

        //
        // 摘要:
        //     PC网站，GET
        public static string API_AccessToken_PC = "https://graph.qq.com/oauth2.0/token";

        //
        // 摘要:
        //     WAP网站，GET
        public static string API_AccessToken_WAP = "https://graph.z.qq.com/moc2/token";

        //
        // 摘要:
        //     PC GET
        public static string API_OpenID_PC = "https://graph.qq.com/oauth2.0/me";

        //
        // 摘要:
        //     WAP GET
        public static string API_OpenID_WAP = "https://graph.z.qq.com/moc2/me";

        //
        // 摘要:
        //     GET
        public static string API_Get_User_Info = "https://graph.qq.com/user/get_user_info";

        //
        // 摘要:
        //     APP ID
        public static string APPID = "";

        //
        // 摘要:
        //     APP Key
        public static string APPKey = "";

        //
        // 摘要:
        //     回调
        public static string Redirect_Uri = "";
    }
    //
    // 摘要:
    //     Step1：获取Authorization Code Url：http://wiki.connect.qq.com/%E4%BD%BF%E7%94%A8authorization_code%E8%8E%B7%E5%8F%96access_token
    public class QQ_Authorization_RequestEntity
    {
        //
        // 摘要:
        //     授权类型，此值固定为“code”。
        [Required]
        public string response_type
        {
            get;
            set;
        } = "code";


        //
        // 摘要:
        //     申请QQ登录成功后，分配给应用的appid。
        [Required]
        public string client_id
        {
            get;
            set;
        } = QQConfig.APPID;


        //
        // 摘要:
        //     成功授权后的回调地址，必须是注册appid时填写的主域名下的地址， 建议设置为网站首页或网站的用户中心。 注意需要将url进行URLEncode。
        [Required]
        public string redirect_uri
        {
            get;
            set;
        } = QQConfig.Redirect_Uri;


        //
        // 摘要:
        //     client端的状态值。用于第三方应用防止CSRF攻击，成功授权后回调时会原样带回。 请务必严格按照流程检查用户与state参数状态的绑定。
        [Required]
        public string state
        {
            get;
            set;
        } = Guid.NewGuid().ToString("N");


        //
        // 摘要:
        //     请求用户授权时向用户显示的可进行授权的列表。 可填写的值是API文档中列出的接口，以及一些动作型的授权（目前仅有：do_like），如果要填写多个接口名称，请用逗号隔开。
        //     例如：scope=get_user_info,list_album,upload_pic,do_like 不传则默认请求对接口get_user_info进行授权。
        //     建议控制授权项的数量，只传入必要的接口名称，因为授权项越多，用户越可能拒绝进行任何授权。
        public string scope
        {
            get;
            set;
        } = "get_user_info";


        //
        // 摘要:
        //     仅PC网站接入时使用。 用于展示的样式。不传则默认展示为PC下的样式。 如果传入“mobile”，则展示为mobile端下的样式。
        public string display
        {
            get;
            set;
        }

        //
        // 摘要:
        //     仅WAP网站接入时使用。 QQ登录页面版本（1：wml版本； 2：xhtml版本），默认值为1。
        public string g_ut
        {
            get;
            set;
        }
    }
    //
    // 摘要:
    //     OpenAPI调用说明_OAuth2.0 前提说明 1. 该appid已经开通了该OpenAPI的使用权限。 从API列表的接口列表中可以看到，有的接口是完全开放的，有的接口则需要提前提交申请，以获取访问权限。
    //     2. 准备访问的资源是用户授权可访问的。 网站调用该OpenAPI读写某个openid（用户）的信息时，必须是该用户已经对你的appid进行了该OpenAPI的授权
    //     （例如用户已经设置了相册不对外公开，则网站是无法读取照片信息的）。 用户可以进入QQ空间->设置->授权管理进行访问权限的设置。 3. 已经成功获取到Access
    //     Token，并且Access Token在有效期内。 调用OpenAPI接口 QQ登录提供了用户信息/动态同步/日志/相册/微博等OpenAPI（详见API列表），
    //     网站需要将请求发送到某个具体的OpenAPI接口，以访问或修改用户数据。
    public class QQ_OpenAPI_RequestEntity
    {
        //
        // 摘要:
        //     可通过使用Authorization_Code获取Access_Token 或来获取。 access_token有3个月有效期。
        [Required]
        public string access_token
        {
            get;
            set;
        }

        //
        // 摘要:
        //     申请QQ登录成功后，分配给应用的appid
        [Required]
        public string oauth_consumer_key
        {
            get;
            set;
        } = QQConfig.APPID;


        //
        // 摘要:
        //     用户的ID，与QQ号码一一对应。 可通过调用https://graph.qq.com/oauth2.0/me?access_token=YOUR_ACCESS_TOKEN
        //     来获取。
        [Required]
        public string openid
        {
            get;
            set;
        }
    }

    //
    // 摘要:
    //     Step2：通过Authorization Code获取Access Token
    public class QQ_AccessToken_RequestEntity
    {
        //
        // 摘要:
        //     授权类型，在本步骤中，此值为“authorization_code”。
        [Required]
        public string grant_type
        {
            get;
            set;
        } = "authorization_code";


        //
        // 摘要:
        //     申请QQ登录成功后，分配给网站的appid。
        [Required]
        public string client_id
        {
            get;
            set;
        } = QQConfig.APPID;


        //
        // 摘要:
        //     申请QQ登录成功后，分配给网站的appkey。
        [Required]
        public string client_secret
        {
            get;
            set;
        } = QQConfig.APPKey;


        //
        // 摘要:
        //     上一步返回的authorization code。 如果用户成功登录并授权，则会跳转到指定的回调地址，并在URL中带上Authorization Code。
        //     例如，回调地址为www.qq.com/my.php，则跳转到：http://www.qq.com/my.php?code=520DD95263C1CFEA087******
        //     注意此code会在10分钟内过期。
        [Required]
        public string code
        {
            get;
            set;
        }

        //
        // 摘要:
        //     与上面一步中传入的redirect_uri保持一致。
        [Required]
        public string redirect_uri
        {
            get;
            set;
        } = QQConfig.Redirect_Uri;

    }
    public class QQ_OpenId_ResultEntity
    {
        //
        // 摘要:
        //     client id
        public string client_id
        {
            get;
            set;
        }

        //
        // 摘要:
        //     openid是此网站上唯一对应用户身份的标识， 网站可将此ID进行存储便于用户下次登录时辨识其身份， 或将其与用户在网站上的原有账号进行绑定。
        public string openid
        {
            get;
            set;
        }
    }

    public class QQ_OpenId_RequestEntity
    {
        //
        // 摘要:
        //     Step3：在Step1中获取到的access token。 Url：http://wiki.connect.qq.com/%E8%8E%B7%E5%8F%96%E7%94%A8%E6%88%B7openid_oauth2-0
        [Required]
        public string access_token
        {
            get;
            set;
        }
    }


    //
    // 摘要:
    //     获取登录用户的昵称、头像、性别 Url：http://wiki.connect.qq.com/get_user_info
    public class QQ_OpenId_get_user_info_ResultEntity
    {
        //
        // 摘要:
        //     返回码
        public int ret
        {
            get;
            set;
        }

        //
        // 摘要:
        //     如果ret 小于 0，会有相应的错误信息提示，返回数据全部用UTF-8编码。
        public string msg
        {
            get;
            set;
        }

        //
        // 摘要:
        //     用户在QQ空间的昵称。
        public string nickname
        {
            get;
            set;
        }

        //
        // 摘要:
        //     大小为30×30像素的QQ空间头像URL。
        public string figureurl
        {
            get;
            set;
        }

        //
        // 摘要:
        //     大小为50×50像素的QQ空间头像URL。
        public string figureurl_1
        {
            get;
            set;
        }

        //
        // 摘要:
        //     大小为100×100像素的QQ空间头像URL。
        public string figureurl_2
        {
            get;
            set;
        }

        //
        // 摘要:
        //     大小为40×40像素的QQ头像URL。
        public string figureurl_qq_1
        {
            get;
            set;
        }

        //
        // 摘要:
        //     大小为100×100像素的QQ头像URL。需要注意，不是所有的用户都拥有QQ的100x100的头像，但40x40像素则是一定会有。
        public string figureurl_qq_2
        {
            get;
            set;
        }

        //
        // 摘要:
        //     性别。 如果获取不到则默认返回"男"
        public string gender
        {
            get;
            set;
        }

        //
        // 摘要:
        //     标识用户是否为黄钻用户（0：不是；1：是）。
        public string is_yellow_vip
        {
            get;
            set;
        }

        //
        // 摘要:
        //     标识用户是否为黄钻用户（0：不是；1：是）
        public string vip
        {
            get;
            set;
        }

        //
        // 摘要:
        //     黄钻等级
        public string yellow_vip_level
        {
            get;
            set;
        }

        //
        // 摘要:
        //     黄钻等级
        public string level
        {
            get;
            set;
        }

        //
        // 摘要:
        //     标识是否为年费黄钻用户（0：不是； 1：是）
        public string is_yellow_year_vip
        {
            get;
            set;
        }
    }
        //
        // 摘要:
        //     返回说明： 如果成功返回，即可在返回包中获取到Access Token。
        public class QQ_AccessToken_ResultEntity
        {
            //
            // 摘要:
            //     授权令牌，Access_Token。
            public string access_token
            {
                get;
                set;
            }

            //
            // 摘要:
            //     该access token的有效期，单位为秒。
            public int expires_in
            {
                get;
                set;
            }

            //
            // 摘要:
            //     在授权自动续期步骤中，获取新的Access_Token时需要提供的参数。
            public string refresh_token
            {
                get;
                set;
            }
        }
    }
