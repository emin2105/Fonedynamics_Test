using Fonedynamics_Test.API.Services;
using Fonedynamics_Test.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fonedynamics_Test.API.Endpoints
{
    public static class MapEndpointsExtension
    {
        public static void MapEndpoints(this WebApplication app)
        {
            app.MapPost("api/sms/post", async (SMS sms, [FromServices] SMSService service) =>
            {
                await service.Publish(sms);
            })
            .WithName("SMS");

            app.MapGet("api/sms/{id}", async (Guid id, [FromServices] SMSService service) =>
            {
                var sms = await service.Get(id);
                if (sms == null)
                    return Results.NotFound();
                return Results.Ok(sms);
            })
            .WithName("Get");

            app.MapGet("api/sms/list", async ([FromServices] SMSService service) =>
            {
                return await service.GetAll();
            })
            .WithName("All");
        }
    }
}
