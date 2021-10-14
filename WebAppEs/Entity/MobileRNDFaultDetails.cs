using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAppEs.Models;

namespace WebAppEs.Entity
{
    public class MobileRNDFaultDetails
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string EmployeeID { get; set; }

        [ForeignKey("MobileRNDFaultsEntry")]
        public Guid FaultEntryID { get; set; }
        public virtual MobileRNDFaultsEntry MobileRNDFaultsEntry { get; set; }

        [Required]
        [StringLength(50)]
        public DateTime? Date { get; set; }

        [Required]
        [StringLength(150)]
        public Guid CategoryID { get; set; }

        [Required]
        [StringLength(150)]
        public Guid SubCategoryID { get; set; }

        [Required]
        [StringLength(50)]
        public int FaultQty { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
        public Guid LUser { get; set; }
    }
}
