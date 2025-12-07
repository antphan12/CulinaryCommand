using System.ComponentModel.DataAnnotations;

namespace CulinaryCommand.Models
{
    public class CompanySignupModel
    {
        [Required, MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string CompanyCode { get; set; } = string.Empty;

        [MaxLength(256)]
        public string? Address { get; set; } = string.Empty;

        [MaxLength(128)]
        public string? City { get; set; } = string.Empty;

        [MaxLength(64)]
        public string? State { get; set; } = string.Empty;

        [MaxLength(32)]
        public string? ZipCode { get; set; } = string.Empty;

        [MaxLength(128)]
        public string? Phone { get; set; }

        [MaxLength(128)]
        public string? Email { get; set; }

        public string? Description { get; set; }
        public string? LLCName { get; set; }
        public string? TaxId { get; set; }
    }
}