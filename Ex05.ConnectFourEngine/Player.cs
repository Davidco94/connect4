using System;

namespace Ex05.ConnectFourEngine
{
    public abstract class Player
    {
        private readonly int r_PlayerId;
        private readonly string r_Name;
        private readonly GameBoard.eCoin r_PlayerSign;
        private int m_NumberOfWins;

        public event Action<Player> ScoreChangedEventHandler;

        public Player(int i_Id, string i_PlayerName, GameBoard.eCoin i_DesiredSign)
        {
            this.r_PlayerId = i_Id;
            this.r_Name = i_PlayerName;
            this.r_PlayerSign = i_DesiredSign;
            this.m_NumberOfWins = 0;
        }

        public GameBoard.eCoin Sign
        {
            get
            {
                return this.r_PlayerSign;
            }
        }

        public int ID
        {
            get
            {
                return this.r_PlayerId;
            }
        }

        public string Name
        {
            get
            {
                return this.r_Name;
            }
        }

        public int Wins
        {
            get
            {
                return this.m_NumberOfWins;
            }

            set
            {
                this.m_NumberOfWins = value;
                OnScoreChanged();
            }
        }

        public override string ToString()
        {
            return Name;
        }

        protected virtual void OnScoreChanged()
        {
            if (ScoreChangedEventHandler != null)
            {
                ScoreChangedEventHandler.Invoke(this);
            }
        }
    }
}
