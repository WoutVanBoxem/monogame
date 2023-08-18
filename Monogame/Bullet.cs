using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Media;


namespace Monogame {
    public class Bullet
    {
        public Vector2 Position { get; set; }
        public float Speed { get; private set; }
        public bool IsActive { get; set; } = true;
        private Texture2D _texture;

        public BulletDirection Direction { get; set; } 

        public enum BulletDirection
        {
            Left,
            Right
        }

        public Bullet(Vector2 position, BulletDirection direction, GraphicsDevice graphicsDevice)
        {
            Position = position;
            Direction = direction; 
            Speed = direction == BulletDirection.Right ? 10f : -10f;

            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new[] { Color.White });
        }

        public void Update()
        {
            Position = new Vector2(Position.X + Speed, Position.Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
                spriteBatch.Draw(_texture, new Rectangle((int)Position.X, (int)Position.Y, 5, 5), Color.Red);
        }

        public Rectangle GetBoundingBox()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, 5, 5);
        }
    }

}