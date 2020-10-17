using System;
using System.Collections.Generic;

namespace Model
{
    public class Locality
    {
        public string Name;
        public Coordinates Coordinates;
        public TimeZone TimeZone;
        public string[] Landmarks;
    }

    public class Localities
    {
        private static readonly Locality Sydney = new Locality() {
            Name = "Sydney",
            Coordinates = new Coordinates(-33.8688, 151.2093),
            TimeZone = new TimeZone("(UTC+10:00) GMT+10:00", 10*60),
            Landmarks = new string[]{ "Sydney Harbour Bridge", "Sydney Opera House" }
        };

        private static readonly Locality NorthPole = new Locality() {
            Name = "North Pole",
            Coordinates = new Coordinates(90.0, 0.0),
            TimeZone = null,
            Landmarks = new string[]{ "Santa's Workshop"}
        };

        public static readonly IEnumerable<Locality> All = new Locality[]{ NorthPole, Sydney };
    }
}
