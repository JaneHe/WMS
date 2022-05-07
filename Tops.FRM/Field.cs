using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Tops.FRM
{
    public class Field
    {
        public string FieldName { get; set; }
        public string Title { get; set; }
        public string FieldType { get; set; }
        /// <summary>
        /// 是否为返回值
        /// 例如：设置一个name参数为返回值
        /// 就必须在sql上写：select name=[值]
        /// </summary>
        public bool IsReturn { get; set; }
        public bool IsLoop { get; set; }
        public bool IsNull { get; set; }
        public int Index { get; set; }
        public string Sql { get; set; }
        public Dictionary<string,string> DataList {
            get
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                if(this.Sql.ToLower().IndexOf("select")>=0)
                {
                    DataSet ds = Biz.BizDbHelper.Query(this.Sql, null);
                    if (ds != null && ds.Tables.Count > 0&&ds.Tables[0].Columns.Contains("key") && ds.Tables[0].Columns.Contains("value"))
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            dic.Add(ds.Tables[0].Rows[i]["key"]+"", ds.Tables[0].Rows[i]["value"]+"");
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(this.Sql.Trim()))
                {
                    var arr=this.Sql.Trim().Split(';');
                    if (arr.Length > 0)
                    {
                        foreach (var item in arr)
                        {
                            var arrItem = item.Split('=');
                            if (arrItem.Length == 2)
                            {
                                dic.Add(arrItem[0], arrItem[1]);
                            }
                        }
                    }
                }
                return dic;
            }
            private set{}
        } 
        public Field()
        {
            FieldType = "String";
        }
        public static List<Field> GetFieldsByBizName(string bizName)
        {
            List<Field> list = new List<Field>() ;
            string sqlStr = @"
                            select s_FieldName as FieldName,
                            s_FieldType as FieldType,
                            b_IsReturn as IsReturn 
                            from FRM_Field
                            where s_BizName=@BizName";
            SqlParameter[] ps = { 
                                    new SqlParameter("@BizName", SqlDbType.VarChar)
                                };
            ps[0].Value = bizName;
            DataSet ds = Biz.FrmDbHelper.Query(sqlStr, ps);
            if (ds != null && ds.Tables.Count > 0)
            {
                list = new List<Field>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Field model = new Field();
                    model.FieldName = ds.Tables[0].Rows[i]["FieldName"] + "";
                    model.FieldType = ds.Tables[0].Rows[i]["FieldType"] + "";
                    model.IsReturn = Convert.ToBoolean(ds.Tables[0].Rows[i]["IsReturn"]);
                    list.Add(model);
                }
            }
            return list;
        }
        


        
    }
}
