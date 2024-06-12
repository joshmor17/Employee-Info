using EmployeeInformation.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeInformation.Services
{
    public class EmployeeServices
    {
        public string UserConnectionString { get; set; }
        private string StoredProcedure = "[dbo].[spEmployee]";
        //Table

        public IEnumerable<Employee> Employees()
        {
            List<Employee> items = new List<Employee>();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@iquerytype", 0);

            DataTable dt = SCObjects.ExecGetData(cmd, UserConnectionString);
            if(dt != null)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    var employee = new Employee
                    {
                        Id = int.Parse(dr["Employeeno"].ToString()),
                        Firstname = dr["tfirstname"].ToString(),
                        Middlename = dr["tmiddlename"].ToString(),
                        Lastname = dr["tlastname"].ToString(),
                        Email = dr["temail"].ToString(),
                        Gender = dr["tgender"].ToString(),
                        Age = int.Parse(dr["iAge"].ToString()),
                        Birthdate = DateTime.Parse(dr["dbirthdate"].ToString()),
                        Birthplace = dr["tbirthplace"].ToString(),
                        ContactNo = dr["icontact"].ToString(),

                    };
                    items.Add(employee);
                }
            }

            return items;
        }
        public string Save(Employee item)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@tfirstname", item.Firstname);
            cmd.Parameters.AddWithValue("@tmiddlename", item.Middlename);
            cmd.Parameters.AddWithValue("@tlastname", item.Lastname);
            cmd.Parameters.AddWithValue("@temail", item.Email);
            cmd.Parameters.AddWithValue("@tGender", item.Gender.ToString());
            cmd.Parameters.AddWithValue("@iAge", item.Age.ToString());
            cmd.Parameters.AddWithValue("@dBirthDate", item.Birthdate.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@tBirthplace", item.Birthplace);
            cmd.Parameters.AddWithValue("@iContactNo", item.ContactNo);
            cmd.Parameters.AddWithValue("@tPassword", item.Password);
            cmd.Parameters.AddWithValue("@iquerytype", 1);
            return SCObjects.ExecNonQuery(cmd, UserConnectionString);
        }

        public string Login(Employee model)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EmployeeID", model.Id);
            cmd.Parameters.AddWithValue("@tPassword", model.Password);
            cmd.Parameters.AddWithValue("@iQueryType", 2);
            
            return SCObjects.ExecNonQuery(cmd, UserConnectionString);
        }

        public string Delete(int id)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@oid", id);
            cmd.Parameters.AddWithValue("@iquerytype", 3);
            return SCObjects.ExecNonQuery(cmd, UserConnectionString);
        }

        public string GetEmail(Employee model, int EmployeeID)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);
            cmd.Parameters.AddWithValue("@iquerytype", 4);

            DataTable dt = SCObjects.ExecGetData(cmd, UserConnectionString);
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                   string email = dr["temail"].ToString();
                }
                
            }

            return dt.Rows[0][0].ToString();
        }
    }
}
