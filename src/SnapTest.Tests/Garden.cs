using System.Collections.Generic;

namespace SnapTest.Tests
{
    /// <summary>
    /// The Address class is a simple POCO class for representing interesting test data.
    /// </summary>
    public class Address
    {
        public string Street;
        public string Postcode;
    }

    /// <summary>
    /// The Garden class is a simple POCO class for representing interesting test data.
    /// </summary>
    public class Garden
    {
        public string Name;
        public int Rating;
        public Address Address;
        public IEnumerable<string> Trees;

        /// <summary>
        /// A sample Garden object that can be used as test data.
        /// </summary>
        /// <returns></returns>
        public readonly static Garden Flagstaff = new Garden() {
            Name = "Flagstaff",
            Rating = 4,
            Address = new Address() { Street = "William", Postcode = "3000" },
            Trees = new List<string>(new string[] { "Elm", "Eucalyptus", "Morton Bay Fig" })
        };
    }
}
