using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppEs.ViewModel.Home
{
    public class FaultPercentageForChartWithFunAes
    {
        public double FunctionalSingle { get; set; }
        public double AestheticsSingle { get; set; }
        public double[] Functional { get; set; }
        public double[] Aesthetic { get; set; }
    }
}
