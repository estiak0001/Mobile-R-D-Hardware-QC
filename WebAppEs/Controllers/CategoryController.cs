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
    public class CategoryController : Controller
    {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ISetupService _setupService;
		private readonly ILogger<AdminController> _logger;

		public CategoryController(
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

			var DataModel = _setupService.GetAllCategoryList();
			return View(DataModel);
        }

		public IActionResult CreateCategory(Guid Id)
		{
			var employeeID = HttpContext.Session.GetString("EmployeeID");
			if (employeeID == null)
			{
				return RedirectToAction("Logout", "Account");
			}

			var ModelData = _setupService.GetCategory(Id);
			var SubCategoryList = _setupService.GetSubCategory(Id);
			MRNDQC_CategoryVM viewModel = new MRNDQC_CategoryVM();
			if (ModelData != null)
			{
				viewModel = ModelData;
				viewModel.MRNDQC_SubCategoryVM = SubCategoryList;
			}
			return View(viewModel);
		}

		[HttpPost]
		//[Authorize("Roles")]
		public IActionResult CreateCategory(MRNDQC_CategoryVM viewModel)
		{
			var employeeID = HttpContext.Session.GetString("EmployeeID");
			var UserID = HttpContext.Session.GetString("Id");
			if (employeeID == null)
			{
				return RedirectToAction("Logout", "Account");
			}

			ClaimsPrincipal currentUser = this.User;
			var ID = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
			Guid newGuid = Guid.Parse(ID);
			bool isSuperAdmin = currentUser.IsInRole("SuperAdmin");
			bool isAdmin = currentUser.IsInRole("Admin");

			viewModel.LUser = newGuid;
			

			if (ModelState.IsValid)
			{
				var IsSubmit =  _setupService.AddCategory(viewModel);
				if (IsSubmit.CategoryName == null)
				{
					ViewBag.Alert = CommonServices.ShowAlert(Alerts.Warning, "This Category Already Exist!");
					return View(viewModel);
				}
				else
				{
					var SubCategoryList = _setupService.GetSubCategory(IsSubmit.Id);
					MRNDQC_CategoryVM data = new MRNDQC_CategoryVM();
					data.CategoryName = IsSubmit.CategoryName;
					data.Id = IsSubmit.Id;
					data.LUser = newGuid;
					data.MRNDQC_SubCategoryVM = SubCategoryList;
					if (viewModel.Id != Guid.Empty)
                    {
						var SubCategoryList2 = _setupService.GetSubCategory(viewModel.Id);
						data.Id = viewModel.Id;
						data.IsUpdate = "Update Again";
						data.MRNDQC_SubCategoryVM = SubCategoryList2;
					}
					ViewBag.Alert = CommonServices.ShowAlert(Alerts.Success, "Data added");

				return View(data);
				}
			}
			ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "Unknown error");
			return View(viewModel);
		}

		[HttpPost]
		public JsonResult SubCategoryAdd([FromBody] AddSubCategoryVM model)
		{
			ClaimsPrincipal currentUser = this.User;
			var ID = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
			Guid newGuid = Guid.Parse(ID);
			bool isSuperAdmin = currentUser.IsInRole("SuperAdmin");
			bool isAdmin = currentUser.IsInRole("Admin");
			var DetailsSubmit = false;
			//bool status = false;
			StatusModel status = new StatusModel();
			status.success = false;
			var employeeID = HttpContext.Session.GetString("EmployeeID");
			model.LUser = newGuid;


			//var details = _setupService.GetSubCatByCatID(model.Id);
			//if (details != null)
			//{
			//	var isDelete = _setupService.RemoveDetails(details);
			//}

			foreach (var it in model.MRNDQC_SubCategoryVM)
			{
				if(it.CategoryID == null)
                {
					it.CategoryID = model.Id;
					it.LUser = model.LUser;
					DetailsSubmit = _setupService.AddSubCategory(it);
				}
			}
			

			if (DetailsSubmit)
			{
				status.success = true;
			}

			return Json(status);
		}
	}
}
