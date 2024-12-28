using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubExplore.Models.Enums
{
    public enum ModerationActionStatus
    {
        Pending,
        Completed,
        Rejected,
        Cancelled
    }

    public enum ValidationStatus
    {
        Pending,
        Approved,
        Rejected,
        NeedsRevision
    }

    public enum VerificationStatus
    {
        Pending,
        InReview,    // Ajout de cet état qui était dans OrganizationEnums
        Verified,
        Rejected,
        Expired      // Ajout de cet état qui était dans OrganizationEnums
    }

    public enum ReportReason
    {
        Inappropriate,
        Inaccurate,
        Duplicate,
        Safety,
        Other
    }

    public enum ReportStatus
    {
        Pending,
        InReview,
        Resolved,
        Dismissed
    }
}
