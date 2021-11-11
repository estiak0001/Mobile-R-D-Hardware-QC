using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppEs.Entity
{
    [Table(name: "MRNDHQC_TopFaultHead")]
    public class MRNDHQC_TopFaultHead
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string EmployeeID { get; set; }

        [Required]
        [StringLength(50)]
        public string AnalysisType { get; set; }

        [Required]
        [StringLength(50)]
        public DateTime? Date { get; set; }

        [Required]
        [StringLength(50)]
        public Guid PartsModelID { get; set; }

        [Required]
        [StringLength(50)]
        public string LineNo { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
        public Guid LUser { get; set; }
    }
}
