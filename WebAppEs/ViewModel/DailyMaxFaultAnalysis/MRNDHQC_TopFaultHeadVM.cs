using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebAppEs.ViewModel.Category;
using WebAppEs.ViewModel.FaultsEntry;
using WebAppEs.ViewModel.PartsModel;

namespace WebAppEs.ViewModel.DailyMaxFaultAnalysis
{
    public class MRNDHQC_TopFaultHeadVM
    {
        public Guid Id { get; set; }
        public string EmployeeID { get; set; }
        public string AnalysisType { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }
        public string DateString { get; set; }
        public Guid PartsModelID { get; set; }
        public string LineNo { get; set; }
        public string Model { get; set; }
        public string ModelWithLine { get; set; }
        public bool StatusIsToday { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
        public Guid LUser { get; set; }
        public string wwwrootpath { get; set; }
        public IEnumerable<MRNDHQC_TopFaultAnalysisVM> MRNDHQC_TopFaultAnalysisList { get; set; }
        public IEnumerable<MRNDHQC_TopFaultHeadVM> MRNDHQC_TopFaultHeadList { get; set; }

        public IEnumerable<PartsModelViewModel> PartsModelViewModel { get; set; }
        public IEnumerable<MRNDQC_CategoryVM> MRNDQC_CategoryVM { get; set; }
        public IEnumerable<MRNDQC_SubCategoryVM> MRNDQC_SubCategoryVM { get; set; }
        public IEnumerable<MobileRNDFaultsEntryViewModel> MobileRNDFaultsEntryViewModelList { get; set; }

        public string IsUpdate { get; set; }
    }
}
