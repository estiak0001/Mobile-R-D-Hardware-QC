using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppEs.Entity
{
    [Table(name: "MRNDQC_SubCategory")]
    public class MRNDQC_SubCategory
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(150)]
        public string SubCategoryName { get; set; }

        [Required]
        [StringLength(150)]
        public string FaultType { get; set; }

        [ForeignKey("MRNDQC_Category")]
        public Guid CategoryID { get; set; }
        public virtual MRNDQC_Category MRNDQC_Category { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
        public Guid LUser { get; set; }
    }
}
