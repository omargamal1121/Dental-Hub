using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Common
{
    public enum CaseStatus
    {
        Assigned,
        InProgress,
        Completed
    }
    public enum Gender
    {
        Male,
        Female
    }

    public enum DiagnosisSource
    {
        AI,
        Clinic
    }

    public enum CaseSortBy
    {
        Name,
        Date,
        Age
    }


}
