using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Media;


public class FinishFlag
{
    public Texture2D Sprite { get; set; }
    public Vector2 Position { get; set; }

    public FinishFlag(Texture2D sprite, Vector2 position)
    {
        Sprite = sprite;
        Position = position;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        float scaleWidth = 50f / Sprite.Width;
        float scaleHeight = 100f / Sprite.Height;
        Vector2 scale = new Vector2(scaleWidth, scaleHeight);
        spriteBatch.Draw(Sprite, Position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }
    public Rectangle GetBoundingBox()
    {
        float scaleWidth = 50f / Sprite.Width;
        float scaleHeight = 100f / Sprite.Height;
        return new Rectangle((int)Position.X, (int)Position.Y, (int)(Sprite.Width * scaleWidth), (int)(Sprite.Height * scaleHeight));
    }


}

