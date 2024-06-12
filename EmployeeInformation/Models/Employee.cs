using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeInformation.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public DateTime Birthdate { get; set; }
        public string Birthplace { get; set; }
        public string ContactNo { get; set; }
        public string Password { get; set; }

        public int OTP { get; set; }
        public int VerifyOTP { get; set; }


    }

    public enum Gender
    {
        Male,
        Female,
        Others
    }

}
