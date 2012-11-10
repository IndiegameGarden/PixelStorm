using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TTengine.Core;

namespace Pixie1.Behaviors
{
    public class LinearMotionBehavior: ThingControl
    {
        /**
         * current direction of motion - may be changed
         */
        public Vector2 CurrentDirection = Vector2.Zero;

        public LinearMotionBehavior(Vector2 initialDirection)
        {
            CurrentDirection = initialDirection;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            // keep this control always-active
            IsTargetMoveDefined = true;
            AllowNextMove();
        }

        protected override void OnNextMove()
        {
            base.OnNextMove();

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

        }
    }    
}
