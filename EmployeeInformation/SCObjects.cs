using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeInformation
{
    public static class SCObjects
    {
        public static string ExecNonQuery(SqlCommand cmd, string SqlConnectionString)
        {
            string result = string.Empty;

            SqlConnection cn = new SqlConnection();
            try
            {
                cn.ConnectionString = SqlConnectionString;
                cn.Open();
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();

                result = "Process has been successfully completed.";

            }
            catch (Exception ex)
            {
                result = "Error(s) : \n" + ex.Message;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                    cn.Close();
            }

            return result;
        }
        public static DataTable ExecGetData(SqlCommand cmd,string SqlConnectionString)
        {
            DataTable dt = new DataTable();

            SqlConnection cn = new SqlConnection();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                cn.ConnectionString = SqlConnectionString;
                cn.Open();
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                da.SelectCommand = cmd;
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                dt = null;
                //throw new Exception();
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                    cn.Close();
            }

            return dt;
        }
    }
}
