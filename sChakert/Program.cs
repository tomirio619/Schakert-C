using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sChakert.Magic;
using sChakert.MoveGeneration;

namespace sChakert
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Utilities.PrintHexConstants();
            MagicGenerator.Init();
            var rookMoves = AttackBitboard.GetSlidingMoves(2048, 134235144, 0, isRook:true);
            var board = Utilities.ToChessBoard(rookMoves);
            Console.WriteLine("The board is:\n" + board);
#if DEBUG
            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
#endif
        }
    }
}