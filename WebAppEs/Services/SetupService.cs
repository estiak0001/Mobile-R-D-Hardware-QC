using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppEs.Data;
using WebAppEs.Entity;
using WebAppEs.ViewModel.Category;

namespace WebAppEs.Services
{
    public class SetupService : ISetupService
    {
        private readonly ApplicationDbContext _context;

        public SetupService(ApplicationDbContext context)
        {
            _context = context;
        }

        public MRNDQC_Category AddCategory(MRNDQC_CategoryVM viewModel)
        {
            var result = 0;
            var UpdateDataSet =  _context.MRNDQC_Category.Where(x => x.Id == viewModel.Id).FirstOrDefault();
            var existing =  _context.MRNDQC_Category.Where(x => x.CategoryName == viewModel.CategoryName).FirstOrDefault();
            MRNDQC_Category mdata = new MRNDQC_Category();
            if (existing != null)
            {
                return mdata;
            }
            else
            {
                if (UpdateDataSet != null)
                {
                    UpdateDataSet.CategoryName = viewModel.CategoryName;
                    UpdateDataSet.LUser = viewModel.LUser;
                    UpdateDataSet.UpdatedOn = DateTime.Now;
                    _context.MRNDQC_Category.Update(UpdateDataSet);
                    result =  _context.SaveChanges();
                    mdata = UpdateDataSet;
                }
                else
                {
                    
                    mdata.CategoryName = viewModel.CategoryName;
                    mdata.LUser = viewModel.LUser;
                    _context.MRNDQC_Category.Add(mdata);
                    result =  _context.SaveChanges();
                }
            }
            if(result>0)
            {
                return mdata;
            }
            return mdata;
        }

        public List<MRNDQC_CategoryVM> GetAllCategoryList()
        {
            var items = (from cat in _context.MRNDQC_Category

                         select new MRNDQC_CategoryVM()
                         {
                             Id = cat.Id,
                             CategoryName = cat.CategoryName,
                             IsUpdate = ""
                         }).ToList();
            return items;
        }

        public MRNDQC_CategoryVM GetCategory(Guid Id)
        {
            var items = (from cat in _context.MRNDQC_Category.Where(x => x.Id == Id)

                         select new MRNDQC_CategoryVM()
                         {
                             Id = cat.Id,
                             CategoryName = cat.CategoryName,
                             IsUpdate = "Update"
                         }).FirstOrDefault();
            return items;
        }

        public List<MRNDQC_SubCategoryVM> GetSubCategory(Guid CatId)
        {
            var items = (from cat in _context.MRNDQC_SubCategory.Where(x => x.CategoryID == CatId)

                         select new MRNDQC_SubCategoryVM()
                         {
                             Id = cat.Id,
                             SubCategoryName = cat.SubCategoryName,
                             FaultType = cat.FaultType,
                             CategoryID = cat.CategoryID,
                         }).ToList();
            return items;
        }

        public List<MRNDQC_SubCategoryVM> GetSubCatList()
        {
            var items = (from cat in _context.MRNDQC_SubCategory
                         join mc in _context.MRNDQC_Category
                                on new { X1 = cat.CategoryID } equals new { X1 = mc.Id }
                                into rmp
                         from cm in rmp.DefaultIfEmpty()
                         select new MRNDQC_SubCategoryVM()
                         {
                             Id = cat.Id,
                             SubCategoryName = cat.SubCategoryName,
                             FaultType = cat.FaultType,
                             CategoryName = cm.CategoryName,
                         }).ToList();
            return items;
        }

        public bool UpdateCategory(MRNDQC_CategoryVM viewModel)
        {
            throw new NotImplementedException();
        }

        public List<MRNDQC_SubCategory> GetSubCatByCatID(Guid Id)
        {
            var items = _context.MRNDQC_SubCategory.Where(x => x.CategoryID == Id).ToList();
            return items;
        }

        public MRNDQC_SubCategoryVM GetSubCategory2(Guid Id)
        {
            var items = (from subcat in _context.MRNDQC_SubCategory.Where(x => x.Id == Id)

                         select new MRNDQC_SubCategoryVM()
                         {
                             Id = subcat.Id,
                             SubCategoryName = subcat.SubCategoryName,
                             FaultType = subcat.FaultType,
                             CategoryID = subcat.CategoryID,
                             IsUpdate = "Update"
                         }).FirstOrDefault();
            return items;
        }

        public List<MRNDQC_SubCategoryVM> GetSubCategory3(Guid Id, string FaultType)
        {
            var items = (from subcat in _context.MRNDQC_SubCategory.Where(x => x.CategoryID == Id && x.FaultType == FaultType)

                         select new MRNDQC_SubCategoryVM()
                         {
                             Id = subcat.Id,
                             SubCategoryName = subcat.SubCategoryName,
                             FaultType = subcat.FaultType,
                             CategoryID = subcat.CategoryID,
                             IsUpdate = ""
                         }).ToList();
            return items;
        }

        public bool RemoveDetails(List<MRNDQC_SubCategory> Model)
        {
            _context.MRNDQC_SubCategory.RemoveRange(Model);
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

        public bool AddSubCategory(MRNDQC_SubCategoryVM viewModel)
        {
            var UpdateDataSet = _context.MRNDQC_SubCategory.Where(x => x.Id == viewModel.Id).FirstOrDefault();
            var IsExist = _context.MRNDQC_SubCategory.Where(x => x.SubCategoryName == viewModel.SubCategoryName && x.CategoryID == viewModel.CategoryID).FirstOrDefault();
            if (viewModel == null)
            {
                return false;
            }
            else
            {
                if (UpdateDataSet == null)
                {
                    if (IsExist == null)
                    {
                        _context.MRNDQC_SubCategory.Add(new MRNDQC_SubCategory()
                        {
                            CategoryID = (Guid)viewModel.CategoryID,
                            SubCategoryName = viewModel.SubCategoryName,
                            FaultType = viewModel.FaultType,
                            LUser = viewModel.LUser
                        });
                        var result = _context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    UpdateDataSet.SubCategoryName = viewModel.SubCategoryName;
                    UpdateDataSet.FaultType = viewModel.FaultType;
                    UpdateDataSet.LUser = viewModel.LUser;
                    UpdateDataSet.UpdatedOn = DateTime.Now;
                    _context.MRNDQC_SubCategory.Update(UpdateDataSet);

                    var result = _context.SaveChanges();
                    return true;
                }
            }
        }
    }
}
