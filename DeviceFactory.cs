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
                var hasFaultyAc = bool.Parse(deviceToken[3]);
                var isBusy = bool.Parse(deviceToken[4]);

                var behaviour = new DeviceBehaviour()
                {
                    IsService = isServiceElevator,
                    HasFaultyACUnit = hasFaultyAc,
                    IsBusy = isBusy
                };

                #region BlockedBehaviour
                //var isBlocked = bool.Parse(deviceToken[2]);
                //behaviour.IsBlocked = isBlocked;
                #endregion

                var device = new Device(name, behaviour, this.transport);

                devices.Add(device);
            }

            return devices;
        }
    }
}
