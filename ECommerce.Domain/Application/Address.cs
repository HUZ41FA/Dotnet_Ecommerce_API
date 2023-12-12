using ECommerce.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ECommerce.Domain.Application
{
    public class Address : CommonFields
    {
        [Key]
        public string Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string HouseNo { get; set; }
        [Required]
        [MaxLength(255)]
        public string StreetNo { get; set; }
        [Required]
        [MaxLength(255)]
        public string AddressLine1 { get; set; }
        [MaxLength(255)]
        public string AddressLine2 { get; set; }
        public string Region { get; set; }

        #region Relationships
        public string CityId { get; set; }
        public City City { get; set; }
        #endregion
    }
}
