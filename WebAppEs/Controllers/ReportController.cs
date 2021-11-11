using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAppEs.Models;
using WebAppEs.Services;
using WebAppEs.ViewModel.DailyMaxFaultAnalysis;
using WebAppEs.ViewModel.FaultsEntry;
using WebAppEs.ViewModel.Report;

namespace WebAppEs.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IDataAccessService _dataAccessService;
        private readonly ILogger<AdminController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ReportController(IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IDataAccessService dataAccessService, ILogger<AdminController> logger, IWebHostEnvironment hostingEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _roleManager = roleManager;
            _dataAccessService = dataAccessService;
            _logger = logger;
            _webHostEnvironment = hostingEnvironment;
        }
        [Authorize("Authorization")]
        public IActionResult Index()
        {
            var employeeID = HttpContext.Session.GetString("EmployeeID");
            if (employeeID == null)
            {
                return RedirectToAction("Logout", "Account");
            }

            ReportViewModel viewmodel = new ReportViewModel();
            //var data = _dataAccessService.GetAllFaultsList();
            viewmodel.PartsModelViewModel = _dataAccessService.GetAllPartsModelList();
            viewmodel.EmployeeListVM = _dataAccessService.GetAllEmployeeList();
            //viewmodel.WithQty = "qt";
            //viewmodel.WithPercentage = "per";
            return View(viewmodel);
        }
        public JsonResult LoadPreView(DateTime? FromDate, DateTime? ToDate, string LineNo, Guid PartsModelID, string LotNo, string EmployeeID)
        {
            MobileRNDFaultsEntryViewModel data = new MobileRNDFaultsEntryViewModel();
            var result = _dataAccessService.SortableAllFaultsList(FromDate, ToDate, LineNo, PartsModelID, LotNo, EmployeeID);

            return Json(result);
        }


        [HttpPost]
        public IActionResult Print([FromForm] ReportViewModel viewmodel)
        {
            //var employeeID = HttpContext.Session.GetString("EmployeeID");
            //if (employeeID == null)
            //{
            //    return RedirectToAction("Logout", "Account");
            //}
            //using (var workbook = new XLWorkbook())
            //{
            //    //String[] format = { "dd-MM-yy" };
            //    var fromDatestring = String.Format("{0:M-d-yyyy}", viewmodel.FromDate);
            //    var todatestring = String.Format("{0:M-d-yyyy}", viewmodel.ToDate);
            //    var fileName = "Faults (" + fromDatestring + " to " + todatestring + ")";
            //    var fileName2 = "Faults (" + fromDatestring + " to " + todatestring + ").xlsx";

            //    if (fromDatestring == "" && todatestring == "")
            //    {
            //        fileName = "FaultsList";
            //        fileName2 = "FaultsList.xlsx";
            //    }

            //    //workbook.ColumnWidth = 19;
            //    var result = _dataAccessService.SortableAllFaultsList(viewmodel.FromDate, viewmodel.ToDate, viewmodel.LineNo, viewmodel.PartsModelID, viewmodel.LotNo, viewmodel.EmployeeID);

            //    var worksheet = workbook.Worksheets.Add(fileName);

            //    worksheet.Column(1).Width = 15;
            //    worksheet.Column(2).Width = 15;
            //    worksheet.Column(3).Width = 15;
            //    worksheet.Column(4).Width = 15;
            //    worksheet.Column(5).Width = 15;
            //    worksheet.Column(6).Width = 22;
            //    worksheet.Column(7).Width = 22;
            //    worksheet.Column(8).Width = 22;
            //    worksheet.Column(9).Width = 22;
            //    worksheet.Column(10).Width = 22;
            //    worksheet.Column(11).Width = 22;
            //    worksheet.Column(12).Width = 22;
            //    worksheet.Column(13).Width = 18;

            //    // Background

            //    //worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 7)).Merge();


            //    // Set some values with different font sizes
            //    var currentRow = 1;
            //    worksheet.Cell(currentRow, 1).Value = "Process Development";
            //    var range = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 13));
            //    range.Merge().Style.Font.SetBold().Font.FontSize = 14;
            //    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            //    worksheet.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //    range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //    currentRow = 2;
            //    if (fromDatestring == "" && todatestring == "")
            //    {
            //        worksheet.Cell(currentRow, 1).Value = "Faults List";
            //    }
            //    else
            //    {
            //        worksheet.Cell(currentRow, 1).Value = "Faults List (" + fromDatestring + " to " + todatestring + ")";
            //    }

            //    var range2 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 13));
            //    range2.Merge().Style.Font.SetBold().Font.FontSize = 14;
            //    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            //    worksheet.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //    range2.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //    currentRow = 4;
            //    worksheet.Cell(currentRow, 1).Value = "Date";
            //    worksheet.Cell(currentRow, 1).Style.Font.FontSize = 11;
            //    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            //    worksheet.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //    worksheet.Cell(currentRow, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //    worksheet.Cell(currentRow, 2).Value = "Line";
            //    worksheet.Cell(currentRow, 2).Style.Font.FontSize = 11;
            //    worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
            //    worksheet.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //    worksheet.Cell(currentRow, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


            //    worksheet.Cell(currentRow, 3).Value = "Model";
            //    worksheet.Cell(currentRow, 3).Style.Font.FontSize = 11;
            //    worksheet.Cell(currentRow, 3).Style.Font.Bold = true;
            //    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //    worksheet.Cell(currentRow, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //    worksheet.Cell(currentRow, 4).Value = "Lot";
            //    worksheet.Cell(currentRow, 4).Style.Font.FontSize = 11;
            //    worksheet.Cell(currentRow, 4).Style.Font.Bold = true;
            //    worksheet.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //    worksheet.Cell(currentRow, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


            //    worksheet.Cell(currentRow, 5).Value = "Total Check";
            //    worksheet.Cell(currentRow, 5).Style.Font.FontSize = 11;
            //    worksheet.Cell(currentRow, 5).Style.Font.Bold = true;
            //    worksheet.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //    worksheet.Cell(currentRow, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //    if (viewmodel.WithQty) {
            //        worksheet.Cell(currentRow, 6).Value = "Func. Material Fault";
            //        worksheet.Cell(currentRow, 6).Style.Font.FontSize = 11;
            //        worksheet.Cell(currentRow, 6).Style.Font.Bold = true;
            //        worksheet.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        worksheet.Cell(currentRow, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


            //        worksheet.Cell(currentRow, 7).Value = "Func. Production Fault";
            //        worksheet.Cell(currentRow, 7).Style.Font.FontSize = 11;
            //        worksheet.Cell(currentRow, 7).Style.Font.Bold = true;
            //        worksheet.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        worksheet.Cell(currentRow, 7).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //        worksheet.Cell(currentRow, 8).Value = "Func. Software Fault";
            //        worksheet.Cell(currentRow, 8).Style.Font.FontSize = 11;
            //        worksheet.Cell(currentRow, 8).Style.Font.Bold = true;
            //        worksheet.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        worksheet.Cell(currentRow, 8).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //        worksheet.Cell(currentRow, 9).Value = "Total Func.";
            //        worksheet.Cell(currentRow, 9).Style.Font.FontSize = 11;
            //        worksheet.Cell(currentRow, 9).Style.Font.Bold = true;
            //        worksheet.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        worksheet.Cell(currentRow, 9).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //        worksheet.Cell(currentRow, 10).Value = "Aes. Matarial Fault";
            //        worksheet.Cell(currentRow, 10).Style.Font.FontSize = 11;
            //        worksheet.Cell(currentRow, 10).Style.Font.Bold = true;
            //        worksheet.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        worksheet.Cell(currentRow, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


            //        worksheet.Cell(currentRow, 11).Value = "Aes. Production Fault";
            //        worksheet.Cell(currentRow, 11).Style.Font.FontSize = 11;
            //        worksheet.Cell(currentRow, 11).Style.Font.Bold = true;
            //        worksheet.Cell(currentRow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        worksheet.Cell(currentRow, 11).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


            //        worksheet.Cell(currentRow, 12).Value = "Total Aes.";
            //        worksheet.Cell(currentRow, 12).Style.Font.FontSize = 11;
            //        worksheet.Cell(currentRow, 12).Style.Font.Bold = true;
            //        worksheet.Cell(currentRow, 12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        worksheet.Cell(currentRow, 12).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            //    }
            //    else
            //    {
            //        worksheet.Cell(currentRow, 6).Value = "Func. Material Fault (%)";
            //        worksheet.Cell(currentRow, 6).Style.Font.FontSize = 11;
            //        worksheet.Cell(currentRow, 6).Style.Font.Bold = true;
            //        worksheet.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        worksheet.Cell(currentRow, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


            //        worksheet.Cell(currentRow, 7).Value = "Func. Production Fault (%)";
            //        worksheet.Cell(currentRow, 7).Style.Font.FontSize = 11;
            //        worksheet.Cell(currentRow, 7).Style.Font.Bold = true;
            //        worksheet.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        worksheet.Cell(currentRow, 7).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //        worksheet.Cell(currentRow, 8).Value = "Func. Software Fault (%)";
            //        worksheet.Cell(currentRow, 8).Style.Font.FontSize = 11;
            //        worksheet.Cell(currentRow, 8).Style.Font.Bold = true;
            //        worksheet.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        worksheet.Cell(currentRow, 8).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //        worksheet.Cell(currentRow, 9).Value = "Total Func. (%)";
            //        worksheet.Cell(currentRow, 9).Style.Font.FontSize = 11;
            //        worksheet.Cell(currentRow, 9).Style.Font.Bold = true;
            //        worksheet.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        worksheet.Cell(currentRow, 9).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //        worksheet.Cell(currentRow, 10).Value = "Aes. Matarial Fault (%)";
            //        worksheet.Cell(currentRow, 10).Style.Font.FontSize = 11;
            //        worksheet.Cell(currentRow, 10).Style.Font.Bold = true;
            //        worksheet.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        worksheet.Cell(currentRow, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


            //        worksheet.Cell(currentRow, 11).Value = "Aes. Production Fault (%)";
            //        worksheet.Cell(currentRow, 11).Style.Font.FontSize = 11;
            //        worksheet.Cell(currentRow, 11).Style.Font.Bold = true;
            //        worksheet.Cell(currentRow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        worksheet.Cell(currentRow, 11).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


            //        worksheet.Cell(currentRow, 12).Value = "Total Aes. (%)";
            //        worksheet.Cell(currentRow, 12).Style.Font.FontSize = 11;
            //        worksheet.Cell(currentRow, 12).Style.Font.Bold = true;
            //        worksheet.Cell(currentRow, 12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        worksheet.Cell(currentRow, 12).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            //    }


            //    worksheet.Cell(currentRow, 13).Value = "Employee ID";
            //    worksheet.Cell(currentRow, 13).Style.Font.FontSize = 11;
            //    worksheet.Cell(currentRow, 13).Style.Font.Bold = true;
            //    worksheet.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //    worksheet.Cell(currentRow, 13).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


            //    foreach (var item in result)
            //    {
            //        currentRow++;
            //        worksheet.Cell(currentRow, 1).Value = item.Date.ToString();
            //        worksheet.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        worksheet.Cell(currentRow, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //        worksheet.Cell(currentRow, 2).Value = item.Line;
            //        worksheet.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        worksheet.Cell(currentRow, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //        worksheet.Cell(currentRow, 3).Value = item.ModelName;
            //        worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        worksheet.Cell(currentRow, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //        worksheet.Cell(currentRow, 4).Value = item.LotNo;
            //        worksheet.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        worksheet.Cell(currentRow, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


            //        worksheet.Cell(currentRow, 5).Value = item.TotalCheckedQty;
            //        worksheet.Cell(currentRow, 5).Style.Font.FontColor = XLColor.Black;
            //        worksheet.Cell(currentRow, 5).Style.Fill.PatternType = XLFillPatternValues.Solid;
            //        worksheet.Cell(currentRow, 5).Style.Fill.SetBackgroundColor(XLColor.Gainsboro);
            //        worksheet.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        worksheet.Cell(currentRow, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //        if (viewmodel.WithQty)
            //        {
            //            worksheet.Cell(currentRow, 6).Value = item.FuncMaterialFault;
            //            worksheet.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //            worksheet.Cell(currentRow, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //            worksheet.Cell(currentRow, 7).Value = item.FuncProductionFault;
            //            worksheet.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //            worksheet.Cell(currentRow, 7).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //            worksheet.Cell(currentRow, 8).Value = item.FuncSoftwareFault;
            //            worksheet.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //            worksheet.Cell(currentRow, 8).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //            worksheet.Cell(currentRow, 9).Value = item.TotalFunctionalFault;
            //            worksheet.Cell(currentRow, 9).Style.Font.FontColor = XLColor.Black;
            //            worksheet.Cell(currentRow, 9).Style.Fill.PatternType = XLFillPatternValues.Solid;
            //            worksheet.Cell(currentRow, 9).Style.Fill.SetBackgroundColor(XLColor.Gainsboro);
            //            worksheet.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //            worksheet.Cell(currentRow, 9).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //            worksheet.Cell(currentRow, 10).Value = item.AesthMaterialFault;
            //            worksheet.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //            worksheet.Cell(currentRow, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //            worksheet.Cell(currentRow, 11).Value = item.AesthProductionFault;
            //            worksheet.Cell(currentRow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //            worksheet.Cell(currentRow, 11).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //            worksheet.Cell(currentRow, 12).Value = item.TotalAestheticFault;
            //            worksheet.Cell(currentRow, 12).Style.Font.FontColor = XLColor.Black;
            //            worksheet.Cell(currentRow, 12).Style.Fill.PatternType = XLFillPatternValues.Solid;
            //            worksheet.Cell(currentRow, 12).Style.Fill.SetBackgroundColor(XLColor.Gainsboro);
            //            worksheet.Cell(currentRow, 12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //            worksheet.Cell(currentRow, 12).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            //        }
            //        else
            //        {
            //            worksheet.Cell(currentRow, 6).Value = Math.Round(((double)item.FuncMaterialFaultd), 2);
            //            worksheet.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //            worksheet.Cell(currentRow, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //            worksheet.Cell(currentRow, 7).Value = Math.Round(((double)item.FuncProductionFaultd), 2);
            //            worksheet.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //            worksheet.Cell(currentRow, 7).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //            worksheet.Cell(currentRow, 8).Value = Math.Round(((double)item.FuncSoftwareFaultd), 2);
            //            worksheet.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //            worksheet.Cell(currentRow, 8).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //            worksheet.Cell(currentRow, 9).Value = Math.Round(((double)item.TotalFunctionalFaultd), 2);
            //            worksheet.Cell(currentRow, 9).Style.Font.FontColor = XLColor.Black;
            //            worksheet.Cell(currentRow, 9).Style.Fill.PatternType = XLFillPatternValues.Solid;
            //            worksheet.Cell(currentRow, 9).Style.Fill.SetBackgroundColor(XLColor.Gainsboro);
            //            worksheet.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //            worksheet.Cell(currentRow, 9).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //            worksheet.Cell(currentRow, 10).Value = Math.Round(((double)item.AesthMaterialFaultd), 2);
            //            worksheet.Cell(currentRow, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //            worksheet.Cell(currentRow, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //            worksheet.Cell(currentRow, 11).Value = Math.Round(((double)item.AesthProductionFaultd), 2);
            //            worksheet.Cell(currentRow, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //            worksheet.Cell(currentRow, 11).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //            worksheet.Cell(currentRow, 12).Value = Math.Round(((double)item.TotalAestheticFaultd), 2);
            //            worksheet.Cell(currentRow, 12).Style.Font.FontColor = XLColor.Black;
            //            worksheet.Cell(currentRow, 12).Style.Fill.PatternType = XLFillPatternValues.Solid;
            //            worksheet.Cell(currentRow, 12).Style.Fill.SetBackgroundColor(XLColor.Gainsboro);
            //            worksheet.Cell(currentRow, 12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //            worksheet.Cell(currentRow, 12).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            //        }

            //        worksheet.Cell(currentRow, 13).Value = item.EmployeeID;
            //        worksheet.Cell(currentRow, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        worksheet.Cell(currentRow, 13).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            //    }

            //    using (var stream = new MemoryStream())
            //    {
            //        workbook.SaveAs(stream);
            //        var content = stream.ToArray();

            //        return File(
            //            content,
            //            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            //            fileName2);
            //    }
            //}
            return View();
        }

        public IActionResult PrintDetails(ReportViewModel viewmodel)
        {
            ReportViewModel data = new ReportViewModel();
            data.Date = DateTime.Today;
            return View(data);
        }

        [HttpPost]
        public IActionResult PrintListDetails([FromForm] ReportViewModel viewmodel)
        {
            var employeeID = HttpContext.Session.GetString("EmployeeID");
            if (employeeID == null)
            {
                return RedirectToAction("Logout", "Account");
            }
            using (var workbook = new XLWorkbook())
            {
                //String[] format = { "dd-MM-yy" };
                var DateString = String.Format("{0:M-d-yyyy}", viewmodel.Date);
                //var fileName = "Faults_Details (" + DateString + ")";
                var fileName2 = "Daily_Analysis (" + DateString  + ").xlsx";

                var InLine = _dataAccessService.Inline(viewmodel.Date);
                var Aging = _dataAccessService.Aging(viewmodel.Date);
                var OQC = _dataAccessService.OQC(viewmodel.Date);

                var worksheet = worksheetMethod(workbook.Worksheets.Add("In Line"), InLine, DateString, "In Line Production");

                var worksheet2 = worksheetMethod(workbook.Worksheets.Add("Aging"), Aging, DateString, "Aging");

                var worksheet3 = worksheetMethod(workbook.Worksheets.Add("OQC"), OQC, DateString, "OQC");

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName2);
                }
            }
        }
        public IXLWorksheet worksheetMethod(IXLWorksheet worksheet, List<MRNDHQC_TopFaultAnalysisVM> result, string DateString, string title)
        {
            worksheet.Column(1).Width = 15;
            worksheet.Column(2).Width = 15;
            worksheet.Column(3).Width = 23;
            worksheet.Column(4).Width = 25;
            worksheet.Column(5).Width = 40;
            worksheet.Column(6).Width = 40;
            worksheet.Column(7).Width = 15;
            worksheet.Column(8).Width = 40;
            worksheet.Column(9).Width = 60;

            worksheet.Column(1).Style.Alignment.WrapText = true;
            worksheet.Column(2).Style.Alignment.WrapText = true;
            worksheet.Column(3).Style.Alignment.WrapText = true;


            worksheet.Column(4).Style.Alignment.WrapText = true;
            worksheet.Column(5).Style.Alignment.WrapText = true;
            worksheet.Column(6).Style.Alignment.WrapText = true;
            worksheet.Column(7).Style.Alignment.WrapText = true;
            worksheet.Column(8).Style.Alignment.WrapText = true;
            worksheet.Column(9).Style.Alignment.WrapText = true;

            worksheet.Rows().AdjustToContents();
            // Background

            //worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 7)).Merge();


            // Set some values with different font sizes
            var currentRow = 1;
            var path2 = Path.Combine(_webHostEnvironment.WebRootPath, "Logo", "LogoPNG.png");
            worksheet.Cell(currentRow, 1).Value = "Cellular Phone Qc  Department";
            var image2 = worksheet.AddPicture(path2).MoveTo(worksheet.Cell(currentRow, 1), new Point(10, 6));
            image2.Height = 40;
            image2.Width = 100;
            var range = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 9));
            range.Merge().Style.Font.SetBold().Font.FontSize = 14;
            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Rockwell Condensed";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            currentRow = 2;

            worksheet.Cell(currentRow, 1).Value = "Date: " + DateString;
            var range2 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 9));
            range2.Merge().Style.Font.SetBold().Font.FontSize = 14;
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            range2.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            currentRow = 3;
            worksheet.Cell(currentRow, 1).Value = "Daily Maximum Fault Analysis Report ("+title+")";
            var range7 = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 9));
            range7.Merge().Style.Font.SetBold().Font.FontSize = 14;
            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Rockwell Condensed";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.SteelBlue;
            worksheet.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            range7.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            currentRow = 5;
            worksheet.Cell(currentRow, 1).Value = "Model";
            worksheet.Cell(currentRow, 1).Style.Font.FontSize = 12;
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(currentRow, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Cell(currentRow, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            worksheet.Cell(currentRow, 2).Value = "Line";
            worksheet.Cell(currentRow, 2).Style.Font.FontSize = 12;
            worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(currentRow, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Cell(currentRow, 2).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;


            //worksheet.Cell(currentRow, 3).Value = "Image of Fault";
            //worksheet.Cell(currentRow, 3).Style.Font.FontSize = 11;
            //worksheet.Cell(currentRow, 3).Style.Font.Bold = true;
            //worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //worksheet.Cell(currentRow, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell(currentRow, 3).Value = "Top Faulty Item";
            worksheet.Cell(currentRow, 3).Style.Font.FontSize = 12;
            worksheet.Cell(currentRow, 3).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(currentRow, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Cell(currentRow, 3).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;


            worksheet.Cell(currentRow, 4).Value = "Image";
            worksheet.Cell(currentRow, 4).Style.Font.FontSize = 12;
            worksheet.Cell(currentRow, 4).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(currentRow, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Cell(currentRow, 4).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            worksheet.Cell(currentRow, 5).Value = "Fault";
            worksheet.Cell(currentRow, 5).Style.Font.FontSize = 12;
            worksheet.Cell(currentRow, 5).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(currentRow, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Cell(currentRow, 5).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            worksheet.Cell(currentRow, 6).Value = "Reason";
            worksheet.Cell(currentRow, 6).Style.Font.FontSize = 12;
            worksheet.Cell(currentRow, 6).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(currentRow, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Cell(currentRow, 6).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            worksheet.Cell(currentRow, 7).Value = "Sample";
            worksheet.Cell(currentRow, 7).Style.Font.FontSize = 12;
            worksheet.Cell(currentRow, 7).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(currentRow, 7).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Cell(currentRow, 7).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            worksheet.Cell(currentRow, 8).Value = "Remarks";
            worksheet.Cell(currentRow, 8).Style.Font.FontSize = 12;
            worksheet.Cell(currentRow, 8).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(currentRow, 8).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Cell(currentRow, 8).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            worksheet.Cell(currentRow, 9).Value = "Probable Solution And Recommendation";
            worksheet.Cell(currentRow, 9).Style.Font.FontSize = 12;
            worksheet.Cell(currentRow, 9).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(currentRow, 9).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Cell(currentRow, 9).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            foreach (var group in result.GroupBy(item => new { item.Model, item.Line }))
            {
                currentRow++;
                var curr = currentRow;
                //worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 9)).Merge();

                worksheet.Cell(currentRow, 1).Value = group.Key.Model;
                worksheet.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(currentRow, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Cell(currentRow, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                //var ranges = worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, 9));
                //ranges.Merge().Style.Font.SetBold().Font.FontSize = 11;
                //ranges.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                //ranges.Style.Fill.SetBackgroundColor(XLColor.White);

                //worksheet.Range(worksheet.Cell(currentRow, 2), worksheet.Cell(currentRow, 9)).Merge();

                worksheet.Cell(currentRow, 2).Value = "Line " + group.Key.Line;
                worksheet.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(currentRow, 2).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Cell(currentRow, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                //var ranges22 = worksheet.Range(worksheet.Cell(currentRow, 3), worksheet.Cell(currentRow, 8));
                //ranges22.Merge().Style.Font.SetBold().Font.FontSize = 11;
                //ranges22.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                //ranges22.Style.Fill.SetBackgroundColor(XLColor.DarkGray);

                foreach (var group1 in group)
                {
                    worksheet.Cell(currentRow, 3).Value = group1.Category;
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Cell(currentRow, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(currentRow, 3).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    if (group1.ImageUrl == null)
                    {
                        worksheet.Cell(currentRow, 4).Value = "Picture wasn't detected";
                        worksheet.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Cell(currentRow, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(currentRow, 4).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    }
                    else
                    {
                        //var path2 = _webHostEnvironment.WebRootPath + $@"{ group1.ImageUrl}";
                        var path = Path.Combine(_webHostEnvironment.WebRootPath, "FaultImages", group1.ImageUrl);
                        if (System.IO.File.Exists(path))
                        {
                            //int iColumnWidth = (int)((worksheet.Column(4).Width - 1) * 7 + 12); // To convert column width in pixel unit.
                            var image = worksheet.AddPicture(path).MoveTo(worksheet.Cell(currentRow, 4), new Point(6, 6));
                            image.Height = 40;
                            image.Width = 165;
                            worksheet.Cell(currentRow, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Row(currentRow).Height = 40;
                        }
                        else
                        {
                            worksheet.Cell(currentRow, 4).Value = "Picture wasn't detected";
                            worksheet.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            worksheet.Cell(currentRow, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Cell(currentRow, 4).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        }
                    }

                    worksheet.Cell(currentRow, 5).Value = group1.SubCategory;
                    worksheet.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Cell(currentRow, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(currentRow, 5).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    worksheet.Cell(currentRow, 6).Value = group1.Reason;
                    worksheet.Cell(currentRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Cell(currentRow, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(currentRow, 6).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    worksheet.Cell(currentRow, 7).Value = group1.Sample;
                    worksheet.Cell(currentRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Cell(currentRow, 7).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(currentRow, 7).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    worksheet.Cell(currentRow, 8).Value = group1.Remarks;
                    worksheet.Cell(currentRow, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Cell(currentRow, 8).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(currentRow, 8).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    worksheet.Cell(currentRow, 9).Value = group1.ProblemSolAndRec;
                    worksheet.Cell(currentRow, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    worksheet.Cell(currentRow, 9).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(currentRow, 9).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    currentRow++;
                }
                currentRow -= 1;
                var ranges2 = worksheet.Range(worksheet.Cell(curr, 1), worksheet.Cell(currentRow, 1));
                ranges2.Merge().Style.Font.SetBold().Font.FontSize = 11;
                ranges2.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                var ranges5 = worksheet.Range(worksheet.Cell(curr, 2), worksheet.Cell(currentRow, 2));
                ranges5.Merge().Style.Font.SetBold().Font.FontSize = 11;
                ranges5.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            return worksheet;
        }
    }
}

