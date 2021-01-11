using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace EmployeeManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeRepo _employeeRepo;
        private readonly IWebHostEnvironment _hostEnvironment;

        public HomeController(IEmployeeRepo employeeRepo,
                              IWebHostEnvironment hostEnvironment)
        {
            _employeeRepo = employeeRepo;
            _hostEnvironment = hostEnvironment;
        }

        [AllowAnonymous]
        public ViewResult Index()
        {
            return View(_employeeRepo.GetEmployeeList());
        }

        public ViewResult Details(int? id)
        {
            Employee model = _employeeRepo.GetEmployeeById(id.Value);
            if(model == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id.Value);
            }


            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel();

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
        [Authorize]
        public IActionResult Create(EmployeeCreateViewModel employeeCreateViewModel)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadedFileName(employeeCreateViewModel);
                Employee newEmp = new Employee()
                {
                    Name = employeeCreateViewModel.Name,
                    Department = employeeCreateViewModel.Department,
                    Email = employeeCreateViewModel.Email,
                    PhotoPath = uniqueFileName
                };
                _employeeRepo.AddEmployee(newEmp);
                return RedirectToAction("Details", new { id = newEmp.EmployeeID });
            }
            return View();
        }

        [HttpGet]
        [Authorize]
        public ViewResult Edit(int employeeid)
        {

            Employee employee = _employeeRepo.GetEmployeeById(employeeid);

            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel()
            {
                EmployeeId = employee.EmployeeID,
                Email = employee.Email,
                Department = employee.Department,
                Name = employee.Name,
                ExistingPhotoPath = employee.PhotoPath
            };

            return View(employeeEditViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel employeeEditViewModel)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeeRepo.GetEmployeeById(employeeEditViewModel.EmployeeId);
                employee.Name = employeeEditViewModel.Name;
                employee.Department = employeeEditViewModel.Department;
                employee.Email = employeeEditViewModel.Email;
                if(employeeEditViewModel.Photo != null)
                {
                    if(employeeEditViewModel.ExistingPhotoPath != null)
                    {
                        string existingPath = Path.Combine(_hostEnvironment.WebRootPath, "Images", employeeEditViewModel.ExistingPhotoPath);
                        if (System.IO.File.Exists(existingPath))
                            System.IO.File.Delete(existingPath);
                    }
                    employee.PhotoPath = UploadedFileName(employeeEditViewModel);
                }
                _employeeRepo.update(employee);
                return RedirectToAction("Details", new { id = employee.EmployeeID });
            }
            return View();
        }

        private string UploadedFileName(EmployeeCreateViewModel employeeEditViewModel)
        {
            string uniqueFileName = string.Empty;
            if (employeeEditViewModel.Photo != null)
            {
                string uploadfolder = Path.Combine(_hostEnvironment.WebRootPath, "Images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + employeeEditViewModel.Photo.FileName;
                string exactImagePath = Path.Combine(uploadfolder, uniqueFileName);
                using (FileStream fileToSave = new FileStream(exactImagePath, FileMode.Create))
                {
                    employeeEditViewModel.Photo.CopyTo(fileToSave);//create new file on exactImagePath 
                }
            }

            return uniqueFileName;
        }
    }
}
