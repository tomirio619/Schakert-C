using System;

namespace sChakert.Chessboard
{
    internal class Chessboard
    {
        /// <summary>
        ///     Array containing all of the bitboards
        /// </summary>
        private readonly ulong[] Bitboards;

        /// <summary>
        ///     Bitboards of all the pieces
        /// </summary>
        private readonly ulong WhitePawns;

        /// <summary>
        ///     Bitboards of all the pieces
        /// </summary>
        private readonly ulong WhiteRooks;

        /// <summary>
        ///     Bitboards of all the pieces
        /// </summary>
        private readonly ulong WhiteKnights;

        /// <summary>
        ///     Bitboards of all the pieces
        /// </summary>
        private readonly ulong WhiteBishops;

        /// <summary>
        ///     Bitboards of all the pieces
        /// </summary>
        private readonly ulong WhiteQueens;

        /// <summary>
        ///     Bitboards of all the pieces
        /// </summary>
        private readonly ulong WhiteKing;

        /// <summary>
        ///     Bitboards of all the pieces
        /// </summary>
        private readonly ulong BlackPawns;

        /// <summary>
        ///     Bitboards of all the pieces
        /// </summary>
        private readonly ulong BlackRooks;

        /// <summary>
        ///     Bitboards of all the pieces
        /// </summary>
        private readonly ulong BlackKnights;

        /// <summary>
        ///     Bitboards of all the pieces
        /// </summary>
        private readonly ulong BlackBishops;

        /// <summary>
        ///     Bitboards of all the pieces
        /// </summary>
        private readonly ulong BlackQueens;

        /// <summary>
        ///     Bitboards of all the pieces
        /// </summary>
        private readonly ulong BlackKing;


        public Chessboard()
        {
            // White pieces
            WhitePawns = 0xFF00;
            WhiteRooks = 0x81;
            WhiteKnights = 0x42;
            WhiteBishops = 0x24;
            WhiteKing = 0x10;
            WhiteQueens = 0x8;

            // Black pieces
            BlackPawns = 0xFF000000000000;
            BlackRooks = 0x8100000000000000;
            BlackKnights = 0x4200000000000000;
            BlackBishops = 0x2400000000000000;
            BlackKing = 0x1000000000000000;
            BlackQueens = 0x800000000000000;

            // Add all the bitboards to our bitboard array
            Bitboards = new ulong[12]
            {
                WhitePawns,
                WhiteRooks,
                WhiteKnights,
                WhiteBishops,
                WhiteQueens,
                WhiteKing,
                BlackPawns,
                BlackRooks,
                BlackKnights,
                BlackBishops,
                BlackQueens,
                BlackKing
            };
        }

        /// <summary>
        ///     Create a chessboard from a given FEN string.
        /// </summary>
        /// <param name="FEN">The FEN string</param>
        public Chessboard(string FEN)
        {
            // TODO parse a FEN string and set the chessboard
        }

        /// <summary>
        ///     Get the piece and the type of a piece on a given board index (if there is one)
        /// </summary>
        /// <param name="boardIndex">The board index</param>
        /// <returns>A tuple containing the color and the type of the chess piece one the specified board index</returns>
        public Tuple<Color, Type> GetPieceTypeAndColour(int boardIndex)
        {
            var bitboardPos = 1UL << boardIndex;
            for (var i = 0; i != 12; ++i)
                if ((Bitboards[i] & bitboardPos) > 0)
                {
                    var color = i > 6 ? Color.Black : Color.White;
                    return new Tuple<Color, Type>(color, (Type) i);
                }
            // empty
            return new Tuple<Color, Type>(Color.None, Type.None);
        }
    }
}