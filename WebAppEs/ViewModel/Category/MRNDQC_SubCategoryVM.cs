using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppEs.ViewModel.Category
{
    public class MRNDQC_SubCategoryVM
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; }

        [Required]
        [Display(Name = "Sub Category")]
        public string SubCategoryName { get; set; }

        [Required]
        [Display(Name = "Fault Type")]
        public string FaultType { get; set; }

        [Required]
        [Display(Name = "Category")]
        public Guid? CategoryID { get; set; }

        public Guid LUser { get; set; }

        public string IsUpdate { get; set; }
        public IEnumerable<MRNDQC_CategoryVM> MRNDQC_CategoryVM { get; set; }
    }
}
