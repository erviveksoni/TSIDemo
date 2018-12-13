using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simulator
{
    class Program
    {
        private static CancellationTokenSource ctsSource;

        // duration for which the simulation should run
        private static int durationMinutes = 5;

        static void Main(string[] args)
        {
            ctsSource = new CancellationTokenSource();

            // Event hub connection string
            var connectionString =
                "Endpoint=sb://tsieventhub-ns.servicebus.windows.net/;SharedAccessKeyName=SendPolicy;SharedAccessKey=aw+mNuPRuopfm2brdcXLvwgFZ0x9LTSMhEzcoe4JeBI=;EntityPath=tsieventhub";
            var transport = new EventHubTransport(connectionString);

            try
            {
                var deviceFactory = new DeviceFactory(transport);

                var dataRows = new List<string[]>();
                var csvLines = File.ReadAllLines(@"Data/SimulationDevices.csv", Encoding.UTF8);
                var rawrows = csvLines.Skip(1); // skip header row
                foreach (string line in rawrows)
                {
                    dataRows.Add(line.Split(','));
                }

                Console.WriteLine($"Preparing {dataRows.Count} devices...");

                var devices = deviceFactory.GenerateDevices(dataRows);

                StartSendLoopAsync(devices);

                Console.WriteLine($"Press any key to stop the loop or wait for {durationMinutes} minute(s)...");
                Console.ReadKey();
                ctsSource.Cancel();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                transport.CloseTransport().GetAwaiter();
            }
        }

        private static async void StartSendLoopAsync(IList<Device> devices)
        {
            try
            {
                var tasks = new List<Task>();
                foreach (var device in devices)
                {
                    tasks.Add(device.SendTelemetryAsync(durationMinutes, ctsSource.Token));
                }

                Console.WriteLine($"Starting to send telemetry...");

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                Console.WriteLine("Operation was completed...");
                Console.WriteLine($"Press any key to exit...");
            }
        }
    }
}
