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
        private SampleDataGenerator dscGenerator;

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

            this.dscGenerator = new SampleDataGenerator(0, 0.6);
            this.temperatureGenerator = new SampleDataGenerator(22, 24, 30, peakFrequencyInTicks);
            this.humidityGenerator = new SampleDataGenerator(20, 50);
            this.vibrationGenerator = new SampleDataGenerator(0, 10);

            this.loadGenerator = !this.deviceBehaviour.IsService ? new SampleDataGenerator(200, 800) : new SampleDataGenerator(300, 1200);
        }

        public async Task SendTelemetryAsync(int durationInMinutes, CancellationToken token)
        {
            var monitorData = new Telemetry();
            this.currentFloor = 0;
            monitorData.Temperature = this.temperatureGenerator.GetNextValue();
            monitorData.Humidity = this.humidityGenerator.GetNextValue();

            // if elevator is busy then send 2 messages per second else 1 message
            var totalMessages = (this.deviceBehaviour.IsBusy ? 2 : 1) * durationInMinutes * 60;

            // if elevator is busy then frequence is 500ms else 1s
            int frequency = (this.deviceBehaviour.IsBusy ? 1 : 2) * (REPORT_FREQUENCY_IN_SECONDS * 500);

            int totalMessagesSent = 0;

            while (!token.IsCancellationRequested &&
                   totalMessagesSent <= totalMessages)
            {
                try
                {
                    var previousFloor = this.currentFloor;
                    monitorData.DeviceId = this.deviceId;
                    monitorData.TimeStamp = DateTime.UtcNow;

                    this.InjectfaultyAcBehaviour(this.deviceBehaviour.HasFaultyACUnit, monitorData);

                    this.InjectJerkyBehaviour(this.deviceBehaviour.IsJerky, monitorData);

                    this.InjectBlockedElevatorBehaviour(this.deviceBehaviour.IsBlocked, previousFloor, monitorData);

                    this.InjectAUXPowerBehaviour(this.deviceBehaviour.IsOnAuxPower, monitorData);

                    this.InjectFaultyDscBehaviour(this.deviceBehaviour.IsDSCMalfunctioning, monitorData);

                    monitorData.Floor = this.currentFloor;

                    await this.transport.SendEventAsync(monitorData.ToString());
                }
                finally
                {
                    totalMessagesSent++;
                    await Task.Delay(TimeSpan.FromMilliseconds(frequency), token);
                }
            }

            Console.WriteLine($"Device {this.deviceId} finished sending telemetry messages!");
        }

        private void InjectfaultyAcBehaviour(bool isfaulty, Telemetry monitorData)
        {
            if (!isfaulty)
            {
                monitorData.Temperature = this.temperatureGenerator.GetNextValue();
                monitorData.Humidity = this.humidityGenerator.GetNextValue();
            }
            else
            {
                monitorData.Temperature = Utility.IncrementValue(monitorData.Temperature, 40);
                monitorData.Humidity = Utility.DecrementValue(monitorData.Humidity, 2);
            }
        }

        private void InjectJerkyBehaviour(bool isJerky, Telemetry monitorData)
        {
            if (!isJerky)
            {
                monitorData.Vibration = this.vibrationGenerator.GetNextValue();
                monitorData.Vibration = monitorData.Vibration > 5 ? 5 - Utility.GetRandomValue(0, 4) : monitorData.Vibration;
                monitorData.Jerks = 0;
            }
            else
            {
                monitorData.Vibration = this.vibrationGenerator.GetNextValue();
                monitorData.Vibration = monitorData.Vibration < 5 ? 5 + Utility.GetRandomValue(0, 4) : monitorData.Vibration;
                monitorData.Jerks = Utility.GetRandomValue(0, 4);
            }
        }

        private void InjectBlockedElevatorBehaviour(bool isblocked, int previousFloor, Telemetry monitorData)
        {
            if (!isblocked)
            {
                this.currentFloor = Utility.GetFloorIncremental(previousFloor, 1, 20);
                var floorDiff = this.currentFloor - previousFloor;
                var distance = floorDiff * 10;

                monitorData.Distance = distance < 0 ? distance * -1 : distance;
                monitorData.NumberOfDoorCycles = Utility.GetRandomValue(1, 5);
                monitorData.Load = this.loadGenerator.GetNextValue();
            }
            else
            {
                // Assign only once
                if (this.currentFloor < 1)
                {
                    this.currentFloor = Utility.GetRandomValue(1, 15);
                }

                monitorData.Vibration = 0;
                monitorData.Load = 0;
                monitorData.Distance = 0;
                monitorData.NumberOfDoorCycles = Utility.GetRandomValue(0, 1);
            }
        }

        private void InjectAUXPowerBehaviour(bool isOnAuxPower, Telemetry monitorData)
        {
            if (!isOnAuxPower)
            {
                monitorData.PowerType = PowerType.Main.ToString();
            }
            else
            {
                monitorData.PowerType = PowerType.AUX.ToString();
                monitorData.Jerks = Utility.GetRandomValue(1, 4);
            }
        }

        private void InjectFaultyDscBehaviour(bool isfaulty, Telemetry monitorData)
        {
            if (!isfaulty)
            {
                monitorData.DoorSafetyReading = this.dscGenerator.GetNextValue();
                monitorData.DoorSafetyReading = monitorData.DoorSafetyReading < 0.7
                    ? (0.7 - monitorData.DoorSafetyReading) + monitorData.DoorSafetyReading
                    : monitorData.DoorSafetyReading;
            }
            else
            {
                monitorData.DoorSafetyReading = this.dscGenerator.GetNextValue();
            }
        }
    }
}
