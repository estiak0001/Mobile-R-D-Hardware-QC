using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppEs.ViewModel.DailyMaxFaultAnalysis
{
    public class MRNDHQC_TopFaultAnalysisVM
    {
        public Guid Id { get; set; }

        public string EmployeeID { get; set; }
        public string AnalysisType { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }
        public string DateString { get; set; }
        public Guid PartsModelID { get; set; }
        public string Model { get; set; }
        public Guid CategoryID { get; set; }
        public string Category { get; set; }
        public Guid SubCategoryID { get; set; }
        public string SubCategory { get; set; }
        public string Reason { get; set; }
        public int Sample { get; set; }
        public int Quantity { get; set; }
        public string Remarks { get; set; }
        public string ProblemSolAndRec { get; set; }
        public string ImageUrl { get; set; }
        public Guid HeadID { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
        public Guid LUser { get; set; }
        public string Line { get; set; }
        public string DisplayUrl { get; set; }
    }
}
