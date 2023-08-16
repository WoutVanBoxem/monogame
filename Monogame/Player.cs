using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

public class Player
{
    public Vector2 Position { get; set; }
    public int CurrentFrame { get; set; }
    public PlayerAnimation CurrentAnimation { get; set; }
    public float TimeSinceLastFrame { get; set; }
    public const float FrameTime = 0.2f;
    public bool FacingRight { get; set; }
    public float JumpForce { get; set; }
    public bool IsJumping { get; set; }
    public float JumpTime { get; set; }
    public const float MaxJumpTime = 1f;
    public float JumpSpeed { get; set; }
    public GraphicsDeviceManager Graphics { get; set; }
    public int FrameWidth { get; set; }
    public int FrameHeight { get; set; }





    public Player(Vector2 position, GraphicsDeviceManager graphics, int framewidth, int frameheight)

    {
        Position = position;
        CurrentFrame = 0;
        CurrentAnimation = PlayerAnimation.Standing;
        FacingRight = true;
        JumpForce = 10.0f;
        IsJumping = false;
        Graphics = graphics;
        FrameWidth= framewidth;
        FrameHeight= frameheight;
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D spriteSheet, int frameWidth)
    {
        int[] animationRowYCoordinates = { 20, 115, 209, 299, 397 }; 
        int[] frameHeights = { 83 - 20, 181 - 115, 275 - 209, 363 - 299, 452 - 397 }; 
        int framesPerRow = GetFramesPerRow();
        int x = (CurrentFrame % framesPerRow) * frameWidth;
        int y = animationRowYCoordinates[GetAnimationRow()]; 
        int frameHeight = frameHeights[GetAnimationRow()]; 
        Rectangle sourceRectangle = new Rectangle(x, y, frameWidth, frameHeight);
        SpriteEffects effects = FacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        spriteBatch.Draw(spriteSheet, Position, sourceRectangle, Color.White, 0.0f, Vector2.Zero, 1.0f, effects, 0.0f);
    }

    public enum PlayerAnimation
    {
        Standing,
        Walking
    }

    public Rectangle GetBoundingBox()
    {
        return new Rectangle((int)Position.X, (int)Position.Y, FrameWidth, FrameHeight);

    }


    public int GetFramesPerRow()
    {
        switch (CurrentAnimation)
        {
            case PlayerAnimation.Standing:
                return 8;
            case PlayerAnimation.Walking:
                return 6;
            default:
                throw new Exception("Unsupported animation.");
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

    public void Update(GameTime gameTime, List<Solid> solids, KeyboardState keyboardState)
    {
        TimeSinceLastFrame += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (Position.X < 0) Position = new Vector2(0, Position.Y);
        if (Position.X > Graphics.PreferredBackBufferWidth - FrameWidth) Position = new Vector2(Graphics.PreferredBackBufferWidth - FrameWidth, Position.Y);

        if (TimeSinceLastFrame >= FrameTime)
        {
            CurrentFrame = (CurrentFrame + 1) % GetFramesPerRow();
            TimeSinceLastFrame = 0;
        }


        Position = new Vector2(Position.X, Position.Y + 3.0f);

        
        int[] frameHeights = { 83 - 20, 181 - 115, 275 - 209, 363 - 299, 452 - 397 };  
        int frameHeight = frameHeights[GetAnimationRow()]; 

        foreach (Solid solid in solids)
        {
            if (Position.Y + frameHeight > solid.Bounds.Top && Position.Y < solid.Bounds.Bottom)
            {
                Position = new Vector2(Position.X, solid.Bounds.Top - frameHeight);
                IsJumping = false;
                JumpTime = 0;
                JumpSpeed = JumpForce;
            }
        }

        
        if (keyboardState.IsKeyDown(Keys.Up) && !IsJumping)
        {
            IsJumping = true;
        }

        if (IsJumping)
        {
            JumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            CurrentAnimation = PlayerAnimation.Standing;

            if (JumpTime < MaxJumpTime)
            {
                
                Position = new Vector2(Position.X, Position.Y - JumpSpeed);

                
                JumpSpeed *= 0.97f;
            }
        }

    }



}
