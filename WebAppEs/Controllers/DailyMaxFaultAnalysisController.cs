
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
using WebAppEs.ViewModel.DailyMaxFaultAnalysis;
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
            MobileRNDFaultsEntryViewModel viewmodel = new MobileRNDFaultsEntryViewModel();
            List<MRNDQC_SubCategoryVM> sub = new List<MRNDQC_SubCategoryVM>();
            var category = _setupService.GetAllCategoryList();
            var subCategory = _setupService.GetSubCatList();
            viewmodel.MRNDQC_CategoryVM = category;
            viewmodel.MRNDQC_SubCategoryVM = sub;
            viewmodel.PartsModelViewModel = _dataAccessService.GetAllPartsModelList();
            viewmodel.ButtonText = "Submit";
            viewmodel.Date = DateTime.Today;
            return View(viewmodel);
        }

        //[HttpPost]
        public JsonResult LoadFaultsData(ParameterToLoadDataViewModel model)
        {
            var details = _dataAccessService.GetSortedFaultsDetails(model.Date, model.ModelID, model.CategoryID, model.FaultType, model.SubCategoryID);
            return Json(details);
        }
    }
}
