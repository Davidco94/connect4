using System;

namespace Ex05.ConnectFourEngine
{
    internal struct Move
    {
        private int m_ColumnNumber;
        private int? m_MoveScore;

        public Move(int i_ColumnNumber)
        {
            this.m_ColumnNumber = i_ColumnNumber;
            this.m_MoveScore = null;
        }

        public int ColumnNumber
        {
            get
            {
                return this.m_ColumnNumber;
            }

            set
            {
                if(ColumnNumber >= 1)
                {
                    this.m_ColumnNumber = value;
                }
                else
                {
                    this.m_ColumnNumber = 1; // Exception to come instead when we learn them
                }
            }
        }

        public int? Score
        {
            get
            {
                return this.m_MoveScore;
            }

            set
            {
                this.m_MoveScore = value;
            }
        }
    }
}
