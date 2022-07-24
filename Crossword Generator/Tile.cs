using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Crossword_Generator
{
    class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Size { get; set; }

        public char Letter { get; set; }
        public int NumLabel { get; set; }
        public bool UnChanged { get { return Letter == ' '; } }
        public bool Blocked { get; set; }

        public Tile(int x, int y, int size)
        {
            Letter = ' ';
            X = x;
            Y = y;
            Size = size;
            NumLabel = 0;
        }

        public void Reset()
        {
            Letter = ' ';
            Blocked = false;
            NumLabel = 0;
        }

        public void Draw(SpriteBatch sb, bool drawLetters)
        {
            if (Letter != ' ')
            {
                if (!Blocked)
                {
                    sb.Draw(Game1.crosswordTile, new Rectangle(X * Size, Y * Size, Size, Size), Color.White);
                }
                else
                {
                    sb.Draw(Game1.crosswordTile, new Rectangle(X * Size, Y * Size, Size, Size), new Color(255,255,255));
                }

                if(NumLabel != 0)
                {
                    Vector2 numSize = Game1.SmallArial.MeasureString(NumLabel.ToString());
                    sb.DrawString(Game1.SmallArial, NumLabel.ToString(), new Vector2(X*Size+2,Y*Size), Color.Black);
                }

                if(drawLetters)
                {
                    Vector2 letterSize = Game1.Arial.MeasureString(Letter.ToString());
                    sb.DrawString(Game1.Arial, Letter.ToString(), new Vector2(X * Size + Size/2 - (int)(letterSize.X/2), Y * Size + Size/2 - (int)(letterSize.Y/2)), Color.Black);
                }
            }
            else
            {
                if (!Blocked)
                {
                    sb.Draw(Game1.emptyTile, new Rectangle(X * Size, Y * Size, Size, Size), Color.White);
                }
                else
                {
                    sb.Draw(Game1.emptyTile, new Rectangle(X * Size, Y * Size, Size, Size), new Color(255,255,255));
                }
            }
        }
    }
}
