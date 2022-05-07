using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace Tops.FRM
{
    public class TopsMeta
    {
        public static string SysLocation = System.Configuration.ConfigurationManager.AppSettings["SysLocation"];
        public string Field { get; set; }
        public string Scene { get; set; }
        public string Location { get; set; }
        public string Caption { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
        public string CommonID { get; set; }
        public string Sql { get; set; }
        public int Width { get; set; }
        public int MaxLength { get; set; }
        public bool Hide { get; set; }
        public bool NotNull { get; set; }
        public Dictionary<string, string> DataList
        {
            get
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                if (this.Sql!=null&&this.Sql.ToLower().IndexOf("select") >= 0)
                {
                    DataSet ds = Biz.BizDbHelper.Query(this.Sql, null);
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Columns.Contains("key") && ds.Tables[0].Columns.Contains("value"))
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            dic.Add(ds.Tables[0].Rows[i]["key"] + "", ds.Tables[0].Rows[i]["value"] + "");
                        }
                    }
                }
                else if (this.Sql!=null&&(!string.IsNullOrEmpty(this.Sql.Trim())))
                {
                    var arr = this.Sql.Trim().Split(';');
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
        public TopsMeta()
        {

        }
        /// <summary>
        /// 获取字段的数据字典
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="scene"></param>
        /// <returns></returns>
        public static TopsMeta FindMeta(string fieldName, string scene = null)
        {
            TopsMeta model = new TopsMeta()
                {
                    Field = fieldName,
                    Caption = fieldName,
                    Width=100
                };
            #region 创建sql
            string sqlStr = @"
select 
    Field=s_Field,
    Scene=s_Scene,
    Location=s_Location,
    Caption=s_Caption,
    Type=s_Type,
    Format=s_Format,
    CommonID=s_CommonID,
    Sql=s_Sql,
    Width=s_Width
from tDictionary
where s_Field=@Field
and isnull(s_Scene,'')=isnull(@Scene,'')
and isnull(s_Location,'')=isnull(@Location,'');
            ";
            #endregion
            SqlParameter[] ps = { 
                                    new SqlParameter("@Field", SqlDbType.VarChar),
                                    new SqlParameter("@Scene", SqlDbType.VarChar),
                                    new SqlParameter("@Location", SqlDbType.VarChar)
                                };
            ps[0].Value = fieldName;
            ps[1].Value = scene;
            ps[2].Value = TopsMeta.SysLocation;
            DataSet ds = Biz.BizDbHelper.Query(sqlStr, ps);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var dr = ds.Tables[0].Rows[0];
                //总结
                model.Scene = dr["Scene"] + "";
                model.Location = dr["Location"] + "";
                model.Caption = dr["Caption"] + "";
                model.Type = dr["Type"] + "";
                model.Format = dr["Format"] + "";
                model.CommonID = dr["CommonID"] + "";
                model.Sql = dr["Sql"] + "";
                try
                {
                    model.Width = Convert.ToInt32(dr["Width"] + "");
                }catch{}
                model.Width = model.Width <= 0 ? 100 : model.Width;
            }
            return model;
        }
        /// <summary>
        /// 获取查询对象所有字段的数据字典
        /// </summary>
        /// <param name="queryName"></param>
        /// <returns></returns>
        public static List<Dictionary<string, TopsMeta>> LoadMetas2(string queryName)
        {
            List<Dictionary<string, TopsMeta>> metasList = new List<Dictionary<string, TopsMeta>>();
            try
            {
                var biz = Biz.GetBiz(queryName, BizType.Query);
                var br = Biz.ExcuteMeta(queryName);
                if (!br.IsSuccess)
                    throw new Exception(br.Msgs[0]);
                var dtList = br.Data as List<DataTable>;
                if (dtList != null)
                {
                    var hFields =TopsCommon.FieldsStr2List( biz.HiddenFields);
                    var hNotNulls = TopsCommon.FieldsStr2List(biz.NotNulls);
                    for (int i = 0; i < dtList.Count; i++)
                    {
                        Dictionary<string, TopsMeta> metas = new Dictionary<string, TopsMeta>();
                        for (int j = 0; j < dtList[i].Columns.Count; j++)
                        {
                            var colName = dtList[i].Columns[j].ColumnName;
                            
                            if (colName == "RowNumber")
                                continue;
                            string scene = null;
                            if (biz.DicScenes.ContainsKey(colName))
                                scene = biz.DicScenes[colName];
                            var model = TopsMeta.FindMeta(colName, scene);
                            model.Scene = scene;
                            model.MaxLength = dtList[i].Columns[j].MaxLength;
                            if (hFields.Count > 0 && hFields.Contains(colName))
                            {
                                model.Hide = true;
                            }
                            if (hNotNulls.Count > 0 && hNotNulls.Contains(colName))
                            {
                                model.NotNull = true;
                            }
                            if (string.IsNullOrEmpty((model.Type+"").Trim()))
                                model.Type = dtList[i].Columns[j].DataType.Name+"";
                            metas.Add(dtList[i].Columns[j].ColumnName, model);
                        }
                        metasList.Add(metas);
                    }
                }
                return metasList;
            }
            catch (Exception ex)
            {
                throw new Exception("[M001]" + ex.Message);
            }
        }
        public static List<Dictionary<string, TopsMeta>> LoadMetas(string queryName)
        {
            List<Dictionary<string, TopsMeta>> metasList = new List<Dictionary<string, TopsMeta>>();
            try
            {
                var biz = Biz.GetBiz(queryName, BizType.Query);
                var br = Biz.ExcuteMeta(queryName);
                if (!br.IsSuccess)
                    throw new Exception("[LM-001]获取META失败："+br.Msgs[0]);
                var dtList = br.Data as List<DataTable>;
                if (dtList != null)
                {
                    var hFields = TopsCommon.FieldsStr2List(biz.HiddenFields);
                    var hNotNulls = TopsCommon.FieldsStr2List(biz.NotNulls);
                    for (int i = 0; i < dtList.Count; i++)
                    {
                        Dictionary<string, TopsMeta> metas = new Dictionary<string, TopsMeta>();
                        for (int j = 0; j < dtList[i].Rows.Count; j++)
                        {
                            var colName = dtList[i].Rows[j]["ColumnName"]+"";

                            if (colName == "RowNumber")
                                continue;
                            string scene = null;
                            if (biz.DicScenes.ContainsKey(colName))
                                scene = biz.DicScenes[colName];
                            var model = TopsMeta.FindMeta(colName, scene);
                            model.Scene = scene;
                            model.MaxLength = Convert.ToInt32(dtList[i].Rows[j]["ColumnSize"]);
                            if (hFields.Count > 0 && hFields.Contains(colName))
                            {
                                model.Hide = true;
                            }
                            if (hNotNulls.Count > 0 && hNotNulls.Contains(colName))
                            {
                                model.NotNull = true;
                            }
                            if (string.IsNullOrEmpty((model.Type + "").Trim()))
                                model.Type =System.Type.GetType(dtList[i].Rows[j]["DataType"]+"").Name;
                            metas.Add(colName, model);
                        }
                        metasList.Add(metas);
                    }
                }
                return metasList;
            }
            catch (Exception ex)
            {
                throw new Exception("[M001]" + ex.Message);
            }
        }
    }
}
