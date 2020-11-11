using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public static class ModelBuilderExtension
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                new Employee()
                {
                    EmployeeID = 1,
                    Name = "PuruKC",
                    Email = "Puru@201gmail.com",
                    Department = Dept.HR
                },
                new Employee()
                {
                    EmployeeID = 2,
                    Name = "PuruKC",
                    Email = "Puru202gmail.com",
                    Department = Dept.HR
                });
        }
    }
}
