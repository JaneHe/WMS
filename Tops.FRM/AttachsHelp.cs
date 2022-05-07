using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Globalization;
using System.Reflection;
using System.Data;
using System.Net.Mime; 
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.Xml; 
using NPOI.HSSF.Util;
using System.Text.RegularExpressions;

namespace Tops.FRM
{
    public  class AttachsHelp
    {
        
        public  void SendMail()
        {
            

            //Timer T = new Timer(60000); //设置了每隔多长时间执行一次  

            //T.Elapsed += new ElapsedEventHandler(T_Elapsed);
            //T.AutoReset = true; //重置
            //T.Enabled = true;
        }


        int count = 0;

        /// <summary>
        /// 获取报表定时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                DateTime dt = DateTime.Now; //前一天
                //每天早上XX点发送邮件


                //if (dt.Hour == 08 && dt.Minute == 00)
                //{
                    SendResetPasswordEmail();//邮件发送
                //}

                count = 1;

              

            }
            catch (Exception ee)
            {
                new Tops.FRM.EmailErrorLog().writeInLog(ee + "####位置一 ########");
            }
        }


        /// <summary>
        /// 发邮件的代码
        /// </summary>
        /// <param></param>
        public void SendResetPasswordEmail()
        {
            EmailErrorLog email = new EmailErrorLog();
            try
            {
                
                string d_date = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
                string path0 = AppDomain.CurrentDomain.BaseDirectory + @"Send\\每日报表Excel\\" + DateTime.Now.ToString("yyyyMMdd");
                string path = AppDomain.CurrentDomain.BaseDirectory + @"Send\\每日报表Excel\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + d_date;

                //判断文件夹是否存在，不存在就重新创建文件夹
                if (!File.Exists(path0))
                {
                    //创建文件夹
                    Directory.CreateDirectory(path0);
                }

                string url1 = path + "-产量报表.xls";

                //生成Excel文件
                WriteExcel(url1, "up_SelProQulity", "产量报表", 1);
            }
            catch (Exception e)
            {
                email.writeInLog(e.Message);
            }
        }

        /// <summary>
        ///生成Excel文件
        /// </summary>
        /// <param name="url">文件路径</param>
        /// <param name="upName">存储过程名称</param>
        /// <param name="tableName1">临时表名</param>
        /// <param name="num">多少个sheet</param>

        public void WriteExcel(string url, string upName, string tableName1, int num)
        {


            if (!string.IsNullOrEmpty(url))
            {
                //HSSFWorkbook book = new HSSFWorkbook();



                //查询生产与品质日报数据信息

                DataTable table = new DataTable();

                DataTable Ptable = new DataTable();
                DataTable dtlistproduct = new DataTable();
                DataSet ds = null;
                //DataTable dt = null;
                SqlParameter[] sp = { };
                ds = new Tops.FRM.DbHelperSQL(0).RunProcedure(upName, sp, tableName1);
                dtlistproduct = ds.Tables[0];

                if (dtlistproduct.Rows.Count != 0)
                {
                    DataTable dtdetail = new DataTable();
                    table.TableName = "生产与品质日报统计";
                    for (int j = 0; j < dtlistproduct.Rows.Count; j++)//
                    {
                        table.Columns.Add(dtlistproduct.Rows[j][0].ToString(), typeof(string));
                    }
                    table.Columns.Add("合计", typeof(string));
                    for (int i = 1; i < dtlistproduct.Columns.Count; i++)//
                    {

                        decimal counts = 0;
                        decimal counts1 = 0;
                        decimal counts2 = 0;
                        object[] obj = new object[dtlistproduct.Rows.Count + 1];//+1合计
                        if (i % 4 == 0)
                        {
                            for (int j = 0; j < dtlistproduct.Rows.Count; j++)//
                            {
                                obj[j] = dtlistproduct.Rows[j][i].ToString();
                                counts1 = counts1 + Convert.ToDecimal(string.IsNullOrEmpty(dtlistproduct.Rows[j][i - 3].ToString()) ? "0" : dtlistproduct.Rows[j][i - 3].ToString());
                                counts2 = counts2 + Convert.ToDecimal(string.IsNullOrEmpty(dtlistproduct.Rows[j][i - 2].ToString()) ? "0" : dtlistproduct.Rows[j][i - 2].ToString());

                            }
                            if (counts2 != 0)
                            {
                                obj[dtlistproduct.Rows.Count] = (object)(counts2 / counts1).ToString("p");
                            }
                            table.Rows.Add(obj);
                            continue;
                        }

                        for (int j = 0; j < dtlistproduct.Rows.Count; j++)//
                        {
                            //obj[j] = dtlistproduct.Rows[j][i].ToString();
                            obj[j] = string.Format("{0:N}", dtlistproduct.Rows[j][i]);
                            counts = counts + Convert.ToDecimal(string.IsNullOrEmpty(dtlistproduct.Rows[j][i].ToString()) ? "0" : dtlistproduct.Rows[j][i].ToString());
                            //counts = counts + int.Parse(string.IsNullOrEmpty(dtlistproduct.Rows[j][i].ToString()) ? "0" : dtlistproduct.Rows[j][i].ToString());
                        }
                        //obj[dtlistproduct.Rows.Count] = (object)counts;
                        obj[dtlistproduct.Rows.Count] = string.Format("{0:N}", (object)counts);
                        table.Rows.Add(obj);


                        //decimal counts = 0;
                        //object[] obj = new object[dtlistproduct.Rows.Count+1];//+1合计
                        //for (int j = 0; j < dtlistproduct.Rows.Count; j++)//
                        //{
                        //    obj[j] = dtlistproduct.Rows[j][i].ToString();
                        //    counts = counts + Convert.ToDecimal(string.IsNullOrEmpty(dtlistproduct.Rows[j][i].ToString()) ? "0" : dtlistproduct.Rows[j][i].ToString());
                        //    //counts = counts + int.Parse(string.IsNullOrEmpty(dtlistproduct.Rows[j][i].ToString()) ? "0" : dtlistproduct.Rows[j][i].ToString());
                        //}
                        //obj[dtlistproduct.Rows.Count] =(object)counts;
                        //table.Rows.Add(obj);
                    }

                    DataTable dtdetailPtemp = new DataTable();


                    string FileName = "生产品质日报";
                    //string str = BeginDate.ToString("yyyy-MM-dd") + "活动日报";
                    string str = "查询条件：   " + "操作时间范围:" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + "至" + DateTime.Now.ToString("yyyy-MM-dd") + "产量统计";
                    //dtdetailPtemp.Columns.Add(str, typeof(string));
                    //tempDS.Tables.Add(dtdetail);
                    //PtempDs.Tables.Add(dtdetailPtemp);
                    DataSet tempDS = new DataSet();
                    DataSet PtempDs = new DataSet();
                    tempDS.Tables.Add(table);
                    PtempDs.Tables.Add(Ptable);
                    PtempDs.Tables.Add(dtdetailPtemp);
                    Ptable.Columns.Add(str, typeof(string));
                    dtdetailPtemp.Columns.Add(str, typeof(string));
                    //DefectExportExecel(tempDS, PtempDs);


                    HSSFWorkbook book = new HSSFWorkbook();
                    //普通单元格样式
                    ICellStyle cellstyle = book.CreateCellStyle();
                    cellstyle.Alignment = HorizontalAlignment.CENTER; //水平居中
                    cellstyle.VerticalAlignment = VerticalAlignment.CENTER;//垂直居中
                    IFont cellfont = book.CreateFont();
                    cellstyle.SetFont(cellfont);
                    //边框
                    cellstyle.BorderBottom = BorderStyle.THIN;
                    cellstyle.BorderLeft = BorderStyle.THIN;
                    cellstyle.BorderRight = BorderStyle.THIN;
                    cellstyle.BorderTop = BorderStyle.THIN;
                    cellstyle.BottomBorderColor = HSSFColor.BLACK.index;
                    cellstyle.LeftBorderColor = HSSFColor.BLACK.index;
                    cellstyle.RightBorderColor = HSSFColor.BLACK.index;
                    cellstyle.TopBorderColor = HSSFColor.BLACK.index;


                    ICellStyle cellstyle2 = book.CreateCellStyle();
                    cellstyle2.Alignment = HorizontalAlignment.CENTER; //水平居中
                    cellstyle2.VerticalAlignment = VerticalAlignment.CENTER;//垂直居中
                    IFont cellfont2 = book.CreateFont();
                    cellstyle.SetFont(cellfont2);
                    //边框
                    cellstyle2.BorderBottom = BorderStyle.THIN;
                    cellstyle2.BorderLeft = BorderStyle.THIN;
                    cellstyle2.BorderRight = BorderStyle.THIN;
                    cellstyle2.BorderTop = BorderStyle.THICK;
                    cellstyle2.BottomBorderColor = HSSFColor.BLACK.index;
                    cellstyle2.LeftBorderColor = HSSFColor.BLACK.index;
                    cellstyle2.RightBorderColor = HSSFColor.BLACK.index;
                    cellstyle2.TopBorderColor = HSSFColor.BLACK.index;

                    //头部单元格样式
                    ICellStyle cellheadstyle = book.CreateCellStyle();
                    cellheadstyle.Alignment = HorizontalAlignment.CENTER;
                    cellheadstyle.VerticalAlignment = VerticalAlignment.CENTER;
                    IFont cellheadfont = book.CreateFont();
                    cellheadstyle.SetFont(cellheadfont);

                    //边框
                    cellheadstyle.BorderBottom = BorderStyle.THIN;
                    cellheadstyle.BorderLeft = BorderStyle.THIN;
                    cellheadstyle.BorderRight = BorderStyle.THIN;
                    cellheadstyle.BorderTop = BorderStyle.THIN;
                    cellheadstyle.BottomBorderColor = HSSFColor.BLACK.index;
                    cellheadstyle.LeftBorderColor = HSSFColor.BLACK.index;
                    cellheadstyle.RightBorderColor = HSSFColor.BLACK.index;
                    cellheadstyle.TopBorderColor = HSSFColor.BLACK.index;
                    //标题单元格样式
                    ICellStyle titlestyle = book.CreateCellStyle();
                    titlestyle.Alignment = HorizontalAlignment.CENTER;
                    titlestyle.VerticalAlignment = VerticalAlignment.CENTER;
                    IFont titlefont = book.CreateFont();
                    titlestyle.SetFont(titlefont);

                    //合计 单元格背景颜色
                    ICellStyle colorStyle = book.CreateCellStyle();
                    colorStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.TURQUOISE.index; //背景颜色
                    //colorStyle.FillPattern = FillPattren.SolidForeground;
                    int count = tempDS.Tables.Count;
                    for (int i = 0; i < count; i++)
                    {
                        DataTable dt = tempDS.Tables[i];
                        DataTable pdt = PtempDs.Tables[i];
                        ISheet sheet = book.CreateSheet(dt.TableName);
                        sheet.IsPrintGridlines = true; //打印时显示网格线
                        sheet.DisplayGridlines = true;//查看时显示网格线

                        CellRangeAddress cellRangeAddress = new CellRangeAddress(0, 0, 0, dt.Columns.Count + 2);
                        sheet.AddMergedRegion(cellRangeAddress);
                        IRow prow = sheet.CreateRow(0);
                        prow = sheet.CreateRow(0);
                        prow.HeightInPoints = 30;
                        ICell cell6 = prow.CreateCell(0);
                        cell6.CellStyle = cellstyle;
                        ICell cell7 = prow.CreateCell(1);
                        cell7.CellStyle = cellstyle;
                        ICell cell8 = prow.CreateCell(2);
                        cell8.CellStyle = cellstyle;
                        foreach (DataColumn pcol in pdt.Columns)
                        {
                            ICell pcell = prow.CreateCell(0);
                            pcell.SetCellValue(pcol.ColumnName);
                            pcell.CellStyle = cellheadstyle;
                        }

                        int rowIndex = 2;
                        int colIndex = 3;
                        IRow row = sheet.CreateRow(1);
                        row = sheet.CreateRow(1);
                        row.HeightInPoints = 25;
                        //修改列的宽度
                        //IRow row2 = sheet.CreateRow(2);
                        //row2 = sheet.CreateRow(2);
                        //ICell cell0 = row2.CreateCell(0);
                        //cell0.SetCellValue("其它");
                        //cell0.CellStyle.Alignment = HorizontalAlignment.Left;
                        //cell0.CellStyle.VerticalAlignment = VerticalAlignment.Bottom;
                        //ICell cell3 = row2.CreateCell(2);
                        //cell3.SetCellValue("尺寸");
                        //cell3.CellStyle.Alignment = HorizontalAlignment.Right;
                        //cell3.CellStyle.VerticalAlignment = VerticalAlignment.Top;
                        foreach (DataColumn col in dt.Columns)
                        {
                            ICell cell = row.CreateCell(colIndex);
                            cell.SetCellValue(col.ColumnName);
                            cell.CellStyle = cellheadstyle;

                            sheet.SetColumnWidth(colIndex, 15 * 256);

                            colIndex++;
                        }
                        ICell cell5 = row.CreateCell(0);
                        cell5.CellStyle = cellstyle;
                        ICell cell3 = row.CreateCell(1);
                        cell3.CellStyle = cellstyle;
                        ICell cell4 = row.CreateCell(2);
                        cell4.CellStyle = cellstyle;

                        sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(2, 2, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(3, 3, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(4, 4, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(5, 5, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(2, 5, 0, 0));//钉卷

                        sheet.AddMergedRegion(new CellRangeAddress(6, 6, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(7, 7, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(8, 8, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(9, 9, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(6, 9, 0, 0));//焊接

                        sheet.AddMergedRegion(new CellRangeAddress(10, 10, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(11, 11, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(12, 12, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(13, 13, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(10, 13, 0, 0));//化成

                        sheet.AddMergedRegion(new CellRangeAddress(14, 14, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(15, 15, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(16, 16, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(17, 17, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(14, 17, 0, 0));//聚合

                        sheet.AddMergedRegion(new CellRangeAddress(18, 18, 1, 2));//组立
                        sheet.AddMergedRegion(new CellRangeAddress(19, 19, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(20, 20, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(21, 21, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(18, 21, 0, 0));

                        sheet.AddMergedRegion(new CellRangeAddress(22, 22, 1, 2));//清洗
                        sheet.AddMergedRegion(new CellRangeAddress(23, 23, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(24, 24, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(25, 25, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(22, 25, 0, 0));

                        sheet.AddMergedRegion(new CellRangeAddress(26, 26, 1, 2));//老化
                        sheet.AddMergedRegion(new CellRangeAddress(27, 27, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(28, 28, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(29, 29, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(26, 29, 0, 0));

                        sheet.AddMergedRegion(new CellRangeAddress(30, 30, 1, 2));//奈印
                        sheet.AddMergedRegion(new CellRangeAddress(31, 31, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(32, 32, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(33, 33, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(30, 33, 0, 0));

                        sheet.AddMergedRegion(new CellRangeAddress(34, 34, 1, 2));//测试
                        sheet.AddMergedRegion(new CellRangeAddress(35, 35, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(36, 36, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(37, 37, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(34, 37, 0, 0));

                        sheet.AddMergedRegion(new CellRangeAddress(38, 38, 1, 2));//剪脚
                        sheet.AddMergedRegion(new CellRangeAddress(39, 39, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(40, 40, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(41, 41, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(38, 41, 0, 0));

                        sheet.AddMergedRegion(new CellRangeAddress(42, 42, 1, 2));//编带
                        sheet.AddMergedRegion(new CellRangeAddress(43, 43, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(44, 44, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(45, 45, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(42, 45, 0, 0));

                        sheet.AddMergedRegion(new CellRangeAddress(46, 46, 1, 2));//外观
                        sheet.AddMergedRegion(new CellRangeAddress(47, 47, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(48, 48, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(49, 49, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(46, 49, 0, 0));

                        sheet.AddMergedRegion(new CellRangeAddress(50, 50, 1, 2));//外观
                        sheet.AddMergedRegion(new CellRangeAddress(51, 51, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(52, 52, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(53, 53, 1, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(50, 53, 0, 0));

                        //sheet.AddMergedRegion(new CellRangeAddress(37, 37, 0, 2));//WIP合计

                        //导入数据行
                        foreach (DataRow rows in dt.Rows)
                        {
                            colIndex = 3;
                            row = sheet.CreateRow(rowIndex);
                            row.HeightInPoints = 25;


                            if (rowIndex == 2)
                            {
                                ICell cell0 = row.CreateCell(0);
                                cell0.SetCellValue("钉卷");
                                cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell0.CellStyle = cellstyle2;
                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("投入数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle2;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle2;

                            }
                            else if (rowIndex == 3)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("良品数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 4)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("在制品数量（WIP）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 5)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("合格率（%）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                            }
                            if (rowIndex == 6)
                            {
                                //cellstyle.BorderTop = BorderStyle.THICK;

                                ICell cell0 = row.CreateCell(0);
                                cell0.SetCellValue("焊接");
                                cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell0.CellStyle = cellstyle2;
                                //cell0.CellStyle.BorderBottom = BorderStyle.THICK;
                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("投入数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle2;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle2;

                            }
                            else if (rowIndex == 7)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("良品数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 8)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("在制品数量（WIP）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 9)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("合格率（%）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                            }
                            if (rowIndex == 10)
                            {
                                ICell cell0 = row.CreateCell(0);
                                cell0.SetCellValue("化成");
                                cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell0.CellStyle = cellstyle2;
                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("投入数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle2;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle2;

                            }
                            else if (rowIndex == 11)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("良品数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 12)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("在制品数量（WIP）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 13)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("合格率（%）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                            }
                            if (rowIndex == 14)
                            {
                                ICell cell0 = row.CreateCell(0);
                                cell0.SetCellValue("聚合");
                                cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell0.CellStyle = cellstyle2;
                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("投入数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle2;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle2;

                            }
                            else if (rowIndex == 15)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("良品数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 16)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("在制品数量（WIP）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 17)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("合格率（%）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                            }
                            if (rowIndex == 18)
                            {
                                ICell cell0 = row.CreateCell(0);
                                cell0.SetCellValue("组立");
                                cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell0.CellStyle = cellstyle2;
                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("投入数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle2;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle2;

                            }
                            else if (rowIndex == 19)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("良品数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 20)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("在制品数量（WIP）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 21)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("合格率（%）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                            }

                            if (rowIndex == 22)
                            {
                                ICell cell0 = row.CreateCell(0);
                                cell0.SetCellValue("清洗");
                                cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell0.CellStyle = cellstyle2;
                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("投入数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle2;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle2;

                            }
                            else if (rowIndex == 23)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("良品数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 24)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("在制品数量（WIP）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 25)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("合格率（%）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                            }
                            if (rowIndex == 26)
                            {
                                ICell cell0 = row.CreateCell(0);
                                cell0.SetCellValue("老化");
                                cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell0.CellStyle = cellstyle2;
                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("投入数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle2;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle2;

                            }
                            else if (rowIndex == 27)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("良品数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 28)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("在制品数量（WIP）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 29)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("合格率（%）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                            }
                            if (rowIndex == 30)
                            {
                                ICell cell0 = row.CreateCell(0);
                                cell0.SetCellValue("奈印");
                                cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell0.CellStyle = cellstyle2;
                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("投入数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle2;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle2;

                            }
                            else if (rowIndex == 31)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("良品数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 32)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("在制品数量（WIP）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 33)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("合格率（%）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                            }
                            if (rowIndex == 34)
                            {
                                ICell cell0 = row.CreateCell(0);
                                cell0.SetCellValue("测试");
                                cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell0.CellStyle = cellstyle2;
                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("投入数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle2;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle2;

                            }
                            else if (rowIndex == 35)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("良品数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 36)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("在制品数量（WIP）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 37)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("合格率（%）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                            }

                            if (rowIndex == 38)
                            {
                                ICell cell0 = row.CreateCell(0);
                                cell0.SetCellValue("剪脚");
                                cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell0.CellStyle = cellstyle2;
                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("投入数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle2;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle2;

                            }
                            else if (rowIndex == 39)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("良品数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 40)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("在制品数量（WIP）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 41)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("合格率（%）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                            }
                            if (rowIndex == 42)
                            {
                                ICell cell0 = row.CreateCell(0);
                                cell0.SetCellValue("编带");
                                cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell0.CellStyle = cellstyle2;
                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("投入数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle2;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle2;

                            }
                            else if (rowIndex == 43)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("良品数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 44)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("在制品数量（WIP）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 45)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("合格率（%）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                            }
                            if (rowIndex == 46)
                            {
                                ICell cell0 = row.CreateCell(0);
                                cell0.SetCellValue("外观");
                                cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell0.CellStyle = cellstyle2;
                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("投入数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle2;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle2;
                            }
                            else if (rowIndex == 47)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("良品数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 48)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("在制品数量（WIP）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 49)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("合格率（%）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                            }
                            if (rowIndex == 50)
                            {
                                ICell cell0 = row.CreateCell(0);
                                cell0.SetCellValue("包装入库");
                                cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell0.CellStyle = cellstyle2;
                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("投入数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle2;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle2;

                            }
                            else if (rowIndex == 51)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("良品数");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 52)
                            {

                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("在制品数量（WIP）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle;
                            }
                            else if (rowIndex == 53)
                            {
                                ICell cell0 = row.CreateCell(0);
                                cell0.CellStyle = cellstyle;
                                ICell cell1 = row.CreateCell(1);
                                cell1.SetCellValue("合格率（%）");
                                cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                                cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                                cell1.CellStyle = cellstyle;
                                ICell cell2 = row.CreateCell(2);
                                cell2.CellStyle = cellstyle;
                            }
                            string ItemValue = dt.Rows[rowIndex - 2][0].ToString();
                            foreach (DataColumn col in dt.Columns)
                            {
                                ICell cell = row.CreateCell(colIndex);
                                cell.SetCellValue(rows[col.ColumnName].ToString());
                                if (rowIndex % 4 == 2)
                                {
                                    cell.CellStyle = cellstyle2;
                                }
                                else
                                {
                                    cell.CellStyle = cellstyle;
                                }


                                colIndex++;
                            }
                            rowIndex++;
                        }
                    }
                    //写入到客户端  
                    //System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    //book.Write(ms);
                    //byte[] b = ms.ToArray();
                    //book = null;
                    //ms.Close();
                    //ms.Dispose();
                    //return b;


                    // 写入项目的指定文件夹中  
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        book.Write(ms);
                        using (FileStream fs = new FileStream(url, FileMode.Create, FileAccess.Write))
                        {
                            byte[] data = ms.ToArray();
                            fs.Write(data, 0, data.Length);
                            fs.Flush();
                        }
                        book = null;
                    }
                }
            }
        }

        

        public void DefectExportExecel(DataSet tempDs, DataSet PtempDs)
        {
            HSSFWorkbook book = new HSSFWorkbook();
            //普通单元格样式
            ICellStyle cellstyle = book.CreateCellStyle();
            cellstyle.Alignment = HorizontalAlignment.CENTER; //水平居中
            cellstyle.VerticalAlignment = VerticalAlignment.CENTER;//垂直居中
            IFont cellfont = book.CreateFont();
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
                DataTable pdt = PtempDs.Tables[i];
                ISheet sheet = book.CreateSheet(dt.TableName);
                sheet.IsPrintGridlines = true; //打印时显示网格线
                sheet.DisplayGridlines = true;//查看时显示网格线

                CellRangeAddress cellRangeAddress = new CellRangeAddress(0, 0, 0, dt.Columns.Count + 2);
                sheet.AddMergedRegion(cellRangeAddress);
                IRow prow = sheet.CreateRow(0);
                prow = sheet.CreateRow(0);
                prow.HeightInPoints = 30;

                foreach (DataColumn pcol in pdt.Columns)
                {
                    ICell pcell = prow.CreateCell(0);
                    pcell.SetCellValue(pcol.ColumnName);
                    pcell.CellStyle = cellheadstyle;
                }

                int rowIndex = 2;
                int colIndex = 3;
                IRow row = sheet.CreateRow(1);
                row = sheet.CreateRow(1);
                row.HeightInPoints = 25;
                //修改列的宽度
                //IRow row2 = sheet.CreateRow(2);
                //row2 = sheet.CreateRow(2);
                //ICell cell0 = row2.CreateCell(0);
                //cell0.SetCellValue("其它");
                //cell0.CellStyle.Alignment = HorizontalAlignment.Left;
                //cell0.CellStyle.VerticalAlignment = VerticalAlignment.Bottom;
                //ICell cell3 = row2.CreateCell(2);
                //cell3.SetCellValue("尺寸");
                //cell3.CellStyle.Alignment = HorizontalAlignment.Right;
                //cell3.CellStyle.VerticalAlignment = VerticalAlignment.Top;
                foreach (DataColumn col in dt.Columns)
                {
                    ICell cell = row.CreateCell(colIndex);
                    cell.SetCellValue(col.ColumnName);
                    cell.CellStyle = cellheadstyle;

                    sheet.SetColumnWidth(colIndex, 15 * 256);

                    colIndex++;
                }
                sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 2));
                sheet.AddMergedRegion(new CellRangeAddress(2, 2, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(3, 3, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(4, 4, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(5, 5, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(2, 5, 0, 0));//钉卷

                sheet.AddMergedRegion(new CellRangeAddress(6, 6, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(7, 7, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(8, 8, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(9, 9, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(6, 9, 0, 0));//焊接

                sheet.AddMergedRegion(new CellRangeAddress(10, 10, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(11, 11, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(12, 12, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(13, 13, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(10, 13, 0, 0));//化成

                sheet.AddMergedRegion(new CellRangeAddress(14, 14, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(15, 15, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(16, 16, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(17, 17, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(14, 17, 0, 0));//聚合

                sheet.AddMergedRegion(new CellRangeAddress(18, 18, 1, 2));//组立
                sheet.AddMergedRegion(new CellRangeAddress(19, 19, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(20, 20, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(21, 21, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(18, 21, 0, 0));

                sheet.AddMergedRegion(new CellRangeAddress(22, 22, 1, 2));//清洗
                sheet.AddMergedRegion(new CellRangeAddress(23, 23, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(24, 24, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(25, 25, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(22, 25, 0, 0));

                sheet.AddMergedRegion(new CellRangeAddress(26, 26, 1, 2));//老化
                sheet.AddMergedRegion(new CellRangeAddress(27, 27, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(28, 28, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(29, 29, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(26, 29, 0, 0));

                sheet.AddMergedRegion(new CellRangeAddress(30, 30, 1, 2));//奈印
                sheet.AddMergedRegion(new CellRangeAddress(31, 31, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(32, 32, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(33, 33, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(30, 33, 0, 0));

                sheet.AddMergedRegion(new CellRangeAddress(34, 34, 1, 2));//测试
                sheet.AddMergedRegion(new CellRangeAddress(35, 35, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(36, 36, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(37, 37, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(34, 37, 0, 0));

                sheet.AddMergedRegion(new CellRangeAddress(38, 38, 1, 2));//剪脚
                sheet.AddMergedRegion(new CellRangeAddress(39, 39, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(40, 40, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(41, 41, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(38, 41, 0, 0));

                sheet.AddMergedRegion(new CellRangeAddress(42, 42, 1, 2));//编带
                sheet.AddMergedRegion(new CellRangeAddress(43, 43, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(44, 44, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(45, 45, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(42, 45, 0, 0));

                sheet.AddMergedRegion(new CellRangeAddress(46, 46, 1, 2));//外观
                sheet.AddMergedRegion(new CellRangeAddress(47, 47, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(48, 48, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(49, 49, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(46, 49, 0, 0));

                sheet.AddMergedRegion(new CellRangeAddress(50, 50, 1, 2));//外观
                sheet.AddMergedRegion(new CellRangeAddress(51, 51, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(52, 52, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(53, 53, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(50, 53, 0, 0));

                //sheet.AddMergedRegion(new CellRangeAddress(37, 37, 0, 2));//WIP合计

                //导入数据行
                foreach (DataRow rows in dt.Rows)
                {
                    colIndex = 3;
                    row = sheet.CreateRow(rowIndex);
                    row.HeightInPoints = 25;


                    if (rowIndex == 2)
                    {
                        ICell cell0 = row.CreateCell(0);
                        cell0.SetCellValue("钉卷");
                        cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("投入数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;

                    }
                    else if (rowIndex == 3)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("良品数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 4)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("在制品数量（WIP）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 5)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("合格率（%）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    if (rowIndex == 6)
                    {
                        ICell cell0 = row.CreateCell(0);
                        cell0.SetCellValue("焊接");
                        cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("投入数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;

                    }
                    else if (rowIndex == 7)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("良品数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 8)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("在制品数量（WIP）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 9)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("合格率（%）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    if (rowIndex == 10)
                    {
                        ICell cell0 = row.CreateCell(0);
                        cell0.SetCellValue("化成");
                        cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("投入数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;

                    }
                    else if (rowIndex == 11)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("良品数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 12)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("在制品数量（WIP）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 13)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("合格率（%）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    if (rowIndex == 14)
                    {
                        ICell cell0 = row.CreateCell(0);
                        cell0.SetCellValue("聚合");
                        cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("投入数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;

                    }
                    else if (rowIndex == 15)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("良品数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 16)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("在制品数量（WIP）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 17)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("合格率（%）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    if (rowIndex == 18)
                    {
                        ICell cell0 = row.CreateCell(0);
                        cell0.SetCellValue("组立");
                        cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("投入数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;

                    }
                    else if (rowIndex == 19)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("良品数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 20)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("在制品数量（WIP）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 21)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("合格率（%）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }

                    if (rowIndex == 22)
                    {
                        ICell cell0 = row.CreateCell(0);
                        cell0.SetCellValue("清洗");
                        cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("投入数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;

                    }
                    else if (rowIndex == 23)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("良品数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 24)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("在制品数量（WIP）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 25)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("合格率（%）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    if (rowIndex == 26)
                    {
                        ICell cell0 = row.CreateCell(0);
                        cell0.SetCellValue("老化");
                        cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("投入数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;

                    }
                    else if (rowIndex == 27)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("良品数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 28)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("在制品数量（WIP）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 29)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("合格率（%）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    if (rowIndex == 30)
                    {
                        ICell cell0 = row.CreateCell(0);
                        cell0.SetCellValue("奈印");
                        cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("投入数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;

                    }
                    else if (rowIndex == 31)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("良品数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 32)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("在制品数量（WIP）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 33)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("合格率（%）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    if (rowIndex == 34)
                    {
                        ICell cell0 = row.CreateCell(0);
                        cell0.SetCellValue("测试");
                        cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("投入数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;

                    }
                    else if (rowIndex == 35)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("良品数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 36)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("在制品数量（WIP）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 37)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("合格率（%）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }

                    if (rowIndex == 38)
                    {
                        ICell cell0 = row.CreateCell(0);
                        cell0.SetCellValue("剪脚");
                        cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("投入数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;

                    }
                    else if (rowIndex == 39)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("良品数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 40)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("在制品数量（WIP）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 41)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("合格率（%）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    if (rowIndex == 42)
                    {
                        ICell cell0 = row.CreateCell(0);
                        cell0.SetCellValue("编带");
                        cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("投入数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;

                    }
                    else if (rowIndex == 43)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("良品数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 44)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("在制品数量（WIP）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 45)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("合格率（%）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    if (rowIndex == 46)
                    {
                        ICell cell0 = row.CreateCell(0);
                        cell0.SetCellValue("外观");
                        cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("投入数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;

                    }
                    else if (rowIndex == 47)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("良品数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 48)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("在制品数量（WIP）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 49)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("合格率（%）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    if (rowIndex == 50)
                    {
                        ICell cell0 = row.CreateCell(0);
                        cell0.SetCellValue("包装入库");
                        cell0.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell0.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("投入数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;

                    }
                    else if (rowIndex == 51)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("良品数");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 52)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("在制品数量（WIP）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }
                    else if (rowIndex == 53)
                    {

                        ICell cell1 = row.CreateCell(1);
                        cell1.SetCellValue("合格率（%）");
                        cell1.CellStyle.Alignment = HorizontalAlignment.CENTER;
                        cell1.CellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                    }

                    string ItemValue = dt.Rows[rowIndex - 2][0].ToString();
                    foreach (DataColumn col in dt.Columns)
                    {
                        ICell cell = row.CreateCell(colIndex);
                        cell.SetCellValue(rows[col.ColumnName].ToString());
                        cell.CellStyle = cellstyle;

                        colIndex++;
                    }
                    rowIndex++;
                }
            }
            


        }



     
    }
}
