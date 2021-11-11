using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WebAppEs.Data;
using WebAppEs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAppEs.ViewModel.PartsModel;
using WebAppEs.Entity;
using WebAppEs.ViewModel.FaultsEntry;
using WebAppEs.ViewModel.Home;
using WebAppEs.ViewModel.Register;
using WebAppEs.ViewModel.Report;
using WebAppEs.ViewModel.DailyMaxFaultAnalysis;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace WebAppEs.Services
{
    public class DataAccessService : IDataAccessService
	{
		private readonly IMemoryCache _cache;
		private readonly ApplicationDbContext _context;
		private readonly IWebHostEnvironment _hostEnvironment;
		public DataAccessService(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, IMemoryCache cache)
		{
			_cache = cache;
			_context = context;
			_hostEnvironment = hostEnvironment;
		}

		public async Task<List<NavigationMenuViewModel>> GetMenuItemsAsync(ClaimsPrincipal principal)
		{
			var isAuthenticated = principal.Identity.IsAuthenticated;
			if (!isAuthenticated)
			{
				return new List<NavigationMenuViewModel>();
			}

			var roleIds = await GetUserRoleIds(principal);

			var permissions = await _cache.GetOrCreateAsync("Permissions",
				async x => await (from menu in _context.NavigationMenu select menu).ToListAsync());

			var rolePermissions = await _cache.GetOrCreateAsync("RolePermissions",
				async x => await (from menu in _context.RoleMenuPermission select menu).Include(x => x.NavigationMenu).ToListAsync());

			var data = (from menu in rolePermissions
						join p in permissions on menu.NavigationMenuId equals p.Id
						where roleIds.Contains(menu.RoleId)
						select p)
							  .Select(m => new NavigationMenuViewModel()
							  {
								  Id = m.Id,
								  Name = m.Name,
								  Area = m.Area,
								  Visible = m.Visible,
								  IsExternal = m.IsExternal,
								  ActionName = m.ActionName,
								  ExternalUrl = m.ExternalUrl,
								  DisplayOrder = m.DisplayOrder,
								  ParentMenuId = m.ParentMenuId,
								  ControllerName = m.ControllerName,
							  }).Distinct().OrderBy(x=> x.DisplayOrder).ToList();

			return data;
		}

		public async Task<bool> GetMenuItemsAsync(ClaimsPrincipal ctx, string ctrl, string act)
		{
			var result = false;
			var roleIds = await GetUserRoleIds(ctx);
			var data = await (from menu in _context.RoleMenuPermission
							  where roleIds.Contains(menu.RoleId)
							  select menu)
							  .Select(m => m.NavigationMenu)
							  .Distinct()
							  .ToListAsync();

			foreach (var item in data)
			{
				result = (item.ControllerName == ctrl && item.ActionName == act);
				if (result)
				{
					break;
				}
			}

			return result;
		}

		public async Task<List<NavigationMenuViewModel>> GetPermissionsByRoleIdAsync(string id)
		{
			var items = await (from m in _context.NavigationMenu
							   join rm in _context.RoleMenuPermission
								on new { X1 = m.Id, X2 = id } equals new { X1 = rm.NavigationMenuId, X2 = rm.RoleId }
								into rmp
							   from rm in rmp.DefaultIfEmpty()
							   select new NavigationMenuViewModel()
							   {
								   Id = m.Id,
								   Name = m.Name,
								   Area = m.Area,
								   ActionName = m.ActionName,
								   ControllerName = m.ControllerName,
								   IsExternal = m.IsExternal,
								   ExternalUrl = m.ExternalUrl,
								   DisplayOrder = m.DisplayOrder,
								   ParentMenuId = m.ParentMenuId,
								   Visible = m.Visible,
								   Permitted = rm.RoleId == id
							   })
							   .AsNoTracking()
							   .ToListAsync();
			return items;
		}

		public async Task<bool> SetPermissionsByRoleIdAsync(string id, IEnumerable<Guid> permissionIds)
		{
			var existing = await _context.RoleMenuPermission.Where(x => x.RoleId == id).ToListAsync();
			_context.RemoveRange(existing);

			foreach (var item in permissionIds)
			{
				await _context.RoleMenuPermission.AddAsync(new RoleMenuPermission()
				{
					RoleId = id,
					NavigationMenuId = item,
				});
			}

			var result = await _context.SaveChangesAsync();

			// Remove existing permissions to roles so it can re evaluate and take effect
			_cache.Remove("RolePermissions");

			return result > 0;
		}

		private async Task<List<string>> GetUserRoleIds(ClaimsPrincipal ctx)
		{
			var userId = GetUserId(ctx);
			var data = await (from role in _context.UserRoles
							  where role.UserId == userId
							  select role.RoleId).ToListAsync();
			return data;
		}

		private string GetUserId(ClaimsPrincipal user)
		{
			return ((ClaimsIdentity)user.Identity).FindFirst(ClaimTypes.NameIdentifier)?.Value;
		}

        public async Task<bool> AddPartsModel(PartsModelViewModel viewModel)
        {
			var result = 0;
			var UpdateDataSet = await _context.MobileRNDPartsModels.Where(x => x.Id == viewModel.ID).FirstOrDefaultAsync();
			var existing = await _context.MobileRNDPartsModels.Where(x => x.ModelName == viewModel.Name).FirstOrDefaultAsync();

			if(existing != null)
            {
				return false;
			}
            else
            {
				if(UpdateDataSet != null)
                {
					UpdateDataSet.ModelName = viewModel.Name;

					_context.MobileRNDPartsModels.Update(UpdateDataSet);
					result = await _context.SaveChangesAsync();
				}
                else
                {
					_context.MobileRNDPartsModels.Add(new MobileRNDPartsModels()
					{
						ModelName = viewModel.Name
					});
					result = await _context.SaveChangesAsync();
				}
			}
			return result > 0;
		}

        public  List<PartsModelViewModel> GetAllPartsModelList()
        {
			var items =  (from partModel in _context.MobileRNDPartsModels
							   
							   select new PartsModelViewModel()
							   {
								   ID = partModel.Id,
								   Name = partModel.ModelName
							   }).ToList();
			return items;
		}

        public  bool AddFaultsEntry(MobileRNDFaultsEntryViewModel viewModel)
        {
			if (viewModel == null)
            {
                return false;
            }
            else
            {
				_context.MobileRNDFaultsEntry.Add(new MobileRNDFaultsEntry()
				{
					Date = viewModel.Date,
					EmployeeID = viewModel.EmployeeID,
					LineNo = viewModel.LineNo,
					PartsModelID = viewModel.PartsModelID,
					LotNo = viewModel.LotNo,
					TotalIssueQty = (int)viewModel.TotalIssueQty,
					Shipment = viewModel.Shipment,
					Shift = viewModel.Shift,
					TypeOfProduction = viewModel.TypeOfProduction,
					QCPass = (int)viewModel.QCPass,
					LUser = viewModel.UserID
				});
            }
            var result =  _context.SaveChanges();
            return result>0;
		}

		public bool AddTopFaultHeadEntry(MRNDHQC_TopFaultHeadVM viewModel)
		{
			if (viewModel == null)
			{
				return false;
			}
			else
			{
				_context.MRNDHQC_TopFaultHead.Add(new MRNDHQC_TopFaultHead()
				{
					Date = viewModel.Date,
					EmployeeID = viewModel.EmployeeID,
					PartsModelID = viewModel.PartsModelID,
					LineNo = viewModel.LineNo,
					AnalysisType = viewModel.AnalysisType,
					LUser = viewModel.LUser
				});
			}
			var result = _context.SaveChanges();
			return result > 0;
		}

		public  bool UpdateFaultsEntry(MobileRNDFaultsEntryViewModel viewModel)
		{
			var datetimetoday = DateTime.Today;
			//var existing = null;
			//			var existing = await _context.MobileRNDFaultsEntry.Where(x => x.Date == DateTime.Today).FirstOrDefaultAsync();
			if (viewModel == null)
			{
				return false;
			}
			else
			{
				_context.MobileRNDFaultsEntry.Update(new MobileRNDFaultsEntry()
				{
					Id = (Guid)viewModel.Id,
					Date = viewModel.Date,
					EmployeeID = viewModel.EmployeeID,
					LineNo = viewModel.LineNo,
					PartsModelID = viewModel.PartsModelID,
					LotNo = viewModel.LotNo,
					Shipment = viewModel.Shipment,
					TypeOfProduction = viewModel.TypeOfProduction,
					QCPass = (int)viewModel.QCPass,
					UpdatedOn = DateTime.Today,
					LUser = viewModel.UserID,
					Shift = viewModel.Shift,
					TotalIssueQty = (int)viewModel.TotalIssueQty,
				});
			}
			var result =  _context.SaveChanges();
			return true;
		}

		public List<MobileRNDFaultsEntryViewModel> GetAllFaultsList(string EmployeeID)
        {
			if(EmployeeID == null)
            {
				EmployeeID = "";
            }
			var items = (from faults in _context.MobileRNDFaultsEntry
						 join model in _context.MobileRNDPartsModels
								on new { X1 = faults.PartsModelID } equals new { X1 = model.Id }
								into rmp
						 from rm in rmp.DefaultIfEmpty()
						 select new MobileRNDFaultsEntryViewModel()
						 {
							 Id = faults.Id,
							 Date = faults.Date,
							 EmployeeID = faults.EmployeeID,
							 //DateString =  faults.Date.ToString("MM/dd/yyyy"),
							 Line = "Line " + faults.LineNo,
							 ModelName = rm.ModelName,
							 ModelNameWithLot = rm.ModelName + "/" + faults.LotNo + " Order",
							 TotalIssueQty = faults.TotalIssueQty,
							 Shipment = faults.Shipment,
							 Shift = faults.Shift,
							 TypeOfProduction = faults.TypeOfProduction,
							 QCPass = faults.QCPass,
							 CreateDate = faults.CreatedOn,
							 LineNo = faults.LineNo,
							 StatusIsToday = faults.Date == DateTime.Today ? true : false
						 }).Distinct().OrderBy(d => d.Date).ToList();

			return items;
		}

		public List<MobileRNDFaultsEntryViewModel> SortableAllFaultsList(DateTime? startDate, DateTime? toDate, string lineNo, Guid ModelID, string lotNo, string EmployeeID)
		{
			if (lineNo == null)
			{
				lineNo = "";
			}
			if (ModelID == null)
			{
				ModelID = Guid.Empty;
			}
			if (lotNo == null)
			{
				lotNo = "";
			}
			if (EmployeeID == null)
			{
				EmployeeID = "";
			}

			var items = (from faults in _context.MobileRNDFaultsEntry
						  join model in _context.MobileRNDPartsModels
								 on new { X1 = faults.PartsModelID } equals new { X1 = model.Id }
								 into rmp
						  from rm in rmp.DefaultIfEmpty()
						  select new MobileRNDFaultsEntryViewModel()
						  {
							  Id = faults.Id,
							  Date = faults.Date,
							  EmployeeID = faults.EmployeeID,
							  //DateString = faults.Date == null ? null : String.Format("{0:MM/dd/yyyy}", faults.Date),
							  Line = "Line " + faults.LineNo,
							  ModelName = rm.ModelName,
							  ModelNameWithLot = rm.ModelName + "/" + faults.LotNo + " Order",
							  PartsModelID = faults.PartsModelID,
							  LotNo = faults.LotNo,
							  TotalIssueQty = faults.TotalIssueQty,

							  CreateDate = faults.CreatedOn,
							  LineNo = faults.LineNo,
							  StatusIsToday = faults.Date == DateTime.Today ? true : false
						  }).Distinct().OrderBy(d => d.Date).ThenByDescending(x => x.TotalIssueQty).Where(s => ((startDate == null && toDate == null) || (s.Date >= startDate && s.Date <= toDate)) && (lineNo == "" || s.LineNo == lineNo) && (ModelID == Guid.Empty || s.PartsModelID == ModelID) && (lotNo == "" || s.LotNo == lotNo) && (EmployeeID == "" || s.EmployeeID == EmployeeID)).ToList();
			return items;
		}

		public MobileRNDFaultsEntryViewModel GetFaults(Guid Id)
        {
			var items = (from faults in _context.MobileRNDFaultsEntry.Where(x => x.Id == Id) 
						 join model in _context.MobileRNDPartsModels
								on new { X1 = faults.PartsModelID } equals new { X1 = model.Id }
								into rmp
						 from rm in rmp.DefaultIfEmpty()
						 select new MobileRNDFaultsEntryViewModel()
						 {
							 Id = faults.Id,
							 EmployeeID = faults.EmployeeID,
							 Date = faults.Date,
							 DateString = String.Format("{0:MM/dd/yyyy}", faults.Date),
							 Line = "Line " + faults.LineNo,
							 LineNo = faults.LineNo,
							 LotNo = faults.LotNo,
							 Shipment = faults.Shipment,
							 Shift = faults.Shift,
							 TypeOfProduction = faults.TypeOfProduction,
							 QCPass = faults.QCPass,
							 ModelNameWithLot = rm.ModelName + "/" + faults.LotNo + " Order",
							 TotalIssueQty = faults.TotalIssueQty,
							 PartsModelID = faults.PartsModelID,
							 Disabled = "disabled",
							 ModelName = rm.ModelName,
						 }).FirstOrDefault();
			return items;
		}

        public bool AddFaultsDetails(MobileRNDFaultDetailsViewModel viewModel)
        {
			if (viewModel == null)
			{
				return false;
			}
			else
			{
				_context.MobileRNDFaultDetails.Add(new MobileRNDFaultDetails()
				{
					Date = viewModel.Date,
					EmployeeID = viewModel.EmployeeID,
					FaultEntryID = viewModel.FaultEntryId,
					CategoryID = viewModel.CategoryID,
					SubCategoryID = viewModel.SubCategoryID,
					FaultQty = viewModel.FaultQty,
					UpdatedOn = DateTime.Today,
					LUser = viewModel.UserID
				});
			}
			var result = _context.SaveChanges();

			return result>0;
		}

		public bool AddTopFaultsDetails(MRNDHQC_TopFaultAnalysisVM viewModel)
		{
			if (viewModel == null)
			{
				return false;
			}
			else
			{
				_context.MRNDHQC_TopFaultAnalysis.Add(new MRNDHQC_TopFaultAnalysis()
				{
					HeadID = viewModel.HeadID,
					CategoryID = viewModel.CategoryID,
					SubCategoryID = viewModel.SubCategoryID,
					Reason = viewModel.Reason,
					Sample = viewModel.Sample,
					Remarks = viewModel.Remarks,
					ProblemSolAndRec = viewModel.ProblemSolAndRec,
					ImageUrl = viewModel.ImageUrl,
					UpdatedOn = DateTime.Today,
					LUser = viewModel.LUser
				});
			}
			var result = _context.SaveChanges();

			return result > 0;
		}

		public MobileRNDFaultsEntryViewModel GetSortedFaults(DateTime? sortdate, string lineNo, Guid ModelID, string lotNo, string Shipment, string Shift, string TypeOfProduction)
        {
			var items = (from faults in _context.MobileRNDFaultsEntry.Where(x => x.Date == sortdate && x.LineNo == lineNo && x.PartsModelID == ModelID && x.LotNo == lotNo && x.Shipment == Shipment && x.Shift == Shift && x.TypeOfProduction == TypeOfProduction)
						 join model in _context.MobileRNDPartsModels
								on new { X1 = faults.PartsModelID } equals new { X1 = model.Id }
								into rmp
						 from rm in rmp.DefaultIfEmpty()
						 select new MobileRNDFaultsEntryViewModel()
						 {
							 Id = faults.Id,
							 EmployeeID = faults.EmployeeID,
							 Date = faults.Date,
							 DateString = String.Format("{0:MM/dd/yyyy}", faults.Date),
							 Line = "Line " + faults.LineNo,
							 LineNo = faults.LineNo,
							 LotNo = faults.LotNo,
							 ModelNameWithLot = rm.ModelName + "/" + faults.LotNo + " Order",
							 TotalIssueQty = faults.TotalIssueQty,
							 PartsModelID = faults.PartsModelID,
							 Disabled = "disabled",
							 QCPass = faults.QCPass
						 }).FirstOrDefault();
			return items;
		}

		public List<MobileRNDFaultDetails> GetSortedFaultsDetails(Guid Id)
        {
			var items = _context.MobileRNDFaultDetails.Where(x => x.FaultEntryID == Id).ToList();
			return items;
		}

        public  bool RemoveDetails(List<MobileRNDFaultDetails> Model)
        {
			_context.MobileRNDFaultDetails.RemoveRange(Model);
			var result =  _context.SaveChanges();

			if(result == 0)
            {
				return false;
			}
            else
            {
				return true;
			}
		}

        public List<MobileRNDFaultDetailsViewModel> GetFaultsDetails(Guid id)
        {
            var items = (from fadt in _context.MobileRNDFaultDetails.Where(x => x.FaultEntryID == id)
						 join cat in _context.MRNDQC_Category
								on new { X1 = fadt.CategoryID } equals new { X1 = cat.Id }
								into rmp
						 from catt in rmp.DefaultIfEmpty()
						 join subcat in _context.MRNDQC_SubCategory
								on new { X1 = fadt.SubCategoryID } equals new { X1 = subcat.Id }
								into subrmp
						 from subcatt in subrmp.DefaultIfEmpty()
						 select new MobileRNDFaultDetailsViewModel()
                         {
                             FaultEntryId = fadt.FaultEntryID,
                             EmployeeID = fadt.EmployeeID,
                             Date = fadt.Date,
							 CategoryID = fadt.CategoryID,
							 SubCategoryID = fadt.SubCategoryID,
							 CategoryName = catt.CategoryName,
							 SubCategoryName = subcatt.SubCategoryName,
							 FaultQty = fadt.FaultQty,
							 FaultType = subcatt.FaultType == "A" ? "Aesthetic" : "Functional",
							 FaultTypeKey = subcatt.FaultType
                         }).ToList();

			return items;
        }

        public PartsModelViewModel GetPartsModelList(Guid Id)
        {
			var items = (from partModel in _context.MobileRNDPartsModels.Where(x => x.Id == Id)

						 select new PartsModelViewModel()
						 {
							 ID = partModel.Id,
							 Name = partModel.ModelName,
							 IsUpdate = "Update"
						 }).FirstOrDefault();

			return items;
		}

        public DashboasrViewModel GetDashboardData(DateTime? YesterdayDate, DateTime LastSevenDayStart, DateTime LastMonthDayStart)
        {
			var today = DateTime.Today;

			DashboasrViewModel data = new DashboasrViewModel();
			


			return data;
		}

		public DashboasrViewModel GetSingelDayData(DateTime? Date)
		{

			if (Date == null)
			{
				Date = (DateTime)(from record in _context.MobileRNDFaultsEntry orderby record.Date select record.Date).Last();
			}

			DashboasrViewModel data = new DashboasrViewModel();
			ChartLevelViewModel lavel = new ChartLevelViewModel();
			FaultPercentageForChartWithFunAes percentage = new FaultPercentageForChartWithFunAes();

			lavel.LavelName = (from head in _context.MobileRNDFaultsEntry.Where(x => x.Date == Date).OrderBy(x => x.LineNo)
							   join model in _context.MobileRNDPartsModels
										  on new { X1 = head.PartsModelID } equals new { X1 = model.Id }
										  into rmp
							   from rm in rmp.DefaultIfEmpty()
							   select ("Line " + head.LineNo + ": " + rm.ModelName)).ToArray();

			percentage.Functional = (from head in _context.MobileRNDFaultsEntry.Where(x => x.Date == Date).OrderBy(x => x.LineNo)
                              select new
                              {
                                  EmployeeID = head.EmployeeID,
                                  totalFaultsCheck = ((_context.MobileRNDFaultDetails.Where(x => x.FaultEntryID == head.Id).Sum(x => x.FaultQty))+ head.QCPass),
                                  qcPass = head.QCPass,
                                  functionalTotalWithoutMinor = (from fadt in _context.MobileRNDFaultDetails.Where(x => x.FaultEntryID == head.Id)
                                                                 join cat in _context.MRNDQC_Category
                                                                        on new { X1 = fadt.CategoryID } equals new { X1 = cat.Id }
                                                                        into rmp
                                                                 from catt in rmp.DefaultIfEmpty()
                                                                 join subcat in _context.MRNDQC_SubCategory
                                                                        on new { X1 = fadt.SubCategoryID } equals new { X1 = subcat.Id }
                                                                        into subrmp
                                                                 from subcatt in subrmp.DefaultIfEmpty()
                                                                 select new MobileRNDFaultDetailsViewModel()
                                                                 {
                                                                     FaultQty = fadt.FaultQty,
                                                                     FaultType = subcatt.FaultType,
																	 CategoryName = catt.CategoryName
                                                                 }).Where(s => s.CategoryName != "Minor" && s.FaultType == "F").Sum(x => x.FaultQty)

                              }).Select(a => Math.Round((a.totalFaultsCheck == 0 ? 0 : ((double)a.functionalTotalWithoutMinor / (double)a.totalFaultsCheck) * 100), 2)).ToArray();


			percentage.Aesthetic = (from head in _context.MobileRNDFaultsEntry.Where(x => x.Date == Date).OrderBy(x => x.LineNo)
									 select new
									 {
										 EmployeeID = head.EmployeeID,
										 totalFaultsCheck = ((_context.MobileRNDFaultDetails.Where(x => x.FaultEntryID == head.Id).Sum(x => x.FaultQty)) + head.QCPass),
										 qcPass = head.QCPass,
										 AestheticTotalWithoutMinor = (from fadt in _context.MobileRNDFaultDetails.Where(x => x.FaultEntryID									== head.Id)
																		join cat in _context.MRNDQC_Category
																			   on new { X1 = fadt.CategoryID } equals new { X1 = cat.Id }
																			   into rmp
																		from catt in rmp.DefaultIfEmpty()
																		join subcat in _context.MRNDQC_SubCategory
																			   on new { X1 = fadt.SubCategoryID } equals new { X1 =														subcat.Id }
																			   into subrmp
																		from subcatt in subrmp.DefaultIfEmpty()
																		select new MobileRNDFaultDetailsViewModel()
																		{
																			FaultQty = fadt.FaultQty,
																			FaultType = subcatt.FaultType,
																			CategoryName = catt.CategoryName
																		}).Where(s => s.CategoryName != "Minor" && s.FaultType == "A").Sum(x										=> x.FaultQty)

									 }).Select(a => Math.Round((a.totalFaultsCheck == 0 ? 0 : ((double)a.AestheticTotalWithoutMinor / (double)a.totalFaultsCheck) * 100), 2)).ToArray();
			data.Lavel = lavel;
			data.FaultPercentageForChartWithFunAes = percentage;
			return data;
		}

		public double totalCheck(Guid a, int qcpass)
        {
			var totalFault = _context.MobileRNDFaultDetails.Where(x => x.Id == a).Sum(x=> x.FaultQty);
			double totalcheck = ((double)totalFault + (double)qcpass);
			return totalcheck;
        }

		public double totalFunc(Guid id)
		{
			var Functional = (from fadt in _context.MobileRNDFaultDetails.Where(x => x.FaultEntryID == id)
						 join cat in _context.MRNDQC_Category
								on new { X1 = fadt.CategoryID } equals new { X1 = cat.Id }
								into rmp
						 from catt in rmp.DefaultIfEmpty()
						 join subcat in _context.MRNDQC_SubCategory
								on new { X1 = fadt.SubCategoryID } equals new { X1 = subcat.Id }
								into subrmp
						 from subcatt in subrmp.DefaultIfEmpty()
						 select new MobileRNDFaultDetailsViewModel()
						 {
							 FaultQty = fadt.FaultQty,
							 FaultType = subcatt.FaultType
						 }).Where(s => s.FaultType == "F").Sum(x=> x.FaultQty);

			return Functional;
		}

		public double totalAes(Guid id)
		{
			var Functional = (from fadt in _context.MobileRNDFaultDetails.Where(x => x.FaultEntryID == id)
							  join cat in _context.MRNDQC_Category
									 on new { X1 = fadt.CategoryID } equals new { X1 = cat.Id }
									 into rmp
							  from catt in rmp.DefaultIfEmpty()
							  join subcat in _context.MRNDQC_SubCategory
									 on new { X1 = fadt.SubCategoryID } equals new { X1 = subcat.Id }
									 into subrmp
							  from subcatt in subrmp.DefaultIfEmpty()
							  select new MobileRNDFaultDetailsViewModel()
							  {
								  FaultQty = fadt.FaultQty,
								  FaultType = subcatt.FaultType
							  }).Where(s => s.FaultType == "A").Sum(x => x.FaultQty);

			return Functional;
		}

		public List<EmployeeListVM> GetAllEmployeeList()
        {
			var items = (from user in _context.Users

						 select new EmployeeListVM()
						 {
							 EmployeeID = user.EmployeeID,
							 EmployeeName = user.Name + " ("+ user.EmployeeID+")"
						 }).ToList();
			return items;
		}

        public List<MRNDHQC_TopFaultAnalysisVM> Inline(DateTime? Date)
        {
			var data = (from fadt in _context.MRNDHQC_TopFaultHead.Where(x => x.Date == Date && x.AnalysisType == "In Line").DefaultIfEmpty()
						 join entry in _context.MRNDHQC_TopFaultAnalysis on fadt.Id equals entry.HeadID 
						 join model in _context.MobileRNDPartsModels on fadt.PartsModelID equals model.Id
						join cat in _context.MRNDQC_Category on entry.CategoryID equals cat.Id
						join Subcat in _context.MRNDQC_SubCategory on entry.SubCategoryID equals Subcat.Id
						select new MRNDHQC_TopFaultAnalysisVM()
						 {
							 Line = fadt.LineNo,
							 Model = model.ModelName,
							 Category = cat.CategoryName,
							 SubCategory = Subcat.SubCategoryName,
							 Reason = entry.Reason,
							 Sample = entry.Sample,
							 Remarks = entry.Remarks,
							 ProblemSolAndRec = entry.ProblemSolAndRec,
							ImageUrl = entry.ImageUrl,
							DisplayUrl = Path.Combine(_hostEnvironment.WebRootPath, "FaultImages", entry.ImageUrl) ,
						 }).OrderBy(d => d.Line).ToList();
				return data;
		}

		public List<MRNDHQC_TopFaultAnalysisVM> Aging(DateTime? Date)
		{
			var data = (from fadt in _context.MRNDHQC_TopFaultHead.Where(x => x.Date == Date && x.AnalysisType == "Aging").DefaultIfEmpty()
						join entry in _context.MRNDHQC_TopFaultAnalysis on fadt.Id equals entry.HeadID
						join model in _context.MobileRNDPartsModels on fadt.PartsModelID equals model.Id
						join cat in _context.MRNDQC_Category on entry.CategoryID equals cat.Id
						join Subcat in _context.MRNDQC_SubCategory on entry.SubCategoryID equals Subcat.Id
						select new MRNDHQC_TopFaultAnalysisVM()
						{
							Line = fadt.LineNo,
							Model = model.ModelName,
							Category = cat.CategoryName,
							SubCategory = Subcat.SubCategoryName,
							Reason = entry.Reason,
							Sample = entry.Sample,
							Remarks = entry.Remarks,
							ProblemSolAndRec = entry.ProblemSolAndRec,
							ImageUrl = entry.ImageUrl,
							DisplayUrl = Path.Combine(_hostEnvironment.WebRootPath, "FaultImages", entry.ImageUrl),
						}).OrderBy(d => d.Line).ToList();
			return data;
		}

		public List<MRNDHQC_TopFaultAnalysisVM> OQC(DateTime? Date)
		{
			var data = (from fadt in _context.MRNDHQC_TopFaultHead.Where(x => x.Date == Date && x.AnalysisType == "OQC").DefaultIfEmpty()
						join entry in _context.MRNDHQC_TopFaultAnalysis on fadt.Id equals entry.HeadID
						join model in _context.MobileRNDPartsModels on fadt.PartsModelID equals model.Id
						join cat in _context.MRNDQC_Category on entry.CategoryID equals cat.Id
						join Subcat in _context.MRNDQC_SubCategory on entry.SubCategoryID equals Subcat.Id
						select new MRNDHQC_TopFaultAnalysisVM()
						{
							Line = fadt.LineNo,
							Model = model.ModelName,
							Category = cat.CategoryName,
							SubCategory = Subcat.SubCategoryName,
							Reason = entry.Reason,
							Sample = entry.Sample,
							Remarks = entry.Remarks,
							ProblemSolAndRec = entry.ProblemSolAndRec,
							ImageUrl = entry.ImageUrl,
							DisplayUrl = Path.Combine(_hostEnvironment.WebRootPath, "FaultImages", entry.ImageUrl),
						}).OrderBy(d => d.Line).ToList();
			return data;
		}

		public bool RemoveeNTRY(Guid Id)
        {
			var entry = _context.MobileRNDFaultsEntry.Where(x=> x.Id == Id).FirstOrDefault();
			_context.MobileRNDFaultsEntry.Remove(entry);
			var result =  _context.SaveChanges();
			if(result == 0)
            {
				return false;
            }
            else
            {
				return true;
			}
		}

        public DateTime LastDate()
        {
			return DateTime.Today ;
		}

        public List<AutoCompleteViewModel> FaultsTypeAutoComplete(string Prefix, string type)
        {
			List<AutoCompleteViewModel> result = new List<AutoCompleteViewModel>();
				return result;
		}
        public List<MobileRNDFaultDetailsViewModel> GetSortedFaultsDetails(DateTime? sortdate, Guid? ModelID, Guid? CategoryID, string FaultType, Guid? SubCategoryID)
        {
			var data = (from fadt in _context.MobileRNDFaultDetails.Where(x => x.Date == sortdate)
						join en in _context.MobileRNDFaultsEntry
								on new { X1 = fadt.FaultEntryID } equals new { X1 = en.Id }
								into rmp
						from entry in rmp.DefaultIfEmpty()
						join md in _context.MobileRNDPartsModels
								on new { X1 = entry.PartsModelID } equals new { X1 = md.Id }
								into mdrmp
						from model in mdrmp.DefaultIfEmpty()

						join ct in _context.MRNDQC_Category
								on new { X1 = fadt.CategoryID } equals new { X1 = ct.Id }
								into ctmdrmp
						from cat in ctmdrmp.DefaultIfEmpty()

						join sct in _context.MRNDQC_SubCategory
								on new { X1 = fadt.SubCategoryID } equals new { X1 = sct.Id }
								into sctmdrmp
						from subcat in sctmdrmp.DefaultIfEmpty()

						select new MobileRNDFaultDetailsViewModel()
						{
							Model = model.ModelName,
							CategoryName = cat.CategoryName,
							SubCategoryName = subcat.SubCategoryName,
							FaultType = subcat.FaultType,
							FaultQty = fadt.FaultQty,
							ModelID = entry.PartsModelID,
							CategoryID = fadt.CategoryID,
							SubCategoryID = fadt.SubCategoryID
						}).OrderByDescending(d => d.FaultQty).Where(p=> (ModelID == null || p.ModelID == ModelID) && (CategoryID == null || p.CategoryID == CategoryID) && (FaultType == null || p.FaultType == FaultType) && (SubCategoryID == null || p.SubCategoryID == SubCategoryID)).ToList();
			return data;
		}

		public List<MRNDHQC_TopFaultAnalysisVM> AllTopFaultModelWise(DateTime? sortdate, Guid ModelID, string AnalysisType, string LineNo)
		{
			var items = (from faults in _context.MRNDHQC_TopFaultHead.Where(x => x.Date == sortdate && x.PartsModelID == ModelID && x.AnalysisType == AnalysisType && x.LineNo == LineNo)
						 join dt in _context.MRNDHQC_TopFaultAnalysis
							on new { X1 = faults.Id } equals new { X1 = dt.HeadID }
							into dtrmp
						 from details in dtrmp.DefaultIfEmpty()
						 join model in _context.MobileRNDPartsModels
							on new { X1 = faults.PartsModelID } equals new { X1 = model.Id }
							into rmp
						 from rm in rmp.DefaultIfEmpty()
						 join cat in _context.MRNDQC_Category
							on new { X1 = details.CategoryID } equals new { X1 = cat.Id }
							into catrmp
						 from category in catrmp.DefaultIfEmpty()
						 join cat in _context.MRNDQC_SubCategory
							on new { X1 = details.SubCategoryID } equals new { X1 = cat.Id }
							into subcatrmp
						 from subcategory in subcatrmp.DefaultIfEmpty()

						 select new MRNDHQC_TopFaultAnalysisVM()
						 {
							 Id = details.Id,
							 AnalysisType = faults.AnalysisType,
							 Model = rm.ModelName,
							 PartsModelID = faults.PartsModelID,
							 Category = category.CategoryName,
							 CategoryID = details.CategoryID,
							 SubCategory = subcategory.SubCategoryName,
							 SubCategoryID = details.SubCategoryID,
							 ImageUrl = details.ImageUrl,
							 DisplayUrl = Path.Combine(_hostEnvironment.WebRootPath, "FaultImages", details.ImageUrl),
							 Quantity = 0,
							 Reason = details.Reason,
							 Sample = details.Sample,
							 Remarks = details.Remarks,
							 ProblemSolAndRec = details.ProblemSolAndRec
						 }).ToList();
			return items;
		}

		public MRNDHQC_TopFaultHeadVM GetSortedTopFaultsHead(DateTime? sortdate, Guid ModelID, string AnalysisType)
		{
			var items = (from faults in _context.MRNDHQC_TopFaultHead.Where(x => x.Date == sortdate && x.PartsModelID == ModelID && x.AnalysisType == AnalysisType)
						 join model in _context.MobileRNDPartsModels
							on new { X1 = faults.PartsModelID } equals new { X1 = model.Id }
							into rmp
						 from rm in rmp.DefaultIfEmpty()
						 select new MRNDHQC_TopFaultHeadVM()
						 {
							 Id = faults.Id,
							 EmployeeID = faults.EmployeeID,
							 Date = faults.Date,
							 DateString = String.Format("{0:MM/dd/yyyy}", faults.Date),
							 Model = rm.ModelName,
							 PartsModelID = faults.PartsModelID,
							 //Disabled = "disabled"
						 }).FirstOrDefault();
			return items;
		}

		public List<MRNDHQC_TopFaultHeadVM> HeadList()
		{
			var items = (from faults in _context.MRNDHQC_TopFaultHead
						 join model in _context.MobileRNDPartsModels
							on new { X1 = faults.PartsModelID } equals new { X1 = model.Id }
							into rmp
						 from rm in rmp.DefaultIfEmpty()
						 select new MRNDHQC_TopFaultHeadVM()
						 {
							 Id = faults.Id,
							 EmployeeID = faults.EmployeeID,
							 Date = faults.Date,
							 DateString = String.Format("{0:MM/dd/yyyy}", faults.Date),
							 Model = rm.ModelName,
							 PartsModelID = faults.PartsModelID,
							 StatusIsToday = faults.Date == DateTime.Today ? true : false,
							 AnalysisType = faults.AnalysisType,
							 ModelWithLine = rm.ModelName + " / Line " + faults.LineNo
							 //Disabled = "disabled"
						 }).ToList();
			return items;
		}

		public List<MRNDHQC_TopFaultAnalysis> AllDataByHedID(Guid Id)
		{
			var items = _context.MRNDHQC_TopFaultAnalysis.Where(x => x.HeadID == Id).ToList();
			return items;
		}


		public MRNDHQC_TopFaultHeadVM LoadDataHead(Guid Id)
		{
			var items = (from faults in _context.MRNDHQC_TopFaultHead.Where(x => x.Id == Id)
						 join model in _context.MobileRNDPartsModels
							on new { X1 = faults.PartsModelID } equals new { X1 = model.Id }
							into rmp
						 from rm in rmp.DefaultIfEmpty()
						 select new MRNDHQC_TopFaultHeadVM()
						 {
							 Id = faults.Id,
							 EmployeeID = faults.EmployeeID,
							 Date = faults.Date,
							 DateString = String.Format("{0:MM/dd/yyyy}", faults.Date),
							 Model = rm.ModelName,
							 PartsModelID = faults.PartsModelID,
							 AnalysisType = faults.AnalysisType,
							 LineNo = faults.LineNo,
						 }).FirstOrDefault();
			return items;
		}

		public bool RemoveTopDetailsByModelWise(List<MRNDHQC_TopFaultAnalysis> Model)
		{
			_context.MRNDHQC_TopFaultAnalysis.RemoveRange(Model);
			var result = _context.SaveChanges();

			if (result == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public List<MRNDHQC_TopFaultAnalysisVM> LoadTopFaultDetailsData(Guid headID)
		{
			var items = (from faults in _context.MRNDHQC_TopFaultHead.Where(x => x.Id == headID)
						 join dt in _context.MRNDHQC_TopFaultAnalysis
							on new { X1 = faults.Id } equals new { X1 = dt.HeadID }
							into dtrmp
						 from details in dtrmp.DefaultIfEmpty()
						 join model in _context.MobileRNDPartsModels
							on new { X1 = faults.PartsModelID } equals new { X1 = model.Id }
							into rmp
						 from rm in rmp.DefaultIfEmpty()
						 join cat in _context.MRNDQC_Category
							on new { X1 = details.CategoryID } equals new { X1 = cat.Id }
							into catrmp
						 from category in catrmp.DefaultIfEmpty()
						 join cat in _context.MRNDQC_SubCategory
							on new { X1 = details.SubCategoryID } equals new { X1 = cat.Id }
							into subcatrmp
						 from subcategory in subcatrmp.DefaultIfEmpty()

						 select new MRNDHQC_TopFaultAnalysisVM()
						 {
							 Id = details.Id,
							 AnalysisType = faults.AnalysisType,
							 Model = rm.ModelName,
							 PartsModelID = faults.PartsModelID,
							 Category = category.CategoryName,
							 CategoryID = details.CategoryID,
							 SubCategory = subcategory.SubCategoryName,
							 SubCategoryID = details.SubCategoryID,
							 ImageUrl = details.ImageUrl,
							 DisplayUrl = "/FaultImages/"+ details.ImageUrl,
							 Quantity = 0,
							 Reason = details.Reason,
							 Sample = details.Sample,
							 Remarks = details.Remarks,
							 ProblemSolAndRec = details.ProblemSolAndRec
						 }).ToList();
			return items;
		}
	}
}