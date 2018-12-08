using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;

namespace Simulator
{
    public class EventHubTransport
    {
        private EventHubClient eventHubClient;

        public EventHubTransport(string connectionString)
        {
            this.eventHubClient = EventHubClient.CreateFromConnectionString(connectionString);
        }

        public async Task SendEventAsync(string payload)
        {
            Guid eventId = Guid.NewGuid();
            var bytes = Encoding.UTF8.GetBytes(payload);
            EventData message = new EventData(bytes);
            message.Properties["EventId"] = eventId.ToString();

            try
            {
                await this.eventHubClient.SendAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                     "{0}{0}*** Exception: SendEventAsync ***{0}{0}EventId: {1}{0}Event Data: {2}{0}Exception: {3}{0}{0}",
                     Console.Out.NewLine,
                     eventId,
                     payload,
                     ex);
            }
        }

        public async Task CloseTransport()
        {
            await this.eventHubClient.CloseAsync();
        }
    }
}
