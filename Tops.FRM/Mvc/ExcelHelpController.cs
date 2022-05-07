using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Data.SqlClient;
using NPOI.SS.Util;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;

namespace Tops.FRM.Mvc
{
    public class ExcelHelpController : Controller
    {
        DbHelperSQL db = new DbHelperSQL();
        public ActionResult GetDataName()
        {
            return Json(ExcelHelp.GetDataName());
        }
        public ActionResult OutEeclx()
        {
            string name = Request.Form["dataName"];
            string flie = DateTime.Now.ToString("yyyyMMddHHmmss");
            DataTable dt = ExcelHelp.GetNameData(name);
            if (dt != null)
            {
                ExcelHelp.DataTabletoExcel(dt, flie);
                Response.Write("true");
            }
            else
            {
                Response.Write("false");
            }
            return null;
        }
        public ActionResult OutCsv()
        {
            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            DataTable dt = ExcelHelp.GetSelWorkLog();
            if (dt != null)
            {
                ExcelHelp.ExportToSvc(dt, fileName);
                Response.Write("true");
            }
            else
            {
                Response.Write("false");
            }
            return null;
        }


        /// <summary>
        /// Jane 2017-01-11 Excel下载
        /// </summary>
        /// <returns></returns>
        [Tops.FRM.Mvc.Filters.LoginFilter]
        public ActionResult Download()
        {

            try
            {

                DataSet DS = new DataSet();
                //数据源
                DataTable DataTB = new DataTable();
                //列宽度数据
                Dictionary<string, int> ColumnsWidthList = new Dictionary<string, int>();

                //拼接查询条件
                string str = Request.Form["exportcondition"];

                //查询设备使用率数据 
                DataTable RowList = SelInSearchObject(Request.Form["exportcolumn"], new SqlParameter[] { new SqlParameter() });

                SqlParameter[] spList = new SqlParameter[Request.Form.AllKeys.Length - 4];

                for (int index = 0, spindex = 0; index < Request.Form.AllKeys.Length; index++)
                {
                    if (Request.Form.AllKeys[index].IndexOf("exportdata") == -1 && Request.Form.AllKeys[index].IndexOf("exportcolumn") == -1 && Request.Form.AllKeys[index].IndexOf("exportsheetname") == -1 && Request.Form.AllKeys[index].IndexOf("exportcondition") == -1)
                    {
                        spList[spindex] = new SqlParameter("@" + Request.Form.AllKeys[index], Request.Form[index]);
                        spindex++;
                    }
                }

                DataTable DtList = SelInSearchObject(Request.Form["exportdata"], spList);

                //Datatable对象表名
                DataTB.TableName = Request.Form["exportsheetname"];

                //查询导出对象的列数据
                for (int i = 0; i < RowList.Rows.Count; i++)
                {
                    DataTB.Columns.Add(Convert.ToString(RowList.Rows[i][1]), typeof(string));
                    ColumnsWidthList.Add(Convert.ToString(RowList.Rows[i][1]), Convert.ToInt32(RowList.Rows[i][2]));
                }


                //查询导出对象的行数据（值数据）
                for (int i = 0; i < DtList.Rows.Count; i++)
                {
                    object[] obj = new object[DtList.Columns.Count];
                    for (int j = 0; j < DtList.Columns.Count; j++)
                    {
                        obj[j] = DtList.Rows[i][j].ToString();
                    }
                    DataTB.Rows.Add(obj);

                }



                //文档名称
                string FileName = Request.Form["exportsheetname"];

                DS.Tables.Add(DataTB);

                return File(ExportExecel(DataTB.TableName, str, ColumnsWidthList, DS), "application/vnd.ms-excel", FileName + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls");
            }
            catch (Exception ex)
            {
                return View();
            }

        }


        /// <summary>
        /// Jane 2018-04-08 Excel下载数据库设计
        /// </summary>
        /// <returns></returns> 
        public ActionResult DownloadSheetDesign()
        {

            try
            {

                DataSet DS = new DataSet();
                //数据源
                DataTable DataTB = new DataTable();

                //列宽度数据
                Dictionary<string, int> ColumnsWidthList = new Dictionary<string, int>();

                //查询已经编写好数据表名的表
                DataTable SheetList = SelInSearchObject("SelAllSheets", new SqlParameter[] { });

                //数据库表字段集合
                List<DataTable> DataLists = new List<DataTable>();
                //查询数据表数据 
                for (int index = 0; index < SheetList.Rows.Count; index++)
                {
                    DataLists.Add(SelInSearchObject("SelAllColumns", new SqlParameter[] { new SqlParameter("@Sheet", SheetList.Rows[index]["name"].ToString()) }));
                }

                //查询导出对象的列数据
                ColumnsWidthList.Add("表名", 45);
                ColumnsWidthList.Add("表说明", 15);
                ColumnsWidthList.Add("字段名", 15);
                ColumnsWidthList.Add("主键", 10);
                ColumnsWidthList.Add("类型", 10);
                ColumnsWidthList.Add("长度", 10);
                ColumnsWidthList.Add("允许空", 50);
                ColumnsWidthList.Add("默认值", 10);
                ColumnsWidthList.Add("字段说明", 35);
                DataTB.Columns.Add("表名");
                DataTB.Columns.Add("表说明");
                DataTB.Columns.Add("字段名");
                DataTB.Columns.Add("主键");
                DataTB.Columns.Add("类型");
                DataTB.Columns.Add("长度");
                DataTB.Columns.Add("允许空");
                DataTB.Columns.Add("默认值");
                DataTB.Columns.Add("字段说明");


                for (int index = 0; index < DataLists.Count; index++)
                {
                    //查询导出对象的行数据（值数据）
                    for (int i = 0; i < DataLists[index].Rows.Count; i++)
                    {
                        object[] obj = new object[DataLists[index].Columns.Count];
                        for (int j = 0; j < DataLists[index].Columns.Count; j++)
                        {
                            obj[j] = DataLists[index].Rows[i][j].ToString();
                        }
                        DataTB.Rows.Add(obj);
                    }
                }

                DS.Tables.Add(DataTB);


                //文档名称
                string FileName = "数据库设计";

                return File(ExportExcels(ColumnsWidthList, DS), "application/vnd.ms-excel", FileName + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls");
            }
            catch (Exception ex)
            {
                return View();
            }

        }


        /// <summary>
        /// 执行查询对象
        /// </summary>
        /// <param name="strName">查询对象</param>
        /// <param name="sp">参数</param> 
        /// <returns></returns>
        public DataTable SelInSearchObject(String bizName, SqlParameter[] sp)
        {
            //BizResult rs = new BizResult();
            //Biz biz = Biz.GetBiz(bizName);
            //if (biz == null)
            //    throw new Exception("[B-0002 找不到对象：" + bizName + "]");
            ////string sqlStr = Biz.GetSql(biz);
            //SqlParameter[] ps = Biz.GetSqlParameters(biz, request, method);


            Tops.FRM.BizResult br = Tops.FRM.Biz.Execute(bizName, sp);
            List<DataTable> dts = br.Data as List<DataTable>;
            DataTable dt = dts[0];
            return dt;
        }


        /// <summary>
        /// 导出Excel共用方法（数据库设计）
        /// </summary> 
        /// <param name="dic">列宽度数据</param>
        /// <param name="tempDs">数据源</param>
        /// <returns></returns>
        public byte[] ExportExcels(Dictionary<string, int> dic, DataSet tempDs)
        {
            try
            {
                HSSFWorkbook book = new HSSFWorkbook();
                //普通单元格样式
                ICellStyle cellstyle = book.CreateCellStyle();
                cellstyle.Alignment = HorizontalAlignment.CENTER; //水平居中
                cellstyle.VerticalAlignment = VerticalAlignment.CENTER;//垂直居中
                //cellstyle.WrapText = true; //自动换行
                IFont cellfont = book.CreateFont();
                //cellfont.FontHeightInPoints = 11; //11号字体
                cellstyle.SetFont(cellfont);

                //头部单元格样式
                ICellStyle cellheadstyle = book.CreateCellStyle();
                cellheadstyle.Alignment = HorizontalAlignment.CENTER;
                cellheadstyle.VerticalAlignment = VerticalAlignment.CENTER;
                IFont cellheadfont = book.CreateFont();
                cellheadstyle.SetFont(cellheadfont);
                //标题单元格样式
                ICellStyle titlestyle = book.CreateCellStyle();
                titlestyle.Alignment = HorizontalAlignment.CENTER;
                titlestyle.VerticalAlignment = VerticalAlignment.CENTER;
                IFont titlefont = book.CreateFont();
                titlestyle.SetFont(titlefont);

                int count = tempDs.Tables.Count;

                for (int i = 0; i < count; i++)
                {
                    DataTable dt = tempDs.Tables[i];
                    ISheet sheet = book.CreateSheet(dt.TableName);
                    sheet.IsPrintGridlines = true; //打印时显示网格线
                    sheet.DisplayGridlines = true;//查看时显示网格线

                    CellRangeAddress cellRangeAddress = new CellRangeAddress(0, 0, 0, 7);
                    sheet.AddMergedRegion(cellRangeAddress);
                    IRow prow = sheet.CreateRow(0);
                    prow = sheet.CreateRow(0);
                    prow.HeightInPoints = 25;


                    int rowIndex = 1;

                    for (int index = 0; index < dt.Columns.Count; index++)
                    {
                        sheet.SetColumnWidth(index, dic[dt.Columns[index].ColumnName] * 256);
                    }
                    //导入数据行
                    for (int k = 0, m = 0; k < dt.Rows.Count; k++, m++)
                    {
                        IRow row = sheet.CreateRow(m + rowIndex);
                        row.HeightInPoints = 25;
                        string ItemValue = dt.Rows[k][0].ToString();

                        //当如果是表记录则做单行处理
                        if (!string.IsNullOrEmpty(dt.Rows[k]["表名"].ToString()))
                        {
                            row.HeightInPoints = 25;
                            ICell sheetcell = row.CreateCell(0);
                            sheetcell.SetCellValue(dt.Rows[k]["表名"].ToString() + " [" + dt.Rows[k]["表说明"].ToString() + "]");
                            sheet.AddMergedRegion(new CellRangeAddress(m + rowIndex, m + rowIndex, 0, 7));
                            sheetcell.CellStyle = cellstyle;

                            m = m + 1;
                            row = sheet.CreateRow(m + rowIndex);
                            row.HeightInPoints = 25;
                            for (int index = 2, cellindex = 0; index < dt.Columns.Count; index++, cellindex++)
                            {
                                ICell cell = row.CreateCell(cellindex);
                                cell.SetCellValue(dt.Columns[index].ToString());
                                cell.CellStyle = cellstyle;
                            }

                            m = m + 1;
                            row = sheet.CreateRow(m + rowIndex);
                            row.HeightInPoints = 25;
                            for (int index = 2, colIndex = 0; index < dt.Columns.Count; index++, colIndex++)
                            {
                                ICell cell = row.CreateCell(colIndex);
                                cell.SetCellValue(dt.Rows[k][dt.Columns[index]].ToString());
                                cell.CellStyle = cellstyle;
                            }

                            continue;
                        }

                        for (int index = 2, colIndex = 0; index < dt.Columns.Count; index++, colIndex++)
                        {
                            ICell cell = row.CreateCell(colIndex);
                            cell.SetCellValue(dt.Rows[k][dt.Columns[index]].ToString());
                            cell.CellStyle = cellstyle;
                            //if (ItemValue != "")
                            //{
                            //    ICellStyle style1 = book.CreateCellStyle();

                            //    style1.Alignment = HorizontalAlignment.CENTER; //水平居中
                            //    style1.VerticalAlignment = VerticalAlignment.CENTER;//垂直居中
                            //    IFont cellfonts = book.CreateFont();
                            //    style1.SetFont(cellfonts);
                            //    row.CreateCell(colIndex).CellStyle = style1;
                            //} 

                        }
                    }
                }

                // 写入到客户端  
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                book.Write(ms);
                byte[] b = ms.ToArray();
                book = null;
                ms.Close();
                ms.Dispose();
                return b;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// 导出Excel共用方法
        /// </summary>
        /// <param name="tablestr">表格名称</param>
        /// <param name="searchstr">搜索条件</param>
        /// <param name="tempDs">数据源</param>
        /// <param name="dic">列宽度数据</param>
        /// <returns></returns>
        public byte[] ExportExecel(string tablestr, string searchstr, Dictionary<string, int> dic, DataSet tempDs)
        {
            HSSFWorkbook book = new HSSFWorkbook();
            //普通单元格样式
            ICellStyle cellstyle = book.CreateCellStyle();
            cellstyle.Alignment = HorizontalAlignment.CENTER; //水平居中
            cellstyle.VerticalAlignment = VerticalAlignment.CENTER;//垂直居中
            //cellstyle.WrapText = true; //自动换行
            IFont cellfont = book.CreateFont();
            //cellfont.FontHeightInPoints = 11; //11号字体
            cellstyle.SetFont(cellfont);

            //头部单元格样式
            ICellStyle cellheadstyle = book.CreateCellStyle();
            cellheadstyle.Alignment = HorizontalAlignment.CENTER;
            cellheadstyle.VerticalAlignment = VerticalAlignment.CENTER;
            IFont cellheadfont = book.CreateFont();
            cellheadstyle.SetFont(cellheadfont);
            //标题单元格样式
            ICellStyle titlestyle = book.CreateCellStyle();
            titlestyle.Alignment = HorizontalAlignment.CENTER;
            titlestyle.VerticalAlignment = VerticalAlignment.CENTER;
            IFont titlefont = book.CreateFont();
            titlestyle.SetFont(titlefont);
            int count = tempDs.Tables.Count;
            for (int i = 0; i < count; i++)
            {
                DataTable dt = tempDs.Tables[i];
                ISheet sheet = book.CreateSheet(dt.TableName);
                sheet.IsPrintGridlines = true; //打印时显示网格线
                sheet.DisplayGridlines = true;//查看时显示网格线

                CellRangeAddress cellRangeAddress = new CellRangeAddress(0, 0, 0, 10);
                sheet.AddMergedRegion(cellRangeAddress);
                IRow prow = sheet.CreateRow(0);
                prow = sheet.CreateRow(0);
                prow.HeightInPoints = 25;

                ICell pcell = prow.CreateCell(0);
                pcell.SetCellValue(tablestr);
                pcell.CellStyle = cellheadstyle;

                CellRangeAddress cellRangeAddresss = new CellRangeAddress(1, 1, 0, 10);
                sheet.AddMergedRegion(cellRangeAddresss);
                IRow prows = sheet.CreateRow(1);
                prow = sheet.CreateRow(1);
                prow.HeightInPoints = 25;

                ICell pcells = prow.CreateCell(0);
                pcells.SetCellValue(searchstr);
                pcells.CellStyle = cellheadstyle;

                int rowIndex = 3;
                int colIndex = 0;
                IRow row = sheet.CreateRow(2);
                row = sheet.CreateRow(2);
                row.HeightInPoints = 25;
                foreach (DataColumn col in dt.Columns)
                {
                    ICell cell = row.CreateCell(colIndex);
                    cell.SetCellValue(col.ColumnName);
                    cell.CellStyle = cellheadstyle;
                    sheet.SetColumnWidth(colIndex, dic[col.ColumnName] * 256);
                    colIndex++;
                }
                //导入数据行
                foreach (DataRow rows in dt.Rows)
                {
                    colIndex = 0;
                    row = sheet.CreateRow(rowIndex);
                    row.HeightInPoints = 25;
                    string ItemValue = dt.Rows[rowIndex - 3][0].ToString();
                    foreach (DataColumn col in dt.Columns)
                    {
                        ICell cell = row.CreateCell(colIndex);
                        cell.SetCellValue(rows[col.ColumnName].ToString());
                        cell.CellStyle = cellstyle;
                        //if (ItemValue != "")
                        //{
                        //    ICellStyle style1 = book.CreateCellStyle();

                        //    style1.Alignment = HorizontalAlignment.CENTER; //水平居中
                        //    style1.VerticalAlignment = VerticalAlignment.CENTER;//垂直居中
                        //    IFont cellfonts = book.CreateFont();
                        //    style1.SetFont(cellfonts);
                        //    row.CreateCell(colIndex).CellStyle = style1;
                        //}
                        colIndex++;
                    }
                    rowIndex++;
                }
            }
            // 写入到客户端  
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            byte[] b = ms.ToArray();
            book = null;
            ms.Close();
            ms.Dispose();
            return b;
        }
    }
}
