﻿//
// Method IntersectPixels used under Microsoft Permissive License (Ms-PL). (http://create.msdn.com/downloads/?id=15)
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTengine.Core;
using TTengine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pixie1
{
    /**
     * base class for any visible thing in the Pixie universe
     */
    public class Thing: Spritelet
    {
        /// <summary>
        /// if true can pass anything
        /// </summary>
        public bool IsGodMode = false;

        /// <summary>
        /// Determines what intensity levels of background pixel color this Thing can pass.
        /// Intensity is the sum of R,G,B bytes of pixel. Any background pixel at threshold value or
        /// brighter is passable for this Thing.
        /// </summary>
        public int PassableIntensityThreshold = 298; // TODO get from level the default

        /// <summary>
        /// centre of screen viewing pos in pixels for ALL PixieSpritelets
        /// </summary>
        public static Vector2 ViewPos = Vector2.Zero;

        /// <summary>
        /// position in the level, in pixels, in sub-pixel resolution
        /// </summary>
        public Vector2 Position = Vector2.Zero;

        /// <summary>
        /// Position.X as a rounded integer
        /// </summary>
        public int PositionX
        {
            get { return (int)Math.Round(Position.X); }
            set { Position.X = (float)value; }
        }

        /// <summary>
        /// Position.Y as a rounded integer
        /// </summary>
        public int PositionY
        {
            get { return (int)Math.Round(Position.Y); }
            set { Position.Y = (float)value; }
        }


        /// <summary>
        /// Target.X as a rounded integer
        /// </summary>
        public int TargetX
        {
            get { return (int)Math.Round(Target.X); }
            set { Target.X = (float)value; }
        }

        /// <summary>
        /// Target.Y as a rounded integer
        /// </summary>
        public int TargetY
        {
            get { return (int)Math.Round(Target.Y); }
            set { Target.Y = (float)value; }
        }

        /// <summary>
        /// position in the level, in pixels, where the entity's Position should move towards in a smooth fashion
        /// </summary>
        public Vector2 Target = Vector2.Zero;

        /// <summary>
        /// a direction (if any) the entity is facing towards e.g. up (0,-1), down (0,1) or right (1,0).
        /// </summary>
        public Vector2 FacingDirection = Vector2.Zero;

        /// <summary>
        /// to set both Position and Target in one go
        /// </summary>
        public Vector2 PositionAndTarget
        {
            set
            {
                Position = value;
                Target = value;
            }
        }

        /// <summary>
        /// the bounding rectangle of the sprite of this Thing
        /// </summary>
        protected Rectangle BoundingRectangle = new Rectangle();

        /// <summary>
        /// a 'relative to normal' velocity-of-moving factor i.e. 1f == normal velocity
        /// </summary>
        public float Velocity = 1f;

        /// <summary>
        /// relative speed of the smooth motion for moving towards Target. Linear speed.
        /// </summary>
        public float TargetSpeed = 10f;

        /// <summary>
        /// the target move delta for current Update() round
        /// </summary>
        public Vector2 TargetMove = Vector2.Zero;

        // used for the collision detection per-pixel
        protected Color[] textureData;

        /// <summary>
        /// create a single-pixel Thing
        /// </summary>
        public Thing()
            : base("pixie")
        {
            DrawInfo.Center = Vector2.Zero;
        }

        /// <summary>
        /// create a Thing from arbitrary bitmap shape
        /// </summary>
        /// <param name="bitmapFile">content graphics file</param>
        public Thing(string bitmapFile)
            : base(bitmapFile)
        {
            BoundingRectangle.Width = Texture.Width;
            BoundingRectangle.Height = Texture.Height;
            textureData = new Color[BoundingRectangle.Width * BoundingRectangle.Height];
            Texture.GetData(textureData);
            DrawInfo.Center = Vector2.Zero;
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);

            // update BoundingRectangle values
            BoundingRectangle.X = PositionX;
            BoundingRectangle.Y = PositionY;

            // update position of the smooth motion of this Thing in the TTengine
            Motion.Position = Screen.Center + Motion.ScaleAbs * (  FromPixels( Position - ViewPos)); // TODO ViewPos smoothing using Draw cache?
            //Motion.Position = Position - ViewPos; // alternative to above

            // take steering inputs if any, and move pixie
            if (TargetMove.LengthSquared() > 0f)
            {
                Target += TargetMove;
                // if passable...
                if (IsGodMode || !CollidesWithBackground(TargetX,TargetY))
                {
                    // walk is ok
                }else{
                    Target -= TargetMove; // cancel the move
                }
                
            }            

            Vector2 vdif = Target - Position;
            if (vdif.LengthSquared() > 0f) // if target not reached yet
            {
                Vector2 vmove = vdif;
                vmove.Normalize();
                vmove *= TargetSpeed ;
                // convert speed vector to move vector (x = v * t)
                vmove *= p.Dt;
                // check if target reached already (i.e. move would overshoot target)
                if (vmove.LengthSquared() >= vdif.LengthSquared())
                {
                    Position = Target;
                }
                else
                {
                    // apply move towards target
                    Position += vmove;
                }
            }

            // reset TargetMove for next round - child ThingControls adapt this value.
            TargetMove = Vector2.Zero;

        }

        public bool Collides(Thing other)
        {
            return IntersectPixels(BoundingRectangle, textureData, other.BoundingRectangle, other.textureData );
        }

        public bool CollidesWithBackground(int posX, int posY)
        {
            if (posX < 0) 
                return true;
            if (posY < 0) 
                return true;
            Rectangle bgSampleRect = new Rectangle(posX, posY, BoundingRectangle.Width, BoundingRectangle.Height);
            Rectangle thingRect = new Rectangle(posX, posY, BoundingRectangle.Width, BoundingRectangle.Height);
            int N = bgSampleRect.Width * bgSampleRect.Height;
            Color[] bgTextureData = new Color[N];
            Level.Current.bg.Texture.GetData<Color>(0, bgSampleRect, bgTextureData, 0, N);
            return IntersectPixelsBg(thingRect, textureData, bgSampleRect, bgTextureData);
        }

        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels
        /// between two sprites.
        /// Method code from http://create.msdn.com/en-US/education/catalog/tutorial/collision_2d_perpixel
        /// used under Microsoft Permissive License (Ms-PL). 
        /// </summary>
        /// <param name="rectangleA">Bounding rectangle of the first sprite</param>
        /// <param name="dataA">Pixel data of the first sprite</param>
        /// <param name="rectangleB">Bouding rectangle of the second sprite</param>
        /// <param name="dataB">Pixel data of the second sprite</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        static bool IntersectPixels(Rectangle rectangleA, Color[] dataA,
                                    Rectangle rectangleB, Color[] dataB)
        {
            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
        }

        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels
        /// of a sprite with non-passable colored pixels in the background.
        /// Derived from above IntersectPixels() method.
        /// </summary>
        /// <param name="rectangleA">Bounding rectangle of the sprite</param>
        /// <param name="dataA">Pixel data of the sprite</param>
        /// <param name="rectangleB">Bouding rectangle of the background snapshot</param>
        /// <param name="dataB">Pixel data of the background snapshot</param>
        /// <returns>True if collision with background; false otherwise</returns>
        bool IntersectPixelsBg(Rectangle rectangleA, Color[] dataA,
                                    Rectangle rectangleB, Color[] dataB)
        {
           

            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    // If pixel A not completely transparent,
                    // and BG pixel non-passable (by comparing intensity to threshold),
                    if (colorA.A != 0 && (colorB.R + colorB.G + colorB.B) < PassableIntensityThreshold )
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
        }

    }
}