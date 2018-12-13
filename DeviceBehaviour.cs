namespace Simulator
{
    public class DeviceBehaviour
    {
        /// <summary>
        /// Is elevator a service lift or Passanger
        /// Has impact on load capacity
        /// </summary>
        public bool IsService { get; set; }

        /// <summary>
        /// has elevator a faulty AC unit
        /// Temperature and humidity vary outside normal range
        /// </summary>
        public bool HasFaultyACUnit { get; set; }

        /// <summary>
        /// Elevator is very frequently used highlighting number of trips
        /// Increases the number of messages reported to the cloud indicating
        /// increase in distance travelled
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
        /// Also induces jerky behaviour
        /// </summary>
        public bool IsOnAuxPower { get; set; }

        /// <summary>
        /// Is Elevator blocked on a floor
        /// Sets distance, load and vibration to 0
        /// </summary>
        public bool IsBlocked { get; set; }

        /// <summary>
        /// Is Door Safety Circuit Malfunctioning
        /// Sets the DoorSafetyReading value
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
