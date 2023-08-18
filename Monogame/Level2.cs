using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace Monogame
{
    public class Level2 : BaseLevel
    {
        Texture2D level2Background;
        Texture2D shellTexture;
        Shell shell;
        private Texture2D pixel;
        private Enemy enemy;
        Texture2D enemySprite;



        public Level2(Game1 game) : base(game)
        { }

        public override void LoadContent()
        {
            level2Background = game.Content.Load<Texture2D>("level2background");
            int frameWidth = game.FrameWidth;
            int frameHeight = game.FrameHeight;
            int playerX = 10;
            int playerY = game.GraphicsManager.PreferredBackBufferHeight - 20 - frameHeight;
            game.player = new Player(new Vector2(playerX, playerY), game.GraphicsManager, frameWidth, frameHeight);
            shellTexture = game.Content.Load<Texture2D>("shell");
            pixel = new Texture2D(game.GraphicsManager.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            enemySprite = game.Content.Load<Texture2D>("hero_spritesheet"); 
            int enemyX = game.GraphicsManager.PreferredBackBufferWidth - game.FrameWidth - 10;
            int enemyY = game.GraphicsManager.PreferredBackBufferHeight - 73 - game.FrameHeight;
            enemy = new Enemy(new Vector2(enemyX, enemyY), game.FrameWidth, game.FrameHeight);



            
            shell = new Shell(shellTexture, game.GraphicsManager.PreferredBackBufferHeight, game.GraphicsManager.PreferredBackBufferWidth);
        }


        public override void Update(GameTime gameTime)
        {
            

            if (game.CurrentGameState == Game1.GameState.Playing)
            {
                
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
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    Bullet.BulletDirection direction = game.player.FacingRight ? Bullet.BulletDirection.Right : Bullet.BulletDirection.Left;
                    Vector2 bulletStartingPosition = game.player.FacingRight ?
                        new Vector2(game.player.Position.X + game.player.FrameWidth - 15, game.player.Position.Y + game.player.FrameHeight / 2 + 10) :
                        new Vector2(game.player.Position.X + 10, game.player.Position.Y + game.player.FrameHeight / 2 + 13);
                    game.ShootBullet(direction, bulletStartingPosition, gameTime);
                }
                List<Bullet> bulletsToRemove = new List<Bullet>();

                for (int i = game.bullets.Count - 1; i >= 0; i--)
                {
                    var bullet = game.bullets[i];
                    bullet.Update();

                    if (bullet.Direction == Bullet.BulletDirection.Right && bullet.GetBoundingBox().Intersects(enemy.GetBoundingBox()))
                    {
                        enemy.TakeDamage(gameTime);
                        bulletsToRemove.Add(bullet);
                    }

                    else if (bullet.Direction == Bullet.BulletDirection.Left && bullet.GetBoundingBox().Intersects(game.player.GetBoundingBox()))
                    {
                        game.player.TakeDamage(gameTime);
                        bulletsToRemove.Add(bullet);
                    }
                }

                foreach (var bullet in bulletsToRemove)
                {
                    game.bullets.Remove(bullet);
                }

                if (gameTime.TotalGameTime.TotalSeconds % 2 < 0.1)
                {
                    Bullet.BulletDirection direction = Bullet.BulletDirection.Left;
                    Vector2 bulletStartingPosition = new Vector2(enemy.Position.X - 10, enemy.Position.Y + enemy.FrameHeight / 2 + 10);
                    game.ShootBullet(direction, bulletStartingPosition, gameTime);
                }

                int playerStartY = game.GraphicsManager.PreferredBackBufferHeight - 20 - game.FrameHeight;

                if (!shell.IsDefeated && game.player.GetBoundingBox().Intersects(shell.GetBoundingBox()))
                {
                    
                    float defeatThreshold = 20.0f;
                    

                    if (game.player.Position.Y + game.player.FrameHeight < shell.Position.Y - defeatThreshold)
                    {
                        shell.Defeat();
                    }
                    else if (!game.player.IsInvulnerable)
                    {
                        game.player.TakeDamage(gameTime);
                    }
                }
                if (enemy.IsDefeated())
                {
                    game.CurrentGameState = Game1.GameState.Win;
                }
                if (enemy.IsBlinking && gameTime.TotalGameTime - enemy.lastBlinkTime > enemy.blinkInterval)
                {
                    enemy.IsVisible = !enemy.IsVisible;
                    enemy.lastBlinkTime = gameTime.TotalGameTime;

                    if (gameTime.TotalGameTime - enemy.lastDamageTime > enemy.blinkDuration)
                    {
                        enemy.IsBlinking = false;
                        enemy.IsVisible = true;
                    }
                }

                shell.Update(game.GraphicsManager.PreferredBackBufferWidth);




            }
            if (game.player.HealthPoints <= 0)
            {
                game.CurrentGameState = Game1.GameState.GameOver;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(level2Background, new Rectangle(0, 0, game.GraphicsManager.PreferredBackBufferWidth, game.GraphicsManager.PreferredBackBufferHeight), Color.White);


            if (game.CurrentGameState == Game1.GameState.Playing)
            {
                
                game.player.Draw(spriteBatch, game.spriteSheet, game.FrameWidth);

                shell.Draw(spriteBatch);
                //spriteBatch.Draw(pixel, game.player.GetBoundingBox(), Color.Red * 0.5f);
                //spriteBatch.Draw(pixel, shell.GetBoundingBox(), Color.Blue * 0.5f); 


                foreach (var bullet in game.bullets)
                {
                    bullet.Draw(spriteBatch);
                }
                DrawHealthBar(spriteBatch);
                DrawEnemyHealthBar(spriteBatch);
                int[] animationRowYCoordinates = { 20, 115, 209, 299, 397 };
                int frameHeight = 275 - 209; 
                Rectangle enemySourceRectangle = new Rectangle(0, animationRowYCoordinates[2], game.FrameWidth, frameHeight); 

                if (enemy.IsVisible)
                {
                    spriteBatch.Draw(enemySprite, enemy.Position, enemySourceRectangle, Color.Red * 0.5f, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.FlipHorizontally, 0.0f);
                }

            }
        }

        public override void Reset()
        {
            

            int frameWidth = game.FrameWidth;
            int frameHeight = game.FrameHeight;
            int playerX = 10;
            int playerY = game.GraphicsManager.PreferredBackBufferHeight - 20 - frameHeight;
            game.bullets.Clear(); 

            
            game.player = new Player(new Vector2(playerX, playerY), game.GraphicsManager, frameWidth, frameHeight);
        }
        private void DrawHealthBar(SpriteBatch spriteBatch)
        {
            int barWidth = 200;  
            int barHeight = 20;  
            int border = 2;  
            float percentage = (float)game.player.HealthPoints / 5; 

            Rectangle backgroundBar = new Rectangle(10, 10, barWidth + 2 * border, barHeight + 2 * border);  
            Rectangle healthBar = new Rectangle(10 + border, 10 + border, (int)(barWidth * percentage), barHeight);

            spriteBatch.Draw(pixel, backgroundBar, Color.Black);  
            spriteBatch.Draw(pixel, healthBar, Color.Green);  
        }
        private void DrawEnemyHealthBar(SpriteBatch spriteBatch)
        {
            int barWidth = 200;
            int barHeight = 20;
            int border = 2;
            float percentage = (float)enemy.HealthPoints / 20; 

            int xPos = game.GraphicsManager.PreferredBackBufferWidth - barWidth - 10 - 2 * border; 
            int yPos = 10;

            Rectangle backgroundBar = new Rectangle(xPos, yPos, barWidth + 2 * border, barHeight + 2 * border);
            Rectangle healthBar = new Rectangle(xPos + border, yPos + border, (int)(barWidth * percentage), barHeight);

            spriteBatch.Draw(pixel, backgroundBar, Color.Black);
            spriteBatch.Draw(pixel, healthBar, Color.Red);  
        }


    }
}
