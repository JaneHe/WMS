using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Data;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace Tops.FRM.Controllers
{
    [Tops.FRM.Mvc.Filters.ToolAdminFilter]
    public class ToolsController : Controller
    {

        [Tops.FRM.Mvc.Filters.ToolsFilter]
        public ActionResult Index()
        {
            var routeData = ControllerContext.RouteData;
            var CustController = routeData.Values["CustController"] == null ? "" : routeData.Values["CustController"].ToString();
            var Page = routeData.Values["Page"] == null ? "" : routeData.Values["Page"].ToString();
            routeData.Values["controller"] = CustController;
            routeData.Values["action"] = Page;
            return View(new TopsModel(ControllerContext));
        }

        #region 业务/查询对象
        public string GetBizs(int Type = 1)
        {
            BizResult br = new BizResult();
            br.IsSuccess = true;
            br.Data = Biz.GetBizList((BizType)Type).ToList();
            return br.ToJson();
        }
        [Tops.FRM.Mvc.Filters.ToolsFilter]
        public string AddBiz(string BizName, bool IsSys, int Type = 1)
        {
            BizResult br = new BizResult();
            br.IsSuccess = Biz.AddBiz(new Biz() { BizName = BizName, IsSys = IsSys, Type = (BizType)Type });
            if (!br.IsSuccess)
                br.Msgs.Add("Biz:" + BizName + " 已经存在！");
            return br.ToJson();
        }
        public string GetQueries()
        {
            BizResult br = new BizResult();
            br.IsSuccess = true;
            br.Data = Biz.GetBizList(BizType.Query).ToList();
            return br.ToJson();
        }
        [Tops.FRM.Mvc.Filters.ToolsFilter]
        public string SaveBiz(string data, int Type = 1)
        {

            BizResult br = new BizResult();
            try
            {
                Biz biz = JsonConvert.DeserializeObject<Biz>(data);
                biz.Type = (BizType)Type;
                br.IsSuccess = Biz.SaveBiz(biz);
                if (!br.IsSuccess)
                    br.Msgs.Add("保存失败");
            }
            catch (Exception ex)
            {
                br.Msgs.Add(ex.Message);
            }
            return br.ToJson();
        }
        [Tops.FRM.Mvc.Filters.ToolsFilter]
        public string DelBiz(string BizName)
        {
            BizResult br = new BizResult();
            try
            {
                br.IsSuccess = Biz.DelBiz(BizName) >= 0;
                if (!br.IsSuccess)
                    br.Msgs.Add("删除失败");
            }
            catch (Exception ex)
            {
                br.Msgs.Add(ex.Message);
            }
            return br.ToJson();
        }
        [Tops.FRM.Mvc.Filters.ToolsFilter]
        public string RenameBiz(string OldBizName, string NewBizName)
        {
            BizResult br = new BizResult();
            try
            {
                br.IsSuccess = Biz.RenameBiz(OldBizName, NewBizName) >= 0;
                if (!br.IsSuccess)
                    br.Msgs.Add("重命名失败");
            }
            catch (Exception ex)
            {
                br.Msgs.Add(ex.Message);
            }
            return br.ToJson();
        }
        [Tops.FRM.Mvc.Filters.ToolsFilter]
        public string GetBiz(string BizName, int Type = 1)
        {

            var br = new BizResult();
            try
            {
                br.Data = Biz.GetBiz(BizName, (BizType)Type);
                br.IsSuccess = true;
            }
            catch (Exception ex)
            {
                br.Msgs.Add(ex.Message);
            }
            return br.ToJson();
        }
        /// <summary>
        /// 获取对象的信息 -- 查阅历史版本 JaneHe 20191029
        /// </summary>
        /// <param name="BizName"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        [Tops.FRM.Mvc.Filters.ToolsFilter]
        public string GetBizByOldVersion(string BizId, int Type = 1)
        {

            var br = new BizResult();
            try
            {
                //查询历史版本的记录
                var OldVersionBiz = Tops.FRM.Biz.Execute("SelDataVersionById", new System.Data.SqlClient.SqlParameter[] { new System.Data.SqlClient.SqlParameter { ParameterName = "Id", Value = BizId } }, Tops.FRM.BizType.Query);
                List<System.Data.DataTable> list = OldVersionBiz.Data as List<System.Data.DataTable>;
                string AI = list[0].Rows[0]["s_AI"].ToString();
                string BizNAme = list[0].Rows[0]["s_BizName"].ToString();

                br.Data = Biz.GetBizByOldVersion(BizId, BizNAme, AI, (BizType)Type);
                br.IsSuccess = true;
            }
            catch (Exception ex)
            {
                br.Msgs.Add(ex.Message);
            }
            return br.ToJson();
        }
        #endregion
        #region 模块
        public string GetModules(string ParentID = "0")
        {
            BizResult br = new BizResult();
            br.IsSuccess = true;
            br.Data = Module.GetModulesByParentID(ParentID);
            return br.ToJson();
        }
        public string GetSysModules(string ParentID = "0")
        {
            BizResult br = new BizResult();
            br.IsSuccess = true;
            br.Data = Module.GetSysModules("0", false);
            return br.ToJson();
        }
        public string GetModule(string ModuleID)
        {
            BizResult br = new BizResult();
            br.Data = Module.GetModulesByID(ModuleID);
            br.IsSuccess = br.Data == null ? false : true;
            return br.ToJson();
        }
        public string GetBiz0Query(string ModuleID, string QueryName, int QueryType = 1)
        {
            BizResult br = new BizResult();
            br.Data = Module.GetBiz0Query(QueryName, QueryType, ModuleID);
            br.IsSuccess = br.Data == null ? false : true;
            return br.ToJson();
        }

        #endregion
        #region 工具
        public ActionResult GetCodeHtml(string ViewName, string data)
        {
            CodeFactory cf = JsonConvert.DeserializeObject<CodeFactory>(data);
            return View(ViewName, cf);
        }
        public string GetDML(string TableName, int Type = 1)
        {
            BizResult br = new BizResult();
            br.Data = TopsTools.GetDML(TableName, Type);
            br.IsSuccess = true;
            return br.ToJson();
        }
        public string Export(string BizName)
        {
            var rs = "";
            var bizNames = BizName.Split(',');
            if (bizNames.Length > 0)
            {
                foreach (string name in bizNames)
                {
                    var model = Biz.GetBiz(name, BizType.Auto);
                    rs += @"
/*
对象：" + name + @"
时间：" + DateTime.Now.ToString() + @"
*/
";
                    if (model != null)
                    {
                        rs += @"" + Biz.Export(model);
                    }
                    else
                    {
                        rs += @"--找不到业务/查询对象：" + BizName + "!";
                    }
                }
            }
            return rs;
        }

        /// <summary>
        /// 导出某个模块的数据字典 2016-07-20 HWJ
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns> 
        public string ExportDictionary(string ModuleName)
        {

            SqlParameter[] sps = new SqlParameter[]{ 
                new SqlParameter(){ ParameterName="ModuleName",Value=ModuleName } 
            };
            BizResult br = Biz.Execute("Tool_SelExportDictionary_Jane", sps, BizType.Query);

            StringBuilder sb = new StringBuilder();

            if (br.IsSuccess)
            {
                List<DataTable> dtlist = br.Data as List<DataTable>;

                foreach (DataTable dt in dtlist)
                {
                    foreach (DataRow row in dt.Rows)
                    {

                        sb.AppendLine("INSERT INTO [dbo].[tDictionary]" +
           "([s_Field]" +
           ",[s_Scene]" +
           ",[s_Location]" +
           ",[s_Caption]" +
           ",[s_Type]" +
           ",[s_Format]" +
           ",[s_CommonID]" +
           ",[s_Sql]" +
           ",[s_Width]" +
           ",[s_ModuleName])" +
     "VALUES" +
     "('" + (string.IsNullOrEmpty(row["Field"].ToString()) ? "" : row["Field"].ToString()) + "','" +
                    (string.IsNullOrEmpty(row["Scene"].ToString()) ? "" : row["Scene"].ToString()) + "','" +
                    (string.IsNullOrEmpty(row["Location"].ToString()) ? "" : row["Location"].ToString()) + "','" +
                    (string.IsNullOrEmpty(row["Caption"].ToString()) ? "" : row["Caption"].ToString()) + "','" +
                    (string.IsNullOrEmpty(row["Type"].ToString()) ? "" : row["Type"].ToString()) + "','" +
                    (string.IsNullOrEmpty(row["Format"].ToString()) ? "" : row["Format"].ToString()) + "','" +
                    (string.IsNullOrEmpty(row["CommonID"].ToString()) ? "" : row["CommonID"].ToString()) + "','" +
                    (string.IsNullOrEmpty(row["Sql"].ToString()) ? "" : row["Sql"].ToString()) + "','" +
                    (string.IsNullOrEmpty(row["Width"].ToString()) ? "" : row["Width"].ToString()) + "','" +
                    (string.IsNullOrEmpty(row["ModuleName"].ToString()) ? "" : row["ModuleName"].ToString()) + "')");
                    }

                }
            }

            return sb.ToString();
        }

        #endregion
        /// <summary>
        /// 用于业务执行
        /// </summary>
        /// <param name="BizName"></param>
        /// <returns></returns>
        [Tops.FRM.Mvc.Filters.ExceptionFilter]
        public string AjaxPost(string BizName)
        {
            var rs = new TopsModel(ControllerContext).RunBiz(BizName);
            return rs.ToJson();
        }
        /// <summary>
        /// 用于查询
        /// </summary>
        /// <param name="QueryName"></param>
        /// <returns></returns>
        [Tops.FRM.Mvc.Filters.ExceptionFilter]
        public string AjaxGet(string QueryName)
        {
            var rs = new TopsModel(ControllerContext).LoadQuery(QueryName);
            return rs.ToJson();
        }
        /// <summary>
        /// 用于查询列表数据
        /// </summary>

        /// <param name="QueryName"></param>
        /// <param name="page">第几页，由1开始</param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [Tops.FRM.Mvc.Filters.ExceptionFilter]
        public string AjaxGridData(string QueryName, int page = 1, int pagesize = 10)
        {
            SqlParameter[] ps = { 
                                    new SqlParameter("@beginIndex", SqlDbType.Int),
                                    new SqlParameter("@endIndex", SqlDbType.Int)
                                };
            ps[0].Value = (page - 1) * pagesize + 1;
            ps[1].Value = page * pagesize;
            var rs = new TopsModel(ControllerContext).LoadQuery(QueryName, ps);
            List<DataTable> rows = null;
            if (rs.IsSuccess)
            {
                rows = rs.Data as List<DataTable>;
                rs.Data = rows[0];
                DataTable dtRowsCount = rows.Where(p => p.TableName == "QueryTable_RowsCount").First();
                rs.Other1 = (dtRowsCount == null ? 0 : dtRowsCount.Rows[0]["RowsCount"]);
            }

            return rs.ToJson();
        }



    }

}
