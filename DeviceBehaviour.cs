namespace Simulator
{
    public class DeviceBehaviour
    {
        /// <summary>
        /// Is elevator a service lift or Passanger
        /// </summary>
        public bool IsService { get; set; }

        /// <summary>
        /// has elevator a faulty AC unit
        /// </summary>
        public bool HasFaultyACUnit { get; set; }

        /// <summary>
        /// Elevator is very frequently used highlighting number of trips 
        /// </summary>
        public bool IsBusy { get; set; }

        /// <summary>
        /// Vibration value will increase
        /// beyond certain value
        /// Random Jerky behaviour
        /// </summary>
        public bool IsJerky { get; set; }

        /// <summary>
        /// Elevator is running on AUX power
        /// </summary>
        public bool IsOnAuxPower { get; set; }

        /// <summary>
        /// Is Elevator blocked on a floor
        /// </summary>
        public bool IsBlocked { get; set; }

        /// <summary>
        /// Is Door Safety Circuit Malfunctioning
        /// </summary>
        public bool IsDSCMalfunctioning { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum PowerType
    {
        Main,

        AUX
    }
}
