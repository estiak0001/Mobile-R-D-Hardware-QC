using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppEs.Entity
{
    [Table(name: "MRNDQC_Category")]
    public class MRNDQC_Category
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(150)]
        public string CategoryName { get; set; }


        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
        public Guid LUser { get; set; }
    }
}
