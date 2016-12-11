using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadaCode.SwissKnife;

namespace sChakert.Magic
{
    static class Utilities
    {
        /*Random number generator.*/
        private static Random rand = new Random();

        /*Mersenne Twister RNG, seeded Mersenne Twister with Random device.*/
        private static MersenneTwister mt = new MersenneTwister(rand.Next());

        /*64 bit De Bruijn number.*/
        private static UInt64 debruijn64 = 0x03f79d71b4cb0a89;

        /// <summary>
        ///  The deBruijn bit positions.
        /// </summary>
        private static int[] MultiplyDeBruijnBitPosition = new int[64]
        {
            0, 47, 1, 56, 48, 27, 2, 60,
            57, 49, 41, 37, 28, 16, 3, 61,
            54, 58, 35, 52, 50, 42, 21, 44,
            38, 32, 29, 23, 17, 11, 4, 62,
            46, 55, 26, 59, 40, 36, 15, 53,
            34, 51, 20, 43, 31, 22, 10, 45,
            25, 39, 14, 33, 19, 30, 9, 24,
            13, 18, 8, 12, 7, 6, 5, 63
        };


        /*
        File constants
        */
        public const int FILE_A = 0;
        public const int FILE_B = 1;
        public const int FILE_C = 2;
        public const int FILE_D = 3;
        public const int FILE_E = 4;
        public const int FILE_F = 5;
        public const int FILE_G = 6;
        public const int FILE_H = 7;

        /// <summary>
        /// Bit masks for clearing a certain file.
        /// Index this array using the static file names, like "FILE_A".
        /// </summary>
        public static UInt64[] clearFile = new UInt64[8]
        {
            0xFEFEFEFEFEFEFEFE, // FILE_A
            0xFDFDFDFDFDFDFDFD, // FILE_B
            0xFBFBFBFBFBFBFBFB, // FILE_C
            0xF7F7F7F7F7F7F7F7, // FILE_D
            0xEFEFEFEFEFEFEFEF, // FILE_E
            0xDFDFDFDFDFDFDFDF, // FILE_F
            0xBFBFBFBFBFBFBFBF, // FILE_G
            0x7F7F7F7F7F7F7F7F // FILE_H
        };

        /// <summary>
        /// Rotate a 64 unsigned integer. If n is positive, the bits wil be rotated towards the LSB.
        /// If n is negative, the bits will be rotated towards the MSB.
        /// </summary>
        /// <param name="v">The 64 unsigned int to Rotate</param>
        /// <param name="n">Indicates the number of bits to which side the given 64 bit number will be rotated.
        /// If the sign is positive, the bits will be rotated to the LSB. If the sign is negative, they will be rotated
        /// to the MSB.</param>
        /// <returns></returns>
        public static UInt64 Rotate(UInt64 v, UInt64 n)
        {
            n = n & 63UL;
            if (n > 0)
                v = (v >> (int) n) | (v << (64 - (int) n));
            return v;
        }

        /// <summary>
        /// Check whether the system uses little or big endian format.
        /// </summary>
        /// <returns>
        /// <code>True</code> if the system usese little-endian format, <code>False</code> otherwise.
        /// </returns>
        public static unsafe bool isLittleEndian()
        {
            short number = 0x1;
            char* numPtr = (char*) &number;
            return (numPtr[0] == 1);
        }

        /// <summary>
        /// Converts a bitboard to the corresponding bitstring (containing 0's and 1's)
        /// </summary>
        /// <param name="bitboard">The bitboard</param>
        /// <returns>Bitstring of the bitboard.</returns>
        public static string ToBitString(UInt64 bitboard)
        {
            StringBuilder boardString = new StringBuilder(64);
            // Fil the string with zeros
            boardString.Append('0', 64);
            for (int i = 0; i < 64; i++)
            {
                if (((1UL << i) & bitboard) > 0)
                {
                    boardString[i] = '1';
                }
            }
            // The bits are returned in the reversed order. Therefore we return the reversed string.
            if (!isLittleEndian())
            {
                return Reverse(boardString.ToString());
            }
            return boardString.ToString();
        }

        /// <summary>
        /// Convert a uint64 to HEX.
        /// </summary>
        /// <param name="value">The uint64 value from which we want to have the HEX representation</param>
        /// <returns></returns>
        public static string toHEX(UInt64 value)
        {
            string HEX = value.ToString("X16");
            return "0x" + HEX;
        }

        /// <summary>
        /// Get the reverse of a string
        /// </summary>
        /// <param name="s">The string to reverse</param>
        /// <returns></returns>
        public static string Reverse(String s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }


        /// <summary>
        /// Formats a given bitboard string to a chess board.
        /// </summary>
        /// <param name="bitboard"></param>
        /// <returns>The bitboard string formatted as a chess board. </returns>
        public static string FormatBitBoard(string bitboard)
        {
            if (bitboard.Length == 8)
            {
                return bitboard;
            }
            int begin = 8;
            string subBitString = bitboard.Substring(begin);
            string rank = bitboard.Substring(0, 8);
            return FormatBitBoard(subBitString) + "\n" + rank;
        }

        /// <summary>
        /// Converts a bitboard to a binary string representing a chess board.
        /// </summary>
        /// <param name="bitboard"></param>
        /// <returns>Binary string formatted as a chess board.</returns>
        public static string ToChessBoard(UInt64 bitboard)
        {
            string binaryString = ToBitString(bitboard);
            return FormatBitBoard(binaryString);
        }

        /// <summary>
        /// Print formatted hex constants.
        /// This can be used to see if the conversion to a bitstring complies with the
        /// underlying representation(big - or little - endian).
        /// </summary>
        public static void PrintHexConstants()
        {
            // Output if system is using little-endian representation
            Console.WriteLine("Is little-endian representation: " + isLittleEndian());

            // Hexadecimal constants for testing the bitboard representation.
            UInt64[] LERF_CONSTANTS = new UInt64[8]
            {
                0x0101010101010101, // a-file
                0x8080808080808080, // h-file
                0x00000000000000FF, // 1st rank
                0xFF00000000000000, // 8th rank
                0x8040201008040201, // a1-h8 diagonal
                0x0102040810204080, // h1-a8 antidiagonal
                0x55AA55AA55AA55AA, // light squares
                0xAA55AA55AA55AA55 // dark squares
            };
            for (int i = 0; i < 8; i++)
            {
                string binary = ToBitString(LERF_CONSTANTS[i]);
                Console.WriteLine("Hexadecimal:" + toHEX(LERF_CONSTANTS[i]));
                Console.WriteLine("Binary:\n" + FormatBitBoard(binary));
            }

            string bitString = ToBitString(0x00000000FF000000);
            string formattedBitString = FormatBitBoard(bitString);
            Console.WriteLine("rank 4 bitString: \n" + formattedBitString);
        }

        /// <summary>
        /// Get all the indices of the non-zero bits
        /// </summary>
        /// <param name="bitboard">The bitboard</param>
        /// <returns>A vector with the indices of the non-zero bits.</returns>
        public static List<int> GetActiveBitIndices(UInt64 bitboard)
        {
            List<int> activeBitIndices = new List<int>();
            int boardIndex;
            while (bitboard > 0)
            {
                // The bitboard has one or more active bits
                boardIndex = BitScanForward(bitboard);
                activeBitIndices.Add(boardIndex);
                bitboard >>= (boardIndex + 1);
            }
            return activeBitIndices;
        }

        /// <summary>
        /// Get a random 64 bit number.
        /// </summary>
        /// <returns>random 64 bit number</returns>
        public static UInt64 RandomUint64()
        {
            UInt64 u1, u2, u3, u4;
            u1 = (UInt64) (mt.Next()) & 0xFFFF;
            u2 = (UInt64) (mt.Next()) & 0xFFFF;
            u3 = (UInt64) (mt.Next()) & 0xFFFF;
            u4 = (UInt64) (mt.Next()) & 0xFFFF;
            return u1 | (u2 << 16) | (u3 << 32) | (u4 << 48);
        }

        /// <summary>
        /// Get a random 64 bit number with only a few bits set.
        /// This method is used in finding a potential magic number for the rook and bishop movements.
        /// </summary>
        /// <returns></returns>
        public static UInt64 RandomUint64Fewbits()
        {
            return RandomUint64() & RandomUint64() & RandomUint64();
        }

        /// <summary>
        /// Calculate the index of the first non-zero LSB in a given bitboard.
        /// It requires one more operation than the earlier one involving modulus division,
        /// but the multiply may be faster.
        /// The expression (v & -v) extracts the least significant 1 bit from v.
        /// The constant 0x03f79d71b4cb0a89 is a de Bruijn sequence,
        /// which produces a unique pattern of bits into the high 6 bits for each possible bit position that it is multiplied against.
        /// When there are no bits set, it returns 0.
        /// More information can be found by reading the paper
        /// "Using de Bruijn Sequences to Index 1 in a Computer Word by Charles E. Leiserson, Harald Prokof, and Keith H. Randall".
        /// </summary>
        /// <param name="bitboard">The bitboard for which we want to find the index of the first non-zero LSB.</param>
        /// <returns>The index of the first non-zero LSB.</returns>
        public static int BitScanForward(UInt64 bitboard)
        {
            bitboard ^= (bitboard - 1);
            bitboard *= debruijn64;
            bitboard >>= 58;
            return MultiplyDeBruijnBitPosition[bitboard];
        }
    }
}