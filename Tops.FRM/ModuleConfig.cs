using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tops.FRM
{
    /// <summary>
    /// 序列化之后供js使用
    /// </summary>
    public class ModuleConfig
    {
        public static string GetModuleConfig(List<Dictionary<string, TopsMeta>> metas, List<Lookup> lookups, Biz biz, string StatusCtrl = null)
        {
            Dictionary<string, Lookup> dicLookups = new Dictionary<string, Lookup>();
            foreach (var item in lookups)
            {
                dicLookups.Add(item.KeyField, item);
            }
            Dictionary<string, Object> dicGrids = new Dictionary<string, object>();
            for (int i = 1; i < metas.Count; i++)
            {
                var cols = ModuleConfig.GetColumns(metas[i], lookups, biz.Sqls[i], biz, StatusCtrl);
                dicGrids.Add("grid" + i, cols);
            }
            return new
            {
                metas = metas,
                lookups = dicLookups,
                colsList = dicGrids
            }.ToJson();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="metas"></param>
        /// <param name="lookups"></param>
        /// <param name="bizSql"></param>
        /// <param name="biz"></param>
        /// <param name="StatusCtrl">add|edit|null</param>
        /// <returns></returns>
        public static List<object> GetColumns(Dictionary<string, TopsMeta> metas, List<Lookup> lookups, BizSql bizSql, Biz biz,string StatusCtrl=null)
        {
            var cols = new List<object>();
            var lookup = new Object();
            var lookupItem = lookups.Where(p => p.KeyField == bizSql.Key).ToList<Lookup>();
            if (lookupItem.Count > 0)
            {
                lookup = lookupItem.First();
            }
            foreach (var meta in metas)
            {
                var hFields = TopsCommon.FieldsStr2List(biz.HiddenFields);
                var isHide = false;
                if (hFields.Count > 0 && hFields.Contains(meta.Key))
                {
                    isHide = true;
                }
                object editor = null;
                bool allowEdit = false;
                if(StatusCtrl == "add"){
                    if (!TopsCommon.FieldsStr2List(biz.ReadOnly4Adds).Contains(meta.Key))
                        allowEdit = true;
                }
                else if (StatusCtrl == "edit")
                {
                    if (!TopsCommon.FieldsStr2List(biz.ReadOnly4Edits).Contains(meta.Key))
                        allowEdit = true;   
                }
                if (allowEdit)
                {
                    //编辑器类型，包括text、checkbox、date、select、spinner|int|float
                    var type = "text";
                    if (meta.Value.Type == "Int32")
                        type = "int";
                    else if (meta.Value.Type == "Decimal" || meta.Value.Type == "Double")
                        type = "float";
                    else if (meta.Value.Type == "DateTime")
                        type = "date";
                    else if (meta.Value.Type == "Boolean")
                        type = "checkbox";
                    editor = new
                    {
                        type = type
                    };
                }
                cols.Add(new
                {
                    name = meta.Key,
                    display = meta.Value.Caption,
                    align = (meta.Value.Type == "Int32"||meta.Value.Type == "Decimal" || meta.Value.Type == "Double")?"right":"left",
                    width = meta.Value.Width,
                    hide = isHide,
                    type = meta.Value.Type == "DateTime" ? "date" : meta.Value.Type,
                    editor = editor,
                    frozen = false
                });
            }
            return cols;
        }
    }
}
