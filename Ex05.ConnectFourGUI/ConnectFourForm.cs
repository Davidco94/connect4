using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Ex05.ConnectFourGUI
{
    using Ex05.ConnectFourEngine;

    public partial class ConnectFourForm : Form
    {
        private const string k_GameTitle = "Connect Four!!";
        private const int k_DropButtonHeight = 17;
        private const int k_DropButtonWidth = BoardButton.k_ButtonSize;
        private readonly GameEngine r_TheGame;
        private readonly List<Button> r_DropButtons;
        private readonly BoardButton[,] r_GameBoardButtons;
        private Label m_LabelPlayer1;
        private Label m_LabelPlayer2;

        public ConnectFourForm(GameEngine i_TheGame)
        {
            this.r_TheGame = i_TheGame;
            this.r_DropButtons = new List<Button>(i_TheGame.Board.Columns);
            this.r_GameBoardButtons = new BoardButton[i_TheGame.Board.Rows, i_TheGame.Board.Columns];

            initializeFormByGameEngine();
        }

        private void initializeFormByGameEngine()
        {
            r_TheGame.Player1.ScoreChangedEventHandler += this.Player_ScoreChanged;
            r_TheGame.Player2.ScoreChangedEventHandler += this.Player_ScoreChanged;
            initializeFormSpreadByGameBoard();
            initializeFormDropButtons();
            initializeFormBoardButtons();
            initializeFormPlayerLabels();
        }

        private void initializeFormSpreadByGameBoard()
        {
            int rows = r_TheGame.Board.Rows;
            int cols = r_TheGame.Board.Columns;
            int width = BoardButton.k_InitialSpaceFromLeftEdge
                        + (cols * BoardButton.k_ButtonSize)
                        + ((cols - 1) * BoardButton.k_SpaceBetweenButtons)
                        + BoardButton.k_InitialSpaceFromLeftEdge;
            int height = (k_DropButtonHeight * 2) 
                         + BoardButton.k_SpaceBetweenButtons
                         + (rows * (BoardButton.k_ButtonSize + BoardButton.k_SpaceBetweenButtons))
                         + BoardButton.k_InitialSpaceFromTopEdge;

            this.ClientSize = new System.Drawing.Size(width, height);
            this.Text = k_GameTitle;
        }

        private void initializeFormDropButtons()
        {
            Button buttonToAdd;
            int buttonTop = k_DropButtonHeight;

            for(int i = 0; i < this.r_DropButtons.Capacity; i++)
            {
                buttonToAdd = new Button();
                buttonToAdd.Left = BoardButton.k_InitialSpaceFromLeftEdge
                                   + (i * (k_DropButtonWidth + BoardButton.k_SpaceBetweenButtons));
                buttonToAdd.Top = buttonTop;
                buttonToAdd.Width = BoardButton.k_ButtonSize;
                buttonToAdd.Height = k_DropButtonHeight;
                buttonToAdd.Text = (i + 1).ToString();
                this.r_DropButtons.Add(buttonToAdd);
                this.Controls.Add(buttonToAdd);
            }
        }

        private void initializeFormBoardButtons()
        {
            BoardButton buttonToAdd;

            for(int i = 0; i < r_TheGame.Board.Rows; i++)
            {
                for(int j = 0; j < r_TheGame.Board.Columns; j++)
                {
                    buttonToAdd = new BoardButton(i, j);
                    r_TheGame.Board.GetCell(i, j).GotChangedEventHandler += buttonToAdd.BoardCell_GotChanged;
                    this.r_GameBoardButtons[i, j] = buttonToAdd;
                    this.Controls.Add(buttonToAdd);
                }
            }
        }

        private void initializeFormPlayerLabels()
        {
            BoardButton lowestButton = this.r_GameBoardButtons[this.r_GameBoardButtons.GetLength(0) - 1, 0];
            int middleOfForm = this.ClientSize.Width / 2;

            this.m_LabelPlayer1 = new Label();
            updatePlayerLabelByScore(r_TheGame.Player1, this.m_LabelPlayer1);
            this.m_LabelPlayer1.Width = middleOfForm;
            this.m_LabelPlayer1.TextAlign = ContentAlignment.MiddleCenter;
            this.m_LabelPlayer1.Top = lowestButton.Bottom + (BoardButton.k_ButtonSize / 2);
            this.m_LabelPlayer1.Left = 0;
            this.Controls.Add(this.m_LabelPlayer1);
            this.m_LabelPlayer2 = new Label();
            updatePlayerLabelByScore(r_TheGame.Player2, this.m_LabelPlayer2);
            this.m_LabelPlayer2.Width = middleOfForm;
            this.m_LabelPlayer2.TextAlign = ContentAlignment.MiddleCenter;
            this.m_LabelPlayer2.Top = lowestButton.Bottom + (BoardButton.k_ButtonSize / 2);
            this.m_LabelPlayer2.Left = middleOfForm;
            this.Controls.Add(this.m_LabelPlayer2);
        }

        private void updatePlayerLabelByScore(Player i_Player, Label i_Label)
        {
            i_Label.Text = string.Format("{0}: {1}", i_Player.Name, i_Player.Wins);
        }

        private void enableAllDropButtons()
        {
            foreach (Button currentDropButton in DropButtons)
            {
                currentDropButton.Enabled = true;
            }
        }

        public void Player_ScoreChanged(Player i_ThePlayer)
        {
            if(i_ThePlayer.ID == 1)
            {
                updatePlayerLabelByScore(i_ThePlayer, this.m_LabelPlayer1);
            }
            else
            {
                updatePlayerLabelByScore(i_ThePlayer, this.m_LabelPlayer2);
            }
        }

        public void RequestPlayAgain(GameEngine.eGameStatus i_HowTheGameFinished)
        {
            string headerForPlayers;
            string messageForPlayers;
            bool didPlayer1Win = i_HowTheGameFinished == GameEngine.eGameStatus.Player1Win;

            if (i_HowTheGameFinished == GameEngine.eGameStatus.Tie)
            {
                headerForPlayers = "A Tie!";
                messageForPlayers = @"Tie!!
Another Round?";
            }
            else
            {
                headerForPlayers = "A Win!";
                messageForPlayers = string.Format(
                    @"{0} Won!!
Another Round?",
                    didPlayer1Win ? r_TheGame.Player1.Name : r_TheGame.Player2.Name);
            }

            if (MessageBox.Show(messageForPlayers, headerForPlayers, MessageBoxButtons.YesNo, MessageBoxIcon.Information)
                == DialogResult.No)
            {
                this.Close();
            }
            else
            {
                enableAllDropButtons();
            }
        }

        public void SwitchCurrentPlayerLabelToBold()
        {
            if(this.r_TheGame.CurrentPlayer == this.r_TheGame.Player1)
            {
                this.m_LabelPlayer1.Font = new Font(this.m_LabelPlayer1.Font, FontStyle.Bold);
                this.m_LabelPlayer2.Font = new Font(this.m_LabelPlayer1.Font, FontStyle.Regular);
            }
            else
            {
                this.m_LabelPlayer1.Font = new Font(this.m_LabelPlayer1.Font, FontStyle.Regular);
                this.m_LabelPlayer2.Font = new Font(this.m_LabelPlayer1.Font, FontStyle.Bold);
            }
        }

        public List<Button> DropButtons
        {
            get
            {
                return this.r_DropButtons;
            }
        }
    }
}
