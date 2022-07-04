using Fonedynamics_Test.Shared.Models;

namespace Fonedynamics_Test.API.ViewModel
{
    public record SMSListViewModel
    {
        public string? Id { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public string? Status { get; set; }

        public SMSListViewModel(ProcessedSMS processedSMS)
        {
            Id = processedSMS.Id.ToString();
            From = processedSMS.From;
            To = processedSMS.To;
            Status = processedSMS.Status;
        }
    }
}
