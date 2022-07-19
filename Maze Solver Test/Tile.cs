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
        private int width;
        private int height;

        private Rectangle Rect { get { return new Rectangle(x * width + pixelBuffer, y * height + pixelBuffer, width, height); } }
        private int pixelBuffer;

        public int totalDistance = int.MaxValue;
        public TileType type;
        public bool Visited { get; set; }
        public int X { get { return x; } }
        public int Y { get { return y; } }

        public Tile(TileType type, int x, int y,int width, int height, int buffer)
        {
            this.x = x;
            this.y = y;
            pixelBuffer = buffer;

            this.width = width;
            this.height = height;
            this.type = type;
        }

        public void Draw(SpriteBatch sb, bool drawWeights)
        {
            switch (type)
            {
                case TileType.Wall: { sb.Draw(Game1.blackSquare, Rect, Color.White); break; }
                case TileType.Start: { sb.Draw(Game1.greenSquare, Rect, Color.White); break; }
                case TileType.End: { sb.Draw(Game1.redSquare, Rect, Color.White); break; }
                case TileType.Empty:
                    if (drawWeights)
                    {
                        sb.Draw(Game1.whiteSquare, Rect, new Color(totalDistance * 5, totalDistance * 5, totalDistance * 5));
                    }
                    else
                    {
                        sb.Draw(Game1.whiteSquare, Rect, Color.White);
                    }
                    break;
            }
        }

        public void Draw(SpriteBatch sb, Color color, bool drawWeights)
        {
            if (type != TileType.End && type != TileType.Start)
            {
                sb.Draw(Game1.whiteSquare, Rect, color);
            }
            else
            {
                Draw(sb,drawWeights);
            }
        }
    }
}
