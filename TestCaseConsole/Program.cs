using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestCaseConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var testCase = new TestCase();
            testCase.ArrangeTestCaseEnviroment();
            testCase.SimulateMouseKeyboardEvents();
        }
    }
}
