using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace Tops.FRM
{
    public static class ExcelHelp
    {
        #region [获取数据库所有表名]
        public static List<dataName> GetDataName()
        {
            List<dataName> list = new List<dataName>();
            string sql = "Select TABLE_NAME FROM TopsERP_BIZ.INFORMATION_SCHEMA.TABLES Where TABLE_TYPE='BASE TABLE' ";
            using (SqlDataReader dr = new DbHelperSQL(1).ExecuteReader(sql))
            {
                while (dr.Read())
                {
                    dataName itme = new dataName();
                    itme.Name = dr["TABLE_NAME"].ToString();
                    list.Add(itme);
                }
            }
            return list;
        }
        #endregion
        #region 根据表名查询数据
        public static DataTable GetNameData(string Name)
        {
            string sql = "SELECT * FROM [dbo].[" + Name + "]";
            DataSet ds = new DbHelperSQL(1).Query(sql);
            return ds.Tables[0];
        }
        #endregion
        #region 写入Excel
        public static void DataTabletoExcel(DataTable tmpDataTable, string strFileName)
        {
            if (tmpDataTable == null)
            {
                return;
            }
            int rowNum = tmpDataTable.Rows.Count;
            int columnNum = tmpDataTable.Columns.Count;
            int rowIndex = 1;
            int columnIndex = 0;
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlApp.DefaultFilePath = "D:\\";
            xlApp.DisplayAlerts = true;
            xlApp.SheetsInNewWorkbook = 1;
            Microsoft.Office.Interop.Excel.Workbook xlBook = xlApp.Workbooks.Add(true);
            //将DataTable的列名导入Excel表第一行
            foreach (DataColumn dc in tmpDataTable.Columns)
            {
                columnIndex++;
                xlApp.Cells[rowIndex, columnIndex] = dc.ColumnName;
            }
            //将DataTable中的数据导入Excel中
            for (int i = 0; i < rowNum; i++)
            {
                rowIndex++;
                columnIndex = 0;
                for (int j = 0; j < columnNum; j++)
                {
                    columnIndex++;
                    xlApp.Cells[rowIndex, columnIndex] = tmpDataTable.Rows[i][j].ToString();
                }
            }
            xlBook.SaveCopyAs(strFileName + ".xlsx");
        } 
        #endregion
        #region [查询数据库数据]
        public static DataTable GetSelWorkLog()
        {
            SqlParameter[] psra ={
                                 };
            DataSet ds = new DbHelperSQL(1).RunProcedure("proc_worklog",psra,"bt");
            return ds.Tables[0];
        } 
        #endregion
        #region [导出CSV]
        public static void ExportToSvc(DataTable dt, string strName)
        {
            string strPath = "D:\\" + strName + ".csv";
            if (File.Exists(strPath))
            {
                File.Delete(strPath);
            }
            //先打印标头
            StringBuilder strColu = new StringBuilder();
            StringBuilder strValue = new StringBuilder();
            int i = 0;
            StreamWriter sw = new StreamWriter(new FileStream(strPath, FileMode.CreateNew), Encoding.Default);
            for (i = 0; i <= dt.Columns.Count - 1; i++)
            {
                strColu.Append(dt.Columns[i].ColumnName);
                strColu.Append(",");
            }
            strColu.Remove(strColu.Length - 1, 1);//移出掉最后一个,字符
            sw.WriteLine(strColu);
            foreach (DataRow dr in dt.Rows)
            {
                strValue.Remove(0, strValue.Length);//移出
                for (i = 0; i <= dt.Columns.Count - 1; i++)
                {
                    strValue.Append(dr[i].ToString());
                    strValue.Append(",");
                }
                strValue.Remove(strValue.Length - 1, 1);//移出掉最后一个,字符
                sw.WriteLine(strValue);
            }
            sw.Close();
            //System.Diagnostics.Process.Start(strPath);
        } 
        #endregion
    }
}
public class dataName
{
    public string Name { get; set; }
}

