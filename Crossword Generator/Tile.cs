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
        public bool UnChanged { get { return Letter == ' '; } }
        public bool Blocked { get; set; }

        public Tile(int x, int y, int size)
        {
            Letter = ' ';
            X = x;
            Y = y;
            Size = size;
        }

        public void Reset()
        {
            Letter = ' ';
            Blocked = false;
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

                if(drawLetters)
                {
                    Vector2 letterSize = Game1.Arial.MeasureString(Letter.ToString());
                    sb.DrawString(Game1.Arial, Letter.ToString(), new Vector2(X * Size + Size / 3, Y * Size + (Size - letterSize.Y) / 2), Color.Black);
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
