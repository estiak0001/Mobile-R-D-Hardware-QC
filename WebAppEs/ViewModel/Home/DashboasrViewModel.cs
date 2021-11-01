using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebAppEs.ViewModel.Home
{
    public class DashboasrViewModel
    {
        //public DateTime Date { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }
        public ChartLevelViewModel Lavel { get; set; }
        public FaultPercentageForChartWithFunAes FaultPercentageForChartWithFunAes { get; set; }
    }
}
