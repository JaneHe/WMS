using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Globalization;
using System.Reflection;
using System.Data;
using System.Net.Mime;

namespace Tops.FRM
{
    /// <summary>
    /// 邮件帮助类
    /// </summary>
    public static class EmailHelp
    {
        private static LogHelp log = new LogHelp() { type = "Email" };

        /// <summary>
        /// 发送邮件 JaneHe 2017-09-07
        /// </summary>
        public static void SendEmail()
        {
            Timer T = new Timer(60000);
            T.Elapsed += new ElapsedEventHandler(T_Elapsed);
            T.AutoReset = true;
            T.Enabled = true;
        }

        /// <summary>
        /// 发送邮件方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            //查询邮件信息
            BizResult br = Biz.Execute("SelEmailInfo", new SqlParameter[] { new SqlParameter() }, BizType.Query);
            List<DataTable> list = br.Data as List<DataTable>;
            List<tEmailInfo> EmailInfoList = new ModelHandle<tEmailInfo>().FillModel(list[0]);
            //DataSet set = new Tops.FRM.DbHelperSQL(0).RunProcedure("up_SelEmailInfo", new SqlParameter[] { null }, "up_SelEmailInfo");
            //DataTable tb = set.Tables[0];
            //List<tEmailInfo> list = new ModelHandle<tEmailInfo>().FillModel(tb);

            //当前时分
            int hour = DateTime.Now.Hour;
            int minute = DateTime.Now.Minute;

            if (EmailInfoList != null)
            {

                //利用当前时间的时分与数据库中数据表的时间的时分进行比较，如果相同，则触发发送邮件
                foreach (tEmailInfo emailinfo in EmailInfoList)
                {
                    Dictionary<int, int> dic = new Dictionary<int, int>();
                    if (emailinfo.SendTime1 != null)
                    {
                        if (emailinfo.EmailTime != 0 && emailinfo.SendTime1 != DateTime.MinValue)
                        {
                            TimeSpan ts1 = new TimeSpan(emailinfo.SendTime1.Ticks);
                            TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);
                            TimeSpan ts = ts1.Subtract(ts2).Duration();

                            int isSame = ts.TotalSeconds % emailinfo.EmailTime > 60 ? 0 : 1;
                            if (isSame.Equals(1) && !dic.Keys.Contains(ts1.Hours))
                                dic.Add(ts1.Hours, ts1.Minutes);
                        }
                    }
                    if (emailinfo.SendTime2 != null && emailinfo.SendTime2 != DateTime.MinValue)
                    {
                        if (emailinfo.EmailTime != 0)
                        {
                            TimeSpan ts1 = new TimeSpan(emailinfo.SendTime2.Ticks);
                            TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);
                            TimeSpan ts = ts1.Subtract(ts2).Duration();

                            int isSame = ts.TotalSeconds % emailinfo.EmailTime > 60 ? 0 : 1;
                            if (isSame.Equals(1) && !dic.Keys.Contains(ts1.Hours))
                                dic.Add(ts1.Hours, ts1.Minutes);
                        }
                    }
                    //if (dic.Count > 0)
                    //    SendEmailInfo(emailinfo);


                }
            }
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="emailinfo"></param>
        public static void SendEmailInfo(tEmailInfo emailinfo)
        {

            try
            {
                //获取文件的HTML
                string html = ReadFile(emailinfo.Address);
                //检索HTML部分
                int StartIndex = html.IndexOf("<html>");
                int EndIndex = html.IndexOf("</html>");
                //截取html部分代码
                if (!StartIndex.Equals(-1) && !EndIndex.Equals(-1))
                {
                    html = html.Substring(StartIndex, EndIndex - StartIndex);
                }

                //输出页访问字符串
                StringBuilder sb = new StringBuilder();

                //存储过程全局变量
                string[] StoredProcedures = new string[0];
                if (emailinfo.StoredProcedure.IndexOf(",") != -1)
                {
                    StoredProcedures = emailinfo.StoredProcedure.Split(',');
                }
                else if (!string.IsNullOrEmpty(emailinfo.StoredProcedure))
                {
                    StoredProcedures = new string[1] { emailinfo.StoredProcedure };
                }


                //DataSet set = new Tops.FRM.DbHelperSQL(0).RunProcedure(emailinfo.StoredProcedure, new SqlParameter[] { null }, emailinfo.StoredProcedure);
                //DataTable tb = set.Tables[0];

                if (StoredProcedures != null)
                {

                    for (int procedureindex = 0; procedureindex < StoredProcedures.Length; procedureindex++)
                    {

                        BizResult br = Biz.Execute(StoredProcedures[procedureindex], new SqlParameter[] { new SqlParameter() }, BizType.Query);
                        List<DataTable> list = br.Data as List<DataTable>;
                        DataTable tb = list[0];

                        //如果不存在该模板标识则跳过
                        if (!html.IndexOf(StoredProcedures[procedureindex]).Equals(-1) && !string.IsNullOrEmpty(StoredProcedures[procedureindex]))
                        {
                            //目标替换文本
                            string targetstr = html.Substring(html.IndexOf("<" + StoredProcedures[procedureindex] + ">") + (StoredProcedures[procedureindex].Length + 2), html.IndexOf("</" + StoredProcedures[procedureindex] + ">") - html.IndexOf("<" + StoredProcedures[procedureindex] + ">") - (StoredProcedures[procedureindex].Length + 2));

                            if (tb.Rows.Count > 0)
                            {
                                for (int rowindex = 0; rowindex < tb.Rows.Count; rowindex++)
                                {
                                    //循环替换文本
                                    string replacestr = targetstr;
                                    for (int colindex = 0; colindex < tb.Columns.Count; colindex++)
                                    {
                                        replacestr = replacestr.Replace("$" + tb.Columns[colindex].ColumnName, tb.Rows[rowindex][tb.Columns[colindex].ColumnName].ToString());

                                    }
                                    sb.Append(replacestr);
                                }
                            }
                            html = html.Replace(targetstr, sb.ToString());
                            html = html.Replace("<" + StoredProcedures[procedureindex] + ">", string.Empty).Replace("</" + StoredProcedures[procedureindex] + ">", string.Empty);

                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                SendEmail(emailinfo, html);
            }
            catch (Exception ex)
            {
                log.Write("框架内置邮件发送--" + "主键ID=" + emailinfo.Id + "  邮件名称=" + emailinfo.EmailName + "  发送时间=" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "  错误=" + ex.Message + "  错误类型=" + ex.GetType());
            }
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="emailinfo"></param>
        /// <param name="content">输出内容</param>
        public static void SendEmail(tEmailInfo emailinfo, string content)
        {
            EmailErrorLog email = new EmailErrorLog();
            MailMessage mail = new MailMessage();
            try
            {
                //查询邮件相关的发送接收人信息
                BizResult br = Biz.Execute("SelEmailPerson", new SqlParameter[] { new SqlParameter("@EmailInfoID", emailinfo.Id) }, BizType.Query);
                List<DataTable> list = br.Data as List<DataTable>;
                List<tEmailPerson> EmailPersonList = new ModelHandle<tEmailPerson>().FillModel(list[0]);


                SmtpClient client = new SmtpClient("smtp.qiye.163.com", 25);
                client.UseDefaultCredentials = true;
                client.Credentials = new System.Net.NetworkCredential("admin@tops828.com", "LianWang2020");
                client.EnableSsl = true;  //--QQ邮箱发送要加上这句话
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Timeout = 300000;

                MailAddress from = new MailAddress("admin@tops828.com", "admin");//填写电子邮件地址，和显示名称 
                mail.From = from;
                foreach (tEmailPerson person in EmailPersonList)
                    mail.To.Add(new MailAddress(person.EmailAddress));
                //邮件主题 
                mail.Subject = emailinfo.EmailName + "  " + DateTime.Now.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);
                mail.SubjectEncoding = Encoding.UTF8;
                //邮件内容
                mail.Body = content;
                //设置正文内容是否是包含Html的格式
                mail.IsBodyHtml = true;
                //设置邮件优先级别
                mail.Priority = MailPriority.High;

                //判断是否有附件
                if (!string.IsNullOrEmpty(emailinfo.AttachsAddress))
                {
                    //附件部分
                    string[] attachmensPaths = emailinfo.AttachsAddress.Split(';');

                    for (int i = 0; i < attachmensPaths.Length; i++)
                    {
                        attachmensPaths[i] = attachmensPaths[i].Substring(1, attachmensPaths[i].Length - 1).Replace("@DateTime", DateTime.Now.ToString("yyyyMMddmmddss")).Replace("@PrevDate", DateTime.Now.AddDays(-1).ToString("yyyyMMdd")).Replace("@Date", DateTime.Now.ToString("yyyyMMdd"));
                        //判断附件是否存在于指定位置
                        if (!File.Exists(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Replace("\\", "/") + attachmensPaths[i]))
                        {
                            continue;
                        }
                        Attachment data = new Attachment(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Replace("\\", "/") + attachmensPaths[i], MediaTypeNames.Application.Octet);//实例化附件   
                        data.Name = attachmensPaths[i].Substring(attachmensPaths[i].LastIndexOf("/") + 1, attachmensPaths[i].Length - attachmensPaths[i].LastIndexOf("/") - 1);
                        //data.ContentDisposition.CreationDate = System.IO.File.GetCreationTime(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Replace("\\", "/") + attachmensPaths[i]);//获取附件的创建日期  
                        //data.ContentDisposition.ModificationDate = System.IO.File.GetLastWriteTime(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Replace("\\", "/") + attachmensPaths[i]);//获取附件的修改日期  
                        //data.ContentDisposition.ReadDate = System.IO.File.GetLastAccessTime(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Replace("\\", "/") + attachmensPaths[i]);//获取附件的读取日期  
                        mail.Attachments.Add(data);//添加到附件中  
                        log.Write("邮件名称=" + data.Name + "  路径=" + System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Replace("\\", "/") + attachmensPaths[i]);
                    }
                }
                //有指定附件但是没加到邮件附件则不予以发送邮件
                if (!(!string.IsNullOrEmpty(emailinfo.AttachsAddress) && mail.Attachments.Count == 0))
                {
                    client.Send(mail);
                    foreach (Attachment aI in mail.Attachments)
                    {
                        aI.Dispose();
                    }
                    log.Write("框架内置邮件发送--" + "主键ID=" + emailinfo.Id + "  邮件名称=" + emailinfo.EmailName + "  附件数量=" + (mail.Attachments != null ? mail.Attachments.Count : 0) + "  发送时间=" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + " 发送成功! 邮件信息=" + client.Host + "--" + client.PickupDirectoryLocation + "--" + client.Port + "--" + client.TargetName);

                }
                else
                {
                    log.Write("邮件因为指定附件地址,但系统匹配不到该文件,故无法送!");
                }
                mail.Dispose();
            }
            catch (Exception ex)
            {
                log.Write("框架内置邮件发送--" + "主键ID=" + emailinfo.Id + "  邮件名称=" + emailinfo.EmailName + "  发送时间=" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "  错误=" + ex.Message + "  错误类型=" + ex.GetType());
                mail.Dispose();
            }
        }

        /// <summary>  
        /// 读取已经设置好的文件 
        /// </summary>  
        /// <param name="path">文件路径"/Views/Device/DeviceList.cshtml"</param>  
        /// <returns></returns>  
        public static string ReadFile(string Path)
        {
            try
            {
                StreamReader sr = new StreamReader(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Replace("\\", "/") + Path, System.Text.Encoding.GetEncoding("utf-8"));
                string content = sr.ReadToEnd().ToString();
                log.Write(content);
                sr.Close();
                return content;
            }
            catch
            {
                return "<span style='color:red; font-size:x-large;'>Sorry,The Ariticle wasn't found!! It may have been deleted accidentally from Server.</span>";
            }
        }


    }

    /// <summary>
    /// 邮件信息实体类
    /// </summary>
    public class tEmailInfo
    {
        public int Id { get; set; }
        public string EmailName { get; set; }
        public int EmailTime { get; set; }
        public DateTime SendTime1 { get; set; }
        public DateTime SendTime2 { get; set; }
        public string Address { get; set; }
        public string StoredProcedure { get; set; }
        public string AttachsAddress { get; set; }
    }

    /// <summary>
    /// 邮件信息相关人员类
    /// </summary>
    public class tEmailPerson
    {
        public string EmailAddress { get; set; }
        public string Remark { get; set; }
        public string Name { get; set; }
        public int PersonState { get; set; }
    }

    /// <summary>
    /// DataTable 转换实体类
    /// </summary>
    public class ModelHandle<T> where T : new()
    {

        /// <summary>  
        /// 填充对象列表：用DataTable填充实体类
        /// </summary>  
        public List<T> FillModel(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            List<T> modelList = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                //T model = (T)Activator.CreateInstance(typeof(T));  
                T model = new T();
                for (int i = 0; i < dr.Table.Columns.Count; i++)
                {
                    PropertyInfo propertyInfo = model.GetType().GetProperty(dr.Table.Columns[i].ColumnName);
                    if (propertyInfo != null && dr[i] != DBNull.Value)
                        propertyInfo.SetValue(model, dr[i], null);
                }

                modelList.Add(model);
            }
            return modelList;
        }
    }

    /// <summary>
    /// 日志类
    /// </summary>
    public class LogHelp
    {

        public string type { get; set; }

        #region 错误日志记录
        public System.Object lockobj = new object();
        public void Write(string description)
        {
            description = @" 
**********************************************************************************
日志：" + description + "[" + DateTime.Now + "]";
            lock (lockobj)
            {
                StreamWriter sw = null;
                try
                {
                    string url = AppDomain.CurrentDomain.BaseDirectory + "\\日志\\" + type;
                    if (!Directory.Exists(url))//判断文件夹是否存在
                    {
                        Directory.CreateDirectory(url);//新建文件夹
                    }
                    sw = new StreamWriter(url + "\\Log_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log", true);
                    sw.WriteLine(description);
                    sw.Flush();
                }
                catch (Exception ex)
                {
                    sw.Write(ex.Message);
                    sw.Flush();
                    //throw ex; 错误不抛出，防止阻断下步操作
                }
                finally
                {
                    if (sw != null)
                        sw.Close();
                }
            }
        }
        #endregion
    }


}
