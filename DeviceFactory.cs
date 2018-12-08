using System.Collections.Generic;

namespace Simulator
{
    public class DeviceFactory
    {
        private EventHubTransport transport;
        public DeviceFactory(EventHubTransport transport)
        {
            this.transport = transport;
        }

        public IList<Device> GenerateDevices(List<string[]> deviceList)
        {
            var devices = new List<Device>();
            foreach (var deviceToken in deviceList)
            {
                var name = deviceToken[0];
                var isServiceElevator = bool.Parse(deviceToken[1]);
                var isBlocked = bool.Parse(deviceToken[2]);
                var hasFaultyAC = bool.Parse(deviceToken[3]);

                var behaviour = new DeviceBehaviour()
                {
                    IsBlocked = isBlocked,
                    IsService = isServiceElevator,
                    HasFaultyACUnit = hasFaultyAC
                };

                var device = new Device(name, behaviour, this.transport);

                devices.Add(device);
            }

            return devices;
        }
    }
}
