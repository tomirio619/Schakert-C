using System;
using System.Collections.Generic;

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
        ///     Encode a move given the current board index on which the piece stands, the new board index after the move was
        ///     applied and the type of the piece
        /// </summary>
        /// <param name="fromBoardIndex">The original position of the piece</param>
        /// <param name="toBoardIndex">The new position of the piece</param>
        /// <param name="pieceType">The type of the piece</param>
        /// <returns></returns>
        public static List<int> EncodeMove(int fromBoardIndex, int toBoardIndex, Type pieceType, Chessboard chessBoard)
        {
            var pieceColor = chessBoard.GetPieceColor(fromBoardIndex);
            var newSquareInfo = chessBoard.GetPieceTypeAndColour(toBoardIndex);
            var newSquareOccupied = newSquareInfo.Item1;
            var capturedPieceType = newSquareInfo.Item3;

            // Start encoding the move(s)
            if (pieceType == Type.Pawn)
                return EncodePawnMoves(fromBoardIndex, toBoardIndex, newSquareOccupied, pieceColor, capturedPieceType);
            //TODO castling
            return new List<int>();
        }

        private static List<int> EncodePawnMoves(int fromBoardIndex, int toBoardIndex, bool newSquareOccupied,
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
                kindOfMove = 4;
                if ((toBoardIndex < 8) || (toBoardIndex > 55))
                    kindOfMove = 15;
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
                pawnMoves.Add(CreateMove(kindOfMove, fromBoardIndex, toBoardIndex, capturedPieceType, Type.Knight));
                pawnMoves.Add(CreateMove(kindOfMove, fromBoardIndex, toBoardIndex, capturedPieceType, Type.Bishop));
                pawnMoves.Add(CreateMove(kindOfMove, fromBoardIndex, toBoardIndex, capturedPieceType, Type.Rook));
                pawnMoves.Add(CreateMove(kindOfMove, fromBoardIndex, toBoardIndex, capturedPieceType, Type.Queen));
                return pawnMoves;
            }
            // Only one move
            pawnMoves.Add(CreateMove(kindOfMove, fromBoardIndex, toBoardIndex, capturedPieceType));
            return pawnMoves;
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
        public static int CreateMove(int kindOfMove, int fromSquare, int toSquare, Type typeOfCapturedPiece = 0,
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

        public static void DoMove(int encodedMove)
        {
        }
    }
}