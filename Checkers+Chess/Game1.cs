using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BoardGames
{
    enum TeamColor
    {
        Red,
        Black,
        White
    }
    
    enum ChessPieceType
    {
        Pawn,
        Rook,
        Knight,
        Bishop,
        Queen,
        King
    }

    enum GameMode
    {
        Checkers,
        Chess
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static Texture2D whiteTile;
        public static Texture2D blackTile;
        public static Texture2D grayTile;
        public static Texture2D moveCircle;
        public static Texture2D takePieceCircle;

        public static Texture2D[] redChecker;
        public static Texture2D[] blackChecker;
        public static Texture2D chessTextures;

        public static SpriteFont Arial16;
        public static SpriteFont gameOverText;

        GameMode currentMode = GameMode.Chess;

        KeyboardState prevFrameKBState;

        CheckersManager checkersManager;
        ChessManager chessManager;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            whiteTile = Content.Load<Texture2D>("WhiteTile");
            blackTile = Content.Load<Texture2D>("GrayTile");
            grayTile = Content.Load<Texture2D>("Light Gray Tile");
            moveCircle = Content.Load<Texture2D>("TileMoveLabelNoBorder");
            takePieceCircle = Content.Load<Texture2D>("TakeablePieceCircle");

            redChecker = new Texture2D[2];
            redChecker[0] = Content.Load<Texture2D>("RedChecker");
            redChecker[1] = Content.Load<Texture2D>("RedKingChecker");
            
            blackChecker = new Texture2D[2];
            blackChecker[0] = Content.Load<Texture2D>("BlackChecker");
            blackChecker[1] = Content.Load<Texture2D>("BlackKingChecker");

            chessTextures = Content.Load<Texture2D>("Chess Pieces");

            Arial16 = Content.Load<SpriteFont>("Arial");
            gameOverText = Content.Load<SpriteFont>("LargeGameOverText");

            _graphics.PreferredBackBufferWidth = 1000;
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.ApplyChanges();

            checkersManager = new CheckersManager();
            chessManager = new ChessManager();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            KeyboardState kbState = Keyboard.GetState();

            switch (currentMode)
            {
                case GameMode.Chess:
                    chessManager.Update();
                    break;
                case GameMode.Checkers:
                    checkersManager.Update();
                    break;
            }

            if(kbState.IsKeyDown(Keys.D1) && prevFrameKBState.IsKeyUp(Keys.D1))
            {
                currentMode = GameMode.Chess;
            }
            if (kbState.IsKeyDown(Keys.D2) && prevFrameKBState.IsKeyUp(Keys.D2))
            {
                currentMode = GameMode.Checkers;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Tan);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            switch (currentMode)
            {
                case GameMode.Chess:
                    chessManager.Draw(_spriteBatch);
                    break;
                case GameMode.Checkers:
                    checkersManager.Draw(_spriteBatch);
                    break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
