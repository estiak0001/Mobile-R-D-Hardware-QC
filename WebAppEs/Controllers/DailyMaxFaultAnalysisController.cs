
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
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
        private readonly IWebHostEnvironment _hostEnvironment;

        public DailyMaxFaultAnalysisController(
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                IDataAccessService dataAccessService,
                IWebHostEnvironment hostEnvironment,
                ISetupService setupService,
        ILogger<AdminController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dataAccessService = dataAccessService;
            _logger = logger;
            _setupService = setupService;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            MRNDHQC_TopFaultHeadVM head = new MRNDHQC_TopFaultHeadVM();
            var headList = _dataAccessService.HeadList();
            head.MRNDHQC_TopFaultHeadList = headList;
            return View(head);
        }



        //[Authorize("Authorization")]
        public IActionResult CreateDailyMaxFaultsInfo(Guid Id)
        {
            MRNDHQC_TopFaultHeadVM viewmodel = new MRNDHQC_TopFaultHeadVM();
            List<MRNDQC_SubCategoryVM> sub = new List<MRNDQC_SubCategoryVM>();
            var category = _setupService.GetAllCategoryList();
            var subCategory = _setupService.GetSubCatList();
            if (Id == Guid.Empty)
            {
                viewmodel.MRNDQC_CategoryVM = category;
                viewmodel.MRNDQC_SubCategoryVM = sub;
                viewmodel.PartsModelViewModel = _dataAccessService.GetAllPartsModelList();
                viewmodel.Date = DateTime.Today;
                viewmodel.wwwrootpath = _hostEnvironment.WebRootPath;
                return View(viewmodel);
            }
            else
            {
                var head = _dataAccessService.LoadDataHead(Id);
                var DetailsData = _dataAccessService.LoadTopFaultDetailsData(Id);
                viewmodel = head;
                viewmodel.MRNDQC_CategoryVM = category;
                viewmodel.MRNDQC_SubCategoryVM = sub;
                viewmodel.PartsModelViewModel = _dataAccessService.GetAllPartsModelList();
                viewmodel.IsUpdate = "Update";
                viewmodel.Date = head.Date;
                viewmodel.MRNDHQC_TopFaultAnalysisList = DetailsData;
                viewmodel.wwwrootpath = _hostEnvironment.WebRootPath;
                return View(viewmodel);
            }
        }

        //[HttpPost]
        public JsonResult LoadFaultsData(ParameterToLoadDataViewModel model)
        {
            var details = _dataAccessService.GetSortedFaultsDetails(model.Date, model.ModelID, model.CategoryID, model.FaultType, model.SubCategoryID);
            return Json(details);
        }

        public class ImageParam
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }

        public ImageParam Upload_File()
        {
            ImageParam im = new ImageParam();
            im.Name = string.Empty;
            try
            {
                long size = 0;
                var file = Request.Form.Files;
                var filename = ContentDispositionHeaderValue
                                .Parse(file[0].ContentDisposition)
                                .FileName
                                .Trim('"');
                string FilePathee = _hostEnvironment.WebRootPath;

                string FilePath = _hostEnvironment.WebRootPath + $@"\FaultImages\{ filename}";

                size += file[0].Length;

                using (FileStream fs = System.IO.File.Create(FilePath))
                {
                    file[0].CopyTo(fs);
                    fs.Flush();
                }
                im.Url = "/FaultImages/"+ filename;
                im.Name =  filename;
            }
            catch (Exception ex)
            {
                im.Url = ex.Message;
            }
            return im;
        }
        public JsonResult FileRemove(string ImgPath)
        {
            string Path = _hostEnvironment.WebRootPath + $@"{ ImgPath}";
            FileInfo file = new FileInfo(Path);
            if (file.Exists)
            {
                file.Delete();
            }
            return Json(true);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        //[AllowAnonymous]
        public JsonResult TopFaultsDetailsAdd([FromBody] MRNDHQC_TopFaultHeadVM model)
        {
            ClaimsPrincipal currentUser = this.User;
            var ID = HttpContext.Session.GetString("Id");
            Guid newGuid = Guid.Parse(ID);
            bool isSuperAdmin = currentUser.IsInRole("SuperAdmin");
            bool isAdmin = currentUser.IsInRole("Admin");

            var HedSubmit = false;
            var DetailsSubmit = false;
            //bool status = false;
            StatusModel status = new StatusModel();
            status.success = false;
            var employeeID = HttpContext.Session.GetString("EmployeeID");
            model.LUser = newGuid;
            if (ModelState.IsValid)
            {
                var head = _dataAccessService.GetSortedTopFaultsHead(model.Date, model.PartsModelID, model.AnalysisType);
                if (head == null)
                {
                    model.EmployeeID = employeeID;
                    HedSubmit = _dataAccessService.AddTopFaultHeadEntry(model);
                    if (HedSubmit)
                    {
                        var Savedhead = _dataAccessService.GetSortedTopFaultsHead(model.Date, model.PartsModelID, model.AnalysisType);

                        foreach (var it in model.MRNDHQC_TopFaultAnalysisList)
                        {
                            it.HeadID = Savedhead.Id;
                            it.LUser = newGuid;
                            var IsSubmit = _dataAccessService.AddTopFaultsDetails(it);
                        }
                    }
                }
                else
                {
                    var details = _dataAccessService.AllDataByHedID(head.Id);
                    if (details != null)
                    {
                        var isDelete = _dataAccessService.RemoveTopDetailsByModelWise(details);
                    }

                    foreach (var it in model.MRNDHQC_TopFaultAnalysisList)
                    {
                        it.HeadID = head.Id;
                        it.LUser = newGuid;

                        DetailsSubmit = _dataAccessService.AddTopFaultsDetails(it);
                    }
                }
            }
            else
            {
                status.success = false;
            }

            if (DetailsSubmit)
            {
                status.success = true;
            }

            return Json(status);
        }

        public JsonResult LoadSortedTopResult(DateTime Date,  Guid ModelID, string AnalysisType, string LineNo)
        {
            List<MRNDHQC_TopFaultAnalysisVM> data = new List<MRNDHQC_TopFaultAnalysisVM>();
            var listdata = _dataAccessService.AllTopFaultModelWise(Date, ModelID, AnalysisType, LineNo);
            if(listdata.Count != 0)
            {
                data = listdata;
            }
            return Json(data);
        }
    }
}
