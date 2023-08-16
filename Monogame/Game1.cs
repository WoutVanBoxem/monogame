using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Media;


namespace Monogame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Player player;
        Rectangle floor;
        Texture2D pixel;
        Texture2D spriteSheet;
        int frameWidth = 10;
        int frameHeight = 10;
        List<Solid> solids = new List<Solid>();
        SpriteFont defaultFont;
        Rectangle startButtonRectangle;
        Song menuSong;
        Song gameSong;
        Coin coin;
        Texture2D coinSpriteSheet;
        FallingBlock fallingBlock;
        FinishFlag finishFlag;
        Texture2D finishFlagSprite;




        public enum GameState
        {
            Menu,
            Playing,
            Paused
        }

        public GameState CurrentGameState = GameState.Menu;

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
            defaultFont = Content.Load<SpriteFont>("DefaultFont");
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteSheet = Content.Load<Texture2D>("hero_spritesheet");
            coinSpriteSheet = Content.Load<Texture2D>("coin_sprite");
            menuSong = Content.Load<Song>("menu");
            gameSong = Content.Load<Song>("inGame");
           
            frameWidth = 80;
            frameHeight = 63;
            int playerX = 10;
            int playerY = _graphics.PreferredBackBufferHeight - 20 - frameHeight;
            player = new Player(new Vector2(playerX, playerY), _graphics, frameWidth, frameHeight);
            floor = new Rectangle(0, _graphics.PreferredBackBufferHeight - 20, _graphics.PreferredBackBufferWidth, 20);
            solids.Add(new Solid(floor));
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            Vector2 startButtonPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2 - 100, _graphics.PreferredBackBufferHeight / 2 - 25);
            startButtonRectangle = new Rectangle((int)startButtonPosition.X, (int)startButtonPosition.Y, 200, 50); // 200x50 is de grootte van de knop
            Vector2 coinPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2 - 8, _graphics.PreferredBackBufferHeight - 110); // 8 is de helft van de munt breedte (16) en 32 is de hoogte van de munt boven de vloer.
            coin = new Coin(coinSpriteSheet, coinPosition);
            fallingBlock = new FallingBlock(GraphicsDevice, new Vector2(coin.Position.X - 50, -40));
            finishFlagSprite = Content.Load<Texture2D>("finish");
            finishFlag = new FinishFlag(finishFlagSprite,new Vector2(coinPosition.X + 350, coinPosition.Y));


        }


        protected override void Update(GameTime gameTime)
        {
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
                CurrentGameState = GameState.Playing;
                MediaPlayer.Stop();
                MediaPlayer.Play(gameSong);
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

            if (CurrentGameState == GameState.Playing)
            {
                coin.Update(gameTime);

                // Player update eerst
                player.Update(gameTime, solids, keyboardState);

                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    player.Position -= new Vector2(2, 0);
                    player.FacingRight = false;
                    player.CurrentAnimation = Player.PlayerAnimation.Walking;
                }
                else if (keyboardState.IsKeyDown(Keys.Right))
                {
                    player.Position += new Vector2(2, 0);
                    player.FacingRight = true;
                    player.CurrentAnimation = Player.PlayerAnimation.Walking;
                }
                else
                {
                    player.CurrentAnimation = Player.PlayerAnimation.Standing;
                }

                if (!coin.IsCollected && player.GetBoundingBox().Intersects(coin.GetBoundingBox()))
                {
                    coin.IsCollected = true;
                }

                // Controleren afstand tot coin en activeren van de balk
                float distanceToCoin = Vector2.Distance(player.Position, coin.Position);
                if (distanceToCoin < 120  && !fallingBlock.IsActive) // controleer of de balk niet al actief is
                {
                    fallingBlock.IsActive = true;
                }
                if (player.GetBoundingBox().Intersects(fallingBlock.GetBoundingBox()))
                {
                    ResetGame();
                    CurrentGameState = GameState.Menu;
                }
                if (player.GetBoundingBox().Intersects(finishFlag.GetBoundingBox()))
                {
                    ResetGame();
                    CurrentGameState = GameState.Menu;
                }



                // Update de vallende balk
                fallingBlock.Update();
            }



            base.Update(gameTime);
        }




        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            _spriteBatch.Draw(texture: pixel, destinationRectangle: floor, color: Color.Black);

            if (CurrentGameState == GameState.Menu || CurrentGameState == GameState.Paused)
            {
                _spriteBatch.Draw(pixel, startButtonRectangle, Color.Gray); // De grijze knop

                string buttonText = "Press enter to start";
                Vector2 stringSize = defaultFont.MeasureString(buttonText);
                Vector2 stringPosition = new Vector2(startButtonRectangle.Center.X - stringSize.X / 2, startButtonRectangle.Center.Y - stringSize.Y / 2);

                _spriteBatch.DrawString(defaultFont, buttonText, stringPosition, Color.White); // Tekst in het midden van de knop
            }

            if (CurrentGameState == GameState.Playing)
            {
                if (!coin.IsCollected)
                {
                    coin.Draw(_spriteBatch);
                }

                player.Draw(_spriteBatch, spriteSheet, frameWidth);
                finishFlag.Draw(_spriteBatch);

            }
            fallingBlock.Draw(_spriteBatch);


            _spriteBatch.End();
            base.Draw(gameTime);
        }
        private void ResetGame()
        {
            // Reset player
            int playerX = 10;
            int playerY = _graphics.PreferredBackBufferHeight - 20 - frameHeight;
            player = new Player(new Vector2(playerX, playerY), _graphics, frameWidth, frameHeight);

            // Reset coin
            Vector2 coinPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2 - 8, _graphics.PreferredBackBufferHeight - 110);
            coin = new Coin(coinSpriteSheet, coinPosition);

            // Reset falling block
            fallingBlock = new FallingBlock(GraphicsDevice, new Vector2(coin.Position.X - 50, -40));

            // Andere objecten die je wilt resetten...
        }


    }
}