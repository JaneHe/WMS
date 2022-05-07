using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.IO;
using Newtonsoft.Json;
namespace Tops.FRM.Controllers
{
    public class SMSController : Controller
    {
        /// <summary>
        /// 序列号
        /// </summary>
        private const string SerialNo = "3SDK-EGD-0130-MERRR";
        /// <summary>
        /// 密钥
        /// </summary>
        private const string Key = "144556";
        private smslocahost.SDKService sms = new smslocahost.SDKService();
        internal class SmsObject
        {
            /// <summary>
            /// 标题
            /// </summary>
            internal string Head = "";
            /// <summary>
            /// 正文
            /// </summary>
            internal string Content = "";
            /// <summary>
            /// 注脚
            /// </summary>
            internal string Foot = "【模塑科技信息管理系统】";
            /// <summary>
            /// 定时短信的定时时间，格式为:年年年年月月日日时时分分秒秒,例如:20090504111010 代表2009年5月4日 11时10分10秒，短信会在指定的时间发送出去
            /// sendTime值为空时，为即时发送短信
            /// sendTime值不为空时，为定时发送短信
            /// </summary>
            internal string SendTime = "";
            /// <summary>
            /// 手机号码(字符串数组,最多为200个手机号码)
            /// </summary>
            internal string[] Mobiles;
            /// <summary>
            /// 字符编码，默认为"GBK"
            /// </summary>
            internal string Charset = "GBK";
            /// <summary>
            /// 短信等级，范围1~5，数值越高优先级越高
            /// </summary>
            internal int Priority = 5;
            /// <summary>
            /// 短信ID
            /// </summary>
            internal long smsID = 0;


            /// <summary>
            /// 
            /// </summary>
            /// <param name="content">短信正文</param>
            /// <param name="mobiles">手机号码</param>
            internal SmsObject(string content, String[] mobiles)
            {
                Content = content;
                Mobiles = mobiles;
            }
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="so"></param>
        /// <returns>
        ///   0	    短信发送成功
        ///  17	    发送信息失败
        ///  18	    发送定时信息失败
        ///  101	客户端网络故障
        ///  305	服务器端返回错误，错误的返回值（返回值不是数字字符串）
        ///  307	目标电话号码不符合规则，电话号码必须是以0、1开头
        ///  997	平台返回找不到超时的短信，该信息是否成功无法确定
        ///  998	由于客户端网络问题导致信息发送超时，该信息是否成功下发无法确定
        /// </returns>
        internal int sendSms(SmsObject so)
        {
            int iResult;
            string SMScontext = so.Head + so.Content + so.Foot;
            iResult = sms.sendSMS(SerialNo, Key, so.SendTime, so.Mobiles, SMScontext, "", so.Charset, so.Priority,so.smsID);
            return iResult;
        }

        public void SSS()
        {
            sendSms(new SmsObject("恭喜发财", new string[] { "13570282764" }));
        }
        

        public string sendSMS()
        {
            BizResult br = new BizResult();
            string phone = Request.Params["phone"];
            string content = Request.Params["content"];
            int iResult = sendSms(new SmsObject(content, new string[] { phone }));

            if (iResult == 0)
            {
                br.IsSuccess = true;
                br.Msgs.Add("发送成功");
            }
            else
            {
                br.IsSuccess = false;
                br.Msgs.Add("发送失败");
            }

            string json = JsonConvert.SerializeObject(br);
            return json;
        }
    }
}