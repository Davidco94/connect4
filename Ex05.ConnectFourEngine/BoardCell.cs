using System;

namespace Ex05.ConnectFourEngine
{
    public class BoardCell
    {
        private GameBoard.eCoin m_CurrentData;

        public event Action<BoardCell> GotChangedEventHandler;

        public BoardCell()
        {
            this.m_CurrentData = GameBoard.eCoin.Empty;
        }

        protected virtual void OnGotChanged()
        {
            if (GotChangedEventHandler != null)
            {
                GotChangedEventHandler.Invoke(this);
            }
        }

        public GameBoard.eCoin CurrentSign
        {
            get
            {
                return this.m_CurrentData;
            }

            set
            {
                this.m_CurrentData = value;
                this.OnGotChanged();
            }
        }

        public bool IsEmpty
        {
            get
            {
                return this.m_CurrentData == GameBoard.eCoin.Empty;
            }
        }
    }
}
