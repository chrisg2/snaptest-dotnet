using System;

namespace Model
{
    public class TimeZone
    {
        public string DisplayName;
        public int BaseUTCOffsetMinutes;
        public DateTime CurrentTime => DateTime.UtcNow.AddMinutes(BaseUTCOffsetMinutes);

        public TimeZone(string displayName, int baseUTCOffsetMinutes) { DisplayName = displayName; BaseUTCOffsetMinutes = baseUTCOffsetMinutes; }
    }
}
