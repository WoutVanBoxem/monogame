using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System;


namespace Monogame
{
    public class Enemy
    {
        public Vector2 Position { get; set; }
        public int HealthPoints { get; set; } = 20;
        public int FrameWidth { get; private set; }
        public int FrameHeight { get; private set; }
        public int CurrentFrame { get; set; }
        public int CurrentAnimationRow { get; set; } = 2;  

        public int[] animationRowYCoordinates = { 20, 115, 209, 299, 397 };
        public int[] frameHeights = { 83 - 20, 181 - 115, 275 - 209, 363 - 299, 452 - 397 };
        public bool IsBlinking { get; set; } = false;
        public bool IsVisible { get; set; } = true;
        public TimeSpan blinkDuration = TimeSpan.FromSeconds(1);
        public TimeSpan blinkInterval = TimeSpan.FromMilliseconds(100);
        public TimeSpan lastBlinkTime;
        public TimeSpan lastDamageTime;


        public Enemy(Vector2 position, int frameWidth, int frameHeight)
        {
            Position = position;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
        }

        public Rectangle GetBoundingBox()
        {
            int x = (CurrentFrame % FrameWidth) * FrameWidth;
            int y = animationRowYCoordinates[CurrentAnimationRow];
            int actualFrameHeight = frameHeights[CurrentAnimationRow];
            Rectangle sourceRectangle = new Rectangle(x, y, FrameWidth, actualFrameHeight);

            return new Rectangle((int)Position.X, (int)Position.Y, sourceRectangle.Width, sourceRectangle.Height);
        }

        public void TakeDamage(GameTime gameTime)
        {
            HealthPoints -= 1;
            IsBlinking = true;
            lastDamageTime = gameTime.TotalGameTime;
            lastBlinkTime = gameTime.TotalGameTime;
        }


        public bool IsDefeated()
        {
            return HealthPoints <= 0;
        }
    }
}