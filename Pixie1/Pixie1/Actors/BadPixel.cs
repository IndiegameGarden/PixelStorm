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
        LinearMotionBehavior MyMotion;

        public static BadPixel Create(int tp)
        {
            string tpString;
            switch (tp)
            {
                case 0:
                    tpString = "t-block";
                    break;
                case 1:
                    tpString = "t-l";
                    break;
                case 2:
                    tpString = "t-s";
                    break;
                case 3:
                    tpString = "t-z";
                    break;
                case 4:
                    tpString = "t-stick";
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

            /*
            SubsumptionBehavior sub = new SubsumptionBehavior();
            Add(sub);
            */

            /*
            Wandering = new RandomWanderBehavior(2.7f, 11.3f);
            Wandering.MoveSpeed = 0.7f;
            sub.Add(Wandering);
            */

            MyMotion = new LinearMotionBehavior(new Vector2(-1f, 0f));
            Add(MyMotion);
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
            Pixie pixie = Level.Current.pixie;

            // hack - not visible at first to avoid position-flicker
            if (SimTime < 0.3f)
                Visible = false;
            else
                Visible = true;

            // if attached, disable motion
            MyMotion.Active = true; // !(Parent is Pixie);

            // check start of attachment to pixie
            if (!(Parent is Thing))
            {
                List<Thing> l = DetectCollisions(vecLeft);
                if (l.Count > 0)
                {
                    foreach (Thing t in l)
                    {
                        if (t is BadPixel)
                        {
                            BadPixel bp = t as BadPixel;
                            if (bp.Parent is Pixie)
                            {
                                pixie.AddNextUpdate(this); // become a child - attach to it.
                                AttachmentPosition = (new Vector2(PositionX, PositionY) - pixie.Target); // new relative position
                                break;
                            }
                        }
                        else if (t is Pixie)
                        {
                            pixie.AddNextUpdate(this); // become a child - attach to it.
                            AttachmentPosition = (new Vector2(PositionX, PositionY) - pixie.Target); // new relative position

                            break;
                        }
                    }
                }
            }

            // check self-delete
            Vector2 pp = pixie.Target;
            if (Target.X < pp.X - 80f)
                Delete = true;
            if (CollidesWithBackground(Vector2.Zero) && SimTime < 0.2f)
            {
                Delete = true;
            }

        }
    }
}
