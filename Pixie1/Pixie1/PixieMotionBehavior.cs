﻿// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TTengine.Core;

namespace TTengine.Util
{
    public class PixieMotionBehavior : Motion
    {
        public const float MIN_VELOCITY = 1.0f;

        /// <summary>
        /// sets a target position for cursor to move towards in pixels
        /// </summary>
        public Vector2 Target = Vector2.Zero;

        /// <summary>
        /// speed for moving towards Target
        /// </summary>
        public float TargetSpeed = 0f;

        /// <summary>
        /// set target for Scale
        /// </summary>
        public float ScaleTarget = 1.0f;

        /// <summary>
        /// speed for scaling towards ScaleTarget
        /// </summary>
        public float ScaleSpeed = 0f;

        // zoom, scale etc. related vars for panel
        public float ZoomTarget = 1.0f;

        public float ZoomSpeed = 0f;

        protected override void OnNewParent()
        {
            base.OnNewParent();
            Motion = Parent.Motion;
            DrawInfo = Parent.DrawInfo;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            // motion towards target
            Vector2 vDif = (Target - Position);
            Velocity = vDif * TargetSpeed; 
            if (Velocity.Length() < MIN_VELOCITY)
                Velocity *= ( MIN_VELOCITY / Velocity.Length() );
            if (vDif.Length() < 0.1f) // TODO const
            {
                Velocity = Vector2.Zero;
                Position = Target;
            }
            // handle scaling over time
            //ScaleToTarget(ScaleTarget, ScaleSpeed, 0.01f);

            // handle dynamic zooming
            //ZoomToTarget();
        }

        private void ZoomToTarget()
        {
            // handle dynamic zooming
            if (ZoomSpeed > 0f)
            {
                if (Motion.Zoom < ZoomTarget)
                {
                    Motion.Zoom *= (1.0f + ZoomSpeed);
                    if (Motion.Zoom > ZoomTarget)
                        Motion.Zoom = ZoomTarget;
                }
                else if (Motion.Zoom > ZoomTarget)
                {
                    Motion.Zoom /= (1.0f + ZoomSpeed);
                    if (Motion.Zoom < ZoomTarget)
                        Motion.Zoom = ZoomTarget;
                }
            }
        }

        // scaling logic during OnUpdate()
        private void ScaleToTarget(float targetScale, float spd, float spdMin)
        {
            if (spd > 0)
            {
                if (Motion.Scale < targetScale)
                {
                    Motion.Scale += spdMin + spd * (targetScale - Motion.Scale); //*= 1.01f;
                    if (Motion.Scale > targetScale)
                    {
                        Motion.Scale = targetScale;
                    }
                }
                else if (Motion.Scale > targetScale)
                {
                    Motion.Scale += -spdMin + spd * (targetScale - Motion.Scale); //*= 1.01f;
                    if (Motion.Scale < targetScale)
                    {
                        Motion.Scale = targetScale;
                    }
                }
                if (DrawInfo != null)
                    DrawInfo.LayerDepth = 0.8f - Motion.Scale / 1000.0f;
            }
        }

    }
}
