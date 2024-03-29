﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Maze_Solver_Test
{
    class Maze
    {
        Tile[,] maze;
        private Tile start = null;
        private Tile end = null;

        private int width;
        private int height;

        private int screenWidth;
        private int screenHeight;
        private int pixelBuffer;

        private bool builtAlternateItem;
        private KeyboardState prevKBstate;

        private List<Tile> solvedMaze;
        private bool drawWeights;

        private int animationMode = -1;
        private Stack<Tile> animationStack;
        private float animationTimer = 0;

        private bool writingWidthSize;
        private bool writingHeightSize;
        private string widthSizeTextBox;
        private string heightSizeTextBox;

        private int heightOffset;

        private Rectangle WidthTextRect { get {return new Rectangle(screenWidth + (int)(pixelBuffer * 1.5), pixelBuffer + 150, 100, 50); } }
        private Rectangle HeightTextRect { get { return new Rectangle(screenWidth + (int)(pixelBuffer * 1.5), pixelBuffer+50, 100, 50); } }
        private Rectangle SizeChangeConfirm { get { return new Rectangle(screenWidth + (int)(pixelBuffer * 1.5), screenHeight + heightOffset - 50 + pixelBuffer, 100, 50); } }

        Random rng;

        public Maze(int x, int y, int width, int height, int buffer)
        {
            maze = new Tile[x, y];
            this.width = x;
            this.height = y;
            screenHeight = height;
            screenWidth = width;
            pixelBuffer = buffer;

            widthSizeTextBox = x.ToString();
            heightSizeTextBox = y.ToString();

            rng = new Random();

            solvedMaze = new List<Tile>();
            animationStack = new Stack<Tile>();

            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    CreateSquare(TileType.Empty, i, j);
                }
            }
        }

        public Tile CreateSquare(TileType type, int x, int y)
        {
            return maze[x, y] = new Tile(type, x, y,screenWidth/width,screenHeight/height, pixelBuffer,heightOffset);
        }

        public void Update(GameTime gameTime)
        {
            MouseState currentState = Mouse.GetState();
            KeyboardState kbState = Keyboard.GetState();

            switch (animationMode)
            {
                case -1:
                    if (currentState.MiddleButton == ButtonState.Pressed)
                    {
                        builtAlternateItem = true;
                    }

                    if (currentState.LeftButton == ButtonState.Pressed && InRange(MouseTilePosX(currentState), MouseTilePosY(currentState)))
                    {
                        ClearDrawings();

                        Tile newItem = maze[MouseTilePosX(currentState), MouseTilePosY(currentState)];

                        if (builtAlternateItem)
                        {
                            builtAlternateItem = false;
                            if (end != null && end.type == TileType.End)
                            {
                                end.type = TileType.Empty;
                            }

                            newItem.type = TileType.End;
                            end = newItem;
                        }
                        else if (newItem != null && newItem.type != TileType.End)
                        {
                            newItem.type = TileType.Wall;
                        }
                    }
                    else if (currentState.RightButton == ButtonState.Pressed && InRange(MouseTilePosX(currentState), MouseTilePosY(currentState)))
                    {
                        ClearDrawings();

                        Tile newItem = maze[MouseTilePosX(currentState), MouseTilePosY(currentState)];

                        if (builtAlternateItem)
                        {
                            builtAlternateItem = false;
                            if (start != null && start.type == TileType.Start)
                            {
                                start.type = TileType.Empty;
                            }

                            start = newItem;
                            newItem.type = TileType.Start;
                        }
                        else if (newItem != null && newItem.type != TileType.Start)
                        {
                            newItem.type = TileType.Empty;
                        }
                    }
                    else if (currentState.LeftButton == ButtonState.Pressed && MouseInsideRect(currentState, WidthTextRect))
                    {
                        writingWidthSize = true;
                        widthSizeTextBox = "";
                    }
                    else if (currentState.LeftButton == ButtonState.Pressed && MouseInsideRect(currentState, HeightTextRect))
                    {
                        writingHeightSize = true;
                        heightSizeTextBox = "";
                    }
                    else if(currentState.LeftButton == ButtonState.Pressed && MouseInsideRect(currentState, SizeChangeConfirm))
                    {
                        int widthResult;
                        int heightResult;
                        if (int.TryParse(widthSizeTextBox, out widthResult) && widthResult < 300 && widthResult > 1)
                        {
                            if (int.TryParse(heightSizeTextBox, out heightResult) && heightResult < 300 && heightResult > 1)
                            {
                                width = widthResult;
                                height = heightResult;

                                int size = MathHelper.Max(width, height);

                                screenWidth = (800 / size) * width;
                                screenHeight = (800 / size) * height;

                                heightOffset = 0;

                                if(screenHeight < 300)
                                {
                                    heightOffset = 300 - screenHeight;
                                }

                                maze = new Tile[width, height];

                                for (int i = 0; i < maze.GetLength(0); i++)
                                {
                                    for (int j = 0; j < maze.GetLength(1); j++)
                                    {
                                        CreateSquare(TileType.Empty, i, j);
                                    }
                                }

                                Game1._graphics.PreferredBackBufferWidth = (800 / size) * width + pixelBuffer * 2 + 100;
                                Game1._graphics.PreferredBackBufferHeight = (800 / size) * height + pixelBuffer * 2 + heightOffset;
                                Game1._graphics.ApplyChanges();
                            }
                        }  
                    }

                    if (!MouseInsideRect(currentState, WidthTextRect) && writingWidthSize)
                    {
                        writingWidthSize = false;
                        if (widthSizeTextBox == "")
                            widthSizeTextBox = width.ToString();
                    }

                    if (!MouseInsideRect(currentState, HeightTextRect) && writingHeightSize)
                    {
                        writingHeightSize = false;
                        if (heightSizeTextBox == "")
                            heightSizeTextBox = height.ToString();
                    }

                    if (SingleKeyPress(kbState, Keys.D))
                    {
                        drawWeights = !drawWeights;
                    }

                    if (SingleKeyPress(kbState, Keys.P))
                    {
                        List<Tile> oldSolution;

                        do
                        {
                            oldSolution = new List<Tile>();

                            oldSolution.AddRange(solvedMaze);
                            PruneSolution(solvedMaze);

                        } while (oldSolution.Count != solvedMaze.Count);
                    }

                    if (!writingWidthSize && !writingHeightSize)
                    {
                        if (SingleKeyPressMazeSolve(kbState, Keys.D1))
                        {
                            ClearDrawings();
                            solvedMaze = DijikstraAlgorith();
                        }
                        else if (SingleKeyPressMazeSolve(kbState, Keys.D2))
                        {
                            ClearDrawings();
                            solvedMaze = DepthSearch();
                        }
                        else if (SingleKeyPressMazeSolve(kbState, Keys.D3))
                        {
                            ClearDrawings();
                            solvedMaze = DijikstraAlgorithScreenWrap();
                        }
                        else if (SingleKeyPressMazeSolve(kbState, Keys.D4))
                        {
                            ClearDrawings();
                            solvedMaze = DepthSearchScreenWrap();
                        }

                        else if (SingleKeyPressMazeSolve(kbState, Keys.D5))
                        {
                            ClearDrawings();
                            animationMode = 1;
                            #region Setup For Dijikstra Animation
                            //Animate DijikstraAlgorith

                            ResetTilesVisitedAndDistance();

                            Tile currentNode = end;

                            currentNode.totalDistance = 0;

                            while (currentNode != null)
                            {
                                currentNode.Visited = true;

                                if (IsTileValid(currentNode.X + 1, currentNode.Y) && currentNode.totalDistance + 1 < maze[currentNode.X + 1, currentNode.Y].totalDistance)
                                {
                                    maze[currentNode.X + 1, currentNode.Y].totalDistance = currentNode.totalDistance + 1;
                                }
                                if (IsTileValid(currentNode.X - 1, currentNode.Y) && currentNode.totalDistance + 1 < maze[currentNode.X - 1, currentNode.Y].totalDistance)
                                {
                                    maze[currentNode.X - 1, currentNode.Y].totalDistance = currentNode.totalDistance + 1;
                                }
                                if (IsTileValid(currentNode.X, currentNode.Y + 1) && currentNode.totalDistance + 1 < maze[currentNode.X, currentNode.Y + 1].totalDistance)
                                {
                                    maze[currentNode.X, currentNode.Y + 1].totalDistance = currentNode.totalDistance + 1;
                                }
                                if (IsTileValid(currentNode.X, currentNode.Y - 1) && currentNode.totalDistance + 1 < maze[currentNode.X, currentNode.Y - 1].totalDistance)
                                {
                                    maze[currentNode.X, currentNode.Y - 1].totalDistance = currentNode.totalDistance + 1;
                                }

                                Tile newCurrent = null;

                                for (int i = 0; i < maze.GetLength(0); i++)
                                {
                                    for (int j = 0; j < maze.GetLength(1); j++)
                                    {
                                        if (!maze[i, j].Visited && (newCurrent == null || maze[i, j].totalDistance < newCurrent.totalDistance))
                                        {
                                            newCurrent = maze[i, j];
                                        }
                                    }
                                }

                                currentNode = newCurrent;
                            }

                            foreach (Tile item in maze)
                            {
                                if (item != null)
                                {
                                    item.Visited = false;
                                }
                            }

                            animationStack = new Stack<Tile>();

                            animationStack.Push(start);
                            start.Visited = true;

                            #endregion
                        }
                        else if (SingleKeyPressMazeSolve(kbState, Keys.D6))
                        {
                            ClearDrawings();
                            animationMode = 2;
                            #region Setup For Depth Search Animation

                            ResetTilesVisitedAndDistance();

                            animationStack = new Stack<Tile>();

                            animationStack.Push(start);
                            start.Visited = true;
                            #endregion
                        }
                        else if (SingleKeyPressMazeSolve(kbState, Keys.D7))
                        {
                            ClearDrawings();
                            animationMode = 3;
                            #region Setup For Dijikstra Screen Wrap Animation
                            ResetTilesVisitedAndDistance();

                            Tile currentNode = end;

                            currentNode.totalDistance = 0;

                            while (currentNode != null)
                            {
                                currentNode.Visited = true;

                                if (IsTileValid(currentNode.X + 1, currentNode.Y) && currentNode.totalDistance + 1 < maze[currentNode.X + 1, currentNode.Y].totalDistance)
                                {
                                    maze[currentNode.X + 1, currentNode.Y].totalDistance = currentNode.totalDistance + 1;
                                }
                                else if (!InRange(currentNode.X + 1, currentNode.Y) && IsTileValid(currentNode.X + 1 - width, currentNode.Y) && currentNode.totalDistance + 1 < maze[currentNode.X + 1 - width, currentNode.Y].totalDistance)
                                {
                                    maze[currentNode.X + 1 - width, currentNode.Y].totalDistance = currentNode.totalDistance + 1;
                                }

                                if (IsTileValid(currentNode.X - 1, currentNode.Y) && currentNode.totalDistance + 1 < maze[currentNode.X - 1, currentNode.Y].totalDistance)
                                {
                                    maze[currentNode.X - 1, currentNode.Y].totalDistance = currentNode.totalDistance + 1;
                                }
                                else if (!InRange(currentNode.X - 1, currentNode.Y) && IsTileValid(currentNode.X - 1 + width, currentNode.Y) && currentNode.totalDistance + 1 < maze[currentNode.X - 1 + width, currentNode.Y].totalDistance)
                                {
                                    maze[currentNode.X - 1 + width, currentNode.Y].totalDistance = currentNode.totalDistance + 1;
                                }

                                if (IsTileValid(currentNode.X, currentNode.Y + 1) && currentNode.totalDistance + 1 < maze[currentNode.X, currentNode.Y + 1].totalDistance)
                                {
                                    maze[currentNode.X, currentNode.Y + 1].totalDistance = currentNode.totalDistance + 1;
                                }
                                else if (!InRange(currentNode.X, currentNode.Y + 1) && IsTileValid(currentNode.X, currentNode.Y + 1 - height) && currentNode.totalDistance + 1 < maze[currentNode.X, currentNode.Y + 1 - height].totalDistance)
                                {
                                    maze[currentNode.X, currentNode.Y + 1 - height].totalDistance = currentNode.totalDistance + 1;
                                }

                                if (IsTileValid(currentNode.X, currentNode.Y - 1) && currentNode.totalDistance + 1 < maze[currentNode.X, currentNode.Y - 1].totalDistance)
                                {
                                    maze[currentNode.X, currentNode.Y - 1].totalDistance = currentNode.totalDistance + 1;
                                }
                                else if (!InRange(currentNode.X, currentNode.Y - 1) && IsTileValid(currentNode.X, currentNode.Y - 1 + height) && currentNode.totalDistance + 1 < maze[currentNode.X, currentNode.Y - 1 + height].totalDistance)
                                {
                                    maze[currentNode.X, currentNode.Y - 1 + height].totalDistance = currentNode.totalDistance + 1;
                                }

                                Tile newCurrent = null;

                                for (int i = 0; i < maze.GetLength(0); i++)
                                {
                                    for (int j = 0; j < maze.GetLength(1); j++)
                                    {
                                        if (!maze[i, j].Visited && (newCurrent == null || maze[i, j].totalDistance < newCurrent.totalDistance))
                                        {
                                            newCurrent = maze[i, j];
                                        }
                                    }
                                }

                                currentNode = newCurrent;
                            }

                            foreach (Tile item in maze)
                            {
                                if (item != null)
                                {
                                    item.Visited = false;
                                }
                            }

                            animationStack = new Stack<Tile>();

                            animationStack.Push(start);
                            start.Visited = true;
                            #endregion

                        }
                        else if (SingleKeyPressMazeSolve(kbState,Keys.D8))
                        {
                            ClearDrawings();
                            animationMode = 4;
                            #region Setup For Depth Search Screen Wrap Animation
                            ResetTilesVisitedAndDistance();

                            animationStack = new Stack<Tile>();

                            animationStack.Push(start);
                            start.Visited = true;
                            #endregion
                        }

                        else if (SingleKeyPress(kbState, Keys.D0))
                        {
                            ClearDrawings();
                            animationMode = 5;
                            #region Setup For Maze Generation Animation
                            for (int i = 0; i < maze.GetLength(0); i++)
                            {
                                for (int j = 0; j < maze.GetLength(1); j++)
                                {
                                    CreateSquare(TileType.Wall, i, j);
                                }
                            }

                            animationStack = new Stack<Tile>();

                            int xStart = rng.Next(0, width);
                            int yStart = rng.Next(0, height);

                            if (xStart % 2 == 0)
                            {
                                if (xStart + 1 < width)
                                {
                                    xStart++;
                                }
                                else
                                {
                                    xStart--;
                                }
                            }

                            if (yStart % 2 == 0)
                            {
                                if (yStart + 1 < height)
                                {
                                    yStart++;
                                }
                                else
                                {
                                    yStart--;
                                }
                            }

                            animationStack.Push(maze[xStart, yStart]);

                            //Commenting out allows for loop to exist at start point
                            maze[xStart, yStart].Visited = true;
                            maze[xStart, yStart].type = TileType.Empty;
                            #endregion
                        }

                        if (SingleKeyPress(kbState, Keys.V))
                        {
                            ClearDrawings();
                            MakeMaze(0);
                        }
                        else if (SingleKeyPress(kbState, Keys.B))
                        {
                            ClearDrawings();
                            MakeMaze(1);
                        }
                        else if (SingleKeyPress(kbState, Keys.N))
                        {
                            ClearDrawings();
                            MakeMaze(2);
                        }
                        else if (SingleKeyPress(kbState, Keys.M))
                        {
                            ClearDrawings();
                            MakeMaze(3);
                        }
                    }
                    else if (writingWidthSize && kbState.GetPressedKeyCount() != 0)
                    {
                        widthSizeTextBox = UpdateTextBox(kbState, widthSizeTextBox);
                    }
                    else if (writingHeightSize && kbState.GetPressedKeyCount() != 0)
                    {
                        heightSizeTextBox = UpdateTextBox(kbState, heightSizeTextBox);
                    }
                    break;
                case 1:
                    #region Dijikstra Animation
                    //Loop while we have tiles in the stack
                    if (animationStack.Count != 0 && animationTimer > 0.025)
                    {
                        animationTimer = 0;

                        Tile current = animationStack.Peek();

                        Tile nextMove = null;

                        List<int> choices = new List<int> { 1, 2, 3, 4 };

                        Tile selection = null;

                        while (choices.Count != 0 && selection == null)
                        {
                            int choice = choices[rng.Next(0, choices.Count)];
                            //Check the neighbors of the current Tile, Put them on stack if valid
                            if (choice == 1 && IsTileValid(current.X + 1, current.Y) && maze[current.X + 1, current.Y].totalDistance < current.totalDistance)
                            {
                                nextMove = maze[current.X + 1, current.Y];
                            }
                            else if (choice == 2 && IsTileValid(current.X - 1, current.Y) && maze[current.X - 1, current.Y].totalDistance < current.totalDistance)
                            {
                                nextMove = maze[current.X - 1, current.Y];
                            }
                            else if (choice == 3 && IsTileValid(current.X, current.Y + 1) && maze[current.X, current.Y + 1].totalDistance < current.totalDistance)
                            {
                                nextMove = maze[current.X, current.Y + 1];
                            }
                            else if (choice == 4 && IsTileValid(current.X, current.Y - 1) && maze[current.X, current.Y - 1].totalDistance < current.totalDistance)
                            {
                                nextMove = maze[current.X, current.Y - 1];
                            }

                            choices.Remove(choice);
                        }

                        if (nextMove != null)
                        {
                            animationStack.Push(nextMove);
                            nextMove.Visited = true;
                        }

                        //If there are no valid neighbors, back up to the previous tile
                        if (nextMove == null)
                        {
                            animationStack.Pop();
                        }

                        //If the current tile is the end, quit the search
                        if (animationStack.Count == 0 || animationStack.Peek().type == TileType.End)
                        {
                            animationMode = -1;
                        }

                    }
                    #endregion
                    break;
                case 2:
                    #region Depth Search Animation
                    if (animationStack.Count != 0 && animationTimer > 0.025)
                    {
                        animationTimer = 0;

                        Tile current = animationStack.Peek();

                        List<int> choices = new List<int> { 1, 2, 3, 4 };

                        Tile selection = null;

                        while (choices.Count != 0 && selection == null)
                        {
                            int choice = choices[rng.Next(0, choices.Count)];
                            //Check the neighbors of the current Tile, Put them on stack if valid
                            if (IsTileValid(current.X + 1, current.Y) && choice == 1)
                            {
                                selection = maze[current.X + 1, current.Y];
                            }
                            else if (IsTileValid(current.X - 1, current.Y) && choice == 2)
                            {
                                selection = maze[current.X - 1, current.Y];
                            }
                            else if (IsTileValid(current.X, current.Y + 1) && choice == 3)
                            {
                                selection = maze[current.X, current.Y + 1];
                            }
                            else if (IsTileValid(current.X, current.Y - 1) && choice == 4)
                            {
                                selection = maze[current.X, current.Y - 1];
                            }

                            choices.Remove(choice);
                        }

                        //If there are no valid neighbors, back up to the previous tile
                        if (selection == null)
                        {
                            animationStack.Pop().totalDistance = 40;
                        }
                        else
                        {
                            animationStack.Push(selection);
                            selection.Visited = true;
                        }

                        //If the current tile is the end, quit the search
                        if (animationStack.Count == 0 || animationStack.Peek().type == TileType.End)
                        {
                            animationMode = -1;
                        }

                    }
                    #endregion
                    break;
                case 3:
                    #region Dijikstra Screen Wrap Animation
                    if (animationStack.Count != 0 && animationTimer > 0.025)
                    {
                        animationTimer = 0;

                        Tile current = animationStack.Peek();

                        Tile nextMove = null;

                        List<int> choices = new List<int> { 1, 2, 3, 4 };

                        Tile selection = null;

                        while (choices.Count != 0 && selection == null)
                        {
                            int choice = choices[rng.Next(0, choices.Count)];
                            //Check the neighbors of the current Tile, Put them on stack if valid
                            if (choice == 1 && IsTileValid(current.X + 1, current.Y) && maze[current.X + 1, current.Y].totalDistance < current.totalDistance)
                            {
                                nextMove = maze[current.X + 1, current.Y];
                            }
                            else if (choice == 1 && !InRange(current.X + 1, current.Y) && IsTileValid(current.X + 1 - width, current.Y) && maze[current.X + 1 - width, current.Y].totalDistance < current.totalDistance)
                            {
                                nextMove = maze[current.X + 1 - width, current.Y];
                            }

                            else if (choice == 2 && IsTileValid(current.X - 1, current.Y) && maze[current.X - 1, current.Y].totalDistance < current.totalDistance)
                            {
                                nextMove = maze[current.X - 1, current.Y];
                            }
                            else if (choice == 2 && !InRange(current.X - 1, current.Y) && IsTileValid(current.X - 1 + width, current.Y) && maze[current.X - 1 + width, current.Y].totalDistance < current.totalDistance)
                            {
                                nextMove = maze[current.X - 1 + width, current.Y];
                            }

                            else if (choice == 3 && IsTileValid(current.X, current.Y + 1) && maze[current.X, current.Y + 1].totalDistance < current.totalDistance)
                            {
                                nextMove = maze[current.X, current.Y + 1];
                            }
                            else if (choice == 3 && !InRange(current.X, current.Y + 1) && IsTileValid(current.X, current.Y + 1 - height) && maze[current.X, current.Y + 1 - height].totalDistance < current.totalDistance)
                            {
                                nextMove = maze[current.X, current.Y + 1 - height];
                            }

                            else if (choice == 4 && IsTileValid(current.X, current.Y - 1) && maze[current.X, current.Y - 1].totalDistance < current.totalDistance)
                            {
                                nextMove = maze[current.X, current.Y - 1];
                            }
                            else if (choice == 4 && !InRange(current.X, current.Y - 1) && IsTileValid(current.X, current.Y - 1 + height) && maze[current.X, current.Y - 1 + height].totalDistance < current.totalDistance)
                            {
                                nextMove = maze[current.X, current.Y - 1 + height];
                            }

                            choices.Remove(choice);
                        }

                        if (nextMove != null)
                        {
                            animationStack.Push(nextMove);
                            nextMove.Visited = true;
                        }

                        //If there are no valid neighbors, back up to the previous tile
                        if (nextMove == null)
                        {
                            animationStack.Pop();
                        }

                        //If the current tile is the end, quit the search
                        if (animationStack.Count == 0 || animationStack.Peek().type == TileType.End)
                        {
                            animationMode = -1;
                        }

                    }
                    #endregion
                    break;
                case 4:
                    #region Depth Search Screen Wrap Animation
                    if (animationStack.Count != 0 && animationTimer > 0.025)
                    {
                        animationTimer = 0;

                        Tile current = animationStack.Peek();

                        List<int> choices = new List<int> { 1, 2, 3, 4 };

                        Tile selection = null;

                        while (choices.Count != 0 && selection == null)
                        {
                            int choice = choices[rng.Next(0, choices.Count)];
                            //Check the neighbors of the current Tile, Put them on stack if valid

                            //Check the neighbors of the current Tile, Put them on stack if valid
                            if (IsTileValid(current.X + 1, current.Y) && choice == 1)
                            {
                                selection = maze[current.X + 1, current.Y];
                            }
                            else if (!InRange(current.X + 1, current.Y) && IsTileValid(current.X + 1 - width, current.Y) && choice == 1)
                            {
                                selection = maze[current.X + 1 - width, current.Y];
                            }

                            else if (IsTileValid(current.X - 1, current.Y) && choice == 2)
                            {
                                selection = maze[current.X - 1, current.Y];
                            }
                            else if (!InRange(current.X - 1, current.Y) && IsTileValid(current.X - 1 + width, current.Y) && choice == 2)
                            {
                                selection = maze[current.X - 1 + width, current.Y];
                            }

                            else if (IsTileValid(current.X, current.Y + 1) && choice == 3)
                            {
                                selection = maze[current.X, current.Y + 1];
                            }
                            else if (!InRange(current.X, current.Y + 1) && IsTileValid(current.X, current.Y + 1 - height) && choice == 3)
                            {
                                selection = maze[current.X, current.Y + 1 - height];
                            }

                            else if (IsTileValid(current.X, current.Y - 1) && choice == 4)
                            {
                                selection = maze[current.X, current.Y - 1];
                            }
                            else if (!InRange(current.X, current.Y - 1) && IsTileValid(current.X, current.Y - 1 + height) && choice == 4)
                            {
                                selection = maze[current.X, current.Y - 1 + height];
                            }

                            choices.Remove(choice);
                        }

                        //If there are no valid neighbors, back up to the previous tile
                        if (selection == null)
                        {
                            animationStack.Pop().totalDistance = 40;
                        }
                        else
                        {
                            animationStack.Push(selection);
                            selection.Visited = true;
                        }


                        //If the current tile is the end, quit the search
                        if (animationStack.Count == 0 || animationStack.Peek().type == TileType.End)
                        {
                            animationMode = -1;
                        }
                    }
                    #endregion
                    break;
                case 5:
                    #region Maze Generator Animation

                    //Loop while we have tiles in the stack
                    if (animationStack.Count != 0 && animationTimer > 0.025)
                    {
                        animationTimer = 0;

                        Tile current = animationStack.Peek();

                        List<int> choices = new List<int> { 1, 2, 3, 4 };

                        Tile preSelection = null;
                        Tile selection = null;

                        while (choices.Count != 0 && selection == null)
                        {
                            int choice = choices[rng.Next(0, choices.Count)];
                            //Check the neighbors of the current Tile, Put them on stack if valid
                            if (IsTileValidEmpty(current.X + 2, current.Y) && choice == 1)
                            {
                                preSelection = maze[current.X + 1, current.Y];
                                selection = maze[current.X + 2, current.Y];
                            }
                            else if (IsTileValidEmpty(current.X - 2, current.Y) && choice == 2)
                            {
                                preSelection = maze[current.X - 1, current.Y];
                                selection = maze[current.X - 2, current.Y];
                            }
                            else if (IsTileValidEmpty(current.X, current.Y + 2) && choice == 3)
                            {
                                preSelection = maze[current.X, current.Y + 1];
                                selection = maze[current.X, current.Y + 2];
                            }
                            else if (IsTileValidEmpty(current.X, current.Y - 2) && choice == 4)
                            {
                                preSelection = maze[current.X, current.Y - 1];
                                selection = maze[current.X, current.Y - 2];
                            }

                            choices.Remove(choice);
                        }

                        //If there are no valid neighbors, back up to the previous tile
                        if (selection == null)
                        {
                            animationStack.Pop();
                            if (animationStack.Count > 0)
                                animationStack.Pop();
                        }
                        else
                        {
                            animationStack.Push(preSelection);
                            animationStack.Push(selection);

                            selection.type = TileType.Empty;
                            preSelection.type = TileType.Empty;

                            preSelection.Visited = true;
                            selection.Visited = true;
                        }

                        //If the current tile is the end, quit the search
                        if (animationStack.Count == 0)
                        {
                            animationMode = -1;
                        }
                    }
                    #endregion
                    break;
                default:
                    break;
            }

            if(currentState.LeftButton == ButtonState.Pressed || currentState.RightButton == ButtonState.Pressed)
            {
                animationMode = -1;
            }

            prevKBstate = kbState;
        }

        #region DijikstraAlgorith
        public List<Tile> DijikstraAlgorith()
        {
            ResetTilesVisitedAndDistance();

            Tile currentNode = end;

            currentNode.totalDistance = 0;

            while (currentNode != null)
            {
                currentNode.Visited = true;

                if (IsTileValid(currentNode.X+1,currentNode.Y) && currentNode.totalDistance + 1 < maze[currentNode.X + 1, currentNode.Y].totalDistance)
                {
                    maze[currentNode.X + 1, currentNode.Y].totalDistance = currentNode.totalDistance + 1;
                }
                if (IsTileValid(currentNode.X - 1, currentNode.Y) && currentNode.totalDistance + 1 < maze[currentNode.X - 1, currentNode.Y].totalDistance)
                {
                    maze[currentNode.X - 1, currentNode.Y].totalDistance = currentNode.totalDistance + 1;
                }
                if (IsTileValid(currentNode.X, currentNode.Y+1) && currentNode.totalDistance + 1 < maze[currentNode.X, currentNode.Y+1].totalDistance)
                {
                    maze[currentNode.X , currentNode.Y+1].totalDistance = currentNode.totalDistance + 1;
                }
                if (IsTileValid(currentNode.X, currentNode.Y-1) && currentNode.totalDistance + 1 < maze[currentNode.X, currentNode.Y-1].totalDistance)
                {
                    maze[currentNode.X, currentNode.Y-1].totalDistance = currentNode.totalDistance + 1;
                }

                Tile newCurrent = null;

                for (int i = 0; i < maze.GetLength(0); i++)
                {
                    for (int j = 0; j < maze.GetLength(1); j++)
                    {
                        if (!maze[i,j].Visited && (newCurrent == null || maze[i,j].totalDistance < newCurrent.totalDistance))
                        {
                            newCurrent = maze[i, j];
                        }
                    }
                }

                currentNode = newCurrent;
            }

            foreach (Tile item in maze)
            {
                if (item != null)
                {
                    item.Visited = false;
                }
            }

            Stack<Tile> stack = new Stack<Tile>();

            stack.Push(start);
            start.Visited = true;

            //Loop while we have tiles in the stack
            while (stack.Count != 0)
            {
                Tile current = stack.Peek();

                Tile nextMove = null;

                List<int> choices = new List<int> { 1, 2, 3, 4 };

                Tile selection = null;

                while (choices.Count != 0 && selection == null)
                {
                    int choice = choices[rng.Next(0, choices.Count)];
                    //Check the neighbors of the current Tile, Put them on stack if valid
                    if (choice == 1 && IsTileValid(current.X + 1, current.Y) && maze[current.X + 1, current.Y].totalDistance < current.totalDistance)
                    {
                        nextMove = maze[current.X + 1, current.Y];
                    }
                    else if (choice == 2 && IsTileValid(current.X - 1, current.Y) && maze[current.X - 1, current.Y].totalDistance < current.totalDistance)
                    {
                        nextMove = maze[current.X - 1, current.Y];
                    }
                    else if (choice == 3 && IsTileValid(current.X, current.Y + 1) && maze[current.X, current.Y + 1].totalDistance < current.totalDistance)
                    {
                        nextMove = maze[current.X, current.Y + 1];
                    }
                    else if (choice == 4 && IsTileValid(current.X, current.Y - 1) && maze[current.X, current.Y - 1].totalDistance < current.totalDistance)
                    {
                        nextMove = maze[current.X, current.Y - 1];
                    }

                    choices.Remove(choice);
                }

                if(nextMove != null)
                {
                    stack.Push(nextMove);
                    nextMove.Visited = true;
                }

                //If there are no valid neighbors, back up to the previous tile
                if(nextMove == null)
                {
                    stack.Pop();
                }

                //If the current tile is the end, quit the search
                if (stack.Count == 0 || stack.Peek().type == TileType.End)
                {
                    break;
                }

            }


            // 3. ADD PATH TO SOLUTION LIST ***********************************
            List<Tile> path = new List<Tile>();
            //    - Add verts found during the search to the "path" List above
            //    - You can use the path list's AddRange() method to make this easier

            path.AddRange(stack);

            // All done! This should now be returning the full "path"
            return path;
        }
        #endregion

        #region DijikstraAlgorith Screen Wrap
        public List<Tile> DijikstraAlgorithScreenWrap()
        {
            ResetTilesVisitedAndDistance();

            Tile currentNode = end;

            currentNode.totalDistance = 0;

            while (currentNode != null)
            {
                currentNode.Visited = true;

                if (IsTileValid(currentNode.X + 1, currentNode.Y) && currentNode.totalDistance + 1 < maze[currentNode.X + 1, currentNode.Y].totalDistance)
                {
                    maze[currentNode.X + 1, currentNode.Y].totalDistance = currentNode.totalDistance + 1;
                }
                else if (!InRange(currentNode.X + 1, currentNode.Y) && IsTileValid(currentNode.X + 1-width, currentNode.Y) && currentNode.totalDistance + 1 < maze[currentNode.X + 1-width, currentNode.Y].totalDistance)
                {
                    maze[currentNode.X + 1-width, currentNode.Y].totalDistance = currentNode.totalDistance + 1;
                }

                if (IsTileValid(currentNode.X - 1, currentNode.Y) && currentNode.totalDistance + 1 < maze[currentNode.X - 1, currentNode.Y].totalDistance)
                {
                    maze[currentNode.X - 1, currentNode.Y].totalDistance = currentNode.totalDistance + 1;
                }
                else if (!InRange(currentNode.X - 1, currentNode.Y) && IsTileValid(currentNode.X - 1+width, currentNode.Y) && currentNode.totalDistance + 1 < maze[currentNode.X - 1+width, currentNode.Y].totalDistance)
                {
                    maze[currentNode.X - 1+width, currentNode.Y].totalDistance = currentNode.totalDistance + 1;
                }

                if (IsTileValid(currentNode.X, currentNode.Y + 1) && currentNode.totalDistance + 1 < maze[currentNode.X, currentNode.Y + 1].totalDistance)
                {
                    maze[currentNode.X, currentNode.Y + 1].totalDistance = currentNode.totalDistance + 1;
                }
                else if (!InRange(currentNode.X, currentNode.Y + 1) && IsTileValid(currentNode.X, currentNode.Y + 1-height) && currentNode.totalDistance + 1 < maze[currentNode.X, currentNode.Y + 1-height].totalDistance)
                {
                    maze[currentNode.X, currentNode.Y + 1-height].totalDistance = currentNode.totalDistance + 1;
                }

                if (IsTileValid(currentNode.X, currentNode.Y - 1) && currentNode.totalDistance + 1 < maze[currentNode.X, currentNode.Y - 1].totalDistance)
                {
                    maze[currentNode.X, currentNode.Y - 1].totalDistance = currentNode.totalDistance + 1;
                }
                else if (!InRange(currentNode.X, currentNode.Y - 1) && IsTileValid(currentNode.X, currentNode.Y - 1+height) && currentNode.totalDistance + 1 < maze[currentNode.X, currentNode.Y - 1+height].totalDistance)
                {
                    maze[currentNode.X, currentNode.Y - 1+height].totalDistance = currentNode.totalDistance + 1;
                }

                Tile newCurrent = null;

                for (int i = 0; i < maze.GetLength(0); i++)
                {
                    for (int j = 0; j < maze.GetLength(1); j++)
                    {
                        if (!maze[i, j].Visited && (newCurrent == null || maze[i, j].totalDistance < newCurrent.totalDistance))
                        {
                            newCurrent = maze[i, j];
                        }
                    }
                }

                currentNode = newCurrent;
            }

            foreach (Tile item in maze)
            {
                if (item != null)
                {
                    item.Visited = false;
                }
            }

            Stack<Tile> stack = new Stack<Tile>();

            stack.Push(start);
            start.Visited = true;

            //Loop while we have tiles in the stack
            while (stack.Count != 0)
            {
                Tile current = stack.Peek();

                Tile nextMove = null;

                List<int> choices = new List<int> { 1, 2, 3, 4 };

                Tile selection = null;

                while (choices.Count != 0 && selection == null)
                {
                    int choice = choices[rng.Next(0, choices.Count)];
                    //Check the neighbors of the current Tile, Put them on stack if valid
                    if (choice == 1 && IsTileValid(current.X + 1, current.Y) && maze[current.X + 1, current.Y].totalDistance < current.totalDistance)
                    {
                        nextMove = maze[current.X + 1, current.Y];
                    }
                    else if (choice == 1 && !InRange(current.X + 1, current.Y) && IsTileValid(current.X + 1-width, current.Y) && maze[current.X + 1-width, current.Y].totalDistance < current.totalDistance)
                    {
                        nextMove = maze[current.X + 1-width, current.Y];
                    }

                    else if (choice == 2 && IsTileValid(current.X - 1, current.Y) && maze[current.X - 1, current.Y].totalDistance < current.totalDistance)
                    {
                        nextMove = maze[current.X - 1, current.Y];
                    }
                    else if (choice == 2 && !InRange(current.X - 1, current.Y) && IsTileValid(current.X - 1+width, current.Y) && maze[current.X - 1+width, current.Y].totalDistance < current.totalDistance)
                    {
                        nextMove = maze[current.X - 1+width, current.Y];
                    }

                    else if (choice == 3 && IsTileValid(current.X, current.Y + 1) && maze[current.X, current.Y + 1].totalDistance < current.totalDistance)
                    {
                        nextMove = maze[current.X, current.Y + 1];
                    }
                    else if (choice == 3 && !InRange(current.X, current.Y + 1) && IsTileValid(current.X, current.Y + 1-height) && maze[current.X, current.Y + 1-height].totalDistance < current.totalDistance)
                    {
                        nextMove = maze[current.X, current.Y + 1-height];
                    }

                    else if (choice == 4 && IsTileValid(current.X, current.Y - 1) && maze[current.X, current.Y - 1].totalDistance < current.totalDistance)
                    {
                        nextMove = maze[current.X, current.Y - 1];
                    }
                    else if (choice == 4 && !InRange(current.X, current.Y - 1) && IsTileValid(current.X, current.Y - 1+height) && maze[current.X, current.Y - 1+height].totalDistance < current.totalDistance)
                    {
                        nextMove = maze[current.X, current.Y - 1+height];
                    }

                    choices.Remove(choice);
                }

                if (nextMove != null)
                {
                    stack.Push(nextMove);
                    nextMove.Visited = true;
                }

                //If there are no valid neighbors, back up to the previous tile
                if (nextMove == null)
                {
                    stack.Pop();
                }

                //If the current tile is the end, quit the search
                if (stack.Count == 0 || stack.Peek().type == TileType.End)
                {
                    break;
                }

            }


            // 3. ADD PATH TO SOLUTION LIST ***********************************
            List<Tile> path = new List<Tile>();
            //    - Add verts found during the search to the "path" List above
            //    - You can use the path list's AddRange() method to make this easier

            path.AddRange(stack);

            // All done! This should now be returning the full "path"
            return path;
        }
        #endregion

        #region DepthSearch
        public List<Tile> DepthSearch()
        {
            ResetTilesVisitedAndDistance();

            Stack<Tile> stack = new Stack<Tile>();

            stack.Push(start);
            start.Visited = true;

            //Loop while we have tiles in the stack
            while (stack.Count != 0)
            {
                Tile current = stack.Peek();

                List<int> choices = new List<int> { 1, 2, 3, 4 };

                Tile selection = null;

                while (choices.Count != 0 && selection == null)
                {
                    int choice = choices[rng.Next(0,choices.Count)];
                    //Check the neighbors of the current Tile, Put them on stack if valid
                    if (IsTileValid(current.X + 1, current.Y) && choice == 1)
                    {
                        selection = maze[current.X + 1, current.Y];
                    }
                    else if (IsTileValid(current.X - 1, current.Y) && choice == 2)
                    {
                        selection = maze[current.X - 1, current.Y];
                    }
                    else if (IsTileValid(current.X, current.Y + 1) && choice == 3)
                    {
                        selection = maze[current.X, current.Y + 1];
                    }
                    else if (IsTileValid(current.X, current.Y - 1) && choice == 4)
                    {
                        selection = maze[current.X, current.Y - 1];
                    }

                    choices.Remove(choice);
                }

                //If there are no valid neighbors, back up to the previous tile
                if (selection == null)
                {
                    stack.Pop();
                }
                else
                {
                    stack.Push(selection);
                    selection.Visited = true;
                }

                //If the current tile is the end, quit the search
                if (stack.Count == 0 || stack.Peek().type == TileType.End)
                {
                    break;
                }

            }


            // 3. ADD PATH TO SOLUTION LIST ***********************************
            List<Tile> path = new List<Tile>();
            //    - Add verts found during the search to the "path" List above
            //    - You can use the path list's AddRange() method to make this easier

            path.AddRange(stack);

            // All done! This should now be returning the full "path"
            return path;
        }
        #endregion

        #region BreadthSearch
        public List<Tile> BreadthSearch()
        {
            ResetTilesVisitedAndDistance();

            Queue<Tile> queue = new Queue<Tile>();

            queue.Enqueue(start);
            start.Visited = true;

            //Loop while we have tiles in the stack
            while (queue.Count != 0)
            {
                Tile current = queue.Peek();

                //Check the neighbors of the current Tile, Put them on stack if valid
                if (IsTileValid(current.X + 1, current.Y))
                {
                    queue.Enqueue(maze[current.X + 1, current.Y]);
                    maze[current.X + 1, current.Y].Visited = true;
                }
                else if (IsTileValid(current.X - 1, current.Y))
                {
                    queue.Enqueue(maze[current.X - 1, current.Y]);
                    maze[current.X - 1, current.Y].Visited = true;
                }
                else if (IsTileValid(current.X, current.Y + 1))
                {
                    queue.Enqueue(maze[current.X, current.Y + 1]);
                    maze[current.X, current.Y + 1].Visited = true;
                }
                else if (IsTileValid(current.X, current.Y - 1))
                {
                    queue.Enqueue(maze[current.X, current.Y - 1]);
                    maze[current.X, current.Y - 1].Visited = true;
                }
                //If there are no valid neighbors, back up to the previous tile
                else
                {
                    queue.Dequeue();
                }

                //If the current tile is the end, quit the search
                if (queue.Count == 0 || queue.Peek().type == TileType.End)
                {
                    break;
                }

            }


            // 3. ADD PATH TO SOLUTION LIST ***********************************
            List<Tile> path = new List<Tile>();
            //    - Add verts found during the search to the "path" List above
            //    - You can use the path list's AddRange() method to make this easier

            path.AddRange(queue);

            // All done! This should now be returning the full "path"
            return path;
        }
        #endregion

        #region DepthSearch Screen Wrap
        public List<Tile> DepthSearchScreenWrap()
        {
            ResetTilesVisitedAndDistance();

            Stack<Tile> stack = new Stack<Tile>();

            stack.Push(start);
            start.Visited = true;

            //Loop while we have tiles in the stack
            while (stack.Count != 0)
            {
                Tile current = stack.Peek();

                List<int> choices = new List<int> { 1, 2, 3, 4 };

                Tile selection = null;

                while (choices.Count != 0 && selection == null)
                {
                    int choice = choices[rng.Next(0, choices.Count)];
                    //Check the neighbors of the current Tile, Put them on stack if valid

                    //Check the neighbors of the current Tile, Put them on stack if valid
                    if (IsTileValid(current.X + 1, current.Y) && choice == 1)
                    {
                        selection = maze[current.X + 1, current.Y];
                    }
                    else if (!InRange(current.X + 1, current.Y) && IsTileValid(current.X + 1 - width, current.Y) && choice == 1)
                    {
                        selection = maze[current.X + 1 - width, current.Y];
                    }

                    else if (IsTileValid(current.X - 1, current.Y) && choice == 2)
                    {
                        selection = maze[current.X - 1, current.Y];
                    }
                    else if (!InRange(current.X - 1, current.Y) && IsTileValid(current.X - 1 + width, current.Y) && choice == 2)
                    {
                        selection = maze[current.X - 1 + width, current.Y];
                    }

                    else if (IsTileValid(current.X, current.Y + 1) && choice == 3)
                    {
                        selection = maze[current.X, current.Y + 1];
                    }
                    else if (!InRange(current.X, current.Y + 1) && IsTileValid(current.X, current.Y + 1 - height) && choice == 3)
                    {
                        selection = maze[current.X, current.Y + 1 - height];
                    }

                    else if (IsTileValid(current.X, current.Y - 1) && choice == 4)
                    {
                        selection = maze[current.X, current.Y - 1];
                    }
                    else if (!InRange(current.X, current.Y - 1) && IsTileValid(current.X, current.Y - 1 + height) && choice == 4)
                    {
                        selection = maze[current.X, current.Y - 1 + height];
                    }

                    choices.Remove(choice);
                }

                //If there are no valid neighbors, back up to the previous tile
                if (selection == null)
                {
                    stack.Pop();
                }
                else
                {
                    stack.Push(selection);
                    selection.Visited = true;
                }


                //If the current tile is the end, quit the search
                if (stack.Count == 0 || stack.Peek().type == TileType.End)
                {
                    break;
                }

            }


            // 3. ADD PATH TO SOLUTION LIST ***********************************
            List<Tile> path = new List<Tile>();
            //    - Add verts found during the search to the "path" List above
            //    - You can use the path list's AddRange() method to make this easier

            path.AddRange(stack);

            // All done! This should now be returning the full "path"
            return path;
        }
        #endregion

        #region Maze Generator
        public void MakeMaze(int startType)
        {
            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    CreateSquare(TileType.Wall, i, j);
                }
            }

            Stack<Tile> stack = new Stack<Tile>();

            int xStart = rng.Next(0, width);
            int yStart = rng.Next(0, height);

            if(xStart%2 == 0)
            {
                if(xStart+1 < width)
                {
                    xStart++;
                }
                else
                {
                    xStart--;
                }
            }

            if (yStart % 2 == 0)
            {
                if (yStart + 1 < height)
                {
                    yStart++;
                }
                else
                {
                    yStart--;
                }
            }

            stack.Push(maze[xStart,yStart]);
            
            //Commenting out allows for loop to exist at start point
            maze[xStart, yStart].Visited = true;
            maze[xStart, yStart].type = TileType.Empty;

            //Loop while we have tiles in the stack
            while (stack.Count != 0)
            {
                Tile current = stack.Peek();

                List<int> choices = new List<int> { 1, 2, 3, 4 };

                Tile preSelection = null;
                Tile selection = null;

                while (choices.Count != 0 && selection == null)
                {
                    int choice = choices[rng.Next(0, choices.Count)];
                    //Check the neighbors of the current Tile, Put them on stack if valid
                    if (IsTileValidEmpty(current.X + 2, current.Y) && choice == 1)
                    {
                        preSelection = maze[current.X + 1, current.Y];
                        selection = maze[current.X + 2, current.Y];
                    }
                    else if (IsTileValidEmpty(current.X - 2, current.Y) && choice == 2)
                    {
                        preSelection = maze[current.X - 1, current.Y];
                        selection = maze[current.X - 2, current.Y];
                    }
                    else if (IsTileValidEmpty(current.X, current.Y + 2) && choice == 3)
                    {
                        preSelection = maze[current.X, current.Y+1];
                        selection = maze[current.X, current.Y + 2];
                    }
                    else if (IsTileValidEmpty(current.X, current.Y - 2) && choice == 4)
                    {
                        preSelection = maze[current.X, current.Y-1];
                        selection = maze[current.X, current.Y - 2];
                    }

                    choices.Remove(choice);
                }

                //If there are no valid neighbors, back up to the previous tile
                if (selection == null)
                {
                    stack.Pop();
                    if(stack.Count > 0)
                        stack.Pop();
                }
                else
                {
                    stack.Push(preSelection);
                    stack.Push(selection);

                    selection.type = TileType.Empty;
                    preSelection.type = TileType.Empty;

                    preSelection.Visited = true;
                    selection.Visited = true;
                }

                //If the current tile is the end, quit the search
                if (stack.Count == 0)
                {
                    break;
                }
            }

            if(startType == 1)
            {
                maze[0, 1].type = TileType.Start;
                start = maze[0, 1];
                maze[width - 1, height - 2].type = TileType.End;
                end = maze[width - 1, height - 2];
            }
            else if(startType == 2)
            {
                if (maze[width / 2, 1].type == TileType.Wall || maze[width / 2, height-2].type == TileType.Wall)
                {
                    maze[width / 2+1, 0].type = TileType.Start;
                    start = maze[width / 2+1, 0];
                    maze[width / 2+1, height - 1].type = TileType.End;
                    end = maze[width / 2+1, height - 1];
                }
                else
                {
                    maze[width / 2, 0].type = TileType.Start;
                    start = maze[width / 2, 0];
                    maze[width / 2, height - 1].type = TileType.End;
                    end = maze[width / 2, height - 1];
                }
            }
            else if(startType == 3)
            {
                if (maze[1, height / 2].type == TileType.Wall || maze[width - 2, height / 2].type == TileType.Wall)
                {
                    maze[0, height / 2+1].type = TileType.Start;
                    start = maze[0, height / 2+1];
                    maze[width - 1, height / 2+1].type = TileType.End;
                    end = maze[width - 1, height / 2+1];
                }
                else
                {
                    maze[0, height / 2].type = TileType.Start;
                    start = maze[0, height / 2];
                    maze[width - 1, height / 2].type = TileType.End;
                    end = maze[width - 1, height / 2];
                }
                
            }
        }
        #endregion

        public void PruneSolution(List<Tile> path)
        {
            for (int i = 0; i < path.Count; i++)
            {
                Tile current = path[i];
                for (int j = path.Count-1; j >= 0; j--)
                {
                    Tile compared = path[j];
                    if(current.X == compared.X && Math.Abs(current.Y-compared.Y) == 1 && Math.Abs(i-j) != 1)
                    {
                        path.RemoveRange(i+1, j-(i+1));
                        return;
                    }

                    if (current.Y == compared.Y && Math.Abs(current.X - compared.X) == 1 && Math.Abs(i - j) != 1)
                    {
                        path.RemoveRange(i+1, j - (i+1));
                        return;
                    }
                }
            }
        }

        private int MouseTilePosX(MouseState currentState)
        {
            if (currentState.X - pixelBuffer < 0)
                return -1;
            return (currentState.X - pixelBuffer) / (screenWidth / width);
        }

        private int MouseTilePosY(MouseState currentState)
        {
            if (currentState.Y - pixelBuffer< 0)
                return -1;
            return (currentState.Y - pixelBuffer) / (screenHeight / height);
        }

        private void ResetTilesVisitedAndDistance()
        {
            foreach (Tile item in maze)
            {
                if (item != null)
                {
                    item.Visited = false;
                    item.totalDistance = int.MaxValue;
                }
            }
        }

        public bool InRange(int x, int y)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                return true;
            }
            return false;
        }

        public bool MouseInsideRect(MouseState mouse, Rectangle rect)
        {
            if(mouse.X > rect.X && mouse.X < rect.X + rect.Width)
            {
                if(mouse.Y > rect.Y && mouse.Y < rect.Y + rect.Height)
                {
                    return true;
                }
            }
            return false;
        }

        private void ClearDrawings()
        {
            animationStack.Clear();
            solvedMaze.Clear();
        }

        private string UpdateTextBox(KeyboardState kbState, string textBox)
        {
            foreach (Keys item in kbState.GetPressedKeys())
            {
                switch (item)
                {
                    case Keys.D0:
                    case Keys.D1:
                    case Keys.D2:
                    case Keys.D3:
                    case Keys.D4:
                    case Keys.D5:
                    case Keys.D6:
                    case Keys.D7:
                    case Keys.D8:
                    case Keys.D9:
                    case Keys.NumPad0:
                    case Keys.NumPad1:
                    case Keys.NumPad2:
                    case Keys.NumPad3:
                    case Keys.NumPad4:
                    case Keys.NumPad5:
                    case Keys.NumPad6:
                    case Keys.NumPad7:
                    case Keys.NumPad8:
                    case Keys.NumPad9:
                        if (SingleKeyPress(kbState, item) && textBox.Length < 3)
                        {
                            textBox += item.ToString().Replace("D", "");
                        }
                        break;
                    case Keys.Back:
                        if (SingleKeyPress(kbState, item) && textBox.Length > 0)
                        {
                            textBox = textBox.Substring(0, textBox.Length - 1);
                        }
                        break;
                    default:
                        break;
                }
            }

            return textBox;
        }

        public bool IsTileValid(int x, int y)
        {
            //In range of array
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                //If Tile is not a wall and has not been visited
                if (maze[x, y].type != TileType.Wall && !maze[x, y].Visited)
                {
                    return true;
                }
            }
            return false;
        }

        public bool SingleKeyPress(KeyboardState state, Keys key)
        {
            if (state.IsKeyDown(key) && !prevKBstate.IsKeyDown(key))
            {
                return true;
            }
            return false;
        }

        //Single Key Press + Start and End exist
        private bool SingleKeyPressMazeSolve(KeyboardState state, Keys key)
        {
            if (state.IsKeyDown(key) && !prevKBstate.IsKeyDown(key) && start != null && end != null)
            {
                return true;
            }
            return false;
        }

        public bool IsTileValidEmpty(int x, int y)
        {
            //In range of array
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                //If Tile is not a wall and has not been visited
                if (maze[x, y].type != TileType.Empty && !maze[x, y].Visited)
                {
                    return true;
                }
            }
            return false;
        }

        public void Draw(SpriteBatch sb, GameTime gameTime)
        {
            animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (Tile tile in maze)
            {
                if (tile != null)
                {
                    if (solvedMaze.Contains(tile))
                    {
                        float value = solvedMaze.IndexOf(tile) / ((float)solvedMaze.Count);
                        tile.Draw(sb,new Color(1-value,value,0),drawWeights);
                    }
                    else if (animationStack.Contains(tile))
                    {
                        List<Tile> tiles = new List<Tile>();
                        tiles.AddRange(animationStack);
                        float value = tiles.IndexOf(tile) / ((float)animationStack.Count);
                        tile.Draw(sb, new Color(1 - value, value, 0), drawWeights);
                    }
                    else
                    {
                        tile.Draw(sb,drawWeights);
                    }
                }
            }
            Vector2 widthWordSize = Game1.arial.MeasureString(widthSizeTextBox);
            Vector2 heightWordSize = Game1.arial.MeasureString(heightSizeTextBox);

            if (!writingWidthSize)
                sb.Draw(Game1.whiteSquare, WidthTextRect, Color.Beige);
            else
                sb.Draw(Game1.whiteSquare, WidthTextRect, Color.LightGreen);
            sb.DrawString(Game1.arial, "Width", new Vector2(screenWidth + pixelBuffer * 2, pixelBuffer + 115), Color.Black);
            sb.DrawString(Game1.arial, widthSizeTextBox, new Vector2(screenWidth + (int)(pixelBuffer*1.5) + 50 - (int)(widthWordSize.X/2), pixelBuffer + 156), Color.Black);


            if (!writingHeightSize)
                sb.Draw(Game1.whiteSquare, HeightTextRect, Color.Beige);
            else
                sb.Draw(Game1.whiteSquare, HeightTextRect, Color.LightGreen);
            sb.DrawString(Game1.arial, "Height", new Vector2(screenWidth + pixelBuffer * 2-5,pixelBuffer+15), Color.Black);
            sb.DrawString(Game1.arial, heightSizeTextBox, new Vector2(screenWidth + (int)(pixelBuffer * 1.5) + 50 - (int)(heightWordSize.X / 2), pixelBuffer + 56), Color.Black);

            sb.Draw(Game1.whiteSquare, SizeChangeConfirm, Color.Beige);
        }
    }
}
