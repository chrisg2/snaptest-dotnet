using System;

namespace SnapTest.Examples
{
    public class Examples
    {
        public static int Main()
        {
            Console.WriteLine("This is SnapTest.Examples");

            var result = SantaTests.Santa_lives_at_the_NorthPole();

            return result ? 0 : 1;
        }
    }
}
