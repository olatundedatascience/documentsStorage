using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Collections;


namespace XMN.DataLayer
{
    public partial class DataAccessLayer
    {
        public SqlConnection Conn;
        private string mvarErrorMessage;
        private ArrayList SQLStm = new ArrayList();
        private int mytimeout;

        public DataAccessLayer()
        {
            Conn = new SqlConnection();
        }

        public int SetTimeout
        {
            get { return mytimeout; }
            set
            {
                mytimeout = value;
                //   Conn.ConnectionTimeout = value;
            }

        }

        public string ErrorMessage
        {
            get { return mvarErrorMessage; }
            set { mvarErrorMessage = value; }
        }

        public string CreateTempTable(string tempName, string baseTable, string query = "")
        {

            string TempTableName = "";
            string SQLQuery = null;
            long n = 0;
            Random radNum = new Random();
            n = radNum.Next(1, 10000);

            TempTableName = "tmp" + tempName + n.ToString();

            SQLQuery = "if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[" + TempTableName + " ]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)";
            SQLQuery = SQLQuery + " drop table " + TempTableName;

            SQLStm.Add(SQLQuery);

            if (string.IsNullOrEmpty(query))
            {
                SQLQuery = "Select * into " + TempTableName + " from " + baseTable + " where 1 = 2 ";
            }
            else
            {
                SQLQuery = query;
            }

            SQLStm.Add(SQLQuery);

            try
            {
                ExecuteBatch();
            }
            catch (Exception)
            {
                TempTableName = "";


            }

            return TempTableName;

        }

        public void AddStatement(string SQLQuery)
        {
            SQLStm.Add(SQLQuery);
        }

        public void Clearbatch()
        {
            SQLStm.Clear();
        }

        public bool ExecuteBatch()
        {
            bool ExecStatus = true;

            if ((SQLStm.Count == 0))
            {
                mvarErrorMessage = "No Query in List to Execute";
                ExecStatus = false;
                return ExecStatus;
            }

            int i = 0;
            int p = 0;

            SqlTransaction SQLTrans = null;

            openConnection();

            SQLTrans = Conn.BeginTransaction();

            SqlCommand mycmd = new SqlCommand();
            mycmd.Connection = Conn;
            mycmd.Transaction = SQLTrans;

            for (i = 0; i <= SQLStm.Count - 1; i++)
            {
                try
                {
                    mycmd.CommandText = SQLStm[i].ToString();
                    p = mycmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    ExecStatus = false;
                    mvarErrorMessage = ex.Message.ToString();
                    break;
                }
            }

            if (ExecStatus)
                SQLTrans.Commit();
            else
                SQLTrans.Rollback();

            SQLStm.Clear();

            SQLTrans.Dispose();

            Conn.Close();

            return ExecStatus;


        }

        public DataSet GetDataSet(string myQuery)
        {
            if (string.IsNullOrEmpty(myQuery))
            {
                throw new Exception("Invalid Query");

            }
            DataSet ds = new DataSet();
            openConnection();
            try
            {
                SqlDataAdapter adapt = new SqlDataAdapter(myQuery, Conn);
                adapt.Fill(ds);
            }
            catch (Exception)
            {
            }
            Conn.Close();
            return ds;

        }

        public DataTable GetDataTable(string myQuery)
        {
            if (string.IsNullOrEmpty(myQuery))
            {
                throw new Exception("Invalid Query");
            }
            SqlDataReader dr = null;
            DataTable dt = new DataTable();
            openConnection();
            SqlCommand mycmd = null;
            mycmd = new SqlCommand(myQuery, Conn);
            mycmd.CommandTimeout = 300;
            try
            {
                dr = mycmd.ExecuteReader();
                dt.Load(dr);
                dr.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            Conn.Close();
            return dt;

        }

        public int ExecuteQuery(string myquery)
        {
            if (string.IsNullOrEmpty(myquery))
            {
                throw new Exception("Invalid Query");
            }
            openConnection();
            int i = 0;
            try
            {
                SqlCommand mycmd = new SqlCommand(myquery, Conn);
                i = mycmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            Conn.Close();
            return i;
        }

        public SqlDataReader GetReader(string myQuery)
        {
            SqlDataReader myReader = null;
            SqlCommand mycmd = null;
            openConnection();
            try
            {
                mycmd = new SqlCommand(myQuery, Conn);
                myReader = mycmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            Conn.Close();
            return myReader;
        }

        public DataSet GetDataSet(string SQLStm, string opt)
        {
            DataSet ds = null;
            ds = GetDataSet(SQLStm);
            return ds;
        }

        public string GetRowData(DataSet DataSetObj, string ColumnName)
        {
            string Datavalue = "";
            DataRow dr = null;

            foreach (DataRow dr_loopVariable in DataSetObj.Tables[0].Rows)
            {
                dr = dr_loopVariable;
                Datavalue = "" + dr[ColumnName];
            }

            return Datavalue;
        }

        public DataRow GetDataRow(string tableSqlQuery, int rowIndex = 0)
        {
            var tbl = GetDataTable(tableSqlQuery);
            if (tbl.Rows.Count == 0)
                return null;
            return tbl.Rows[rowIndex];
        }

        private void openConnection()
        {
            //try
            //{
                if (Conn.State != ConnectionState.Open)
                {
                    Conn.ConnectionString = DataAccessLayer.ConnectionString;
                    //   Conn.ConnectionTimeout = 600;
                    Conn.Open();
                }
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}
        }

        public static string ConnectionString
        {
            //  get { return string.Format("Data Source={0};Initial Catalog={1};User ID={2}; password={3} ;Timeout=300 ", ".", "Online_Regstar", "sa", "TINNITMXN@@!"); }

            get { return string.Format("Data Source={0};Initial Catalog={1};User ID={2}; password={3} ;Timeout=300 ", ".\\newapps", "Regstar", "sa", "TINNITMXN@@!"); }

           // get { return string.Format("Data Source={0};Initial Catalog={1};User ID={2}; password={3} ;Timeout=300 ", "pacwebserver", "Online_Regstar", "sa", "pacadminsvr14+"); }
            // get { return string.Format("Data Source={0};Initial Catalog={1};User ID={2}; password={3} ;Timeout=300 ", ".", "Online_Regstar", "sa", "aremu"); }
        }

        public static string ProviderName
        {
            get { return "System.Data.SqlClient"; }
        }

        public int IncrementIdentityField(string fieldName)
        {
            string myQuery = string.Format(" if not exists (select * from identity_table where field_name = '{0}') insert into identity_table(field_name,current_record) values('{0}',0) ", fieldName);
            myQuery += string.Format(" update identity_table set current_record = current_record + 1 where field_name = '{0}' ", fieldName);
            myQuery += string.Format(" select * from identity_table where field_name = '{0}' ", fieldName);

            var row = GetDataRow(myQuery);
            return (int)row["current_record"];
        }
    }

    public class CacheData
    {
        // DataAccessLayer mydata;

        public CacheData()
        {
            // mydata = new DataAccessLayer();
        }

        public void LoadAll()
        {


        }

        public void Reset(string cacheKey)
        {
            HttpContext.Current.Cache.Remove(cacheKey);
        }

        
    }
}