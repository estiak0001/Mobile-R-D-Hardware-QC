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
                         }).ToList();
            return items;
        }

        public List<MRNDQC_SubCategoryVM> GetSubCatList()
        {
            var items = (from cat in _context.MRNDQC_SubCategory

                         select new MRNDQC_SubCategoryVM()
                         {
                             Id = cat.Id,
                             SubCategoryName = cat.SubCategoryName,
                             FaultType = cat.FaultType,
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
            var items = (from cat in _context.MRNDQC_SubCategory.Where(x => x.Id == Id)

                         select new MRNDQC_SubCategoryVM()
                         {
                             Id = cat.Id,
                             SubCategoryName = cat.SubCategoryName,
                             FaultType = cat.FaultType,
                         }).FirstOrDefault();
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
            if (viewModel == null)
            {
                return false;
            }
            else
            {
                _context.MRNDQC_SubCategory.Add(new MRNDQC_SubCategory()
                {
                    CategoryID = viewModel.CategoryID,
                    SubCategoryName = viewModel.SubCategoryName,
                    FaultType = viewModel.FaultType,
                    LUser = viewModel.LUser
                });
            }
            var result = _context.SaveChanges();

            return true;
        }
    }
}
