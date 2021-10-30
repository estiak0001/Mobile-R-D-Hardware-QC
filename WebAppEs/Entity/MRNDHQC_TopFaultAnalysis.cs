using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppEs.Entity
{
    [Table(name: "MRNDHQC_TopFaultAnalysis")]
    public class MRNDHQC_TopFaultAnalysis
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(150)]
        public Guid CategoryID { get; set; }

        [Required]
        [StringLength(150)]
        public Guid SubCategoryID { get; set; }

        [Required]
        public string Reason { get; set; }

        [Required]
        [StringLength(50)]
        public int Sample { get; set; }
        [Required]
        [StringLength(50)]
        public int Quantity { get; set; }
        [Required]
        public string Remarks { get; set; }

        [Required]
        public string ProblemSolAndRec { get; set; }

        [Required]
        [StringLength(150)]
        public string ImageUrl { get; set; }

        [ForeignKey("MRNDHQC_TopFaultHead")]
        public Guid HeadID { get; set; }
        public virtual MRNDHQC_TopFaultHead MRNDHQC_TopFaultHead { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
        public Guid LUser { get; set; }
    }
}
