using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Tops.FRM
{
   public  class EmailErrorLog
    {
        private string logFileName;
        private int logFileSizes;
        
        /// <summary>
        /// 写入日志文件
        /// </summary>
        public EmailErrorLog()
        {
            logFileName = (System.AppDomain.CurrentDomain.BaseDirectory + "/TopsLog/EmailError" + DateTime.Now.ToString("yyyyMMdd") + ".log".Replace("/", "\\")).Replace("\\\\", "\\"); 
        }

       /// <summary>
        /// 自动删除日志文件大小,此方法已经重载.
        /// </summary>
        /// <param name="fileSize">日志文件大小,单位KB</param>
        public EmailErrorLog(int fileSize):this()
        {
            if(fileSize != 0)
            {
                this.logFileSizes = fileSize * 1024;
            }
            else
            {
                this.logFileSizes = 1024;
            }
        }

        /// <summary>
        /// 日志文件完全名,
        /// </summary>
        public string LogFileName
        {
            set
            {
                this.logFileName = value;
            }
        }

        /// <summary>
        /// 写入日志信息
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="IsAutoDelete">是否自动删除日志</param>
        public void writeInLog(string msg)
        {
            if (logFileSizes != 0)
            {
                writeInLog(msg, true);
            }
            else
            {
                writeInLog(msg, false);
            }
        }

        /// <summary>
        /// 写入日志信息
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="IsAutoDelete">是否自动删除日志</param>
        private void writeInLog(string msg, bool IsAutoDelete)
        {
            try
            {
                FileInfo fileinfo = new FileInfo(logFileName);
                if (IsAutoDelete)
                {
                    if (fileinfo.Exists && fileinfo.Length >= logFileSizes)
                    {
                        fileinfo.Delete();
                    }
                }
                using (FileStream fs = fileinfo.OpenWrite())
                {
                    StreamWriter sw = new StreamWriter(fs);
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine("=====================================");
                    sw.Write("添加日期为:" + DateTime.Now.ToString() + "\r\n");
                    sw.Write("日志内容为:" + msg + "\r\n");
                    sw.WriteLine("=====================================");
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }



    }
}
