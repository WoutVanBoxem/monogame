using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System;

namespace Monogame
{
    public class Shell
    {
        private Texture2D texture;
        public Vector2 Position { get; set; }
        public bool MovingRight { get; set; }
        public float Speed { get; set; }
        private float scale = 0.2f;
        public bool IsDefeated { get; set; }

        public Shell(Texture2D texture, int screenHeight, int screenWidth)
        {
            this.texture = texture;
            this.Position = new Vector2(screenWidth - texture.Width * scale, screenHeight - 20 - texture.Height * scale);
            this.MovingRight = false;
            this.Speed = 5;
            this.IsDefeated = false;
        }

        public void Update(int screenWidth)
        {
            if (MovingRight)
            {
                Position = new Vector2(Position.X + Speed, Position.Y);
                if (Position.X + texture.Width * scale > screenWidth)
                {
                    MovingRight = false;
                }
            }
            else
            {
                Position = new Vector2(Position.X - Speed, Position.Y);
                if (Position.X < 0)
                {
                    MovingRight = true;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsDefeated)
            {
                if (MovingRight)
                {
                    spriteBatch.Draw(texture, Position, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(texture, Position, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.FlipHorizontally, 0);
                }
            }
        }

        public Rectangle GetBoundingBox()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, (int)(texture.Width * scale), (int)(texture.Height * scale));
        }

        public void Defeat()
        {
            IsDefeated = true;
            Speed = 0; 
        }
    }


}