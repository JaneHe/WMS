using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Tops.FRM
{
    public class TopsModel
    {
        public ControllerContext ControllerContext { get; set; }
        public bool IsPublic { get; set; }
        public TopsModel(ControllerContext context)
        {
            ControllerContext = context;
        }
        public TopsModel(ControllerContext context,bool isPublic)
        {
            ControllerContext = context;
            IsPublic = isPublic;
        }
        /// <summary>
        /// 执行查询对象
        /// </summary>
        /// <param name="QueryName">查询对象名称</param>
        /// <param name="otherPs">其它参数,会覆盖请求中的参数</param>
        /// <returns></returns>
        public BizResult LoadQuery(string QueryName, SqlParameter[] otherPs = null, string sortname = "", string sortorder = "asc")
        {
            var rs = Biz.Execute(QueryName, ControllerContext.HttpContext.Request, BizType.Query, otherPs, BizMethod.GET, sortname, sortorder);
            if (!rs.IsSuccess)
                throw new Exception(rs.Msgs[0]);
            return rs;
        }
        /// <summary>
        /// 执行查询对象,获取结构
        /// </summary>
        /// <param name="QueryName">查询对象名称</param>
        /// <param name="otherPs">其它参数,会覆盖请求中的参数</param>
        /// <returns></returns>
        public BizResult LoadStruct(string QueryName)
        {
            var rs = Biz.Execute(QueryName, ControllerContext.HttpContext.Request, BizType.Query, null, BizMethod.GET);
            if (!rs.IsSuccess)
                throw new Exception(rs.Msgs[0]);
            return rs;
        }
        /// <summary>
        /// 执行业务操作
        /// </summary>
        /// <param name="BizName">对象名称</param>
        /// <param name="otherPs">其它参数,会覆盖请求中的参数</param>
        /// <returns></returns>
        public BizResult RunBiz(string BizName, SqlParameter[] otherPs = null)
        {
            return Biz.Execute(BizName, ControllerContext.HttpContext.Request, BizType.Business, otherPs,BizMethod.POST);
        }

    }
}
