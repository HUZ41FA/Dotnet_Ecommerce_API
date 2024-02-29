using ECommerce.Domain.Models.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ECommerce.Domain.Models.Application
{
    public class SiteUser : IdentityUser, ICommonFields
    {
        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; } = null!;
        [Required]
        [MaxLength(255)]
        public string LastName { get; set; } = null!;
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

        [ForeignKey("CreatedBy")]
        public SiteUser Creator { get; set; }
        [ForeignKey("UpdatedBy")]
        public SiteUser Updater { get; set; }
        [ForeignKey("DeletedBy")]
        public SiteUser Deleter { get; set; }

        #region Not Mapped
        [NotMapped]
        public string Password { get; set; }
        #endregion
    }
}
