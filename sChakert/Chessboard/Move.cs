﻿using System;
using System.Collections.Generic;
using sChakert.MoveGeneration;

namespace sChakert.Chessboard
{
    public static class Move
    {
/*
Assuming little endiannes, A move is encoded as follows (unsigned 32 bit integer):
0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 | 11 | 12 | 13 | 14 | 15 | 16 | 17 | 18 | 19 | 20 | 21 | 22 | 23 | 24 | 25 | 26 | 27 | 28 | 29 | 30 | 31
<----------------->
Kind of move
                   <----------------------->
                   From square
                                            <---------------------------->
                                            To square
                                                                          <----------------------->
                                                                          Type of captured piece
                                                                                                   <---------------------->
                                                                                                   Promotion? Type to promote into                                                                                             
                         */


        /// <summary>
        /// Encode a move given the current board index on which the piece stands, the new board index after the move was
        /// applied and the type of the piece
        /// </summary>
        /// <param name="fromBoardIndex">The original position of the piece</param>
        /// <param name="toBoardIndex">The new position of the piece</param>
        /// <param name="pieceType">The type of the piece</param>
        /// <param name="chessBoard">The chessboard</param>
        /// <returns></returns>
        public static List<int> GetMoves(Chessboard chessBoard)
        {
            var moves = new List<int>();
            var emptySquares = chessBoard.Bitboards[Chessboard.EmptySquares];
            var blackPieces = chessBoard.Bitboards[Chessboard.BlackPieces];
            var whitePieces = chessBoard.Bitboards[Chessboard.WhitePieces];
            // Loop through all the bitboards of individual pieces
            for (var i = 0; i < 12; i++)
            {
                var curBitboard = chessBoard.Bitboards[i];
                var activeBitsIndices = Utilities.GetActiveBitIndices(curBitboard);

                switch (i)
                {
                    case 0:
                    case 6:
                        moves.AddRange(GetPawnMoves(activeBitsIndices, chessBoard));
                        break;
//                    default:
//                        throw new IndexOutOfRangeException();

                }
            }


            // Start encoding the move(s)

            //TODO castling
            return new List<int>();
        }

        /// <summary>
        /// Given a list of integers that indicate on which squares pawns are present, encode the corresponding moves
        /// </summary>
        /// <param name="activeBitsIndices">List indicating the squares that are occupied by pawns of a certain color</param>
        /// <param name="chessBoard">The chessboard.</param>
        /// <returns>A a list containing all of the encoded moves that are possible.</returns>
        private static List<int> GetPawnMoves(List<int> activeBitsIndices, Chessboard chessBoard)
        {
            var moves = new List<int>();
            foreach (var fromBoardIndex in activeBitsIndices)
            {
                var pieceColor = chessBoard.GetPieceColor(fromBoardIndex);
                var emptySquares = chessBoard.Bitboards[Chessboard.EmptySquares];
                var blackPieces = chessBoard.Bitboards[Chessboard.BlackPieces];
                var whitePieces = chessBoard.Bitboards[Chessboard.WhitePieces];
                var moveBitboard = 0UL;
                var pawnPos = 1UL << fromBoardIndex;
                // Determine the moves for a single pawn
                if (pieceColor == Color.Black)
                {
                    moveBitboard = AttackBitboard.BlackPawnSinglePushMoves(pawnPos, emptySquares);
                    moveBitboard |= AttackBitboard.BlackPawnDoublePushMoves(pawnPos, emptySquares);
                    moveBitboard |= AttackBitboard.BlackPawnCaptureMoves(pawnPos, emptySquares, blackPieces);
                    moveBitboard |=
                        AttackBitboard.BlackPawnEnPassantMove(pawnPos, emptySquares, StateManager.EnPassantPos);
                }
                else
                {
                    moveBitboard = AttackBitboard.WhitePawnSinglePushMoves(pawnPos, emptySquares);
                    moveBitboard |= AttackBitboard.WhitePawnDoublePushMoves(pawnPos, emptySquares);
                    moveBitboard |= AttackBitboard.WhitePawnCaptureMoves(pawnPos, emptySquares, whitePieces);
                    moveBitboard |=
                        AttackBitboard.WhitePawnEnPassantMove(pawnPos, emptySquares, StateManager.EnPassantPos);
                }
                var pawnToBoardIndices = Utilities.GetActiveBitIndices(moveBitboard);
                // Loop through every pawn dest square, encode and store the move
                foreach (var toBoardIndex in pawnToBoardIndices)
                {
                    
                    var newSquareInfo = chessBoard.GetPieceTypeAndColour(toBoardIndex);
                    var newSquareOccupied = false;
                    var capturedPieceType = Type.None;
                    if (newSquareInfo > -1)
                    {
                        newSquareOccupied = Convert.ToBoolean(newSquareInfo & 1);
                        capturedPieceType = (Type) ((newSquareInfo & 0x1c) >> 3);
                    }
                    moves.AddRange(EncodePawnMove(fromBoardIndex, toBoardIndex, newSquareOccupied, pieceColor,
                        capturedPieceType));
                }
            }
            return moves;
        }

        /// <summary>
        /// Encode the move of a pawn.
        /// </summary>
        /// <param name="fromBoardIndex">The current board index of the pawn</param>
        /// <param name="toBoardIndex">The board index to which the pawn will move</param>
        /// <param name="newSquareOccupied">Indicated whether the new square is occupied</param>
        /// <param name="pieceColor">The color of the pawn.</param>
        /// <param name="capturedPieceType">The piece that is captured during the move, if any</param>
        /// <returns></returns>
        private static IEnumerable<int> EncodePawnMove(int fromBoardIndex, int toBoardIndex, bool newSquareOccupied,
            Color pieceColor, Type capturedPieceType)
        {
            var kindOfMove = 0;
            var newEnPassantSquare = 0;
            var boardIndexDiff = Math.Abs(toBoardIndex - fromBoardIndex);
            var diagonalMove = boardIndexDiff != 8;
            var pawnMoves = new List<int>();
            if (newSquareOccupied)
            {
                // The new square is occupied, this can either be a normal capture move or a promotion capture move
                kindOfMove = ((toBoardIndex < 8) || (toBoardIndex > 55)) ? 15 : 4;
            }
            // Square is not occupied
            else if (diagonalMove)
            {
                // enPassant Capture
                kindOfMove = 5;
            }
            else if (boardIndexDiff == 16)
            {
                // Double pawn push
                kindOfMove = 1;
                newEnPassantSquare = pieceColor == Color.White
                    ? fromBoardIndex + 8
                    : toBoardIndex + 8;
            }
            if (kindOfMove >= 8)
            {
                // Promotion move
                pawnMoves.Add(EncodeMove(kindOfMove, fromBoardIndex, toBoardIndex, capturedPieceType, Type.Knight));
                pawnMoves.Add(EncodeMove(kindOfMove, fromBoardIndex, toBoardIndex, capturedPieceType, Type.Bishop));
                pawnMoves.Add(EncodeMove(kindOfMove, fromBoardIndex, toBoardIndex, capturedPieceType, Type.Rook));
                pawnMoves.Add(EncodeMove(kindOfMove, fromBoardIndex, toBoardIndex, capturedPieceType, Type.Queen));
                return pawnMoves;
            }
            // Only one move
            pawnMoves.Add(EncodeMove(kindOfMove, fromBoardIndex, toBoardIndex, capturedPieceType));
            return pawnMoves;
        }

        /// <summary>
        /// Encode the move of a sliding pieces (queen, rook or bishop) or a knight
        /// </summary>
        /// <param name="fromBoardIndex">The departing square.</param>
        /// <param name="toBoardIndex">The destination square.</param>
        /// <param name="newSquareOccupied">Indicaties whether the destination square is occupied or not.</param>
        /// <param name="pieceColor">Color of the piece being moved.</param>
        /// <param name="capturedPieceType">Type of the captured piece.</param>
        /// <returns></returns>
        private static int EncodeMove(int fromBoardIndex, int toBoardIndex, bool newSquareOccupied,
            Color pieceColor, Type capturedPieceType)
        {
            var kindOfMove = newSquareOccupied ? 4 : 0;
            return EncodeMove(kindOfMove, fromBoardIndex, toBoardIndex, capturedPieceType);
        }

        private static int EncodeKingMove(int fromBoardIndex, int toBoardIndex, bool newSquareOccupied,
            Color pieceColor, Type capturedPieceType)
        {
            var kindOfMove = 0;
            var boardIndexDiff = Math.Abs(toBoardIndex - fromBoardIndex);
            if (newSquareOccupied)
            {
                kindOfMove = 4;
            }
            else if (boardIndexDiff == 2)
            {
                // castling move, determine if it is queen or king side
            }
            return EncodeMove(kindOfMove, fromBoardIndex, toBoardIndex, capturedPieceType);
        }

        /// <summary>
        /// Encode a move based on the specified paramaters
        /// </summary>
        /// <param name="kindOfMove"></param>
        /// <param name="fromSquare"></param>
        /// <param name="toSquare"></param>
        /// <param name="typeOfCapturedPiece"></param>
        /// <param name="typeToPromoteInto"></param>
        /// <returns></returns>
        public static int EncodeMove(int kindOfMove, int fromSquare, int toSquare, Type typeOfCapturedPiece = 0,
            Type typeToPromoteInto = 0)
        {
            var move = 0;
            move |= kindOfMove;
            move |= fromSquare << 5;
            move |= toSquare << 10;
            move |= (int) typeOfCapturedPiece << 16;
            move |= (int) typeToPromoteInto << 21;
            return move;
        }


        /// <summary>
        /// Apply a given mode.
        /// </summary>
        /// <param name="encodedMove">The encoded move</param>
        public static void DoMove(int encodedMove)
        {
            // Decode the move
        }
    }
}