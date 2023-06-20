using System;

namespace Ex05.ConnectFourEngine
{
    public class GameBoard
    {
        private const eCoin k_EmptyCell = eCoin.Empty;
        private readonly int r_Rows;
        private readonly int r_Cols;
        private int m_NumberOfFilledCells;
        private BoardCell[,] m_TheBoard = null;

        public event Action<int> ColumnFilledEventHandler;

        public GameBoard(int i_Rows, int i_Cols)
        {
            this.r_Rows = i_Rows;
            this.r_Cols = i_Cols;
            this.m_NumberOfFilledCells = 0;
            this.m_TheBoard = new BoardCell[this.r_Rows, this.r_Cols];
            this.ClearBoard(true);
        }

        internal void ClearBoard(bool i_IsInitialization)
        {
            for(int i = 0; i < this.r_Rows; i++)
            {
                for(int j = 0; j < this.r_Cols; j++)
                {
                    if(i_IsInitialization)
                    {
                        this.m_TheBoard[i, j] = new BoardCell();
                    }
                    else
                    {
                        this.m_TheBoard[i, j].CurrentSign = k_EmptyCell;
                    }
                }
            }

            this.m_NumberOfFilledCells = 0;
        }

        // i_NotARealPlacementRequest will be true when this method is called only to evaluate the AI's next best move
        internal bool CheckIfMoveOkAndPlaceCoin(Move i_CurrentMove, eCoin i_PlayerSign, out Cell o_PlacementOfCoin, bool i_NotARealPlacementRequest)
        {
            int columnNumberToDrop = i_CurrentMove.ColumnNumber - 1;
            o_PlacementOfCoin = new Cell(0, columnNumberToDrop);
            bool isMoveInBounds = columnNumberToDrop < this.r_Cols && columnNumberToDrop >= 0;
            bool isMovePossible = !IsColumnFull(columnNumberToDrop);
            bool isMoveValid = isMovePossible && isMoveInBounds;

            if (isMoveValid)
            {
                for(int i = this.r_Rows - 1; i >= 0; i--)
                {
                    if(this.m_TheBoard[i, columnNumberToDrop].IsEmpty)
                    {
                        this.m_TheBoard[i, columnNumberToDrop].CurrentSign = i_PlayerSign;
                        this.m_NumberOfFilledCells++;
                        o_PlacementOfCoin.Row = i;
                        if(o_PlacementOfCoin.Row == 0 && !i_NotARealPlacementRequest)
                        {
                            OnColumnFilled(o_PlacementOfCoin.Column + 1);
                        }

                        break;
                    }
                }
            }

            return isMoveValid;
        }

        protected virtual void OnColumnFilled(int i_Col)
        {
            if(ColumnFilledEventHandler != null)
            {
                ColumnFilledEventHandler.Invoke(i_Col);
            }
        }

        internal GameEngine.eGameStatus CheckNewGameStatus(Cell i_LastCellFilled, eCoin i_Player1Sign)
        {
            GameEngine.eGameStatus gameStatus = GameEngine.eGameStatus.InPlay;
            bool didPlayerWin;

            didPlayerWin = checkIfCreatedFourInARow(i_LastCellFilled);
            didPlayerWin |= checkIfCreatedFourInAColumn(i_LastCellFilled);
            didPlayerWin |= checkIfCreatedFourInLeftDiagonal(i_LastCellFilled);
            didPlayerWin |= checkIfCreatedFourInRightDiagonal(i_LastCellFilled);
            if (didPlayerWin)
            {
                gameStatus = this.m_TheBoard[i_LastCellFilled.Row, i_LastCellFilled.Column].CurrentSign == i_Player1Sign
                                 ? GameEngine.eGameStatus.Player1Win
                                 : GameEngine.eGameStatus.Player2Win;
            }
            else if(this.IsBoardFull())
            {
                gameStatus = GameEngine.eGameStatus.Tie;
            }

            return gameStatus;
        }

        private bool checkIfCreatedFourInARow(Cell i_Placement)
        {
            eCoin signToCheck = this.m_TheBoard[i_Placement.Row, i_Placement.Column].CurrentSign;
            bool reachedFourInARow = false;
            int counterInARow = 0;
            int rightBoundColumn = Math.Min(i_Placement.Column + 3, this.r_Cols - 1);
            int leftBoundColumn = Math.Max(i_Placement.Column - 3, 0);
            
            for(int i = leftBoundColumn; i <= rightBoundColumn; i++)
            {
                reachedFourInARow |= incCounterOnMatch(i_Placement.Row, i, signToCheck, ref counterInARow);
            }

            return reachedFourInARow;
        }

        private bool checkIfCreatedFourInAColumn(Cell i_Placement)
        {
            eCoin signToCheck = this.m_TheBoard[i_Placement.Row, i_Placement.Column].CurrentSign;
            bool reachedFourInACol = false;
            int counterInACol = 0;
            int upperBoundRow = Math.Max(i_Placement.Row - 3, 0);
            int lowerBoundRow = Math.Min(i_Placement.Row + 3, this.r_Rows - 1);
            
            for (int i = upperBoundRow; i <= lowerBoundRow; i++)
            {
                reachedFourInACol |= incCounterOnMatch(i, i_Placement.Column, signToCheck, ref counterInACol);
            }

            return reachedFourInACol;
        }

        private bool checkIfCreatedFourInRightDiagonal(Cell i_Placement)
        {
            eCoin signToCheck = this.m_TheBoard[i_Placement.Row, i_Placement.Column].CurrentSign;
            bool reachedFourInDiagonal = false;
            int counterInDiagonal = 0;
            int startPlacementRow = i_Placement.Row - 3;
            int startPlacementCol = i_Placement.Column - 3;
            int rowBound = Math.Min(i_Placement.Row + 3, this.r_Rows - 1);
            int colBound = Math.Min(i_Placement.Column + 3, this.r_Cols - 1);
            
            for(int i = startPlacementRow, j = startPlacementCol; i <= rowBound && j <= colBound; i++, j++)
            {
                if(!isCellOutOfBounds(i, j))
                {
                    reachedFourInDiagonal |= incCounterOnMatch(i, j, signToCheck, ref counterInDiagonal);
                }
            }

            return reachedFourInDiagonal;
        }

        private bool checkIfCreatedFourInLeftDiagonal(Cell i_Placement)
        {
            eCoin signToCheck = this.m_TheBoard[i_Placement.Row, i_Placement.Column].CurrentSign;
            bool reachedFourInDiagonal = false;
            int counterInDiagonal = 0;
            int startPlacementRow = i_Placement.Row - 3;
            int startPlacementCol = i_Placement.Column + 3;
            int rowBound = Math.Min(i_Placement.Row + 3, this.r_Rows - 1);
            int colBound = Math.Max(i_Placement.Column - 3, 0);
           
            for (int i = startPlacementRow, j = startPlacementCol; i <= rowBound && j >= colBound; i++, j--)
            {
                if(!isCellOutOfBounds(i, j))
                {
                    reachedFourInDiagonal |= incCounterOnMatch(i, j, signToCheck, ref counterInDiagonal);
                }
            }

            return reachedFourInDiagonal;
        }

        private bool incCounterOnMatch(int i_CellRow, int i_CellColumn, eCoin i_SignToMatch, ref int io_CounterInARow)
        {
            if (this.m_TheBoard[i_CellRow, i_CellColumn].CurrentSign == i_SignToMatch)
            {
                io_CounterInARow++;
            }
            else
            {
                io_CounterInARow = 0;
            }

            return io_CounterInARow == 4;
        }

        internal bool IsCellOutOfBounds(Cell i_ToCheck)
        {
            return isCellOutOfBounds(i_ToCheck.Row, i_ToCheck.Column);
        }

        private bool isCellOutOfBounds(int i_RowIndex, int i_ColumnIndex)
        {
            return i_RowIndex < 0 || i_RowIndex >= this.r_Rows || i_ColumnIndex < 0 || i_ColumnIndex >= this.r_Cols;
        }

        internal bool IsColumnFull(int i_Column)
        {
            return !this.m_TheBoard[0, i_Column].IsEmpty;
        }

        internal bool IsBoardFull()
        {
            return this.r_Cols * this.r_Rows == this.m_NumberOfFilledCells;
        }

        internal void ClearCell(Cell i_CellToClear)
        {
            this.m_TheBoard[i_CellToClear.Row, i_CellToClear.Column].CurrentSign = k_EmptyCell;
            this.m_NumberOfFilledCells--;
        }

        public BoardCell GetCell(int i_Row, int i_Col)
        {
            return this.m_TheBoard[i_Row, i_Col];
        }

        public int Rows
        {
            get
            {
                return this.r_Rows;
            }
        }

        public int Columns
        {
            get
            {
                return this.r_Cols;
            }
        }

        public eCoin EmptyCell
        {
            get
            {
                return k_EmptyCell;
            }
        }

        public eCoin this[int i_Row, int i_Col]
        {
            get
            {
                return this.m_TheBoard[i_Row, i_Col].CurrentSign;
            }

            set
            {
                this.m_TheBoard[i_Row, i_Col].CurrentSign = value;
            }
        }

        public enum eCoin
        {
            X,
            O,
            Empty,
        }
    }
}
