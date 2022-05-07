using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EncryptCode;
using System.Web;
using System.IO;
using System.Net.Mail;
using System.Xml;

namespace Tops.FRM.Controllers
{
    public class AccountController : Controller
    {

        [HttpGet]
        public ActionResult Login()
        {
            #region 用于session跨域共享解决方式
            SqlParameter[] ps = {  
                                new SqlParameter("@IPAddress",SqlDbType.VarChar),
                                new SqlParameter("@UserAgent",SqlDbType.VarChar)
                                };
            ps[0].Value = Tops.FRM.TopsUser.IpAddress;
            ps[1].Value = Tops.FRM.TopsUser.UserAgent;
            BizResult br2 = Biz.Execute("SelSessionState", ps, BizType.Query);
            List<DataTable> list = br2.Data as List<DataTable>;
            if (list[0].Rows.Count > 0)
            {
                string LoginState = list[0].Rows[0]["LoginState"].ToString();
                string UserNO = list[0].Rows[0]["UserNO"].ToString();

                if (LoginState.Equals("0") && System.Web.HttpContext.Current.Session["UserNo"] == null)
                {
                    System.Web.HttpContext.Current.Session["UserNo"] = UserNO;
                    System.Web.HttpContext.Current.Session["Powers"] = new TopsUser().GetPowers();
                    System.Web.HttpContext.Current.Session["Roles"] = new TopsUser().GetRoles();

                    string transferUrl = "/Home/Index";
                    System.Web.HttpContext.Current.Response.Redirect(transferUrl); 
                }

            }
            #endregion

            return View();
        }
        [HttpGet]
        public ActionResult mLogin()
        {
            return View();
        }
        /// <summary>
        /// 注册页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public string Login_QRCode(string QRCode)
        {
            return new TopsUser().Login_QRCode(QRCode).ToJson();
        }
        /// <summary>
        /// 新增领队用户
        /// </summary>
        /// <param name="UserNo"></param>
        /// <param name="UserName"></param>
        /// <param name="PassWord"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddLeaderUser(string UserNo, string UserName, string PassWord)
        {
            BizResult br = new BizResult();
            TopsModel tm = new TopsModel(ControllerContext);
            SqlParameter[] ps = { 
                                    new SqlParameter("@UserNo", SqlDbType.VarChar),
                                    new SqlParameter("@UserName", SqlDbType.VarChar),
                                    new SqlParameter("@Password", SqlDbType.VarChar)
                                };
            ps[0].Value = UserNo;
            ps[1].Value = UserName;
            ps[2].Value = Encrypt.Do(UserNo + PassWord);
            br = tm.RunBiz("AddLeaderUser", ps);
            return Json(br);
        }

        [HttpPost]
        public string Login(string UserNO, string Password)
        {
            return new TopsUser().Login(UserNO, Password).ToJson();
        }
         

        [HttpPost]
        public string CheckLogin(string UserNO, string Password)
        {
            return new TopsUser().CheckLogin(UserNO, Password).ToJson();
        }

        public ActionResult Logout()
        {
            #region 用于session跨域共享解决方式
            SqlParameter[] pss = {  
                                new SqlParameter("@IPAddress",SqlDbType.VarChar),
                                new SqlParameter("@UserAgent",SqlDbType.VarChar)
                                };
            pss[0].Value = Tops.FRM.TopsUser.IpAddress;
            pss[1].Value = Tops.FRM.TopsUser.UserAgent;
            BizResult br2 = Biz.Execute("SelSessionState", pss, BizType.Query);
            List<DataTable> list = br2.Data as List<DataTable>;
            if (list[0].Rows.Count > 0)
            {
                string LoginState = list[0].Rows[0]["LoginState"].ToString();
                string UserNO = list[0].Rows[0]["UserNO"].ToString();

                if (LoginState.Equals("0") && System.Web.HttpContext.Current.Session["UserNo"] == null)
                {
                    System.Web.HttpContext.Current.Session["UserNo"] = UserNO;
                    System.Web.HttpContext.Current.Session["Powers"] = new TopsUser().GetPowers();
                    System.Web.HttpContext.Current.Session["Roles"] = new TopsUser().GetRoles();

                    string transferUrl = "/Home/Index";
                    System.Web.HttpContext.Current.Response.Redirect(transferUrl);
                    return View();
                }
            } 
            #endregion 

            try
            {
                var numCode = 2;
                var userNo = Session["UserNo"];
                TopsModel tm = new TopsModel(ControllerContext);
                SqlParameter[] ps = {
                                    new SqlParameter("@NumCode",numCode),
                                    new SqlParameter("@UserNo",userNo)
                                };
                tm.RunBiz("UpdateRespondTime", ps);
            }
            catch (Exception e)
            {
                TopsUser.Logout();
                return View("Login");
                //throw new Exception("登陆超时，请重新登陆" + e.Message);
            }
            finally
            {
                TopsUser.Logout();
            }

            return View("Login");
        }
        [Tops.FRM.Mvc.Filters.LoginFilter]
        public string AddUser()
        {
            BizResult br = new BizResult();
            TopsModel tm = new TopsModel(ControllerContext);
            SqlParameter[] ps = { 
                                    new SqlParameter("@Password", SqlDbType.VarChar)
                                };
            ps[0].Value = Encrypt.Do(Request.Form["UserNO"] + "111111");
            br = tm.RunBiz("AddUser", ps);
            return br.ToJson();
        }

        //重置密码
        [Tops.FRM.Mvc.Filters.LoginFilter]
        public string ResetPwd(string UserNO, string UserName)
        {
            BizResult br = new BizResult();
            TopsModel tm = new TopsModel(ControllerContext);
            SqlParameter[] ps = { 
                                    new SqlParameter("@Password", SqlDbType.VarChar),
                                    new SqlParameter("@UserNO", UserNO),
                                    new SqlParameter("@UserName", UserName)
                                };
            ps[0].Value = Encrypt.Do(UserNO + "111111");
            br = tm.RunBiz("UpdateResetPassword", ps);
            return br.ToJson();
        }
        //更改密码
        [Tops.FRM.Mvc.Filters.LoginFilter]
        public string UpdatePwd()
        {
            BizResult br = new BizResult();
            TopsModel tm = new TopsModel(ControllerContext);
            SqlParameter[] ps = { 
                                    new SqlParameter("@OldPwd", SqlDbType.VarChar),
                                    new SqlParameter("@NewPwd", SqlDbType.VarChar)
                                };
            ps[0].Value = Encrypt.Do(Request.Form["UserNO"] + Request.Form["OldPwd"]);
            ps[1].Value = Encrypt.Do(Request.Form["UserNO"] + Request.Form["NewPwd"]);
            br = tm.RunBiz("UpdatePwd", ps);
            return br.ToJson();
        }

        /// <summary>
        /// 不需要先验证是否已登陆
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string UpdatePwdByUser()
        {
            BizResult br = new BizResult();
            TopsModel tm = new TopsModel(ControllerContext);
            SqlParameter[] ps = {
                                    new SqlParameter("@OldPwd", SqlDbType.VarChar),
                                    new SqlParameter("@NewPwd", SqlDbType.VarChar)
                                };
            ps[0].Value = Encrypt.Do(Request.Params["UserNO"] + Request.Params["OldPwd"]);
            ps[1].Value = Encrypt.Do(Request.Params["UserNO"] + Request.Params["NewPwd"]);
            br = tm.RunBiz("UpdatePwd", ps);
            return br.ToJson();
        }

        [HttpPost]
        [Tops.FRM.Mvc.Filters.LoginFilter]
        public string UpdateRespondTime()
        {
            var userNo = Request.Params["UserNo"];
            var numCode = 1; //1、5分钟发送一次请求，更新一下响应时间  2、记录退出时间
            BizResult br = new BizResult();
            TopsModel tm = new TopsModel(ControllerContext);
            SqlParameter[] ps = {
                                    new SqlParameter("@NumCode",numCode),
                                    new SqlParameter("@UserNo",userNo)
                                };
            br = tm.RunBiz("UpdateRespondTime", ps);
            return br.ToJson();
        }

        /// <summary>
        /// MES系统tools登录页
        /// </summary>
        /// <returns></returns>
        public ActionResult ToolsLogin()
        {
            return View();
        }

        /// <summary>
        /// MES系统tools登录
        /// </summary>
        /// <param name="UserNO"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        [HttpPost]
        public string ToolLogin(string UserNO, string Password)
        {
            return new TopsUser().ToolLogin(UserNO, Password).ToJson();
        }

        /// <summary>
        /// MES系统tools登录
        /// </summary>
        /// <returns></returns>
        public ActionResult ToolsLogout()
        {
            try
            {
                var numCode = 2;
                var userNo = Session["ToolsUserNO"];
                TopsModel tm = new TopsModel(ControllerContext);
                SqlParameter[] ps = {
                                    new SqlParameter("@NumCode",numCode),
                                    new SqlParameter("@UserNo",userNo)
                                };
                tm.RunBiz("UpdateRespondTime", ps);
            }
            catch (Exception e)
            {
                TopsUser.ToolLogout();
                return View("ToolsLogin");
                //throw new Exception("登陆超时，请重新登陆" + e.Message);
            }
            finally
            {
                TopsUser.ToolLogout();
            }
            return View("ToolsLogin");
        }

        /// <summary>
        /// MES系统tools注册
        /// </summary>
        /// <returns></returns>
        public ActionResult ToolsRegister()
        {
            return View();
        }

        /// <summary>
        /// MES系统tools新增用户
        /// </summary>
        /// <returns></returns>
        public string AddToolsUser()
        {
            BizResult br = new BizResult();
            TopsModel tm = new TopsModel(ControllerContext);
            SqlParameter[] ps = { 
                                    new SqlParameter("@UserNO", SqlDbType.VarChar),
                                    new SqlParameter("@UserName", SqlDbType.VarChar),
                                    new SqlParameter("@PassWord", SqlDbType.VarChar),
                                    new SqlParameter("@EncryptionCipher", SqlDbType.VarChar),
                                    new SqlParameter("@Email", SqlDbType.VarChar),
                                };
            ps[0].Value = Request.Form["UserNO"];
            ps[1].Value = Request.Form["UserName"];
            ps[2].Value = Request.Form["Password"];
            ps[3].Value = Encrypt.Do(Request.Form["UserNO"] + Request.Form["Password"]);
            ps[4].Value = Request.Form["Email"];
            br = tm.RunBiz("AddToolUser", ps);

            //加密
            string EncryptionCipher = Tops.FRM.Encryption.Encrypt("he9198@qq.com", "Master");

            string content = "<span>您好,申请人" + Request.Form["UserName"] + "在HNAH_MES系统申请注册,请审核</span><br />" +
                    "<span>通过请点击:</span><a href='" +
                    (System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority + "/Account/CheckPass?UserNO=" + Request.Form["UserNO"] + "&UserName=" + Request.Form["UserName"] + "&IsCheck=1&Email=" + EncryptionCipher) +
                    "'>通过</a><br />" +
                    "<span>不通过请点击:</span><a href='" +
                    (System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority + "/Account/CheckPass?UserNO=" + Request.Form["UserNO"] + "&UserName=" + Request.Form["UserName"] + "&IsCheck=2&Email=" + EncryptionCipher) +
                    "'>不通过</a><br />";
            string title = "HNAH_MES系统tools用户注册审核" + "  " + DateTime.Now.ToString("yyyy-MM-dd");

            List<string> Emaillist = GetEmailPerson();

            SendEmail(Emaillist, Request.Form["UserNO"], Request.Form["UserName"], content, title);
            return br.ToJson();
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="EncryptionCipher">发送邮件地址</param>
        /// <param name="UserNO">申请人账号</param>
        /// <param name="UserName">申请人</param>
        /// <param name="Content">内容</param>
        /// <param name="title">邮件标题</param>
        private void SendEmail(List<string> EncryptionCiphers, string UserNO = "", string UserName = "", string content = "", string title = "")
        {
            MailMessage mail = new MailMessage();
            try
            {

                SmtpClient client = new SmtpClient("smtp.qiye.163.com", 25);
                client.UseDefaultCredentials = true;
                client.Credentials = new System.Net.NetworkCredential("admin@tops828.com", "LWtops828.com");
                client.EnableSsl = true;  //--QQ邮箱发送要加上这句话
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Timeout = 300000;

                MailAddress from = new MailAddress("admin@tops828.com", "admin");//填写电子邮件地址，和显示名称 
                mail.From = from;
                foreach (string EncryptionCipher in EncryptionCiphers)
                {
                    mail.To.Add(new MailAddress(EncryptionCipher));
                }
                //邮件主题 
                mail.Subject = title;
                mail.SubjectEncoding = Encoding.UTF8;


                //邮件内容
                mail.Body = content;

                //设置正文内容是否是包含Html的格式
                mail.IsBodyHtml = true;
                //设置邮件优先级别
                mail.Priority = MailPriority.High;

                client.Send(mail);

                mail.Dispose();
            }
            catch (Exception ex)
            {
                mail.Dispose();
            }
        }


        /// <summary>
        /// MES系统tools 用户注册审核页
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckPass()
        {

            TopsModel tm = new TopsModel(ControllerContext);

            string Email = Request.QueryString["Email"].ToString();
            string UserNO = Request.QueryString["UserNO"].ToString();
            string IsCheck = Request.QueryString["IsCheck"].ToString();

            Email = Tops.FRM.Encryption.Decrypt(Email, "Master");

            SqlParameter[] ps = {
                                    new SqlParameter("@IsCheck",IsCheck),
                                    new SqlParameter("@UserNO",UserNO),
                                    new SqlParameter("@Email",Email)
                                };
            BizResult biz = tm.RunBiz("UpdateCheckPassRecord", ps);
            Dictionary<string, object> dic = biz.OutPut as Dictionary<string, object>;

            string Msgs = string.Empty;
            if (!biz.IsSuccess)
            {
                Msgs = biz.Msgs[0];
            }
            else
            {
                string content = "您的HNAH_MES系统账号已经通过审核！";
                if (IsCheck.Equals("2"))
                {
                    content = "您的HNAH_MES系统账号没有通过审核！";
                }
                string title = "HNAH_MES系统tools用户注册审核" + "  " + DateTime.Now.ToString("yyyy-MM-dd");
                SendEmail(new List<string>() { dic["EmailAddress"].ToString() }, string.Empty, string.Empty, content, title);
            }
            ViewBag.Msgs = Msgs;

            return View();
        }


        /// <summary>
        /// MES系统tools忘记密码
        /// </summary>
        /// <returns></returns>
        public ActionResult ForgetPassWord()
        {
            return View();

        }


        /// <summary>
        /// 发送验证邮箱邮件
        /// </summary>
        public string SendVerificationEmail()
        {
            BizResult br;
            TopsModel tm = new TopsModel(ControllerContext);

            try
            {
                string Email = Request["Email"];
                string UserName = Request["UserName"];
                string EmailCode = new Random().Next(1, 1000000).ToString();
                string title = "HNAH_MES系统tools用户重置密码" + "  " + DateTime.Now.ToString("yyyy-MM-dd");
                string content = "您好,您的邮箱验证为" + EmailCode;

                SqlParameter[] ps = {
                                    new SqlParameter("@Email",Email),
                                    new SqlParameter("@EmailCode",EmailCode),
                                    new SqlParameter("@UserName",UserName)
                                };
                br = tm.RunBiz("UpdateToolUserReset", ps);

                if (br.IsSuccess)
                {
                    SendEmail(new List<string>() { Email }, string.Empty, string.Empty, content, title);
                }
            }
            catch (Exception ex)
            {
                br = new BizResult() { IsSuccess = false, Msgs = new List<string>() { ex.Message } };
            }
            return br.ToJson();
        }


        /// <summary>
        /// 验证码校验
        /// </summary>
        /// <returns></returns>
        public string VerificationCode()
        {
            BizResult br;
            TopsModel tm = new TopsModel(ControllerContext);
            try
            {
                string Email = Request["Email"];
                string UserName = Request["UserName"];
                string EmailCode = Request["EmailCode"];

                SqlParameter[] ps = {
                                    new SqlParameter("@Email",Email),
                                    new SqlParameter("@EmailCode",EmailCode),
                                    new SqlParameter("@UserName",UserName)
                                };
                br = tm.LoadQuery("SelVerificationEmailCode", ps);

            }
            catch (Exception ex)
            {
                br = new BizResult() { IsSuccess = false, Msgs = new List<string>() { ex.Message } };
            }
            return br.ToJson();
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        public ActionResult ResetPassWord()
        {
            return View();
        }

        /// <summary>
        /// 修改tools用户信息
        /// </summary>
        /// <returns></returns>
        public string UpdateToolsUser()
        {

            BizResult br = new BizResult();
            TopsModel tm = new TopsModel(ControllerContext);
            try
            {
                SqlParameter[] ps = {  
                                    new SqlParameter("@UserNO", SqlDbType.VarChar), 
                                    new SqlParameter("@UserName", SqlDbType.VarChar), 
                                    new SqlParameter("@EncryptionCipher", SqlDbType.VarChar) 
                                };
                ps[0].Value = Request.Form["UserNO"];
                ps[1].Value = Request.Form["UserName"];
                ps[2].Value = Encrypt.Do(Request.Form["UserNO"] + Request.Form["Password"]);
                br = tm.RunBiz("UpdateToolUserPassWord", ps);
            }
            catch (Exception ex)
            {
                br = new BizResult() { IsSuccess = false, Msgs = new List<string>() { ex.Message } };
            }

            return br.ToJson();
        }

        /// <summary>
        /// 获取邮件审核人，并发送审核邮件
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public List<string> GetEmailPerson()
        {
            string EmailPersonPath = Server.MapPath("/SendToMailInfo.xml");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(EmailPersonPath);


            XmlElement rootElem = xmlDoc.DocumentElement;   //获取根节点  
            XmlNodeList personNodes = rootElem.GetElementsByTagName("Client"); //获取Client子节点集合 

            List<string> strEmails = new List<string>();
            foreach (XmlNode node in personNodes)
            {
                string strName = ((XmlElement)node).GetAttribute("Name");   //获取name属性值  

                XmlNodeList subEmailNodes = ((XmlElement)node).GetElementsByTagName("Email");  //获取Client子XmlElement集合  
                string strEmail = subEmailNodes[0].InnerText;

                strEmails.Add(strEmail);
            }

            return strEmails;
        }
    }
}
