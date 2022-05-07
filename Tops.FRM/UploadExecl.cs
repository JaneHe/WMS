using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Collections;


namespace Tops.FRM
{
   public class UploadExecl
    {
       private string strDir = "/UploadExecl";
       public string PathAppSetting = "";
       public string pathName = "";
       string connString = "Data Source=121.37.59.31,1735;Initial Catalog=TopsERP_BIZ;User ID=sa;Password=GTA789yuntong;";
       public UploadExecl(string pathAppSetting)
       {
           PathAppSetting = pathAppSetting;
           strDir = System.Configuration.ConfigurationManager.AppSettings[pathAppSetting];
       }

       public BizResult UploadExeclData(HttpRequestBase Request, string FileName)
       {
          // string path = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
           pathName = HttpContext.Current.Server.MapPath(strDir) + FileName;
           BizResult br = new BizResult();
           //string path = "";
           Stream stream = null;
           int biaoji = 0;//1表示上传日需求，2表示上传长期需求，3表示上传库存
           int r = FileName.IndexOf(".");
           string TableName = "";
           string sheetName = "";
           try
           {
               if (Request.Files.Count > 0)
                   stream = Request.Files[0].InputStream;
               else
                   stream = Request.InputStream;
               if (string.IsNullOrEmpty((strDir + "").Trim()))
                   throw new Exception("[U001]找不到地址设置项：" + PathAppSetting);
               string fileExt = Path.GetExtension(FileName);
               
               if (stream.Length > 0)
               {
                   
                   FileStream fs = new FileStream(pathName, FileMode.OpenOrCreate);
                   using (BinaryWriter writer = new BinaryWriter(fs))
                   {
                       byte[] bf = new byte[stream.Length];
                       stream.Read(bf, 0, (Int32)stream.Length);
                       // 写入十进制数，字符串和字符
                       writer.Write(bf);
                   }                   
               }

           }
           catch (Exception)
           {              
               throw;
           }
           int RowCoun = 0;
           if (r == 4)
           {
               //执行日需求execl表
               
               TableName = "tDailyDemand";
               sheetName = "Daily Package$";
               biaoji = 1;
               RowCoun=AddTable(Request, pathName, FileName, TableName, sheetName, biaoji);
               if (RowCoun > 0)
               {
                   br.Data = RowCoun;
                   br.IsSuccess = true;
               }
               else
               {
                   br.IsSuccess = false;
                   br.Msgs.Add("该文档不符合导入的数据类型");
               }
               return br;
           }
           else if (FileName.IndexOf("LAB") > -1)
           {
               //执行LAB的execl表
               TableName = "tPlanDemand";
               sheetName = "F3X Standard Forecast$";
               biaoji =2;
               RowCoun = AddTable(Request, pathName, FileName, TableName, sheetName, biaoji);
               if (RowCoun > 0)
               {
                   br.Data = RowCoun;
                   br.IsSuccess = true;
               }
               else
               {
                   br.IsSuccess = false;
                   br.Msgs.Add("该文档不符合导入的数据类型");
               }
               return br;
           }
           else if (FileName.Contains("报表"))
           {
               //执行库存execl表
               TableName = "tStockManage";
               sheetName = "汇总$";
               biaoji = 3;
               RowCoun = AddTable(Request, pathName, FileName, TableName, sheetName, biaoji);
               if (RowCoun > 0)
               {
                   br.Data = RowCoun;
                   br.IsSuccess = true;
               }
               else
               {
                   br.IsSuccess = false;
                   br.Msgs.Add("该文档不符合导入的数据类型");
               }
               return br;
           }
           else
           {
               br.Msgs.Add("[U004]错误：所选择的文件不符合，请重新选择!");
               return br;
           }

       }
       
       public int AddTable(HttpRequestBase Request,string pathName,string FileName,string TableName,string sheetName,int biaoji)
       {
            DataTable dt = new DataTable();
            int row = 0;
           OleDbConnection objConn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pathName + ";" + "Extended Properties=Excel 12.0;"); //获取ExcelFileName 為excel路徑
           objConn.Open();
           try
           {
              
               DataTable schemaTable = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null); string A = "";
               if (sheetName == "汇总$")
               {
                   if (schemaTable.TableName.Contains("汇总$"))
                   {
                       sheetName = "汇总$";
                   }
                   else
                   {
                       sheetName = "总汇表$";
                   }
               }
               if (0 < schemaTable.Rows.Count)
               {
                   dt = DbHelperSQL.ExcelToDataTable(pathName, sheetName, true);

                   if (biaoji == 1)//执行读写日需求表
                   {
                       dt.Rows.RemoveAt(0);

                   }
                   else if (biaoji == 2)//执行读写长期需求表
                   {
                       int sumcolumns = dt.Columns.Count;
                       ArrayList list = new ArrayList();
                       for (int m = 8; m < sumcolumns; m++)
                       {
                           string aa = dt.Rows[0][m].ToString();
                           list.Add(aa);
                       }
                       for (int n = 0; n < sumcolumns - 8; n++)
                       {
                           dt.Columns.Add("date" + (n + 1)).SetOrdinal(9 + 2 * n);
                       }

                       dt.Rows.RemoveAt(0);
                       int ll = dt.Rows.Count;
                       int ls = ll - 1;
                       dt.Rows.RemoveAt(ls);

                       int count = dt.Rows.Count;
                       for (int i = 0; i < sumcolumns - 8; i++)
                       {
                           for (int j = 0; j < count; j++)
                           {
                               dt.Rows[j]["date" + (i + 1)] = list[i].ToString();
                           }
                       }
                   }
                   else if (biaoji == 3)//执行读写库存表
                   {
                       string l=dt.Rows[1][0].ToString();
                       if (l.Contains("存货编码"))
                       {
                           A = "YES";
                       }
                       else
                       {
                           A = "NO";
                       }
                       dt.Rows.RemoveAt(0);
                       dt.Rows.RemoveAt(0);
                       
                   }
                   else
                   {
                       return row=0;
                   }
                   int count1 = dt.Rows.Count;//查询DataTable数据的行数               
                   dt.Columns.Add("FileName");
                   dt.Columns.Add("s_Creator");
                   dt.Columns.Add("d_CreateTime");
                   dt.Columns.Add("s_Updator");
                   dt.Columns.Add("d_UpdaTime");

                   int length = FileName.IndexOf(".");
                   string Name = FileName.Substring(0, length);

                   for (int i = 0; i < count1; i++)
                   {
                       dt.Rows[i]["FileName"] = Name;
                       dt.Rows[i]["s_Creator"] = "admin";
                       dt.Rows[i]["d_CreateTime"] = DateTime.Now;
                       dt.Rows[i]["s_Updator"] = "admin";
                       dt.Rows[i]["d_UpdaTime"] = DateTime.Now;
                   }
                   int liecount = dt.Columns.Count;
                   if (liecount > 1)
                   {
                      
                       SqlBulkCopy sqlbulkcopy = new SqlBulkCopy(connString, SqlBulkCopyOptions.UseInternalTransaction);

                       sqlbulkcopy.DestinationTableName = TableName;//数据库中的表名  
                       if (biaoji == 1)
                       {
                           for (int i = 0; i < liecount; i++)
                           {
                               sqlbulkcopy.ColumnMappings.Add(dt.Columns[i].ColumnName, i + 1);//前面参数是excelDataTable中名称，后面是数据库字段名称
                           }
                       }
                       else if (biaoji == 2)
                       {
                           for (int i = 0; i < liecount - 5; i++)
                           {
                               sqlbulkcopy.ColumnMappings.Add(dt.Columns[i].ColumnName, i + 1);//前面参数是excelDataTable中名称，后面是数据库字段名称
                           }
                           for (int j = 5; j > 0; j--)
                           {
                               sqlbulkcopy.ColumnMappings.Add(dt.Columns[liecount - j].ColumnName, 194 - j);
                           }
                       }
                       else if (biaoji == 3)
                       {
                           if (A == "YES")
                           {
                               int count = dt.Rows.Count;
                               dt.Rows.RemoveAt(count - 2);
                               dt.Rows.RemoveAt(count - 2);
                               for (int i = 0; i < liecount - 5; i++)
                               {
                                   sqlbulkcopy.ColumnMappings.Add(dt.Columns[i].ColumnName, i + 1);//前面参数是excelDataTable中名称，后面是数据库字段名称
                               }
                               for (int j = 5; j > 0; j--)
                               {
                                   sqlbulkcopy.ColumnMappings.Add(dt.Columns[liecount - j].ColumnName, 17 - j);
                               }
                           }
                           else
                           {
                               int count = dt.Rows.Count;
                               dt.Rows.RemoveAt(count - 2);
                               dt.Rows.RemoveAt(count - 2);
                               for (int i = 0; i < liecount - 5; i++)
                               {
                                   sqlbulkcopy.ColumnMappings.Add(dt.Columns[i].ColumnName, i + 2);//前面参数是excelDataTable中名称，后面是数据库字段名称
                               }
                               for (int j = 5; j > 0; j--)
                               {
                                   sqlbulkcopy.ColumnMappings.Add(dt.Columns[liecount - j].ColumnName, 17 - j);
                               }
                           } 
                                                   
                       }
                       sqlbulkcopy.WriteToServer(dt);                     
                       sqlbulkcopy.Close();
                       string storedProcName = "up_AddSupplyAbility";
                       using (SqlConnection connection = new SqlConnection(connString))
                       {
                           connection.Open();
                           SqlCommand command = new SqlCommand(storedProcName, connection);
                           command.CommandTimeout = 180;
                           command.CommandType = CommandType.StoredProcedure;
                           SqlDataReader reader = command.ExecuteReader();                         
                       }
                       row = dt.Rows.Count;
                   }
               }
           }
           catch (SqlException ex)
           {
               throw ex;
           }
           finally
           {
               objConn.Close();
               objConn.Dispose();
               FileInfo fi = new FileInfo(pathName);
               if (fi.Exists)
               {
                   fi.Delete();
               }
           }
           return row;
       }
   }
      
}

