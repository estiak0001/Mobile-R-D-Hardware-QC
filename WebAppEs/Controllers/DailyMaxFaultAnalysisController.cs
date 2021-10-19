
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppEs.Models;
using WebAppEs.Services;
using WebAppEs.ViewModel.Category;
using WebAppEs.ViewModel.FaultsEntry;

namespace WebAppEs.Controllers
{
    public class DailyMaxFaultAnalysisController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IDataAccessService _dataAccessService;
        private readonly ILogger<AdminController> _logger;
        private readonly ISetupService _setupService;

        public DailyMaxFaultAnalysisController(
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                IDataAccessService dataAccessService,
                ISetupService setupService,
        ILogger<AdminController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dataAccessService = dataAccessService;
            _logger = logger;
            _setupService = setupService;
        }
        public IActionResult Index()
        {
            return View();
        }



        //[Authorize("Authorization")]
        public IActionResult CreateDailyMaxFaultsInfo(Guid Id)
        {
            //var employeeID = HttpContext.Session.GetString("EmployeeID");
            //if (employeeID == null)
            //{
            //    return RedirectToAction("Logout", "Account");
            //}

            MobileRNDFaultsEntryViewModel viewmodel = new MobileRNDFaultsEntryViewModel();
            //List<MRNDQC_SubCategoryVM> sub = new List<MRNDQC_SubCategoryVM>();
            //if (Id == Guid.Empty)
            //{
            //    viewmodel.Date = DateTime.Today;
            //    var category = _setupService.GetAllCategoryList();
            //    var subCategory = _setupService.GetSubCatList();
            //    viewmodel.MRNDQC_CategoryVM = category;
            //    viewmodel.MRNDQC_SubCategoryVM = sub;
            //    viewmodel.PartsModelViewModel = _dataAccessService.GetAllPartsModelList();
            //    viewmodel.ButtonText = "Submit";
            //    return View(viewmodel);
            //}
            //else
            //{
            //    viewmodel = _dataAccessService.GetFaults(Id);
            //    var category = _setupService.GetAllCategoryList();
            //    var subCategory = _setupService.GetSubCatList();
            //    viewmodel.MRNDQC_CategoryVM = category;
            //    viewmodel.MRNDQC_SubCategoryVM = sub;
            //    viewmodel.PartsModelViewModel = _dataAccessService.GetAllPartsModelList();
            //    viewmodel.MobileRNDFaultDetailsViewModel = _dataAccessService.GetFaultsDetails(Id);
            //    viewmodel.ButtonText = "Update";
            //  return View(viewmodel);
            //}
            List<MRNDQC_SubCategoryVM> sub = new List<MRNDQC_SubCategoryVM>();
            //viewmodel.Date = DateTime.Today;
            var category = _setupService.GetAllCategoryList();
            var subCategory = _setupService.GetSubCatList();
            viewmodel.MRNDQC_CategoryVM = category;
            viewmodel.MRNDQC_SubCategoryVM = sub;
            viewmodel.PartsModelViewModel = _dataAccessService.GetAllPartsModelList();
            viewmodel.ButtonText = "Submit";
            return View(viewmodel);
        }

        [HttpPost]
        public JsonResult LoadFaultsData([FromBody] MobileRNDFaultsEntryViewModel model)
        {
            var head = _dataAccessService.GetSortedFaults(model.Date, model.LineNo, model.PartsModelID, model.LotNo, model.Shipment, model.Shift, model.TypeOfProduction);
            var details = _dataAccessService.GetSortedFaultsDetails(head.Id);

            return Json(details);
        }
    }
}
