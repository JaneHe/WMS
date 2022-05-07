using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Tops.FRM
{
    public class Module
    {
        public string id { get; set; }
        public string text { get; set; }
        public bool isexpand { get; set; }
        public int Tag { get; set; }
        /// <summary>
        /// 最顶层模块的ParentID都为0
        /// </summary>
        public string ParentID { get; set; }
        public string url { get; set; }
        public bool IsLeaf { get; set; }
        public string Rank { get; set; }
        public string Helper { get; set; }
        public string Type { get; set; }
        public string No { get; set; }
        public string icon { get; set; }
        public bool IsCom { get; set; }
        public string ComQueryName { get; set; }
        public string ComObjectName { get; set; }
        public string ComAddBiz { get; set; }
        public string ComModifyBiz { get; set; }
        public string ComDelBiz { get; set; }
        public List<Module> children { get; set; }
        public string DepartmentCode { get; set; }
        /// <summary>
        /// 通过父亲获取模块对象
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public static List<Module> GetModulesByParentID(string parentID, bool getChild = true)
        {
            List<Module> list = new List<Module>();
            string sqlStr = @"SELECT 
ModuleID=s_ModuleID,
ModuleName=s_ModuleName,
Tag=case n_Tag&1 when 0 then 0 when 1 then 1 end,
ParentID=s_ParentID,
Url=s_Url,
IconUrl=s_IconUrl,
IsLeaf=n_IsLeaf,
Helper=cast(s_Helper AS varchar(max)),
    IsCom=b_IsCom,
    ComQueryName=s_ComQueryName,
    ComObjectName=s_ComObjectName,
    ComAddBiz=s_ComAddBiz,
    ComModifyBiz=s_ComModifyBiz,
    ComDelBiz=s_ComDelBiz 
FROM tModule where s_ParentID=@ParentID
group by s_ModuleID,s_ModuleName,n_Tag,s_ParentID,s_Url,
s_IconUrl,n_IsLeaf,cast(s_Helper AS varchar(max)),b_IsCom,s_ComQueryName,s_ComObjectName,
s_ComAddBiz,s_ComModifyBiz,s_ComDelBiz,n_No
order by n_No asc ";
            SqlParameter[] ps = { 
                                    new SqlParameter("@ParentID", SqlDbType.VarChar)
                                };
            ps[0].Value = parentID;
            DataSet ds = Biz.BizDbHelper.Query(sqlStr, ps);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                var dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Module m = new Module()
                    {
                        id = dt.Rows[i]["ModuleID"] + "",
                        text = dt.Rows[i]["ModuleName"] + "",
                        ParentID = dt.Rows[i]["ParentID"] + "",
                        url = dt.Rows[i]["Url"] + "",
                        icon = dt.Rows[i]["IconUrl"] + "",
                        IsLeaf = Convert.ToBoolean(dt.Rows[i]["IsLeaf"])
                    };
                    m.text = m.text + "[" + m.id + "]";
                    if (getChild)
                        m.children = Module.GetModulesByParentID(m.id);
                    //如果到叶子，就添加
                    if (m.children.Count == 0 && m.IsLeaf)
                    {
                        m.children = GetDefaultMenu(m.id);
                        m.icon = "/TopsLib/Content/icon/01/038.gif";
                    }
                    else
                    {
                        m.isexpand = true;
                    }
                    //如果不是叶子m.IsLeaf ? "module_leaf" : 
                    m.Type = "module";
                    list.Add(m);
                }
            }
            return list;
        }
        /// <summary>
        /// 通过id获取模块对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Module GetModulesByID(string id)
        {
            string sqlStr = @"SELECT 
ModuleID=s_ModuleID,
ModuleName=s_ModuleName,
Tag=case n_Tag&1 when 0 then 0 when 1 then 1 end,
ParentID=s_ParentID,
Url=s_Url,
IconUrl=s_IconUrl,
IsLeaf=n_IsLeaf,
No=n_No,
Helper=s_Helper,
    IsCom=b_IsCom,
    ComQueryName=s_ComQueryName,
    ComObjectName=s_ComObjectName,
    ComAddBiz=s_ComAddBiz,
    ComModifyBiz=s_ComModifyBiz,
    ComDelBiz=s_ComDelBiz 
FROM tModule where s_ModuleID=@ModuleID";
            SqlParameter[] ps = { 
                                    new SqlParameter("@ModuleID", SqlDbType.VarChar)
                                };
            ps[0].Value = id;
            DataSet ds = Biz.BizDbHelper.Query(sqlStr, ps);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                var dt = ds.Tables[0];
                Module m = new Module()
                {
                    id = dt.Rows[0]["ModuleID"] + "",
                    text = dt.Rows[0]["ModuleName"] + "",
                    No = dt.Rows[0]["No"] + "",
                    url = dt.Rows[0]["Url"] + "",
                    icon = dt.Rows[0]["IconUrl"] + "",
                    Tag = Convert.ToInt32(dt.Rows[0]["Tag"]),
                    IsLeaf = Convert.ToBoolean(dt.Rows[0]["IsLeaf"]),
                    Helper = dt.Rows[0]["Helper"] + "",
                    ParentID = dt.Rows[0]["ParentID"] + "",
                    IsCom = Convert.ToBoolean(dt.Rows[0]["IsCom"]),
                    ComQueryName = dt.Rows[0]["ComQueryName"] + "",
                    ComObjectName = dt.Rows[0]["ComObjectName"] + "",
                    ComAddBiz = dt.Rows[0]["ComAddBiz"] + "",
                    ComModifyBiz = dt.Rows[0]["ComModifyBiz"] + "",
                    ComDelBiz = dt.Rows[0]["ComDelBiz"] + ""
                };
                return m;
            }
            return null;
        }
        /// <summary>
        /// 获取模块下默认节点（业务对象节点，查询对象节点）
        /// </summary>
        /// <param name="moduleID"></param>
        /// <returns></returns>
        public static List<Module> GetDefaultMenu(string moduleID)
        {
            List<Module> list = new List<Module>();
            //业务对象
            Module mBiz = new Module()
            {
                id = moduleID + "_Biz",
                text = "业务对象",
                Type = "module_b",
                children = Module.GetBizQuery4Module(moduleID, 2),
                ParentID = moduleID,
                icon = "/TopsLib/Content/icon/03/081.gif"
            };
            //查询对象
            Module mQuery = new Module()
            {
                id = moduleID + "_Query",
                text = "查询对象",
                Type = "module_q",
                children = Module.GetBizQuery4Module(moduleID, 1),
                ParentID = moduleID,
                icon = "/TopsLib/Content/icon/03/103.gif"
            };
            list.Add(mBiz);
            list.Add(mQuery);
            return list;
        }
        /// <summary>
        /// 获取模块下关联的业务对象或查询对象
        /// </summary>
        /// <param name="moduleID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<Module> GetBizQuery4Module(string moduleID, int type)
        {
            List<Module> list = new List<Module>();
            string sqlStr = @"select BizName=s_BizName from tModuleBiz where s_ModuleID=@ModuleID and n_Type=@Type";
            SqlParameter[] ps = { 
                                    new SqlParameter("@ModuleID", SqlDbType.VarChar),
                                    new SqlParameter("@Type", SqlDbType.Int)
                                };
            ps[0].Value = moduleID;
            ps[1].Value = type;
            DataSet ds = Biz.BizDbHelper.Query(sqlStr, ps);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                var dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Module m = new Module()
                    {
                        id = dt.Rows[i]["BizName"] + "",
                        text = dt.Rows[i]["BizName"] + "",
                        Type = "module_" + (type == 1 ? "q" : "b") + "_l",
                        ParentID = moduleID
                    };
                    list.Add(m);
                }
            }
            return list;
        }
        /// <summary>
        /// 查询业务对象或查询对象(不包含当前模块已关联的对象)
        /// </summary>
        /// <param name="queryNme"></param>
        /// <param name="queryType"></param>
        /// <param name="moduleID"></param>
        /// <returns></returns>
        public static List<string> GetBiz0Query(string queryNme, int queryType, string moduleID)
        {
            List<string> list = new List<string>();
            string sqlStr = @"
SELECT  BizName=s_BizName FROM FRM_Biz 
WHERE s_BizName like '%'+@BizName+'%' and n_Type=@Type
and s_BizName not in (
    select s_BizName from tModuleBiz where s_ModuleID=@ModuleID
)
";
            SqlParameter[] ps = { 
                                    new SqlParameter("@BizName", SqlDbType.VarChar),
                                    new SqlParameter("@Type", SqlDbType.Int),
                                    new SqlParameter("@ModuleID", SqlDbType.VarChar),
                                };
            ps[0].Value = queryNme;
            ps[1].Value = queryType;
            ps[2].Value = moduleID;
            DataSet ds = Biz.FrmDbHelper.Query(sqlStr, ps);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                var dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    list.Add(dt.Rows[i]["BizName"] + "");
                }
            }
            return list;
        }
        /// <summary>
        /// 递归获取系统模块
        /// </summary>
        /// <param name="parentID">父节点ID</param>
        /// <param name="IsCheckPower">是否检查权限</param>
        /// <returns></returns>
        public static List<Module> GetSysModules(string parentID = "0", bool IsCheckPower = true)
        {
            List<Module> list = new List<Module>();
            SqlParameter[] ps = { 
                                    new SqlParameter("@ParentID", SqlDbType.VarChar),
                                    new SqlParameter("@IsCheckPower", SqlDbType.Bit)
                                };
            ps[0].Value = parentID;
            ps[1].Value = IsCheckPower ? 1 : 0;
            var br = Biz.Execute("SelModules", ps);
            if (!br.IsSuccess)
                throw new Exception(br.Msgs[0]);
            var dtList = br.Data as List<DataTable>;
            if (dtList != null && dtList[0].Rows.Count > 0)
            {
                var dt = dtList[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var isAdd = true;

                    //项目模块地址
                    string url = dt.Rows[i]["Url"].ToString();
                    //辨别是否是内网或者外网设备进行连接
                    if (Tops.FRM.TopsUser.IpAddress.IndexOf("192.168.").Equals(-1) && Tops.FRM.TopsUser.IpAddress.IndexOf("127.0.").Equals(-1))
                    {
                        if (url.Contains("http://"))
                        {
                            url = url.Replace("192.168.61.3:8889", "58.47.128.48:8889");
                            url = url.Replace("192.168.61.230:8888", "58.47.128.48:8888");
                            url = url.Replace("192.168.61.234", "58.47.128.48:8889");
                        }
                    }

                    Module m = new Module()
                    {
                        id = dt.Rows[i]["ModuleID"] + "",
                        text = dt.Rows[i]["ModuleName"] + "",
                        ParentID = dt.Rows[i]["ParentID"] + "",
                        IsLeaf = Convert.ToBoolean(dt.Rows[i]["IsLeaf"]),
                        url = url + "",
                        icon = dt.Rows[i]["IconUrl"] + "",
                        IsCom = Convert.ToBoolean(dt.Rows[i]["IsCom"]),
                        isexpand = true
                    };
                    if (!m.IsLeaf)
                    {
                        m.children = Module.GetSysModules(m.id, IsCheckPower);
                        if (m.children.Count == 0)
                            isAdd = false;
                    }
                    if (isAdd)
                        list.Add(m);
                    if (m.IsCom)
                        m.url = "/q/" + m.id;
                }
            }
            return list;
        }

        /// <summary>
        /// 递归获取文控系统模块
        /// </summary>
        /// <param name="parentID">父节点ID</param>
        /// <param name="IsCheckPower">是否检查权限</param>
        /// <returns></returns>
        public static List<Module> GetDMSysModules(string parentID = "0", bool IsCheckPower = true)
        {
            List<Module> list = new List<Module>();
            SqlParameter[] ps = { 
                                    new SqlParameter("@ParentID", SqlDbType.VarChar),
                                    new SqlParameter("@IsCheckPower", SqlDbType.Bit)
                                };
            ps[0].Value = parentID;
            ps[1].Value = IsCheckPower;
            var br = Biz.Execute("SelDMSysModules", ps);
            if (!br.IsSuccess)
                throw new Exception(br.Msgs[0]);
            var dtList = br.Data as List<DataTable>;
            if (dtList != null && dtList[0].Rows.Count > 0)
            {
                var dt = dtList[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var isAdd = true;
                    Module m = new Module()
                    {
                        id = dt.Rows[i]["DepartmentID"] + "",
                        DepartmentCode = dt.Rows[i]["DepartmentCode"] + "",
                        text = dt.Rows[i]["DepartmentName"] + "",
                        ParentID = dt.Rows[i]["ParentID"] + "",
                        IsLeaf = Convert.ToBoolean(dt.Rows[i]["IsLeaf"]),
                        Rank = dt.Rows[i]["Rank"] + "",
                        isexpand = false
                    };
                    if (!m.IsLeaf)
                    {
                        m.children = Module.GetDMSysModules(m.DepartmentCode, IsCheckPower);
                        if (m.children.Count == 0)
                            isAdd = false;
                    }
                    if (isAdd)
                        list.Add(m);

                }
            }
            return list;
        }


        /// <summary>
        /// 递归删除模块
        /// </summary>
        /// <param name="moduleID"></param>
        /// <param name="delChilds"></param>
        /// <returns></returns>
        public static BizResult DelSysModules(string moduleID, bool delChilds = true)
        {
            if (delChilds)
            {
                List<Module> childs = GetModulesByParentID(moduleID);
                if (childs != null && childs.Count > 0)
                {
                    //删除儿子
                    foreach (var m in childs)
                    {
                        DelSysModules(m.id);
                    }
                }
            }
            //删除自己
            var br = new BizResult();
            SqlParameter[] ps = { 
                                new SqlParameter("@ModuleID",SqlDbType.VarChar)
                                };
            ps[0].Value = moduleID;
            br = Biz.Execute("DelModule", ps, BizType.Business);
            return br;

        }
    }
}
