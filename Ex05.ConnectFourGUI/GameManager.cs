using System;

namespace Ex05.ConnectFourGUI
{
    using System.Windows.Forms;
    using Ex05.ConnectFourEngine;

    internal class GameManager
    {
        private const int k_Player1Id = 1;
        private const int k_Player2Id = 2;
        private GameEngine m_TheGame;
        private SettingsForm m_SettingsForm;
        private ConnectFourForm m_GameForm;

        public void RunGame()
        {
            this.m_SettingsForm = new SettingsForm();

            this.m_SettingsForm.ShowDialog();
            if(this.m_SettingsForm.DialogResult == DialogResult.OK)
            {
                m_TheGame = createFromGameEngineFromSettings();
                startConnectFour();
            }
        }

        private void startConnectFour()
        {
            this.m_GameForm = new ConnectFourForm(this.m_TheGame);
            this.bindEventHandlerForDropButtons();
            this.m_TheGame.Board.ColumnFilledEventHandler += this.GameBoard_ColumnFilled;
            this.m_TheGame.Board.ColumnFilledEventHandler += this.GameBoard_ColumnFilled;
            this.m_GameForm.SwitchCurrentPlayerLabelToBold();
            this.m_GameForm.ShowDialog();
        }

        private GameEngine createFromGameEngineFromSettings()
        {
            Player player1 = new HumanPlayer(k_Player1Id, this.m_SettingsForm.Player1Name, GameBoard.eCoin.X);
            Player player2;

            if(this.m_SettingsForm.IsPlayer2Human)
            {
                player2 = new HumanPlayer(k_Player2Id, this.m_SettingsForm.Player2Name, GameBoard.eCoin.O);
            }
            else
            {
                player2 = new SmartComputerPlayer(k_Player2Id, this.m_SettingsForm.Player2Name, GameBoard.eCoin.O);
            }

            GameEngine theGame = new GameEngine(
                this.m_SettingsForm.BoardRows,
                this.m_SettingsForm.BoardCols,
                player1,
                player2);

            return theGame;
        }

        private void bindEventHandlerForDropButtons()
        {
            foreach(Button currentDropButton in this.m_GameForm.DropButtons)
            {
                currentDropButton.Click += this.DropButton_Click;
            }
        }

        public void DropButton_Click(object i_Sender, EventArgs i_E)
        {
            Button senderAsButton = i_Sender as Button;
            int columnNumber = int.Parse(senderAsButton.Text);

            this.m_TheGame.TakeTurnIfValidAndCheckGameOver(columnNumber, out GameEngine.eGameStatus o_StatusAfterTurn);
            if(o_StatusAfterTurn != GameEngine.eGameStatus.InPlay)
            {
                this.m_GameForm.RequestPlayAgain(o_StatusAfterTurn);
                this.m_TheGame.FinishGame(o_StatusAfterTurn);
            }

            this.m_GameForm.SwitchCurrentPlayerLabelToBold();
        }

        public void GameBoard_ColumnFilled(int i_Column)
        {
            this.m_GameForm.DropButtons[i_Column - 1].Enabled = false;
        }
    }
}
