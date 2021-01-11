﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        [Display(Name ="RoleName")]
        public string Role { get; set; }
    }
}
