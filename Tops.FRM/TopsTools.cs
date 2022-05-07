using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace Tops.FRM
{
            
    public class CodeFactory{
        public string QueryName{get;set;}
        public string CodeType{get;set;}
        public bool IsSearch{get;set;}
        public bool IsAdd{get;set;}
        public bool IsExport{ get; set; }
        public bool IsPrint{get;set;} 
        public bool IsSave{get;set;}
        public bool IsEdit{get;set;}
        public bool IsCancel{get;set;}
        public bool IsDel{get;set;}
        public bool IsCheck{get;set;}
        public string BizAdd { get; set; }
        public string BizModify{get;set;}
        public string BizDel{get;set;}
        public string BizCheck{get;set;}
        public string ModifyUrl { get; set; }
        public string Title { get; set; }
    }
    public class TopsTools
    {
        /// <summary>
        /// 生成执行语句
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="mdlType"></param>
        /// <returns></returns>
        public static string GetDML(string tableName,int mdlType)
        {
            string sqlRs="";
            string sqlStr = @"
select 
FieldName=c.name,
[Type]=t.name,
Length=c.length ,
IsNull=c.isnullable
from syscolumns c  
join systypes t on c.xtype = t.xusertype
join sysobjects o on o.id=c.id 
where o.xtype='U' and o.name=@TableName
";
            SqlParameter[] ps = { 
                                    new SqlParameter("@TableName", SqlDbType.VarChar)
                                };
            ps[0].Value = tableName;
            DataSet ds = Biz.BizDbHelper.Query(sqlStr, ps);
            if (ds != null && ds.Tables.Count > 0)
            {
                var dt=ds.Tables[0];
                var fieldName="";
                switch (mdlType) {
                    case 1: //Select
                        {
                            sqlRs += @"
select";
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                fieldName=dt.Rows[i]["FieldName"]+"";
                                if (i > 0)
                                    sqlRs += ",";
                                sqlRs += @"
    " + fieldName.Substring(2) + "=" + fieldName;
                            }
                            sqlRs += @"
from " + tableName;
                            break;
                        }
                    case 2://Insert
                        {
                            sqlRs += @"
insert into "+ tableName+" (";
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                fieldName = dt.Rows[i]["FieldName"] + "";
                                if (i > 0)
                                    sqlRs += ",";
                                sqlRs += @"
    "+fieldName;
                            }
                            sqlRs += @"
)values (";
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                fieldName = dt.Rows[i]["FieldName"] + "";
                                if (i > 0)
                                    sqlRs += ",";
                                sqlRs += @"
    @:" + fieldName.Substring(2);
                            }
                            sqlRs += @"
)";
                            break;
                        }
                    case 3://Update
                        {
                            sqlRs += @"
update "+tableName+" set ";
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                fieldName = dt.Rows[i]["FieldName"] + "";
                                if (i > 0)
                                    sqlRs += ",";
                                sqlRs += @"
    " + fieldName + "=@:" + fieldName.Substring(2);
                            }
                            break;
                        }
                }
            }
            return sqlRs;
        }
        
    }
}
