using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
            spriteSheet = Content.Load<Texture2D>("hero_spritesheet");
            frameWidth = 80; 
            frameHeight = 63; 
            int playerX = 10; 
            int playerY = _graphics.PreferredBackBufferHeight - 20 - frameHeight; 
            player = new Player(new Vector2(playerX, playerY), _graphics, frameWidth);
            floor = new Rectangle(0, _graphics.PreferredBackBufferHeight - 20, _graphics.PreferredBackBufferWidth, 20);
            solids.Add(new Solid(floor));
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState keyboardState = Keyboard.GetState();

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

            player.Update(gameTime, solids, keyboardState);

            base.Update(gameTime);
        }



        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            _spriteBatch.Draw(texture: pixel, destinationRectangle: floor, color: Color.Black);
            player.Draw(_spriteBatch, spriteSheet, frameWidth);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}