using sChakert.Magic;

namespace sChakert.MoveGeneration
{
    public static class AttackBitboard
    {
        /*
        These are post-shift masks.
        This means that we first apply the bitshift,
        and then remove unwanted wraps that could occur
        in certain circumstances.
        */

        /// <summary>
        ///     mask in which the bits on rank 4 are turned on.
        /// </summary>
        private const ulong Rank4 = 0xFF000000;

        /// <summary>
        ///     mask in which the bits on rank 5 are turned on.
        /// </summary>
        private const ulong Rank5 = 0xFF00000000;

        /// <summary>
        ///     Get the bitboard representing only the white pawns that are able to double push move.
        /// </summary>
        /// <param name="blackPawns">Bitboard representing the black pawns.</param>
        /// <param name="emptySquares">Bitboard representing the empty squares.</param>
        /// <returns>Bitboard representing the black pawns that are able to double push move.</returns>
        public static ulong BlackPawnsAbleToDoublePush(ulong blackPawns, ulong emptySquares)
        {
            var emptyRank6 = SouthOne(emptySquares & Rank5) & emptySquares;
            return BlackPawnsAbleToPush(blackPawns, emptyRank6);
        }

        /// <summary>
        ///     Get the bitboard representing only the black pawns that are able to single push move.
        /// </summary>
        /// <param name="blackPawns">Bitboard representing the black pawns.</param>
        /// <param name="emptySquares">Bitboard representing the empty squares.</param>
        /// <returns></returns>
        public static ulong BlackPawnsAbleToPush(ulong blackPawns, ulong emptySquares)
        {
            return NorthOne(emptySquares) & blackPawns;
        }

        /// <summary>
        ///     Get the double push moves of all the black pawns.
        /// </summary>
        /// <param name="blackPawns">Bitboard representing the black pawns.</param>
        /// <param name="emptySquares">Bitboard representing the empty squares.</param>
        /// <returns>Bitboard representing all the double push moves for the black pawns.</returns>
        public static ulong BlackPawnsDoublePushMoves(ulong blackPawns, ulong emptySquares)
        {
            var singlePushs = BlackPawnsSinglePushMoves(blackPawns, emptySquares);
            return SouthOne(singlePushs) & Rank5 & emptySquares;
        }

        /// <summary>
        ///     Get the single push moves of all the black pawns.
        /// </summary>
        /// <param name="blackPawns">Bitboard representing the white pawns.</param>
        /// <param name="emptySquares">Bitboard representing the empty squares.</param>
        /// <returns>Bitboard representing all the single push moves for the black pawns.</returns>
        public static ulong BlackPawnsSinglePushMoves(ulong blackPawns, ulong emptySquares)
        {
            return SouthOne(blackPawns) & emptySquares;
        }

        private static ulong EastOne(ulong bitboard)
        {
            return (bitboard << 1) & Utilities.ClearFile[Utilities.FILE_A];
        }

        /*
        Moves for all the pieces.
        A good site for easy calculating with bitboards is the following:
        http://cinnamonchess.altervista.org/bitboard_calculator/Calc.html
        */

        /// <summary>
        ///     Get the bitboard representing the knight moves.
        /// </summary>
        /// <param name="knightPos">Bitboard representing the position of the king.</param>
        /// <param name="friendlyPieces">Bitboard representing all of the friendly pieces.</param>
        /// <returns>Bitboard representing the moves of the king.</returns>
        public static ulong GetKingMoves(ulong knightPos, ulong friendlyPieces)
        {
            var kingN = NorthOne(knightPos);
            var kingNE = NorthEastOne(knightPos);
            var kingE = EastOne(knightPos);
            var kingSE = SouthEastOne(knightPos);
            var kingS = SouthOne(knightPos);
            var kingSW = SouthWestOne(knightPos);
            var kingW = WestOne(knightPos);
            var kingNW = NorthWestOne(knightPos);
            var kingMoves = kingN | kingNE | kingE | kingSE | kingS | kingSW | kingW | kingNW;
            /*
            Final AND makes sure we only move to squares that are empty or occupied by an enemy piece.
            */
            return kingMoves & ~friendlyPieces;
        }

        /// <summary>
        ///     Get the knight moves given a bitboard containing the knights and the friendly pieces.
        /// </summary>
        /// <param name="knightPos">Bitboard representing the position of the knight</param>
        /// <param name="friendlyPieces">Bitboard representing the friendly pieces</param>
        /// <returns>Bitboard representing all the moves of the knight.</returns>
        public static ulong GetKnightMoves(ulong knightPos, ulong friendlyPieces)
        {
            var NNWclip = Utilities.ClearFile[Utilities.FILE_H];
            var SSWclip = Utilities.ClearFile[Utilities.FILE_H];
            var NNEclip = Utilities.ClearFile[Utilities.FILE_A];
            var SSEclip = Utilities.ClearFile[Utilities.FILE_A];

            var NWWclip = Utilities.ClearFile[Utilities.FILE_H] & Utilities.ClearFile[Utilities.FILE_G];
            var SWWclip = Utilities.ClearFile[Utilities.FILE_H] & Utilities.ClearFile[Utilities.FILE_G];
            var NEEclip = Utilities.ClearFile[Utilities.FILE_A] & Utilities.ClearFile[Utilities.FILE_B];
            var SEEclip = Utilities.ClearFile[Utilities.FILE_A] & Utilities.ClearFile[Utilities.FILE_B];

            var knightNWW = (knightPos & NWWclip) << 6;
            var knightNEE = (knightPos & NEEclip) << 10;
            var knightNNW = (knightPos & NNWclip) << 15;
            var knightNNE = (knightPos & NNEclip) << 17;

            var knightSEE = (knightPos & SEEclip) >> 6;
            var knightSWW = (knightPos & SWWclip) >> 10;
            var knightSSE = (knightPos & SSEclip) >> 15;
            var knightSSW = (knightPos & SSWclip) >> 17;

            var knightMoves = knightNWW | knightNEE | knightNNW | knightNNE | knightSEE | knightSWW | knightSSW |
                              knightSSE;
            return knightMoves & ~friendlyPieces;
        }

        /// <summary>
        ///     Get the moveset of a single rook by performing a special lookup in a move database.
        ///     This lookup is based on pre-caluculated "magic" values.
        /// </summary>
        /// <param name="piecePos">The bitboard representing the piece (either rook or bishop)</param>
        /// <param name="rookPos">The bitboard representing the rook</param>
        /// <param name="allPieces">The bitboard </param>
        /// <param name="friendlyPieces">The bitboard containing the friendly pieces</param>
        /// <param name="isRook">Indicates if the piece is a rook or not</param>
        /// <returns></returns>
        public static ulong GetSlidingMoves(ulong piecePos, ulong allPieces, ulong friendlyPieces, bool isRook = true)
        {
            var boardIndex = Utilities.GetActiveBitIndices(piecePos)[0];
            var attackSet = isRook
                ? MagicGenerator.GetRookAttackSet(boardIndex)
                : MagicGenerator.GetBishopAttackSet(boardIndex);
            var attackVariation = attackSet & allPieces;
            // Perform the magic lookup
            var bitCountAttackSet = Utilities.GetActiveBitIndices(attackSet);
            var magicNumber = isRook
                ? MagicGenerator.RookMagicNumbers[boardIndex]
                : MagicGenerator.BishopMagicNumbers[boardIndex];
            var magicIndex = (attackVariation*magicNumber) >> (64 - bitCountAttackSet.Count);
            // Return the moveset AND-ed with the negation of the friendly pieces.
            return isRook
                ? MagicGenerator.RookLookupTable[boardIndex, magicIndex] & ~friendlyPieces
                : MagicGenerator.BishopLookupTable[boardIndex, magicIndex] & ~friendlyPieces;
        }

        private static ulong NorthEastOne(ulong bitboard)
        {
            return (bitboard << 9) & Utilities.ClearFile[Utilities.FILE_A];
        }

        private static ulong NorthOne(ulong bitboard)
        {
            return bitboard << 8;
        }

        private static ulong NorthWestOne(ulong bitboard)
        {
            return (bitboard << 7) & Utilities.ClearFile[Utilities.FILE_H];
        }

        private static ulong SouthEastOne(ulong bitboard)
        {
            return (bitboard >> 7) & Utilities.ClearFile[Utilities.FILE_A];
        }

        private static ulong SouthOne(ulong bitboard)
        {
            return bitboard >> 8;
        }

        private static ulong SouthWestOne(ulong bitboard)
        {
            return (bitboard >> 9) & Utilities.ClearFile[Utilities.FILE_H];
        }

        private static ulong WestOne(ulong bitboard)
        {
            return (bitboard >> 1) & Utilities.ClearFile[Utilities.FILE_H];
        }

        /// <summary>
        ///     Get the bitboard representing only the white pawns that are able to double push move.
        /// </summary>
        /// <param name="whitePawns">Bitboard representing the white pawns.</param>
        /// <param name="emptySquares">Bitboard representing the empty squares.</param>
        /// <returns>Bitboard representing the white pawns that are able to double push move.</returns>
        public static ulong WhitePawnsAbleToDoublePush(ulong whitePawns, ulong emptySquares)
        {
            var emptyRank3 = SouthOne(emptySquares & Rank4) & emptySquares;
            return WhitePawnsAbleToPush(whitePawns, emptyRank3);
        }

        /// <summary>
        ///     Get the bitboard representing only the white pawns that are able to single push move.
        /// </summary>
        /// <param name="whitePawns">Bitboard representing the white pawns.</param>
        /// <param name="emptySquares">Bitboard representing the empty squares.</param>
        /// <returns>Bitboard representing the white pawns that are able to single push move.</returns>
        public static ulong WhitePawnsAbleToPush(ulong whitePawns, ulong emptySquares)
        {
            return SouthOne(emptySquares) & whitePawns;
        }

        /// <summary>
        ///     Get the double push moves of all the white pawns.
        /// </summary>
        /// <param name="whitePawns">Bitboard representing the white pawns.</param>
        /// <param name="emptySquares">Bitboard representing the empty squares.</param>
        /// <returns>Bitboard representing all the double push moves for the white pawns.</returns>
        public static ulong WhitePawnsDoublePushMoves(ulong whitePawns, ulong emptySquares)
        {
            var singlePushs = WhitePawnsSinglePushMoves(whitePawns, emptySquares);
            return NorthOne(singlePushs) & Rank4 & emptySquares;
        }

        /// <summary>
        ///     Get the single push moves of all the white pawns.
        /// </summary>
        /// <param name="whitePawns">Bitboard representing the white pawns.</param>
        /// <param name="emptySquares">Bitboard representing the empty squares</param>
        /// <returns>Bitboard representing all the single push moves for the white pawns.</returns>
        public static ulong WhitePawnsSinglePushMoves(ulong whitePawns, ulong emptySquares)
        {
            return NorthOne(whitePawns) & emptySquares;
        }
    }
}