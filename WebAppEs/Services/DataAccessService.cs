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

namespace WebAppEs.Services
{
    public class DataAccessService : IDataAccessService
	{
		private readonly IMemoryCache _cache;
		private readonly ApplicationDbContext _context;

		public DataAccessService(ApplicationDbContext context, IMemoryCache cache)
		{
			_cache = cache;
			_context = context;
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
					TotalCheckedQty = viewModel.TotalCheckedQty,
					Shipment = viewModel.Shipment,
					Shift = viewModel.Shift,
					TypeOfProduction = viewModel.TypeOfProduction,
					QCPass = viewModel.QCPass,
					LUser = viewModel.UserID
				}) ;
            }
            var result =  _context.SaveChanges();
            return result>0;
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
					TotalCheckedQty = viewModel.TotalCheckedQty,
					
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
							 TotalCheckedQty = faults.TotalCheckedQty,
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
							  TotalCheckedQty = faults.TotalCheckedQty,

							  CreateDate = faults.CreatedOn,
							  LineNo = faults.LineNo,
							  StatusIsToday = faults.Date == DateTime.Today ? true : false
						  }).Distinct().OrderBy(d => d.Date).ThenByDescending(x => x.TotalCheckedQty).Where(s => ((startDate == null && toDate == null) || (s.Date >= startDate && s.Date <= toDate)) && (lineNo == "" || s.LineNo == lineNo) && (ModelID == Guid.Empty || s.PartsModelID == ModelID) && (lotNo == "" || s.LotNo == lotNo) && (EmployeeID == "" || s.EmployeeID == EmployeeID)).ToList();
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
							 TotalCheckedQty = faults.TotalCheckedQty,
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
							 TotalCheckedQty = faults.TotalCheckedQty,
							 PartsModelID = faults.PartsModelID,
							 Disabled = "disabled"
						 }).FirstOrDefault();
			return items;
		}

        //public MobileRNDFaultsEntryViewModel GetSortedFaults(DateTime sortdate, int lineNo, string ModelID, string lotNo)
        //{
        //    throw new NotImplementedException();
        //}

        public List<MobileRNDFaultDetails> GetSortedFaultsDetails(Guid Id)
        {
			//var items = (from fadt in _context.MobileRNDFaultDetails.Where(x => x.FaultEntryID == Id)
						 
			//			 select new MobileRNDFaultDetails()
			//			 {
			//				 FaultEntryID = fadt.FaultEntryID,
			//				 EmployeeID = fadt.EmployeeID,
			//				 Date = fadt.Date,
							 
			//			 }).ToList();
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
							 FaultType = subcatt.FaultType == "A" ? "Aesthetic" : "Functional"
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
			//if(Date == null)
   //         {
			//	Date = (DateTime)(from record in _context.MobileRNDFaultsEntry orderby record.Date select record.Date).Last();
			//}

			DashboasrViewModel data = new DashboasrViewModel();
			ChartLevelViewModel lavel = new ChartLevelViewModel();
			FunctionalFaultsPercentageViewModel FuncPercentage = new FunctionalFaultsPercentageViewModel();

			AestheticFaultsPercentageViewModel AesPercentage = new AestheticFaultsPercentageViewModel();

			

			

			return data;
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

        public List<DetailsReportViewModel> DetailsReportList(DateTime? Date)
        {
			var data = (from fadt in _context.MobileRNDFaultDetails.Where(x => x.Date == Date).DefaultIfEmpty()
						 join entry in _context.MobileRNDFaultsEntry on fadt.FaultEntryID equals entry.Id 
						 join model in _context.MobileRNDPartsModels on entry.PartsModelID equals model.Id
						 select new DetailsReportViewModel()
						 {
							 LineNo = entry.LineNo,
							 LineWithModel = "Assembly Line "+entry.LineNo+" Model: "+model.ModelName +"/"+entry.LotNo,

						 }).OrderBy(d => d.LineNo).ToList();
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
    }
}