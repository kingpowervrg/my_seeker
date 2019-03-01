using Mono.Data.Sqlite;
using System;

namespace SqliteDriver
{
    public class SQLiteHelper
    {
        public static Action<string> OnLog, OnError;
        private static SQLiteHelper m_instance;
        static SqliteDatabase sqlConnection;

        public static SQLiteHelper Instance()
        {
            if (m_instance == null)
            {

            }
            return m_instance;
        }

        public SQLiteHelper(string connectionStr)
        {
            OpenSql(connectionStr);
        }

        public static void Initialize(string dbname, Action<string> onLog, Action<string> onError)
        {
            OnLog = onLog;
            OnError = onError;
            m_instance = new SQLiteHelper(dbname);
        }

        /// <summary>
        /// 创建数据库连接
        /// </summary>
        /// <param name="connectionStr"></param>
        private void OpenSql(string connectionStr)
        {
            try
            {
                sqlConnection = new SqliteDatabase(connectionStr);
                sqlConnection.Open();
                OnLog("数据库: " + connectionStr + " 连接成功!!!");
            }
            catch (Exception e)
            {
                if (OnError != null)
                    OnError("数据库打开异常" + e.ToString());
            }
        }
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void CloseSql()
        {
            try
            {
                if (sqlConnection != null)
                {
                    sqlConnection.Close();
                    SqliteConnection.ClearAllPools();
                    sqlConnection = null;
                }
                m_instance = null;
                OnLog("数据库关闭。");
            }
            catch (Exception e)
            {
                if (OnError != null)
                    OnError("数据库关闭异常!!!\n" + e.ToString());
            }

        }
        /// <summary>
        /// 执行读取命令
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <returns></returns>
        public DataTable ExecuteQuery(string sqlQuery)
        {
            DataTable row;
            try
            {
                //sqlDataReader = sqlCommand.ExecuteReader();
                row = sqlConnection.ExecuteQuery(sqlQuery);
            }
            catch (Exception e)
            {
                if (OnError != null)
                    OnError("sqlQuery:" + sqlQuery + "    sqlCommand.ExecuteReader -> Exception:" + e);
                return null;
            }
            return row;
        }

        /// <summary>
        /// 读取全部表数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="idTitle"></param>
        /// <param name="valueTitle"></param>
        /// <returns></returns>
        public DataTable GetReadFullTable(string name)
        {
            string query = "SELECT * FROM " + name;
            //SqliteDataReader sqReader = ExecuteQuery(query);
            DataTable sqReader = ExecuteQuery(query);

            if (sqReader == null)
            {
                if (OnError != null)
                    OnError("SqliteDataReader 数据库读取为空 !!!");
                return null;
            }
            return sqReader;
        }
        public DataTable GetSelectWhereCondition(string name, string fieldName, object fieldValue)
        {
            string query = "select * from " + name + " where " + fieldName + "= " + (fieldValue is string ? ("'" + fieldValue + "'") : (fieldValue));

            DataTable sqReader = ExecuteQuery(query);

            if (sqReader == null)
            {
                if (OnError != null)
                    OnError("SqliteDataReader 数据库读取为空 !!!");
                return null;
            }
            return sqReader;
        }
        public DataTable GetSelectWhereConditionStr(string name, string conditionStr)
        {
            string query = "select * from " + name + " where " + conditionStr;

            DataTable sqReader = ExecuteQuery(query);

            if (sqReader == null)
            {
                if (OnError != null)
                    OnError("SqliteDataReader 数据库读取为空 !!!");
                return null;
            }
            return sqReader;
        }
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="idTitle"></param>
        /// <param name="valueTitle"></param>
        /// <returns></returns>
        /// 
        public DataTable GetSelectWhere(string name, object sn)
        {
            string query = "select * from " + name + " where id= " + (sn is string ? ("'" + sn + "'") : (sn));

            DataTable sqReader = ExecuteQuery(query);

            if (sqReader == null)
            {
                if (OnError != null)
                    OnError("SqliteDataReader 数据库读取为空 !!!");
                return null;
            }
            return sqReader;
        }

        /// <summary>
        /// 查询指定的列
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnsName"></param>
        /// <returns></returns>
        public DataTable GetSpecificColumns(string tableName, params string[] columnsName)
        {
            string query = "select {0} from {1}";
            string selectColunms = "*";
            if (columnsName.Length > 0)
            {
                selectColunms = string.Empty;
                for (int i = 0; i < columnsName.Length; ++i)
                {
                    selectColunms += columnsName[i];
                    if (i != columnsName.Length - 1)
                        selectColunms += ",";
                }
            }

            string queryStr = string.Format(query, selectColunms, tableName);
            DataTable sqlReader = ExecuteQuery(queryStr);

            if (sqlReader == null)
            {
                if (OnError != null)
                    OnError("SqliteDataReader 数据库读取为空 !!!");
                return null;
            }
            return sqlReader;


        }

        /// <summary>
        /// 获取表数据数量
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetSelectCount(string name)
        {
            string query = "select count(sn) from " + name;

            var sqReader = ExecuteQuery(query);

            if (sqReader == null)
            {
                if (OnError != null)
                    OnError("SqliteDataReader 数据库读取为空 !!!");
                return 0;
            }
            sqReader.Read();
            return sqReader.GetInt32(0);
        }
    }
}
