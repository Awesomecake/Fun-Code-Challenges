using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maze_Solver_Test
{
    enum TileType
    {
        Wall,
        Empty,
        Start,
        End
    }

    class Tile
    {
        private int x;
        private int y;
        int width;
        int height;

        public int totalDistance = int.MaxValue;
        public TileType type;
        public bool Visited { get; set; }
        public int X { get { return x; } }
        public int Y { get { return y; } }

        public Tile(TileType type, int x, int y,int width, int height)
        {
            this.x = x;
            this.y = y;

            this.width = width;
            this.height = height;
            this.type = type;
        }

        public void Draw(SpriteBatch sb, bool drawWeights)
        {
            if (type == TileType.Wall)
            {
                sb.Draw(Game1.blackSquare, new Rectangle(x * width, y * height, width, height), Color.White);
            }
            else if (type == TileType.Start)
            {
                sb.Draw(Game1.greenSquare, new Rectangle(x * width, y * height, width, height), Color.White);
            }
            else if (type == TileType.End)
            {
                sb.Draw(Game1.redSquare, new Rectangle(x * width, y * height, width, height), Color.White);
            }
            else if (type == TileType.Empty)
            {
                if (drawWeights)
                {
                    sb.Draw(Game1.whiteSquare, new Rectangle(x * width, y * height, width, height), new Color(totalDistance * 5, totalDistance * 5, totalDistance * 5));
                }
                else
                {
                    sb.Draw(Game1.whiteSquare, new Rectangle(x * width, y * height, width, height), Color.White);
                }
            }
        }
        
        public void Draw(SpriteBatch sb, Color color, bool drawWeights)
        {
            if (type != TileType.End && type != TileType.Start)
            {
                sb.Draw(Game1.whiteSquare, new Rectangle(x * width, y * height, width, height), color);
            }
            else
            {
                Draw(sb,drawWeights);
            }
        }
    }
}
