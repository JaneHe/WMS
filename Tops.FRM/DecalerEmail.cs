using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Timers;
using System.Xml;
using System.Data;
using Microsoft;
using System.IO;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.Web;
namespace Tops.FRM
{
    public class DecalerEmail
    {
        public void TimingSendMail()
        {
            System.Timers.Timer timer = new System.Timers.Timer(20000);
            timer.Enabled = true;
            timer.Interval = 600000; //执行间隔时间,单位为毫秒; 这里实际间隔为10分钟  
            timer.Start();

            timer.Elapsed += new ElapsedEventHandler(T_Elapsed);
            SendResetPasswordEmail();
        }



        /// <summary>
        /// 获取报表定时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {

                DateTime dt0 = DateTime.Now;

                //每天早上8-18点发送邮件
                if (dt0.Hour >= 8 && dt0.Hour <= 18)
                {
                    SendResetPasswordEmail();//邮件发送


                    //循环读取数据
                    SqlParameter[] rownamesp1 = {
                };
                    int count = new Tops.FRM.DbHelperSQL(0).ExecuteSql("declareEmail", rownamesp1);//发送后修改状态为已发送
                }

            }
            catch (Exception ee)
            {

            }
        }










        /// <summary>
        /// 发邮件的代码
        /// </summary>
        /// <param></param>
        public void SendResetPasswordEmail()
        {

            try
            {
                //读取发件人信息"
                XmlDocument ComPanyDoc = new XmlDocument();
                string str = "/UpEmailInfo.xml";
                str = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + str;
                ComPanyDoc.Load(str);
                XmlElement CompanyElem = ComPanyDoc.DocumentElement;   //获取根节点  
                XmlNodeList CompanyNodes = CompanyElem.GetElementsByTagName("Client"); //获取Company子节点集合 
                string ComEmailName = ""; string ComEmailAddre = ""; string ComEmailPw = "";
                foreach (XmlNode node in CompanyNodes)
                {

                    ComEmailName = ((XmlElement)node).GetAttribute("Name");   //获取name属性值
                    ComEmailAddre = ((XmlElement)node).GetElementsByTagName("Email")[0].InnerText;      //"3175532760@qq.com"发件人邮箱
                    ComEmailPw = ((XmlElement)node).GetElementsByTagName("password")[0].InnerText;//发件人密码
                }

                MailAddress from = new MailAddress(ComEmailAddre, ComEmailName); //填寫電子郵件地址，和顯示名稱
                MailMessage mail = new MailMessage();
                mail.From = from;

                //读取收件人 consigneeInfo.xml
                XmlDocument consigeenInfo = new XmlDocument();
                string strs = "DeclaerEmail.xml";
                strs = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + strs;
                consigeenInfo.Load(strs);
                XmlElement CompanyElem1 = consigeenInfo.DocumentElement;   //获取根节点  
                XmlNodeList CompanyNodes1 = CompanyElem1.GetElementsByTagName("Company"); //获取Company子节点集合 
                string consigeenName = "";
                string consigeenEmailAddre = "";//收件人邮箱
                string consigeenEmailAddre2 = "";//收件人邮箱


                foreach (XmlNode node in CompanyNodes1)
                {
                    consigeenName = ((XmlElement)node).GetAttribute("Name");   //获取name属性值
                    consigeenEmailAddre = ((XmlElement)node).GetElementsByTagName("Email")[0].InnerText;
                    consigeenEmailAddre2 = ((XmlElement)node).GetElementsByTagName("Email2")[0].InnerText;
                    //consigeenEmailAddre2 = ((XmlElement)node).GetElementsByTagName("Email2")[0].InnerText;
                }

                //循环读取数据
                SqlParameter[] rownamesp1 = {
                                            };
                DataTable dt1 = SelInWorkProduct("declareEmail", rownamesp1);
                //生成Excel表格
                if (dt1.Rows.Count == 0)
                {
                    return;
                }
                else
                {


                    mail.Attachments.Add(new Attachment(DataToExcel(dt1, "sendFj")));//邮箱附件（Excel表格作为附件发送）

                    mail.To.Add(consigeenEmailAddre);//收件人邮箱consigeenEmailAddre
                    mail.To.Add(consigeenEmailAddre2);//收件人邮箱consigeenEmailAddre
                    mail.Priority = MailPriority.High;  //优先级别最高   
                    mail.Subject = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " 报关邮件提醒";  //主题
                    mail.Body = "报关邮件提醒";

                    mail.IsBodyHtml = true;//设置显示htmls

                    //設置發送郵件服務地址
                    SmtpClient client = new SmtpClient("smtp.qiye.163.com", 25);//个人邮箱协议(企业邮箱需要修改)
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    //填寫服務器地址相關的用戶名和密碼信息
                    client.Credentials = new System.Net.NetworkCredential(ComEmailAddre, ComEmailPw);//发件人邮箱和SMTP服务密码
                    //发送邮件
                    client.Send(mail);
                    //int count = new Tops.FRM.DbHelperSQL(0).ExecuteSql("up_UpdateOverTimeEmail", rownamesp1);//发送邮箱后修改邮箱状态为已发送
                }

            }
            catch (Exception e)
            {

            }
        }
        /// <summary>
        /// 执行查询对象
        /// </summary
        /// <param name="strName">查询对象</param>
        /// <param name="sp">参数</param>
        /// <returns></returns>
        public DataTable SelInWorkProduct(String strName, SqlParameter[] sp)
        {
            DataSet set = new Tops.FRM.DbHelperSQL(0).RunProcedure(strName, sp, strName);
            DataTable dt = set.Tables[0];
            return dt;
        }





        /// <summary>  
        /// Datatable生成Excel表格并返回路径  
        /// </summary>  
        /// <param name="m_DataTable">Datatable</param>  
        /// <param name="s_FileName">文件名</param>  
        /// <returns></returns>  
        public string DataToExcel(System.Data.DataTable m_DataTable, string s_FileName)
        {
            string FileName = AppDomain.CurrentDomain.BaseDirectory + ("/SendMailExcel/") + s_FileName + ".xls";  //Excel文件存放路径  
            if (System.IO.File.Exists(FileName))                                //存在则删除  
            {
                System.IO.File.Delete(FileName);
            }
            System.IO.FileStream objFileStream;
            System.IO.StreamWriter objStreamWriter;
            string strLine = "";
            objFileStream = new System.IO.FileStream(FileName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
            objStreamWriter = new System.IO.StreamWriter(objFileStream, Encoding.Unicode);
            for (int i = 0; i < m_DataTable.Columns.Count; i++)
            {
                strLine = strLine + m_DataTable.Columns[i].Caption.ToString() + Convert.ToChar(9);      //写列标题  
            }
            objStreamWriter.WriteLine(strLine);
            strLine = "";
            for (int i = 0; i < m_DataTable.Rows.Count; i++)
            {
                for (int j = 0; j < m_DataTable.Columns.Count; j++)
                {
                    if (m_DataTable.Rows[i].ItemArray[j] == null)
                        strLine = strLine + " "+Convert.ToChar(9);                                    //写内容  
                    else
                    {
                        string rowstr = "";
                        rowstr = m_DataTable.Rows[i].ItemArray[j].ToString();
                        if (rowstr.IndexOf("\r\n") > 0)
                            rowstr = rowstr.Replace("\r\n", "");
                        //if (rowstr.IndexOf("\t") > 0)
                        //    rowstr = rowstr.Replace("\t", " ");
                        strLine = strLine + rowstr + Convert.ToChar(9);
                    }
                }
                objStreamWriter.WriteLine(strLine);
                strLine = "";
            }
            objStreamWriter.Close();
            objFileStream.Close();
            return FileName;        //返回生成文件的绝对路径  
        }

    }

}




