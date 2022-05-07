using NPOI.SS.UserModel;
//using Spire.Xls.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TopsCoating.Controllers;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using TopsCoating.Controllers;

namespace TopsHNAH.Controllers
{
    public class SubmitController : Controller
    {
        public ActionResult ExcelSubmit() {
            return View();
        }


        public ActionResult SubmitProjectinfo()
        {
            return View();
        }

        private string connectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString1"];
        //上传对象
        private OrderUploadController zyupload = new OrderUploadController();

        //存储文件夹路径
        private string saveFolderPath = "UploadFile\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "\\";
          /// <summary>
          /// 文档上传方法
          /// </summary>
          /// <returns></returns>
          public JsonResult Upload()
          {
              DateTime getData = DateTime.Now.AddDays(-1);//获取当前时间的前一天
              var dateTime = Convert.ToDateTime(getData.ToString("yyyy-MM-dd"));
              HttpFileCollectionBase files = Request.Files;
              DataTable dtDaily = new DataTable();
              if (files.Count > 0)
              {
                  try
                  {
                      string savepath = "";
                      for (int index = 0; index < files.Count; index++)
                      {
                          #region 默认
                          Stream stream = files[index].InputStream;
                          savepath = AppDomain.CurrentDomain.BaseDirectory + saveFolderPath + files[0].FileName;
                          //保存到本地项目中
                          zyupload.SaveFileToExcel(AppDomain.CurrentDomain.BaseDirectory + saveFolderPath, files[0].FileName, stream);
                          IWorkbook workbook = null;
                          using (FileStream fs = new FileStream(savepath, FileMode.Open, FileAccess.Read))
                          {
                              workbook = WorkbookFactory.Create(fs);
                          }
                          int Number = 1;//表个数

                          DateTime getData1 = DateTime.Now;//获取当前时间
                          var plancode = getData1.ToString("yyyyMMddHHmmss");
                          for (int b = 0; b < Number; b++)
                          {
                              ISheet sheet = workbook.GetSheetAt(b);
                              //var titleName = zyupload.GetValueBySheet(sheet, 0, 0, 7);//标题名称
                              //if (titleName == "外库库存信息")
                              //{
                              var numbers = sheet.LastRowNum;
                              var rowsum = 0;
                          #endregion
                              dtDaily.Columns.Add("projectname");  // 前 为 Datatable 表名字段，后 为 数据库字段
                              //dtDaily.Columns.Add("series");
                              //dtDaily.Columns.Add("brand");
                              dtDaily.Columns.Add("isusestate");
                              dtDaily.Columns.Add("Creator");
                              dtDaily.Columns.Add("submitname");
                              dtDaily.Columns.Add("submitcode");
                              //循环的到每一条Excel数据
                              for (int i = 1; i < numbers ; i++)
                              {
                                  rowsum = i;
                                  if (zyupload.GetValueBySheet(sheet, rowsum, 0).Trim() == "")
                                  {
                                      continue;
                                  }
                                  #region 变量 与判断
                                  string projectname = zyupload.GetValueBySheet(sheet, rowsum, 0);//
                                  //string series = zyupload.GetValueBySheet(sheet, rowsum, 1);//

                                  //string brand = zyupload.GetValueBySheet(sheet, rowsum, 2);//
                                  string submitname = zyupload.GetValueBySheet(sheet, rowsum, 1);//
                                  string submitcode = zyupload.GetValueBySheet(sheet, rowsum, 2);//
                                  string isusestate = zyupload.GetValueBySheet(sheet, rowsum, 3);//
                                  if (isusestate == "正常使用") {
                                      isusestate = "1";
                                  }
                                  else if (isusestate == "调试中")
                                  {
                                      isusestate = "2";
                                  }
                                  else {
                                      isusestate = "3";
                                  }
                               
                                  string Creator = Tops.FRM.TopsUser.UserNO; //创建人
                                  #endregion
                                  DataRow dataRow = dtDaily.NewRow();


                                  dataRow["projectname"] = projectname;//
                                  //dataRow["series"] = series;//     
                                  //dataRow["brand"] = brand;//
                                  dataRow["isusestate"] = isusestate;//     
                                  dataRow["submitname"] = submitname;//    
                                  dataRow["submitcode"] = submitcode;//
                                  dataRow["Creator"] = Creator;//创建人
                                  dtDaily.Rows.Add(dataRow);
                              }
                              // }
                          }
                      }
                  }
                  catch (Exception ex)
                  {
                      return new JsonResult() { Data = new { IsSuccess = false, Msg = ex.Message } };
                  }
              }
              return new JsonResult() { Data = new { IsSuccess = true, Data = DataTableToJson(dtDaily) } };
          } /// <summary>
          /// 文档上传方法
          /// </summary>
          /// <returns></returns>
          public JsonResult Upload_1()
          {
              DateTime getData = DateTime.Now.AddDays(-1);//获取当前时间的前一天
              var dateTime = Convert.ToDateTime(getData.ToString("yyyy-MM-dd"));
              HttpFileCollectionBase files = Request.Files;
              DataTable dtDaily = new DataTable();
              if (files.Count > 0)
              {
                  try
                  {
                      string savepath = "";
                      for (int index = 0; index < files.Count; index++)
                      {
                          #region 默认
                          Stream stream = files[index].InputStream;
                          savepath = AppDomain.CurrentDomain.BaseDirectory + saveFolderPath + files[0].FileName;
                          //保存到本地项目中
                          zyupload.SaveFileToExcel(AppDomain.CurrentDomain.BaseDirectory + saveFolderPath, files[0].FileName, stream);
                          IWorkbook workbook = null;
                          using (FileStream fs = new FileStream(savepath, FileMode.Open, FileAccess.Read))
                          {
                              workbook = WorkbookFactory.Create(fs);
                          }
                          int Number = 1;//表个数

                          DateTime getData1 = DateTime.Now;//获取当前时间
                          var plancode = getData1.ToString("yyyyMMddHHmmss");
                          for (int b = 0; b < Number; b++)
                          {
                              ISheet sheet = workbook.GetSheetAt(b);
                              //var titleName = zyupload.GetValueBySheet(sheet, 0, 0, 7);//标题名称
                              //if (titleName == "外库库存信息")
                              //{
                              var numbers = sheet.LastRowNum;
                              var rowsum = 0;
                          #endregion
                              dtDaily.Columns.Add("s_ProjectName");  // 前 为 Datatable 表名字段，后 为 数据库字段
                              dtDaily.Columns.Add("s_SubmitName");
                              dtDaily.Columns.Add("s_PartNo");
                              dtDaily.Columns.Add("n_MeasurCount");
                              dtDaily.Columns.Add("n_QualifiedCount");
                              dtDaily.Columns.Add("n_Category");
                              dtDaily.Columns.Add("s_PlastivPartsDesc");
                              dtDaily.Columns.Add("s_Remark");
                              dtDaily.Columns.Add("s_Creator");
                              dtDaily.Columns.Add("d_CreateTime");
                              //循环的到每一条Excel数据
                              for (int i = 2; i < numbers; i++)
                              {
                                  rowsum = i;
                                  if (zyupload.GetValueBySheet(sheet, rowsum, 0).Trim() == "")
                                  {
                                      continue;
                                  }
                                  #region 变量 与判断
                                  string s_ProjectName = zyupload.GetValueBySheet(sheet, rowsum, 0);//
                                  string s_SubmitName = zyupload.GetValueBySheet(sheet, rowsum, 1);//

                                  string s_PartNo = zyupload.GetValueBySheet(sheet, rowsum, 2);//
                                  string n_MeasurCount = zyupload.GetValueBySheet(sheet, rowsum, 3);//
                                  string n_QualifiedCount = zyupload.GetValueBySheet(sheet, rowsum, 4);//
                                  string n_Category = zyupload.GetValueBySheet(sheet, rowsum, 5);//
                                  if (n_Category == "注塑件")
                                  {
                                      n_Category = "1";
                                  }
                                  else if (n_Category == "喷涂件")
                                  {
                                      n_Category = "2";
                                  }
                                  else
                                  {
                                      n_Category = "3";
                                  }

                                  string s_PlastivPartsDesc = zyupload.GetValueBySheet(sheet, rowsum, 6);//
                                  if (s_PlastivPartsDesc == "百分表")
                                  {
                                      s_PlastivPartsDesc = "1";
                                  }
                                  else
                                  {
                                      s_PlastivPartsDesc = "2";
                                  }
                                  string s_Remark = zyupload.GetValueBySheet(sheet, rowsum, 7);//
                                  string s_Creator = Tops.FRM.TopsUser.UserNO; //创建人
                                  #endregion
                                  DataRow dataRow = dtDaily.NewRow();


                                  dataRow["s_ProjectName"] = s_ProjectName;//
                                  dataRow["s_SubmitName"] = s_SubmitName;//     
                                  dataRow["s_PartNo"] = s_PartNo;//
                                  dataRow["n_MeasurCount"] = n_MeasurCount;//     
                                  dataRow["n_QualifiedCount"] = n_QualifiedCount;//    
                                  dataRow["n_Category"] = n_Category;// 
                                  dataRow["s_PlastivPartsDesc"] = s_PlastivPartsDesc;// 
                                  dataRow["s_Remark"] = s_Remark;//
                                  dataRow["s_Creator"] = s_Creator;//创建人
                                  dataRow["d_CreateTime"] = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"); ;//创建人
                                  dtDaily.Rows.Add(dataRow);
                              }
                              // }
                          }
                      }
                  }
                  catch (Exception ex)
                  {
                      return new JsonResult() { Data = new { IsSuccess = false, Msg = ex.Message } };
                  }
              }
              return new JsonResult() { Data = new { IsSuccess = true, Data = DataTableToJson(dtDaily) } };
          }

          /// <summary>
          /// DataTable to Json
          /// </summary>
          /// <param name="table"></param>
          /// <returns></returns>
          public string DataTableToJson(DataTable table)
          {
              var JsonString = new StringBuilder();
              if (table.Rows.Count > 0)
              {
                  JsonString.Append("[");
                  for (int i = 0; i < table.Rows.Count; i++)
                  {
                      JsonString.Append("{");
                      for (int j = 0; j < table.Columns.Count; j++)
                      {
                          if (j < table.Columns.Count - 1)
                          {
                              JsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                          }
                          else if (j == table.Columns.Count - 1)
                          {
                              JsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                          }
                      }
                      if (i == table.Rows.Count - 1)
                      {
                          JsonString.Append("}");
                      }
                      else
                      {
                          JsonString.Append("},");
                      }
                  }
                  JsonString.Append("]");
              }
              return JsonString.ToString();
          }

          /// <summary>
          /// 执行查询对象
          /// </summary>
          /// <param name="strName">查询对象</param>
          /// <param name="sp">参数</param>
          /// <returns></returns>
          public DataTable SelInWorkProduct(String strName, SqlParameter[] sp)
          {
              Tops.FRM.BizResult br = Tops.FRM.Biz.Execute(strName, sp);
              List<DataTable> dts = br.Data as List<DataTable>;
              DataTable dt = dts[0];
              return dt;
          }

    }
}
