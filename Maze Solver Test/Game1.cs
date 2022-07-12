using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Maze_Solver_Test
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static Texture2D blackSquare;
        public static Texture2D greenSquare;
        public static Texture2D redSquare;
        public static Texture2D whiteSquare;

        Maze maze;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            CreateMaze(101, 101);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            blackSquare = Content.Load<Texture2D>("BlackTile");
            redSquare = Content.Load<Texture2D>("redSquare");
            greenSquare = Content.Load<Texture2D>("greenSquare");
            whiteSquare = Content.Load<Texture2D>("WhiteTile");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            maze.Update(gameTime);

            base.Update(gameTime);
        }

        public void CreateMaze(int x, int y)
        {
            int size = MathHelper.Max(x, y);
            maze = new Maze(x, y, (800/size)*x, (800/size)*y);

            _graphics.PreferredBackBufferWidth = (800 / size) * x;
            _graphics.PreferredBackBufferHeight = (800 / size) * y;
            _graphics.ApplyChanges();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            maze.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
