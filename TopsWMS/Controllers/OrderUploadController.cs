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

namespace TopsCoating.Controllers
{
    public class OrderUploadController : Controller
    {
        //
        private string ConnectionString = Tops.FRM.DbHelperSQL.strDecryDES(System.Configuration.ConfigurationManager.AppSettings["ConnectionString1"], "tops828.");

        #region Excel文档 public void UploadExcel(string savepath, List<UploadObject> objlist, string bizName, Tops.FRM.BizType biztype = Tops.FRM.BizType.Query)
        /// <summary>
        /// 上传Excel文档
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

        #region 上传Excel文档 public void UploadExcel(string savepath, List<UploadObject> objlist, string bizsql)
        /// <summary>
        /// 上传Excel文档
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
                        row[upobj.ColumnName] = int.Parse(upobj.ColumnValue.ToString());
                        break;
                    case "System.Single":
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
                ICell icells = Sheet.GetRow(RowIndex).GetCell(CellStartIndex);
                if (icells != null)
                {
                    if (icells.CellType == CellType.NUMERIC && DateUtil.IsCellDateFormatted(icells))
                    {
                        return icells.DateCellValue.ToString();
                    }
                    if (icells.CellType == CellType.FORMULA)
                    {
                        return icells.NumericCellValue.ToString();
                    }
                    return Sheet.GetRow(RowIndex).GetCell(CellStartIndex).ToString();
                }
                return "";
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                for (int index = 0; index < CellEndIndex - CellStartIndex; index++)
                {

                    sb.Append(Sheet.GetRow(RowIndex).GetCell(CellStartIndex + index).ToString().Trim());
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
        /// 字段名称a
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
}
