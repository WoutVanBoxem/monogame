using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System;


namespace Monogame
{
    public class Game1 : Game
    {
        public ILevel CurrentLevel { get; set; }
        public Player player;
        public List<Bullet> bullets = new List<Bullet>();
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public Texture2D spriteSheet;
        public int frameWidth = 80;
        public int frameHeight = 10;
        public List<Solid> solids = new List<Solid>();
        public SpriteFont defaultFont;
        public Rectangle startButtonRectangle;
        public Song menuSong;
        public Song gameSong;
        public Texture2D pixel; 
        public Rectangle floor;
        public GameState CurrentGameState = GameState.Menu;
        private TimeSpan bulletCooldown = TimeSpan.FromMilliseconds(200);
        private TimeSpan lastBulletTime;
        public GraphicsDeviceManager GraphicsManager => _graphics;
        public int FrameWidth => frameWidth;
        public int FrameHeight => frameHeight;
        public Rectangle Level1ButtonRectangle;
        public Rectangle Level2ButtonRectangle;
        private TimeSpan lastStateChangeTime;


        public enum GameState
        {
            Menu,
            Playing,
            Paused,
            GameOver,
            Win
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            CurrentLevel = new Level1(this);
            CurrentLevel.LoadContent();
            defaultFont = Content.Load<SpriteFont>("DefaultFont");
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteSheet = Content.Load<Texture2D>("hero_spritesheet");
            menuSong = Content.Load<Song>("menu");
            gameSong = Content.Load<Song>("inGame");
            floor = new Rectangle(0, _graphics.PreferredBackBufferHeight - 20, _graphics.PreferredBackBufferWidth, 20);
            solids.Add(new Solid(floor));
            Vector2 level1ButtonPosition = new Vector2(_graphics.PreferredBackBufferWidth / 3 - 100, _graphics.PreferredBackBufferHeight / 2 + 40);
            Level1ButtonRectangle = new Rectangle((int)level1ButtonPosition.X, (int)level1ButtonPosition.Y, 200, 50);

            Vector2 level2ButtonPosition = new Vector2(2 * _graphics.PreferredBackBufferWidth / 3 - 100, _graphics.PreferredBackBufferHeight / 2 + 40);
            Level2ButtonRectangle = new Rectangle((int)level2ButtonPosition.X, (int)level2ButtonPosition.Y, 200, 50);

        }

        protected override void Update(GameTime gameTime)
        {
            CurrentLevel.Update(gameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.P) && CurrentGameState == GameState.Playing)
            {
                CurrentGameState = GameState.Paused;
                MediaPlayer.Stop();
                MediaPlayer.Play(menuSong);
            }
            else if (keyboardState.IsKeyDown(Keys.Enter) && (CurrentGameState == GameState.Menu || CurrentGameState == GameState.Paused))
            {
                
                if (gameTime.TotalGameTime - lastStateChangeTime > TimeSpan.FromMilliseconds(500))
                {
                    CurrentGameState = GameState.Playing;
                    lastStateChangeTime = gameTime.TotalGameTime;  
                    MediaPlayer.Stop();
                    MediaPlayer.Play(gameSong);
                }
            }
            if (CurrentGameState == GameState.Menu)
            {
                if (MediaPlayer.State != MediaState.Playing)
                {
                    MediaPlayer.Play(menuSong);
                }
            }
            else if (CurrentGameState == GameState.Playing)
            {
                if (MediaPlayer.State != MediaState.Playing)
                {
                    MediaPlayer.Play(gameSong);
                }
            }

            if (CurrentGameState == GameState.Menu || CurrentGameState == GameState.Paused)
            {
                MouseState mouseState = Mouse.GetState();

                if (Level1ButtonRectangle.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed)
                {
                    CurrentLevel = new Level1(this);
                    CurrentLevel.LoadContent();
                    CurrentGameState = GameState.Playing;
                    MediaPlayer.Stop();
                    MediaPlayer.Play(gameSong);
                }
                else if (Level2ButtonRectangle.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed)
                {
                    CurrentLevel = new Level2(this);
                    CurrentLevel.LoadContent();
                    CurrentGameState = GameState.Playing;
                    MediaPlayer.Stop();
                    MediaPlayer.Play(gameSong);
                }
            }
            if (CurrentGameState == GameState.GameOver)
            {
                if (keyboardState.IsKeyDown(Keys.Enter) && (gameTime.TotalGameTime - lastStateChangeTime > TimeSpan.FromMilliseconds(500)))
                {
                    player.HealthPoints = 5;
                    CurrentGameState = GameState.Menu;
                    lastStateChangeTime = gameTime.TotalGameTime;
                }
            }
            if (CurrentGameState == GameState.Win)
            {
                if (keyboardState.IsKeyDown(Keys.Enter) && (gameTime.TotalGameTime - lastStateChangeTime > TimeSpan.FromMilliseconds(500)))
                {
                    player.HealthPoints = 5;
                    CurrentGameState = GameState.Menu;
                    lastStateChangeTime = gameTime.TotalGameTime;
                }
            }

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();


            CurrentLevel.Draw(_spriteBatch);


            _spriteBatch.Draw(texture: pixel, destinationRectangle: floor, color: Color.Black);

            if (CurrentGameState == GameState.Menu || CurrentGameState == GameState.Paused)
            {
                _spriteBatch.Draw(pixel, startButtonRectangle, Color.Gray);

                string buttonText = "Press enter to start";
                Vector2 stringSize = defaultFont.MeasureString(buttonText);
                Vector2 stringPosition = new Vector2(startButtonRectangle.Center.X - stringSize.X / 2, startButtonRectangle.Center.Y - stringSize.Y / 2);

                _spriteBatch.DrawString(defaultFont, buttonText, stringPosition, Color.White);
                _spriteBatch.Draw(pixel, Level1ButtonRectangle, Color.Gray);
                _spriteBatch.DrawString(defaultFont, "Level 1", new Vector2(Level1ButtonRectangle.Center.X - 30, Level1ButtonRectangle.Center.Y - 10), Color.White);

                _spriteBatch.Draw(pixel, Level2ButtonRectangle, Color.Gray);
                _spriteBatch.DrawString(defaultFont, "Level 2", new Vector2(Level2ButtonRectangle.Center.X - 30, Level2ButtonRectangle.Center.Y - 10), Color.White);
            }
            if(CurrentGameState==GameState.GameOver)
            {
                string gameOverText = "Je hebt verloren! Druk op Enter om terug te gaan naar het hoofdmenu.";
                Vector2 textSize = defaultFont.MeasureString(gameOverText); 
                Vector2 position = new Vector2((GraphicsDevice.Viewport.Width - textSize.X) / 2, GraphicsDevice.Viewport.Height / 2);
                _spriteBatch.DrawString(defaultFont, gameOverText, position, Color.Red);
            }
            if (CurrentGameState == GameState.Win)
            {
                string winText = "Je hebt gewonnen! Druk op Enter om terug te gaan naar het hoofdmenu.";
                Vector2 textSize = defaultFont.MeasureString(winText);
                Vector2 position = new Vector2((GraphicsDevice.Viewport.Width - textSize.X) / 2, GraphicsDevice.Viewport.Height / 2);
                _spriteBatch.DrawString(defaultFont, winText, position, Color.Green);
            }


            _spriteBatch.End();
            base.Draw(gameTime);
        }


        public void ResetGame()  
        {
            int playerX = 10;
            int playerY = _graphics.PreferredBackBufferHeight - 20 - frameHeight;
            player = new Player(new Vector2(playerX, playerY), _graphics, frameWidth, frameHeight);
        }

        public void ShootBullet(Bullet.BulletDirection direction, Vector2 bulletStartingPosition, GameTime gameTime) 
        {
            if (gameTime.TotalGameTime - lastBulletTime > bulletCooldown)
            {
                Bullet bullet = new Bullet(bulletStartingPosition, direction, GraphicsDevice);
                bullets.Add(bullet);
                lastBulletTime = gameTime.TotalGameTime;
            }
        }
    }
}
