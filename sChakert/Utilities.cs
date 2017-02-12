using System;
using System.Collections.Generic;
using System.Text;
using RadaCode.SwissKnife;

namespace sChakert
{
    internal static class Utilities
    {
        /// <summary>
        /// File constants
        /// </summary>
        public const int FileA = 0;

        public const int FileB = 1;
        public const int FileC = 2;
        public const int FileD = 3;
        public const int FileE = 4;
        public const int FileF = 5;
        public const int FileG = 6;
        public const int FileH = 7;

        /// <summary>
        /// Random number generator
        /// </summary>
        private static readonly Random Rand = new Random();

        /// <summary>
        /// Mersenne Twister RNG, seeded Mersenne Twister with a random number
        /// </summary>
        private static readonly MersenneTwister Mt = new MersenneTwister(Rand.Next());

        /// <summary>
        /// 64 bit De Bruijn number
        /// </summary>
        private const ulong Debruijn64 = 0x03f79d71b4cb0a89;

        // ‭001111 110111 100111 010111 000110 110100 110010 110000 101010 001001‬

        /// <summary>
        /// The deBruijn bit positions.
        /// </summary>
        private static readonly int[] MultiplyDeBruijnBitPosition = new int[]
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

        private static int[] MultiplyDeBruijnBitPositionTest = new int[64];

        /// <summary>
        /// Indicates if the system is using little endianess
        /// </summary>
        public static bool SystemIsLittleEndian = IsLittleEndian();

        /// <summary>
        /// Bit masks for clearing a certain file.
        /// Index this array using the static file names, like "FILE_A".
        /// </summary>
        public static ulong[] ClearFile = new ulong[]
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
        /// Calculate the index of the first non-zero LSB in a given bitboard.
        /// @See https://chessprogramming.wikispaces.com/BitScan
        /// </summary>
        /// <param name="bitboard">The bitboard for which we want to find the index of the first non-zero LSB.</param>
        /// <returns>The index of the first non-zero LSB.</returns>
        public static int BitScanForward(ulong bitboard)
        {
            // Contains all bits set including and below the least signifant set bit
            bitboard ^= bitboard - 1;
            // Multiply with the DeBruijn number
            bitboard *= Debruijn64;
            // Create index
            bitboard >>= 58;
            return MultiplyDeBruijnBitPosition[bitboard];
        }

        /// <summary>
        /// Formats a given bitboard string to a chess board.
        /// </summary>
        /// <param name="bitboard"></param>
        /// <returns>The bitboard string formatted as a chess board. </returns>
        public static string FormatBitBoard(string bitboard)
        {
            if (bitboard.Length == 8)
                return bitboard;
            var begin = 8;
            var subBitString = bitboard.Substring(begin);
            var rank = bitboard.Substring(0, 8);
            return FormatBitBoard(subBitString) + "\n" + rank;
        }

        /// <summary>
        ///  Get all the indices of the non-zero bits
        /// </summary>
        /// <param name="bitboard">The bitboard</param>
        /// <returns>A vector with the indices of the non-zero bits.</returns>
        public static List<int> GetActiveBitIndices(ulong bitboard)
        {
            var activeBitIndices = new List<int>();
            var boardIndex = 0;
            while (bitboard > 0)
            {
                // The bitboard has one or more active bits
                var nextBitIndex = BitScanForward(bitboard);
                boardIndex += nextBitIndex;
                activeBitIndices.Add(boardIndex);
                bitboard >>= nextBitIndex + 1;
                boardIndex++;
            }
            return activeBitIndices;
        }

        /// <summary>
        ///  Check whether the system uses little or big endian format.
        /// </summary>
        /// <returns>
        /// <code>True</code> if the system usese little-endian format, <code>False</code> otherwise.
        /// </returns>
        public static unsafe bool IsLittleEndian()
        {
            short number = 0x1;
            var numPtr = (char*) &number;
            return numPtr[0] == 1;
        }

        /// <summary>
        ///  Print formatted hex constants.
        ///  This can be used to see if the conversion to a bitstring complies with the
        ///  underlying representation(big - or little - endian).
        /// </summary>
        public static void PrintHexConstants()
        {
            // Output if system is using little-endian representation
            Console.WriteLine("Is little-endian representation: " + SystemIsLittleEndian);

            // Hexadecimal constants for testing the bitboard representation.
            var lerfConstants = new ulong[]
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
            for (var i = 0; i < 8; i++)
            {
                var binary = ToBitString(lerfConstants[i]);
                Console.WriteLine("Hexadecimal:" + ToHex(lerfConstants[i]));
                Console.WriteLine("Binary:\n" + FormatBitBoard(binary));
            }

            var bitString = ToBitString(0x00000000FF000000);
            var formattedBitString = FormatBitBoard(bitString);
            Console.WriteLine("rank 4 bitString: \n" + formattedBitString);
        }

        /// <summary>
        /// Get a random 64 bit number.
        /// </summary>
        /// <returns>random 64 bit number</returns>
        public static ulong RandomUint64()
        {
            var u1 = (ulong) Mt.Next() & 0xFFFF;
            var u2 = (ulong) Mt.Next() & 0xFFFF;
            var u3 = (ulong) Mt.Next() & 0xFFFF;
            var u4 = (ulong) Mt.Next() & 0xFFFF;
            return u1 | (u2 << 16) | (u3 << 32) | (u4 << 48);
        }

        /// <summary>
        /// Get a random 64 bit number with only a few bits set.
        /// This method is used in finding a potential magic number for the rook and bishop movements.
        /// </summary>
        /// <returns></returns>
        public static ulong RandomUint64Fewbits()
        {
            return RandomUint64() & RandomUint64() & RandomUint64();
        }

        /// <summary>
        /// Get the reverse of a string
        /// </summary>
        /// <param name="s">The string to reverse</param>
        /// <returns></returns>
        public static string Reverse(string s)
        {
            var charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        /// <summary>
        ///     Rotate a 64 unsigned integer. If n is positive, the bits wil be rotated towards the LSB.
        ///     If n is negative, the bits will be rotated towards the MSB.
        /// </summary>
        /// <param name="v">The 64 unsigned int to Rotate</param>
        /// <param name="n">
        ///     Indicates the number of bits to which side the given 64 bit number will be rotated.
        ///     If the sign is positive, the bits will be rotated to the LSB. If the sign is negative, they will be rotated
        ///     to the MSB.
        /// </param>
        /// <returns></returns>
        public static ulong Rotate(ulong v, ulong n)
        {
            n = n & 63UL;
            if (n > 0)
                v = (v >> (int) n) | (v << (64 - (int) n));
            return v;
        }

        /// <summary>
        ///     Converts a bitboard to the corresponding bitstring (containing 0's and 1's)
        /// </summary>
        /// <param name="bitboard">The bitboard</param>
        /// <returns>Bitstring of the bitboard.</returns>
        public static string ToBitString(ulong bitboard)
        {
            var boardString = new StringBuilder(64);
            // Fil the string with zeros
            boardString.Append('0', 64);
            for (var i = 0; i < 64; i++)
                if (((1UL << i) & bitboard) > 0)
                    boardString[i] = '1';
            // The bits are returned in the reversed order. Therefore we return the reversed string.
            return SystemIsLittleEndian ? Reverse(boardString.ToString()) : boardString.ToString();
        }

        /// <summary>
        ///     Converts a bitboard to a binary string representing a chess board.
        /// </summary>
        /// <param name="bitboard"></param>
        /// <returns>Binary string formatted as a chess board.</returns>
        public static string ToChessBoard(ulong bitboard)
        {
            var binaryString = ToBitString(bitboard);
            return FormatBitBoard(binaryString);
        }

        /// <summary>
        ///     Convert a uint64 to HEX.
        /// </summary>
        /// <param name="value">The uint64 value from which we want to have the HEX representation</param>
        /// <returns></returns>
        public static string ToHex(ulong value)
        {
            var hex = value.ToString("X16");
            return "0x" + hex;
        }

        /// <summary>
        /// Generate a deBruijn number based on the PreferOne algorithm
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static ulong PreferOneDeBruijn64(int n = 6)
        {
            Console.WriteLine("Test");
            // start with a string of n zeros
            var binaryDeBruijn = new StringBuilder();
            binaryDeBruijn.Append('0', n);
            while (true)
            {
                var curLenght = binaryDeBruijn.Length;
                var begin = curLenght - n + 1;
                // Second paramater of ToString specified lenght of substring
                var newRowWithOne = binaryDeBruijn.ToString(begin, n - 1) + '1';
                var newRowWithZero = binaryDeBruijn.ToString(begin, n - 1) + '0';
                if (!binaryDeBruijn.ToString().Contains(newRowWithOne))
                    binaryDeBruijn.Append("1");
                else if (!binaryDeBruijn.ToString().Contains(newRowWithZero))
                    binaryDeBruijn.Append("0");
                else
                    break;
            }
            // Drop the last n-1 bits, as they are equal to the first n-1 bits.
            // You can imagine the other bits in a circle
            var deBruijnCyclic = binaryDeBruijn.ToString(0, binaryDeBruijn.Length - n + 1);
            var deBruijn = Convert.ToUInt64(deBruijnCyclic, 2);
            return deBruijn;
        }
    }
}