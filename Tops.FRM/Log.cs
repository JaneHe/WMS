using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Tops.FRM
{
    public class Log
    {
        public static string LogPath
        {
            get
            {
                try
                {
                    return (System.AppDomain.CurrentDomain.BaseDirectory + System.Configuration.ConfigurationManager.AppSettings["LogPath"].Replace("/", "\\")).Replace("\\\\", "\\");

                }
                catch
                {
                    try
                    {
                        return HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["LogPath"]);
                    }
                    catch
                    {
                        return (System.AppDomain.CurrentDomain.BaseDirectory + "/TopsLog/".Replace("/", "\\")).Replace("\\\\", "\\");
                    }
                }
            }
        }
        static System.Object lockThis = new object();
        /// <summary>
        /// 单例模式，线程安全
        /// </summary>
        private static readonly Log log = new Log();
        public static Log GetInstance()
        {
            return log;
        }
        public static void WriteSqlLog(string sqlStr, SqlParameter[] ps, BizType bizType, string bizName, string description, string checkName = null)
        {
            Log log = Log.GetInstance();
            log.WriteForSql(sqlStr, ps, bizType, bizName, description, checkName);
        }
        public static void WriteLog(string filename, string description)
        {
            Log log = Log.GetInstance();
            log.Write(filename, description);
        }

        private void WriteForSql(string sqlStr, SqlParameter[] ps, BizType bizType, string bizName, string description, string checkName = null)
        {
            var newPth = LogPath;
            if (!newPth.EndsWith("\\"))
                newPth += "\\";
            var filePath = newPth + "topslog" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            //加锁，防止并发问题
            lock (lockThis)
            {
                //FileStream fs = null;
                StreamWriter sw = null;
                try
                {
                    //创建文件夹
                    if (!System.IO.Directory.Exists(newPth))
                    {
                        System.IO.Directory.CreateDirectory(newPth);
                    }
                    /*if (!System.IO.File.Exists(filePath))
                    {
                        fs = System.IO.File.Create(filePath);
                    }
                    else
                    {
                        fs = File.OpenWrite(filePath);
                    }
                    fs.Close();*/

                    StringBuilder logStr = new StringBuilder();
                    logStr.Append(@"
**************************************************************************************************************------>" + DateTime.Now);
                    logStr.Append(@"
类型：" + bizType + " " + (bizType == BizType.Query ? "查询对象：" : "业务对象：") + bizName + " " + (checkName == null ? "" : ("业务检查：" + checkName)));
                    logStr.Append(@"
说明：" + description);
                    logStr.Append(@"
***************************************************************************************************************
SQL:
");
                    logStr.Append(sqlStr.Replace("\n", "\r\n"));
                    logStr.Append(@"
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
参数：");
                    if (ps != null)
                    {
                        for (int i = 0; i < ps.Length; i++)
                        {
                            logStr.Append(@"
（" + i + "）ParameterName:\"" + ps[i].ParameterName + "\" DbType:\"" + ps[i].DbType + "\" Value:\"" + (ps[i].Value ?? "null") + "\"");
                        }
                    }
                    logStr.Append(@"
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
                    sw = new StreamWriter(filePath, true);
                    sw.Write(logStr.ToString());
                    sw.Flush();
                }
                catch (Exception ex)
                {
                    //throw ex;   错误不抛出，防止阻断下步操作
                }
                finally
                {
                    if (sw != null)
                        sw.Close();
                    /*if(fs!=null)
                        fs.Close();*/
                }
            }
        }
        private void Write(string filename, string description)
        {
            var newPth = LogPath;
            if (!newPth.EndsWith("\\"))
                newPth += "\\";
            var filePath = newPth + filename + ".log";
            lock (lockThis)
            {
                StreamWriter sw = null;
                try
                {

                    //创建文件夹
                    if (!System.IO.Directory.Exists(newPth))
                    {
                        System.IO.Directory.CreateDirectory(newPth);
                    }
                    sw = new StreamWriter(filePath, true);

                    string logStr = description;
                    sw.Write(logStr);
                    sw.Flush();

                }
                catch (Exception ex)
                {
                    //throw ex; 错误不抛出，防止阻断下步操作
                }
                finally
                {
                    if (sw != null)
                        sw.Close();
                }

            }

        }
    }
}
