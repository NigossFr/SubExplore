using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SubExplore.Models.Enums;

namespace SubExplore.Models
{
    public class SubscriptionGift
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ModeratorId { get; set; }

        public int? AdminId { get; set; }

        [Required]
        public int Duration { get; set; }

        public string? Reason { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public SubscriptionGiftStatus Status { get; set; } = SubscriptionGiftStatus.Pending;

        // Navigation properties
        [ForeignKey("ModeratorId")]
        public virtual User? Moderator { get; set; }

        [ForeignKey("AdminId")]
        public virtual User? Admin { get; set; }
    }
}
