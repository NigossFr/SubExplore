using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubExplore.Models.Enums
{
    public enum DocumentType
    {
        FederationCertificate,
        Insurance,
        ProfessionalLicense,
        CompanyRegistration,
        TaxDocument,
        Other
    }

    public enum DocumentStatus
    {
        Pending,
        Valid,
        Invalid,
        Expired
    }

}
