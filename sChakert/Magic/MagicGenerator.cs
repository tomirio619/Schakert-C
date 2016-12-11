using System;
using System.Collections.Generic;
using System.Linq;

namespace sChakert.Magic
{
    public static class MagicGenerator
    {
        public static UInt64[] rookMagicNumbers = new UInt64[64];
        /*
        Bishop magic numbers
        */
        public static UInt64[] bishopMagicNumbers = new UInt64[64];
        /*
        Rook lookup table
        */
        public static UInt64[,] rookLookupTable = new UInt64[64, 4096];
        /*
        Bishop lookup table
        */
        public static UInt64[,] bishopLookupTable = new UInt64[64, 4096];

        /// <summary>
        /// Generate the attack set of the bishop for a given boardindex.
        /// </summary>
        /// <param name="boardIndex">The board index</param>
        /// <returns>The attack set belonging to the bishop at the given board index.</returns>
        private static UInt64 getBishopAttackSet(int boardIndex)
        {
            UInt64 attackSet = 0UL;
            for (int i = boardIndex + 9; i%8 != 7 && i%8 != 0 && i <= 55; i += 9)
            {
                attackSet |= (1UL << i);
            }
            for (int i = boardIndex - 7; i%8 != 7 && i%8 != 0 && i >= 8; i -= 7)
            {
                attackSet |= (1UL << i);
            }
            for (int i = boardIndex - 9; i%8 != 7 && i%8 != 0 && i >= 8; i -= 9)
            {
                attackSet |= (1UL << i);
            }
            for (int i = boardIndex + 7; i%8 != 7 && i%8 != 0 && i <= 55; i += 7)
            {
                attackSet |= (1UL << i);
            }
            return attackSet;
        }

        /// <summary>
        /// Generate the attack set of the rook for a given board index.
        /// </summary>
        /// <param name="boardIndex">The board index.</param>
        /// <returns>The attack set belonging to the rook at the given board index.</returns>
        private static UInt64 getRookAttackSet(int boardIndex)
        {
            UInt64 attackSet = 0UL;
            for (int i = boardIndex + 8; i <= 55; i += 8)
            {
                attackSet |= (1UL << i);
            }
            for (int i = boardIndex - 8; i >= 8; i -= 8)
            {
                attackSet |= (1UL << i);
            }
            for (int i = boardIndex + 1; i%8 != 7 && i%8 != 0; i++)
            {
                attackSet |= (1UL << i);
            }
            for (int i = boardIndex - 1; i%8 != 7 && i%8 != 0 && i >= 0; i--)
            {
                attackSet |= (1UL << i);
            }
            return attackSet;
        }

        /// <summary>
        /// Generate the correct rook moveset for a given attack variation.
        /// </summary>
        /// <param name="rookAttackVariation">The attack variation.</param>
        /// <param name="boardIndex">The board index.</param>
        /// <returns>The move set belonging to the rook with the given attack variation at the given board index.</returns>
        private static UInt64 getRookMoveSet(UInt64 rookAttackVariation, int boardIndex)
        {
            UInt64 rookMoveSet = 0UL;
            int i;
            for (i = boardIndex + 8; i < 56 && (rookAttackVariation & (1UL << i)) == 0; i += 8)
            {
                // North
                rookMoveSet |= (1UL << i);
            }
            for (i = boardIndex - 8; i > 7 && (rookAttackVariation & (1UL << i)) == 0; i -= 8)
            {
                // South
                rookMoveSet |= (1UL << i);
            }
            for (i = boardIndex + 1; i%8 != 7 && i%8 != 0 && (rookAttackVariation & (1UL << i)) == 0; i += 1)
            {
                // East
                rookMoveSet |= (1UL << i);
            }
            for (i = boardIndex - 1; i%8 != 7 && i%8 != 0 && i >= 0 && (rookAttackVariation & (1UL << i)) == 0; i -= 1)
            {
                // West
                rookMoveSet |= (1UL << i);
            }
            return rookMoveSet;
        }

        /// <summary>
        /// Generate the correct bishop moveset for a given attack variation.
        /// </summary>
        /// <param name="bishopAttackVariation">The attack variation.</param>
        /// <param name="boardIndex">The board index.</param>
        /// <returns>The move set belonging to the bishop with the given attack variation at the given board index.</returns>
        private static UInt64 getBishopMoveSet(UInt64 bishopAttackVariation, int boardIndex)
        {
            UInt64 bishopMoveSet = 0UL;
            int i;
            for (i = boardIndex + 9;
                i%8 != 0 && i%8 != 7 && i < 56 && (bishopAttackVariation & (1UL << i)) == 0;
                i += 9)
            {
                // North east
                bishopMoveSet |= (1UL << i);
            }
            for (i = boardIndex - 7; i%8 != 0 && i%8 != 7 && i > 7 && (bishopAttackVariation & (1UL << i)) == 0; i -= 7)
            {
                // South east
                bishopMoveSet |= (1UL << i);
            }
            for (i = boardIndex + 7;
                i%8 != 0 && i%8 != 7 && i < 56 && (bishopAttackVariation & (1UL << i)) == 0;
                i += 7)
            {
                // North west
                bishopMoveSet |= (1UL << i);
            }
            for (i = boardIndex - 9; i%8 != 0 && i%8 != 7 && i > 7 && (bishopAttackVariation & (1UL << i)) == 0; i -= 9)
            {
                // South west
                bishopMoveSet |= (1UL << i);
            }
            return bishopMoveSet;
        }

        /// <summary>
        /// Get the possible moves for a rook given the attack variation and the board index.
        /// </summary>
        /// <param name="attackVariation">The attack variation.</param>
        /// <param name="boardIndex">The board index.</param>
        /// <returns>The possible moves given of the rook given the attack variation and the board index</returns>
        private static UInt64 getRookPossibleMoves(UInt64 attackVariation, int boardIndex)
        {
            UInt64 moves = 0UL;
            int i;
            for (i = boardIndex + 8; i < 64; i += 8)
            {
                moves |= (1UL << i);
                if ((attackVariation & (1UL << i)) != 0) break;
            }
            for (i = boardIndex - 8; i >= 0; i -= 8)
            {
                moves |= (1UL << i);
                if ((attackVariation & (1UL << i)) != 0) break;
            }
            for (i = boardIndex + 1; i%8 != 0; i++)
            {
                moves |= (1UL << i);
                if ((attackVariation & (1UL << i)) != 0) break;
            }
            for (i = boardIndex - 1; i%8 != 7 && i >= 0; i--)
            {
                moves |= (1UL << i);
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
        private static UInt64 getBishopPossibleMoves(UInt64 attackVariation, int boardIndex)
        {
            UInt64 moves = 0UL;
            int i;
            for (i = boardIndex + 9; i%8 != 0 && i < 64; i += 9)
            {
                moves |= (1UL << i);
                if ((attackVariation & (1UL << i)) != 0) break;
            }
            for (i = boardIndex - 7; i%8 != 0 && i >= 0; i -= 7)
            {
                moves |= (1UL << i);
                if ((attackVariation & (1UL << i)) != 0) break;
            }
            for (i = boardIndex + 7; i%8 != 7 && i < 64; i += 7)
            {
                moves |= (1UL << i);
                if ((attackVariation & (1UL << i)) != 0) break;
            }
            for (i = boardIndex - 9; i%8 != 7 && i >= 0; i -= 9)
            {
                moves |= (1UL << i);
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
        private static List<UInt64> getAttackVariations(UInt64 attackSet, int boardIndex)
        {
            List<UInt64> attackVariations;
            List<int> activeBitIndices = Utilities.GetActiveBitIndices(attackSet);
            int variationCount = (1 << activeBitIndices.Count());
            // The size of the list becomes the variation count
            attackVariations = new List<UInt64>(new UInt64[variationCount]);
            for (int variation = 0; variation < variationCount; variation++)
            {
                List<int> activeBitIndicesInVariation = Utilities.GetActiveBitIndices((UInt64) variation);
                for (int i = 0; i < activeBitIndicesInVariation.Count(); i++)
                {
                    attackVariations[variation] |= (1UL << activeBitIndices[activeBitIndicesInVariation[i]]);
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
        private static void generateMagicNumbers(bool isRook)
        {
            if (isRook)
            {
                Console.WriteLine("Rook magic numbers");
            }
            else
            {
                Console.WriteLine("Bishop magic numbers");
            }
            // Current magic number
            UInt64 magicNumber;
            // Our mapping
            UInt64[] mapping = new UInt64[4096];
            // Whether we found a magic number for the current board index
            bool magicFound;
            // attack set
            UInt64 attackSet;
            // Active bits in attack set
            List<int> activeBitIndicesInAttackSet;
            // Total number of non-zero bits in the attack set
            int bitCountAttackSet;
            // Attack variations
            List<UInt64> attackVariations;
            // Total number of variations
            int variationCount;

            for (int boardIndex = 0; boardIndex < 64; boardIndex++)
            {
                magicFound = false;
                attackSet = isRook ? getRookAttackSet(boardIndex) : getBishopAttackSet(boardIndex);
                activeBitIndicesInAttackSet = Utilities.GetActiveBitIndices(attackSet);
                bitCountAttackSet = activeBitIndicesInAttackSet.Count();
                variationCount = (int) (1UL << bitCountAttackSet);
                attackVariations = getAttackVariations(attackSet, boardIndex);
                do
                {
                    // Clear our previous mapping
                    Array.Clear(mapping, 0, mapping.Length);
                    // Generate a random number that could possibly be a "magic number"
                    magicNumber = Utilities.RandomUint64Fewbits();
                    for (int variation = 0; variation < attackVariations.Count(); variation++)
                    {
                        UInt64 attackVariation = attackVariations[variation];
                        UInt64 magicIndex = ((attackVariation*magicNumber) >> (64 - bitCountAttackSet));
                        UInt64 moveset = isRook
                            ? getRookMoveSet(attackVariation, boardIndex)
                            : getBishopMoveSet(attackVariation, boardIndex);
                        if (mapping[magicIndex] != 0 && mapping[magicIndex] != moveset)
                        {
                            // We found a Collission
                            break;
                        }
                        // No collission, or stored in mapping was 0
                        mapping[magicIndex] = moveset;
                        magicFound = (variation + 1 == variationCount);
                    }
                } while (!magicFound);
                // We found a magic number
                Console.WriteLine(boardIndex + " " + Utilities.toHEX(magicNumber));
                if (isRook)
                {
                    rookMagicNumbers[boardIndex] = magicNumber;
                }
                else
                {
                    bishopMagicNumbers[boardIndex] = magicNumber;
                }
            }
        }

        /// <summary>
        /// Fill the lookup tables with the correct  possible moves.
        /// </summary>
        /// <param name="isRook"></param>
        private static void populateLookupTables(bool isRook = true)
        {
            // Calculate the magic numbers based
            generateMagicNumbers(isRook);
            // Non-zero bits in the attack set
            int bitCountAttackSet;
            // The attack set
            UInt64 attackSet;
            // All of the variations of the attack set
            List<UInt64> attackVariations;
            // Current variation of the attack set
            UInt64 attackVariation;
            // Current magic number
            UInt64 magicNumber;
            // The moves given the current attack variation
            UInt64 moves;
            // The magic index
            UInt64 magicIndex;
            for (int boardIndex = 0; boardIndex < 64; boardIndex++)
            {
                attackSet = isRook ? getRookAttackSet(boardIndex) : getBishopAttackSet(boardIndex);
                magicNumber = isRook ? rookMagicNumbers[boardIndex] : bishopMagicNumbers[boardIndex];

                bitCountAttackSet = Utilities.GetActiveBitIndices(attackSet).Count();
                attackVariations = getAttackVariations(attackSet, boardIndex);

                for (int variation = 0; variation < attackVariations.Count(); variation++)
                {
                    attackVariation = attackVariations[variation];
                    magicIndex = ((attackVariation*magicNumber) >> (64 - bitCountAttackSet));
                    moves = isRook
                        ? getRookPossibleMoves(attackVariation, boardIndex)
                        : getBishopPossibleMoves(attackVariation, boardIndex);
                    if (isRook)
                    {
                        rookLookupTable[boardIndex, magicIndex] = moves;
                    }
                    else
                    {
                        //Bishop
                        bishopLookupTable[boardIndex, magicIndex] = moves;
                    }
                }
            }
        }

        /// <summary>
        /// Constructor. Initialize the lookup tables for both the rook and the bishop.
        /// </summary>
        public static void Init()
        {
            populateLookupTables();
            populateLookupTables(false);
        }
    }
}