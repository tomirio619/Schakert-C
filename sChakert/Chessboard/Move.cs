namespace sChakert.Chessboard
{
    public static class Move
    {
        public static int EncodeMove(int fromBoardIndex, int toBoardIndex, Type pieceType)
        {
            // TODO implement this
            /*
             Moves are encoded as follows:
             Quiet moves:  0       
             Double pawn push: 1
             King castle: 2
             Queen castle: 3          
             Captures: 4
             EnPassant capture: 5
             Knight-promotion: 6
             Bishop-promotion: 7
             Rook-promotion: 8
             Queen-promotion: 9
             Knight-promo capture: 10
             Bishop-promo capture: 11
             Rook-promo capture: 12
             Queen-promo capture: 13
            */
            return -1;
        }
    }
}