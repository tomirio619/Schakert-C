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
            var rookMoves = AttackBitboard.GetSlidingMoves(2048, 134235144, 0, true);
            var board = Utilities.ToChessBoard(rookMoves);
            Console.WriteLine("The board is:\n" + board);
            StateManager.SaveCurrentState();
            StateManager.BlackKingHasMoved = 1;
            StateManager.WhiteKingHasMoved = 1;
            StateManager.RestorePreviousState();
            Debug.WriteLine(StateManager.BlackKingHasMoved);
            Debug.WriteLine(StateManager.WhiteKingHasMoved);

#if DEBUG
            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
#endif
        }
    }
}