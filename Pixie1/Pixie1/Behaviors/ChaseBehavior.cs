﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTengine.Core;
using Microsoft.Xna.Framework;

namespace Pixie1.Behaviors
{
    /// <summary>
    /// lets a Thing chase another Thing when it's visible.
    /// </summary>
    public class ChaseBehavior: ThingControl
    {
        /// <summary>
        /// followed target of this chase behavior
        /// </summary>
        public Thing ChaseTarget;

        /// <summary>
        /// chase range in pixels
        /// </summary>
        public float ChaseRange = 10.0f;

        public ChaseBehavior(Thing chaseTarget)
        {
            this.ChaseTarget = chaseTarget;
        }

        protected override void  OnNextMove()
        {
 	        base.OnNextMove();

            // compute direction towards chase-target
            Vector2 dif = ChaseTarget.Position - ParentThing.Target;
            // choose one direction randomly, if diagonals would be required
            if (dif.X != 0f && dif.Y != 0f)
            {
                float r = RandomMath.RandomUnit();
                if (r > 0.5f)
                    dif.X = 0f;
                else
                    dif.Y = 0f;
            }
            dif.Normalize();
            TargetMove = dif;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            if (ChaseTarget.Visible)
            {
                Vector2 dif = ChaseTarget.Position - ParentThing.Target;
                float dist = dif.Length();
                if (dist > 0f && dist <= ChaseRange )
                {
                    // indicate we're chasing
                    IsTargetMoveDefined = true;
                    AllowNextMove();
                }
            }
        }
    }
}
