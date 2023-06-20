using System;

namespace Ex05.ConnectFourEngine
{
    public class HumanPlayer : Player
    {
        public HumanPlayer(int i_Id, string i_PlayerName, GameBoard.eCoin i_DesiredSign) 
            : base(i_Id, i_PlayerName, i_DesiredSign)
        {
        }
    }
}
