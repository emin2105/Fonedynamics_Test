using Fonedynamics_Test.API.ViewModel;
using Fonedynamics_Test.Shared.Data;
using Fonedynamics_Test.Shared.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Fonedynamics_Test.API.Services
{
    public class SMSService
    {
        private readonly IPublishEndpoint publishEndpoint;
        private readonly SMSDbContext context;

        public SMSService(IPublishEndpoint publishEndpoint, SMSDbContext context)
        {
            this.publishEndpoint = publishEndpoint;
            this.context = context;
        }
        public async Task Publish(SMS sms)
        {
            await publishEndpoint.Publish(sms);
        }

        public async Task<IEnumerable<SMSListViewModel>> GetAll()
        {
            return await context.ProcessedSMS.Select(s => new SMSListViewModel(s)).ToListAsync();
        }

        public async Task<SMSViewModel> Get(Guid id)
        {
            var result = await context.ProcessedSMS.FirstOrDefaultAsync(s => s.Id == id);
            if (result != null)
            {
                return new SMSViewModel(result);
            }
            return null;
        }

    }
}
