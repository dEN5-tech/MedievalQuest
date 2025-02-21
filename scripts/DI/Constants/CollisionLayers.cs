using System;

namespace MedivalQuest.DI.Constants
{
    public static class CollisionLayers
    {
        public const int World = 1;
        public const int Player = 2;
        public const int PlayerHitbox = 3;
        public const int Enemy = 4;
        public const int EnemyHitbox = 5;
        public const int Projectile = 6;
        public const int Collectible = 7;
        public const int Item = 8;

        // Utility method to convert layer to bitmask
        public static uint ToBitmask(params int[] layers)
        {
            uint mask = 0;
            foreach (int layer in layers)
            {
                mask |= (uint)(1 << (layer - 1));
            }
            return mask;
        }
    }
} 