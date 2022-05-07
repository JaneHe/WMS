using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web;
using System.Data.SqlClient;

namespace Tops.FRM
{
    public class TopsCommon
    {
        /// <summary>
        /// SqlDbType转换为C#数据类型
        /// </summary>
        /// <param name="sqlType"></param>
        /// <returns></returns>
        public static Type SqlType2CsharpType2(SqlDbType sqlType)
        {
            switch (sqlType)
            {
                case SqlDbType.BigInt:
                    return typeof(Int64);
                case SqlDbType.Binary:
                    return typeof(Object);
                case SqlDbType.Bit:
                    return typeof(Boolean);
                case SqlDbType.Char:
                    return typeof(String);
                case SqlDbType.DateTime:
                    return typeof(DateTime);
                case SqlDbType.Decimal:
                    return typeof(Decimal);
                case SqlDbType.Float:
                    return typeof(Double);
                case SqlDbType.Image:
                    return typeof(Object);
                case SqlDbType.Int:
                    return typeof(Int32);
                case SqlDbType.Money:
                    return typeof(Decimal);
                case SqlDbType.NChar:
                    return typeof(String);
                case SqlDbType.NText:
                    return typeof(String);
                case SqlDbType.NVarChar:
                    return typeof(String);
                case SqlDbType.Real:
                    return typeof(Single);
                case SqlDbType.SmallDateTime:
                    return typeof(DateTime);
                case SqlDbType.SmallInt:
                    return typeof(Int16);
                case SqlDbType.SmallMoney:
                    return typeof(Decimal);
                case SqlDbType.Text:
                    return typeof(String);
                case SqlDbType.Timestamp:
                    return typeof(Object);
                case SqlDbType.TinyInt:
                    return typeof(Byte);
                case SqlDbType.Udt://自定义的数据类型
                    return typeof(Object);
                case SqlDbType.UniqueIdentifier:
                    return typeof(Object);
                case SqlDbType.VarBinary:
                    return typeof(Object);
                case SqlDbType.VarChar:
                    return typeof(String);
                case SqlDbType.Variant:
                    return typeof(Object);
                case SqlDbType.Xml:
                    return typeof(Object);
                default:
                    return null;
            }
        }
        public static Type fieldType2CsharpType(string sqlType)
        {
            switch (sqlType)
            {
                case "Int":
                    return typeof(Int64);
                case "Bit":
                    return typeof(Boolean);
                case "BeginDateTime":
                    return typeof(DateTime);
                case "EndDateTime":
                    return typeof(DateTime);
                case "DateTime":
                    return typeof(DateTime);
                case "Float":
                    return typeof(Decimal);
                case "String":
                    return typeof(String);
                default:
                    return null;
            }
        }


        /// <summary>
        /// sql server数据类型（如：varchar）
        /// 转换为SqlDbType类型
        /// </summary>
        /// <param name="sqlTypeString"></param>
        /// <returns></returns>
        public static SqlDbType SqlTypeString2SqlType(string sqlTypeString)
        {
            SqlDbType dbType = SqlDbType.Variant;//默认为Object

            switch (sqlTypeString)
            {
                case "int":
                    dbType = SqlDbType.Int;
                    break;
                case "varchar":
                    dbType = SqlDbType.VarChar;
                    break;
                case "bit":
                    dbType = SqlDbType.Bit;
                    break;
                case "datetime":
                    dbType = SqlDbType.DateTime;
                    break;
                case "decimal":
                    dbType = SqlDbType.Decimal;
                    break;
                case "float":
                    dbType = SqlDbType.Float;
                    break;
                case "image":
                    dbType = SqlDbType.Image;
                    break;
                case "money":
                    dbType = SqlDbType.Money;
                    break;
                case "ntext":
                    dbType = SqlDbType.NText;
                    break;
                case "nvarchar":
                    dbType = SqlDbType.NVarChar;
                    break;
                case "smalldatetime":
                    dbType = SqlDbType.SmallDateTime;
                    break;
                case "smallint":
                    dbType = SqlDbType.SmallInt;
                    break;
                case "text":
                    dbType = SqlDbType.Text;
                    break;
                case "bigint":
                    dbType = SqlDbType.BigInt;
                    break;
                case "binary":
                    dbType = SqlDbType.Binary;
                    break;
                case "char":
                    dbType = SqlDbType.Char;
                    break;
                case "nchar":
                    dbType = SqlDbType.NChar;
                    break;
                case "numeric":
                    dbType = SqlDbType.Decimal;
                    break;
                case "real":
                    dbType = SqlDbType.Real;
                    break;
                case "smallmoney":
                    dbType = SqlDbType.SmallMoney;
                    break;
                case "sql_variant":
                    dbType = SqlDbType.Variant;
                    break;
                case "timestamp":
                    dbType = SqlDbType.Timestamp;
                    break;
                case "tinyint":
                    dbType = SqlDbType.TinyInt;
                    break;
                case "uniqueidentifier":
                    dbType = SqlDbType.UniqueIdentifier;
                    break;
                case "varbinary":
                    dbType = SqlDbType.VarBinary;
                    break;
                case "xml":
                    dbType = SqlDbType.Xml;
                    break;
            }
            return dbType;
        }
        /// <summary>
        /// FieldType类型（如：String）
        /// 转换为SqlDbType类型
        /// </summary>
        /// <param name="fieldType"></param>
        /// <returns></returns>
        public static SqlDbType fieldTypeString2SqlType(string fieldType)
        {
            SqlDbType dbType = SqlDbType.Variant;//默认为Object
            fieldType = fieldType.ToLower();
            switch (fieldType)
            {
                case "int":
                    dbType = SqlDbType.Int;
                    break;
                case "string":
                    dbType = SqlDbType.VarChar;
                    break;
                case "bit":
                    dbType = SqlDbType.Bit;
                    break;
                case "datetime":
                    dbType = SqlDbType.DateTime;
                    break;
                case "float":
                    dbType = SqlDbType.Float;
                    break;
                case "begindatetime":
                    dbType = SqlDbType.DateTime;
                    break;
                case "enddatetime":
                    dbType = SqlDbType.DateTime;
                    break;
            }
            return dbType;
        }
        /// <summary>
        /// 获取一个SqlParameter参数中的SqlDbType枚举的类型,通过字符串名称
        /// </summary>
        /// <param name="ParamType">(GetStoreProcParams中获得的参数类型字符串)</param>
        /// <returns></returns>
        public static SqlDbType GetSqlDbType(string ParamType)
        {

            SqlDbType sqlDbType = new SqlDbType();
            foreach (string s in Enum.GetNames(sqlDbType.GetType()))
            {
                if (s == ParamType)
                {
                    return (SqlDbType)Enum.Parse(sqlDbType.GetType(), s);
                }

            }
            return SqlDbType.VarChar;

        }
        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static string GetSearchCondition(HttpRequestBase req)
        {
            StringBuilder sb = new StringBuilder();
            if (req != null && req.Form != null)
            {
                sb.Append("?");
                foreach (string key in req.Form.AllKeys)
                {
                    sb.Append(key + "=" + req.Form[key]);
                    sb.Append("&");
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 转换类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        public static Object ChangeType(Object value, string fieldType, string fieldName)
        {
            try
            {
                Type conversionType = TopsCommon.fieldType2CsharpType(fieldType);
                if (fieldType.EndsWith("DateTime") && string.IsNullOrEmpty(value + ""))
                {
                    if (fieldName.StartsWith("Begin"))
                        return "1900-01-01";
                    else if (fieldName.StartsWith("End"))
                        return DateTime.MaxValue;
                    else
                        return "2900-01-01";
                }
                else
                {
                    return Convert.ChangeType(value, conversionType);
                }
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// HWJ转换类型(开始时间BeginDateTime和结束时间EndDateTime)
        /// </summary> 
        /// <returns></returns>
        public static List<SqlParameter> ChangeDateType(List<SqlParameter> list, Dictionary<string, Object> dicRequest)
        {
            int StartIndex = -1;
            int EndIndex = -1;

            for (int index = 0; index < list.Count; index++)
            {
                if (list[index].ParameterName.StartsWith("@Begin"))
                {
                    StartIndex = index;
                }
                else if (list[index].ParameterName.StartsWith("@End"))
                {
                    EndIndex = index;
                }
            }
            if (StartIndex != -1 && EndIndex != -1)
            {

                if (dicRequest.Keys.Contains(list[StartIndex].ParameterName.Replace("@", "")) && dicRequest.Keys.Contains(list[EndIndex].ParameterName.Replace("@", "")))
                {
                    //如果填了开始时间，没有填入结束时间，则结束时间默认是开始时间的隔日
                    if (((!string.IsNullOrEmpty(list[StartIndex].Value.ToString()) && list[StartIndex].Value.ToString() != DateTime.MaxValue.ToString("yyyy/MM/dd HH:mm:ss") && list[StartIndex].Value.ToString() != "1900-01-01")
                        && (string.IsNullOrEmpty(list[EndIndex].Value.ToString()) || list[EndIndex].Value.ToString() == DateTime.MaxValue.ToString("yyyy/MM/dd HH:mm:ss")))
                        || (DateTime.Parse(list[StartIndex].Value.ToString()) > DateTime.Parse(list[EndIndex].Value.ToString()) && !string.IsNullOrEmpty(list[EndIndex].Value.ToString())))
                    {
                        list[EndIndex].Value = (DateTime.Parse(list[StartIndex].Value.ToString()).AddDays(1)).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
            }

            return list;
        }


        /// <summary>
        /// 把分号隔开的字符串变成数组
        /// </summary>
        /// <param name="fieldsStr"></param>
        /// <returns></returns>
        public static List<string> FieldsStr2List(string fieldsStr)
        {
            List<string> list = new List<string>();
            if (!string.IsNullOrEmpty(fieldsStr.Trim()))
            {
                var ar = fieldsStr.Split(';');
                if (ar.Length > 0)
                    list = ar.ToList<string>();
            }
            return list;
        }
        /// <summary>
        /// 把分号隔+等号隔开的字符串变成键值对
        /// </summary>
        /// <param name="fieldsStr"></param>
        /// <returns></returns>
        public static Dictionary<string, string> FieldsStr2KeyVal(string fieldsStr)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(fieldsStr.Trim()))
            {
                var ar = fieldsStr.Split(';');
                if (ar.Length > 0)
                {
                    for (int i = 0; i < ar.Length; i++)
                    {
                        var itemAr = ar[i].Split('=');
                        if (itemAr.Length == 2)
                        {
                            dic.Add(itemAr[0], itemAr[1]);
                        }
                    }
                }

            }
            return dic;
        }
    }
}
