using System;
using System.Collections.Generic;

namespace Ex05.ConnectFourEngine
{
    public class SmartComputerPlayer : Player, IArtificialIntelligence
    {
        private const int k_Player = 0;
        private const int k_Opponent = 1;
        private static readonly Random sr_RandomNumber = new Random();

        public SmartComputerPlayer(int i_Id, string i_PlayerName, GameBoard.eCoin i_DesiredSign) 
            : base(i_Id, i_PlayerName, i_DesiredSign)
        {
        }

        Move IArtificialIntelligence.GenerateMoveForPlayer(Player i_Player, Player i_Opponent, GameBoard i_GameBoard)
        {
            Move moveToTake;
            GameBoard.eCoin[] playerSigns = { i_Player.Sign, i_Opponent.Sign };

            moveToTake = generateBestMove(i_GameBoard, playerSigns);

            return moveToTake;
        }

        private Move generateBestMove(GameBoard i_GameBoard, GameBoard.eCoin[] i_PlayerSigns)
        {
            List<Move> allPossibleMoves = new List<Move>(i_GameBoard.Columns);
            
            for(int i = 0; i < i_GameBoard.Columns; i++)
            {
                if(!i_GameBoard.IsColumnFull(i))
                {
                    allPossibleMoves.Add(new Move(i + 1));
                }
            }

            return pickBestMove(i_GameBoard, i_PlayerSigns, allPossibleMoves);
        }

        private Move pickBestMove(GameBoard i_GameBoard, GameBoard.eCoin[] i_PlayerSigns, List<Move> i_AllPossibleMoves)
        {
            int currentMoveScore;
            Move bestMove = new Move(1);
            bestMove.Score = null;

            foreach(Move currentMove in i_AllPossibleMoves)
            {
                currentMoveScore = evaluateMove(i_GameBoard, i_PlayerSigns, currentMove);
                if(bestMove.Score == null || currentMoveScore > bestMove.Score)
                {
                    bestMove = currentMove;
                    bestMove.Score = currentMoveScore;
                }
            }

            return bestMove;
        }

        private int evaluateMove(GameBoard i_GameBoard, GameBoard.eCoin[] i_PlayerSigns, Move i_CurrentMove)
        {
            int currentMoveScore;
            int currentMovePotential;
            int currentMoveDanger;
            bool winConditionMet;
            Cell placementToTryAndPutCoin;

            i_GameBoard.CheckIfMoveOkAndPlaceCoin(i_CurrentMove, i_PlayerSigns[k_Player], out placementToTryAndPutCoin, true);
            GameEngine.eGameStatus newGameStatus = i_GameBoard.CheckNewGameStatus(placementToTryAndPutCoin, i_PlayerSigns[k_Player]);
            winConditionMet = newGameStatus != GameEngine.eGameStatus.InPlay;
            if (winConditionMet)
            {
                currentMoveScore = int.MaxValue;
            }
            else
            {
                currentMovePotential = scanBoardForPotential(i_GameBoard, i_PlayerSigns[k_Player]);
                currentMoveDanger = scanBoardForDanger(i_GameBoard, i_PlayerSigns);
                currentMoveScore = currentMovePotential - currentMoveDanger;
                currentMoveScore += createdThreeWayOpenTrap(i_GameBoard, placementToTryAndPutCoin)
                                        ? int.MaxValue / 2
                                        : 0;
            }

            i_GameBoard.ClearCell(placementToTryAndPutCoin);

            return currentMoveScore;
        }

        private int scanBoardForPotential(GameBoard i_GameBoard, GameBoard.eCoin i_SignToCheck)
        {
            int currentMovePotential = 0;
            Cell currentCell = new Cell(0, 0);

            for (int i = 0; i < i_GameBoard.Rows; i++)
            {
                for (int j = 0; j < i_GameBoard.Columns; j++)
                {
                    if(i_GameBoard[i, j] == i_SignToCheck)
                    {
                        currentCell.Row = i;
                        currentCell.Column = j;
                        currentMovePotential += checkPotentialConnectToRightOfCell(i_GameBoard, currentCell);
                        currentMovePotential += checkPotentialConnectToTopOfCell(i_GameBoard, currentCell);
                        currentMovePotential += checkPotentialConnectToTopRightOfCell(i_GameBoard, currentCell);
                        currentMovePotential += checkPotentialConnectToTopLeftOfCell(i_GameBoard, currentCell);
                    }
                }
            }

            return currentMovePotential;
        }

        private int scanBoardForDanger(GameBoard i_GameBoard, GameBoard.eCoin[] i_PlayerSigns)
        {
            int currentResponseDanger = 0;
            bool winConditionMet;
            Move opponentResponse;
            Cell placement;
            GameEngine.eGameStatus temporaryGameStatus;

            for (int i = 0; i < i_GameBoard.Columns; i++)
            {
                winConditionMet = false;
                if (!i_GameBoard.IsColumnFull(i))
                {
                    opponentResponse = new Move(i + 1);
                    i_GameBoard.CheckIfMoveOkAndPlaceCoin(opponentResponse, i_PlayerSigns[k_Opponent], out placement, true);
                    temporaryGameStatus = i_GameBoard.CheckNewGameStatus(placement, i_PlayerSigns[k_Opponent]);
                    winConditionMet |= temporaryGameStatus != GameEngine.eGameStatus.InPlay;
                    winConditionMet |= createdThreeWayOpenTrap(i_GameBoard, placement);
                    if (winConditionMet)
                    {
                        currentResponseDanger = int.MaxValue;
                        i_GameBoard.ClearCell(placement);
                        break;
                    }

                    currentResponseDanger = scanBoardForPotential(i_GameBoard, i_PlayerSigns[k_Opponent]);
                    currentResponseDanger = Math.Max(currentResponseDanger, currentResponseDanger);
                    i_GameBoard.ClearCell(placement);
                }
            }

            return currentResponseDanger;
        }

        private int checkPotentialConnectToRightOfCell(GameBoard i_GameBoard, Cell i_CellOfMove)
        {
            int numberOfSimilarSignsInFront = 0;
            Cell cellAheadFourSteps = new Cell(i_CellOfMove.Row, i_CellOfMove.Column + 3);
            bool isFourGoingOutOfBounds = i_GameBoard.IsCellOutOfBounds(cellAheadFourSteps);
            GameBoard.eCoin signToCheck = i_GameBoard[i_CellOfMove.Row, i_CellOfMove.Column];

            if (!isFourGoingOutOfBounds)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (i_GameBoard[i_CellOfMove.Row, i_CellOfMove.Column + i] == signToCheck)
                    {
                        numberOfSimilarSignsInFront++;
                    }
                    else if(i_GameBoard[i_CellOfMove.Row, i_CellOfMove.Column + i] != i_GameBoard.EmptyCell)
                    {
                        numberOfSimilarSignsInFront = 0;
                        break;
                    }
                }
            }

            return numberOfSimilarSignsInFront;
        }

        private int checkPotentialConnectToTopOfCell(GameBoard i_GameBoard, Cell i_CellOfMove)
        {
            int numberOfSimilarSignsAbove = 0;
            Cell cellAboveFourSteps = new Cell(i_CellOfMove.Row - 3, i_CellOfMove.Column);
            bool isFourGoingOutOfBounds = i_GameBoard.IsCellOutOfBounds(cellAboveFourSteps);
            GameBoard.eCoin signToCheck = i_GameBoard[i_CellOfMove.Row, i_CellOfMove.Column];

            if (!isFourGoingOutOfBounds)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (i_GameBoard[i_CellOfMove.Row - i, i_CellOfMove.Column] == signToCheck)
                    {
                        numberOfSimilarSignsAbove++;
                    }
                    else if (i_GameBoard[i_CellOfMove.Row - i, i_CellOfMove.Column] != i_GameBoard.EmptyCell)
                    {
                        numberOfSimilarSignsAbove = 0;
                        break;
                    }
                }
            }

            return numberOfSimilarSignsAbove;
        }

        private int checkPotentialConnectToTopRightOfCell(GameBoard i_GameBoard, Cell i_CellOfMove)
        {
            int numberOfSimilarSignsTopRight = 0;
            Cell cellTopRightFourSteps = new Cell(i_CellOfMove.Row - 3, i_CellOfMove.Column + 3);
            bool isFourGoingOutOfBounds = i_GameBoard.IsCellOutOfBounds(cellTopRightFourSteps);
            GameBoard.eCoin signToCheck = i_GameBoard[i_CellOfMove.Row, i_CellOfMove.Column];

            if (!isFourGoingOutOfBounds)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (i_GameBoard[i_CellOfMove.Row - i, i_CellOfMove.Column + i] == signToCheck)
                    {
                        numberOfSimilarSignsTopRight++;
                    }
                    else if (i_GameBoard[i_CellOfMove.Row - i, i_CellOfMove.Column + i] != i_GameBoard.EmptyCell)
                    {
                        numberOfSimilarSignsTopRight = 0;
                        break;
                    }
                }
            }

            return numberOfSimilarSignsTopRight;
        }

        private int checkPotentialConnectToTopLeftOfCell(GameBoard i_GameBoard, Cell i_CellOfMove)
        {
            int numberOfSimilarSignsTopLeft = 0;
            Cell cellTopLeftFourSteps = new Cell(i_CellOfMove.Row - 3, i_CellOfMove.Column - 3);
            bool isFourGoingOutOfBounds = i_GameBoard.IsCellOutOfBounds(cellTopLeftFourSteps);
            GameBoard.eCoin signToCheck = i_GameBoard[i_CellOfMove.Row, i_CellOfMove.Column];

            if (!isFourGoingOutOfBounds)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (i_GameBoard[i_CellOfMove.Row - i, i_CellOfMove.Column - i] == signToCheck)
                    {
                        numberOfSimilarSignsTopLeft++;
                    }
                    else if (i_GameBoard[i_CellOfMove.Row - i, i_CellOfMove.Column - i] != i_GameBoard.EmptyCell)
                    {
                        numberOfSimilarSignsTopLeft = 0;
                        break;
                    }
                }
            }

            return numberOfSimilarSignsTopLeft;
        }

        private bool createdThreeWayOpenTrap(GameBoard i_GameBoard, Cell i_CellOfMove)
        {
            bool openTrapOnField = false;
            openTrapOnField |= createdThreeWayOpenTrapToTheRight(i_GameBoard, i_CellOfMove);
            openTrapOnField |= createdThreeWayOpenTrapToTheLeft(i_GameBoard, i_CellOfMove);
            openTrapOnField |= createdThreeWayOpenTrapInBetween(i_GameBoard, i_CellOfMove);

            return openTrapOnField;
        }

        private bool createdThreeWayOpenTrapToTheRight(GameBoard i_GameBoard, Cell i_CellOfMove)
        {
            bool openTrapOnField = false;
            GameBoard.eCoin signToCheck = i_GameBoard[i_CellOfMove.Row, i_CellOfMove.Column];
            
            if(i_CellOfMove.Column > 0 && i_CellOfMove.Column + 3 < i_GameBoard.Columns)
            {
                openTrapOnField = true;
                openTrapOnField &= i_GameBoard[i_CellOfMove.Row, i_CellOfMove.Column - 1] == i_GameBoard.EmptyCell;
                openTrapOnField &= i_GameBoard[i_CellOfMove.Row, i_CellOfMove.Column + 1] == signToCheck;
                openTrapOnField &= i_GameBoard[i_CellOfMove.Row, i_CellOfMove.Column + 2] == signToCheck;
                openTrapOnField &= i_GameBoard[i_CellOfMove.Row, i_CellOfMove.Column + 3] == i_GameBoard.EmptyCell;
            }

            return openTrapOnField;
        }

        private bool createdThreeWayOpenTrapToTheLeft(GameBoard i_GameBoard, Cell i_CellOfMove)
        {
            bool openTrapOnField = false;
            GameBoard.eCoin signToCheck = i_GameBoard[i_CellOfMove.Row, i_CellOfMove.Column];
            
            if (i_CellOfMove.Column > 2 && i_CellOfMove.Column + 1 < i_GameBoard.Columns)
            {
                openTrapOnField = true;
                openTrapOnField &= i_GameBoard[i_CellOfMove.Row, i_CellOfMove.Column + 1] == i_GameBoard.EmptyCell;
                openTrapOnField &= i_GameBoard[i_CellOfMove.Row, i_CellOfMove.Column - 1] == signToCheck;
                openTrapOnField &= i_GameBoard[i_CellOfMove.Row, i_CellOfMove.Column - 2] == signToCheck;
                openTrapOnField &= i_GameBoard[i_CellOfMove.Row, i_CellOfMove.Column - 3] == i_GameBoard.EmptyCell;
            }

            return openTrapOnField;
        }

        private bool createdThreeWayOpenTrapInBetween(GameBoard i_GameBoard, Cell i_CellOfMove)
        {
            bool openTrapOnField = false;
            GameBoard.eCoin signToCheck = i_GameBoard[i_CellOfMove.Row, i_CellOfMove.Column];
           
            if (i_CellOfMove.Column > 1 && i_CellOfMove.Column + 2 < i_GameBoard.Columns)
            {
                openTrapOnField = true;
                openTrapOnField &= i_GameBoard[i_CellOfMove.Row, i_CellOfMove.Column + 2] == i_GameBoard.EmptyCell;
                openTrapOnField &= i_GameBoard[i_CellOfMove.Row, i_CellOfMove.Column + 1] == signToCheck;
                openTrapOnField &= i_GameBoard[i_CellOfMove.Row, i_CellOfMove.Column - 1] == signToCheck;
                openTrapOnField &= i_GameBoard[i_CellOfMove.Row, i_CellOfMove.Column - 2] == i_GameBoard.EmptyCell;
            }

            return openTrapOnField;
        }
    }
}
