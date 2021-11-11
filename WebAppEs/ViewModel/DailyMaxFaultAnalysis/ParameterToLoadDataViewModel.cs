using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppEs.ViewModel.DailyMaxFaultAnalysis
{
    public class ParameterToLoadDataViewModel
    {
        public DateTime? Date { get; set; }
        public Guid? ModelID { get; set; }
        public string FaultType { get; set; }
        public Guid? CategoryID { get; set; }
        public Guid? SubCategoryID { get; set; }
        public string LineNo { get; set; }
    }
}
