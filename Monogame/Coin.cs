using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Media;



namespace Monogame
{
    public class Coin
    {
        public Texture2D SpriteSheet { get; set; }
        public Vector2 Position { get; set; }
        private int currentFrame;
        private int frameWidth = 120; // Aanname: elk frame is 16 pixels breed
        private int frameHeight = 116; // Aanname: elk frame is 16 pixels hoog
        private double timeCounter;
        private double fps = 8.0; // 10 frames per seconde voor de munt animatie
        private int totalFrames = 6; // Aanname: 6 frames in de spritesheet
        public bool IsCollected { get; set; }
        public float Scale { get; set; } = 0.4f;

        public Coin(Texture2D spriteSheet, Vector2 position)
        {
            SpriteSheet = spriteSheet;
            Position = position;
        }

        public void Update(GameTime gameTime)
        {
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;
            if (timeCounter >= 1.0 / fps)
            {
                currentFrame++;
                timeCounter = 0;

                if (currentFrame >= totalFrames)
                    currentFrame = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
            Vector2 origin = new Vector2(frameWidth / 2, frameHeight / 2);
            Vector2 drawPosition = Position + origin;  // We voegen de oorsprong toe om het middenpunt te behouden na het schalen
            spriteBatch.Draw(SpriteSheet, drawPosition, sourceRectangle, Color.White, 0f, origin, Scale, SpriteEffects.None, 0f);
        }




        public Rectangle GetBoundingBox()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, frameWidth, frameHeight);
        }
    }
}