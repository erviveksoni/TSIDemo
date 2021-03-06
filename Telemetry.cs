using System;
using Newtonsoft.Json;

namespace Simulator
{
    public class Telemetry
    {
        [JsonProperty("deviceId")]
        public string DeviceId { get; set; }

        [JsonProperty("telemetryTimeStamp")]
        public DateTime TimeStamp { get; set; }

        [JsonProperty("temperature")]
        public double Temperature { get; set; }

        [JsonProperty("humidity")]
        public double Humidity { get; set; }

        [JsonProperty("load")]
        public double Load { get; set; }

        [JsonProperty("distance")]
        public double Distance { get; set; }

        [JsonProperty("vibration")]
        public double Vibration { get; set; }

        [JsonProperty("doorSafetyReading")]
        public double DoorSafetyReading { get; set; }

        [JsonProperty("jerks")]
        public int Jerks { get; set; }

        [JsonProperty("powerType")]
        public string PowerType { get; set; }

        [JsonProperty("floor")]
        public int Floor { get; set; }

        [JsonProperty("doorcycles")]
        public int NumberOfDoorCycles { get; set; }

        public override string ToString()
        {
            JsonSerializerSettings dateFormatSettings =
                new JsonSerializerSettings {DateFormatString = "yyyy-MM-ddTHH:mm:ss.FFFFFFFK"};

            return JsonConvert.SerializeObject(this, dateFormatSettings);
        }
    }
}