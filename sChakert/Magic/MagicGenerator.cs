using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace sChakert.Magic
{
    public static class MagicGenerator
    {
        /// <summary>
        /// Rook magic numbers
        /// </summary>
        public static ulong[] RookMagicNumbers = new ulong[64]
        {
            0x0280018440002210,
            0x0840100040002002,
            0x820008A012008040,
            0x81001810010020B4,
            0x0D80060800040080,
            0x8200181011060004,
            0x0880208015000200,
            0x0A00004405088026,
            0x0014800020804002,
            0x0102002080450200,
            0x0810803001882000,
            0x0041002230028900,
            0x2809000408001100,
            0x100200100C084200,
            0x402100840B003200,
            0x4016000481040042,
            0x0871060022004082,
            0x0440850040006101,
            0x0040808010006000,
            0x0808008010003880,
            0x0004008004380080,
            0x1000180104409020,
            0x0408808041000A00,
            0x80400A0002470084,
            0x8000400280009221,
            0x02045000C0082001,
            0x0014900480200084,
            0x0011001900241000,
            0x0000040080080181,
            0x4A42002200080410,
            0x48000B0C00280610,
            0x000412820000C411,
            0x9040098223800840,
            0x0000600140401002,
            0x00200020C1001100,
            0xC080480080803000,
            0x0CC2801800800400,
            0x2002000280802C00,
            0x0206001C42000801,
            0x1000028342000407,
            0xA24001C224888000,
            0x0040400020008080,
            0x0102018810420020,
            0x2030016300D10008,
            0x0805480100150030,
            0x0001840002008080,
            0xC002000C28020081,
            0x00008080450A000C,
            0x0600204001800880,
            0x0005022040018100,
            0x4000B000A0008080,
            0x0881080084100180,
            0x800080E100401002,
            0x0101800400020080,
            0x4040910210480400,
            0x0008010400814200,
            0x3119810048A20052,
            0x0440120220810442,
            0x104810A000450019,
            0x8020203001000825,
            0xC226002008441002,
            0x06410008020C000D,
            0x0641001A0002BC01,
            0x0084082840810402
        };

        /// <summary>
        ///  Bishop magic numbers
        /// </summary>
        public static ulong[] BishopMagicNumbers = new ulong[64]
        {
            0x3021020203421080,
            0x0004100202013228,
            0x0008020AD208010C,
            0x20241C0A80100801,
            0x0402021000004C40,
            0x0032027014400344,
            0x000040820A4019A0,
            0x802201011D300E10,
            0x0104120204380090,
            0x0240082407220A20,
            0x0100060C03020000,
            0x8020020A020202A4,
            0x1049411040054001,
            0x0200458820081000,
            0x6200005808180422,
            0x0880089082211000,
            0x0208018410100200,
            0x0010002002008900,
            0x0009000806410200,
            0x00880200820A4020,
            0x0002008402131012,
            0x0A83000200809400,
            0xC015000A06909400,
            0x0101080141080100,
            0x9020E00258081100,
            0x0003A04110060200,
            0x00081402480820A0,
            0x1402480002820040,
            0x0803010000104000,
            0x0448081000804400,
            0x400080A0140A0800,
            0x0294110110848182,
            0x8001880805401000,
            0x202403A400887000,
            0x32082088001000A0,
            0x0086008120B20200,
            0x0810008200002200,
            0x0906100502020880,
            0x2401840406011101,
            0x0A08084112004902,
            0x0414042008C00601,
            0x1080440420142500,
            0x0106002208000108,
            0x004070C20083480A,
            0x8100600140400400,
            0x0040032112080100,
            0x0010A40800411480,
            0x000809140C880224,
            0x0404008884908004,
            0x000082031120C000,
            0x0018508400A8101A,
            0x2054010284040000,
            0x040800C0850109A0,
            0x0000D202100500D0,
            0x200A08080800C100,
            0x0002900942088006,
            0x0004208420884000,
            0x0304602104108410,
            0xE008040041445030,
            0x0800040830228800,
            0x110C500440A92201,
            0x8008012004100084,
            0x0002921001480684,
            0x01020202082A0081
        };

        /// <summary>
        /// Rook lookup table
        /// </summary>
        public static ulong[,] RookLookupTable = new ulong[64, 4096];

        /// <summary>
        /// Bishop lookup table
        /// </summary>
        public static ulong[,] BishopLookupTable = new ulong[64, 4096];

        /// <summary>
        /// Indicates whether we use pre calculated magic numbers or not.
        /// </summary>
        public static bool UsePreCalculatedMagicNumbers = true;

        /// <summary>
        ///     Generate the magic numbers based on the piece type provided.
        /// </summary>
        /// <param name="isRook">
        ///     Whether we want to generate the magic numbers for the rook. If this is false,
        ///     the magic numbers for the bishop will be generated.
        /// </param>
        private static void GenerateMagicNumbers(bool isRook)
        {
            if (isRook)
                Debug.WriteLine("Rook magic numbers");
            else
                Debug.WriteLine("Bishop magic numbers");
            // Current magic number
            ulong magicNumber;
            // Our mapping
            var mapping = new ulong[4096];
            // Whether we found a magic number for the current board index
            bool magicFound;
            // attack set
            ulong attackSet;
            // Active bits in attack set
            List<int> activeBitIndicesInAttackSet;
            // Total number of non-zero bits in the attack set
            int bitCountAttackSet;
            // Attack variations
            List<ulong> attackVariations;
            // Total number of variations
            int variationCount;

            for (var boardIndex = 0; boardIndex < 64; boardIndex++)
            {
                magicFound = false;
                attackSet = isRook ? GetRookAttackSet(boardIndex) : GetBishopAttackSet(boardIndex);
                activeBitIndicesInAttackSet = Utilities.GetActiveBitIndices(attackSet);
                bitCountAttackSet = activeBitIndicesInAttackSet.Count();
                variationCount = (int) (1UL << bitCountAttackSet);
                attackVariations = GetAttackVariations(attackSet, boardIndex);
                do
                {
                    // Clear our previous mapping
                    Array.Clear(mapping, 0, mapping.Length);
                    // Generate a random number that could possibly be a "magic number"
                    magicNumber = Utilities.RandomUint64Fewbits();
                    for (var variation = 0; variation < attackVariations.Count(); variation++)
                    {
                        var attackVariation = attackVariations[variation];
                        var magicIndex = (attackVariation*magicNumber) >> (64 - bitCountAttackSet);
                        var moveset = isRook
                            ? GetRookMoveSet(attackVariation, boardIndex)
                            : GetBishopMoveSet(attackVariation, boardIndex);
                        if ((mapping[magicIndex] != 0) && (mapping[magicIndex] != moveset))
                            break;
                        // No collission, or stored in mapping was 0
                        mapping[magicIndex] = moveset;
                        magicFound = variation + 1 == variationCount;
                    }
                } while (!magicFound);
                // We found a magic number
                Debug.WriteLine(Utilities.ToHex(magicNumber) + ",");
                if (isRook)
                    RookMagicNumbers[boardIndex] = magicNumber;
                else
                    BishopMagicNumbers[boardIndex] = magicNumber;
            }
        }

        /// <summary>
        ///     Generate all of the variations from a given attack set.
        /// </summary>
        /// <param name="attackSet">The attack set.</param>
        /// <param name="boardIndex">The board index.</param>
        /// <returns>A vector containing all of the variations for the given attack set.</returns>
        private static List<ulong> GetAttackVariations(ulong attackSet, int boardIndex)
        {
            var activeBitIndices = Utilities.GetActiveBitIndices(attackSet);
            var variationCount = 1 << activeBitIndices.Count();
            // The size of the list becomes the variation count
            var attackVariations = new List<ulong>(new ulong[variationCount]);

            for (var variation = 0; variation < variationCount; variation++)
            {
                var activeBitIndicesInVariation = Utilities.GetActiveBitIndices((ulong) variation);
                for (var i = 0; i < activeBitIndicesInVariation.Count(); i++)
                    attackVariations[variation] |= 1UL << activeBitIndices[activeBitIndicesInVariation[i]];
            }
            return attackVariations;
        }

        /// <summary>
        ///     Generate the attack set of the bishop for a given boardindex.
        /// </summary>
        /// <param name="boardIndex">The board index</param>
        /// <returns>The attack set belonging to the bishop at the given board index.</returns>
        public static ulong GetBishopAttackSet(int boardIndex)
        {
            var attackSet = 0UL;
            for (var i = boardIndex + 9; (i%8 != 7) && (i%8 != 0) && (i <= 55); i += 9)
                attackSet |= 1UL << i;
            for (var i = boardIndex - 7; (i%8 != 7) && (i%8 != 0) && (i >= 8); i -= 7)
                attackSet |= 1UL << i;
            for (var i = boardIndex - 9; (i%8 != 7) && (i%8 != 0) && (i >= 8); i -= 9)
                attackSet |= 1UL << i;
            for (var i = boardIndex + 7; (i%8 != 7) && (i%8 != 0) && (i <= 55); i += 7)
                attackSet |= 1UL << i;
            return attackSet;
        }

        /// <summary>
        ///     Generate the correct bishop moveset for a given attack variation.
        /// </summary>
        /// <param name="bishopAttackVariation">The attack variation.</param>
        /// <param name="boardIndex">The board index.</param>
        /// <returns>The move set belonging to the bishop with the given attack variation at the given board index.</returns>
        private static ulong GetBishopMoveSet(ulong bishopAttackVariation, int boardIndex)
        {
            var bishopMoveSet = 0UL;
            int i;
            for (i = boardIndex + 9;
                (i%8 != 0) && (i%8 != 7) && (i < 56) && ((bishopAttackVariation & (1UL << i)) == 0);
                i += 9)
                bishopMoveSet |= 1UL << i;
            for (i = boardIndex - 7;
                (i%8 != 0) && (i%8 != 7) && (i > 7) && ((bishopAttackVariation & (1UL << i)) == 0);
                i -= 7)
                bishopMoveSet |= 1UL << i;
            for (i = boardIndex + 7;
                (i%8 != 0) && (i%8 != 7) && (i < 56) && ((bishopAttackVariation & (1UL << i)) == 0);
                i += 7)
                bishopMoveSet |= 1UL << i;
            for (i = boardIndex - 9;
                (i%8 != 0) && (i%8 != 7) && (i > 7) && ((bishopAttackVariation & (1UL << i)) == 0);
                i -= 9)
                bishopMoveSet |= 1UL << i;
            return bishopMoveSet;
        }

        /// <summary>
        ///     Get the possible moves for a bishop given the attack variation and the board index.
        /// </summary>
        /// <param name="attackVariation">The attack variation.</param>
        /// <param name="boardIndex">The board index.</param>
        /// <returns>The possible moves given of the bishop given the attack variation and the board index</returns>
        private static ulong GetBishopPossibleMoves(ulong attackVariation, int boardIndex)
        {
            var moves = 0UL;
            int i;
            for (i = boardIndex + 9; (i%8 != 0) && (i < 64); i += 9)
            {
                moves |= 1UL << i;
                if ((attackVariation & (1UL << i)) != 0) break;
            }
            for (i = boardIndex - 7; (i%8 != 0) && (i >= 0); i -= 7)
            {
                moves |= 1UL << i;
                if ((attackVariation & (1UL << i)) != 0) break;
            }
            for (i = boardIndex + 7; (i%8 != 7) && (i < 64); i += 7)
            {
                moves |= 1UL << i;
                if ((attackVariation & (1UL << i)) != 0) break;
            }
            for (i = boardIndex - 9; (i%8 != 7) && (i >= 0); i -= 9)
            {
                moves |= 1UL << i;
                if ((attackVariation & (1UL << i)) != 0) break;
            }
            return moves;
        }

        /// <summary>
        ///     Generate the attack set of the rook for a given board index.
        /// </summary>
        /// <param name="boardIndex">The board index.</param>
        /// <returns>The attack set belonging to the rook at the given board index.</returns>
        public static ulong GetRookAttackSet(int boardIndex)
        {
            var attackSet = 0UL;
            for (var i = boardIndex + 8; i <= 55; i += 8)
                attackSet |= 1UL << i;
            for (var i = boardIndex - 8; i >= 8; i -= 8)
                attackSet |= 1UL << i;
            for (var i = boardIndex + 1; (i%8 != 7) && (i%8 != 0); i++)
                attackSet |= 1UL << i;
            for (var i = boardIndex - 1; (i%8 != 7) && (i%8 != 0) && (i >= 0); i--)
                attackSet |= 1UL << i;
            return attackSet;
        }

        /// <summary>
        ///     Generate the correct rook moveset for a given attack variation.
        /// </summary>
        /// <param name="rookAttackVariation">The attack variation.</param>
        /// <param name="boardIndex">The board index.</param>
        /// <returns>The move set belonging to the rook with the given attack variation at the given board index.</returns>
        private static ulong GetRookMoveSet(ulong rookAttackVariation, int boardIndex)
        {
            var rookMoveSet = 0UL;
            int i;
            for (i = boardIndex + 8; (i < 56) && ((rookAttackVariation & (1UL << i)) == 0); i += 8)
                rookMoveSet |= 1UL << i;
            for (i = boardIndex - 8; (i > 7) && ((rookAttackVariation & (1UL << i)) == 0); i -= 8)
                rookMoveSet |= 1UL << i;
            for (i = boardIndex + 1; (i%8 != 7) && (i%8 != 0) && ((rookAttackVariation & (1UL << i)) == 0); i += 1)
                rookMoveSet |= 1UL << i;
            for (i = boardIndex - 1;
                (i%8 != 7) && (i%8 != 0) && (i >= 0) && ((rookAttackVariation & (1UL << i)) == 0);
                i -= 1)
                rookMoveSet |= 1UL << i;
            return rookMoveSet;
        }

        /// <summary>
        ///     Get the possible moves for a rook given the attack variation and the board index.
        /// </summary>
        /// <param name="attackVariation">The attack variation.</param>
        /// <param name="boardIndex">The board index.</param>
        /// <returns>The possible moves given of the rook given the attack variation and the board index</returns>
        private static ulong GetRookPossibleMoves(ulong attackVariation, int boardIndex)
        {
            ulong moves = 0UL;
            int i;
            for (i = boardIndex + 8; i < 64; i += 8)
            {
                moves |= 1UL << i;
                if ((attackVariation & (1UL << i)) != 0) break;
            }
            for (i = boardIndex - 8; i >= 0; i -= 8)
            {
                moves |= 1UL << i;
                if ((attackVariation & (1UL << i)) != 0) break;
            }
            for (i = boardIndex + 1; i%8 != 0; i++)
            {
                moves |= 1UL << i;
                if ((attackVariation & (1UL << i)) != 0) break;
            }
            for (i = boardIndex - 1; (i%8 != 7) && (i >= 0); i--)
            {
                moves |= 1UL << i;
                if ((attackVariation & (1UL << i)) != 0) break;
            }
            return moves;
        }

        /// <summary>
        /// Constructor. Initialize the lookup tables for both the rook and the bishop.
        /// </summary>
        public static void Init()
        {
            PopulateLookupTables();
            PopulateLookupTables(false);
        }

        /// <summary>
        /// Fill the lookup tables with the correct  possible moves.
        /// </summary>
        /// <param name="isRook"></param>
        private static void PopulateLookupTables(bool isRook = true)
        {
            // Calculate the magic numbers based on whether we are looking at a rook or not
            if (!UsePreCalculatedMagicNumbers)
                GenerateMagicNumbers(isRook);
            // Non-zero bits in the attack set
            int bitCountAttackSet;
            // The attack set
            ulong attackSet;
            // All of the variations of the attack set
            List<ulong> attackVariations;
            // Current variation of the attack set
            ulong attackVariation;
            // Current magic number
            ulong magicNumber;
            // The moves given the current attack variation
            ulong moves;
            // The magic index
            ulong magicIndex;
            for (var boardIndex = 0; boardIndex < 64; boardIndex++)
            {
                attackSet = isRook ? GetRookAttackSet(boardIndex) : GetBishopAttackSet(boardIndex);
                magicNumber = isRook ? RookMagicNumbers[boardIndex] : BishopMagicNumbers[boardIndex];

                bitCountAttackSet = Utilities.GetActiveBitIndices(attackSet).Count();
                attackVariations = GetAttackVariations(attackSet, boardIndex);

                for (var variation = 0; variation < attackVariations.Count(); variation++)
                {
                    attackVariation = attackVariations[variation];
                    magicIndex = (attackVariation*magicNumber) >> (64 - bitCountAttackSet);
                    moves = isRook
                        ? GetRookPossibleMoves(attackVariation, boardIndex)
                        : GetBishopPossibleMoves(attackVariation, boardIndex);
                    if (isRook)
                        RookLookupTable[boardIndex, (int) magicIndex] = moves;
                    else
                        BishopLookupTable[boardIndex, (int) magicIndex] = moves;
                }
            }
        }
    }
}