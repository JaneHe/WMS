using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

namespace Tops.FRM
{
    public class Upload
    {
        private string strDir = "/upload";
        //允许上图的图片格式
        public string[] allowExt = new string[] { ".gif", ".jpg", ".jpeg" };
        public string[] noAllowExt = new string[] { ".exe", ".bat" };
        public string PathAppSetting = "";
        public Upload(string pathAppSetting)
        {
            PathAppSetting = pathAppSetting;
            strDir = System.Configuration.ConfigurationManager.AppSettings[pathAppSetting];
        }
        public BizResult UploadPhoto(HttpRequestBase Request, string FileName)
        {
            BizResult br = new BizResult();
            string path = "";
            Stream stream = null;
            
            try
            {
                if (Request.Files.Count > 0)
                    stream = Request.Files[0].InputStream;
                else
                    stream = Request.InputStream;
                if (string.IsNullOrEmpty((strDir + "").Trim()))
                    throw new Exception("[U001]找不到地址设置项：" + PathAppSetting);
                string fileExt = Path.GetExtension(FileName);
                if (CheckFileExt(fileExt))
                    //var contentLength=stream.Length;
                    if (stream.Length > 0)
                    {
                        if (stream.Length < 1024 * 500)
                        {
                            string newFileNameBase = "p_" + DateTime.Now.ToString("yyyyMMddhhmmssfff");
                            string newFileName = newFileNameBase + Path.GetExtension(FileName);        //新文件名
                            string fileName = HttpContext.Current.Server.MapPath(strDir) + newFileName;
                            string newMinFilename = newFileNameBase + "_s" + Path.GetExtension(FileName);
                            string minfileName = HttpContext.Current.Server.MapPath(strDir) + newMinFilename;
                            if (!Directory.Exists(HttpContext.Current.Server.MapPath(strDir)))
                                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(strDir));
                            path = strDir + newFileName;
                            SaveThumbnailImage(stream, fileName);
                            SaveThumbnailImage(stream, minfileName, 100);
                            br.Data = new
                            {
                                path = strDir + newFileName,
                                minPath = strDir + newMinFilename,
                            };
                            br.IsSuccess = true;
                        }
                        else
                        {
                            br.Msgs.Add("[U002]错误：图片大小超过指定大小:500k");
                        }
                    }
                    else
                    {
                        br.Msgs.Add("[U003]错误：请上传图片");
                    }
            }
            catch (Exception ex)
            {
                br.Msgs.Add("[U004]错误：" + ex.Message);
            }
            return br;
        }
        public BizResult UploadFile(HttpRequestBase Request, string FileName)
        {
            BizResult br = new BizResult();
            string path = "";
            Stream stream = null;
            
            try
            {
                if (Request.Files.Count > 0)
                    stream = Request.Files[0].InputStream;
                else
                    stream = Request.InputStream;
                if (string.IsNullOrEmpty((strDir + "").Trim()))
                    throw new Exception("[U001]找不到地址设置项：" + PathAppSetting);
                string fileExt = Path.GetExtension(FileName);
                if (CheckFileExt(fileExt))
                    //var contentLength=stream.Length;
                    if (stream.Length > 0)
                    {
                        if (stream.Length < 1024 * 1024 * 20)
                        {
                            string newFileNameBase = "f_" + DateTime.Now.ToString("yyyyMMddhhmmssfff");
                            string newFileName = newFileNameBase + Path.GetExtension(FileName);        //新文件名
                            string fileName = HttpContext.Current.Server.MapPath(strDir) + newFileName;
                            if (!Directory.Exists(HttpContext.Current.Server.MapPath(strDir)))
                                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(strDir));
                            path = strDir + newFileName;
                            SaveFile(stream, fileName);
                            br.Data = new
                            {
                                path = strDir + newFileName
                            };
                            br.IsSuccess = true;
                        }
                        else
                        {
                            br.Msgs.Add("[U002]错误：文件超过指定大小:20M");
                        }
                    }
                    else
                    {
                        br.Msgs.Add("[U003]错误：请上传文件");
                    }
            }
            catch (Exception ex)
            {
                br.Msgs.Add("[U004]错误：" + ex.Message);
            }
            return br;
        }
        private bool CheckFileExt(string fileExt)
        {
            if (allowExt!=null&&(!this.allowExt.Contains(fileExt.ToLower())))
                throw new Exception("[U005]格式不对，允许上传图片格式为：" + allowExt.ToJson());
            if (noAllowExt != null && this.noAllowExt.Contains(fileExt.ToLower()))
                throw new Exception("[U005-1]格式不对，不允许上传“" + fileExt.ToLower() + "”格式的文件");
            return true;
        }
        /// <summary>  
        /// 生成缩略图  
        /// </summary>  
        /// <param name="srcfilename">原始文件</param>  
        /// <param name="objfilename">目标文件，不能和原始文件的路径相同</param>  
        /// <param name="width">图宽度</param>  
        /// <param name="heigth">图高度</param>  
        private void SaveThumbnailImage(Stream inputStream, string objfilename, int heigth = 0)
        {
            System.Drawing.Image img = System.Drawing.Image.FromStream(inputStream);
           
            try
            {
                if (heigth > 0)
                {
                    int width = heigth * img.Width / img.Height;
                    img = img.GetThumbnailImage(width, heigth, null, IntPtr.Zero);
                }
                img.Save(objfilename);
            }
            finally
            {
                img.Dispose();
            }
        }
        private void SaveFile(Stream inputStream, string objfilename, int heigth = 0)
        {
            using (FileStream fs = new FileStream(objfilename, FileMode.OpenOrCreate))
            {
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    byte[] bf=new byte[inputStream.Length];
                    inputStream.Read(bf, 0, (Int32)inputStream.Length);
                    // 写入十进制数，字符串和字符
                    writer.Write(bf);
                }
            }


        }
        
        public static string GetSmallPic(string filename)
        {
            string[] fnames = filename.Split('.');
            if (fnames.Length == 2)
            {
                filename = fnames[0] + "_s." + fnames[1];
            }
            return filename;
        }
    }
}
