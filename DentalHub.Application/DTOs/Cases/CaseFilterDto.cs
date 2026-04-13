using DentalHub.Domain.Entities;

namespace DentalHub.Application.DTOs.Cases
{
    public class CaseFilterDto
    {
        public string? PatientName { get; set; }
        public string? CaseType { get; set; }
        public string? Status { get; set; }
        public Gender? Gender { get; set; }
        
        /// <summary>
        /// Determine which field to sort by. E.g., "Name", "Date", "Age"
        /// </summary>
        public string? SortBy { get; set; }
        
        /// <summary>
        /// "asc" or "desc"
        /// </summary>
        public string? SortDirection { get; set; } 

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
