using EmployeeInformation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeInformation
{
    public class EmployeeIndexViewModel
    {
        public IEnumerable<Employee> Employees { get; set; }
        public IEnumerable<Authentication> Authentications { get; set; }
        public IPAddressList IPAddressList { get; set; }
        public Employee Employee { get; set; }
        public string CommandType { get; set; }
    }
}
