using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web;
using SAP.Middleware.Connector;



namespace TopsHNAH.Controllers
{
    /// <summary>
    /// RFC配置类
    /// </summary>
    public class RFCConfig
    {
        /// <summary>
        /// //服务器名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 系统IP
        /// </summary>
        public string AppServerHost { get; set; }
        /// <summary>
        /// 客户端
        /// </summary>
        public string Client { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// 系统ID
        /// </summary>
        public string SystemID { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 系统编号
        /// </summary>
        public string SystemNumber { get; set; }
        /// <summary>
        /// 语言
        /// </summary>
        public string Language { get; set; }
        /// <summary>
        /// RFC方法
        /// </summary>
        public RFCCreateFunction RFCCreateFunction { get; set; }


        public RFCConfig()
        {
            this.Name = Name;
            this.AppServerHost = AppServerHost;
            this.Client = Client;
            this.User = User;
            this.SystemID = SystemID;
            this.Password = Password;
            this.SystemNumber = SystemNumber;
            this.Language = Language;
            this.RFCCreateFunction = RFCCreateFunction;
        }

        /// <summary>
        /// 配置RFC链接函数
        /// </summary>
        public RfcDestination ConfigurationFunction()
        {
            //下面开始建立连接
            RfcConfigParameters RCFParam = new RfcConfigParameters();
            RCFParam.Add(RfcConfigParameters.Name, this.Name);//服务器名称
            RCFParam.Add(RfcConfigParameters.AppServerHost, this.AppServerHost);//系统IP
            RCFParam.Add(RfcConfigParameters.Client, this.Client);//客户端
            RCFParam.Add(RfcConfigParameters.User, this.User);//用户名
            RCFParam.Add(RfcConfigParameters.SystemID, this.SystemID);//系统ID
            RCFParam.Add(RfcConfigParameters.Password, this.Password);//密码
            RCFParam.Add(RfcConfigParameters.SystemNumber, this.SystemNumber);//系统编号
            RCFParam.Add(RfcConfigParameters.Language, this.Language);//语言
            RCFParam.Add(RfcConfigParameters.PoolSize, "5");//连接池大小
            RCFParam.Add(RfcConfigParameters.MaxPoolSize, "10");//连接池最大限制
            RCFParam.Add(RfcConfigParameters.IdleTimeout, "500");//超时时间


            //完成设置
            RfcDestination dest = RfcDestinationManager.GetDestination(RCFParam);

            return dest;
        }
    }


    /// <summary>
    /// RFC方法类
    /// </summary>
    public class RFCCreateFunction
    {
        /// <summary>
        /// 方法名称
        /// </summary>
        public string FunctionName { get; set; }
        /// <summary>
        /// 表名称
        /// </summary>
        public List<string> TableNameList { get; set; }
        /// <summary>
        /// 输出参数数组
        /// </summary>
        public List<List<string>> OutPutList { get; set; }
        /// <summary>
        /// 输入参数数组
        /// </summary>
        public Dictionary<string, object> InPutList { get; set; }
    }


    public struct ZSPP_MESS_BG
    {
        //public char[] WERKS { get; set; }//工厂	
        //public char[] AUFNR { get; set; }//工单号码	
        //public char[] ZFDNO { get; set; }//分单号
        //public char[] VORNR { get; set; }//工序
        //public int LMNGA { get; set; }//报工数量
        //public int XMNGA { get; set; }//报废数量
        //public char[] MEINH { get; set; }///单位

        //public int ISM01 { get; set; }//机器工时
        //public char[] ILE01 { get; set; }//单位
        //public int ISM02 { get; set; }//人工工时
        //public char[] ILE02 { get; set; }//单位


        //public char[] ZMHNO { get; set; }//机台编号
        //public char[] ZFZR { get; set; }//操作员
        //public char[] ZREMARK { get; set; }//备注
        //public DateTime BUDAT { get; set; }//记帐日期

        public string WERKS { get; set; }//工厂	
        public string AUFNR { get; set; }//工单号码	
        public string ZFDNO { get; set; }//分单号
        public string VORNR { get; set; }//工序
        public int LMNGA { get; set; }//报工数量
        public int XMNGA { get; set; }//报废数量
        public string MEINH { get; set; }///单位

        public int ISM01 { get; set; }//机器工时
        public string ILE01 { get; set; }//单位
        public int ISM02 { get; set; }//人工工时
        public string ILE02 { get; set; }//单位
        public string ZMHNO { get; set; }//机台编号
        public string ZFZR { get; set; }//操作员
        public string ZREMARK { get; set; }//备注
        public DateTime BUDAT { get; set; }//记帐日期
        public string SAPAttributekey { get; set; }//工序代码
    }
}
