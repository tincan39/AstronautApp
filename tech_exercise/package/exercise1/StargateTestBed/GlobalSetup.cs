using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StargateTestBed
{
    [SetUpFixture]
    public class GlobalSetup
    {
        [OneTimeSetUp]
        public static void RunBeforeAnyTests()
        {
            Console.WriteLine("Hi");
            // ...
        }

        [OneTimeTearDown]
        public static void RunAfterAnyTests()
        {
            // ...
        }
    }
}
