using ECommerce.Domain.Common;
using Microsoft.EntityFrameworkCore;
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
    [PrimaryKey("AddressId", "UserId")]
    public class UserAddress : CommonFields
    {
        [Required]
        public bool IsDeleted { get; set; } = false;
        [Required]
        public bool IsActive { get; set; } = true;
        public string Region { get; set; }

        #region Relationships
        public string AddressId { get; set; }
        [ForeignKey("AddressId")]
        public Address Address { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public SiteUser SiteUser { get; set; }
        #endregion
    }
}
