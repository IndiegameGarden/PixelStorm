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
        public RandomWanderBehavior Wandering;

        Vector2 vecDown = new Vector2(0, 1f);
        LinearMotionBehavior MyMotion;

        public static BadPixel Create(int tp)
        {
            string tpString;
            switch (tp)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    tpString = "t-block";
                    break;
                case 4:
                    tpString = "t-l";
                    break;
                case 5:
                    tpString = "t-l-1";
                    break;
                case 6:
                    tpString = "t-l-2";
                    break;
                case 7:
                    tpString = "t-l-3";
                    break;
                case 8:
                case 9:
                    tpString = "t-s";
                    break;
                case 10:
                case 11:
                    tpString = "t-s-1";
                    break;
                case 12:
                case 13:
                    tpString = "t-z";
                    break;
                case 14:
                case 15:
                    tpString = "t-z-1";
                    break;
                case 16:
                case 17:
                    tpString = "t-stick";
                    break;
                case 18:
                case 19:
                    tpString = "t-stick-1";
                    break;
                case 20:
                    tpString = "t-t";
                    break;
                case 21:
                    tpString = "t-t-1";
                    break;
                case 22:
                    tpString = "t-t-2";
                    break;
                case 23:
                    tpString = "t-t-3";
                    break;
                default:
                    tpString = "pixie";
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

            MyMotion = new LinearMotionBehavior(new Vector2(0f, 1f));
            MyMotion.MoveSpeed = RandomMath.RandomBetween(0.2f, 2f);
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
            // if attached, disable motion
            MyMotion.Active = true; // !(Parent is Pixie);

            // hack - not visible at first to avoid position-flicker
            if (SimTime < 0.3f)
                Visible = false;
            else
                Visible = true;

            // when floating free - check start of attachment to pixie
            if (!(Parent is Thing))
            {
                List<Thing> l = DetectCollisions(vecDown);
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
                                AttachmentPosition = new Vector2(PositionX - pixie.PositionX, PositionY - pixie.PositionY); // new relative position
                                TTutil.Round(ref AttachmentPosition);
                                pixie.Score += 1;
                                break;
                            }
                        }
                        else if (t is Pixie)
                        {
                            pixie.AddNextUpdate(this); // become a child - attach to it.
                            //AttachmentPosition = (new Vector2(PositionX, PositionY) - pixie.Target); // new relative position
                            //AttachmentPosition = new Vector2(1f,PositionY-pixie.TargetY); // new relative position
                            AttachmentPosition = new Vector2(PositionX - pixie.PositionX, PositionY - pixie.PositionY); // new relative position
                            TTutil.Round(ref AttachmentPosition);
                            //Level.Current.Subtitles.Show(2, "Ouch! That sticks!",3f);
                            pixie.Score += 1;
                            break;
                        }
                    }
                }
            }
        
            // check self-delete
            Vector2 pp = pixie.Target;
            if (PositionY > pixie.PositionY + 130f)     // if way down player somewhere, delete
                Delete = true;
            if (SimTime < 0.2f && CollidesWithBackground(Vector2.Zero))  // new entities that get stuck on background - delete
            {
                Delete = true;
            }

        }

        protected override void OnUpdatePost(ref UpdateParams p) 
        {

        }
    }
}
