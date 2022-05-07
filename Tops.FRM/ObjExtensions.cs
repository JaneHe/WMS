using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace Tops.FRM
{
    public static class ObjExtensions
    {
        public static string ToJson(this object obj,bool isFormatting=false,bool timeSecond=false)
        {
            if (isFormatting)
                return Newtonsoft.Json.JsonConvert.SerializeObject(obj,Formatting.Indented, new IsoDateTimeConverter()
                {
                    DateTimeFormat = timeSecond ? "yyyy'-'MM'-'dd' 'HH':'mm':'ss" : "yyyy'-'MM'-'dd"
                });
            else
                return Newtonsoft.Json.JsonConvert.SerializeObject(obj, new IsoDateTimeConverter()
                {
                    DateTimeFormat = timeSecond ? "yyyy'-'MM'-'dd' 'HH':'mm':'ss" : "yyyy'-'MM'-'dd"
                });
        }
        public static List<DataTable> ToList(this DataSet dsSource)
        {

            List<DataTable> list = new List<DataTable>();
            if (dsSource == null || dsSource.Tables.Count == 0) return list;
            for (int i = 0; i < dsSource.Tables.Count; i++)
            {
                list.Add(dsSource.Tables[i]);
            }
            return list;
        }
        public static Dictionary<Object, Object> ToDictionary(this DataRow dataRow)
        {

            Dictionary<Object, Object> dic = new Dictionary<object, object>();
            if (dataRow.Table.Columns == null || dataRow.Table.Columns.Count == 0) return dic;
            for (int i = 0; i < dataRow.Table.Columns.Count; i++)
            {
                dic.Add(dataRow.Table.Columns[i], dataRow[i]);
            }
            return dic;
        }
        public static List<object> ToList(this DataTable dtSource)
        {

            List<object> list = new List<object>();
            if (dtSource == null || dtSource.Rows.Count == 0) return list;
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                list.Add(dtSource.Rows[i][0]);
            }
            return list;
        }

        /// <summary>
        /// DataTable 转换为List 集合
        /// </summary>
        /// <typeparam name="TResult">类型</typeparam>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static List<TResult> ToList<TResult>(this DataTable dt) where TResult : class,new()
        {

            List<PropertyInfo> prlist = new List<PropertyInfo>();

            Type t = typeof(TResult);

            Array.ForEach<PropertyInfo>(t.GetProperties(), p => { if (dt.Columns.IndexOf(p.Name) != -1) prlist.Add(p); });
            //创建返回的集合
            List<TResult> oblist = new List<TResult>();

            foreach (DataRow row in dt.Rows)
            {
                TResult ob = new TResult();
                prlist.ForEach(p => { if (row[p.Name] != DBNull.Value) p.SetValue(ob, row[p.Name], null); });
                oblist.Add(ob);
            }
            return oblist;
        }
    }
}
