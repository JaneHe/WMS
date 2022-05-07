using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetSpeech;
using System.IO;
using System.Collections;
using Tops.FRM;

namespace TopsHNAH.Controllers
{
    public class SpeechController : Controller
    {
        //
        // GET: /Speech/
        SpVoice voice = new DotNetSpeech.SpVoice();

        Dictionary<string, string> LanguageDic = new Dictionary<string, string>();

        //错误标识
        private int ExceptionLevel = 0;
 

        /// <summary>
        /// 文字转成语音文件，并保存在本地
        /// </summary>
        /// <param name="name">文件名</param>
        /// <param name="format">格式</param>
        /// <param name="text">语音提示文本</param>
        /// <param name="language">语言</param>
        [HttpGet]
        public JsonResult Save(string name, string format = "mp3", string text = "", string language = "")
        {
            SpeechStreamFileMode SpFileMode = SpeechStreamFileMode.SSFMCreateForWrite;

            SpFileStream ss = new SpFileStream();

            string ProjectPath = HttpRuntime.AppDomainAppPath.ToString() + "Scripts\\Radio";

            //完整文件路径
            string FullPath = ProjectPath + "\\" + name + "." + format;

            //拷贝文件路径
            string CopyFullPath = ProjectPath + "\\" + (name + " (1)") + "." + format;

            try
            {
                //查看是否存在该文件夹
                if (!Directory.Exists(ProjectPath))
                {
                    Directory.CreateDirectory(ProjectPath);
                }



                lock (this)
                {
                    //判断文件是否存在
                    if (System.IO.File.Exists(FullPath))
                    {
                        //重新创建一个新的文件拷贝一份之前的文件
                        System.IO.File.Copy(FullPath, CopyFullPath, false);
                        //System.IO.File.Delete(FullPath); 

                        Log.WriteLog("Errors","语音提示Copy");
                    }

                    ExceptionLevel = 1;
                    ss.Open(FullPath, SpFileMode, false);

                    ExceptionLevel = 2;

                    voice.Voice = GetVoice(language);
                    voice.Volume = 100; 
                    voice.AudioOutputStream = ss; 
                    voice.Rate = 1; 
                     
                    voice.Speak(text, SpeechVoiceSpeakFlags.SVSFlagsAsync);


                    Log.WriteLog("Errors", "语音提示1");
                    voice.WaitUntilDone(System.Threading.Timeout.Infinite);
                    ss.Close();


                    Log.WriteLog("Errors", "语音提示2");
                    //因为成功，所以删除拷贝文件
                    System.IO.File.Delete(CopyFullPath);


                    Log.WriteLog("Errors", "语音提示3");
                    //清0
                    ExceptionLevel = 0;

                    return Json(new BizResult() { Data = null, IsSuccess = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                if (ExceptionLevel > 1)
                { 
                    ss.Close();
                }


                Log.WriteLog("Errors", ex.Message);

                //因为异常删除原来文件，把拷贝文件还原,然后把拷贝文件删除
                System.IO.File.Delete(FullPath);
                System.IO.File.Copy(CopyFullPath, FullPath, false);
                System.IO.File.Delete(CopyFullPath);

                ExceptionLevel = 0;

                return Json(new BizResult() { Data = null, IsSuccess = false, Msgs = new List<string>() { ex.Message } }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取语言的设置对象  
        /// </summary>
        /// <param name="language">语言 （中文 chs，英文 eng）</param> 
        /// <returns></returns>
        public SpObjectToken GetVoice(string language = "")
        { 
            string str = "";
            //获取原始数据
            string InitStr = language;

            if (LanguageDic.Count > 0)
            { 
                for (int i = 0; i < voice.GetVoices(string.Empty, string.Empty).Count; i++)
                {
                    str = voice.GetVoices(string.Empty, string.Empty).Item(i).Id;
                    str = str.Substring(59, str.Length - 59);
                    if (str.IndexOf("ZH-CN") != -1)
                    {
                        LanguageDic.Add("chs", str);
                    }
                    else if (str.IndexOf("EN-US") != -1)
                    {
                        LanguageDic.Add("eng", str);
                    }
                }

                switch (language)
                {
                    case "chs":
                        language = LanguageDic[language];
                        break;
                    case "eng":
                        language = LanguageDic[language];
                        break;
                }
            }
             
            if (InitStr.Equals(language))
            {
                return voice.Voice;
            }
            else
            {

                InitStr = language;
            }
             
            DotNetSpeech.ISpeechObjectTokens obj = voice.GetVoices(string.Empty, string.Empty);
             
            int count = obj.Count;
             
            for (int index = 0; index < count; index++)
            {
                str = voice.GetVoices(string.Empty, string.Empty).Item(index).Id;
                str = str.Substring(59, str.Length - 59);
                if (str.Equals(InitStr))
                {
                    return obj.Item(index) as SpObjectToken;
                }
            }

            return voice.Voice;
        }
    }
}
