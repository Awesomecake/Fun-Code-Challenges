using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BoardGames
{
    class ChessPiece
    {
        Texture2D texture;
        public Rectangle rect;
        public TeamColor teamColor;
        public ChessPieceType pieceType;

        Rectangle sourceRect = new Rectangle(0,0,133,133);

        //direction of movement for Pawns
        public int dir = 1;

        public bool hasMovedBefore;

        public bool canBeEnPassanted = false;
        private Vector2 enPassantEnemyPawnLoc;

        public List<Vector2> possibleMoves;

        public bool isCheckingTheKing = false;

        public ChessPiece(Rectangle rect, TeamColor color, ChessPieceType type)
        {
            texture = Game1.chessTextures;
            pieceType = type;

            if (type == ChessPieceType.Pawn)
            {
                sourceRect.X = 667;
                hasMovedBefore = false;
            }
            else if (type == ChessPieceType.Rook)
            {
                sourceRect.X = 533;
            }
            else if (type == ChessPieceType.Knight)
            {
                sourceRect.X = 400;
            }
            else if (type == ChessPieceType.Bishop)
            {
                sourceRect.X = 267;
            }
            else if (type == ChessPieceType.Queen)
            {
                sourceRect.X = 133;
            }
            else if (type == ChessPieceType.King)
            {
                sourceRect.X = 0;
            }

            if (color == TeamColor.Black)
            {
                sourceRect.Y = 133;
                dir = -1;
            }

            this.rect = rect;
            teamColor = color;

            possibleMoves = new List<Vector2>();
        }

        public void ChessPieceUpdate(int x, int y, ChessPiece[,] chessBoard)
        {
            possibleMoves.Clear();
            enPassantEnemyPawnLoc = new Vector2(-1,-1);

            #region Pawn
            if (pieceType == ChessPieceType.Pawn)
            {
                if (InRange(x, y - 1 * dir) && chessBoard[x, y - 1 * dir] == null)
                {
                    possibleMoves.Add(new Vector2(x, y - 1 * dir));

                    if (!hasMovedBefore && InRange(x, y - 2 * dir) && chessBoard[x, y - 2 * dir] == null)
                    {
                        possibleMoves.Add(new Vector2(x, y - 2 * dir));
                    }
                }

                if (InRange(x - 1, y - 1 * dir) && chessBoard[x - 1, y - 1 * dir] != null && chessBoard[x - 1, y - 1 * dir].teamColor != teamColor)
                {
                    possibleMoves.Add(new Vector2(x - 1, y - 1 * dir));
                }
                if (InRange(x + 1, y - 1 * dir) && chessBoard[x + 1, y - 1 * dir] != null && chessBoard[x + 1, y - 1 * dir].teamColor != teamColor)
                {
                    possibleMoves.Add(new Vector2(x + 1, y - 1 * dir));
                }

                //En Passant Movement Checking               
                if (InRange(x - 1, y) && chessBoard[x - 1, y ] != null && chessBoard[x - 1, y].canBeEnPassanted && chessBoard[x - 1, y ].teamColor != teamColor)
                {
                    enPassantEnemyPawnLoc = new Vector2(x - 1, y);
                    possibleMoves.Add(new Vector2(x - 1, y - 1 * dir));
                }
                if (InRange(x + 1, y ) && chessBoard[x + 1, y ] != null && chessBoard[x + 1, y].canBeEnPassanted && chessBoard[x + 1, y].teamColor != teamColor)
                {
                    enPassantEnemyPawnLoc = new Vector2(x + 1, y);
                    possibleMoves.Add(new Vector2(x + 1, y - 1 * dir));
                }
            }
            #endregion

            #region Rook
            if (pieceType == ChessPieceType.Rook)
            {
                for (int i = y + 1; i < 8; i++)
                {
                    if (chessBoard[x, i] == null)
                    {
                        possibleMoves.Add(new Vector2(x, i));
                    }
                    else if (chessBoard[x, i] != null && chessBoard[x, i].teamColor != teamColor)
                    {
                        possibleMoves.Add(new Vector2(x, i));
                        break;
                    }
                    else if (chessBoard[x, i] != null && chessBoard[x, i].teamColor == teamColor)
                    {
                        break;
                    }
                }

                for (int i = y - 1; i >= 0; i--)
                {
                    if (chessBoard[x, i] == null)
                    {
                        possibleMoves.Add(new Vector2(x, i));
                    }
                    else if (chessBoard[x, i] != null && chessBoard[x, i].teamColor != teamColor)
                    {
                        possibleMoves.Add(new Vector2(x, i));
                        break;
                    }
                    else if (chessBoard[x, i] != null && chessBoard[x, i].teamColor == teamColor)
                    {
                        break;
                    }
                }

                for (int i = x - 1; i >= 0; i--)
                {
                    if (chessBoard[i, y] == null)
                    {
                        possibleMoves.Add(new Vector2(i, y));
                    }
                    else if (chessBoard[i, y] != null && chessBoard[i, y].teamColor != teamColor)
                    {
                        possibleMoves.Add(new Vector2(i, y));
                        break;
                    }
                    else if (chessBoard[i, y] != null && chessBoard[i, y].teamColor == teamColor)
                    {
                        break;
                    }
                }

                for (int i = x + 1; i < 8; i++)
                {
                    if (chessBoard[i, y] == null)
                    {
                        possibleMoves.Add(new Vector2(i, y));
                    }
                    else if (chessBoard[i, y] != null && chessBoard[i, y].teamColor != teamColor)
                    {
                        possibleMoves.Add(new Vector2(i, y));
                        break;
                    }
                    else if (chessBoard[i, y] != null && chessBoard[i, y].teamColor == teamColor)
                    {
                        break;
                    }
                }
            }
            #endregion

            #region Bishop
            if (pieceType == ChessPieceType.Bishop)
            {
                for (int i = 1; i < 8; i++)
                {
                    if (!InRange(x + i, y + i))
                    {
                        break;
                    }

                    if (chessBoard[x + i, y + i] == null)
                    {
                        possibleMoves.Add(new Vector2(x + i, y + i));
                    }
                    else if (chessBoard[x + i, y + i] != null && chessBoard[x + i, y + i].teamColor != teamColor)
                    {
                        possibleMoves.Add(new Vector2(x + i, y + i));
                        break;
                    }
                    else if (chessBoard[x + i, y + i] != null && chessBoard[x + i, y + i].teamColor == teamColor)
                    {
                        break;
                    }
                }

                for (int i = 1; i < 8; i++)
                {
                    if (!InRange(x + i, y - i))
                    {
                        break;
                    }

                    if (chessBoard[x + i, y - i] == null)
                    {
                        possibleMoves.Add(new Vector2(x + i, y - i));
                    }
                    else if (chessBoard[x + i, y - i] != null && chessBoard[x + i, y - i].teamColor != teamColor)
                    {
                        possibleMoves.Add(new Vector2(x + i, y - i));
                        break;
                    }
                    else if (chessBoard[x + i, y - i] != null && chessBoard[x + i, y - i].teamColor == teamColor)
                    {
                        break;
                    }
                }

                for (int i = 1; i < 8; i++)
                {
                    if (!InRange(x - i, y + i))
                    {
                        break;
                    }

                    if (chessBoard[x - i, y + i] == null)
                    {
                        possibleMoves.Add(new Vector2(x - i, y + i));
                    }
                    else if (chessBoard[x - i, y + i] != null && chessBoard[x - i, y + i].teamColor != teamColor)
                    {
                        possibleMoves.Add(new Vector2(x - i, y + i));
                        break;
                    }
                    else if (chessBoard[x - i, y + i] != null && chessBoard[x - i, y + i].teamColor == teamColor)
                    {
                        break;
                    }
                }

                for (int i = 1; i < 8; i++)
                {
                    if (!InRange(x - i, y - i))
                    {
                        break;
                    }

                    if (chessBoard[x - i, y - i] == null)
                    {
                        possibleMoves.Add(new Vector2(x - i, y - i));
                    }
                    else if (chessBoard[x - i, y - i] != null && chessBoard[x - i, y - i].teamColor != teamColor)
                    {
                        possibleMoves.Add(new Vector2(x - i, y - i));
                        break;
                    }
                    else if (chessBoard[x - i, y - i] != null && chessBoard[x - i, y - i].teamColor == teamColor)
                    {
                        break;
                    }
                }
            }
            #endregion

            #region Knight
            if (pieceType == ChessPieceType.Knight)
            {
                if (InRange(x + 1, y - 2) && (chessBoard[x + 1, y - 2] == null || chessBoard[x + 1, y - 2].teamColor != teamColor))
                {
                    possibleMoves.Add(new Vector2(x + 1, y - 2));
                }

                if (InRange(x - 1, y - 2) && (chessBoard[x - 1, y - 2] == null || chessBoard[x - 1, y - 2].teamColor != teamColor))
                {
                    possibleMoves.Add(new Vector2(x - 1, y - 2));
                }

                if (InRange(x + 1, y + 2) && (chessBoard[x + 1, y + 2] == null || chessBoard[x + 1, y + 2].teamColor != teamColor))
                {
                    possibleMoves.Add(new Vector2(x + 1, y + 2));
                }

                if (InRange(x - 1, y + 2) && (chessBoard[x - 1, y + 2] == null || chessBoard[x - 1, y + 2].teamColor != teamColor))
                {
                    possibleMoves.Add(new Vector2(x - 1, y + 2));
                }

                if (InRange(x + 2, y - 1) && (chessBoard[x + 2, y - 1] == null || chessBoard[x + 2, y - 1].teamColor != teamColor))
                {
                    possibleMoves.Add(new Vector2(x + 2, y - 1));
                }

                if (InRange(x - 2, y - 1) && (chessBoard[x - 2, y - 1] == null || chessBoard[x - 2, y - 1].teamColor != teamColor))
                {
                    possibleMoves.Add(new Vector2(x - 2, y - 1));
                }

                if (InRange(x + 2, y + 1) && (chessBoard[x + 2, y + 1] == null || chessBoard[x + 2, y + 1].teamColor != teamColor))
                {
                    possibleMoves.Add(new Vector2(x + 2, y + 1));
                }

                if (InRange(x - 2, y + 1) && (chessBoard[x - 2, y + 1] == null || chessBoard[x - 2, y + 1].teamColor != teamColor))
                {
                    possibleMoves.Add(new Vector2(x - 2, y + 1));
                }
            }
            #endregion

            #region Queen
            if (pieceType == ChessPieceType.Queen)
            {
                for (int i = 1; i < 8; i++)
                {
                    if (!InRange(x + i, y + i))
                    {
                        break;
                    }

                    if (chessBoard[x + i, y + i] == null)
                    {
                        possibleMoves.Add(new Vector2(x + i, y + i));
                    }
                    else if (chessBoard[x + i, y + i] != null && chessBoard[x + i, y + i].teamColor != teamColor)
                    {
                        possibleMoves.Add(new Vector2(x + i, y + i));
                        break;
                    }
                    else if (chessBoard[x + i, y + i] != null && chessBoard[x + i, y + i].teamColor == teamColor)
                    {
                        break;
                    }
                }

                for (int i = 1; i < 8; i++)
                {
                    if (!InRange(x + i, y - i))
                    {
                        break;
                    }

                    if (chessBoard[x + i, y - i] == null)
                    {
                        possibleMoves.Add(new Vector2(x + i, y - i));
                    }
                    else if (chessBoard[x + i, y - i] != null && chessBoard[x + i, y - i].teamColor != teamColor)
                    {
                        possibleMoves.Add(new Vector2(x + i, y - i));
                        break;
                    }
                    else if (chessBoard[x + i, y - i] != null && chessBoard[x + i, y - i].teamColor == teamColor)
                    {
                        break;
                    }
                }

                for (int i = 1; i < 8; i++)
                {
                    if (!InRange(x - i, y + i))
                    {
                        break;
                    }

                    if (chessBoard[x - i, y + i] == null)
                    {
                        possibleMoves.Add(new Vector2(x - i, y + i));
                    }
                    else if (chessBoard[x - i, y + i] != null && chessBoard[x - i, y + i].teamColor != teamColor)
                    {
                        possibleMoves.Add(new Vector2(x - i, y + i));
                        break;
                    }
                    else if (chessBoard[x - i, y + i] != null && chessBoard[x - i, y + i].teamColor == teamColor)
                    {
                        break;
                    }
                }

                for (int i = 1; i < 8; i++)
                {
                    if (!InRange(x - i, y - i))
                    {
                        break;
                    }

                    if (chessBoard[x - i, y - i] == null)
                    {
                        possibleMoves.Add(new Vector2(x - i, y - i));
                    }
                    else if (chessBoard[x - i, y - i] != null && chessBoard[x - i, y - i].teamColor != teamColor)
                    {
                        possibleMoves.Add(new Vector2(x - i, y - i));
                        break;
                    }
                    else if (chessBoard[x - i, y - i] != null && chessBoard[x - i, y - i].teamColor == teamColor)
                    {
                        break;
                    }
                }

                for (int i = y + 1; i < 8; i++)
                {
                    if (chessBoard[x, i] == null)
                    {
                        possibleMoves.Add(new Vector2(x, i));
                    }
                    else if (chessBoard[x, i] != null && chessBoard[x, i].teamColor != teamColor)
                    {
                        possibleMoves.Add(new Vector2(x, i));
                        break;
                    }
                    else if (chessBoard[x, i] != null && chessBoard[x, i].teamColor == teamColor)
                    {
                        break;
                    }
                }

                for (int i = y - 1; i >= 0; i--)
                {
                    if (chessBoard[x, i] == null)
                    {
                        possibleMoves.Add(new Vector2(x, i));
                    }
                    else if (chessBoard[x, i] != null && chessBoard[x, i].teamColor != teamColor)
                    {
                        possibleMoves.Add(new Vector2(x, i));
                        break;
                    }
                    else if (chessBoard[x, i] != null && chessBoard[x, i].teamColor == teamColor)
                    {
                        break;
                    }
                }

                for (int i = x - 1; i >= 0; i--)
                {
                    if (chessBoard[i, y] == null)
                    {
                        possibleMoves.Add(new Vector2(i, y));
                    }
                    else if (chessBoard[i, y] != null && chessBoard[i, y].teamColor != teamColor)
                    {
                        possibleMoves.Add(new Vector2(i, y));
                        break;
                    }
                    else if (chessBoard[i, y] != null && chessBoard[i, y].teamColor == teamColor)
                    {
                        break;
                    }
                }

                for (int i = x + 1; i < 8; i++)
                {
                    if (chessBoard[i, y] == null)
                    {
                        possibleMoves.Add(new Vector2(i, y));
                    }
                    else if (chessBoard[i, y] != null && chessBoard[i, y].teamColor != teamColor)
                    {
                        possibleMoves.Add(new Vector2(i, y));
                        break;
                    }
                    else if (chessBoard[i, y] != null && chessBoard[i, y].teamColor == teamColor)
                    {
                        break;
                    }
                }
            }
            #endregion

            #region King
            if (pieceType == ChessPieceType.King)
            {
                if (InRange(x, y - 1) && (chessBoard[x, y - 1] == null || chessBoard[x , y - 1].teamColor != teamColor))
                {
                    possibleMoves.Add(new Vector2(x , y - 1));
                }

                if (InRange(x, y + 1) && (chessBoard[x, y + 1] == null || chessBoard[x, y + 1].teamColor != teamColor))
                {
                    possibleMoves.Add(new Vector2(x, y + 1));
                }

                if (InRange(x - 1, y) && (chessBoard[x-1, y ] == null || chessBoard[x-1, y ].teamColor != teamColor))
                {
                    possibleMoves.Add(new Vector2(x-1, y ));
                }

                if (InRange(x + 1, y) && (chessBoard[x + 1, y] == null || chessBoard[x + 1, y].teamColor != teamColor))
                {
                    possibleMoves.Add(new Vector2(x + 1, y));
                }

                if (InRange(x-1, y - 1) && (chessBoard[x-1, y - 1] == null || chessBoard[x-1, y - 1].teamColor != teamColor))
                {
                    possibleMoves.Add(new Vector2(x-1, y - 1));
                }

                if (InRange(x-1, y + 1) && (chessBoard[x-1, y + 1] == null || chessBoard[x-1, y + 1].teamColor != teamColor))
                {
                    possibleMoves.Add(new Vector2(x-1, y + 1));
                }

                if (InRange(x+1, y - 1) && (chessBoard[x+1, y - 1] == null || chessBoard[x+1, y - 1].teamColor != teamColor))
                {
                    possibleMoves.Add(new Vector2(x+1, y - 1));
                }

                if (InRange(x+1, y + 1) && (chessBoard[x+1, y + 1] == null || chessBoard[x+1, y + 1].teamColor != teamColor))
                {
                    possibleMoves.Add(new Vector2(x+1, y + 1));
                }

                //Checking if King could Castle
                //Basically Hardcoded - Could Cause issues in nonstandard situations
                if (!hasMovedBefore && InRange(x+3,y) && chessBoard[x + 3, y] != null && chessBoard[x + 3, y].pieceType == ChessPieceType.Rook)
                {
                    if (chessBoard[x + 3, y].teamColor == teamColor && !chessBoard[x + 3, y].hasMovedBefore)
                    {
                        if (chessBoard[x + 1, y] == null && chessBoard[x + 2, y] == null)
                        {
                            possibleMoves.Add(new Vector2(x + 2, y));
                        }
                    }
                }

                if (!hasMovedBefore && InRange(x - 4, y) && chessBoard[x - 4, y] != null && chessBoard[x - 4, y].pieceType == ChessPieceType.Rook)
                {
                    if (chessBoard[x - 4, y].teamColor == teamColor && !chessBoard[x - 4, y].hasMovedBefore)
                    {
                        if (chessBoard[x - 3, y] == null && chessBoard[x - 2, y] == null && chessBoard[x - 1, y] == null)
                        {
                            possibleMoves.Add(new Vector2(x - 2, y));
                        }
                    }
                }
            }
            #endregion
        }

        public void MovePiece(Tile currentPos, Tile futurePos, ChessPiece[,] chessPieces)
        {
            //Checks for if En Passant can happen, and if it will happen
            if(pieceType == ChessPieceType.Pawn && enPassantEnemyPawnLoc != new Vector2(-1,-1))
            {
                //If En Passant is happening
                if (enPassantEnemyPawnLoc.X == futurePos.xPos && enPassantEnemyPawnLoc.Y == futurePos.yPos + 1 * dir)
                {
                    chessPieces[futurePos.xPos, futurePos.yPos] = chessPieces[currentPos.xPos, currentPos.yPos];
                    rect = new Rectangle(futurePos.xPos * 100, futurePos.yPos * 100, 100, 100);

                    chessPieces[currentPos.xPos, currentPos.yPos] = null;
                    chessPieces[(int)enPassantEnemyPawnLoc.X, (int)enPassantEnemyPawnLoc.Y] = null;

                    ChessPieceUpdate(futurePos.xPos, futurePos.yPos, chessPieces);
                }

                //Not passant
                else
                {
                    chessPieces[futurePos.xPos, futurePos.yPos] = chessPieces[currentPos.xPos, currentPos.yPos];
                    ChessPieceUpdate(futurePos.xPos, futurePos.yPos, chessPieces);
                    rect = new Rectangle(futurePos.xPos * 100, futurePos.yPos * 100, 100, 100);

                    chessPieces[currentPos.xPos, currentPos.yPos] = null;
                }
            }
            else
            {
                chessPieces[futurePos.xPos, futurePos.yPos] = chessPieces[currentPos.xPos, currentPos.yPos];
                ChessPieceUpdate(futurePos.xPos, futurePos.yPos, chessPieces);
                rect = new Rectangle(futurePos.xPos * 100, futurePos.yPos * 100, 100, 100);

                chessPieces[currentPos.xPos, currentPos.yPos] = null;
            }

            if (!hasMovedBefore)
            {
                hasMovedBefore = true;

                //If first move was a two space move - It can be En Passanted
                if(pieceType == ChessPieceType.Pawn && currentPos.yPos == futurePos.yPos + 2*dir)
                {
                    canBeEnPassanted = true;
                }
            }
        }

        public void MovePiece(Tile currentPos, Vector2 futurePos, ChessPiece[,] chessPieces)
        {
            MovePiece(currentPos, new Tile(null, (int)futurePos.X, (int)futurePos.Y), chessPieces);
        }

        public void TurnChanged()
        {
            canBeEnPassanted = false;
            isCheckingTheKing = false;
        }

        public bool InRange(int x, int y)
        {
            if (x < 8 && y >= 0 && y < 8 && x >= 0)
            {
                return true;
            }
            return false;
        }

        public void Draw(SpriteBatch sb)
        {
            if (isCheckingTheKing)
            {
                sb.Draw(texture, rect, sourceRect, Color.Red);
            }
            else
            {
                sb.Draw(texture, rect, sourceRect, Color.White);
            }
        }
    }
}
