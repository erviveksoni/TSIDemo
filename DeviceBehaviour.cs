namespace Simulator
{
    public class DeviceBehaviour
    {
        // is Elevator blocked on a floor
        public bool IsBlocked { get; set; }

        // Is elevator a service lift or Passanger
        public bool IsService { get; set; }

        // has elevator a faulty AC unit
        public bool HasFaultyACUnit { get; set; }

        // Elevator is very frequently used highlighting number of trips 
        public bool IsBusy { get; set; }
    }
}
