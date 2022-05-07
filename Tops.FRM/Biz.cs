using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data;
using System.Web;

namespace Tops.FRM
{
    public enum BizMethod { POST, GET }
    public enum BizType { Query = 1, Business = 2, Auto = 0 }
    public class BakFileInfo
    {
        public int FileCount { get; set; }
        public string FileName { get; set; }
        public string FileCreateTime { get; set; }
    }
    public class BizSql
    {
        public int Item { get; set; }
        public string Name { get; set; }
        public string Sql { get; set; }
        public string Key { get; set; }
        public string LoopField { get; set; }
        public bool IsRun { get; set; }
    }
    public class BizCheck
    {
        public int Item { get; set; }
        public string Name { get; set; }
        public string Sql { get; set; }
        public bool IsRun { get; set; }
        public string CheckField { get; set; }
    }
    public class Biz
    {
        public string BizName { get; set; }
        public List<BizCheck> Checks { get; set; }
        public List<BizSql> Sqls { get; set; }
        public BizType Type { get; set; }
        public bool IsSys { get; set; }
        public List<Field> Fields { get; set; }
        public List<Lookup> Lookups { get; set; }
        public string CountSql { get; set; }
        public string HiddenFields { get; set; }
        public string Scenes { get; set; }
        public string ReadOnly4Adds { get; set; }
        public string ReadOnly4Edits { get; set; }
        public string NotNulls { get; set; }
        public string Remarks { get; set; }
        public Dictionary<string, string> DicScenes
        {
            get
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(this.Scenes.Trim()))
                {
                    var arr = this.Scenes.Split(';');
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
            private set { }
        }
        public static DbHelperSQL FrmDbHelper = new DbHelperSQL(1);
        public static DbHelperSQL BizDbHelper = new DbHelperSQL(0);
        public Biz()
        {
            Fields = new List<Field>();
            Sqls = new List<BizSql>();
            Checks = new List<BizCheck>();
        }

        #region Biz管理
        /// <summary>
        /// 保存业务对象
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool AddBiz(Biz model)
        {
            string sqlStr = @"
                            if not exists(select 1 from FRM_Biz where s_BizName=@BizName)
                            begin
                                Insert into FRM_Biz(
                                s_BizName,n_Type,b_IsSys
                                ) values (@BizName,@Type,@IsSys)
                            end
                            ";
            SqlParameter[] ps = { 
                                    new SqlParameter("@BizName", SqlDbType.VarChar),
                                    new SqlParameter("@Type", SqlDbType.Int),
                                    new SqlParameter("@IsSys", SqlDbType.Bit)
                                };
            int rowsCount = 0;
            ps[0].Value = model.BizName.Trim();
            ps[1].Value = model.Type;
            ps[2].Value = model.IsSys;
            rowsCount = Biz.FrmDbHelper.ExecuteSql(sqlStr, ps);
            return rowsCount > 0;
        }
        /// <summary>
        /// 保存业务对象
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool SaveBiz(Biz model)
        {
            string sqlStr = Biz.Export(model);
            int rowsCount = 0;
            rowsCount = Biz.FrmDbHelper.ExecuteSqlTran(sqlStr); ;
            return rowsCount > 0;
        }
        public static int DelBiz(string bizName)
        {
          
            string sqlStr = string.Format(@"
  
                --删除相关表
                delete from FRM_Biz where s_BizName='{0}'
                delete from FRM_Check where s_BizName='{0}'
                delete from FRM_Sql where s_BizName='{0}'
                delete from FRM_Field where s_BizName='{0}'
                delete from FRM_Lookup where s_BizName='{0}'
                ", bizName);
            int rowsCount = 0;
            rowsCount = Biz.FrmDbHelper.ExecuteSqlTran(sqlStr);
            return rowsCount;
        }
        public static int RenameBiz(string oldBizName, string newBizName)
        {
            string sqlStr = string.Format(@"
 
                --删除相关表
                update FRM_Biz set s_BizName='{1}' where s_BizName='{0}'
                update FRM_Check set  s_BizName='{1}' where s_BizName='{0}'
                update FRM_Field set  s_BizName='{1}' where s_BizName='{0}'
                update FRM_Sql set  s_BizName='{1}' where s_BizName='{0}'
                update FRM_Lookup set s_BizName='{1}' where s_BizName='{0}'
                ", oldBizName, newBizName);
            int rowsCount = 0;
            rowsCount = Biz.FrmDbHelper.ExecuteSqlTran(sqlStr); ;
            return rowsCount;
        }
        /// <summary>
        /// 获取业务对象
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Biz GetBiz(string bizName, BizType type = BizType.Query)
        {
            Biz model = null;
            #region 创建sql
            string sqlStr = @"
select [Type]=n_Type,IsSys=isnull(b_IsSys,0),CountSql=s_CountSql,HiddenFields=s_HiddenFields,Scenes=s_Scenes,ReadOnly4Adds=s_ReadOnly4Adds,ReadOnly4Edits=s_ReadOnly4Edits,NotNulls=s_NotNulls  
from FRM_Biz where s_BizName=@BizName and n_Type=" + (type == BizType.Auto ? "n_Type" : "@BizType") + @"

select [Sql]=s_Sql,Item=n_Item,[Name]=s_Name,IsRun=b_IsRun,LoopField=s_LoopField,[Key]=s_Key  from FRM_Sql where s_BizName=@BizName order by n_Item;

select BizName=s_BizName,Item=n_Item,[Name]=s_Name,[Sql]=s_Sql,IsRun=b_IsRun,CheckField=s_CheckField from FRM_Check where s_BizName=@BizName order by n_Item;

SELECT BizName=s_BizName,FieldName=s_FieldName,Title=s_Title,FieldType=s_FieldType,IsReturn=b_IsReturn,IsLoop=b_IsLoop,Sql=s_Sql,IsNull=b_IsNull,[Index]=n_Index FROM FRM_Field
where s_BizName=@BizName  order by n_Index;

SELECT BizName=s_BizName,KeyField=s_KeyField,ReturnValueField=s_ReturnValueField,ReturnTextField=s_ReturnTextField,ParamFields=s_ParamFields,QueryName=s_QueryName FROM FRM_Lookup
where s_BizName=@BizName  order by s_KeyField;
            ";
            #endregion
            SqlParameter[] ps = { 
                                    new SqlParameter("@BizName", SqlDbType.VarChar),
                                    new SqlParameter("@BizType", SqlDbType.Int)
                                };
            ps[0].Value = bizName;
            ps[1].Value = (int)type;
            DataSet ds = Biz.FrmDbHelper.Query(sqlStr, ps);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {

                //获取BizSql
                #region 获取BizSql
                List<BizSql> sqls = new List<BizSql>();
                var sqlTb = ds.Tables[1];
                for (int i = 0; i < sqlTb.Rows.Count; i++)
                {
                    BizSql sql = new BizSql()
                    {
                        Sql = sqlTb.Rows[i]["Sql"] + "",
                        Name = sqlTb.Rows[i]["Name"] + "",
                        Key = sqlTb.Rows[i]["Key"] + "",
                        Item = Convert.ToInt32(sqlTb.Rows[i]["Item"]),
                        IsRun = Convert.ToBoolean(sqlTb.Rows[i]["IsRun"]),
                        LoopField = sqlTb.Rows[i]["LoopField"] + ""
                    };
                    sqls.Add(sql);
                }
                #endregion 获取BizSql
                //获取Checks
                #region 获取Checks
                var checkTb = ds.Tables[2];
                List<BizCheck> checks = new List<BizCheck>();
                for (int i = 0; i < checkTb.Rows.Count; i++)
                {
                    BizCheck check = new BizCheck()
                    {
                        Sql = checkTb.Rows[i]["Sql"] + "",
                        Item = Convert.ToInt32(checkTb.Rows[i]["Item"]),
                        Name = checkTb.Rows[i]["Name"] + "",
                        CheckField = checkTb.Rows[i]["CheckField"] + "",
                        IsRun = Convert.ToBoolean(checkTb.Rows[i]["IsRun"]),
                    };
                    checks.Add(check);
                }
                #endregion 获取Checks
                //获取Fields
                #region 获取Fields
                var fieldTb = ds.Tables[3];
                List<Field> fields = new List<Field>();
                for (int i = 0; i < fieldTb.Rows.Count; i++)
                {
                    Field field = new Field()
                    {
                        FieldName = fieldTb.Rows[i]["FieldName"] + "",
                        Title = fieldTb.Rows[i]["Title"] + "",
                        FieldType = fieldTb.Rows[i]["FieldType"] + "",
                        IsReturn = Convert.ToBoolean(fieldTb.Rows[i]["IsReturn"]),
                        IsLoop = Convert.ToBoolean(fieldTb.Rows[i]["IsLoop"]),
                        IsNull = Convert.ToBoolean(fieldTb.Rows[i]["IsNull"]),
                        Sql = fieldTb.Rows[i]["Sql"] + "",
                        Index = Convert.ToInt32(fieldTb.Rows[i]["Index"])
                    };
                    fields.Add(field);
                }
                #endregion 获取Fields
                //获取Lookups
                #region 获取Lookups
                var lookupTb = ds.Tables[4];
                List<Lookup> lookups = new List<Lookup>();
                for (int i = 0; i < lookupTb.Rows.Count; i++)
                {
                    Lookup lookup = new Lookup()
                    {
                        KeyField = lookupTb.Rows[i]["KeyField"] + "",
                        ReturnValueField = lookupTb.Rows[i]["ReturnValueField"] + "",
                        ReturnTextField = lookupTb.Rows[i]["ReturnTextField"] + "",
                        ParamFields = lookupTb.Rows[i]["ParamFields"] + "",
                        QueryName = lookupTb.Rows[i]["QueryName"] + ""
                    };
                    lookups.Add(lookup);
                }
                #endregion 获取Fields
                //总结
                model = new Biz()
                {
                    BizName = bizName,
                    Sqls = sqls,
                    Checks = checks,
                    Fields = fields,
                    Lookups = lookups,
                    CountSql = ds.Tables[0].Rows[0]["CountSql"] + "",
                    HiddenFields = ds.Tables[0].Rows[0]["HiddenFields"] + "",
                    Scenes = ds.Tables[0].Rows[0]["Scenes"] + "",
                    ReadOnly4Adds = ds.Tables[0].Rows[0]["ReadOnly4Adds"] + "",
                    ReadOnly4Edits = ds.Tables[0].Rows[0]["ReadOnly4Edits"] + "",
                    NotNulls = ds.Tables[0].Rows[0]["NotNulls"] + "",
                    IsSys = Convert.ToBoolean(ds.Tables[0].Rows[0]["IsSys"]),
                    Type = (BizType)((ds.Tables[0].Rows[0]["Type"] + "").Equals("") ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["Type"]))
                };
            }
            return model;
        }

        /// <summary>
        /// 获取业务对象 -- 查阅历史版本 JaneHe 20191029
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Biz GetBizByOldVersion(string bizId, string bizName, string AI, BizType type = BizType.Query)
        {
            Biz model = null;
            #region 创建sql
            string sqlStr = @"
select [Type]=n_Type,IsSys=isnull(b_IsSys,0),CountSql=s_CountSql,HiddenFields=s_HiddenFields,Scenes=s_Scenes,ReadOnly4Adds=s_ReadOnly4Adds,ReadOnly4Edits=s_ReadOnly4Edits,NotNulls=s_NotNulls,Remark=s_Remark  
from FRM_BizRecord where s_BizName=@BizName and s_AI=@AI and n_Type=" + (type == BizType.Auto ? "n_Type" : "@BizType") + @"

select [Sql]=s_Sql,Item=n_Item,[Name]=s_Name,IsRun=b_IsRun,LoopField=s_LoopField,[Key]=s_Key  from FRM_SqlRecord where s_BizName=@BizName and s_AI=@AI  order by n_Item;

select BizName=s_BizName,Item=n_Item,[Name]=s_Name,[Sql]=s_Sql,IsRun=b_IsRun,CheckField=s_CheckField from FRM_CheckRecord where s_BizName=@BizName and s_AI=@AI  order by n_Item;

SELECT BizName=s_BizName,FieldName=s_FieldName,Title=s_Title,FieldType=s_FieldType,IsReturn=b_IsReturn,IsLoop=b_IsLoop,Sql=s_Sql,IsNull=b_IsNull,[Index]=n_Index FROM FRM_FieldRecord
where s_BizName=@BizName and s_AI=@AI  order by n_Index;

SELECT BizName=s_BizName,KeyField=s_KeyField,ReturnValueField=s_ReturnValueField,ReturnTextField=s_ReturnTextField,ParamFields=s_ParamFields,QueryName=s_QueryName FROM FRM_LookupRecord
where s_BizName=@BizName and s_AI=@AI  order by s_KeyField;
            ";
            #endregion
            SqlParameter[] ps = { 
                                    new SqlParameter("@BizName", SqlDbType.VarChar),
                                    new SqlParameter("@AI", SqlDbType.VarChar),
                                    new SqlParameter("@BizType", SqlDbType.Int)
                                };
            ps[0].Value = bizName;
            ps[1].Value = AI;
            ps[2].Value = (int)type;
            DataSet ds = Biz.FrmDbHelper.Query(sqlStr, ps);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {

                //获取BizSql
                #region 获取BizSql
                List<BizSql> sqls = new List<BizSql>();
                var sqlTb = ds.Tables[1];
                for (int i = 0; i < sqlTb.Rows.Count; i++)
                {
                    BizSql sql = new BizSql()
                    {
                        Sql = sqlTb.Rows[i]["Sql"] + "",
                        Name = sqlTb.Rows[i]["Name"] + "",
                        Key = sqlTb.Rows[i]["Key"] + "",
                        Item = Convert.ToInt32(sqlTb.Rows[i]["Item"]),
                        IsRun = Convert.ToBoolean(sqlTb.Rows[i]["IsRun"]),
                        LoopField = sqlTb.Rows[i]["LoopField"] + ""
                    };
                    sqls.Add(sql);
                }
                #endregion 获取BizSql
                //获取Checks
                #region 获取Checks
                var checkTb = ds.Tables[2];
                List<BizCheck> checks = new List<BizCheck>();
                for (int i = 0; i < checkTb.Rows.Count; i++)
                {
                    BizCheck check = new BizCheck()
                    {
                        Sql = checkTb.Rows[i]["Sql"] + "",
                        Item = Convert.ToInt32(checkTb.Rows[i]["Item"]),
                        Name = checkTb.Rows[i]["Name"] + "",
                        CheckField = checkTb.Rows[i]["CheckField"] + "",
                        IsRun = Convert.ToBoolean(checkTb.Rows[i]["IsRun"]),
                    };
                    checks.Add(check);
                }
                #endregion 获取Checks
                //获取Fields
                #region 获取Fields
                var fieldTb = ds.Tables[3];
                List<Field> fields = new List<Field>();
                for (int i = 0; i < fieldTb.Rows.Count; i++)
                {
                    Field field = new Field()
                    {
                        FieldName = fieldTb.Rows[i]["FieldName"] + "",
                        Title = fieldTb.Rows[i]["Title"] + "",
                        FieldType = fieldTb.Rows[i]["FieldType"] + "",
                        IsReturn = Convert.ToBoolean(fieldTb.Rows[i]["IsReturn"]),
                        IsLoop = Convert.ToBoolean(fieldTb.Rows[i]["IsLoop"]),
                        IsNull = Convert.ToBoolean(fieldTb.Rows[i]["IsNull"]),
                        Sql = fieldTb.Rows[i]["Sql"] + "",
                        Index = Convert.ToInt32(fieldTb.Rows[i]["Index"])
                    };
                    fields.Add(field);
                }
                #endregion 获取Fields
                //获取Lookups
                #region 获取Lookups
                var lookupTb = ds.Tables[4];
                List<Lookup> lookups = new List<Lookup>();
                for (int i = 0; i < lookupTb.Rows.Count; i++)
                {
                    Lookup lookup = new Lookup()
                    {
                        KeyField = lookupTb.Rows[i]["KeyField"] + "",
                        ReturnValueField = lookupTb.Rows[i]["ReturnValueField"] + "",
                        ReturnTextField = lookupTb.Rows[i]["ReturnTextField"] + "",
                        ParamFields = lookupTb.Rows[i]["ParamFields"] + "",
                        QueryName = lookupTb.Rows[i]["QueryName"] + ""
                    };
                    lookups.Add(lookup);
                }
                #endregion 获取Fields
                //总结
                model = new Biz()
                {
                    BizName = bizName,
                    Sqls = sqls,
                    Checks = checks,
                    Fields = fields,
                    Lookups = lookups,
                    CountSql = ds.Tables[0].Rows[0]["CountSql"] + "",
                    HiddenFields = ds.Tables[0].Rows[0]["HiddenFields"] + "",
                    Scenes = ds.Tables[0].Rows[0]["Scenes"] + "",
                    ReadOnly4Adds = ds.Tables[0].Rows[0]["ReadOnly4Adds"] + "",
                    ReadOnly4Edits = ds.Tables[0].Rows[0]["ReadOnly4Edits"] + "",
                    NotNulls = ds.Tables[0].Rows[0]["NotNulls"] + "",
                    IsSys = Convert.ToBoolean(ds.Tables[0].Rows[0]["IsSys"]),
                    Type = (BizType)((ds.Tables[0].Rows[0]["Type"] + "").Equals("") ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["Type"])),
                    Remarks = ds.Tables[0].Rows[0]["Remark"] + ""
                };
            }
            return model;
        }

        /// <summary>
        /// 获取所有biz
        /// </summary>
        /// <returns></returns>
        public static DataSet GetBizList(BizType type = BizType.Query)
        {
            string sqlStr = "select BizName=s_BizName,IsSys=isnull(b_IsSys,0) from FRM_Biz where n_Type=@BizType";
            SqlParameter[] ps = { 
                                    new SqlParameter("@BizType", SqlDbType.Int)
                                };
            ps[0].Value = type;
            DataSet ds = Biz.FrmDbHelper.Query(sqlStr, ps);

            return ds;
        }
        #endregion Biz管理

        #region Biz执行

        /// <summary>
        /// 执行Biz|Query
        /// </summary>
        /// <param name="bizName">Biz|Query</param>
        /// <param name="request">http 请求</param>
        /// <param name="type">Biz类型</param>
        /// <param name="otherPs">其他参数</param>
        /// <param name="method">Http方法</param>
        /// <param name="sortname">排序字段</param>
        /// <param name="sortorder">排序顺序</param>
        /// <returns></returns>
        public static BizResult Execute(string bizName, HttpRequestBase request, BizType type, SqlParameter[] otherPs = null, BizMethod method = BizMethod.POST, string sortname = "", string sortorder = "asc")
        {
            BizResult rs = new BizResult();
            Biz biz = Biz.GetBiz(bizName, type);
            if (biz == null)
                throw new Exception("[B-0002 找不到对象：" + bizName + "]");
            //string sqlStr = Biz.GetSql(biz);
            SqlParameter[] ps = Biz.GetSqlParameters(biz, request, method);


            if (otherPs != null)
                ps = Biz.ApplySqlParameters(ps, otherPs);
            return ExecuteRoute(/*sqlStr,*/ ps, biz, sortname, sortorder);
        }
        public static BizResult Execute(string bizName, SqlParameter[] ps, BizType type = BizType.Query)
        {
            BizResult rs = new BizResult();
            Biz biz = Biz.GetBiz(bizName, type);
            if (biz == null)
                throw new Exception("[B-0005 找不到" + (type == BizType.Query ? "查询对象" : "业务对象") + "：" + bizName + "]");
            //string sqlStr = Biz.GetSql(biz);
            if (ps == null)
            {
                SqlParameter[] ps2 = { };
                ps = ps2;
            }
            if (!(ps.Where(p => p.ParameterName.Equals("@Operator")).Count() > 0))
            {
                SqlParameter pUserName = new SqlParameter("@Operator", SqlDbType.VarChar);
                pUserName.Value = TopsUser.UserNO;
                List<SqlParameter> psList = ps.ToList();
                psList.Add(pUserName);
                ps = psList.ToArray();
            }

            List<Field> outputs = Field.GetFieldsByBizName(bizName).Where(p => p.IsReturn).ToList<Field>();

            return ExecuteRoute(/*sqlStr,*/ ps, biz);
        }

        public static BizResult ExecuteRoute(/*string sqlStr, */SqlParameter[] ps, Biz biz, string sortname = "", string sortorder = "asc")
        {
            switch (biz.Type)
            {
                case BizType.Query: return ExecuteQuery(ps, biz, sortname, sortorder);
                default: return ExecuteBiz(ps, biz);
            }
        }
        public static BizResult ExecuteQuery(SqlParameter[] ps, Biz biz, string sortname = "", string sortorder = "asc")
        {
            /*if (isSort)
                sqlStr = RegExSqlBySortby(sqlStr, ps);*/
            BizResult rs = new BizResult();
            List<DataTable> rsList = new List<DataTable>();
            try
            {
                foreach (var item in biz.Sqls)
                {
                    DataSet ds = null;
                    if (!item.IsRun)
                        continue;
                    string runSqlBiz = PagenationSql(item.Sql, sortname, sortorder);
                    runSqlBiz = rpSqlParam(runSqlBiz);
                    if (runSqlBiz.Trim() == "")
                        continue;
                    Log.WriteSqlLog(runSqlBiz, ps, biz.Type, biz.BizName, "执行查询对象查询(" + item.Item + ")");
                    ds = Biz.BizDbHelper.Query(runSqlBiz, ps);
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables.Count; i++)
                        {
                            ds.Tables[i].TableName = "QueryTable_" + item.Item + "_" + i;
                            rsList.Add(ds.Tables[i]);
                        }
                    }
                }
                //判断第一个查询包含“[Columns]...[/Columns]”
                bool IsAutoRowsCount = false;
                if (biz.Sqls.Count > 0 && biz.Sqls[0].Sql.IndexOf("--[Columns]") > -1 && biz.Sqls[0].Sql.IndexOf("--[/Columns]") > -1)
                    IsAutoRowsCount = true;
                //如果有合计查询？
                if (!string.IsNullOrEmpty(biz.CountSql) || IsAutoRowsCount)
                {
                    try
                    {
                        // biz.CountSql=biz.CountSql.Replace("\n", "");  
                        DataSet dsCount = null;
                        //如果第一个查询包含“[Columns]...[/Columns]”则进行自动统计
                        if (string.IsNullOrEmpty(biz.CountSql.Replace("\n", "")) && IsAutoRowsCount)
                        {
                            biz.CountSql = AutoRowsCountSql(biz.Sqls[0].Sql);
                        }
                        string runCountSqlBiz = rpSqlParam(biz.CountSql);
                        Log.WriteSqlLog(runCountSqlBiz, ps, biz.Type, biz.BizName, "执行查询对象查询(合计查询)");
                        dsCount = Biz.BizDbHelper.Query(runCountSqlBiz, ps);
                        if (dsCount != null && dsCount.Tables.Count > 0)
                        {
                            dsCount.Tables[0].TableName = "QueryTable_RowsCount";
                            rsList.Add(dsCount.Tables[0]);
                        }
                    }
                    catch (Exception ex)
                    {
                        //JaneHe 20210625 记录请求报错信息
                        FrmDbHelper.AddErrorInfo(ps, biz, ex);
                        throw new Exception("[B-0006 合计查询错误：" + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                rs.IsSuccess = false;
                rs.Msgs = new List<string>() { ex.Message };
                //JaneHe 20210625 记录请求报错信息
                FrmDbHelper.AddErrorInfo(ps, biz, ex);
                return rs;
            }

            rs.Data = rsList;
            rs.IsSuccess = true;
            return rs;
        }
        private static BizResult ExecuteQuery1(SqlParameter[] ps, Biz biz, bool isSort = false)
        {
            /*if (isSort)
                sqlStr = RegExSqlBySortby(sqlStr, ps);*/
            BizResult rs = new BizResult();
            List<DataTable> rsList = new List<DataTable>();
            try
            {
                foreach (var item in biz.Sqls)
                {
                    DataSet ds = null;
                    if (!item.IsRun)
                        continue;
                    string runSqlBiz = rpSqlParam(item.Sql);
                    Log.WriteSqlLog(runSqlBiz, ps, biz.Type, biz.BizName, "执行查询对象查询(" + item.Item + ")");
                    ds = Biz.BizDbHelper.Query(runSqlBiz, ps);
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables.Count; i++)
                        {
                            ds.Tables[i].TableName = "QueryTable_" + item.IsRun + "_" + i;
                            rsList.Add(ds.Tables[i]);
                        }
                    }
                }
                //如果有合计查询？
                if (!string.IsNullOrEmpty(biz.CountSql))
                {
                    try
                    {
                        DataSet dsCount = null;
                        string runCountSqlBiz = rpSqlParam(biz.CountSql);
                        Log.WriteSqlLog(runCountSqlBiz, ps, biz.Type, biz.BizName, "执行查询对象查询(合计查询)");
                        dsCount = Biz.BizDbHelper.Query(runCountSqlBiz, ps);
                        if (dsCount != null && dsCount.Tables.Count > 0)
                        {
                            dsCount.Tables[0].TableName = "QueryTable_RowsCount";
                            rsList.Add(dsCount.Tables[0]);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("[B-0006 合计查询错误：" + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                rs.IsSuccess = false;
                rs.Msgs = new List<string>() { ex.Message };
                return rs;
            }

            rs.Data = rsList;
            rs.IsSuccess = true;
            return rs;
        }
        public static BizResult ExecuteBiz(SqlParameter[] ps, Biz biz)
        {
            List<Field> outputs = Field.GetFieldsByBizName(biz.BizName).Where(p => p.IsReturn).ToList<Field>();
            BizResult rs = new BizResult();
            try
            {

                //执行业务过程
                rs.OutPut = Biz.BizDbHelper.ExecuteSqlTranByBiz(biz, outputs, ps);
                rs.IsSuccess = true;


            }
            catch (BizCheckException ex)
            {
                rs.IsSuccess = false;
                rs.Msgs = new List<string>() { ex.Message };
                rs.Other1 = ex.CheckField;
                //JaneHe 20210625 记录请求报错信息
                FrmDbHelper.AddErrorInfo(ps, biz, ex);
                return rs;
            }
            catch (SqlException ex)
            {
                rs.IsSuccess = false;
                rs.Msgs = new List<string>() { "执行Sql错误：" + ex.Message };
                //JaneHe 20210625 记录请求报错信息
                FrmDbHelper.AddErrorInfo(ps, biz, ex);
                return rs;
            }
            catch (Exception ex)
            {
                rs.IsSuccess = false;
                rs.Msgs = new List<string>() { ex.Message };
                //JaneHe 20210625 记录请求报错信息
                FrmDbHelper.AddErrorInfo(ps, biz, ex);
                return rs;
            }
            return rs;
        }
        public static bool ExecuteChenk(string sqlStr, SqlParameter[] ps, SqlConnection conn = null, SqlTransaction trans = null)
        {
            var IsSuccess = false;
            DataSet ds = null;
            try
            {
                ds = Biz.BizDbHelper.QueryCheck(Biz.rpSqlParam(sqlStr), ps, conn, trans);
            }
            catch (SqlException ex)
            {
                throw new Exception("业务检查Sql错误：" + ex.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                IsSuccess = true;
            }
            return IsSuccess;
        }
        public static BizResult ExcuteMeta2(string bizName)
        {
            BizResult rs = new BizResult();
            List<DataTable> rsList = new List<DataTable>();
            try
            {
                Biz biz = Biz.GetBiz(bizName, BizType.Query);
                if (biz == null)
                    throw new Exception("[B-0009 找不到对象：" + bizName + "]");
                foreach (var item in biz.Sqls)
                {
                    DataSet ds = null;
                    if (!item.IsRun)
                        continue;
                    string runSqlBiz = rpSqlParam(item.Sql);
                    List<SqlParameter> lp = new List<SqlParameter>();
                    for (int i = 0; i < biz.Fields.Count; i++)
                    {
                        SqlDbType sqlDbType = TopsCommon.fieldTypeString2SqlType(biz.Fields[i].FieldType);
                        lp.Add(new SqlParameter("@" + biz.Fields[i].FieldName, sqlDbType));
                    }
                    if (!(lp.Where(p => p.ParameterName.Equals("@Operator")).Count() > 0))
                    {
                        lp.Add(new SqlParameter("@Operator", SqlDbType.VarChar));
                    }
                    if (!(lp.Where(p => p.ParameterName.Equals("@OperatorRoleID")).Count() > 0))
                    {
                        lp.Add(new SqlParameter("@OperatorRoleID", SqlDbType.VarChar));
                    }
                    if (!(lp.Where(p => p.ParameterName.Equals("@beginIndex")).Count() > 0))
                    {
                        lp.Add(new SqlParameter("@beginIndex", SqlDbType.Int));
                    }
                    if (!(lp.Where(p => p.ParameterName.Equals("@endIndex")).Count() > 0))
                    {
                        lp.Add(new SqlParameter("@endIndex", SqlDbType.Int));
                    }
                    ds = Biz.BizDbHelper.Query(runSqlBiz, lp.ToArray());
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables.Count; i++)
                        {
                            ds.Tables[i].TableName = "QueryTable_" + item.IsRun + "_" + i;
                            rsList.Add(ds.Tables[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                rs.IsSuccess = false;
                rs.Msgs = new List<string>() { ex.Message };
                return rs;
            }

            rs.Data = rsList;
            rs.IsSuccess = true;
            return rs;
        }
        public static BizResult ExcuteMeta(string bizName)
        {
            BizResult rs = new BizResult();
            List<DataTable> rsList = new List<DataTable>();
            try
            {
                Biz biz = Biz.GetBiz(bizName, BizType.Query);
                if (biz == null)
                    throw new Exception("[B-0009 找不到对象：" + bizName + "]");
                foreach (var item in biz.Sqls)
                {

                    if (!item.IsRun)
                        continue;
                    string runSqlBiz = rpSqlParam(item.Sql);
                    List<SqlParameter> lp = new List<SqlParameter>();
                    for (int i = 0; i < biz.Fields.Count; i++)
                    {
                        SqlDbType sqlDbType = TopsCommon.fieldTypeString2SqlType(biz.Fields[i].FieldType);
                        lp.Add(new SqlParameter("@" + biz.Fields[i].FieldName, sqlDbType));
                    }
                    if (!(lp.Where(p => p.ParameterName.Equals("@Operator")).Count() > 0))
                    {
                        lp.Add(new SqlParameter("@Operator", SqlDbType.VarChar));
                    }
                    if (!(lp.Where(p => p.ParameterName.Equals("@OperatorRoleID")).Count() > 0))
                    {
                        lp.Add(new SqlParameter("@OperatorRoleID", SqlDbType.VarChar));
                    }
                    if (!(lp.Where(p => p.ParameterName.Equals("@beginIndex")).Count() > 0))
                    {
                        lp.Add(new SqlParameter("@beginIndex", SqlDbType.Int));
                    }
                    if (!(lp.Where(p => p.ParameterName.Equals("@endIndex")).Count() > 0))
                    {
                        lp.Add(new SqlParameter("@endIndex", SqlDbType.Int));
                    }
                    var dt = Biz.BizDbHelper.QuerySchemaTable(runSqlBiz, lp.ToArray());
                    rsList.Add(dt);
                }
            }
            catch (Exception ex)
            {
                rs.IsSuccess = false;
                rs.Msgs = new List<string>() { ex.Message };
                return rs;
            }

            rs.Data = rsList;
            rs.IsSuccess = true;
            return rs;
        }
        public static string GetSql(Biz biz)
        {
            StringBuilder sb = new StringBuilder();
            if (biz.Sqls != null && biz.Sqls.Count > 0)
            {
                foreach (BizSql item in biz.Sqls)
                {
                    sb.Append(item.Sql + "\r\n");
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="biz"></param>
        /// <param name="request"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static SqlParameter[] GetSqlParameters(Biz biz, HttpRequestBase request, BizMethod method)
        {
            Dictionary<string, Object> dicRequest = new Dictionary<string, Object>();
            if (method == BizMethod.POST)
            {
                if (request.Form.AllKeys.Count() > 0)
                {
                    foreach (var key in request.Form.AllKeys)
                    {
                        dicRequest.Add(key, request.Form[key]);
                    }
                }
            }
            else
            {
                if (request.QueryString.AllKeys.Count() > 0)
                {
                    foreach (var key in request.QueryString.AllKeys)
                    {
                        dicRequest.Add(key, request.QueryString[key]);
                    }
                }
            }
            return DoGetSqlParameters(biz, dicRequest, method);
        }
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="biz"></param>
        /// <param name="request"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static SqlParameter[] GetSqlParameters(Biz biz, HttpRequest request, BizMethod method)
        {
            Dictionary<string, Object> dicRequest = new Dictionary<string, Object>();
            if (method == BizMethod.POST)
            {
                if (request.Form.AllKeys.Count() > 0)
                {
                    foreach (var key in request.Form.AllKeys)
                    {
                        dicRequest.Add(key, request.Form[key]);
                    }
                }
            }
            else
            {
                if (request.QueryString.AllKeys.Count() > 0)
                {
                    foreach (var key in request.QueryString.AllKeys)
                    {
                        dicRequest.Add(key, request.QueryString[key]);
                    }
                }
            }
            return DoGetSqlParameters(biz, dicRequest, method);
        }
        public static SqlParameter[] DoGetSqlParameters(Biz biz, Dictionary<string, Object> dicRequest, BizMethod method)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            if (method == BizMethod.POST)
            {
                if (biz.Fields != null && biz.Fields.Count > 0)
                {
                    foreach (Field field in biz.Fields)
                    {


                        SqlDbType sqlDbType = TopsCommon.fieldTypeString2SqlType(field.FieldType);
                        if (field.IsLoop)
                        {
                            Regex objRegEx = new Regex("\\[[0-9]*[0-9]\\]\\[" + field.FieldName + "\\]", RegexOptions.Singleline);
                            var isInRequest = false;
                            if (dicRequest != null)
                            {
                                foreach (var key in dicRequest.Keys)
                                {
                                    if (objRegEx.IsMatch(key))
                                    {
                                        SqlParameter p = new SqlParameter("@" + key, sqlDbType);//, field.FieldType
                                        p.Value = TopsCommon.ChangeType(dicRequest[key], field.FieldType, key);

                                        //if (!field.IsNull && string.IsNullOrEmpty(p.Value + ""))
                                        //{
                                        //    throw new Exception("[B-0007 参数：" + key + "不能为空！");
                                        //}
                                        pList.Add(p);
                                        isInRequest = true;
                                    }
                                }
                            }
                            //如果请求中不包含该参数，则添加一个无值参数
                            if (!isInRequest)
                            {
                                SqlParameter p = new SqlParameter("@" + field.FieldName, sqlDbType);
                            }
                        }
                        else
                        {
                            SqlParameter p = new SqlParameter("@" + field.FieldName, sqlDbType);
                            if (dicRequest.Keys.Contains(field.FieldName))
                            {
                                try
                                {
                                    p.Value = TopsCommon.ChangeType(dicRequest[field.FieldName], field.FieldType, field.FieldName);
                                }
                                catch (FormatException ex)
                                {
                                    if (!(field.IsNull && string.IsNullOrEmpty(dicRequest[field.FieldName] + "")))
                                        throw new Exception("[FM001]" + field.FieldName + ":" + ex.Message);
                                }
                            }
                            pList.Add(p);
                        }
                    }
                }
            }
            else if (method == BizMethod.GET)
            {
                if (biz.Fields != null && biz.Fields.Count > 0)
                {
                    foreach (Field field in biz.Fields)
                    {
                        SqlDbType sqlDbType = TopsCommon.fieldTypeString2SqlType(field.FieldType);
                        SqlParameter p = new SqlParameter("@" + field.FieldName, sqlDbType);
                        if (dicRequest != null)
                        {
                            if (dicRequest.Keys.Contains(field.FieldName))
                            {
                                p.Value = TopsCommon.ChangeType(dicRequest[field.FieldName], field.FieldType, field.FieldName);
                            }
                            /*
                            if (!field.IsNull && string.IsNullOrEmpty(p.Value + ""))
                            {
                                throw new Exception("[B-0008 参数：" + field.FieldName + "不能为空！");
                            }
                             * */
                        }
                        pList.Add(p);
                    }
                }
            }

            //HWJ类型转换(时间)
            //pList = TopsCommon.ChangeDateType(pList, dicRequest);

            /////////获取定义和获取Operator的值
            var us = pList.Where(p => p.ParameterName == "@Operator").ToList();
            SqlParameter pUserName = (us != null && us.Count > 0) ? us.First() : null;
            if (pUserName == null)
            {
                pUserName = new SqlParameter("@Operator", SqlDbType.VarChar);
                pList.Add(pUserName);
            }
            pUserName.Value = TopsUser.UserNO;

            var rd = pList.Where(p => p.ParameterName == "@OperatorRoleID").ToList();
            SqlParameter pRoleID = (rd != null && rd.Count > 0) ? rd.First() : null;
            if (pRoleID == null)
            {
                pRoleID = new SqlParameter("@OperatorRoleID", SqlDbType.VarChar);
                pList.Add(pRoleID);
            }

            pRoleID.Value = TopsUser.GetOperatorRoleID;

            return pList.ToArray();
        }
        private static SqlParameter[] ApplySqlParameters(SqlParameter[] ps, SqlParameter[] otherPs)
        {
            if (otherPs == null || otherPs.Count() == 0)
                return ps;
            List<SqlParameter> psList = ps.ToList();
            for (int i = 0; i < otherPs.Count(); i++)
            {
                var newPsList = psList.Where(p => p.ParameterName.Equals(otherPs[i].ParameterName)).ToList<SqlParameter>();
                if (newPsList.Count() > 0)
                {
                    //存在
                    psList.Remove(newPsList.First());
                }
                psList.Add(otherPs[i]);
            }
            return psList.ToArray();
        }
        private static string RegExSqlBySortby(string sqlStr, SqlParameter[] ps)
        {
            Dictionary<string, object> dic = ps.ToDictionary(p => p.ParameterName, p => p.Value);
            if (!dic.Keys.Contains("@sortby"))
                throw new Exception("[Biz-0002 参数中找不到：@sortby]");
            if (!dic.Keys.Contains("@contraryDir"))
                throw new Exception("[Biz-0002 参数中找不到：@contraryDir]");
            //Regex objRegEx = new Regex("(?<=((^.|^+)" + dic["@sortby"] + "=))[.\\s\\S]*?(?=(=|from))", RegexOptions.Multiline | RegexOptions.Singleline);
            Regex objRegEx = new Regex("(?<=((\\b)" + dic["@sortby"] + "=))[.\\s\\S]*?(?=(=|from))", RegexOptions.Multiline | RegexOptions.Singleline);
            string sortField = objRegEx.Match(sqlStr).Value;
            string[] fs = sortField.Split(',');

            if (fs.Length > 0)
            {
                sortField = "";
                for (int i = 0; i < fs.Length; i++)
                {
                    if (!(i > 0 && i + 1 == fs.Length))
                    {
                        if (i > 0)
                            sortField += ",";
                        sortField += fs[i];
                    }
                }
            }
            sqlStr = sqlStr.Replace("@:sortby", sortField);
            sqlStr = sqlStr.Replace("@:contraryDir", dic["@contraryDir"] + "");
            return sqlStr;
        }
        public static string rpSqlParam(string sql)
        {
            return sql.Replace("@:", "@");
        }
        /// <summary>
        /// 分页创造器
        /// 2013-12-17
        /// masing
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static string PagenationSql(string sql, string sortname, string sortorder)
        {
            var regexBeginMaths = new Regex(@"(--\[Pagenation:[\w\s\,]+\])", RegexOptions.Singleline).Matches(sql);
            var regexEnd = new Regex(@"(--\[/Pagenation\])", RegexOptions.Singleline).Match(sql).Value;
            if (regexBeginMaths.Count > 0 && !string.IsNullOrEmpty(regexEnd))
            {
                var regexBegin = "";
                foreach (var item in regexBeginMaths)
                {
                    regexBegin = item + "";
                    var orderbyStr = regexBegin.Substring(14, regexBegin.Length - 14);//.Replace("--[Pagenation:", "");
                    orderbyStr = orderbyStr.Remove(orderbyStr.Length - 1);
                    if (!string.IsNullOrEmpty(sortname.Trim()))
                        orderbyStr = "[" + sortname + "] " + sortorder;
                    sql = sql.Replace(regexBegin, "select * from ( select RowNumber=row_number() over(order by " + orderbyStr + "),* from (");
                }
                sql = sql.Replace("--[/Pagenation]", ") T_pagenation  ) TT_pagenation  where RowNumber between @:beginIndex and @:endIndex ");
            }
            return sql;
        }
        /// <summary>
        /// 自动统计创造器
        /// 2013-12-19
        /// masing
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static string AutoRowsCountSql(string sql)
        {
            var columnsStrMaths = new Regex(@"(--\[Columns\][\w\W]+?--\[/Columns\])", RegexOptions.Singleline).Matches(sql);
            var regexBeginMaths = new Regex(@"(--\[Pagenation:[\w\s\,]+\])", RegexOptions.Singleline).Matches(sql);
            var regexEnd = new Regex(@"(--\[/Pagenation\])", RegexOptions.Singleline).Match(sql).Value;
            if (columnsStrMaths.Count != regexBeginMaths.Count)
                throw new Exception("[B-000601 标记[Pagenation]与[Columns]必须相等！");
            if (columnsStrMaths.Count > 0)
            {
                for (int i = 0; i < columnsStrMaths.Count; i++)
                {
                    var columnsStr = columnsStrMaths[i] + "";
                    var regexBegin = regexBeginMaths[i] + "";
                    if (!string.IsNullOrEmpty(regexBegin) && !string.IsNullOrEmpty(regexEnd) && !string.IsNullOrEmpty(columnsStr))
                    {
                        sql = sql.Replace(columnsStr, " R=1 ");
                        sql = sql.Replace(regexBegin, " select RowsCount=COUNT(1) from ( ")
                            .Replace("--[/Pagenation]", " ) T_RowsCount ");
                    }
                }
            }
            return sql;
        }
        public static int GetSqlRunCount(BizSql sqlDM, SqlParameter[] ps)
        {
            int rs = 0;
            if (string.IsNullOrEmpty(sqlDM.LoopField))
                rs = 1;
            else
            {
                rs = ps.Where(p => p.ParameterName.EndsWith("][" + sqlDM.LoopField + "]")).Count();
            }
            return rs;
        }
        public static SqlParameter[] GetSqlRunParams(SqlParameter[] ps, Biz biz, int sqlIndex = 0, int loopIndex = 0)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            Regex objRegEx = new Regex("\\[[0-9]*[0-9]\\]\\[[\\w\\W]+\\]", RegexOptions.Singleline);
            //新处理
            if (biz.Fields != null)
            {
                var fieldNames = biz.Fields.Select(p => p.FieldName).ToList();
                foreach (var f in biz.Fields)
                {
                    var IsInParams = false;
                    SqlDbType sqlDbType = TopsCommon.fieldTypeString2SqlType(f.FieldType);
                    foreach (var param in ps)
                    {
                        if (param.ParameterName.Equals("@" + f.FieldName))
                        {

                            if (!f.IsNull && string.IsNullOrEmpty(param.Value + ""))
                            {
                                throw new BizCheckException("[B-0117] 空检查：" + (string.IsNullOrEmpty(f.Title) ? f.FieldName : f.Title) + "不能为空！", f.FieldName);
                            }
                            pList.Add(param);
                            IsInParams = true;
                        }
                        else if (objRegEx.Match(param.ParameterName).Value.Equals("[" + loopIndex + "][" + f.FieldName + "]"))
                        {
                            var tempUF = ps.Where(p => objRegEx.Match(p.ParameterName).Value.Equals("[" + loopIndex + "][UpdateFlag" + sqlIndex + "]"));
                            if (tempUF.Count() > 0)
                            {
                                var currentUpdateFlag = tempUF.First<SqlParameter>();
                                if (currentUpdateFlag != null && !currentUpdateFlag.Value.Equals("delete") && !f.IsNull && string.IsNullOrEmpty(param.Value + ""))
                                {
                                    throw new BizCheckException("[B-0017] 空检查：" + (string.IsNullOrEmpty(f.Title) ? f.FieldName : f.Title) + "不能为空！", f.FieldName);
                                }
                            }
                            pList.Add(new SqlParameter("@" + f.FieldName, param.DbType) { Value = param.Value });
                            IsInParams = true;
                        }
                        //如果该参数不包含在fields中，定位另为参数，需保留
                        else if (
                            !fieldNames.Contains(param.ParameterName.Substring(1))
                            && !pList.Contains(param)
                            && !objRegEx.IsMatch(param.ParameterName)
                            )
                        {
                            pList.Add(param);
                        }
                    }
                    if (!IsInParams)
                        pList.Add(new SqlParameter("@" + f.FieldName, sqlDbType));
                }
            }
            return pList.ToArray();
        }

        #endregion Biz执行

        #region 导出
        public static string Export(Biz model)
        {
            model.BizName = model.BizName.Trim();
            if (string.IsNullOrEmpty(model.BizName.Trim()))
                throw new Exception("BizName不能为空");
            string sqlStr = string.Format(@"
                --删除相关表
                delete from FRM_Biz where s_BizName='{0}'
                delete from FRM_Sql where s_BizName='{0}'
                delete from FRM_Check where s_BizName='{0}'
                delete from FRM_Field where s_BizName='{0}'
                delete from FRM_Lookup where s_BizName='{0}'
                --插入Biz表
                Insert into FRM_Biz(s_BizName,n_Type,b_IsSys,s_CountSql,s_HiddenFields,s_Scenes,s_ReadOnly4Adds,s_ReadOnly4Edits,s_NotNulls) values ('{0}',{1},{2},'{3}','{4}','{5}','{6}','{7}','{8}')
                
                --获取到当前版本的AI号
                declare @AI{0} varchar(500)
                set @AI{0} = (case isnull((select top 1 s_AI from [FRM_BizRecord] where s_BizName = '{0}'
                order by d_CreateTime desc),'') when '' then 'A0' else 
                'A' + Convert(varchar(200),(cast(replace(isnull((select top 1 s_AI from [FRM_BizRecord] where s_BizName = '{0}'
                order by d_CreateTime desc),''),'A','') as int)+1)) end)

                --插入BizRecord表（JaneHe 记录FRM历史修正对象的数据，可供追溯使用 20191023）
                Insert into FRM_BizRecord([n_Id]
           ,[s_BizName] 
           ,[n_Type]
           ,[b_IsSys]
           ,[s_CountSql]
           ,[s_HiddenFields]
           ,[s_Scenes]
           ,[s_ReadOnly4Adds]
           ,[s_ReadOnly4Edits] 
           ,[s_NotNulls]
           ,[s_Remark]
           ,[n_state]
           ,[s_AI]
           ,[s_Creator]
           ,[d_CreateTime])
                VALUES(NEWID(),
           '{0}',{1},{2},'{3}','{4}','{5}','{6}','{7}','{8}','{10}'
           ,1
           ,@AI{0}
           ,'{9}'
           ,GETDATE())
                ", (model.BizName + "").Replace("'", "''"), (int)model.Type, model.IsSys ? 1 : 0, (model.CountSql + "").Replace("'", "''"), (model.HiddenFields + "").Replace("'", "''"), (model.Scenes + "").Replace("'", "''"), (model.ReadOnly4Adds + "").Replace("'", "''"), (model.ReadOnly4Edits + "").Replace("'", "''"), (model.NotNulls + "").Replace("'", "''"), System.Web.HttpContext.Current.Session["ToolsUserNO"].ToString(), (model.Remarks + "").Replace("'", "''"));
            if (model.Checks != null)
            {
                sqlStr += "\r\n--插入Check";
                foreach (var check in model.Checks)
                {
                    sqlStr += string.Format(@"
                INSERT INTO FRM_Check(s_BizName,n_Item,s_Name,s_Sql,b_IsRun,s_CheckField) VALUES('{0}',{1},'{2}','{3}',{4},'{5}')

                --插入CheckRecord表（JaneHe 记录FRM历史修正对象的数据，可供追溯使用 20191023）
                INSERT INTO [dbo].[FRM_CheckRecord]
           ([n_Id]
           ,[s_BizName]
           ,[n_Item]
           ,[s_Name]
           ,[s_Sql]
           ,[b_IsRun]
           ,[s_CheckField]
           ,[n_state]
           ,[s_AI]
           ,[s_Creator]
           ,d_CreateTime) 
           VALUES(NewID(),'{0}',{1},'{2}','{3}',{4},'{5}',1
           ,@AI{0}
           ,'{6}'
           ,GETDATE())
                ", (model.BizName + "").Replace("'", "''"), check.Item, (check.Name + "").Replace("'", "''"), (check.Sql + "").Replace("'", "''"), check.IsRun ? 1 : 0, (check.CheckField + "").Replace("'", "''"), System.Web.HttpContext.Current.Session["ToolsUserNO"].ToString());
                }
            }
            if (model.Sqls != null)
            {
                sqlStr += "\r\n--插入BizSql";
                foreach (var sql in model.Sqls)
                {
                    sqlStr += string.Format(@"
                INSERT INTO FRM_Sql(s_BizName,n_Item,s_Sql,b_IsRun,s_LoopField,s_Name,s_Key) VALUES('{0}',{1},'{2}',{3},'{4}','{5}','{6}')
 
                --插入SqlRecord表（JaneHe 记录FRM历史修正对象的数据，可供追溯使用 20191023）
                INSERT INTO [FRM_SqlRecord]
           ([n_Id]
           ,[s_BizName]
           ,[n_Item] 
           ,[s_Sql]
           ,[b_IsRun]
           ,[s_LoopField]
           ,[s_Name]
           ,[s_Key]
           ,[n_state]
           ,[s_AI]
           ,[s_Creator]
           ,[d_CreateTime])
 VALUES(NEWID(),'{0}',{1},'{2}',{3},'{4}','{5}','{6}',1
           ,@AI{0}
           ,'{7}'
           ,GETDATE())
                ", (model.BizName + "").Replace("'", "''"),
                 sql.Item,
                 (sql.Sql + "").Replace("'", "''"),
                 sql.IsRun ? 1 : 0,
                 (sql.LoopField + "").Replace("'", "''"),
                 (sql.Name + "").Replace("'", "''"),
                 (sql.Key + "").Replace("'", "''"),
                 System.Web.HttpContext.Current.Session["ToolsUserNO"].ToString());
                }
            }
            if (model.Fields != null)
            {
                sqlStr += "\r\n--插入Field";
                foreach (var f in model.Fields)
                {
                    sqlStr += string.Format(@"
                INSERT INTO FRM_Field(s_BizName,s_FieldName,s_Title,s_FieldType,b_IsReturn,b_IsLoop,s_Sql,b_IsNull,n_Index) VALUES('{0}','{1}','{2}','{3}',{4},{5},'{6}',{7},{8})


                --插入FieldRecord表（JaneHe 记录FRM历史修正对象的数据，可供追溯使用 20191023）
                INSERT INTO [FRM_FieldRecord]
           ([n_Id]
           ,[s_BizName]
           ,[s_FieldName]
           ,[s_Title]
           ,[s_FieldType]
           ,[b_IsReturn]
           ,[b_IsLoop]
           ,[s_Sql]
           ,[b_IsNull]
           ,[n_Index]
           ,[n_state]
           ,[s_AI]
           ,[s_Creator]
           ,[s_CreateTime])
  VALUES(NEWID(),'{0}','{1}','{2}','{3}',{4},{5},'{6}',{7},{8},1,@AI{0}
           ,'{9}'
           ,GETDATE())
                ", (model.BizName + "").Replace("'", "''"), (f.FieldName + "").Trim().Replace("'", "''"), (f.Title + "").Replace("'", "''"), f.FieldType, f.IsReturn ? 1 : 0, f.IsLoop ? 1 : 0, (f.Sql + "").Replace("'", "''"), f.IsNull ? 1 : 0, f.Index, System.Web.HttpContext.Current.Session["ToolsUserNO"].ToString());
                }
            }
            if (model.Lookups != null)
            {
                sqlStr += "\r\n--插入Lookup";
                foreach (var l in model.Lookups)
                {
                    sqlStr += string.Format(@"
                INSERT INTO FRM_Lookup(s_BizName,s_KeyField,s_ReturnValueField,s_ReturnTextField,s_ParamFields,s_QueryName) VALUES('{0}','{1}','{2}','{3}','{4}','{5}')


                --插入LookupRecord表（JaneHe 记录FRM历史修正对象的数据，可供追溯使用 20191023）
                INSERT INTO [FRM_LookupRecord]
           ([n_Id]
           ,[s_BizName]
           ,[s_KeyField]
           ,[s_ReturnValueField]
           ,[s_ReturnTextField]
           ,[s_ParamFields]
           ,[s_QueryName]
           ,[n_state]
           ,[s_AI]
           ,[s_Creator]
           ,[d_CreateTime])  VALUES(NEWID(),'{0}','{1}','{2}','{3}','{4}','{5}',1,@AI{0}
           ,'{6}'
           ,GETDATE())
                ", (model.BizName + "").Replace("'", "''"),
                 (l.KeyField + "").Trim().Replace("'", "''"),
                 (l.ReturnValueField + "").Replace("'", "''"),
                 (l.ReturnTextField + "").Replace("'", "''"),
                 (l.ParamFields + "").Replace("'", "''"),
                 (l.QueryName + "").Replace("'", "''"), System.Web.HttpContext.Current.Session["ToolsUserNO"].ToString());
                }
            }
            return sqlStr;
        }
        #endregion 导出
    }


}
