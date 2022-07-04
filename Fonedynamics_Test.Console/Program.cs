using Fonedynamics_Test.Shared.Models;
using MassTransit;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Fonedynamics_Test.Shared.Data;
using Microsoft.EntityFrameworkCore;

IConfiguration configuration = new ConfigurationBuilder()
   .AddJsonFile("appsettings.json")
   .AddEnvironmentVariables()
   .AddCommandLine(args)
   .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection");

var serviceProvider = new ServiceCollection()
    .AddDbContext<SMSDbContext>(options => options.UseSqlServer(connectionString, x => x.MigrationsAssembly("Fonedynamics_Test.Shared"))).BuildServiceProvider();

using (var scope = serviceProvider?.CreateScope())
{
    var context = scope?.ServiceProvider?.GetRequiredService<SMSDbContext>();
    context.Database.Migrate();
}

var bus = Bus.Factory.CreateUsingRabbitMq(config =>
{
    config.Host(configuration.GetValue<string>("RabbitMqUrl"));

    config.ReceiveEndpoint("sms_queue", ep =>
    {
        ep.Handler<SMS>(async context =>
        {
            await ProcessSMSAsync(context.Message);
        });
    });
});

bus.Start();

while (true)
{
    Console.WriteLine("Print \"post\" to post a message or \"quit\" to quit");
    var command = Console.ReadLine();
    if (command == "quit") break;
    else if (command == "post") await PostSMSAsync();
}

Console.Write("Press any key to quit...");
Console.ReadLine();

// stops the bus and ends the connection to RabbitMQ
bus.Stop();

static bool IsValidPhoneNumber(string number)
{
    if (number != null) return Regex.IsMatch(number, @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$");
    return false;
}

async Task ProcessSMSAsync(SMS sms)
{
    try
    {
        using (var scope = serviceProvider?.CreateScope())
        {
            var context = scope?.ServiceProvider?.GetRequiredService<SMSDbContext>();
            foreach (var number in sms.To)
            {
                var processedSMS = new ProcessedSMS
                {
                    Content = sms.Content,
                    From = sms.From,
                    To = number
                };
                if (IsValidPhoneNumber(number))
                {
                    processedSMS.Status = "Delivered";
                    Console.WriteLine(number);
                }
                else { Console.WriteLine("Failed " + number); processedSMS.Status = "Failed"; };

                context?.ProcessedSMS?.Add(processedSMS);
                await context.SaveChangesAsync();
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }


}

async Task PostSMSAsync()
{
    var sms = new SMS();
    Console.Write("Please enter your number: ");
    sms.From = Console.ReadLine();
    Console.Write("Please enter destination numbers separated by coma: ");
    var numbers = Console.ReadLine()?.Split(',');
    sms.To = numbers;
    Console.WriteLine("Please enter the message:");
    sms.Content = Console.ReadLine();
    var data = JsonConvert.SerializeObject(sms);

    try
    {
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(configuration.GetValue<string>("ApiUrl"));
            var content = new StringContent(data);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await client.PostAsync("api/sms/post", content).ConfigureAwait(false);
            Console.WriteLine("Message is sent");

        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }

}