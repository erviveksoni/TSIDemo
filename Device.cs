using System;
using System.Threading;
using System.Threading.Tasks;

namespace Simulator
{
    public class Device
    {
        private string deviceId;
        private EventHubTransport transport;
        private DeviceBehaviour deviceBehaviour;
        private SampleDataGenerator temperatureGenerator;
        private SampleDataGenerator humidityGenerator;
        private SampleDataGenerator vibrationGenerator;
        private SampleDataGenerator loadGenerator;

        private const int REPORT_FREQUENCY_IN_SECONDS = 1;
        private const int PEAK_FREQUENCY_IN_SECONDS = 18;
        private int currentFloor;

        public Device(
            string deviceId,
            DeviceBehaviour deviceBehaviour,
            EventHubTransport transport)
        {
            this.deviceId = deviceId;
            this.transport = transport;
            this.deviceBehaviour = deviceBehaviour;
            int peakFrequencyInTicks = Convert.ToInt32(Math.Ceiling((double)PEAK_FREQUENCY_IN_SECONDS / REPORT_FREQUENCY_IN_SECONDS));

            if (!this.deviceBehaviour.IsService)
            {
                this.temperatureGenerator = new SampleDataGenerator(22, 24, 30, peakFrequencyInTicks);
                this.humidityGenerator = new SampleDataGenerator(20, 50);
                this.vibrationGenerator = new SampleDataGenerator(0, 10);
                this.loadGenerator = new SampleDataGenerator(200, 800);
            }
            else
            {
                this.temperatureGenerator = new SampleDataGenerator(22, 26, 32, peakFrequencyInTicks);
                this.humidityGenerator = new SampleDataGenerator(20, 50);
                this.vibrationGenerator = new SampleDataGenerator(1, 10);
                this.loadGenerator = new SampleDataGenerator(300, 1200);
            }
        }

        public async Task SendTelemetryAsync(int durationInMinutes, CancellationToken token)
        {
            var monitorData = new Telemetry();
            this.currentFloor = 0;
            monitorData.Temperature = this.temperatureGenerator.GetNextValue();
            monitorData.Humidity = this.humidityGenerator.GetNextValue();

            var totalMessages = durationInMinutes * 60; // assuming a rate of 1 message per second
            int totalMessagesSent = 0;
            Random floorRandom = new Random();

            while (!token.IsCancellationRequested &&
                   totalMessagesSent <= totalMessages)
            {
                try
                {
                    var previousFloor = this.currentFloor;
                    monitorData.DeviceId = this.deviceId;
                    monitorData.TimeStamp = DateTime.UtcNow;

                    if (!this.deviceBehaviour.HasFaultyACUnit)
                    {
                        monitorData.Temperature = this.temperatureGenerator.GetNextValue();
                        monitorData.Humidity = this.humidityGenerator.GetNextValue();
                    }
                    else
                    {
                        monitorData.Temperature = Utility.IncrementValue(monitorData.Temperature, 40);
                        monitorData.Humidity = Utility.IncrementValue(monitorData.Humidity, 20);
                    }

                    if (this.deviceBehaviour.IsBlocked)
                    {
                        // Assign only once
                        if (this.currentFloor < 1)
                        {
                            this.currentFloor = Utility.GetFloorRandom(1, 15);
                        }

                        monitorData.Vibration = 0;
                        monitorData.Load = 0;
                        monitorData.Distance = 0;
                    }
                    else
                    {
                        this.currentFloor = Utility.GetFloorIncremental(previousFloor, 1, 20);
                        monitorData.Vibration = this.vibrationGenerator.GetNextValue();
                        monitorData.Load = this.loadGenerator.GetNextValue();
                        var floorDiff = this.currentFloor - previousFloor;
                        var distance = floorDiff * 10;
                        monitorData.Distance = distance < 0 ? distance * -1 : distance;
                    }

                    monitorData.Floor = this.currentFloor;

                //   await this.transport.SendEventAsync(monitorData.ToString());
                }
                finally
                {
                    totalMessagesSent++;
                    await Task.Delay(TimeSpan.FromSeconds(REPORT_FREQUENCY_IN_SECONDS), token);
                }
            }

            Console.WriteLine($"Device {this.deviceId} finished sending telemetry messages!");
        }
    }
}
