﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TTengine.Core;

namespace Pixie1.Behaviors
{
    public class RandomWanderBehavior: ThingControl
    {
        /**
         * current random direction of wandering - may be change
         */
        public Vector2 CurrentDirection = Vector2.Zero;

        /**
         * the random interval during which the direction changes at a random time. Can be
         * tweaked during operation.
         */
        public float MaxDirectionChangeTime, MinDirectionChangeTime;

        // waiting time before a next move is taken
        float wTime = 0f;

        public RandomWanderBehavior(float minDirectionChangeTime, float maxDirectionChangeTime)
        {
            this.MinDirectionChangeTime = minDirectionChangeTime;
            this.MaxDirectionChangeTime = maxDirectionChangeTime;
        }

        /// <summary>
        /// relative speed of chasing target
        /// </summary>
        public float MoveSpeed = 1.0f;

        float dirChangeTime = 0f;
        float timeSinceLastChange = 0f;

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            wTime += p.Dt;
            if (wTime >= 0.2f / MoveSpeed)
                wTime = 0f;

            if (wTime == 0f)
            {
                Vector2 dir = CurrentDirection;

                if (dir.Length() < 0.1f)
                    dir = Vector2.Zero;
                else
                {
                    // choose one direction randomly, if diagonals would be required
                    if (dir.X != 0f && dir.Y != 0f)
                    {
                        float r = RandomMath.RandomUnit();
                        if (r > 0.5f)
                            dir.X = 0f;
                        else
                            dir.Y = 0f;
                    }
                    dir.Normalize();
                }
                TargetMove = dir;

                // direction changing
                timeSinceLastChange += p.Dt;
                if (timeSinceLastChange >= dirChangeTime)
                {
                    timeSinceLastChange = 0f;
                    dirChangeTime = RandomMath.RandomBetween(MinDirectionChangeTime, MaxDirectionChangeTime);
                    CurrentDirection = RandomMath.RandomDirection();
                }
            }
            IsTargetMoveDefined = true;
        }

    }
}
