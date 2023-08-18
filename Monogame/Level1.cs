using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace Monogame
{
    public class Level1 : BaseLevel
    {
        Coin coin;
        Texture2D coinSpriteSheet;
        FallingBlock fallingBlock;
        FinishFlag finishFlag;
        Texture2D finishFlagSprite;
        Texture2D level1Background;


        public Level1(Game1 game) : base(game)
        { }

        public override void LoadContent()
        {
            level1Background = game.Content.Load<Texture2D>("level1background");
            coinSpriteSheet = game.Content.Load<Texture2D>("coin_sprite");
            int frameWidth = game.FrameWidth;
            int frameHeight = game.FrameHeight;
            int playerX = 10;
            int playerY = game.GraphicsManager.PreferredBackBufferHeight - 20 - frameHeight;
            game.player = new Player(new Vector2(playerX, playerY), game.GraphicsManager, frameWidth, frameHeight);
            game.pixel = new Texture2D(game.GraphicsDevice, 1, 1);
            game.pixel.SetData(new[] { Color.White });
            Vector2 startButtonPosition = new Vector2(game.GraphicsManager.PreferredBackBufferWidth / 2 - 100, game.GraphicsManager.PreferredBackBufferHeight / 2 - 25);
            game.startButtonRectangle = new Rectangle((int)startButtonPosition.X, (int)startButtonPosition.Y, 200, 50);
            Vector2 coinPosition = new Vector2(game.GraphicsManager.PreferredBackBufferWidth / 2 - 8, game.GraphicsManager.PreferredBackBufferHeight - 110);
            coin = new Coin(coinSpriteSheet, coinPosition);
            fallingBlock = new FallingBlock(game.GraphicsDevice, new Vector2(coin.Position.X - 50, -40));
            finishFlagSprite = game.Content.Load<Texture2D>("finish");
            finishFlag = new FinishFlag(finishFlagSprite, new Vector2(coinPosition.X + 350, coinPosition.Y));
        }

        public override void Update(GameTime gameTime)
        {
            if (game.CurrentGameState == Game1.GameState.Playing)
            {
                coin.Update(gameTime);

                game.player.Update(gameTime, game.solids, Keyboard.GetState());

                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    game.player.Position -= new Vector2(2, 0);
                    game.player.FacingRight = false;
                    game.player.CurrentAnimation = Player.PlayerAnimation.Walking;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    game.player.Position += new Vector2(2, 0);
                    game.player.FacingRight = true;
                    game.player.CurrentAnimation = Player.PlayerAnimation.Walking;
                }
                else
                {
                    game.player.CurrentAnimation = Player.PlayerAnimation.Standing;
                }

                if (!coin.IsCollected && game.player.GetBoundingBox().Intersects(coin.GetBoundingBox()))
                {
                    coin.IsCollected = true;
                }

                float distanceToCoin = Vector2.Distance(game.player.Position, coin.Position);
                if (distanceToCoin < 120 && !fallingBlock.IsActive)
                {
                    fallingBlock.IsActive = true;
                }
                if (game.player.GetBoundingBox().Intersects(fallingBlock.GetBoundingBox()))
                {
                    game.ResetGame();
                    game.CurrentGameState = Game1.GameState.GameOver;
                }
                if (game.player.GetBoundingBox().Intersects(finishFlag.GetBoundingBox()) && coin.IsCollected)
                {
                    game.ResetGame();
                    game.CurrentGameState = Game1.GameState.Win;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    Bullet.BulletDirection direction = game.player.FacingRight ? Bullet.BulletDirection.Right : Bullet.BulletDirection.Left;
                    Vector2 bulletStartingPosition = game.player.FacingRight ?
                        new Vector2(game.player.Position.X + game.player.FrameWidth - 15, game.player.Position.Y + game.player.FrameHeight / 2+10) :
                        new Vector2(game.player.Position.X + 10, game.player.Position.Y + game.player.FrameHeight / 2+10);
                    game.ShootBullet(direction, bulletStartingPosition, gameTime);
                }

                foreach (var bullet in game.bullets)
                {
                    bullet.Update();
                }

                fallingBlock.Update();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(level1Background, new Rectangle(0, 0, game.GraphicsManager.PreferredBackBufferWidth, game.GraphicsManager.PreferredBackBufferHeight), Color.White);
            if (game.CurrentGameState == Game1.GameState.Playing)
            {
                if (!coin.IsCollected)
                {
                    coin.Draw(spriteBatch);
                }

                game.player.Draw(spriteBatch, game.spriteSheet, game.FrameWidth);
                finishFlag.Draw(spriteBatch);
            }
            fallingBlock.Draw(spriteBatch);
            foreach (var bullet in game.bullets)
            {
                bullet.Draw(spriteBatch);
            }

        }

        public override void Reset()
        {
            Vector2 coinPosition = new Vector2(game.GraphicsManager.PreferredBackBufferWidth / 2 - 8, game.GraphicsManager.PreferredBackBufferHeight - 110);
            coin = new Coin(coinSpriteSheet, coinPosition);
            fallingBlock = new FallingBlock(game.GraphicsDevice, new Vector2(coin.Position.X - 50, -40));
            game.bullets.Clear();
        }
    }
}
