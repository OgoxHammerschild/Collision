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
