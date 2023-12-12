using ECommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Application
{
    public class City : CommonFields
    {
        [Key]
        public string Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
    }
}
