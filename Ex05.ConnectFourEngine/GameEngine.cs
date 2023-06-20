using System;

namespace Ex05.ConnectFourEngine
{
    public class GameEngine
    {
        private readonly Player r_Player1;
        private readonly Player r_Player2;
        private readonly GameBoard r_GameBoard;
        private bool m_Player1IsCurrentPlayer = true;

        public GameEngine(int i_BoardRows, int i_BoardColumns, Player i_Player1, Player i_Player2)
        {
            this.r_GameBoard = new GameBoard(i_BoardRows, i_BoardColumns);
            this.r_Player1 = i_Player1;
            this.r_Player2 = i_Player2;
        }

        public bool TakeTurnIfValidAndCheckGameOver(int i_ColumnPlayerPick, out eGameStatus o_GameStatusAfterTurn)
        {
            bool turnWasValid;
            Move currentMove = new Move(i_ColumnPlayerPick);
            Move computerMove;
            Cell placementOfCoin;

            if(this.m_Player1IsCurrentPlayer)
            {
                turnWasValid = this.r_GameBoard.CheckIfMoveOkAndPlaceCoin(currentMove, this.r_Player1.Sign, out placementOfCoin, false);
                if(turnWasValid)
                {
                    this.m_Player1IsCurrentPlayer = !this.m_Player1IsCurrentPlayer;
                    if(this.r_Player2 is IArtificialIntelligence && !this.r_GameBoard.IsBoardFull())
                    {
                        o_GameStatusAfterTurn = this.r_GameBoard.CheckNewGameStatus(placementOfCoin, this.r_Player1.Sign);
                        if(o_GameStatusAfterTurn == eGameStatus.InPlay)
                        {
                            computerMove = (r_Player2 as IArtificialIntelligence).GenerateMoveForPlayer(r_Player2, r_Player1, r_GameBoard);
                            this.r_GameBoard.CheckIfMoveOkAndPlaceCoin(
                                computerMove,
                                this.r_Player2.Sign,
                                out placementOfCoin,
                                false);
                            this.m_Player1IsCurrentPlayer = !this.m_Player1IsCurrentPlayer;
                        }
                    }
                }
            }
            else
            {
                turnWasValid = this.r_GameBoard.CheckIfMoveOkAndPlaceCoin(currentMove, this.r_Player2.Sign, out placementOfCoin, false);
                this.m_Player1IsCurrentPlayer = turnWasValid;
            }

            if(turnWasValid)
            {
                o_GameStatusAfterTurn = this.r_GameBoard.CheckNewGameStatus(placementOfCoin, this.r_Player1.Sign);
            }
            else
            {
                o_GameStatusAfterTurn = eGameStatus.InPlay;
            }

            return turnWasValid;
        }

        public void FinishGame(eGameStatus i_LastGameStatus)
        {
            switch(i_LastGameStatus)
            {
                case eGameStatus.Player1Win:
                    this.r_Player1.Wins++;
                    break;
                case eGameStatus.Player2Win:
                    this.r_Player2.Wins++;
                    break;
                case eGameStatus.Tie:
                    this.r_Player1.Wins++;
                    this.r_Player2.Wins++;
                    break;
            }

            this.m_Player1IsCurrentPlayer = true;
            Board.ClearBoard(false);
        }

        public GameBoard Board
        {
            get
            {
                return this.r_GameBoard;
            }
        }

        public Player Player1
        {
            get
            {
                return this.r_Player1;
            }
        }

        public Player Player2
        {
            get
            {
                return this.r_Player2;
            }
        }

        public Player CurrentPlayer
        {
            get
            {
                return this.m_Player1IsCurrentPlayer ? this.r_Player1 : this.r_Player2;
            }
        }

        public enum eGameStatus
        {
            InPlay,
            Player1Win,
            Player2Win,
            Tie,
        }
    }
}
