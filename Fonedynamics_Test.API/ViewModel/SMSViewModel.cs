using Fonedynamics_Test.Shared.Models;

namespace Fonedynamics_Test.API.ViewModel
{
    public record SMSViewModel
    {
        public string? Id { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public string? Content { get; set; }
        public string? Status { get; set; }

        public SMSViewModel(ProcessedSMS processedSMS)
        {
            Id = processedSMS.Id.ToString();
            From = processedSMS.From;
            To = processedSMS.To;
            Content = processedSMS.Content;
            Status = processedSMS.Status;
        }
    }
}
