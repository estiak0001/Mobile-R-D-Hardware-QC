﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppEs.ViewModel.FaultsEntry
{
    public class MobileRNDFaultDetailsViewModel
    {
        public Guid FaultEntryId { get; set; }
        public string EmployeeID { get; set; }

        public DateTime? Date { get; set; }

        public string FaultType { get; set; }

        public int FaultQty { get; set; }

        public Guid CategoryID { get; set; }

        public Guid SubCategoryID { get; set; }

        public Guid UserID { get; set; }

        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string Model { get; set; }
        public Guid ModelID { get; set; }
    }
}
