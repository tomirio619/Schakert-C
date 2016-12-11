using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sChakert.Magic;

namespace sChakert
{
    class Program
    {
        static void Main(string[] args)
        {
            Utilities.PrintHexConstants();
            MagicGenerator.Init();
#if DEBUG
            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
#endif
        }
    }
}