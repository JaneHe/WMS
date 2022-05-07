using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using EncryptCode;


namespace TopsHNAH.Controllers
{
    public class StaffManageController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PasswordEncrypt(string UserNO)
        {
            string a = Encrypt.Do("于娜"+"111111");
            a = Encrypt.Do("李贺"+"111111");
            a = Encrypt.Do("王志"+"111111"); 
            a = Encrypt.Do("张凤岩B"+"111111"); 
            a = Encrypt.Do("王玥"+"111111"); 
            a = Encrypt.Do("傅刚A"+"111111"); 
            a = Encrypt.Do("傅刚B"+"111111"); 
            a = Encrypt.Do("刘铁坤"+"111111"); 
            a = Encrypt.Do("王伟"+"111111"); 
            a = Encrypt.Do("张玉海"+"111111"); 
            a = Encrypt.Do("刘兴权"+"111111"); 
            a = Encrypt.Do("刘刚"+"111111"); 
            a = Encrypt.Do("刘朋"+"111111"); 
            a = Encrypt.Do("年欢"+"111111"); 
            a = Encrypt.Do("张杨"+"111111"); 
            a = Encrypt.Do("王洪文"+"111111"); 
            a = Encrypt.Do("赵博"+"111111"); 
            a = Encrypt.Do("奚红"+"111111"); 
            a = Encrypt.Do("minghua"+"111111"); 
            a = Encrypt.Do("刘辉"+"111111"); 
            a = Encrypt.Do("张博"+"111111"); 
            a = Encrypt.Do("吴金龙"+"111111"); 
            a = Encrypt.Do("王江宁"+"111111"); 
            a = Encrypt.Do("王彦娇"+"111111"); 
            a = Encrypt.Do("黄艳萍"+"111111"); 
            a = Encrypt.Do("薄洋"+"111111"); 
            a = Encrypt.Do("王井富"+"111111"); 
            a = Encrypt.Do("程芳"+"111111"); 
            a = Encrypt.Do("吕亚娟"+"111111"); 
            a = Encrypt.Do("李丽"+"111111");
            a = Encrypt.Do("于奇"+"111111");
            a = Encrypt.Do("李卫民"+"111111");
            a = Encrypt.Do("于明凯"+"111111");
            a = Encrypt.Do("李玉莲"+"111111");
            a = Encrypt.Do("肇凯"+"111111");
            a = Encrypt.Do("黄长海"+"111111");
            a = Encrypt.Do("汪宏飞"+"111111");
            a = Encrypt.Do("高维慧"+"111111");
            a = Encrypt.Do("任闯"+"111111");
            a = Encrypt.Do("苏立东"+"111111");
            a = Encrypt.Do("于帅"+"111111");
            a = Encrypt.Do("onsite门槛"+"111111");
            a = Encrypt.Do("中深外库"+"111111");
            a = Encrypt.Do("张朔"+"111111");
            a = Encrypt.Do("任树平"+"111111");
            a = Encrypt.Do("刘永军"+"111111");
            a = Encrypt.Do("王永朕"+"111111");
            a = Encrypt.Do("谭志明"+"111111");
            a = Encrypt.Do("孙兆军"+"111111");
            a = Encrypt.Do("张梦媴"+"111111");
            a = Encrypt.Do("马建"+"111111");
            a = Encrypt.Do("F3X门槛A班"+"111111");
            a = Encrypt.Do("F3X门槛B班"+"111111");
            a = Encrypt.Do("F3X前保B班"+"111111");
            a = Encrypt.Do("F3X前保A班"+"111111");
            a = Encrypt.Do("F3X后保A班"+"111111");
            a = Encrypt.Do("F3X后保A班"+"111111");
            a = Encrypt.Do("高红伟"+"111111");
            a = Encrypt.Do("王天进"+"111111");
            a = Encrypt.Do("江帅"+"111111");
            a = Encrypt.Do("沈文博"+"111111");
            a = Encrypt.Do("李文超"+"111111");
            a = Encrypt.Do("赵建峰"+"111111");
            a = Encrypt.Do("注塑打贴"+"111111");
            a = Encrypt.Do("G2X上架"+"111111");
     
            return Json(Encrypt.Do(UserNO+"111111"));
        }

    }
}
