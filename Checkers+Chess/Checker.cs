using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoardGames
{
    class Checker
    {
        Texture2D[] texture;
        public Rectangle rect;
        public TeamColor teamColor;

        //direction of movement
        int dir;

        public List<Vector2> possibleMoves;

        public List<List<Vector2>> requiredPreMoves;

        int checkerRank = 0;

        public Checker(Rectangle rect, TeamColor color)
        {
            if (color == TeamColor.Red)
            {
                texture = Game1.redChecker;
                dir = 1;
            }
            else if (color == TeamColor.Black)
            {
                texture = Game1.blackChecker;
                dir = -1;
            }

            this.rect = rect;
            teamColor = color;

            possibleMoves = new List<Vector2>();
            requiredPreMoves = new List<List<Vector2>>();
        }

        public void CheckerUpdate(int x, int y, Checker[,] checkers)
        {
            if (teamColor == TeamColor.Red && y == 0)
            {
                checkerRank = 1;
            }
            else if (teamColor == TeamColor.Black && y == 7)
            {
                checkerRank = 1;
            }

            possibleMoves.Clear();
            requiredPreMoves.Clear();

            //If checker is a basic checker or King, it can move forward
            if (checkerRank == 0 || checkerRank == 1)
            {
                if (InRange(x + 1, y - 1 * dir) && checkers[x + 1, y - 1 * dir] == null)
                {
                    possibleMoves.Add(new Vector2(x + 1, y - 1 * dir));
                    requiredPreMoves.Add(new List<Vector2>());
                }
                else if (InRange(x + 1, y - 1 * dir) && checkers[x + 1, y - 1 * dir].teamColor != teamColor)
                {
                    if (InRange(x + 2, y - 2 * dir) && checkers[x + 2, y - 2 * dir] == null)
                    {
                        possibleMoves.Add(new Vector2(x + 2, y - 2 * dir));
                        requiredPreMoves.Add(new List<Vector2>());

                        CheckerCaptureJumpNextMove(x + 2, y - 2 * dir, checkers);
                    }
                }

                if (InRange(x - 1, y - 1 * dir) && checkers[x - 1, y - 1 * dir] == null)
                {
                    possibleMoves.Add(new Vector2(x - 1, y - 1 * dir));
                    requiredPreMoves.Add(new List<Vector2>());
                }
                else if (InRange(x - 1, y - 1 * dir) && checkers[x - 1, y - 1 * dir].teamColor != teamColor)
                {
                    if (InRange(x - 2, y - 2 * dir) && checkers[x - 2, y - 2 * dir] == null)
                    {
                        possibleMoves.Add(new Vector2(x - 2, y - 2 * dir));
                        requiredPreMoves.Add(new List<Vector2>());

                        CheckerCaptureJumpNextMove(x - 2, y - 2 * dir, checkers);

                    }
                }
            }

            //Only King can move backwards
            if (checkerRank == 1)
            {
                if (InRange(x + 1, y + 1 * dir) && checkers[x + 1, y + 1 * dir] == null)
                {
                    possibleMoves.Add(new Vector2(x + 1, y + 1 * dir));
                    requiredPreMoves.Add(new List<Vector2>());
                }
                else if (InRange(x + 1, y + 1 * dir) && checkers[x + 1, y + 1 * dir].teamColor != teamColor)
                {
                    if (InRange(x + 2, y + 2 * dir) && checkers[x + 2, y + 2 * dir] == null)
                    {
                        possibleMoves.Add(new Vector2(x + 2, y + 2 * dir));
                        requiredPreMoves.Add(new List<Vector2>());

                        CheckerCaptureJumpNextMove(x + 2, y + 2 * dir,checkers);
                    }
                }

                if (InRange(x - 1, y + 1 * dir) && checkers[x - 1, y + 1 * dir] == null)
                {
                    possibleMoves.Add(new Vector2(x - 1, y + 1 * dir));
                    requiredPreMoves.Add(new List<Vector2>());
                }
                else if (InRange(x - 1, y + 1 * dir) && checkers[x - 1, y + 1 * dir].teamColor != teamColor)
                {
                    if (InRange(x - 2, y + 2 * dir) && checkers[x - 2, y + 2 * dir] == null)
                    {
                        possibleMoves.Add(new Vector2(x - 2, y + 2 * dir));
                        requiredPreMoves.Add(new List<Vector2>());

                        CheckerCaptureJumpNextMove(x - 2, y + 2 * dir,checkers);
                    }
                }
            }

        }

        private void CheckerCaptureJumpNextMove(int x, int y, Checker[,] checkers)
        {
            if (checkerRank == 0 || checkerRank == 1)
            {
                if (InRange(x+1,y-1*dir) && checkers[x + 1, y - 1 * dir] != null && checkers[x + 1, y - 1 * dir].teamColor != teamColor)
                {
                    if (InRange(x + 2, y - 2 * dir) && checkers[x + 2, y - 2 * dir] == null)
                    {
                        possibleMoves.Add(new Vector2(x + 2, y - 2 * dir));
                        List<Vector2> preMoves = new List<Vector2>();
                        preMoves.Add(new Vector2(x, y));

                        requiredPreMoves.Add(preMoves);

                        CheckerCaptureJumpNextMove(x + 2, y - 2 * dir, checkers,preMoves);
                    }
                }

                if (InRange(x - 1, y - 1 * dir) && checkers[x - 1, y - 1 * dir] != null && checkers[x - 1, y - 1 * dir].teamColor != teamColor)
                {
                    if (InRange(x - 2, y - 2 * dir) && checkers[x - 2, y - 2 * dir] == null)
                    {
                        possibleMoves.Add(new Vector2(x - 2, y - 2 * dir));
                        List<Vector2> preMoves = new List<Vector2>();
                        preMoves.Add(new Vector2(x, y));

                        requiredPreMoves.Add(preMoves);
                        
                        CheckerCaptureJumpNextMove(x - 2, y - 2 * dir, checkers,preMoves);
                    }
                }
            }

            if(checkerRank == 1)
            {
                if (InRange(x + 1, y + 1 * dir) && checkers[x + 1, y + 1 * dir] != null && checkers[x + 1, y + 1 * dir].teamColor != teamColor)
                {
                    if (InRange(x + 2, y + 2 * dir) && checkers[x + 2, y + 2 * dir] == null)
                    {
                        possibleMoves.Add(new Vector2(x + 2, y + 2 * dir));
                        List<Vector2> preMoves = new List<Vector2>();
                        preMoves.Add(new Vector2(x, y));

                        requiredPreMoves.Add(preMoves);

                        CheckerCaptureJumpNextMove(x + 2, y + 2 * dir, checkers, preMoves);
                    }
                }

                if (InRange(x - 1, y + 1 * dir) && checkers[x - 1, y + 1 * dir] != null && checkers[x - 1, y + 1 * dir].teamColor != teamColor)
                {
                    if (InRange(x - 2, y + 2 * dir) && checkers[x - 2, y + 2 * dir] == null)
                    {
                        possibleMoves.Add(new Vector2(x - 2, y + 2 * dir));
                        List<Vector2> preMoves = new List<Vector2>();
                        preMoves.Add(new Vector2(x, y));

                        requiredPreMoves.Add(preMoves);

                        CheckerCaptureJumpNextMove(x - 2, y + 2 * dir, checkers, preMoves);
                    }
                }
            }
        }

        private void CheckerCaptureJumpNextMove(int x, int y, Checker[,] checkers, List<Vector2> preMoves)
        {
            if (checkerRank == 0 || checkerRank == 1)
            {
                if (InRange(x + 1, y - 1 * dir) && checkers[x + 1, y - 1 * dir] != null && checkers[x + 1, y - 1 * dir].teamColor != teamColor)
                {
                    if (InRange(x + 2, y - 2 * dir) && checkers[x + 2, y - 2 * dir] == null && !preMoves.Contains(new Vector2(x+2,y-2*dir)))
                    {
                        possibleMoves.Add(new Vector2(x + 2, y - 2 * dir));

                        List<Vector2> newPreMoves = new List<Vector2>(preMoves);
                        newPreMoves.Add(new Vector2(x, y));

                        requiredPreMoves.Add(newPreMoves);

                        CheckerCaptureJumpNextMove(x + 2, y - 2 * dir, checkers,newPreMoves);
                    }
                }

                if (InRange(x - 1, y - 1 * dir) && checkers[x - 1, y - 1 * dir] != null && checkers[x - 1, y - 1 * dir].teamColor != teamColor)
                {
                    if (InRange(x - 2, y - 2 * dir) && checkers[x - 2, y - 2 * dir] == null && !possibleMoves.Contains(new Vector2(x-2, y-2*dir)))
                    {
                        possibleMoves.Add(new Vector2(x - 2, y - 2 * dir));

                        List<Vector2> newPreMoves = new List<Vector2>(preMoves);
                        newPreMoves.Add(new Vector2(x, y));

                        requiredPreMoves.Add(newPreMoves);

                        CheckerCaptureJumpNextMove(x - 2, y - 2 * dir, checkers,newPreMoves);
                    }
                }
            }

            if (checkerRank == 1)
            {
                if (InRange(x + 1, y + 1 * dir) && checkers[x + 1, y + 1 * dir] != null && checkers[x + 1, y + 1 * dir].teamColor != teamColor)
                {
                    if (InRange(x + 2, y + 2 * dir) && checkers[x + 2, y + 2 * dir] == null && !possibleMoves.Contains(new Vector2(x+2, y+2*dir)))
                    {
                        possibleMoves.Add(new Vector2(x + 2, y + 2 * dir));

                        List<Vector2> newPreMoves = new List<Vector2>(preMoves);
                        newPreMoves.Add(new Vector2(x, y));

                        requiredPreMoves.Add(newPreMoves);

                        CheckerCaptureJumpNextMove(x + 2, y + 2 * dir, checkers, newPreMoves);
                    }
                }

                if (InRange(x - 1, y + 1 * dir) && checkers[x - 1, y + 1 * dir] != null && checkers[x - 1, y + 1 * dir].teamColor != teamColor)
                {
                    if (InRange(x - 2, y + 2 * dir) && checkers[x - 2, y + 2 * dir] == null && !possibleMoves.Contains(new Vector2(x-2, y+2*dir)))
                    {
                        possibleMoves.Add(new Vector2(x - 2, y + 2 * dir));

                        List<Vector2> newPreMoves = new List<Vector2>(preMoves);
                        newPreMoves.Add(new Vector2(x, y));

                        requiredPreMoves.Add(newPreMoves);

                        CheckerCaptureJumpNextMove(x - 2, y + 2 * dir, checkers, newPreMoves);
                    }
                }
            }
        }

        private bool InRange(int x, int y)
        {
            if(x < 8 && y >= 0 && y < 8 && x >= 0)
            {
                return true;
            }
            return false;
        }

        public List<Vector2> RequiresPreviousMoves(Tile tile)
        {
            if(possibleMoves.Contains(new Vector2(tile.xPos, tile.yPos)))
            {
                return requiredPreMoves[possibleMoves.IndexOf(new Vector2(tile.xPos, tile.yPos))];
            }
            return null;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture[checkerRank], rect, Color.White);
        }
    }
}
