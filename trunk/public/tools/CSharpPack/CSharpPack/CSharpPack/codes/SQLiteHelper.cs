using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ClientResourcePacker
{
    public class SQLiteHelper
    {
        /// <summary>
        /// 数据库连接定义
        /// </summary>
        private SQLiteConnection dbConnection;

        private static SQLiteHelper instance;

        //工程需要使用NuGet 添加sqlite-net-pcl依赖，否则会报e_sqlite3.dll问题，连接无法建立
        //命令:PM> Install-Package sqlite-net-pcl
        public SQLiteHelper(string dbPath, bool deleteExist = true)
        {
            try
            {
                if (File.Exists(dbPath))
                {
                    if (PathUtil.IsFileInUsing(dbPath))
                    {
                        Console.WriteLine("数据库文件被其它进程占用");
                        return;
                    }
                }

                if (File.Exists(dbPath) && deleteExist)
                    File.Delete(dbPath);

                dbConnection = new SQLiteConnection(dbPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static SQLiteHelper Instance
        {
            get
            {
                if (instance == null)
                    instance = new SQLiteHelper(PathConfig.SQLITE_DB_PATH);

                return instance;
            }
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void CloseConnection()
        {
            //销毁Connection
            if (dbConnection != null)
            {
                dbConnection.Close();
            }
            dbConnection = null;
        }
        /// <summary>
        /// 向指定数据表中插入数据
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="blobs"></param>
        public void InsertValues(string strSql, object[] blobs)
        {
            dbConnection.Query<object>(strSql, blobs);
        }
        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="colNames"></param>
        /// <param name="colType"></param>
        public void CreateTable(string tableName, string[] colNames, string[] colType)
        {
            StringBuilder _builder = new StringBuilder();
            _builder.Append("CREATE TABLE IF NOT EXISTS " + tableName + " (");
            for (int i = 0; i < colNames.Length; i++)
            {
                if (i == colNames.Length - 1)
                {
                    _builder.Append("'" + colNames[i] + "'" + " " + colType[i]);
                }
                else
                {
                    if (i == 0)
                    {
                        _builder.Append("'" + colNames[i] + "'" + " " + colType[i] + " PRIMARY KEY NOT NULL , ");
                    }
                    else
                    {
                        _builder.Append("'" + colNames[i] + "'" + " " + colType[i] + ", ");
                    }
                }
            }
            _builder.Append("  ) ");
            dbConnection.Query<object>(_builder.ToString());
        }

        /// <summary>
        /// 读取全部表数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="idTitle"></param>
        /// <param name="valueTitle"></param>
        /// <returns></returns>
        public void ReadFullTable(string name, string idTitle, string valueTitle)
        {
            string query = "SELECT * FROM " + name;
            dbConnection.Query<object>(query);
        }
        /// <summary>
        ///  插入数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void InsertInto(string name, string id, string value)
        {
            string query = "INSERT INTO " + name + " VALUES (" + "'" + id + "'" + ", " + "'" + value + "'" + ")";
            dbConnection.Query<object>(query);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void UpdateInto(string name, string id, string value)
        {
            string query = "UPDATE " + name + " SET " + "value" + " = " + "'" + value + "'" + " WHERE " + "id" + " = " + "'" + id + "'";

            dbConnection.Query<object>(query);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteInfo(string name, string id)
        {
            if (id == null || "".Equals(id))
            {
                Console.WriteLine("deleteInto--------ID为空--------表名为:" + name);
                return false;
            }

            string query = "DELETE FROM " + name + " WHERE " + "id" + " = " + "'" + id + "'";

            dbConnection.Query<object>(query);

            return true;

        }
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="idTitle"></param>
        /// <param name="valueTitle"></param>
        /// <returns></returns>
        public List<object> SelectWhere(string name, string id, string idTitle, string valueTitle)
        {
            string query = "SELECT " + valueTitle + " FROM " + name + " WHERE " + idTitle + " = " + "'" + id + "'";

            return dbConnection.Query<object>(query);
        }

        public SQLiteConnection SqliteConnection
        {
            get { return dbConnection; }
        }
    }
}

