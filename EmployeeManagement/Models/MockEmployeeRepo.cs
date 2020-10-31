using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class MockEmployeeRepo : IEmployeeRepo
    {
        private List<Employee> employees;

        public MockEmployeeRepo()
        {
            employees = new List<Employee>()
            {
                new Employee{ EmployeeID=1,Name = "AaronK", Email ="kseaAaron", Department =Dept.IT},
                new Employee{ EmployeeID=2,Name = "Puru", Email ="kseaPuru", Department=Dept.Management},
                new Employee{ EmployeeID=3,Name = "Srijan", Email ="karkiSrijan", Department=Dept.HR},
                //new Employee{ EmployeeID=4,Name = "Eva", Email ="KarkiEva"},

            };
        }

        public Employee AddEmployee(Employee employee)
        {
            employee.EmployeeID = employees.Max(e => e.EmployeeID) + 1;
            employees.Add(employee);
            return employee;
        }

        public void Delete(int id)
        {
            Employee employee = employees.FirstOrDefault(e => e.EmployeeID == id);
            if (employee != null)
            {
                employees.Remove(employee);
            }
        }

        public Employee GetEmployeeById(int employeeId)
        {
            return employees.FirstOrDefault(id => id.EmployeeID == employeeId);
        }

        public IEnumerable<Employee> GetEmployeeList()
        {
            return employees;
        }

        public Employee update(Employee employeeChanges)
        {
            Employee emp = employees.FirstOrDefault(e => e.EmployeeID == employeeChanges.EmployeeID);
            if (emp != null)
            {
                emp.Name = employeeChanges.Name;
                emp.Department = employeeChanges.Department;
                emp.Email = employeeChanges.Email;
            }
            return emp;
        }
    }
}
