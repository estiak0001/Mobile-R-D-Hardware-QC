using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAppEs.Models;

namespace WebAppEs.Entity
{
    [Table(name: "MobileRNDFaultsEntry")]
    public class MobileRNDFaultsEntry
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string EmployeeID { get; set; }
        [Required]
        [StringLength(50)]
        public DateTime? Date { get; set; }
        [Required]
        [StringLength(50)]
        public string LineNo { get; set; }

        [Required]
        [StringLength(50)]
        public Guid PartsModelID { get; set; }

        [Required]
        [StringLength(50)]
        public string LotNo { get; set; }

        [Required]
        [StringLength(50)]
        public string Shipment { get; set; }

        [Required]
        [StringLength(50)]
        public string Shift { get; set; }

        [Required]
        [StringLength(50)]
        public int TotalIssueQty { get; set; }

        [Required]
        [StringLength(150)]
        public string TypeOfProduction { get; set; }

        [Required]
        [StringLength(50)]
        public int QCPass { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
        public Guid LUser { get; set; }
    }
}