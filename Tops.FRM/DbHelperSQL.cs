using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Common;
using System.Collections.Generic;
using System.Linq;
using System.Data.OleDb;
using System.Text;
using System.IO;
using System.Security.Cryptography;
namespace Tops.FRM
{
    /// <summary>
    /// 数据访问基础类
    /// Copyright (C) 2004-2008 By LiTianPing 
    /// </summary>
    public class DbHelperSQL
    {
        //数据库连接字符串(web.config来配置)，可以动态更改connectionString支持多数据库.  
        private string connectionString = "";
        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string strDecryDES(string encryptString, string encryptKey)
        {
            // return encryptString;
            try
            {
                byte[] bytIn = Convert.FromBase64String(encryptString);//把一个得到的64位为基数字组成的字符串转换为8位数组并付给一个字节数组。
                MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);//把字节数组写入流。
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                byte[] rgbIV = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                ICryptoTransform encrypto = dCSP.CreateDecryptor(rgbKey, rgbIV);//创建基本的加密对象
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);//将MS流转换为加密流,以encrypto对流进行加密转换。CryptoStreamMode模式为读访问。
                StreamReader sr = new StreamReader(cs);//从加密流中读取字符。
                return sr.ReadToEnd();//得到一个从加密流开始位置到结束的字符。
            }
            catch
            {
                return encryptString;
            }
        }
        public DbHelperSQL(int index = 0)
        {
            if (index == 0)
                connectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString0"];
            else
                connectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString1"];
        }

        #region 公用方法
        /// <summary>
        /// 判断是否存在某表的某个字段
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="columnName">列名称</param>
        /// <returns>是否存在</returns>
        public bool ColumnExists(string tableName, string columnName)
        {
            string sql = "select count(1) from syscolumns where [id]=object_id('" + tableName + "') and [name]='" + columnName + "'";
            object res = GetSingle(sql);
            if (res == null)
            {
                return false;
            }
            return Convert.ToInt32(res) > 0;
        }

        public int GetMaxID(string FieldName, string TableName)
        {
            string strsql = "select max(" + FieldName + ")+1 from " + TableName;
            object obj = GetSingle(strsql);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }

        public bool Exists(string strSql)
        {
            object obj = GetSingle(strSql);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 表是否存在
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public bool TabExists(string TableName)
        {
            string strsql = "select count(*) from sysobjects where id = object_id(N'[" + TableName + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1";
            //string strsql = "SELECT count(*) FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + TableName + "]') AND type in (N'U')";
            object obj = GetSingle(strsql);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool Exists(string strSql, params SqlParameter[] cmdParms)
        {
            object obj = GetSingle(strSql, cmdParms);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region  执行简单SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string SQLString)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        public int ExecuteSqlByTime(string SQLString, int Times)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.CommandTimeout = Times;
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }
        /// <summary>
        /// 执行SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLString">SQL语句</param>  
        public int ExecuteSqlTran(String SQLString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                SqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    cmd.CommandText = SQLString;
                    int rows = cmd.ExecuteNonQuery();
                    tx.Commit();
                    return rows;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 执行SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLString">SQL语句</param>  
        public int ExecuteSqlTranReturnID(String SQLString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                SqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    cmd.CommandText = SQLString;
                    int rows = Convert.ToInt32(cmd.ExecuteScalar());//cmd.ExecuteNonQuery();
                    tx.Commit();
                    return rows;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 执行SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLString">SQL语句</param>  
        public int ExecuteSqlTran(String SQLString, SqlParameter[] ps)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                SqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    PrepareCommand(cmd, conn, null, SQLString, ps);
                    int rows = cmd.ExecuteNonQuery();
                    tx.Commit();
                    return rows;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }
        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>  
        public int ExecuteSqlTran(List<String> SQLStringList)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                SqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    int count = 0;
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n];
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            count += cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    return count;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }
        /// <summary>
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string SQLString, string content)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(SQLString, connection);
                System.Data.SqlClient.SqlParameter myParameter = new System.Data.SqlClient.SqlParameter("@content", SqlDbType.NText);
                myParameter.Value = content;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
        /// <summary>
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public object ExecuteSqlGet(string SQLString, string content)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(SQLString, connection);
                System.Data.SqlClient.SqlParameter myParameter = new System.Data.SqlClient.SqlParameter("@content", SqlDbType.NText);
                myParameter.Value = content;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    object obj = cmd.ExecuteScalar();
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
        /// <summary>
        /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSqlInsertImg(string strSQL, byte[] fs)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(strSQL, connection);
                System.Data.SqlClient.SqlParameter myParameter = new System.Data.SqlClient.SqlParameter("@fs", SqlDbType.Image);
                myParameter.Value = fs;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }
        public object GetSingle(string SQLString, int Times)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.CommandTimeout = Times;
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }
        /// <summary>
        /// 执行查询语句，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader ExecuteReader(string strSQL)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(strSQL, connection);
            try
            {
                connection.Open();
                SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                throw e;
            }

        }
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public DataSet Query(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
                    command.SelectCommand.CommandTimeout = 0;  //取消超时默认设置  默认是30s   增加一条设置
                    command.Fill(ds, "ds");
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    connection.Close();
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }
        public DataSet Query(string SQLString, int Times)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
                    command.SelectCommand.CommandTimeout = Times;
                    command.Fill(ds, "ds");
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    connection.Close();
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }



        #endregion

        #region 执行带参数的SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string SQLString, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        throw e;
                    }
                }
            }
        }


        /// <summary>
        /// 执行SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        public Dictionary<string, Object> ExecuteSqlTran(string SQLString, List<Field> outputs, params SqlParameter[] cmdParms)
        {
            Dictionary<string, Object> dic = new Dictionary<string, object>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlDataReader sqlDr = null;
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    try
                    {
                        PrepareCommand(cmd, conn, trans, SQLString, cmdParms);
                        sqlDr = cmd.ExecuteReader();

                        while (sqlDr.Read())
                        {
                            if (outputs != null)
                            {
                                foreach (var f in outputs)
                                {
                                    try
                                    {
                                        dic.Add(f.FieldName, sqlDr[f.FieldName]);
                                    }
                                    catch
                                    { }
                                }
                            }
                        }
                        sqlDr.Close();
                        cmd.Parameters.Clear();
                        trans.Commit();
                        return dic;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                    finally
                    {
                        if (sqlDr != null)
                            sqlDr.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 执行Biz，实现数据库事务。
        /// </summary>
        public Dictionary<string, Object> ExecuteSqlTranByBiz(Biz biz, List<Field> outputs, params SqlParameter[] cmdParms)
        {
            Dictionary<string, Object> dic = new Dictionary<string, object>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlDataReader sqlDr = null;
                using (SqlTransaction trans = conn.BeginTransaction())
                {

                    try
                    {
                        var loopFields = biz.Fields.Where(p => p.IsLoop).Select(p => p.FieldName).ToList();
                        SqlParameter[] currentParms = Biz.GetSqlRunParams(cmdParms, biz);
                        //非重复字段业务检查
                        foreach (var check in biz.Checks)
                        {
                            if (!check.IsRun || check.Sql.Trim() == "")
                                continue;
                            if (!loopFields.Contains(check.CheckField))
                            {
                                Log.WriteSqlLog(check.Sql, currentParms, biz.Type, biz.BizName, "执行业务检查：" + check.Item + ".*." + check.Name);
                                if (!Biz.ExecuteChenk(check.Sql, currentParms, conn, trans))
                                    throw new BizCheckException(check.Item + ".*." + check.Name, check.CheckField);
                            }
                        }
                        if (biz.Sqls != null && biz.Sqls.Count > 0)
                        {
                            #region 循环执行sql
                            for (int j = 0; j < biz.Sqls.Count; j++)
                            {
                                var sqlMD = biz.Sqls[j];
                                string cmdText = Biz.rpSqlParam(sqlMD.Sql);


                                //判断当前sql循环次数
                                var sqlRunCount = Biz.GetSqlRunCount(sqlMD, cmdParms);

                                for (int i = 0; i < sqlRunCount; i++)
                                {
                                    SqlCommand cmd = new SqlCommand();
                                    currentParms = Biz.GetSqlRunParams(cmdParms, biz, j, i);
                                    //重复字段业务检查
                                    foreach (var check in biz.Checks)
                                    {
                                        if (!check.IsRun || check.Sql.Trim() == "")
                                            continue;
                                        if (loopFields.Contains(check.CheckField))
                                        {
                                            Log.WriteSqlLog(check.Sql, currentParms, biz.Type, biz.BizName, "执行业务检查：" + check.Item + "." + i + "." + check.Name);
                                            if (!Biz.ExecuteChenk(check.Sql, currentParms, conn, trans))
                                                throw new BizCheckException(check.Item + "." + i + "." + check.Name, check.CheckField);
                                        }
                                    }

                                    Log.WriteSqlLog(cmdText, currentParms, biz.Type, biz.BizName, "执行业务过程[" + sqlMD.Item + "]：" + i + "." + sqlMD.Name);
                                    PrepareCommand(cmd, conn, trans, cmdText, currentParms);
                                    sqlDr = cmd.ExecuteReader();
                                    //获取返回的参数
                                    while (sqlDr.Read())
                                    {
                                        if (outputs != null)
                                        {
                                            foreach (var f in outputs)
                                            {
                                                try
                                                {
                                                    //添加/更新返回值
                                                    if (dic.ContainsKey(f.FieldName))
                                                        dic[f.FieldName] = sqlDr[f.FieldName];
                                                    else
                                                        dic.Add(f.FieldName, sqlDr[f.FieldName]);
                                                    //根据返回值更新业务参数的值，供下一个sql使用
                                                    var cmdP = cmdParms.Where(p => p.ParameterName == "@" + f.FieldName).First();
                                                    if (cmdP != null)
                                                        cmdP.Value = sqlDr[f.FieldName];
                                                }
                                                catch { }
                                            }
                                        }
                                    }
                                    sqlDr.Close();
                                    cmd.Parameters.Clear();
                                }


                            }
                            #endregion 循环执行sql
                        }
                        trans.Commit();
                        return dic;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                    finally
                    {
                        if (sqlDr != null && !sqlDr.IsClosed)
                            sqlDr.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        //public int ExecuteSqlTran(System.Collections.Generic.List<CommandInfo> cmdList)
        //{
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();
        //        using (SqlTransaction trans = conn.BeginTransaction())
        //        {
        //            SqlCommand cmd = new SqlCommand();
        //            try
        //            {
        //                int count = 0;
        //                //循环
        //                foreach (CommandInfo myDE in cmdList)
        //                {
        //                    string cmdText = myDE.CommandText;
        //                    SqlParameter[] cmdParms = (SqlParameter[])myDE.Parameters;
        //                    PrepareCommand(cmd, conn, trans, cmdText, cmdParms);

        //                    if (myDE.EffentNextType == EffentNextType.WhenHaveContine || myDE.EffentNextType == EffentNextType.WhenNoHaveContine)
        //                    {
        //                        if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
        //                        {
        //                            trans.Rollback();
        //                            return 0;
        //                        }

        //                        object obj = cmd.ExecuteScalar();
        //                        bool isHave = false;
        //                        if (obj == null && obj == DBNull.Value)
        //                        {
        //                            isHave = false;
        //                        }
        //                        isHave = Convert.ToInt32(obj) > 0;

        //                        if (myDE.EffentNextType == EffentNextType.WhenHaveContine && !isHave)
        //                        {
        //                            trans.Rollback();
        //                            return 0;
        //                        }
        //                        if (myDE.EffentNextType == EffentNextType.WhenNoHaveContine && isHave)
        //                        {
        //                            trans.Rollback();
        //                            return 0;
        //                        }
        //                        continue;
        //                    }
        //                    int val = cmd.ExecuteNonQuery();
        //                    count += val;
        //                    if (myDE.EffentNextType == EffentNextType.ExcuteEffectRows && val == 0)
        //                    {
        //                        trans.Rollback();
        //                        return 0;
        //                    }
        //                    cmd.Parameters.Clear();
        //                }
        //                trans.Commit();
        //                return count;
        //            }
        //            catch
        //            {
        //                trans.Rollback();
        //                throw;
        //            }
        //        }
        //    }
        //}
        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        //public void ExecuteSqlTranWithIndentity(System.Collections.Generic.List<CommandInfo> SQLStringList)
        //{
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();
        //        using (SqlTransaction trans = conn.BeginTransaction())
        //        {
        //            SqlCommand cmd = new SqlCommand();
        //            try
        //            {
        //                int indentity = 0;
        //                //循环
        //                foreach (CommandInfo myDE in SQLStringList)
        //                {
        //                    string cmdText = myDE.CommandText;
        //                    SqlParameter[] cmdParms = (SqlParameter[])myDE.Parameters;
        //                    foreach (SqlParameter q in cmdParms)
        //                    {
        //                        if (q.Direction == ParameterDirection.InputOutput)
        //                        {
        //                            q.Value = indentity;
        //                        }
        //                    }
        //                    PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
        //                    int val = cmd.ExecuteNonQuery();
        //                    foreach (SqlParameter q in cmdParms)
        //                    {
        //                        if (q.Direction == ParameterDirection.Output)
        //                        {
        //                            indentity = Convert.ToInt32(q.Value);
        //                        }
        //                    }
        //                    cmd.Parameters.Clear();
        //                }
        //                trans.Commit();
        //            }
        //            catch
        //            {
        //                trans.Rollback();
        //                throw;
        //            }
        //        }
        //    }
        //}
        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        public void ExecuteSqlTranWithIndentity(Hashtable SQLStringList)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    try
                    {
                        int indentity = 0;
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            SqlParameter[] cmdParms = (SqlParameter[])myDE.Value;
                            foreach (SqlParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.InputOutput)
                                {
                                    q.Value = indentity;
                                }
                            }
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            foreach (SqlParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.Output)
                                {
                                    indentity = Convert.ToInt32(q.Value);
                                }
                            }
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string SQLString, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader ExecuteReader(string SQLString, params SqlParameter[] cmdParms)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 600;
            try
            {
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                throw e;
            }
            //   finally
            //   {
            //    cmd.Dispose();
            //    connection.Close();
            //   } 

        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public DataSet Query(string SQLString, SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                cmd.CommandTimeout = 600;//TimeOut设置为10分钟 2013.09.23
                /*var reader = cmd.ExecuteReader();
                reader.Read();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string fieldType = reader.GetDataTypeName(i);
                    string fieldName = reader.GetName(i);
                    string fieldType2 = reader.GetFieldType(i).FullName;
                    Console.WriteLine(fieldName + "\t" + fieldType + "\t" + fieldType2);
                }
                reader.Close();*/
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
        }
        /// <summary>
        /// 执行查询语句，获取SchemaTable，返回DataTable
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataTable</returns>
        public DataTable QuerySchemaTable(string SQLString, SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                var reader = cmd.ExecuteReader();
                reader.Read();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string fieldType = reader.GetDataTypeName(i);
                    string fieldName = reader.GetName(i);
                    string fieldType2 = reader.GetFieldType(i).FullName;
                    Console.WriteLine(fieldName + "\t" + fieldType + "\t" + fieldType2);
                }
                var dt = reader.GetSchemaTable();
                reader.Close();
                return dt;
            }
        }
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public DataSet QueryCheck(string SQLString, SqlParameter[] cmdParms, SqlConnection conn, SqlTransaction trans = null)
        {
            SqlConnection connection = conn;
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, trans, SQLString, cmdParms);
            /* var reader = cmd.ExecuteReader();
             reader.Read();
             for (int i = 0; i < reader.FieldCount; i++)
             {
                 string fieldType = reader.GetDataTypeName(i);
                 string fieldName = reader.GetName(i);
                 string fieldType2 = reader.GetFieldType(i).FullName;
                 Console.WriteLine(fieldName + "\t" + fieldType + "\t" + fieldType2);
             }
             reader.Close();*/
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                try
                {
                    da.Fill(ds, "ds");
                    cmd.Parameters.Clear();
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {


                foreach (SqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }


        #endregion

        #region 存储过程操作

        /// <summary>
        /// 执行存储过程，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close ，2014-07-28设置了CommandTimeout为10分钟)
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlDataReader returnReader;
            connection.Open();
            SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            //2014-07-28设置了CommandTimeout为10分钟
            command.CommandTimeout = 600;
            command.CommandType = CommandType.StoredProcedure;
            returnReader = command.ExecuteReader(CommandBehavior.CloseConnection);
            return returnReader;

        }


        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.Fill(dataSet, tableName);
                connection.Close();
                return dataSet;
            }
        }

        public DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName, int Times)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.SelectCommand.CommandTimeout = Times;
                sqlDA.Fill(dataSet, tableName);
                connection.Close();
                return dataSet;
            }
        }


        /// <summary>
        /// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand</returns>
        private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter parameter in parameters)
            {
                if (parameter != null)
                {
                    // 检查未分配值的输出参数,将其分配以DBNull.Value.
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    command.Parameters.Add(parameter);
                }
            }

            return command;
        }

        /// <summary>
        /// 执行存储过程，返回影响的行数  
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        public int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                int result;
                connection.Open();
                SqlCommand command = BuildIntCommand(connection, storedProcName, parameters);
                command.CommandTimeout = 1200;//连接时间：秒
                rowsAffected = command.ExecuteNonQuery();
                result = (int)command.Parameters["ReturnValue"].Value;
                //Connection.Close();
                return result;
            }
        }

        /// <summary>
        /// 创建 SqlCommand 对象实例(用来返回一个整数值) 
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand 对象实例</returns>
        private static SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.Parameters.Add(new SqlParameter("ReturnValue",
                SqlDbType.Int, 4, ParameterDirection.ReturnValue,
                false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return command;
        }
        #endregion

        #region 导入Execl表格数据
        /// <summary> 
        /// 将 Excel 文件转成 DataTable
        ///</summary> 
        ///<param name="serverMapPathExcel">Excel文件及其路径</param>
        ///<param name="strSheetName">工作表名,如:Sheet1</param> 
        ///<param name="isTitleOrDataOfFirstRow">True 第一行是标题,False 第一行是数据</param> 
        ///<returns>DataTable</returns> 
        public static DataTable ExcelToDataTable(string serverMapPathExcel, string strSheetName, bool isTitleOrDataOfFirstRow)
        {
            string HDR = string.Empty;//如果第一行是数据而不是标题的话, 应该写: "HDR=No;"
            if (isTitleOrDataOfFirstRow)
                HDR = "YES";//第一行是标题 
            else
                HDR = "NO";//第一行是数据 
            //源的定义
            string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + serverMapPathExcel + ";" + "Extended Properties='Excel 12.0;HDR=" + HDR + ";IMEX=1';";
            //Sql语句 //string strExcel = string.Format("select * from [{0}$]", strSheetName); 这是一种方法 
            string strExcel = "select * from [" + strSheetName + "]";

            //定义存放的数据表
            DataSet ds = new DataSet();
            //连接数据源
            using (OleDbConnection conn = new OleDbConnection(strConn))
            {
                try
                {
                    conn.Open();
                    //适配到数据源
                    OleDbDataAdapter adapter = new OleDbDataAdapter(strExcel, strConn);
                    adapter.Fill(ds, strSheetName);
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return ds.Tables[strSheetName];
        }

        #endregion

        #region 执行数据添加操作

        /// <summary>
        /// 执行数据库更新操作
        /// </summary>
        /// <param name="commandText">设置数据源执行的语句</param>
        /// <param name="pars">SqlCommand的参数</param>
        /// <returns>返回影响的行数</returns>
        public int AddExecuteNonQuery(string commandText, params SqlParameter[] pars)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        //执行添加操作
                        PreCommand(cmd, connection, commandText, pars);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }
        /// <summary>
        /// 执行添加操作
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="conn">sql 数据库的打开连接</param>
        /// <param name="cmdText">设置数据源执行的语句</param>
        /// <param name="cmdParms">SqlCommand的参数</param>
        protected static void PreCommand(SqlCommand cmd, SqlConnection conn, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            //指定如何执行命令字符串
            cmd.CommandType = CommandType.Text;//cmdType;
            cmd.Parameters.AddRange(cmdParms);
        }

        #endregion



        #region JaneHe
        /// <summary>
        /// JaneHe 2020/1/21 批量将数据写入指定表中
        /// </summary> 
        /// <param name="TableName">表名</param>
        /// <param name="dt">数据源</param>
        public void SqlBulkCopyByDatatable(string TableName, DataTable dt)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlBulkCopy sqlbulkcopy = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.UseInternalTransaction))
                {
                    try
                    {
                        sqlbulkcopy.DestinationTableName = TableName;
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            sqlbulkcopy.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                        }
                        sqlbulkcopy.WriteToServer(dt);

                    }
                    catch (System.Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// JaneHe 20210624 新增异常信息插入到数据表中
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="biz"></param>
        /// <param name="ex"></param>
        public void AddErrorInfo(SqlParameter[] sp, Biz biz, Exception ex)
        {
            StringBuilder sbs = new StringBuilder();
            for (int index = 0; index < sp.Length; index++) {
                sbs.Append(@"["+ index +"]" + sp[index].ParameterName +":" + (sp[index].Value ?? "null") + "  ");
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(@"
declare @AI varchar(max)

set @AI = isnull((
select Top 1 
s_AI
from dbo.FRM_BizRecord with(nolock)
where s_BizName = @s_ErrorRecord_BizName
and n_state = 1 order by REPLACE(s_AI,'A','') desc),'A0')

INSERT INTO [dbo].[FRM_ErrorRecord]
           ([n_ErrorRecord_Id]
           ,[s_ErrorRecord_BizName]
           ,[s_ErrorRecord_sql1]
           ,[s_ErrorRecord_sql2]
           ,[s_ErrorRecord_sql3]
           ,[s_ErrorRecord_sql4]
           ,[s_ErrorRecord_sql5]
           ,[s_ErrorRecord_AI]
           ,[s_ErrorRecord_SqlParameter]
           ,[s_ErrorRecord_Msgs]
           ,[s_ErrorRecord_Operator]
           ,[s_ErrorRecord_Operatime]
           ,[s_ErrorRecord_IPAddress]
           ,[s_ErrorRecord_AgentName]
           ,[n_ErrorRecord_State]
           ,[s_ErrorRecord_Creator]
           ,[d_ErrorRecord_Creatime]
           ,[s_ErrorRecord_Updator]
           ,[d_ErrorRecord_Updatime])
     VALUES
           (NEWID() 
           ,@s_ErrorRecord_BizName
           ,@s_ErrorRecord_sql1
           ,@s_ErrorRecord_sql2 
           ,@s_ErrorRecord_sql3
           ,@s_ErrorRecord_sql4 
           ,@s_ErrorRecord_sql5 
           ,@AI
           ,@s_ErrorRecord_SqlParameter 
           ,@s_ErrorRecord_Msgs
           ,@s_ErrorRecord_Operator
           ,GETDATE()
           ,@s_ErrorRecord_IPAddress
           ,@s_ErrorRecord_AgentName
           ,1
           ,@s_ErrorRecord_Operator
           ,GETDATE()
           ,@s_ErrorRecord_Operator
           ,GETDATE())");

            try
            {
                SqlParameter[] spr = new SqlParameter[] { 
            new SqlParameter("@s_ErrorRecord_BizName",biz.BizName),
            new SqlParameter("@s_ErrorRecord_sql1",biz.Sqls.Count > 0?biz.Sqls[0].Sql:string.Empty),
            new SqlParameter("@s_ErrorRecord_sql2",biz.Sqls.Count > 1?biz.Sqls[1].Sql:string.Empty),
            new SqlParameter("@s_ErrorRecord_sql3",biz.Sqls.Count > 2?biz.Sqls[2].Sql:string.Empty),
            new SqlParameter("@s_ErrorRecord_sql4",biz.Sqls.Count > 3?biz.Sqls[3].Sql:string.Empty),
            new SqlParameter("@s_ErrorRecord_sql5",biz.Sqls.Count > 4?biz.Sqls[4].Sql:string.Empty), 
            new SqlParameter("@s_ErrorRecord_SqlParameter",sbs.ToJson()),
            new SqlParameter("@s_ErrorRecord_Msgs",ex.Message),
            new SqlParameter("@s_ErrorRecord_Operator",TopsUser.UserNO),
            new SqlParameter("@s_ErrorRecord_IPAddress",TopsUser.IpAddress),
            new SqlParameter("@s_ErrorRecord_AgentName",TopsUser.AgentName)
            };

                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    using (SqlCommand sc = new SqlCommand())
                    {
                        //执行添加操作
                        PreCommand(sc, cn, sb.ToString(), spr);
                        int rows = sc.ExecuteNonQuery();
                        sc.Parameters.Clear();
                    }
                }
            }
            catch (Exception exs)
            {
                 
            }
        }

        #endregion
    }

}


