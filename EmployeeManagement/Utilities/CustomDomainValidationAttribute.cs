using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Utilities
{
    public class CustomDomainValidationAttribute : ValidationAttribute
    {
        private readonly string allowedDomain;

        public CustomDomainValidationAttribute(string allowedDomain)
        {
            this.allowedDomain = allowedDomain;
        }
        public override bool IsValid(object value)
        {
            string[] mail = value.ToString().Split("@");
            return mail[1].ToUpper() == allowedDomain.ToUpper();

        }
    }
}
