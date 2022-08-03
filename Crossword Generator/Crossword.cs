using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Crossword_Generator
{
    class Crossword
    {
        Tile[,] crossword;
        List<string> trueListOfWords; 
        List<string> words;

        Random rng;

        KeyboardState prevKB;

        bool drawLetters = true;

        public Crossword()
        {
            rng = new Random();
            crossword = new Tile[25, 25];

            trueListOfWords = new List<string>();
            /*trueListOfWords.Add("baby");
            trueListOfWords.Add("bunny");
            trueListOfWords.Add("calf");
            trueListOfWords.Add("chick");
            trueListOfWords.Add("cub");
            trueListOfWords.Add("duckling");
            trueListOfWords.Add("eaglet");
            trueListOfWords.Add("fawn");
            trueListOfWords.Add("foal");
            trueListOfWords.Add("hatchling");
            trueListOfWords.Add("joey");
            trueListOfWords.Add("kid");
            trueListOfWords.Add("kit");
            trueListOfWords.Add("kitten");
            trueListOfWords.Add("lamb");
            trueListOfWords.Add("piglet");
            trueListOfWords.Add("puppy");*/

            trueListOfWords.Add("JANUARY");
            trueListOfWords.Add("FEBRUARY");
            trueListOfWords.Add("MARCH");
            trueListOfWords.Add("APRIL");
            trueListOfWords.Add("MAY");
            trueListOfWords.Add("JUNE");
            trueListOfWords.Add("JULY");
            trueListOfWords.Add("AUGUST");
            trueListOfWords.Add("SEPTEMBER");
            trueListOfWords.Add("OCTOBER");
            trueListOfWords.Add("NOVEMBER");
            trueListOfWords.Add("DECEMBER");



            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    crossword[i, j] = new Tile(i, j, 800/25);
                }
            }
        }

        public void Update()
        {
            KeyboardState kbState = Keyboard.GetState();

            if (SingleKeyPress(kbState, Keys.C))
            {
                GenerateCrossword();

                int attempt = 0;
                while (words.Count != 0 && attempt < 10)
                {
                    GenerateCrossword();
                }

                System.Diagnostics.Debug.WriteLine(trueListOfWords.Count - words.Count + "/" + trueListOfWords.Count);
            }

            if (SingleKeyPress(kbState, Keys.D))
            {
                drawLetters = !drawLetters;
            }

            prevKB = kbState;
        }

        public void GenerateCrossword()
        {
            foreach (Tile item in crossword)
            {
                item.Reset();
            }

            words = new List<string>();
            words.AddRange(trueListOfWords);

            CreateFirstWord();

            Shuffle(words);

            int loop = 0;
            while (words.Count != 0 && loop < 30)
            {
                if (!ConnectWordsY(words[0]))
                {
                    if (!ConnectWordsX(words[0]))
                    {
                        string word = words[0];
                        words.RemoveAt(0);
                        words.Add(word);
                    }
                }

                loop++;
            }


            int wordNumLabel = 1;
            for (int j = 0; j < crossword.GetLength(1); j++)
            {
                for (int i = 0; i < crossword.GetLength(0); i++)
                {
                    if(crossword[i,j].NumLabel == -1)
                        crossword[i, j].NumLabel = wordNumLabel++;
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (Tile item in crossword)
            {
                item.Draw(sb,drawLetters);
            }
        }

        private void CreateFirstWord()
        {
            string word = GetLongestWord();

            int xLoc = (crossword.GetLength(0) - word.Length) / 2;
            int yLoc = crossword.GetLength(1) / 2;

            while (!InRange(xLoc + word.Length, yLoc))
            {
                xLoc = rng.Next(0, crossword.GetLength(0));
                yLoc = rng.Next(0, crossword.GetLength(1));
            }

            if (!IsRangeValidX(xLoc, yLoc, word))
            {
                return;
            }

            for (int i = 0; i < word.Length; i++)
            {
                if(i == 0)
                {
                    crossword[xLoc, yLoc].NumLabel = -1;
                }
                crossword[xLoc + i, yLoc].Letter = word[i];
            }

            crossword[xLoc - 1, yLoc].Blocked = true;
            crossword[xLoc + word.Length, yLoc].Blocked = true;

            words.Remove(word);
        }

        private bool ConnectWordsY(string word)
        {
            int xLoc = -1;
            int yLoc = -1;

            foreach (Tile item in crossword)
            {
                if (word.Contains(item.Letter) && IsRangeValidY(item.X, item.Y-word.IndexOf(item.Letter), word))
                {
                    xLoc = item.X;
                    yLoc = item.Y - word.IndexOf(item.Letter);
                }
            }

            if (xLoc == -1 || yLoc == -1)
            {
                return false;
            }

            for (int i = 0; i < word.Length; i++)
            {
                if (i == 0)
                {
                    crossword[xLoc, yLoc].NumLabel = -1;
                }
                crossword[xLoc, yLoc+i].Letter = word[i];
            }

            crossword[xLoc, yLoc - 1].Blocked = true;
            crossword[xLoc, yLoc + word.Length].Blocked = true;

            words.Remove(word);

            foreach (Tile item in crossword)
            {
                item.Blocked = SetBlocked(item.X, item.Y);
            }

            return true;
        }

        private bool ConnectWordsX(string word)
        {
            int xLoc = -1;
            int yLoc = -1;

            foreach (Tile item in crossword)
            {
                if (word.Contains(item.Letter) && IsRangeValidX(item.X-word.IndexOf(item.Letter), item.Y, word))
                {
                    xLoc = item.X - word.IndexOf(item.Letter);
                    yLoc = item.Y;
                }
            }

            if (xLoc == -1 || yLoc == -1)
            {
                return false;
            }

            for (int i = 0; i < word.Length; i++)
            {
                if (i == 0)
                {
                    crossword[xLoc, yLoc].NumLabel = -1;
                }
                crossword[xLoc+i, yLoc].Letter = word[i];
            }

            crossword[xLoc - 1, yLoc].Blocked = true;
            crossword[xLoc + word.Length, yLoc].Blocked = true;

            words.Remove(word);

            foreach (Tile item in crossword)
            {
                item.Blocked = SetBlocked(item.X, item.Y);
            }

            return true;
        }

        private bool InRange(int x, int y)
        {
            if(x >= 0 && x < crossword.GetLength(0) && y >= 0 && y < crossword.GetLength(1))
            {
                return true;
            }
            return false;
        }

        private string GetLongestWord()
        {
            string word = "";
            foreach (string item in words)
            {
                if (item.Length > word.Length)
                {
                    word = item;
                }
            }

            return word;
        }

        private bool IsRangeValidX(int x, int y, string word)
        {
            for (int i = 0; i < word.Length; i++)
            {
                if (!InRange(x+i,y) || crossword[x+i,y].Blocked || (!crossword[x + i, y].UnChanged && crossword[x + i, y].Letter != word[i]))
                {
                    return false;
                }
            }

            if(!InRange(x-1,y) || !crossword[x - 1, y].UnChanged)
            {
                return false;
            }
            
            if (!InRange(x + word.Length, y) || !crossword[x + word.Length, y].UnChanged)
            {
                return false;
            }

            return true;
        }

        private bool IsRangeValidY(int x, int y, string word)
        {
            for (int i = 0; i < word.Length; i++)
            {
                if (!InRange(x, y+i) || crossword[x,y+i].Blocked || (!crossword[x, y+i].UnChanged && crossword[x, y + i].Letter != word[i]))
                {
                    return false;
                }
            }

            if(!InRange(x,y-1) || !crossword[x, y - 1].UnChanged)
            {
                return false;
            }
            if (!InRange(x, y + word.Length) || !crossword[x, y + word.Length].UnChanged)
            {
                return false;
            }

            return true;
        }

        private bool SingleKeyPress(KeyboardState kb, Keys key)
        {
            if(kb.IsKeyDown(key) && !prevKB.IsKeyDown(key))
            {
                return true;
            }
            return false;
        }

        public void Shuffle(List<string> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                string value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private bool SetBlocked(int x, int y)
        {
            Tile tile = crossword[x, y];
            if (tile.Blocked)
            {
                return true;
            }

            if (InRange(x + 1, y) && !crossword[x + 1, y].UnChanged && InRange(x,y+1) && !crossword[x, y+1].UnChanged)
            {
                return true;
            }
            else if (InRange(x + 1, y) && !crossword[x + 1, y].UnChanged && InRange(x, y - 1) && !crossword[x, y - 1].UnChanged)
            {
                return true;
            }

            else if (InRange(x - 1, y) && !crossword[x - 1, y].UnChanged && InRange(x, y + 1) && !crossword[x, y + 1].UnChanged)
            {
                return true;
            }
            else if (InRange(x - 1, y) && !crossword[x - 1, y].UnChanged && InRange(x, y - 1) && !crossword[x, y - 1].UnChanged)
            {
                return true;
            }
            return false;
        }
    }
}
