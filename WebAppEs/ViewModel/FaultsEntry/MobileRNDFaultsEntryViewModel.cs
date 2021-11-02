using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebAppEs.ViewModel.Category;
using WebAppEs.ViewModel.PartsModel;

namespace WebAppEs.ViewModel.FaultsEntry
{
    public class MobileRNDFaultsEntryViewModel
    {
        public Guid Id { get; set; }
        public string EmployeeID { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }


        public string DateString { get; set; }
        [Required]
        public string LineNo { get; set; }

        public string Line { get; set; }
        public string ModelName { get; set; }
        public string ModelNameWithLot { get; set; }
        [Required]
        public Guid PartsModelID { get; set; }
        [Required]
        public string LotNo { get; set; }
        public string Lot { get; set; }

        [Required]
        public string Shipment { get; set; }

        [Required]
        public string Shift { get; set; }

        [Required]
        public int? TotalIssueQty { get; set; }

        [Required]
        public string TypeOfProduction { get; set; }

        public int? QCPass { get; set; }

        public IEnumerable<PartsModelViewModel> PartsModelViewModel { get; set; }
        public IEnumerable<MRNDQC_CategoryVM> MRNDQC_CategoryVM { get; set; }
        public IEnumerable<MRNDQC_SubCategoryVM> MRNDQC_SubCategoryVM { get; set; }
        public IEnumerable<MobileRNDFaultsEntryViewModel> MobileRNDFaultsEntryViewModelList { get; set; }
        public string ButtonText { get; set; }
        public string Disabled { get; set; }
        public DateTime CreateDate { get; set; }

        public IEnumerable<MobileRNDFaultDetailsViewModel> MobileRNDFaultDetailsViewModel { get; set; }
        public bool StatusIsToday { get; set; }
        public Guid UserID { get; set; }
    }
}
