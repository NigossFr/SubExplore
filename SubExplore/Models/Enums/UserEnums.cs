using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubExplore.Models.Enums
{
    public enum AccountType
    {
        Standard,
        Moderator,
        Professional,
        Administrator
    }

    public enum SubscriptionStatus
    {
        Free,
        Premium,
        PremiumPlus,
        Suspended
    }

    public enum SubscriptionPlan
    {
        Free,
        Premium,
        PremiumPlus
    }
}
