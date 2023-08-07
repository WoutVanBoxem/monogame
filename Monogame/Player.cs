using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Player
{
    public Vector2 Position { get; set; }
    public int CurrentFrame { get; set; }
    public PlayerAnimation CurrentAnimation { get; set; }
    public float TimeSinceLastFrame { get; set; }
    public const float FrameTime = 0.2f;
    public bool FacingRight { get; set;}


    public Player(Vector2 position)
    {
        Position = position;
        CurrentFrame = 0;
        CurrentAnimation = PlayerAnimation.Standing;
        FacingRight = true;
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D spriteSheet, int frameWidth)
    {
        int[] animationRowYCoordinates = { 20, 115, 209, 299, 397 };  // Hardcoded y values
        int[] frameHeights = { 83 - 20, 181 - 115, 275 - 209, 363 - 299, 452 - 397 };  // Calculated frame heights
        int framesPerRow = GetFramesPerRow();
        int x = (CurrentFrame % framesPerRow) * frameWidth;
        int y = animationRowYCoordinates[GetAnimationRow()];  // Get y coordinate from array
        int frameHeight = frameHeights[GetAnimationRow()];  // Get frame height from array
        Rectangle sourceRectangle = new Rectangle(x, y, frameWidth, frameHeight);
        SpriteEffects effects = FacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        spriteBatch.Draw(spriteSheet, Position, sourceRectangle, Color.White, 0.0f, Vector2.Zero, 1.0f, effects, 0.0f);
    }




    public enum PlayerAnimation
    {
        Standing,
        Walking
    }
    public int GetFramesPerRow()
    {
        switch(CurrentAnimation)
        {
            case PlayerAnimation.Standing:
                return 8;
            case PlayerAnimation.Walking:
                return 6;
            default:
                throw new Exception("Niet geïmplementeerde animatie.");
        }
    }
    public int GetAnimationRow()
    {
        switch (CurrentAnimation)
        {
            case PlayerAnimation.Standing:
                return 0;
            case PlayerAnimation.Walking:
                return 2; 
            default:
                throw new Exception("Unsupported animation.");
        }
    }
    public void Update(GameTime gameTime)
    {
        TimeSinceLastFrame += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (TimeSinceLastFrame >= FrameTime)
        {
            CurrentFrame = (CurrentFrame + 1) % GetFramesPerRow();
            TimeSinceLastFrame = 0;
        }
    }

}
