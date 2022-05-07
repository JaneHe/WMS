using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

namespace Tops.FRM
{
     public class UploadFile
    {
         private string strDir = "/uploadFile";
         public string[] allowExt = new string[] { ".doc",".docx",".xls",".xlsx",".ppt",".pptx",".rar",".zip",".pdf",".jpg",".png",".jpeg",".gif"};
         public string[] noAllowExt = new string[] { ".exe", ".bat" };
         public string PathAppSetting = "";
         public UploadFile(string pathAppSetting)
         {
             PathAppSetting = pathAppSetting;
             strDir = System.Configuration.ConfigurationManager.AppSettings[pathAppSetting];
         }

         public BizResult UploadFileDocument(HttpRequestBase Request, string FileName,string PathName)
         {
             BizResult br = new BizResult();
             //string path  = HttpContext.Current.Server.MapPath(strDir) + FileName;
             //string abspath = "H:/FTP数据服务器/DocuMstSys/" + PathName +"/"+ FileName;
             string abspath = HttpContext.Current.Server.MapPath(strDir) + PathName + "\\" + FileName;
             Stream stream = null;

             try 
             {
                 if (Request.Files.Count > 0)
                     stream = Request.Files[0].InputStream;
                 else
                     stream = Request.InputStream;
                 if(string.IsNullOrEmpty((strDir+"").Trim()))
                     throw new Exception("[U001]找不到地址设置项：" + PathAppSetting);
                 string fileExt = Path.GetExtension(FileName);
                 
                  if (CheckFileExt(fileExt))
                    //var contentLength=stream.Length;
                    if (stream.Length > 0)
                    {
                        if (stream.Length < 1024 * 50000)
                        {
                            using (FileStream fs = new FileStream(abspath, FileMode.OpenOrCreate))
                            {
                                using (BinaryWriter writer = new BinaryWriter(fs))
                                {
                                    byte[] bf = new byte[stream.Length];
                                    stream.Read(bf, 0, (Int32)stream.Length);
                                    // 写入十进制数，字符串和字符
                                    writer.Write(bf);
                                    br.Data = new
                                    {
                                        Url = PathName+"/" + FileName,
                                        DocumentName = FileName,
                                        TypeCode = fileExt.Substring(1),
                                    };
                                    br.IsSuccess = true;
                                }
                            }
                        }
                        else
                        {
                            br.Msgs.Add("[U002]错误：文件大小超过指定大小:50M");
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
             if (allowExt != null && (!this.allowExt.Contains(fileExt.ToLower())))
                 throw new Exception("[U005]格式不对，允许上传文件格式为：" + allowExt.ToJson());
             if (noAllowExt != null && this.noAllowExt.Contains(fileExt.ToLower()))
                 throw new Exception("[U005-1]格式不对，不允许上传“" + fileExt.ToLower() + "”格式的文件");
             return true;
         }


        
    }
}
