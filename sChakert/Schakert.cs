using System;
using System.Diagnostics;
using sChakert.Chessboard;
using sChakert.Magic;
using sChakert.MoveGeneration;

namespace sChakert
{
    internal class Program
    {
        private static void PrintLogo()
        {
            //@formatter:off
            const string logo =
                @"
       _________ .__            __                  __   
  _____\_   ___ \|  |__ _____  |  | __ ____________/  |_ 
 /  ___/    \  \/|  |  \\__  \ |  |/ // __ \_  __ \   __\
 \___ \\     \___|   Y  \/ __ \|    <\  ___/|  | \/|  |  
/____  >\______  /___|  (____  /__|_ \\___  >__|   |__|  
     \/        \/     \/     \/     \/    \/             
            ";
            //@formatter:on
            Console.WriteLine(logo);
        }

        private static void Main(string[] args)
        {
            PrintLogo();
            Console.Title = "Schakert";
            MagicGenerator.Init();
            var pawnMoves = AttackBitboard.BlackPawnEnPassantMove(67108864, 524288, 19);
            var board = Utilities.ToChessBoard(pawnMoves);
            Console.WriteLine("The board is:\n" + board);


#if DEBUG
            // https://stackoverflow.com/questions/16956089/how-to-change-the-command-prompt-for-a-c-sharp-console-application
            Console.WriteLine("Press enter to close...");
            Console.Out.Flush();
            Console.ReadLine();
#endif
        }
    }
}