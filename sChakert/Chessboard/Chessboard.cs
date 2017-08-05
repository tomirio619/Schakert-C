using System;

namespace sChakert.Chessboard
{
    public class Chessboard
    {
        /// <summary>
        ///     Array of bitboards
        /// </summary>
        public ulong[] Bitboards;

        /// <summary>
        ///     Index of bitboard representing the white pawns
        /// </summary>
        public const int WhitePawns = 0;

        /// <summary>
        ///     Index of bitboard representing the white knights
        /// </summary>
        public const int WhiteKnights = 1;

        /// <summary>
        ///      Index of bitboard representing the white bishops
        /// </summary>
        public const int WhiteBishops = 2;

        /// <summary>
        ///     Index of bitboard representing the white rooks
        /// </summary>
        public const int WhiteRooks = 3;

        /// <summary>
        ///      Index of bitboard representing the white queens
        /// </summary>
        public const int WhiteQueens = 4;

        /// <summary>
        ///      Index of bitboard representing the white king
        /// </summary>
        public const int WhiteKing = 5;

        /// <summary>
        ///      Index of bitboard representing the black pawns
        /// </summary>
        public const int BlackPawns = 6;

        /// <summary>
        ///      Index of bitboard representing the black knights
        /// </summary>
        public const int BlackKnights = 7;

        /// <summary>
        ///     Index of bitboard representing the black bishops
        /// </summary>
        public const int BlackBishops = 8;

        /// <summary>
        ///     Index of bitboard representing the black rooks
        /// </summary>
        public const int BlackRooks = 9;

        /// <summary>
        ///     Index of bitboard representing the black queens
        /// </summary>
        public const int BlackQueens = 10;

        /// <summary>
        ///     Index of bitboard representing the black king
        /// </summary>
        public const int BlackKing = 11;

        /// <summary>
        ///     Index of bitboard representing all white pieces
        /// </summary>
        public const int WhitePieces = 12;

        /// <summary>
        ///     Index of bitboard representing all black pieces
        /// </summary>
        public const int BlackPieces = 13;

        /// <summary>
        ///     Index of bitboard representing the empty squares
        /// </summary>
        public const int EmptySquares = 14;


        /// <summary>
        /// Constructor
        /// </summary>
        public Chessboard()
        {
            // Default board
            Bitboards = new ulong[]
            {
                0xFF00, // White pawns
                0x81, // White rooks
                0x42, // White knights
                0x24, // White bishops
                0x8, // White queens
                0x10, // White king
                0xFF000000000000, // Black pawns
                0x8100000000000000, // Black rooks
                0x4200000000000000, // Black knights
                0x2400000000000000, // Black bishops
                0x800000000000000, // Black queens
                0x1000000000000000, // Black king
                0xffff, // White Pieces
                0xffff000000000000, // Black Pieces
                0xffffffff0000
            };
        }

        /// <summary>
        ///     Create a chessboard from a given FEN string.
        /// </summary>
        /// <param name="fen">The FEN string</param>
        public Chessboard(string fen)
        {
            // TODO parse a FEN string and set the chessboard
        }

        /// <summary>
        ///     Get the piece and the type of a piece on a given board index (if there is one)
        /// </summary>
        /// <param name="boardIndex">The board index</param>
        /// <returns>A tuple containing: a bool indicating if the position was occupied,
        /// the color and the type of the chess piece one the specified board index 
        /// (both are none when the position was not occupied).</returns>
        public Tuple<bool, Color, Type> GetPieceTypeAndColour(int boardIndex)
        {
            var bitboardPos = 1UL << boardIndex;
            for (var i = 0; i < 12; i++)
                if ((Bitboards[i] & bitboardPos) > 0)
                {
                    // if index of bitboard is greater than 6, this was a black piece.
                    // Otherwise, this is a white piece
                    var color = i > 6 ? Color.Black : Color.White;
                    // Bitboard index % 6 is the same as the int value of the enum of piece types.
                    return new Tuple<bool, Color, Type>(true, color, (Type) (i%6));
                }
            // empty
            return new Tuple<bool, Color, Type>(false, Color.None, Type.None);
        }

        /// <summary>
        /// Get the color of a piece for a given board index
        /// </summary>
        /// <param name="boardIndex"></param>
        /// <returns></returns>
        public Color GetPieceColor(int boardIndex)
        {
            if ((Bitboards[WhitePieces] & (1UL << boardIndex)) > 0)
                return Color.White;
            return (Bitboards[BlackPieces] & (1UL << boardIndex)) > 0 ? Color.Black : Color.None;
        }
    }
}