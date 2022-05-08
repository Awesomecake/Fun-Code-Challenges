using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoardGames
{
    class Tile
    {
        Texture2D texture;
        Rectangle rect;

        public int xPos;
        public int yPos;

        public Tile(Texture2D image, int xPos, int yPos)
        {
            texture = image;
            rect = new Rectangle(xPos * 100, yPos * 100, 100, 100);

            this.xPos = xPos;
            this.yPos = yPos;
        }

        public bool ContainsMouse(int mouseX, int mouseY)
        {
            int rightSide = rect.X + rect.Width;
            int bottomSide = rect.Y + rect.Height;

            if(mouseX > rect.X && mouseX < rightSide)
            {
                if(mouseY > rect.Y && mouseY < bottomSide)
                {
                    return true;
                }
            }

            return false;
        }

        public void Draw(SpriteBatch sb, Color color)
        {
            sb.Draw(texture, rect, color);
        }
    }
}
