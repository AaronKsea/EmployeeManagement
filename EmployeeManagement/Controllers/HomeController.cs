using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeRepo _employeeRepo;
        public HomeController(IEmployeeRepo employeeRepo)
        {
            _employeeRepo = employeeRepo;
        }

        public ViewResult Index()
        {
            return View(_employeeRepo.GetEmployeeList());
        }

        public ViewResult Details(int? id)
        {
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel();

            Employee model = _employeeRepo.GetEmployeeById(id ?? 1);
            homeDetailsViewModel.employee = model;
            homeDetailsViewModel.Title = "Employee Info";

            return View(homeDetailsViewModel);
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                Employee newEmp = _employeeRepo.AddEmployee(employee);
                //return RedirectToAction("Details", new { id = newEmp.EmployeeID });
            }
            return View();
        }
    }
}
