using System;
using System.Diagnostics;
using sChakert.Chessboard;
using sChakert.Magic;
using sChakert.MoveGeneration;

namespace sChakert
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            MagicGenerator.Init();
            var pawnMoves = AttackBitboard.BlackPawnEnPassantMove(67108864, 524288, 19);
            var board = Utilities.ToChessBoard(pawnMoves);
            Console.WriteLine("The board is:\n" + board);
            
            
#if DEBUG
            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
#endif
        }
    }
}