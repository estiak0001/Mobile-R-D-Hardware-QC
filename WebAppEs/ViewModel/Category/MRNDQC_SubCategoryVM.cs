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

        [Required]
        public string SubCategoryName { get; set; }

        [Required]
        public string FaultType { get; set; }

        public Guid CategoryID { get; set; }

        public Guid LUser { get; set; }

        public string IsUpdate { get; set; }
        public IEnumerable<MRNDQC_CategoryVM> MRNDQC_CategoryVM { get; set; }
    }
}
