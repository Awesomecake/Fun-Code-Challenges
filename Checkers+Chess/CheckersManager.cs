using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BoardGames
{
    class CheckersManager
    {
        List<Tile> tiles;

        Tile clickedTile; 

        MouseState prevMouseState;

        Checker[,] checkers;

        TeamColor currentColor = TeamColor.Red;

        bool gameOver = false;

        public CheckersManager()
        {
            tiles = new List<Tile>();

            checkers = new Checker[8,8];

            for (int i = 0; i < 4; i++)
            {
                AddChecker(i * 2, 7, TeamColor.Red);
                AddChecker(i * 2 + 1, 6, TeamColor.Red);
                AddChecker(i * 2, 5, TeamColor.Red);

                AddChecker(i * 2 + 1, 0, TeamColor.Black);
                AddChecker(i * 2, 1, TeamColor.Black);
                AddChecker(i * 2 + 1, 2, TeamColor.Black);

            }


            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if((i+j)%2 == 1)
                    {
                        tiles.Add(new Tile(Game1.whiteTile, i, j));
                    }
                    else
                    {
                        tiles.Add(new Tile(Game1.blackTile, i, j));
                    }
                }
            }
        }

        public void AddChecker(int x, int y, TeamColor color)
        {
            checkers[x, y] = new Checker(new Rectangle(x * 100, y * 100, 100, 100), color);
        }

        public void Update()
        {
            MouseState state = Mouse.GetState();

            if(state.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
            {
                foreach (Tile tile in tiles)
                {
                    Checker thisTile = checkers[tile.xPos, tile.yPos];
                    if (tile.ContainsMouse(state.X, state.Y) && thisTile != null && thisTile.teamColor == currentColor)
                    {
                        UpdateMouseSelections(tile);
                        if (thisTile != null)
                        {
                            thisTile.CheckerUpdate(tile.xPos,tile.yPos,checkers);
                        }
                    }
                    else if(clickedTile != null && tile.ContainsMouse(state.X, state.Y))
                    {
                        List<Vector2> requiredPreMoves = checkers[clickedTile.xPos, clickedTile.yPos].RequiresPreviousMoves(tile);
                        if (clickedTile != null && requiredPreMoves != null && requiredPreMoves.Count == 0)
                        {
                            checkers[tile.xPos, tile.yPos] = checkers[clickedTile.xPos, clickedTile.yPos];
                            checkers[tile.xPos, tile.yPos].CheckerUpdate(tile.xPos, tile.yPos, checkers);
                            checkers[tile.xPos, tile.yPos].rect = new Rectangle(tile.xPos * 100, tile.yPos * 100, 100, 100);

                            if (Math.Abs(clickedTile.xPos - tile.xPos) == 1)
                            {
                                checkers[clickedTile.xPos, clickedTile.yPos] = null;
                            }
                            else
                            {
                                checkers[clickedTile.xPos, clickedTile.yPos] = null;
                                checkers[(clickedTile.xPos + tile.xPos) / 2, (clickedTile.yPos + tile.yPos) / 2] = null;
                            }

                            clickedTile = null;

                            ChangeTurn();
                            //UpdateMouseSelections(tile);
                        }


                        else if (clickedTile != null && requiredPreMoves != null && requiredPreMoves.Count > 0)
                        {
                            Vector2 requiredMove = Vector2.Zero;
                            Vector2 previousMove = new Vector2(clickedTile.xPos, clickedTile.yPos);

                            for (int i = 0; i < requiredPreMoves.Count; i++)
                            {
                                requiredMove = requiredPreMoves[i];
                                checkers[(int)requiredMove.X, (int)requiredMove.Y] = checkers[(int)previousMove.X, (int)previousMove.Y];
                                checkers[(int)requiredMove.X, (int)requiredMove.Y].rect = new Rectangle((int)requiredMove.X * 100, (int)requiredMove.Y * 100, 100, 100);

                                checkers[(int)previousMove.X, (int)previousMove.Y] = null;
                                checkers[((int)previousMove.X + (int)requiredMove.X) / 2, ((int)previousMove.Y + (int)requiredMove.Y) / 2] = null;

                                previousMove = requiredMove;
                            }

                            Vector2 finalMove = new Vector2(tile.xPos, tile.yPos);
                            checkers[(int)finalMove.X, (int)finalMove.Y] = checkers[(int)previousMove.X, (int)previousMove.Y];
                            checkers[(int)finalMove.X, (int)finalMove.Y].CheckerUpdate((int)finalMove.X, (int)finalMove.Y, checkers);
                            checkers[(int)finalMove.X, (int)finalMove.Y].rect = new Rectangle((int)finalMove.X*100, (int)finalMove.Y * 100, 100, 100);

                            checkers[(int)previousMove.X, (int)previousMove.Y] = null;
                            checkers[((int)previousMove.X + (int)finalMove.X) / 2, ((int)previousMove.Y + (int)finalMove.Y) / 2] = null;

                            clickedTile = null;

                            ChangeTurn();
                            //UpdateMouseSelections(tile);
                        }
                    }
                }
            }

            prevMouseState = state;
        }

        private void UpdateMouseSelections(Tile tile)
        {
            if(clickedTile == null)
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

        private void ChangeTurn()
        {
            if(currentColor == TeamColor.Red)
            {
                currentColor = TeamColor.Black;
            }
            else
            {
                currentColor = TeamColor.Red;
            }

            int numRed = 0;
            int numBlack = 0;

            foreach (Checker checker in checkers)
            {
                if(checker != null)
                {
                    if(checker.teamColor == TeamColor.Black)
                    {
                        numBlack++;
                    }
                    else if(checker.teamColor == TeamColor.Red)
                    {
                        numRed++;
                    }
                }
            }

            if(numRed == 0 || numBlack == 0)
            {
                gameOver = true;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (Tile tile in tiles)
            {
                if(clickedTile == tile)
                {
                    tile.Draw(sb,Color.Yellow);
                }
                else if (clickedTile != null && checkers[clickedTile.xPos,clickedTile.yPos].possibleMoves.Contains(new Vector2(tile.xPos,tile.yPos)))
                {
                    tile.Draw(sb, Color.OrangeRed);
                }
                else
                {
                    tile.Draw(sb,Color.White);
                }
            }

            foreach (Checker checker in checkers)
            {
                if(checker != null)
                    checker.Draw(sb);
            }

            Vector2 distance = Game1.Arial16.MeasureString("Current Turn");
            sb.DrawString(Game1.Arial16, "Current Turn: ", new Vector2(900-distance.X/2, 100), Color.Black);
            
            if(currentColor == TeamColor.Red)
            {
                sb.Draw(Game1.redChecker[0], new Rectangle(850,120,100,100), Color.White);
            }
            else
            {
                sb.Draw(Game1.blackChecker[0], new Rectangle(850, 119, 100, 100), Color.White);
            }

            if (gameOver)
            {
                distance = Game1.gameOverText.MeasureString("GAME IS OVER");
                sb.DrawString(Game1.gameOverText, "GAME IS OVER", new Vector2((800 - distance.X) / 2, (800 - distance.Y) / 2), Color.Blue);
            }
        }
    }
}
