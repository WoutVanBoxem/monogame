using System;
using Microsoft.Xna.Framework;

public class Solid
{
	public Rectangle Bounds { get; set; }
	public Solid(Rectangle bounds)
	{
		Bounds= bounds;
	}
}
