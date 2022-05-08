using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BoardGames
{
    class ChessManager
    {
        List<Tile> tiles;

        List<Tile> lastTurnMoves;

        Tile clickedTile;

        MouseState prevMouseState;

        ChessPiece[,] chessPieces;

        TeamColor currentColor = TeamColor.White;

        bool gameOver = false;

        public ChessManager()
        {
            tiles = new List<Tile>();
            lastTurnMoves = new List<Tile>();

            chessPieces = new ChessPiece[8, 8];

            //Placing Pawns
            for (int i = 0; i < 8; i++)
            {
                AddChessPiece(i, 1, ChessPieceType.Pawn, TeamColor.Black);
                AddChessPiece(i, 6, ChessPieceType.Pawn, TeamColor.White);
            }

            //Setting the Board for white pieces
            AddChessPiece(3, 7, ChessPieceType.Queen, TeamColor.White);
            AddChessPiece(4, 7, ChessPieceType.King, TeamColor.White);

            AddChessPiece(5, 7, ChessPieceType.Bishop, TeamColor.White);
            AddChessPiece(2, 7, ChessPieceType.Bishop, TeamColor.White);

            AddChessPiece(6, 7, ChessPieceType.Knight, TeamColor.White);
            AddChessPiece(1, 7, ChessPieceType.Knight, TeamColor.White);

            AddChessPiece(7, 7, ChessPieceType.Rook, TeamColor.White);
            AddChessPiece(0, 7, ChessPieceType.Rook, TeamColor.White);

            //Setting the Board for black pieces
            AddChessPiece(3, 0, ChessPieceType.Queen, TeamColor.Black);
            AddChessPiece(4, 0, ChessPieceType.King, TeamColor.Black);

            AddChessPiece(5, 0, ChessPieceType.Bishop, TeamColor.Black);
            AddChessPiece(2, 0, ChessPieceType.Bishop, TeamColor.Black);

            AddChessPiece(6, 0, ChessPieceType.Knight, TeamColor.Black);
            AddChessPiece(1, 0, ChessPieceType.Knight, TeamColor.Black);

            AddChessPiece(7, 0, ChessPieceType.Rook, TeamColor.Black);
            AddChessPiece(0, 0, ChessPieceType.Rook, TeamColor.Black);



            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((i + j) % 2 == 1)
                    {
                        tiles.Add(new Tile(Game1.whiteTile, i, j));
                    }
                    else
                    {
                        tiles.Add(new Tile(Game1.grayTile, i, j));
                    }
                }
            }
        }

        public void AddChessPiece(int x, int y, ChessPieceType type, TeamColor color)
        {
            chessPieces[x, y] = new ChessPiece(new Rectangle(x*100, y*100, 100, 100), color, type);
        }

        public void Update()
        {
            MouseState state = Mouse.GetState();

            if (gameOver)
            {
                return;
            }

            if (state.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
            {
                foreach (Tile tile in tiles)
                {
                    ChessPiece thisTile = chessPieces[tile.xPos, tile.yPos];

                    //Selecting a Piece on the Board
                    if (tile.ContainsMouse(state.X, state.Y) && thisTile != null && thisTile.teamColor == currentColor)
                    {
                        //Checks for Castling by clicking on rook
                        if (ActivateCastlingManeuver(tile))
                        {
                            return;
                        }

                        UpdateMouseSelections(tile);
                        if (thisTile != null)
                        {
                            thisTile.ChessPieceUpdate(tile.xPos, tile.yPos, chessPieces);
                        }
                    }

                    //Moving the Piece
                    else if(clickedTile != null && tile.ContainsMouse(state.X, state.Y) && (thisTile == null || thisTile.teamColor != chessPieces[clickedTile.xPos, clickedTile.yPos].teamColor))
                    {
                        //King Specific Actions
                        if (chessPieces[clickedTile.xPos, clickedTile.yPos].pieceType == ChessPieceType.King)
                        {
                            //Stops King from putting itself in check - Basic
                            if (CheckIfTileCanBeTakenNextTurn(new Vector2(tile.xPos, tile.yPos)))
                            {
                                return;
                            }

                            //Checks for Castling by clicking on where King would move
                            if (ActivateCastlingManeuver(new Tile(null, tile.xPos + 1, tile.yPos)))
                            {
                                return;
                            }
                            if (ActivateCastlingManeuver(new Tile(null, tile.xPos - 2, tile.yPos)))
                            {
                                return;
                            }
                        }

                        if (chessPieces[clickedTile.xPos, clickedTile.yPos].possibleMoves.Contains(new Vector2(tile.xPos, tile.yPos)))
                        {
                            chessPieces[clickedTile.xPos, clickedTile.yPos].MovePiece(clickedTile, tile, chessPieces);

                            ChangeTurn();

                            lastTurnMoves.Add(clickedTile);
                            lastTurnMoves.Add(tile);

                            clickedTile = null;
                        }
                    }

                }
            }

            prevMouseState = state;
        }

        private void UpdateMouseSelections(Tile tile)
        {
            if (clickedTile == null)
            {
                clickedTile = tile;
            }
            else
            {
                if (clickedTile == tile)
                {
                    clickedTile = null;
                }
                else
                {
                    clickedTile = tile;
                }
            }
        }

        //Assumes Default Chess Positioning, may require extra value sets for custom map situations
        private bool ActivateCastlingManeuver(Tile rookTile)
        {
            //Checking For Castle-ing
            if (clickedTile != null && rookTile.xPos >= 0 && rookTile.xPos < 8 && chessPieces[rookTile.xPos,rookTile.yPos] != null)
            {
                ChessPiece kingPiece = chessPieces[clickedTile.xPos, clickedTile.yPos];
                ChessPiece rookPiece = chessPieces[rookTile.xPos, rookTile.yPos];

                //You've selected King and Rook
                if (kingPiece.pieceType == ChessPieceType.King && rookPiece.pieceType == ChessPieceType.Rook)
                {
                    //Neither Piece has moved
                    if (!kingPiece.hasMovedBefore && !rookPiece.hasMovedBefore && kingPiece.teamColor == rookPiece.teamColor)
                    {
                        //Make absolutely sure these two pieces should be Castled
                        if (clickedTile.yPos == rookTile.yPos)
                        {
                            //If a piece is between them, Castle Fails
                            for (int i = Math.Min(clickedTile.xPos, rookTile.xPos); i < Math.Max(clickedTile.xPos, rookTile.xPos); i++)
                            {
                                ChessPiece pieceBlockingCastle = chessPieces[i, clickedTile.yPos];

                                if (pieceBlockingCastle != null && pieceBlockingCastle != rookPiece && pieceBlockingCastle != kingPiece)
                                {
                                    return false;
                                }
                            }

                            //If code reaches here, Castling will occur

                            //Kingside Castling
                            if (clickedTile.xPos < rookTile.xPos && clickedTile.xPos == rookTile.xPos - 3)
                            {
                                kingPiece.MovePiece(clickedTile, new Vector2(clickedTile.xPos + 2, clickedTile.yPos), chessPieces);
                                rookPiece.MovePiece(rookTile, new Vector2(rookTile.xPos - 2, rookTile.yPos), chessPieces);

                                clickedTile = null;

                                ChangeTurn();

                                lastTurnMoves.Add(clickedTile);

                                return true;
                            }
                            //Queenside Castling
                            else if (clickedTile.xPos > rookTile.xPos && clickedTile.xPos == rookTile.xPos + 4)
                            {
                                kingPiece.MovePiece(clickedTile, new Vector2(clickedTile.xPos - 2, clickedTile.yPos), chessPieces);
                                rookPiece.MovePiece(rookTile, new Vector2(rookTile.xPos + 3, rookTile.yPos), chessPieces);

                                clickedTile = null;

                                ChangeTurn();

                                lastTurnMoves.Add(clickedTile);

                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private void ChangeTurn()
        {
            lastTurnMoves.Clear();

            if (currentColor == TeamColor.White)
            {
                currentColor = TeamColor.Black;
            }
            else
            {
                currentColor = TeamColor.White;
            }

            List<Vector2> blackKings = new List<Vector2>();
            List<Vector2> whiteKings = new List<Vector2>();

            for (int i = 0; i < chessPieces.GetLength(0); i++)
            {
                for (int j = 0; j < chessPieces.GetLength(1); j++)
                {
                    ChessPiece piece = chessPieces[i, j];
                    if (piece != null)
                    {
                        if (piece.pieceType == ChessPieceType.King)
                        {
                            if (piece.teamColor == TeamColor.White)
                            {
                                whiteKings.Add(new Vector2(i,j));
                            }
                            if (piece.teamColor == TeamColor.Black)
                            {
                                blackKings.Add(new Vector2(i, j));
                            }
                        }

                        if (piece.teamColor == currentColor)
                        {
                            piece.TurnChanged();
                            piece.ChessPieceUpdate(i, j, chessPieces);
                        }
                    }
                }
            }

            //Checking for Check
            if(currentColor == TeamColor.White && whiteKings.Count != 0)
            {
                foreach (Vector2 item in whiteKings)
                {
                    foreach (ChessPiece piece in chessPieces)
                    {
                        if (piece != null && piece.teamColor != currentColor && piece.possibleMoves.Contains(item))
                        {
                            piece.isCheckingTheKing = true;
                        }
                    }
                }
            }
            else if (currentColor == TeamColor.Black && blackKings.Count != 0)
            {
                foreach (Vector2 item in blackKings)
                {
                    foreach (ChessPiece piece in chessPieces)
                    {
                        if (piece != null && piece.teamColor != currentColor && piece.possibleMoves.Contains(item))
                        {
                            piece.isCheckingTheKing = true;
                        }
                    }
                }
            }


            if (whiteKings.Count == 0 || blackKings.Count == 0)
            {
                gameOver = true;
            }
        }

        //Checks if move would kill piece - Currently Misses some situations
        //Doesn't find move if move would take enemy piece
        public bool CheckIfTileCanBeTakenNextTurn(Vector2 kingPosition)
        {
            for (int i = 0; i < chessPieces.GetLength(0); i++)
            {
                for (int j = 0; j < chessPieces.GetLength(1); j++)
                {
                    ChessPiece piece = chessPieces[i, j];
                    if (piece != null && piece.teamColor != currentColor && piece.possibleMoves.Contains(kingPosition))
                    {
                        //Ignoring Pawn forwards
                        if (piece.pieceType == ChessPieceType.Pawn && kingPosition.X == i)
                        {
                            if (kingPosition.Y == j - piece.dir || kingPosition.Y == j - 2*piece.dir)
                            {
                                continue;
                            }
                        }
                        return true;
                    }

                    //Checking for Pawn Diagonals
                    if (piece != null && piece.InRange(i+1,j+piece.dir) && piece.InRange(i - 1, j + piece.dir) && piece.pieceType == ChessPieceType.Pawn )
                    {
                        if (piece.teamColor != currentColor && (kingPosition.X == i + 1 || kingPosition.X == i - 1) && kingPosition.Y == j - piece.dir)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (Tile tile in tiles)
            {
                if (clickedTile == tile)
                {
                    tile.Draw(sb, Color.Yellow);
                }
                else if (clickedTile != null && chessPieces[clickedTile.xPos, clickedTile.yPos].possibleMoves.Contains(new Vector2(tile.xPos, tile.yPos)))
                {
                    if (lastTurnMoves.Contains(tile))
                    {
                        tile.Draw(sb, Color.LightSkyBlue);
                    }
                    else
                    {
                        tile.Draw(sb, Color.White);
                    }


                    if (chessPieces[tile.xPos, tile.yPos] == null)
                    {
                        //Draws red if King would die next turn
                        if (chessPieces[clickedTile.xPos, clickedTile.yPos].pieceType == ChessPieceType.King && CheckIfTileCanBeTakenNextTurn(new Vector2(tile.xPos,tile.yPos)))
                        {
                            sb.Draw(Game1.moveCircle, new Rectangle(tile.xPos * 100 + 30, tile.yPos * 100 + 30, 40, 40), Color.Red);
                        }
                        else
                        {
                            sb.Draw(Game1.moveCircle, new Rectangle(tile.xPos * 100 + 30, tile.yPos * 100 + 30, 40, 40), Color.LightGreen);
                        }
                    }
                }
                else if (lastTurnMoves.Contains(tile))
                {
                    tile.Draw(sb, Color.LightSkyBlue);
                }
                else
                {
                    tile.Draw(sb, Color.White);
                }
            }

            for (int i = 0; i < chessPieces.GetLength(0); i++)
            {
                for (int j = 0; j < chessPieces.GetLength(1); j++)
                {
                    if (chessPieces[i,j] != null)
                    {
                        chessPieces[i, j].Draw(sb);
                        if(clickedTile != null && chessPieces[clickedTile.xPos, clickedTile.yPos].possibleMoves.Contains(new Vector2(i, j)))
                        {
                            sb.Draw(Game1.takePieceCircle, new Rectangle(i* 100, j* 100, 100, 100), Color.Red);
                        }

                        //Draw En Passant Capture - Currently Disabled
                        #region Draw En Passant Capture
                        /*else if (chessPieces[i, j].canBeEnPassanted && clickedTile != null)
                        {
                            if (chessPieces[clickedTile.xPos, clickedTile.yPos].pieceType == ChessPieceType.Pawn)
                            {
                                if (chessPieces[clickedTile.xPos, clickedTile.yPos].teamColor != chessPieces[i, j].teamColor)
                                {
                                    if(chessPieces[i, j].teamColor == TeamColor.Black)
                                    {
                                        if(chessPieces[clickedTile.xPos, clickedTile.yPos].possibleMoves.Contains(new Vector2(i, j-1)))
                                        {
                                            sb.Draw(Game1.takePieceCircle, new Rectangle(i * 100, j * 100, 100, 100), Color.Red);
                                        }
                                    }
                                    else if (chessPieces[i, j].teamColor == TeamColor.White)
                                    {
                                        if (chessPieces[clickedTile.xPos, clickedTile.yPos].possibleMoves.Contains(new Vector2(i, j + 1)))
                                        {
                                            sb.Draw(Game1.takePieceCircle, new Rectangle(i * 100, j * 100, 100, 100), Color.Red);
                                        }
                                    }
                                }
                            }
                        }*/
                        #endregion
                    }
                }
            }

            Vector2 distance = Game1.Arial16.MeasureString("Current Turn");
            sb.DrawString(Game1.Arial16, "Current Turn: ", new Vector2(900 - distance.X / 2, 100), Color.Black);

            if (currentColor == TeamColor.White)
            {
                sb.Draw(Game1.chessTextures, new Rectangle(850, 120, 100, 100),new Rectangle(667,0,133,133), Color.White);
            }
            else
            {
                sb.Draw(Game1.chessTextures, new Rectangle(850, 120, 100, 100), new Rectangle(667, 133, 133, 133), Color.White);
            }

            if (gameOver)
            {
                distance = Game1.gameOverText.MeasureString("GAME IS OVER");
                sb.DrawString(Game1.gameOverText, "GAME IS OVER" ,new Vector2((800-distance.X)/2, (800-distance.Y)/2), Color.Blue);
            }
        }
    }
}
