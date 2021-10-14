using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAppEs.Enums;
using WebAppEs.Handlers;
using WebAppEs.Models;
using WebAppEs.Services;
using WebAppEs.ViewModel.Category;

namespace WebAppEs.Controllers
{
    public class SubCategoryController : Controller
    {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ISetupService _setupService;
		private readonly ILogger<AdminController> _logger;

		public SubCategoryController(
				UserManager<ApplicationUser> userManager,
				RoleManager<IdentityRole> roleManager,
				ISetupService setupService,
				ILogger<AdminController> logger)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_setupService = setupService;
			_logger = logger;
		}
		public IActionResult Index()
		{
			var employeeID = HttpContext.Session.GetString("EmployeeID");
			if (employeeID == null)
			{
				return RedirectToAction("Logout", "Account");
			}

			var DataModel = _setupService.GetSubCatList();
			return View(DataModel);
		}

		public IActionResult CreateSubCategory(Guid Id)
		{
			var employeeID = HttpContext.Session.GetString("EmployeeID");
			if (employeeID == null)
			{
				return RedirectToAction("Logout", "Account");
			}

			var GetSubCategory = _setupService.GetSubCategory2(Id);
			var CategoryList = _setupService.GetAllCategoryList();
			MRNDQC_SubCategoryVM viewModel = new MRNDQC_SubCategoryVM();
			if (GetSubCategory != null)
			{
				viewModel = GetSubCategory;
			}
			viewModel.MRNDQC_CategoryVM = CategoryList;
			return View(viewModel);
		}
		
		[HttpPost]
		public IActionResult CreateSubCategory(MRNDQC_SubCategoryVM model)
		{
			ClaimsPrincipal currentUser = this.User;
			var ID = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
			
			bool isSuperAdmin = currentUser.IsInRole("SuperAdmin");
			bool isAdmin = currentUser.IsInRole("Admin");
			var DetailsSubmit = false;

			StatusModel status = new StatusModel();
			status.success = false;
			var employeeID = HttpContext.Session.GetString("EmployeeID");
			var UserID = HttpContext.Session.GetString("Id");
			Guid newGuid = Guid.Parse(UserID);
			model.LUser = newGuid;
			if (ModelState.IsValid)
			{
				DetailsSubmit = _setupService.AddSubCategory(model);
				if(DetailsSubmit)
                {
					return RedirectToAction("Index", "SubCategory");
				}
                else
                {
					ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "This Sub Category Already Exist");
				}
			}
			else
			{
				ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "Unknown error");
			}
			var CategoryList = _setupService.GetAllCategoryList();
			model.MRNDQC_CategoryVM = CategoryList;
			return View(model);
		}
	}
}
