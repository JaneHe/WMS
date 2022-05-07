using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using EncryptCode;

namespace Tops.FRM
{
    public class TopsUser
    {
        /// <summary>
        /// 远程客户端的 IP 地址
        /// </summary>
        public static string IpAddress
        {
            get
            {
                return System.Web.HttpContext.Current.Request.UserHostAddress;
                //return System.Web.HttpContext.Current.Session["IpAddress"] as String;
            }
        }

        /// <summary>
        /// 远程客户端的 DNS 名称
        /// </summary>
        public static string AgentName
        {
            get
            {
                return System.Web.HttpContext.Current.Request.LogonUserIdentity.Name;
            }
        }

        /// <summary>
        /// 客户端浏览器的原始用户代理信息
        /// </summary>
        public static string UserAgent
        {
            get
            {
                return System.Web.HttpContext.Current.Request.UserAgent;
            }
        }

        /// <summary>
        /// 获取用户编码
        /// </summary>
        public static string UserNO
        {
            get
            {
                try
                {
                    return System.Web.HttpContext.Current.Session["UserNO"] as String;
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }
        /// <summary>
        /// 获取用户姓名
        /// </summary>
        public static string UserName
        {
            get
            {
                return System.Web.HttpContext.Current.Session["UserName"] as String;
            }
        }



        /// <summary>
        /// 获取用户权限
        /// </summary>
        public static List<string> Powers
        {
            get
            {
                List<string> list = System.Web.HttpContext.Current.Session["Powers"] as List<string>;
                var powerCache = System.Configuration.ConfigurationManager.AppSettings["PowerCache"];
                if (string.IsNullOrEmpty(powerCache))
                    powerCache = "false";
                if (list == null || list.Count == 0 || powerCache == "false")
                {
                    list = new TopsUser().GetPowers();
                    System.Web.HttpContext.Current.Session["Powers"] = list;
                }
                return list;
            }
        }
        /// <summary>
        /// 判断当前用户是否含有该权限
        /// </summary>
        /// <param name="power">权限编码</param>
        /// <returns>true:含有该权限；false:不含有该权限</returns>
        public static bool HasPower(string power)
        {
            return TopsUser.Powers.Contains(power.ToUpper());
        }
        public static void Logout()
        {
            System.Web.HttpContext.Current.Session["UserNO"] = null;
            System.Web.HttpContext.Current.Session["UserName"] = null;
            System.Web.HttpContext.Current.Session["Powers"] = null;

            var br = new BizResult();
            SqlParameter[] ps = {  
                                new SqlParameter("@IPAddress",SqlDbType.VarChar),
                                new SqlParameter("@UserAgent",SqlDbType.VarChar)
                                };

            ps[0].Value = Tops.FRM.TopsUser.IpAddress;
            ps[1].Value = Tops.FRM.TopsUser.UserAgent;

            br = Biz.Execute("DelSessionState", ps, BizType.Business);
        }

        public BizResult Login(string UserNO, string password)
        {
            var br = new BizResult();
            var br2 = new BizResult();
            SqlParameter[] ps = { 
                                new SqlParameter("@UserNO",SqlDbType.VarChar),
                                new SqlParameter("@Password",SqlDbType.VarChar),
                                new SqlParameter("@IPAddress",SqlDbType.VarChar),
                                new SqlParameter("@UserAgent",SqlDbType.VarChar)
                                };
            ps[0].Value = UserNO;
            ps[1].Value = Encrypt.Do(UserNO + password);//获取加密后的密码
            ps[2].Value = Tops.FRM.TopsUser.IpAddress;
            ps[3].Value = Tops.FRM.TopsUser.UserAgent;
            br = Biz.Execute("UserLogin", ps, BizType.Business);
            br2 = Biz.Execute("UpdateSessionState", ps, BizType.Business);
            if (br.IsSuccess)
            {
                //添加session
                System.Web.HttpContext.Current.Session["UserNO"] = UserNO;
                if (br.OutPut != null)
                {
                    System.Web.HttpContext.Current.Session["UserName"] = br.OutPut["UserName"];
                }
                System.Web.HttpContext.Current.Session["Powers"] = new TopsUser().GetPowers();
                System.Web.HttpContext.Current.Session["Roles"] = new TopsUser().GetRoles();
                //写日志

                System.Web.HttpContext.Current.Session.Timeout = 120;
            }
            return br;
        }

        /// <summary>
        /// MES系统tools登录
        /// </summary>
        /// <param name="UserNO"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public BizResult ToolLogin(string UserNO, string password)
        {
            var br = new BizResult(); 
            SqlParameter[] ps = { 
                                new SqlParameter("@UserNO",SqlDbType.VarChar),
                                new SqlParameter("@Password",SqlDbType.VarChar) 
                                };
            ps[0].Value = UserNO;
            ps[1].Value = Encrypt.Do(UserNO + password);//获取加密后的密码 
            br = Biz.Execute("UserToolsLogin", ps, BizType.Business); 
            if (br.IsSuccess)
            {
                //添加session
                System.Web.HttpContext.Current.Session["ToolsUserNO"] = UserNO;
                if (br.OutPut != null)
                {
                    System.Web.HttpContext.Current.Session["ToolsUserName"] = br.OutPut["UserName"];
                }

                System.Web.HttpContext.Current.Session.Timeout = 120;
            }
            return br;
        }

        /// <summary>
        /// MES系统tools登出
        /// </summary>
        public static void ToolLogout()
        {
            System.Web.HttpContext.Current.Session["ToolsUserNO"] = null;
            System.Web.HttpContext.Current.Session["ToolsUserName"] = null; 
             
        }


        public BizResult Login_QRCode(string QRCode)
        {
            var br = new BizResult();
            var br2 =  new BizResult();
            SqlParameter[] ps = {
                                new SqlParameter("@QRCode",SqlDbType.VarChar),
                                new SqlParameter("@IPAddress",SqlDbType.VarChar),
                                new SqlParameter("@UserAgent",SqlDbType.VarChar)
                                };
               ps[0].Value = QRCode;
               ps[1].Value = Tops.FRM.TopsUser.IpAddress;
               ps[2].Value = Tops.FRM.TopsUser.UserAgent;
               br = Biz.Execute("UserLogin_QRCode", ps, BizType.Business);
               br2 = Biz.Execute("UserLogin_QRCode_entity", ps, BizType.Query);
               if (br.IsSuccess)
               {
                   List<DataTable> list = br2.Data as List<DataTable>;
                   string sUserNO = (list[0].Rows[0]["s_UserNO"]).ToString().Trim();
                   //添加session
                   System.Web.HttpContext.Current.Session["UserNO"] = sUserNO;
                   System.Web.HttpContext.Current.Session["Powers"] = new TopsUser().GetPowers();
                   System.Web.HttpContext.Current.Session["Roles"] = new TopsUser().GetRoles();
                   //System.Web.HttpContext.Current.Session["UserName"] = QRCode == "" ? "" : QRCode.Substring(0, QRCode.IndexOf("*"));
                   System.Web.HttpContext.Current.Session["UserName"] = (list[0].Rows[0]["s_UserName"]).ToString().Trim();//(list[0].Rows[0]["s_UserNO"]).ToString().Trim();
                   System.Web.HttpContext.Current.Session["QRCode"] = QRCode;
                   //写日志

                   System.Web.HttpContext.Current.Session.Timeout = 120;
               }
               else {
                   new Tops.FRM.EmailErrorLog().writeInLog("对象UserLogin_QRCode" + "没有该二维码" + QRCode);
                   //没有该二维码 
               }
            return br;
        }

        public BizResult CheckLogin(string UserNO, string password)
        {
            var br = new BizResult();
            SqlParameter[] ps = { 
                                new SqlParameter("@UserNO",SqlDbType.VarChar),
                                new SqlParameter("@Password",SqlDbType.VarChar)
                                };
            ps[0].Value = UserNO;
            ps[1].Value = Encrypt.Do(UserNO + password);//获取加密后的密码
            br = Biz.Execute("UserLogin", ps, BizType.Business);
            if (br.IsSuccess)
            {
                //添加session
                System.Web.HttpContext.Current.Session["UserNO"] = UserNO;
                if (br.OutPut != null)
                {
                    System.Web.HttpContext.Current.Session["UserName"] = br.OutPut["UserName"];
                }
                System.Web.HttpContext.Current.Session["Powers"] = new TopsUser().GetPowers();
                System.Web.HttpContext.Current.Session["Roles"] = new TopsUser().GetRoles();
                //写日志

                System.Web.HttpContext.Current.Session.Timeout = 120;
            }
            return br;
        }
        public List<string> GetPowers()
        {
            List<string> powers = new List<string>();
            var br = new BizResult();
            SqlParameter[] ps = { 
                                new SqlParameter("@UserNO",SqlDbType.VarChar)
                                };
            ps[0].Value = UserNO;
            br = Biz.Execute("SelPowersByUserNO", ps, BizType.Query);
            if (br.IsSuccess)
            {
                List<DataTable> list = br.Data as List<DataTable>;
                if (list != null && list.Count > 0 && list[0] != null)
                {
                    for (int i = 0; i < list[0].Rows.Count; i++)
                    {
                        powers.Add((list[0].Rows[i]["PowerID"] + "").ToUpper());
                    }
                }
            }
            else
                throw new Exception(br.Msgs[0]);
            return powers;
        }
        /// <summary>
        /// 获取角色ID
        /// </summary>
        public static string GetOperatorRoleID
        {
            get
            {
                if (UserNO == null || UserNO == "")
                {
                    return "";
                }

                string RoleID;
                var br = new BizResult();
                SqlParameter[] ps ={
                                 new SqlParameter("@UserNO",SqlDbType.VarChar)
                              };
                ps[0].Value = UserNO;
                DbHelperSQL SQL_BK_Helper = Biz.BizDbHelper;        //备份BIZ数据库对象    勇 
                br = Biz.Execute("SelOperatorRoleID", ps, BizType.Query);
                Biz.BizDbHelper = SQL_BK_Helper;                    //还原BIZ数据库对象  勇
                if (br.IsSuccess)
                {
                    List<DataTable> list = br.Data as List<DataTable>;

                    RoleID = (list[0].Rows[0]["OperatorRoleID"]).ToString().Trim();
                }
                else
                {
                    throw new Exception(br.Msgs[0]);
                }
                return RoleID;
            }
        }

        /// <summary>
        /// 获取当前用户角色集合
        /// 曾文勇
        /// 2014-02-28
        /// </summary>
        /// <returns></returns>
        public List<string> GetRoles()
        {
            List<string> Roles = new List<string>();
            var br = new BizResult();
            SqlParameter[] ps = { 
                                new SqlParameter("@UserNO",SqlDbType.VarChar)
                                };
            ps[0].Value = UserNO;
            br = Biz.Execute("SelPowersByUserNO", ps, BizType.Query);
            if (br.IsSuccess)
            {
                List<DataTable> list = br.Data as List<DataTable>;
                if (list != null && list.Count > 0 && list[0] != null)
                {
                    for (int i = 0; i < list[0].Rows.Count; i++)
                    {
                        Roles.Add((list[0].Rows[i]["RoleID"] + "").ToUpper());
                    }
                }
                Roles = Roles.Distinct().ToList();
            }
            else
                throw new Exception(br.Msgs[0]);
            return Roles;
        }

        public static Dictionary<string, string> GetPowersByRoleID(string RoleID)
        {
            Dictionary<string, string> powers = new Dictionary<string, string>();
            var br = new BizResult();
            SqlParameter[] ps = { 
                                new SqlParameter("@RoleID",SqlDbType.VarChar)
                                };
            ps[0].Value = RoleID;
            br = Biz.Execute("SelPowersByRoleID", ps, BizType.Query);
            if (br.IsSuccess)
            {
                List<DataTable> list = br.Data as List<DataTable>;
                if (list != null && list.Count > 0 && list[0] != null)
                {
                    for (int i = 0; i < list[0].Rows.Count; i++)
                    {
                        powers.Add((list[0].Rows[i]["PowerID"] + ""), (list[0].Rows[i]["PowerName"] + ""));
                    }
                }
            }
            return powers;
        }
    }
}


