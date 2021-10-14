using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppEs.ViewModel.Category
{
    public class AddSubCategoryVM
    {
        public Guid Id { get; set; }
        public Guid LUser { get; set; }
        public IEnumerable<MRNDQC_SubCategoryVM> MRNDQC_SubCategoryVM { get; set; }
    }
}
