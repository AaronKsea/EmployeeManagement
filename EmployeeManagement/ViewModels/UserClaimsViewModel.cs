using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    public class UserClaimsViewModel
    {
        public string ClaimType { get; set; }
        public bool IsSelected { get; set; }
    }
}
