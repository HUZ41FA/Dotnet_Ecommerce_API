using ECommerce.Domain.Application;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Common
{
    public class CommonFields : ICommonFields
    {
        [Required]
        public bool IsDeleted { get; set; } = false;
        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public string CreatedBy { get; set; } = null!;
        [Required]
        public DateTime CreatedAt { get; set; }
        [AllowNull]
        [MaxLength(255)]
        public string? UpdatedBy { get; set; }
        [AllowNull]
        public DateTime? UpdatedAt { get; set; }
        [AllowNull]
        [MaxLength(255)]
        public string? DeletedBy { get; set; }
        [AllowNull]
        public DateTime? DeletedAt { get; set; }

        #region Relationships
        [ForeignKey("CreatedBy")]
        public SiteUser Creator { get; set; }
        [ForeignKey("UpdatedBy")]
        public SiteUser Updater { get; set; }
        [ForeignKey("DeletedBy")]
        public SiteUser Deleter { get; set; }
        #endregion
    }
}
