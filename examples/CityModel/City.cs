using System;
using System.Collections.Generic;

namespace CityModel
{
    public class City
    {
        public string Name;
        public Location Location;
        public TimeZone TimeZone;
        public string[] Landmarks;
    }

    public class Cities
    {
        private static readonly City Sydney = new City() {
            Name = "Sydney",
            Location = new Location(-33.8688, 151.2093),
            TimeZone = new TimeZone("(UTC+10:00) GMT+10:00", 10*60),
            Landmarks = new string[]{ "Sydney Harbour Bridge", "Sydney Opera House" }
        };

        private static readonly City NorthPole = new City() {
            Name = "North Pole",
            Location = new Location(90.0, 0.0),
            TimeZone = null,
            Landmarks = new string[]{ "Santa's Workshop"}
        };

        public static readonly IEnumerable<City> AllCities = new City[]{ NorthPole, Sydney };
    }
}
