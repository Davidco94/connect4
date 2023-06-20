using System;

namespace Ex05.ConnectFourGUI
{
    using System.Windows.Forms;
    using Ex05.ConnectFourEngine;

    internal partial class BoardButton : Button
    {
        public const int k_InitialSpaceFromLeftEdge = 15;
        public const int k_InitialSpaceFromTopEdge = 50;
        public const int k_SpaceBetweenButtons = 10;
        public const int k_ButtonSize = 50;
        private eConnectFourButtonState m_State;


        public eConnectFourButtonState State
        {
            get { return m_State; }
            set { m_State = value; }
        }


        public BoardButton(int i_Row, int i_Col)
        {
            this.Height = k_ButtonSize;
            this.Width = k_ButtonSize;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            placeButtonOnScreen(i_Row, i_Col);
        }

        private void placeButtonOnScreen(int i_Row, int i_Col)
        {
            this.Left = k_InitialSpaceFromLeftEdge + (i_Col * (k_SpaceBetweenButtons + k_ButtonSize));
            this.Top = k_InitialSpaceFromTopEdge + (i_Row * (k_SpaceBetweenButtons + k_ButtonSize));
        }

        public void BoardCell_GotChanged(BoardCell i_CellChanged)
        {
            GameBoard.eCoin sign = i_CellChanged.CurrentSign;
            parseImg(sign);
            UpdateBackground();

        }

        public void parseImg(GameBoard.eCoin i_Sign)
        {
            if (i_Sign == GameBoard.eCoin.Empty)
                State = eConnectFourButtonState.Empty;
            else if (i_Sign == GameBoard.eCoin.O)
                State = eConnectFourButtonState.RegularBlack;
            else if (i_Sign == GameBoard.eCoin.X)
                State = eConnectFourButtonState.RegularRed;
        }

        public void UpdateBackground()
        {
            switch (m_State)
            {
                case eConnectFourButtonState.RegularBlack:
                    this.BackgroundImage = global::Ex05.ConnectFourGUI.Resources.blackNotSelected;
                    break;
                case eConnectFourButtonState.RegularRed:
                    this.BackgroundImage = global::Ex05.ConnectFourGUI.Resources.RedNotSelected;
                    break;
                case eConnectFourButtonState.Empty:
                    this.BackgroundImage = Ex05.ConnectFourGUI.Resources.empty;
                    break;
                default:
                    this.BackgroundImage = Ex05.ConnectFourGUI.Resources.empty;
                    break;
            }
        }
    }
}
