using System;
using System.Collections.Generic;
using TTengine.Core;
using TTengine.Util;
using Microsoft.Xna.Framework;
using Pixie1;
using Pixie1.Behaviors;

namespace Pixie1.Actors
{
    public class BadPixel: Thing
    {
        // behaviors - the things that bad pixels do 
        public BlinkBehavior Blinking;
        public ChaseBehavior  Chasing;
        public AlwaysTurnRightBehavior Turning;
        public RandomWanderBehavior Wandering;

        Vector2 vecLeft = new Vector2(-1f, 0f);

        public static BadPixel Create(int tp)
        {
            string tpString;
            switch (tp)
            {
                case 0:
                    tpString = "pixie";
                    break;
                case 1:
                    tpString = "shape2x2";
                    break;
                default:
                    tpString = "shape2x2";
                    break;
            }
            return new BadPixel(tpString);
        }

        bool isCloaky = false;

        public BadPixel(string shape)
            : base(shape)
        {
            IsCollisionFree = false;
            DrawInfo.DrawColor = new Color(255, 10, 4);

            /*
            SubsumptionBehavior sub = new SubsumptionBehavior();
            Add(sub);
            */

            /*
            Wandering = new RandomWanderBehavior(2.7f, 11.3f);
            Wandering.MoveSpeed = 0.7f;
            sub.Add(Wandering);
            */

            LinearMotionBehavior b = new LinearMotionBehavior(new Vector2(-1f, 0f));
            Add(b);
        }

        /// <summary>
        /// set 'cloaky' status, a cloaky is a hardly visible bad pixel
        /// </summary>
        public bool IsCloaky
        {
            get
            {
                return isCloaky;
            }
            set
            {
                if (IsCloaky == value)
                    return;
                // if change - swap dutycycle
                Blinking.DutyCycle = 1f - Blinking.DutyCycle;
                isCloaky = value;                
            }
        }
        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            // check attach
            List<Thing> l = DetectCollisions(vecLeft);
            if (l.Count > 0)
            {
                Thing t = l[0];
                t.AddNextUpdate(this); // become a child - attach to it.
                this.AttachmentPosition = (this.Target - t.Target); // new relative position
            }

            // check self-delete
            Vector2 pp = Level.Current.pixie.Target;
            if (Target.X < pp.X - 80f)
                Delete = true;
            if (CollidesWithBackground(Vector2.Zero) && SimTime < 0.2f)
            {
                Delete = true;
            }

        }
    }
}
