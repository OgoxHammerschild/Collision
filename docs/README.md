# Collision Manager for the MonoGame-Framework (C#)

I made a CollisionManager and a BoxCollider2D for my sidescroller in MonoGame. The CollisionManager checks all colliders against each other. The BoxCollider2D keeps the logic for checking whether he collides with a given collider.

```c#
// Copyright (c) 2016 Daniel Bortfeld
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace MonoGamePortal3Practise
{
    public static class CollisionManager
    {
        private static List<BoxCollider> colliders = new List<BoxCollider>();
        private static List<BoxCollider> removedColliders = new List<BoxCollider>();

        /// <summary>
        /// Register a collider for collision checks in the Manager.
        /// </summary>
        /// <param name="collider">The collider to add.</param>
        public static void AddCollider(BoxCollider collider)
        {
            colliders.Add(collider);
        }

        /// <summary>
        /// Remove all registered Colliders from the Manager's collision check  list.
        /// </summary>
        public static void Clear()
        {
            removedColliders.AddRange(colliders);
        }

        /// <summary>
        /// Remove a specific collider from being checked for collisions by the Manager.
        /// </summary>
        /// <param name="collider">The collider to remove.</param>
        public static void RemoveCollider(BoxCollider collider)
        {
            removedColliders.Add(collider);
        }

        /// <summary>
        /// Update function of the Manager. Call this every frame.
        /// </summary>
        /// <param name="gameTime">delta time</param>
        public static void UpdateColliders(GameTime gameTime)
        {
            colliders.ForEach(c => c.UpdatePosition(gameTime));
            CheckCollisions();

            removedColliders.ForEach(c => colliders.Remove(c));
            removedColliders.Clear();
        }

        /// <summary>
        /// Checks all registered colliders against each other
        /// </summary>
        private static void CheckCollisions()
        {
            foreach (var colliderA in colliders)
                foreach (var colliderB in colliders)
                    if (!colliderA.Equals(colliderB))
                        if (colliderA.IsActive && colliderB.IsActive)
                            colliderA.CheckCollision(colliderB);
        }
    }
}
```

## Box Collider 2D for the MonoGame-Framework (C#)

The BoxCollider2D is a rectangle that can check whether it intersects/collides with another rectangle on the screen. A respective CollisionEvent will be fired when a collison is entered, when it stays and when it's exited.    
There are also functions to determine the direction of the collision.   

```c#
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

        // ...

        // rectangle:
        public int X;
        public int Y;
        public int Width;
        public int Height;

        /// <summary>
        /// Current collisions of this collider
        /// </summary>
        private List<BoxCollider> collisions = new List<BoxCollider>();
        
	// ...

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

        // ...

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

        // ...

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

        // ... for each side ...

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
```
