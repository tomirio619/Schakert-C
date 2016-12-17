using System;
using System.Collections.Generic;
using System.Linq;

namespace sChakert.Magic
{
    public static class MagicGenerator
    {
        public static ulong[] RookMagicNumbers = new ulong[64];
        /*
        Bishop magic numbers
        */
        public static ulong[] BishopMagicNumbers = new ulong[64];
        /*
        Rook lookup table
        */
        public static ulong[,] RookLookupTable = new ulong[64, 4096];
        /*
        Bishop lookup table
        */
        public static ulong[,] BishopLookupTable = new ulong[64, 4096];

        /// <summary>
        /// Generate the attack set of the bishop for a given boardindex.
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
        /// Generate the attack set of the rook for a given board index.
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
        /// Generate the correct rook moveset for a given attack variation.
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
            for (i = boardIndex - 1; (i%8 != 7) && (i%8 != 0) && (i >= 0) && ((rookAttackVariation & (1UL << i)) == 0); i -= 1)
                rookMoveSet |= 1UL << i;
            return rookMoveSet;
        }

        /// <summary>
        /// Generate the correct bishop moveset for a given attack variation.
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
            for (i = boardIndex - 7; (i%8 != 0) && (i%8 != 7) && (i > 7) && ((bishopAttackVariation & (1UL << i)) == 0); i -= 7)
                bishopMoveSet |= 1UL << i;
            for (i = boardIndex + 7;
                (i%8 != 0) && (i%8 != 7) && (i < 56) && ((bishopAttackVariation & (1UL << i)) == 0);
                i += 7)
                bishopMoveSet |= 1UL << i;
            for (i = boardIndex - 9; (i%8 != 0) && (i%8 != 7) && (i > 7) && ((bishopAttackVariation & (1UL << i)) == 0); i -= 9)
                bishopMoveSet |= 1UL << i;
            return bishopMoveSet;
        }

        /// <summary>
        /// Get the possible moves for a rook given the attack variation and the board index.
        /// </summary>
        /// <param name="attackVariation">The attack variation.</param>
        /// <param name="boardIndex">The board index.</param>
        /// <returns>The possible moves given of the rook given the attack variation and the board index</returns>
        private static ulong GetRookPossibleMoves(ulong attackVariation, int boardIndex)
        {
            var moves = 0UL;
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
        /// Get the possible moves for a bishop given the attack variation and the board index.
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
        /// Generate all of the variations from a given attack set.
        /// </summary>
        /// <param name="attackSet">The attack set.</param>
        /// <param name="boardIndex">The board index.</param>
        /// <returns>A vector containing all of the variations for the given attack set.</returns>
        private static List<ulong> GetAttackVariations(ulong attackSet, int boardIndex)
        {
            var activeBitIndices = Utilities.GetActiveBitIndices(attackSet);
            var variationCount = 1 << activeBitIndices.Count();
            // The size of the list becomes the variation count
            List<ulong> attackVariations = new List<ulong>(new ulong[variationCount]);

            for (var variation = 0; variation < variationCount; variation++)
            {
                var activeBitIndicesInVariation = Utilities.GetActiveBitIndices((ulong) variation);
                for (var i = 0; i < activeBitIndicesInVariation.Count(); i++)
                {
                    attackVariations[variation] |= 1UL << activeBitIndices[activeBitIndicesInVariation[i]];
                }
            }
            return attackVariations;
        }

        /// <summary>
        /// Generate the magic numbers based on the piece type provided.
        /// </summary>
        /// <param name="isRook">Whether we want to generate the magic numbers for the rook. If this is false,
        /// the magic numbers for the bishop will be generated.
        /// </param>
        private static void GenerateMagicNumbers(bool isRook)
        {
            if (isRook)
                Console.WriteLine("Rook magic numbers");
            else
                Console.WriteLine("Bishop magic numbers");
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
                Console.WriteLine(boardIndex + " " + Utilities.ToHEX(magicNumber));
                if (isRook)
                    RookMagicNumbers[boardIndex] = magicNumber;
                else
                    BishopMagicNumbers[boardIndex] = magicNumber;
            }
        }

        /// <summary>
        /// Fill the lookup tables with the correct  possible moves.
        /// </summary>
        /// <param name="isRook"></param>
        private static void PopulateLookupTables(bool isRook = true)
        {
            // Calculate the magic numbers based
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
                        RookLookupTable[boardIndex, magicIndex] = moves;
                    else
                        BishopLookupTable[boardIndex, magicIndex] = moves;
                }
            }
        }

        /// <summary>
        /// Constructor. Initialize the lookup tables for both the rook and the bishop.
        /// </summary>
        public static void Init()
        {
            PopulateLookupTables();
            PopulateLookupTables(false);
        }
    }
}