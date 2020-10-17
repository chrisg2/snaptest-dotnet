using System;

namespace SnapTest.Examples
{
    public class Program
    {
        public static int Main()
        {
            Console.WriteLine("This is SnapTest.Examples");

            var result = NakedSnapshotExample.UseNakedSnapshot();

            return result ? 0 : 1;
        }
    }
}
