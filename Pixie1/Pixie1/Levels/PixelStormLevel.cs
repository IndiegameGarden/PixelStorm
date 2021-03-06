﻿using System;
using System.Collections.Generic;
using TTengine.Core;
using TTengine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pixie1;
using Pixie1.Actors;
using Pixie1.Toys;

namespace Pixie1.Levels
{
    /// <summary>
    /// level where blocks come falling at you
    /// </summary>
    public class PixelStormLevel : Level
    {
        public static float SCROLL_SPEED_PIXELS_PER_SEC = 3f;
        public static float MinPauseBetweenBaddies = 0.9f;
        public static float MaxPauseBetweenBaddies = 3.6f;
        public static float SCROLLING_START_TIME = 7.5f;
        public static Vector2 WINNING_POSITION = new Vector2(73f, 7f);
        public static Color LEVEL_FOREGROUND_COLOR = new Color(231, 231, 248);
        public static Color LEVEL_BACKGROUND_COLOR = Color.DarkRed;
        public SubtitleText tObjCount;
        public Boolean hasLost = false;

        float timerNewBaddie = 0f;
        float nextBaddieInterval = 1f;
        int numberOfZoomOuts = 0;

        public PixelStormLevel()
            : base()
        {           
            // Level settings
            SCREEN_MOTION_SPEED = 8.0f;
            DEFAULT_SCALE = 15f;// 15f;
            PIXIE_STARTING_POS = new Vector2(96f, 690f); // in pixels        
            //PIXIE_STARTING_POS = new Vector2(73f, 10f); // in pixels        
            BG_STARTING_POS = new Vector2(96f,720f);
            //PIXIE_STARTING_POS = new Vector2(188f, 0f); // close to win pos
            //BG_STARTING_POS = new Vector2(188f, 0f); 
        }

        protected override void InitLevel()
        {
            base.InitLevel();

            // select bitmap bg            
            Background = new LevelBackground("tetrash.png");
            Background.ForegroundColor = LEVEL_FOREGROUND_COLOR;
            Background.BackgroundColor = LEVEL_BACKGROUND_COLOR;
            Background.TargetSpeed = SCREEN_MOTION_SPEED;
            Add(Background);
            Background.Target = BG_STARTING_POS;
            Background.Position = BG_STARTING_POS;
        }

        protected override void InitBadPixels()
        {
            base.InitBadPixels();

        }

        protected override void InitToys()
        {
            base.InitToys();
            //Vector2 p;
            //Toy t;
            //t = new ZoomOutToy(); p = PIXIE_STARTING_POS + new Vector2(1f,-12f); t.PositionAndTarget = p; Add(t);
        }

        protected override void InitLevelSpecific()
        {
            Music = new GameMusic();
            Add(Music);

            SubtitleText t = new SubtitleText(); 
            t.AddText("Always down. I want to go free!", 4.751f).ScaleVector = new Vector2(1f, 1f);
            t.AddText("See the world, and be ... a tree!", 4.751f).ScaleVector = new Vector2(1f, 1f);
            t.StartTime = 0f;
            Subtitles.Show(0, t);

            tObjCount = new SubtitleText(""); tObjCount.ScaleVector = new Vector2(0.8f, 1f);            
            tObjCount.StartTime = 0f;
            tObjCount.Motion.Position = new Vector2(Screen.Width - 0.25f, 0.035f);
            tObjCount.Visible = false;
            Subtitles.Show(-2, tObjCount);
        }

        protected override bool ScreenBorderHit()
        {
            if (numberOfZoomOuts < 3)
            {
                numberOfZoomOuts++;
                //Motion.Scale /= 1.5f;
                Motion.ScaleTarget /= 1.5f;
                Motion.ScaleSpeed = 0.2f;
                return false;
            }
            return true;
        }

        // scroll the level background to match pixie
        protected override void ScrollBackground(ref UpdateParams p)
        {
            // scrolling background at borders
            Vector2 pixiePos = pixie.Motion.PositionAbs;

            if (pixiePos.X < BOUND_X || pixiePos.X > (Screen.Width - BOUND_X))
            {            
                Background.Target.X = pixie.Position.X;
            }             
        }


        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);
            // adapt scroll speed to how fast pixie goes
            Background.TargetSpeed = SCREEN_MOTION_SPEED * pixie.Velocity;

            // create new pixels falling
            timerNewBaddie += p.Dt;
            if (timerNewBaddie >= nextBaddieInterval && SimTime >= 4.0f )
            {
                timerNewBaddie = 0f;
                nextBaddieInterval = RandomMath.RandomBetween(MinPauseBetweenBaddies, MaxPauseBetweenBaddies);
                BadPixel b = BadPixel.Create( (int) Math.Round(RandomMath.RandomBetween(-0.49f,23.49f)));
                float x = RandomMath.RandomBetween(-40f,40f);
                float y = RandomMath.RandomBetween(-50f,-40f);
                b.PositionAndTarget = new Vector2(x + pixie.Target.X, y + LevelBackground.ViewPos.Y);
                AddNextUpdate(b);
            }
            if (pixie.Score > 0)
            {
                tObjCount.Text = "Tree-score: " + pixie.Score;
                tObjCount.Visible = true;
            }
            // scroll background
            if (SimTime >= SCROLLING_START_TIME)
                Background.Target.Y = BG_STARTING_POS.Y - SCROLL_SPEED_PIXELS_PER_SEC * (SimTime-SCROLLING_START_TIME);
                //Level.Current.Background.Motion.ZoomCenterTarget = Level.Current.pixie.Motion;

            // resolution scale changing
            //if (SimTime>= SCROLLING_START_TIME && Background.Target.Y < 700f && numberOfZoomOuts == 0) 
            //    ScreenBorderHit();
            //if (Background.Target.Y < 710f && numberOfZoomOuts == 1) ScreenBorderHit();
            //if (Background.Target.Y < 700f && numberOfZoomOuts == 2) ScreenBorderHit();
            
            // test position on screen - if pixie beneath lower border much, lose
            if (SimTime>= SCROLLING_START_TIME && !hasLost && ( pixie.Motion.PositionAbsZoomedPixels.Y > Screen.HeightPixels + 100f ))
                hasLost = true;

            if (hasLost)
            {
                Music.Fade( -0.1f * p.Dt);
                this.Background.DrawInfo.DrawColor = Color.White * Music.Volume;
                if (Music.Volume == 0)
                    PixieGame.Instance.Exit();
            }
        }
    }
}
