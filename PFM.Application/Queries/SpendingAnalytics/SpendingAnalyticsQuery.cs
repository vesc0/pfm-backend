using MediatR;
using PFM.Application.Dtos;

namespace PFM.Application.Queries.SpendingAnalytics
{
    public class SpendingAnalyticsQuery : IRequest<SpendingsByCategoryDto>
    {
        public string? Category   { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate   { get; set; }
        public string? Direction { get; set; }
    }
}
