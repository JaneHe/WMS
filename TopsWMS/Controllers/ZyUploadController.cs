using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.IO;
using NPOI.SS.UserModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics; 
using Excel = Microsoft.Office.Interop.Excel;

namespace TopsHNAH.Controllers
{
    public class ZyUploadController : Controller
    {

        private string ConnectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString0"];

        private Microsoft.Office.Interop.Excel.Application workExcel = null;
        private Microsoft.Vbe.Interop.VBComponent vbComponent = null;
        private Microsoft.Office.Interop.Excel.Workbook workbook = null;
//        private string ChangeValuebyVBAStr = "Sub test()\r\n" +
//    "Dim ph$, fn$, sh As Worksheet\r\n" +
//    "ph = ThisWorkbook.Path & \"\\\" \r\n" +
//    "fn = Dir(ph & \"*.xls\")\r\n" +
//    "Do While fn <> \"\" \r\n" +
//        "If fn <> ThisWorkbook.Name Then\r\n" +
//            "With Workbooks.Open(ph & fn)\r\n" +
//                "For Each sh In .Sheets\r\n" +
//                    "sh.UsedRange = sh.UsedRange.Value\r\n" +
//                "Next\r\n" +
//                ".Close True\r\n" +
//            "End With\r\n" +
//        "End If\r\n" +
//        "fn = Dir\r\n" +
//    "Loop\r\n" +
//"End Sub";

//        private string AddExcelTrustbyVBAStr = "Sub sets()\r\n" +
//     "Dim strPw\r\n" +
//      "strPw = \"mst\"\r\n" +
//      "With Application\r\n" +
//          ".Windows.Application.CommandBars(1).Controls(6).Execute\r\n" +
//          ".SendKeys strPw \r\n" +
//          ".SendKeys (\"%v\")\r\n" +
//          ".SendKeys \"{ENTER}\"\r\n" +
//      "End With\r\n" +
//      "DoEvents\r\n" +
//"End Sub";

        #region 上传工单Excel文档 public void UploadExcel(string savepath, List<UploadObject> objlist, string bizName, Tops.FRM.BizType biztype = Tops.FRM.BizType.Query)
        /// <summary>
        /// 上传工单Excel文档
        /// </summary>
        /// <param name="savepath">文件完整路径</param>
        /// <param name="objlist">上传参数对象数组</param>
        /// <param name="queryName">查询对象名称或者业务对象名称</param>
        /// <param name="biztype">查询对象类型或者业务对象类型</param>
        public void UploadExcel(string savepath, List<UploadObject> objlist, string bizName, Tops.FRM.BizType biztype = Tops.FRM.BizType.Query)
        {
            IWorkbook workbook = null;
            using (FileStream fs = new FileStream(savepath, FileMode.Open, FileAccess.Read))
            {
                workbook = WorkbookFactory.Create(fs);
            }

            //默认获取第一个表格
            ISheet sheet = workbook.GetSheetAt(0);


            //创建一个Datatable存储excel数据
            DataTable dt = new DataTable();
            DataRow row = dt.NewRow();

            foreach (UploadObject upobj in objlist)
            {
                dt.Columns.Add(upobj.ColumnName, upobj.ColumnType);
                switch (upobj.ColumnType.ToString())
                {
                    case "System.String":
                        row[upobj.ColumnName] = upobj.ColumnValue.ToString();
                        break;
                    case "System.Int32":
                        row[upobj.ColumnName] = int.Parse(upobj.ColumnValue.ToString());
                        break;
                    case "System.Int16":
                        row[upobj.ColumnName] = float.Parse(upobj.ColumnValue.ToString());
                        break;
                    case "System.DateTime":
                        row[upobj.ColumnName] = DateTime.Parse(upobj.ColumnValue.ToString());
                        break;
                    default:
                        break;
                }

            }

            dt.Rows.Add(row);

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter();
                Tops.FRM.Biz biz = Tops.FRM.Biz.GetBiz(bizName, biztype);
                StringBuilder sqlsb = new StringBuilder();
                foreach (Tops.FRM.BizSql bizsql in biz.Sqls)
                {
                    sqlsb.Append(bizsql.Sql + "\r\n");
                }
                da.InsertCommand = new SqlCommand(sqlsb.ToString(), con);

                foreach (UploadObject upobj in objlist)
                {
                    SqlDbType sqlType;
                    int sqlTypeSize = 0;
                    switch (upobj.ColumnType.ToString())
                    {
                        case "System.String":
                            sqlType = SqlDbType.NVarChar;
                            sqlTypeSize = 500;
                            break;
                        case "System.Int32":
                            sqlType = SqlDbType.Int;
                            break;
                        case "System.Int16":
                            sqlType = SqlDbType.Float;
                            break;
                        case "System.DateTime":
                            sqlType = SqlDbType.DateTime;
                            break;
                        default:
                            return;
                    }
                    if (sqlTypeSize != 0)
                    {
                        da.InsertCommand.Parameters.Add(upobj.ColumnName, sqlType, sqlTypeSize);
                    }
                    else
                    {
                        da.InsertCommand.Parameters.Add(upobj.ColumnName, sqlType);
                    }
                    da.InsertCommand.Parameters[upobj.ColumnName].Value = upobj.ColumnValue;
                }

                da.Update(dt);
            }

        }

        #endregion

        #region 上传工单Excel文档 public void UploadExcel(string savepath, List<UploadObject> objlist, string bizsql)
        /// <summary>
        /// 上传工单Excel文档
        /// </summary>
        /// <param name="savepath">文件完整路径</param>
        /// <param name="objlist">上传参数对象数组</param>
        /// <param name="queryName">查询对象名称或者业务对象名称的执行sql</param> 
        public void UploadExcel(string savepath, List<UploadObject> objlist, string bizsql)
        {
            IWorkbook workbook = null;
            using (FileStream fs = new FileStream(savepath, FileMode.Open, FileAccess.Read))
            {
                workbook = WorkbookFactory.Create(fs);
            }

            //默认获取第一个表格
            ISheet sheet = workbook.GetSheetAt(0);



            //创建一个Datatable存储excel数据
            DataTable dt = new DataTable();
            DataRow row = dt.NewRow();

            foreach (UploadObject upobj in objlist)
            {
                dt.Columns.Add(upobj.ColumnName, upobj.ColumnType);
                switch (upobj.ColumnType.ToString())
                {
                    case "System.String":
                        row[upobj.ColumnName] = upobj.ColumnValue.ToString();
                        break;
                    case "System.Int32":
                        row[upobj.ColumnName] = int.Parse(string.IsNullOrEmpty(upobj.ColumnValue.ToString()) ? "0" : upobj.ColumnValue.ToString());
                        break;
                    case "System.Single":
                        row[upobj.ColumnName] = float.Parse(string.IsNullOrEmpty(upobj.ColumnValue.ToString()) ? "0" : upobj.ColumnValue.ToString());
                        break;
                    case "System.DateTime":
                        row[upobj.ColumnName] = DateTime.Parse(upobj.ColumnValue.ToString());
                        break;
                    default:
                        break;
                }

            }

            dt.Rows.Add(row);

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter();
                da.InsertCommand = new SqlCommand(bizsql, con);

                foreach (UploadObject upobj in objlist)
                {
                    SqlDbType sqlType;
                    int sqlTypeSize = 0;
                    switch (upobj.ColumnType.ToString())
                    {
                        case "System.String":
                            sqlType = SqlDbType.NVarChar;
                            sqlTypeSize = 500;
                            break;
                        case "System.Int32":
                            sqlType = SqlDbType.Int;
                            break;
                        case "System.Single":
                            sqlType = SqlDbType.Float;
                            break;
                        case "System.DateTime":
                            sqlType = SqlDbType.DateTime;
                            break;
                        default:
                            return;
                    }
                    if (sqlTypeSize != 0)
                    {
                        da.InsertCommand.Parameters.Add(upobj.ColumnName, sqlType, sqlTypeSize);
                    }
                    else
                    {
                        da.InsertCommand.Parameters.Add(upobj.ColumnName, sqlType);
                    }
                    da.InsertCommand.Parameters[upobj.ColumnName].Value = upobj.ColumnValue;
                }

                da.Update(dt);
            }

        }
        #endregion

        #region 控制宏进行文档进行公式变成数值的转换 public void OperateExcelChangeValueByVBA(string savepath)
        /// <summary>
        /// 控制宏进行文档进行公式变成数值的转换
        /// </summary>
        /// <param name="savepath">文件路径</param>
        //public void OperateExcelChangeValueByVBA(string savepath)
        //{

        //    try
        //    {
        //        //缺省值
        //        Object missing = System.Reflection.Missing.Value;

        //        workExcel = new Microsoft.Office.Interop.Excel.Application();
        //        //打开文档
        //        workbook = workExcel.Workbooks.Open(savepath);

        //        vbComponent = workbook.VBProject.VBComponents.Add(Microsoft.Vbe.Interop.vbext_ComponentType.vbext_ct_StdModule);

        //        //vbComponent.CodeModule.AddFromString(AddExcelTrustbyVBAStr);

        //        //app.Run("sets");
                 

        //        vbComponent.CodeModule.AddFromString(ChangeValuebyVBAStr);

        //        workExcel.Run("test", missing, missing,

        //               missing, missing, missing, missing, missing, missing, missing,

        //               missing, missing, missing, missing, missing, missing, missing,

        //               missing, missing, missing, missing, missing, missing, missing,

        //               missing, missing, missing, missing, missing, missing, missing);
                 

        //        workbook.Save();  
                
        //    }
        //    catch (Exception ex)
        //    { 
        //    }

        //    Process[] myproc = Process.GetProcesses();
        //    foreach (Process item in myproc)
        //    {
        //        if (item.ProcessName == "EXCEL")
        //        {
        //            item.Kill();
        //        }
        //    }

        //    vbComponent = null;
        //    workbook = null;
        //    workExcel = null;
        //    workbook.Close(); 

        //    GC.Collect();
        //    workExcel.Quit();
        //}
        #endregion

        #region 获取表格中对应位置的值 public string GetValueBySheet(ISheet Sheet, int RowIndex, int CellStartIndex, int CellEndIndex = 0)
        /// <summary>
        /// 获取表格中对应位置的值
        /// </summary>
        /// <param name="Sheet">表格对象</param>
        /// <param name="RowIndex">列位置</param>
        /// <param name="CellStartIndex">行开始位置</param>
        /// <param name="CellEndIndex">行结束位置</param>
        /// <returns></returns>
        public string GetValueBySheet(ISheet Sheet, int RowIndex, int CellStartIndex, int CellEndIndex = 0)
        {

            if (CellEndIndex == 0)
            {
                var value = Sheet.GetRow(RowIndex).GetCell(CellStartIndex);
                //if (value.CellType == CellType.NUMERIC && value.DateCellValue != DateTime.MinValue && (value.ToString().IndexOf("/") != -1 || value.ToString().IndexOf("月") != -1))
                //{
                //    return value.DateCellValue.ToString("yyyy-MM-dd").Trim();
                //}
                //else
                //{
                    return value.ToString();
                //}
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                for (int index = 0; index < CellEndIndex - CellStartIndex; index++)
                {
                    var value = Sheet.GetRow(RowIndex).GetCell(CellStartIndex + index);
                    if (value.CellType == CellType.NUMERIC && value.DateCellValue != DateTime.MinValue && (value.ToString().IndexOf("/") != -1 || value.ToString().IndexOf("月") != -1))
                    {
                        sb.Append(value.DateCellValue.ToString("yyyy-MM-dd").Trim());
                    }
                    else
                    {
                        sb.Append(value.ToString().Trim());
                    }
                }
                return sb.ToString();
            }

        }
        #endregion

        #region 保存Excel到本地指定项目中  public void SaveFileToExcel(string saveDirectory, string savefilename, Stream stream)
        /// <summary>
        /// 保存Excel到本地指定项目中
        /// </summary>
        /// <param name="savepath">文件路径</param> 
        /// <param name="savepath">文件名</param> 
        /// <param name="savepath">上传的文件流</param> 
        public void SaveFileToExcel(string saveDirectory, string savefilename, Stream stream)
        {
            try
            {
                if (!Directory.Exists(saveDirectory))
                {
                    Directory.CreateDirectory(saveDirectory);
                }

                FileStream filestream = new FileStream(saveDirectory + savefilename, FileMode.OpenOrCreate);
                using (BinaryWriter bw = new BinaryWriter(filestream))
                {
                    byte[] b = new byte[stream.Length];
                    stream.Read(b, 0, (int)stream.Length);
                    bw.Write(b);
                }
            }
            catch (Exception ex)
            {

            }
        }


        #endregion
    }

    #region 上传文件对象类 UploadObject
    /// <summary>
    /// 上传文件对象类
    /// </summary> 
    public class UploadObject
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public Type ColumnType { get; set; }

        /// <summary>
        /// 字段类型长度
        /// </summary>
        public int ColumnTypeSize { get; set; }


        private object columnValue;
        /// <summary>
        /// 字段值
        /// </summary>
        public object ColumnValue
        {
            get { return columnValue; }
            set
            {
                switch (this.ColumnName)
                {
                    case "IpAddress":
                        columnValue = Tops.FRM.TopsUser.IpAddress;
                        break;
                    case "UserAgent":
                        columnValue = Tops.FRM.TopsUser.AgentName;
                        break;
                    case "Operator":
                        columnValue = Tops.FRM.TopsUser.UserNO;
                        break;
                    default:
                        columnValue = value;
                        break;
                }
            }

        }
    }
    #endregion

 
    ///// <summary>
    ///// 执行Excel VBA宏帮助类
    ///// </summary>
    //public class ExcelMacroHelper
    //{
    //    /// <summary>
    //    /// 执行Excel中的宏
    //    /// </summary>
    //    /// <param name="excelFilePath">Excel文件路径</param>
    //    /// <param name="macroName">宏名称</param>
    //    /// <param name="parameters">宏参数组</param>
    //    /// <param name="rtnValue">宏返回值</param>
    //    /// <param name="isShowExcel">执行时是否显示Excel</param>
    //    public void RunExcelMacro(
    //                                       string excelFilePath,
    //                                        string macroName,
    //                                        object[] parameters,
    //                                        out object rtnValue,
    //                                        bool isShowExcel
    //                                    )
    //    {
    //        try
    //        {
    //            #region 检查入参

    //            // 检查文件是否存在
    //            if (!File.Exists(excelFilePath))
    //            {
    //                throw new System.Exception(excelFilePath + " 文件不存在");
    //            }

    //            // 检查是否输入宏名称
    //            if (string.IsNullOrEmpty(macroName))
    //            {
    //                throw new System.Exception("请输入宏的名称");
    //            }

    //            #endregion

    //            #region 调用宏处理

    //            // 准备打开Excel文件时的缺省参数对象
    //            object oMissing = System.Reflection.Missing.Value;

    //            // 根据参数组是否为空，准备参数组对象
    //            object[] paraObjects;

    //            if (parameters == null)
    //            {
    //                paraObjects = new object[] { macroName };
    //            }
    //            else
    //            {
    //                // 宏参数组长度
    //                int paraLength = parameters.Length;

    //                paraObjects = new object[paraLength + 1];

    //                paraObjects[0] = macroName;
    //                for (int i = 0; i < paraLength; i++)
    //                {
    //                    paraObjects[i + 1] = parameters[i];
    //                }
    //            }

    //            // 创建Excel对象示例
    //            Excel.ApplicationClass oExcel = new Excel.ApplicationClass();

    //            // 判断是否要求执行时Excel可见
    //            if (isShowExcel)
    //            {
    //                // 使创建的对象可见
    //                oExcel.Visible = true;
    //            }

    //            // 创建Workbooks对象
    //            Excel.Workbooks oBooks = oExcel.Workbooks;

    //            // 创建Workbook对象
    //            Excel._Workbook oBook = null;

    //            // 打开指定的Excel文件
    //            oBook = oBooks.Open(
    //                                    excelFilePath,
    //                                   oMissing,
    //                                    oMissing,
    //                                   oMissing,
    //                               oMissing,
    //                                    oMissing,
    //                                    oMissing,
    //                               oMissing,
    //                                   oMissing,
    //                                  oMissing,
    //                                   oMissing,
    //                                  oMissing,
    //                                   oMissing,
    //                                  oMissing,
    //                                   oMissing
    //                             );

    //            // 执行Excel中的宏
    //            rtnValue = this.RunMacro(oExcel, paraObjects);
    //            oBook.Application.DisplayAlerts = false;
    //            // 保存更改
    //            oBook.Save();

    //            // 退出Workbook
    //            oBook.Close(false, oMissing, oMissing);

    //            #endregion

    //            #region 释放对象

    //            // 释放Workbook对象
    //            System.Runtime.InteropServices.Marshal.ReleaseComObject(oBook);
    //            oBook = null;

    //            // 释放Workbooks对象
    //            System.Runtime.InteropServices.Marshal.ReleaseComObject(oBooks);
    //            oBooks = null;

    //            // 关闭Excel
    //            oExcel.Quit();

    //            // 释放Excel对象
    //            System.Runtime.InteropServices.Marshal.ReleaseComObject(oExcel);
    //            oExcel = null;

    //            // 调用垃圾回收
    //            GC.Collect();

    //            #endregion
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //    }

    //    /// <summary>
    //    /// 执行宏
    //    /// </summary>
    //    /// <param name="oApp">Excel对象</param>
    //    /// <param name="oRunArgs">参数（第一个参数为指定宏名称，后面为指定宏的参数值）</param>
    //    /// <returns>宏返回值</returns>
    //    private object RunMacro(object oApp, object[] oRunArgs)
    //    {
    //        try
    //        {
    //            // 声明一个返回对象
    //            object objRtn;

    //            // 反射方式执行宏
    //            objRtn = oApp.GetType().InvokeMember(
    //                                                    "Run",
    //                                                    System.Reflection.BindingFlags.Default |
    //                                                    System.Reflection.BindingFlags.InvokeMethod,
    //                                                    null,
    //                                                    oApp,
    //                                                    oRunArgs
    //                                                 );

    //            // 返回值
    //            return objRtn;

    //        }
    //        catch (Exception ex)
    //        {
    //            // 如果有底层异常，抛出底层异常
    //            if (ex.InnerException.Message.ToString().Length > 0)
    //            {
    //                throw ex.InnerException;
    //            }
    //            else
    //            {
    //                throw ex;
    //            }
    //        }
    //    }
    //}
 
}
