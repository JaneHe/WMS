using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tops.FRM;
using System.Data;
using System.Data.SqlClient;
using System.IO;


namespace Tops.FRM.Controllers
{
    public class CommonController : Controller
    {
        //
        // GET: /Common/
        [AcceptVerbs(HttpVerbs.Get)]
        [Tops.FRM.Mvc.Filters.PowerFilter(PowerType = Tops.FRM.Mvc.Filters.PowerType.CusModule)]
        public ActionResult Index()
        {
            var routeData = ControllerContext.RouteData;
            var CustController = routeData.Values["CustController"] == null ? "" : routeData.Values["CustController"].ToString();
            var Page = routeData.Values["Page"] == null ? "" : routeData.Values["Page"].ToString();
            routeData.Values["controller"] = CustController;
            routeData.Values["action"] = Page;
            return View(new TopsModel(ControllerContext));
        }
        [AcceptVerbs(HttpVerbs.Get)]
        [Tops.FRM.Mvc.Filters.PowerFilter(PowerType = Tops.FRM.Mvc.Filters.PowerType.ComModule)]
        public ActionResult q(string ModuleID)
        {
            var msg = new Object();
            if (!string.IsNullOrEmpty(ModuleID))
            {
                if (TopsUser.HasPower(ModuleID))
                {
                    Module module = Module.GetModulesByID(ModuleID);
                    if (module != null)
                    {
                        if (module.IsCom)
                        {
                            return View(module);
                        }
                        else
                            msg = "[Error-F003]模块：+" + ModuleID + "为非通用模块。";
                    }
                    else
                        msg = "[Error-F001]找不到模块：+" + ModuleID + "。";
                }
                else
                    msg = "[Error-P002]你没有权限进入该模块,请联系管理人员。";
            }
            else
                msg = "[Error-F002]非法链接。";
            return View("error", msg);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        [Tops.FRM.Mvc.Filters.PowerFilter(PowerType = Tops.FRM.Mvc.Filters.PowerType.ComModule)]
        public ActionResult d(string ModuleID)
        {
            var msg = new Object();
            if (!string.IsNullOrEmpty(ModuleID))
            {
                if (TopsUser.HasPower(ModuleID))
                {
                    Module module = Module.GetModulesByID(ModuleID);
                    if (module != null)
                    {
                        if (module.IsCom)
                        {
                            return View(module);
                        }
                        else
                            msg = "[Error-F1003]模块：+" + ModuleID + "为非通用模块。";
                    }
                    else
                        msg = "[Error-F1001]找不到模块：+" + ModuleID + "。";
                }
                else
                    msg = "[Error-P1002]你没有权限进入该模块,请联系管理人员。";
            }
            else
                msg = "[Error-F1002]非法链接。";
            return View("error", msg);
        }
        [Tops.FRM.Mvc.Filters.LoginFilter]
        public string GetSysModules()
        {
            BizResult br = new BizResult();
            br.IsSuccess = true;
            br.Data = Module.GetSysModules();
            return br.ToJson();
        }
        /// <summary>
        /// 用于查询文控系统菜单列表
        /// </summary>
        /// <returns></returns>
        [Tops.FRM.Mvc.Filters.LoginFilter]
        public string GetDMSysModules()
        {
            BizResult br = new BizResult();
            br.IsSuccess = true;
            br.Data = Module.GetDMSysModules();
            return br.ToJson();
        }

        /// <summary>
        /// 用于查询部门列表
        /// </summary>
        /// <returns></returns>
        public string GetDepSysModules()
        {
            BizResult br = new BizResult();
            br.IsSuccess = true;
            br.Rows = Module.GetDMSysModules();
            return br.ToJson();
        }

        /// <summary>
        /// 用于业务执行
        /// </summary>
        /// <param name="BizName"></param>
        /// <returns></returns>
        [Tops.FRM.Mvc.Filters.PowerFilter(PowerType = Tops.FRM.Mvc.Filters.PowerType.Biz)]
        [Tops.FRM.Mvc.Filters.ExceptionFilter]
        public string AjaxPost(string BizName)
        {
            var rs = new TopsModel(ControllerContext).RunBiz(BizName);
            return rs.ToJson();
        }

        /// <summary>
        /// 用于离线业务执行 HWJ
        /// </summary>
        /// <param name="BizName"></param>
        /// <returns></returns>
        [Tops.FRM.Mvc.Filters.ExceptionFilter]
        public string AjaxOffLinePost(string BizName)
        {
            //string OfflineBizName = BizName + "_OffLine";

            //Biz OffLineBiz = Biz.GetBiz("AddOffLineAjaxRecord", Tops.FRM.BizType.Business);
            //Biz biz = Biz.GetBiz(BizName, Tops.FRM.BizType.Business);

            //biz.Fields.AddRange(OffLineBiz.Fields);
            //biz.Checks.AddRange(OffLineBiz.Checks);
            //biz.Lookups.AddRange(OffLineBiz.Lookups);
            //biz.Sqls.AddRange(OffLineBiz.Sqls);
            //biz.BizName = OfflineBizName;


            //if (Biz.GetBiz(OfflineBizName, Tops.FRM.BizType.Business) == null)
            //{ 
            //    Biz.AddBiz(biz);
            //    Biz.SaveBiz(biz);
            //}
            //Biz.SaveBiz(biz);

            TopsModel TM = new TopsModel(ControllerContext); 
            BizResult br = TM.RunBiz("AddOffLineAjaxRecord");
            if (br.IsSuccess) {
                var rs = TM.RunBiz(BizName);
                return rs.ToJson();
            }
            return new BizResult() { IsSuccess = false, Msgs = br.Msgs }.ToJson();
        }


        /// <summary>
        /// 用于上传图片
        /// </summary>
        /// <param name="PathAppSetting">保存文件的路径，对应webconfig设置项</param>
        /// <returns></returns>
        [Tops.FRM.Mvc.Filters.LoginFilter]
        [Tops.FRM.Mvc.Filters.ExceptionFilter]
        public string Upload(string PathAppSetting, string FileName, string type = "photo")
        {
            var rs = new Object();

            string GetName = Path.GetExtension(FileName);
            if (GetName == ".xls" || GetName == ".xlsx")
            {

                rs = new Tops.FRM.UploadExecl(PathAppSetting).UploadExeclData(Request, FileName);
            }
            else
            {
                if (type == "file")
                    rs = new Tops.FRM.Upload(PathAppSetting) { allowExt = null }.UploadFile(Request, FileName);
                else
                    rs = new Tops.FRM.Upload(PathAppSetting).UploadPhoto(Request, FileName);
            }
            return rs.ToJson();
        }
        /// <summary>
        /// 用于上传文件
        /// </summary>
        /// <param name="PathAppSetting">保存文件的路径，对应webconfig设置项</param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        [Tops.FRM.Mvc.Filters.LoginFilter]
        [Tops.FRM.Mvc.Filters.ExceptionFilter]
        public string UploadFile(string PathAppSetting, string FileName)
        {
            var rs = new Object();
            string GetName = Path.GetExtension(FileName);
            var pathname = Request.Params["argument1"].ToString();
            rs = new Tops.FRM.UploadFile(PathAppSetting).UploadFileDocument(Request, FileName, pathname);

            return rs.ToJson();
        }


        /// <summary>
        /// 用于查询
        /// </summary>
        /// <param name="QueryName"></param>
        /// <returns></returns>
        [Tops.FRM.Mvc.Filters.PowerFilter(PowerType = Tops.FRM.Mvc.Filters.PowerType.Query)]
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
        [Tops.FRM.Mvc.Filters.PowerFilter(PowerType = Tops.FRM.Mvc.Filters.PowerType.Query)]
        [Tops.FRM.Mvc.Filters.ExceptionFilter]
        public string AjaxGridData(string QueryName, int page = 1, int pagesize = 20, string sortname = "", string sortorder = "asc")
        {
            SqlParameter[] ps = { 
                                    new SqlParameter("@beginIndex", SqlDbType.Int),
                                    new SqlParameter("@endIndex", SqlDbType.Int)
                                };
            ps[0].Value = (page - 1) * pagesize + 1;
            ps[1].Value = page * pagesize;
            var rs = new TopsModel(ControllerContext).LoadQuery(QueryName, ps, sortname, sortorder);
            List<DataTable> rows = null;
            if (rs.IsSuccess)
            {
                rows = rs.Data as List<DataTable>;
                if (rows.Count > 0)
                    rs.Data = rows[0];
                var rcTable = rows.Where(p => p.TableName == "QueryTable_RowsCount");
                if (rcTable.Count() > 0)
                {
                    DataTable dtRowsCount = rcTable.First();
                    if (dtRowsCount != null && dtRowsCount.Rows.Count > 0)
                    {
                        rs.Other1 = (!dtRowsCount.Columns.Contains("RowsCount")) ? 0 : dtRowsCount.Rows[0]["RowsCount"];
                        rs.Other2 = dtRowsCount.Rows[0].ToDictionary();
                    }
                    else
                    {
                        rs.Other1 = 0;
                    }
                }
                else
                {
                    rs.Other1 = rows[0].Rows.Count;
                }

            }

            return rs.ToJson();
        }


    }
}
