using System;
using sChakert.Magic;

namespace sChakert.MoveGeneration
{
    public class AttackBitboard
    {
        /*
        These are post-shift masks.
        This means that we first apply the bitshift,
        and then remove unwanted wraps that could occur
        in certain circumstances.
        */

        UInt64 northOne(UInt64 bitboard)
        {
            return bitboard << 8;
        }

        UInt64 southOne(UInt64 bitboard)
        {
            return bitboard >> 8;
        }

        UInt64 eastOne(UInt64 bitboard)
        {
            return (bitboard << 1) & Utilities.clearFile[Utilities.FILE_A];
        }

        UInt64 southEastOne(UInt64 bitboard)
        {
            return (bitboard >> 7) & Utilities.clearFile[Utilities.FILE_A];
        }

        UInt64 northEastOne(UInt64 bitboard)
        {
            return (bitboard << 9) & Utilities.clearFile[Utilities.FILE_A];
        }

        UInt64 westOne(UInt64 bitboard)
        {
            return (bitboard >> 1) & Utilities.clearFile[Utilities.FILE_H];
        }

        UInt64 northWestOne(UInt64 bitboard)
        {
            return (bitboard << 7) & Utilities.clearFile[Utilities.FILE_H];
        }

        UInt64 southWestOne(UInt64 bitboard)
        {
            return (bitboard >> 9) & Utilities.clearFile[Utilities.FILE_H];
        }

        /*
        Moves for all the pieces
        */

        /// <summary>
        /// Get the bitboard representing the knight moves.
        /// </summary>
        /// <param name="king_loc">Bitboard representing the position of the king.</param>
        /// <param name="friendly_pieces">Bitboard representing all of the friendly pieces.</param>
        /// <returns>Bitboard representing the moves of the king.</returns>
        public UInt64 getKingMoves(UInt64 king_loc, UInt64 friendly_pieces)
        {
            UInt64 N = northOne(king_loc);
            UInt64 NE = northEastOne(king_loc);
            UInt64 E = eastOne(king_loc);
            UInt64 SE = southEastOne(king_loc);
            UInt64 S = southOne(king_loc);
            UInt64 SW = southWestOne(king_loc);
            UInt64 W = westOne(king_loc);
            UInt64 NW = northWestOne(king_loc);
            UInt64 king_moves = N | NE | E | SE | S | SW | W | NW;
            /*
            Final AND makes sure we only move to squares that are empty or occupied by an enemy piece.
            */
            return king_moves & ~friendly_pieces;
        }

        /// <summary>
        /// Get the knight moves given a bitboard containing the knights and the friendly pieces.
        /// </summary>
        /// <param name="knight_loc">Bitboard representing the position of the knight</param>
        /// <param name="friendly_pieces">Bitboard representing the friendly pieces</param>
        /// <returns>Bitboard representing all the moves of the knight.</returns>
        public UInt64 getKnightMoves(UInt64 knight_loc, UInt64 friendly_pieces)
        {
            UInt64 NNW_clip = Utilities.clearFile[Utilities.FILE_H];
            UInt64 SSW_clip = Utilities.clearFile[Utilities.FILE_H];
            UInt64 NNE_clip = Utilities.clearFile[Utilities.FILE_A];
            UInt64 SSE_clip = Utilities.clearFile[Utilities.FILE_A];

            UInt64 NWW_clip = Utilities.clearFile[Utilities.FILE_H] & Utilities.clearFile[Utilities.FILE_G];
            UInt64 SWW_clip = Utilities.clearFile[Utilities.FILE_H] & Utilities.clearFile[Utilities.FILE_G];
            UInt64 NEE_clip = Utilities.clearFile[Utilities.FILE_A] & Utilities.clearFile[Utilities.FILE_B];
            UInt64 SEE_clip = Utilities.clearFile[Utilities.FILE_A] & Utilities.clearFile[Utilities.FILE_B];

            UInt64 NWW = (knight_loc & NWW_clip) << 6;
            UInt64 NEE = (knight_loc & NEE_clip) << 10;
            UInt64 NNW = (knight_loc & NNW_clip) << 15;
            UInt64 NNE = (knight_loc & NNE_clip) << 17;

            UInt64 SEE = (knight_loc & SEE_clip) >> 6;
            UInt64 SWW = (knight_loc & SWW_clip) >> 10;
            UInt64 SSE = (knight_loc & SSE_clip) >> 15;
            UInt64 SSW = (knight_loc & SSW_clip) >> 17;

            UInt64 knight_moves = NWW | NEE | NNW | NNE | SEE | SWW | SSW | SSE;
            return knight_moves & ~friendly_pieces;
        }

        /// <summary>
        /// Get the single push moves of all the white pawns.
        /// </summary>
        /// <param name="whitePawns">Bitboard representing the white pawns.</param>
        /// <param name="emptySquares">Bitboard representing the empty squares</param>
        /// <returns>Bitboard representing all the single push moves for the white pawns.</returns>
        public UInt64 whitePawnsSinglePushMoves(UInt64 whitePawns, UInt64 emptySquares)
        {
            return northOne(whitePawns) & emptySquares;
        }

        /// <summary>
        /// Get the single push moves of all the black pawns.
        /// </summary>
        /// <param name="blackPawns">Bitboard representing the white pawns.</param>
        /// <param name="emptySquares">Bitboard representing the empty squares.</param>
        /// <returns>Bitboard representing all the single push moves for the black pawns.</returns>
        public UInt64 blackPawnsSinglePushMoves(UInt64 blackPawns, UInt64 emptySquares)
        {
            return southOne(blackPawns) & emptySquares;
        }

        /// <summary>
        /// Get the double push moves of all the white pawns.
        /// </summary>
        /// <param name="whitePawns">Bitboard representing the white pawns.</param>
        /// <param name="emptySquares">Bitboard representing the empty squares.</param>
        /// <returns>Bitboard representing all the double push moves for the white pawns.</returns>
        public UInt64 whitePawnsDoublePushMoves(UInt64 whitePawns, UInt64 emptySquares)
        {
            // mask in which the bits on rank 4 are turned on.
            const UInt64 rank4 = 0x00000000FF000000;
            UInt64 singlePushs = whitePawnsSinglePushMoves(whitePawns, emptySquares);
            return northOne(singlePushs) & rank4 & emptySquares;
        }

        /// <summary>
        /// Get the double push moves of all the black pawns.
        /// </summary>
        /// <param name="blackPawns">Bitboard representing the black pawns.</param>
        /// <param name="emptySquares">Bitboard representing the empty squares.</param>
        /// <returns>Bitboard representing all the double push moves for the black pawns.</returns>
        public UInt64 blackPawnsDoublePushMoves(UInt64 blackPawns, UInt64 emptySquares)
        {
            // mask in which the bits on rank 5 are turned on.
            const UInt64 rank5 = 0x000000FF00000000;
            UInt64 singlePushs = blackPawnsSinglePushMoves(blackPawns, emptySquares);
            return southOne(singlePushs) & rank5 & emptySquares;
        }

        /// <summary>
        /// Get the bitboard representing only the white pawns that are able to single push move.
        /// </summary>
        /// <param name="whitePawns">Bitboard representing the white pawns.</param>
        /// <param name="emptySquares">Bitboard representing the empty squares.</param>
        /// <returns>Bitboard representing the white pawns that are able to single push move.</returns>
        public UInt64 whitePawnsAbleToPush(UInt64 whitePawns, UInt64 emptySquares)
        {
            return southOne(emptySquares) & whitePawns;
        }

        /// <summary>
        /// Get the bitboard representing only the white pawns that are able to double push move.
        /// </summary>
        /// <param name="whitePawns">Bitboard representing the white pawns.</param>
        /// <param name="emptySquares">Bitboard representing the empty squares.</param>
        /// <returns>Bitboard representing the white pawns that are able to double push move.</returns>
        public UInt64 whitePawnsAbleToDoublePush(UInt64 whitePawns, UInt64 emptySquares)
        {
            const UInt64 rank4 = 0x00000000FF000000;
            UInt64 emptyRank3 = southOne(emptySquares & rank4) & emptySquares;
            return whitePawnsAbleToPush(whitePawns, emptyRank3);
        }
    }
}