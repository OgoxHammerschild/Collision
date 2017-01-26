// Copyright (c) 2016 Daniel Bortfeld
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace MonoGamePortal3Practise
{
    public class BoxCollider
    {
        public delegate void CollisionEvent(BoxCollider other);
        public event CollisionEvent OnCollisionEnter, OnCollisionStay, OnCollisionExit;

        public bool IsTrigger;
        public bool AutoPositionUpdateIsEnabled = true;

        // rectangle:
        public int X;
        public int Y;
        public int Width;
        public int Height;

        /// <summary>
        /// Current collisions of this collider
        /// </summary>
        private List<BoxCollider> collisions = new List<BoxCollider>();

        private bool isActive = true;

        public bool IsActive
        {
            get { return isActive; }
            set
            {
                if (IsActive == value)
                    return;
                isActive = value;
                if (!IsActive)
                {
                    collisions.ForEach(c => c.CheckCollision(this));
                    collisions.Clear();
                }
            }
        }

        // rect sides
        public int Top { get { return Y; } }
        public int Bottom { get { return Y + Height; } }
        public int Left { get { return X; } }
        public int Right { get { return X + Width; } }

        /// <summary>
        /// The center point of the collider rect
        /// </summary>
        public Vector2 Center
        {
            get { return new Vector2(X, Y) + new Vector2(Width / 2, Height / 2); }
            set { X = (int)value.X - Width / 2; Y = (int)value.Y - Height / 2; }
        }

        /// <summary>
        /// The GameObject the BoxCollider is attached to
        /// </summary>
        public GameObject GameObject { get; private set; }

        // Constructor
        public BoxCollider(GameObject gameObject, int width, int height, bool isTrigger)
        {
            GameObject = gameObject;
            X = (int)GameObject.Position.X;
            Y = (int)GameObject.Position.Y;
            Width = width;
            Height = height;
            IsTrigger = isTrigger;

            CollisionManager.AddCollider(this);
        }

        /// <summary>
        /// Update function of BoxCollider. Adjusts the position of the collider relative to the parent GameObject
        /// (if enabled)
        /// </summary>
        /// <param name="gameTime">delta time</param>
        public void UpdatePosition(GameTime gameTime)
        {
            if (!AutoPositionUpdateIsEnabled)
                return;

            /// center the collider to the middle of the <"SpriteRect">
            if (GameObject is Entity && ((Entity)GameObject).SpriteRect != Rectangle.Empty)
            {
                Center = ((Entity)GameObject).Center;
            }
            else
            {
                X = (int)GameObject.Position.X;
                Y = (int)GameObject.Position.Y;
            }
        }

        /// <summary>
        /// Checks whether this collider collides with the other collider. Calls respecive collision events.
        /// </summary>
        /// <param name="other"></param>
        public void CheckCollision(BoxCollider other)
        {
            if (!isActive || !other.isActive ||
                Right < other.Left || other.Right < Left || Bottom < other.Top || other.Bottom < Top)
            {
                // no collision
                if (collisions.Contains(other))
                {
                    // no more colliding
                    if (OnCollisionExit != null)
                        OnCollisionExit(other);
                    collisions.Remove(other);
                }
                return;
            }

            if (collisions.Contains(other))
            {
                // still colliding
                if (OnCollisionStay != null)
                    OnCollisionStay(other);
                return;
            }

            // new collison
            collisions.Add(other);
            if (OnCollisionEnter != null)
                OnCollisionEnter(other);
        }

        /// <summary>
        /// Whether this collider intersects with the other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Intersects(BoxCollider other) // or "CollidesWith" so to say
        {
            if (Equals(other))
                throw new Exception("This and the other Collider are identical. A collider cannot intersect with itself.");

            if ((other.Right >= X && other.X <= Right) && (other.Bottom >= X && other.Y <= Bottom))
                return true;
            return false;
        }

        /// <summary>
        /// Checks if the other BoxCollider is completely inside this BoxCollider
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Contains(BoxCollider other)
        {
            if (Equals(other))
                throw new Exception("This and the other Collider are identical. A collider cannot contain itself.");

            // other top left is contained
            if (other.X > X && other.Right < Right && other.Y > Y && other.Y < Bottom)
                // other top right is contained
                if (other.Right > X && other.Right < Right && other.Y > Y && other.Y < Bottom)
                    // other bot left is contained
                    if (other.X > X && other.X < Right && other.Bottom > Y && other.Bottom < Bottom)
                        // other bot right is contained
                        if (other.Right > X && other.Right < Right && other.Bottom > Y && other.Bottom < Bottom)
                            return true;
            return false;
        }

        #region Minkowsky sum (Magic)

        /// <summary>
        /// Whether this collider is colliding with the top of the other
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CollidesWithTopOf(BoxCollider other)
        {
            float wy = (Width + other.Width) * (Center.Y - other.Center.Y);
            float hx = (Height + other.Height) * (Center.X - other.Center.X);

            if (wy <= -hx && wy <= hx)
                return true;
            return false;
        }

        /// <summary>
        /// Whether this collider is colliding with the bottom of the other
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CollidesWithBottomOf(BoxCollider other)
        {
            float wy = (Width + other.Width) * (Center.Y - other.Center.Y);
            float hx = (Height + other.Height) * (Center.X - other.Center.X);

            if (wy > -hx && wy > hx)
                return true;
            return false;
        }

        /// <summary>
        /// Whether this collider is colliding with the left side of the other
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CollidesWithLeftOf(BoxCollider other)
        {
            float wy = (Width + other.Width) * (Center.Y - other.Center.Y);
            float hx = (Height + other.Height) * (Center.X - other.Center.X);

            if (wy <= -hx && wy > hx)
                return true;
            return false;
        }

        /// <summary>
        /// Whether this collider is colliding with the right side of the other
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CollidesWithRightOf(BoxCollider other)
        {
            float wy = (Width + other.Width) * (Center.Y - other.Center.Y);
            float hx = (Height + other.Height) * (Center.X - other.Center.X);

            if (wy > -hx && wy <= hx)
                return true;
            return false;
        }

        #endregion Minkowsky sum (Magic)

        /// <summary>
        /// Remove this collider from the collider manager.
        /// </summary>
        public void Remove()
        {
            CollisionManager.RemoveCollider(this);
        }
    }
}