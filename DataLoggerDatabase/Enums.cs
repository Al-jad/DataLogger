using System.Runtime.Serialization;

namespace DataLoggerDatabase
{
    public class Enums
    {
        public enum StationType
        {
            Pipes,
            Tank
        }

        public enum ByDuration
        {
            [EnumMember(Value = "Minute")]
            Minute,
            
            [EnumMember(Value = "Hour")]
            Hour,
            
            [EnumMember(Value = "Day")]
            Day,
        }
    }
}
