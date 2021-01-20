using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    //[Authorize(Roles = "Admin,Site Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<AdministrationController> logger;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
                                        UserManager<ApplicationUser> userManager,
                                        ILogger<AdministrationController> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel createRoleViewModel)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = createRoleViewModel.Role
                };
                IdentityResult identityResult = await roleManager.CreateAsync(identityRole);
                if (identityResult.Succeeded)
                {
                    return RedirectToAction("GetRoles", "Administration");
                }
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }

            };
            return View();
        }

        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }


        [HttpGet]
        //[Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRoles(string RoleId)
        {
            // Find the role by Role ID
            var role = await roleManager.FindByIdAsync(RoleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {RoleId} cannot be found";
                return View("NotFound");
            }

            var model = new EditRoleViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name
            };

            // Retrieve all the Users
            //foreach (var user in userManager.Users)
            //InvalidOperationException: There is already an open DataReader associated with this Connection which must be closed first.
            //The problem is you can't have multiple connection at the same time on a DbContext and when you have a foreach on userManager.Users the connection is open and
            //the records are returning one by one and you can't have another query (IsInRole) on the same DbContext.
            //By adding a ToList() to userManager.Users you are getting all the users and the iterating on them.


            /* there is lazy loading enabled in Users property so for each iterations connection is opened, since first iteration opens the connection on dbcontext
            second query is trying to be executed without closing first connection. So this can be avoided by calling ToList() to carry out immediate execution and 
            all objects are loaded in memory and we are querying against that object instead datasource */
            foreach (var user in userManager.Users.ToList())
            {
                // If the user is in this role, add the username to
                // Users property of EditRoleViewModel. This model
                // object is then passed to the view for display
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> EditRoles(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.RoleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.RoleId} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("GetRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.RoleId = roleId;
            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }
            var userRoleViews = new List<UserRoleViewModel>();

            foreach (var user in userManager.Users.ToList())
            {
                var model = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    IsSelected = await userManager.IsInRoleAsync(user, role.Name)

                };
                userRoleViews.Add(model);

            }

            return View(userRoleViews);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }
            IdentityResult result = null;
            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && (await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if (result.Succeeded)
                {
                    if (i < model.Count - 1)
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("EditRoles", new { RoleId = roleId });
                    }
                }
            }
            return RedirectToAction("EditRoles", new { RoleId = roleId });
        }

        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = userManager.Users.ToList();
            return View(users);

        }

        [HttpGet]
        public async Task<IActionResult> EditUsers(string UserId)
        {
            var user = await userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id: {UserId} cannot be found";
                return View("NotFound");
            }

            // GetClaimsAsync retunrs the list of user Claims
            var userClaims = await userManager.GetClaimsAsync(user);
            // GetRolesAsync returns the list of user Roles
            var userRoles = await userManager.GetRolesAsync(user);

            var userModel = new EditUserViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                City = user.City,
                Email = user.Email,
                Roles = userRoles,
                Claims = userClaims.Select(c => c.Type + " : " + c.Value).ToList()
            };

            return View(userModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsers(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id: {model.UserId} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.City = model.City;

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }


        [HttpPost]
        public async Task<IActionResult> DeleteUser(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id: {Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("ListUsers");
            }
        }

        [HttpPost]
        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteRole(string Id)
        {
            var role = await roleManager.FindByIdAsync(Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id: {Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                try
                {
                    var result = await roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("GetRoles");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View("GetRoles");
                }
                catch (DbUpdateException ex)
                {
                    logger.LogError($"Exception Occured : {ex}");
                    // Pass the ErrorTitle and ErrorMessage that you want to show to
                    // the user using ViewBag. The Error view retrieves this data
                    // from the ViewBag and displays to the user.
                    ViewBag.ErrorTitle = $"{role.Name} role is in use";
                    ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted as there are users in this role. If you want to delete this role, please remove the users from the role and then try to delete";
                    return View("Error");
                }

            }
        }

        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")] 
        public async Task<IActionResult> ManageUserRoles(string UserId)
        {
            var user = await userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id: {UserId} cannot be found";
                return View("NotFound");
            }
            else
            {
                ViewBag.UserID = UserId;
                var roles = new List<UserRolesViewModel>();
                foreach (var role in roleManager.Roles.ToList())
                {
                    var alreadyInRole = await userManager.IsInRoleAsync(user, role.Name);
                    var model = new UserRolesViewModel
                    {
                        RoleId = role.Id,
                        RoleName = role.Name,
                        IsSelected = alreadyInRole

                    };
                    roles.Add(model);
                }
                return View(roles);
            }
        }
        
        
        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> userRolesViewModels, string UserId)
        {
            var user = await userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id: {UserId} cannot be found";
                return View("NotFound");
            }
            else
            {
                var roles = await userManager.GetRolesAsync(user);
                var result = await userManager.RemoveFromRolesAsync(user, roles);//remove all existing roles of user first

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Error removing existing roles of the user");
                    return View(userRolesViewModels);
                }

                result = await userManager.AddToRolesAsync(user, userRolesViewModels.Where(x => x.IsSelected == true).Select(y => y.RoleName));//add selected roles from view
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Error adding user roles");
                    return View(userRolesViewModels);
                }
                return RedirectToAction("EditUsers", new { UserId = UserId });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string UserId)
        {
            var user = await userManager.FindByIdAsync(UserId);
            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with Id: {UserId} cannot be found";
                return View("NotFound");
            }

            ViewBag.UserId = UserId;
            var claims = await userManager.GetClaimsAsync(user);
            var userClaims = new List<UserClaimsViewModel>();
            foreach(var claim in ClaimStore.AllClaims)
            {
                var claimExist = claims.Any(c => c.Type == claim.Type && c.Value == "true");
                var claimsModel = new UserClaimsViewModel
                {
                    ClaimType = claim.Type,
                    IsSelected = claimExist
                };
                userClaims.Add(claimsModel);
            }
            return View(userClaims);
        }
        
        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(List<UserClaimsViewModel> models, string UserId)
        {
            var user = await userManager.FindByIdAsync(UserId);
            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with Id: {UserId} cannot be found";
                return View("NotFound");
            }
            else
            {
                var claims = await userManager.GetClaimsAsync(user);
                var result = await userManager.RemoveClaimsAsync(user, claims);//remove all claims first
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Error removing user claims;");
                    View(models);
                }

                var selectedClaims = models.Select(x => new Claim(x.ClaimType, x.IsSelected ? "true" : "false"));
                result = await userManager.AddClaimsAsync(user, selectedClaims); // add selected claims in database
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Error removing user claims;");
                    View(models);
                }
                return RedirectToAction("EditUsers", new { UserId = UserId });
            }
        }
    }
}
 