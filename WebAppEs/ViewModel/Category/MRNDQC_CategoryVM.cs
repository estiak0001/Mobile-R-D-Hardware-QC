using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppEs.ViewModel.Category
{
    public class MRNDQC_CategoryVM
    {
        public Guid Id { get; set; }

        [Required]
        public string CategoryName { get; set; }

        public string FaultType { get; set; }

        public string SubCategoryName { get; set; }

        public string IsUpdate { get; set; }
        public Guid LUser { get; set; }
        public IEnumerable<MRNDQC_SubCategoryVM> MRNDQC_SubCategoryVM { get; set; }

    }
}
