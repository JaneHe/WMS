using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using Tops.FRM;
using System.Text;
using System.Reflection;

namespace TopsCoating.Controllers
{
    public class JITManController : Controller
    {
        /// <summary>
        /// 批量插入缺陷（单面）（电位条码）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateJITMan(string param)
        {
            //先做两次反序列，并转化为参数实体类
            try
            {
                var obj = JsonConvert.DeserializeObject(param);
                List<ParamModel1> parammodellist = JsonConvert.DeserializeObject<List<ParamModel1>>(obj.ToString());

                foreach (ParamModel1 parammodel in parammodellist)
                {
                    SqlParameter[] sp4 = { 
                                         new SqlParameter("@n_Id", parammodel.n_Id),
                                         new SqlParameter("@s_AI",parammodel.s_AI),
                                         new SqlParameter("@n_Qty_Package",parammodel.n_Qty_Package),
                                         new SqlParameter("@n_Qty_Package1",parammodel.n_Qty_Package1)
                                      };
                    DataSet set = new Tops.FRM.DbHelperSQL(0).RunProcedure("up_UpdJITSTDBOM", sp4, "Add");
                }
            }
            catch (Exception ex)
            {
                return Json(new Tops.FRM.BizResult() { IsSuccess = false, Msgs = { ex.Message } });
            }
            return Json(new Tops.FRM.BizResult() { IsSuccess = true });
        }


        /// <summary>
        /// JIT件停用
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteJITMan(string param)
        {
            //先做两次反序列，并转化为参数实体类
            try
            {
                var obj = JsonConvert.DeserializeObject(param);
                List<ParamModel1> parammodellist = JsonConvert.DeserializeObject<List<ParamModel1>>(obj.ToString());

                foreach (ParamModel1 parammodel in parammodellist)
                {
                    SqlParameter[] sp4 = { 
                                         new SqlParameter("@n_Id", parammodel.n_Id)
                                      };
                    DataSet set = new Tops.FRM.DbHelperSQL(0).RunProcedure("up_DelJITSTDBOM", sp4, "Add");
                }
            }
            catch (Exception ex)
            {
                return Json(new Tops.FRM.BizResult() { IsSuccess = false, Msgs = { ex.Message } });
            }
            return Json(new Tops.FRM.BizResult() { IsSuccess = true });
        }


        ///// <summary>
        ///// 批量插入缺陷（单面）（电位条码）SYDD
        ///// </summary>
        ///// <param name="param"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public JsonResult AddDMQCProductByPolesSYDD(string param)
        //{
        //    //先做两次反序列，并转化为参数实体类
        //    try
        //    {
        //        var obj = JsonConvert.DeserializeObject(param);
        //        List<ParamModel> parammodellist = JsonConvert.DeserializeObject<List<ParamModel>>(obj.ToString());

        //        List<object> showMessage = new List<object>();

        //        foreach (ParamModel parammodel in parammodellist)
        //        {
        //            SqlParameter[] sp4 = { new SqlParameter("@BarCode",parammodel.BarCode.Replace("\n","")),
        //                             new SqlParameter("@State", parammodel.State),
        //                             new SqlParameter("@MisName",parammodel.MisName),
        //                             new SqlParameter("@MisType",int.Parse(parammodel.MisType)),
        //                             new SqlParameter("@QCUser",parammodel.QCUser),
        //                                 new SqlParameter("@Operator",Tops.FRM.TopsUser.UserNO),
        //                                 new SqlParameter("@WorkStation",parammodel.WorkStation),
        //                                 new SqlParameter("@cou",parammodel.cou), 
        //                                 new SqlParameter("@Class",parammodel.Class), 
        //                                 new SqlParameter("@MiserableTypeID",parammodel.MiserableTypeID),  
        //                                 new SqlParameter("@IsDDP",string.IsNullOrEmpty(parammodel.IsDDP)?"0":parammodel.IsDDP),
        //                                 new SqlParameter("@PackCode",string.IsNullOrEmpty(parammodel.PackCode)?string.Empty:parammodel.PackCode)};
        //            DataSet set = new Tops.FRM.DbHelperSQL(1).RunProcedure("up_AddDMQCProductByPolesSYDD", sp4, "AddQCProductByPoles");
        //            DataTable dt = set.Tables[0];
        //            int OKCou = int.Parse(dt.Rows[0]["OKCou"].ToString());
        //            if (OKCou <= 0)
        //            {
        //                showMessage.Add(new { barcode = parammodel.BarCode.Replace("\n", "") });
        //            }
        //        }
        //        return Json(new Tops.FRM.BizResult() { IsSuccess = true, Data = showMessage });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new Tops.FRM.BizResult() { IsSuccess = false, Msgs = { ex.Message } });
        //    }
        //}


        ////传递过来的参数
        //public class ParamModel
        //{
        //    public string MisName { get; set; }
        //    public string MisType { get; set; }
        //    public string BarCode { get; set; }
        //    public string QCUser { get; set; }
        //    public int State { get; set; }
        //    public int WorkStation { get; set; }
        //    public int cou { get; set; }
        //    public int Class { get; set; }
        //    public string FirstQCTime { get; set; }
        //    public string UpId { get; set; }
        //    public string MainPId { get; set; }
        //    public string PartPId { get; set; }
        //    public string MisCode { get; set; }
        //    public string MiserableTypeID { get; set; }
        //    public string IsDDP { get; set; }
        //    public string PackCode { get; set; }
        //}

        //传递过来的参数
        public class ParamModel1
        {
            public string n_Id { get; set; }
            public string s_AI { get; set; }
            public string n_Qty_Package { get; set; }
            public string n_Qty_Package1 { get; set; }
        }


    }
}
