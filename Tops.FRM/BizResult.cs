using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Tops.FRM
{
    public class BizResult
    {
        /// <summary>
        /// 返回数据
        /// </summary>
        public Object Data { get; set; }

        public Object Rows { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 信息（通常用于错误信息返回）
        /// </summary>
        public List<string> Msgs { get;set; }
        /// <summary>
        /// 返回后跳转的URL
        /// </summary>
        public string  ToUrl { get; set; }
        /// <summary>
        /// 返回参数
        /// </summary>
        public Dictionary<string, Object> OutPut { get; set; }
        /// <summary>
        /// 备用字段1
        /// </summary>
        public Object Other1 { get; set; }
        /// <summary>
        /// 备用字段2
        /// </summary>
        public Object Other2 { get; set; } 
        public BizResult()
        {
            Msgs=new List<string>();
            OutPut = new Dictionary<string, object>();
        }
    }
}
