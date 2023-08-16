using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Media;

public class FallingBlock
{
    public Vector2 Position { get; set; }
    public Texture2D Sprite { get; set; }
    public bool IsActive { get; set; }
    public int Width { get; set; } = 200;
    public int Height { get; set; } = 30;

    public FallingBlock(GraphicsDevice graphicsDevice, Vector2 position)
    {
        Sprite = new Texture2D(graphicsDevice, 1, 1);
        Sprite.SetData(new[] { Color.SaddleBrown });  // dit maakt het donkerbruin
        Position = position;
        IsActive = false;
    }

    public void Update()
    {
        if (IsActive)
        {
            Vector2 newPosition = Position;
            newPosition.Y += 8f; // snelheid waarmee de balk valt
            Position = newPosition;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Rectangle destinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
        spriteBatch.Draw(Sprite, destinationRectangle, Color.White);
    }

    public Rectangle GetBoundingBox()
    {
        return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
    }
}
