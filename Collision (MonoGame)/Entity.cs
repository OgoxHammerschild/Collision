// Copyright (c) 2016 Daniel Bortfeld
using Microsoft.Xna.Framework;

namespace Collision_MonoGame
{
    public class Entity_Dummy : GameObject_Dummy
    {
        public Vector2 Center
        {
            get
            {
                if (SpriteRect != Rectangle.Empty)
                    return Position + new Vector2(SpriteRect.Width / 2, SpriteRect.Height / 2);
                else return Position;
            }
            set
            {
                if (SpriteRect != Rectangle.Empty)
                    Position = value - new Vector2(SpriteRect.Width / 2, SpriteRect.Height / 2);
                else Position = value;
            }
        }

        public Rectangle SpriteRect
        {
            get { return Rectangle.Empty; }
        }
    }
}
