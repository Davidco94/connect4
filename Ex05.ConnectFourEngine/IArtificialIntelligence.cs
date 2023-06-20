using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ex05.ConnectFourEngine
{
    internal interface IArtificialIntelligence
    {
        Move GenerateMoveForPlayer(Player i_Player, Player i_Opponent, GameBoard i_GameBoard);
    }
}
