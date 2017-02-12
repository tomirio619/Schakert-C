using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace sChakert.Chessboard
{
    public static class StateManager
    {
        /// <summary>
        /// Stack of states (represented as an integer) containing the castling availability, check status for both kings 
        /// and whether kings and rooks have moved.
        /// </summary>
        public static readonly Stack<int> GeneralInfoStack = new Stack<int>();

        /// <summary>
        /// Stack of enPassant square values. The enPassant square is represented as a board index (integer) 
        /// </summary>
        public static readonly Stack<int> EnPassantPositionStack = new Stack<int>();

        /// <summary>
        /// Indicates whether the white king is in check (1) or not (0)
        /// </summary>
        public static int WhiteKingInCheck = 0;

        /// <summary>
        /// Indicates whether the white king has moved (1) or not (0)
        /// </summary>
        public static int WhiteKingHasMoved = 0;

        /// <summary>
        /// Indicates whether the white queenside rook has moved (1) or not (0)
        /// </summary>
        public static int WhiteRookQueenSideHasMoved = 0;

        /// <summary>
        /// Indicates whether the white kingside rook has moved (1) or not (0)
        /// </summary>
        public static int WhiteRookKingSideHasMoved = 0;

        /// <summary>
        /// Indicates whether the black king is in check (1) or not (0)
        /// </summary>
        public static int BlackKingInCheck = 0;

        /// <summary>
        /// Indicates whether the black king has moved (1) or not (0)
        /// </summary>
        public static int BlackKingHasMoved = 0;

        /// <summary>
        /// Indicates whether the black queenside rook has moved (1) or not (0)
        /// </summary>
        public static int BlackRookQueenSideHasMoved = 0;

        /// <summary>
        /// Indicates whether the black kingside rook has moved (1) or not (0)
        /// </summary>
        public static int BlackRookKingSideHasMoved = 0;

        /// <summary>
        /// Board index representing the enPassant square.
        /// </summary>
        public static int EnPassantPos = -1;

        /// <summary>
        /// SaveCurrentState the current state in two stacks: one representing the castling availability of the kings and whether they are in check.
        /// The other one stores the enPassant value square, which is -1 if there is no vulnerable square.
        /// </summary>
        public static void SaveCurrentState()
        {
            var curState = 0;
            var pos = 0;
            // Save current king castling availability + check status for both kings
            curState |= WhiteKingInCheck << pos++;
            curState |= WhiteKingHasMoved << pos++;
            curState |= WhiteRookQueenSideHasMoved << pos++;
            curState |= WhiteRookKingSideHasMoved << pos++;
            curState |= BlackKingInCheck << pos++;
            curState |= BlackKingHasMoved << pos++;
            curState |= BlackRookQueenSideHasMoved << pos++;
            curState |= BlackRookKingSideHasMoved << pos;
            GeneralInfoStack.Push(curState);
            // SaveCurrentState enPassant square
            EnPassantPositionStack.Push(EnPassantPos);
        }

        /// <summary>
        /// RestorePreviousState the previous state from the two stacks.
        /// This is done by converting the integer representing the castling availability and the check status of the king to
        /// a binary string.
        /// </summary>
        public static void RestorePreviousState()
        {
            var previousState = GeneralInfoStack.Pop();
            var previousStateBinString = Utilities.SystemIsLittleEndian
                ? Utilities.Reverse(Convert.ToString(previousState, 2)).PadRight(8, '0')
                : Convert.ToString(previousState, 2).PadRight(8, '0');

            Debug.WriteLine("previous state:" + previousStateBinString);
            var pos = 0;
            /*
             Restore king castling availability + check status for both kings.
             This is done by converting the state (popped from the stack and represented as an integer)
             to a bitstring (padded to a lenght of 8 with zeros).
             This bitstring is used to retrieve the original int values as chars.
             These are converted to integers by subtracting the char '0' from the value.
             */
            WhiteKingInCheck = previousStateBinString[pos++] - '0';
            WhiteKingHasMoved = previousStateBinString[pos++] - '0';
            WhiteRookQueenSideHasMoved = previousStateBinString[pos++] - '0';
            WhiteRookKingSideHasMoved = previousStateBinString[pos++] - '0';
            BlackKingInCheck = previousStateBinString[pos++] - '0';
            BlackKingHasMoved = previousStateBinString[pos++] - '0';
            BlackRookQueenSideHasMoved = previousStateBinString[pos++] - '0';
            BlackRookKingSideHasMoved = previousStateBinString[pos] - '0';
            // Restore current value of the enPassant square
            EnPassantPos = EnPassantPositionStack.Pop();
        }

        public static string GetState()
        {
            var str = new StringBuilder();
            var fields = typeof(StateManager).GetFields(BindingFlags.Public | BindingFlags.Static);
            // We sort the values on descending name
            var alphabeticallySortedFields = fields.Where(x => x.FieldType == typeof(int))
                .OrderByDescending(x => x.Name).Reverse();
            foreach (var field in alphabeticallySortedFields)
                str.Append(field.Name).Append("\t").Append((int) field.GetValue(null)).Append("\n");
            return str.ToString();
        }
    }
}