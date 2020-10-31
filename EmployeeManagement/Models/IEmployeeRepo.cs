using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public interface IEmployeeRepo
    {
        Employee GetEmployeeById(int employeeId);
        IEnumerable<Employee> GetEmployeeList();
        Employee AddEmployee(Employee employee);
        Employee update(Employee employee);
        void Delete(int id);
    }
}
