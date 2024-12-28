using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubExplore.Models
{
    public class Membership
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int OrganizationId { get; set; }

        [Required]
        public MembershipRole Role { get; set; }

        [Required]
        public MembershipStatus Status { get; set; }

        [Required]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public DateTime? EndDate { get; set; }

        public string? LicenseNumber { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization? Organization { get; set; }
    }

    public enum MembershipRole
    {
        Member,
        Instructor,
        Administrator,
        Staff
    }

    public enum MembershipStatus
    {
        Active,
        Pending,
        Suspended,
        Expired
    }
}
