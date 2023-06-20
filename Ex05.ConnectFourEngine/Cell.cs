using System;

namespace Ex05.ConnectFourEngine
{
    internal struct Cell
    {
        private int m_Row;
        private int m_Col;

        public Cell(int i_X, int i_Y)
        {
            this.m_Row = i_X;
            this.m_Col = i_Y;
        }

        public int Row
        {
            get
            {
                return m_Row;
            }

            set
            {
                m_Row = value;
            }
        }

        public int Column
        {
            get
            {
                return this.m_Col;
            }

            set
            {
                this.m_Col = value;
            }
        }
    }
}
