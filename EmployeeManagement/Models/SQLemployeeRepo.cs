using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class SQLemployeeRepo : IEmployeeRepo
    {
        private readonly AppDBContext _appDBContext;

        public SQLemployeeRepo(AppDBContext appDBContext)
        {
            _appDBContext = appDBContext;
        }
        public Employee AddEmployee(Employee employee)
        {
            _appDBContext.Employees.Add(employee);
            _appDBContext.SaveChanges();
            return employee;
        }

        public void Delete(int id)
        {
            Employee employee = _appDBContext.Employees.FirstOrDefault(e => e.EmployeeID == id);
            if (employee != null)
            {
                _appDBContext.Employees.Remove(employee);
                _appDBContext.SaveChanges();
            }
        }

        public Employee GetEmployeeById(int employeeId)
        {
            return _appDBContext.Employees.FirstOrDefault(id => id.EmployeeID == employeeId);
        }

        public IEnumerable<Employee> GetEmployeeList()
        {
            return _appDBContext.Employees;
        }

        public Employee update(Employee employeeChanges)
        {
            var employee = _appDBContext.Employees.Attach(employeeChanges);
            employee.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _appDBContext.SaveChanges();
            return employeeChanges;
        }
    }
}
