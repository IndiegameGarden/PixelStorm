using System;
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
        public SubtitleText tObjCount;
        public float MaxPauseBetweenBaddies = 0.6f;
        public float SCROLLING_START_TIME = 5f;

        float timerNewBaddie = 0f;
        float nextBaddieInterval = 1f;
        Vector2 WINNING_POSITION = new Vector2(73f, 7f);
        Color LEVEL_FOREGROUND_COLOR = new Color(231, 231, 248);
        int numberOfZoomOuts = 0;

        public PixelStormLevel()
            : base()
        {           
            // Level settings
            SCREEN_MOTION_SPEED = 8.0f;
            DEFAULT_SCALE = 20f;// 15f;
            PIXIE_STARTING_POS = new Vector2(95f, 718f); // in pixels        
            //PIXIE_STARTING_POS = new Vector2(73f, 10f); // in pixels        
            BG_STARTING_POS = PIXIE_STARTING_POS;
            //PIXIE_STARTING_POS = new Vector2(188f, 0f); // close to win pos
            //BG_STARTING_POS = new Vector2(188f, 0f); 
        }

        protected override void InitLevel()
        {
            base.InitLevel();

            // select bitmap bg            
            Background = new LevelBackground("tetrash.png");
            Background.ForegroundColor = LEVEL_FOREGROUND_COLOR;
            Background.TargetSpeed = SCREEN_MOTION_SPEED;
            Add(Background);
            Background.Target = PIXIE_STARTING_POS;
            Background.Position = BG_STARTING_POS;
        }

        protected override void InitBadPixels()
        {
            base.InitBadPixels();

        }

        protected override void InitToys()
        {
            base.InitToys();
            Vector2 p;
            Toy t;
            t = new ZoomOutToy(); p = PIXIE_STARTING_POS + new Vector2(1f,-12f); t.PositionAndTarget = p; Add(t);
        }

        protected override void InitLevelSpecific()
        {
            Music = new GameMusic();
            Add(Music);

            SubtitleText t = new SubtitleText(); 
            t.AddText("Go, T! Go up...", 4.751f).ScaleVector = new Vector2(1f, 1f);
            t.AddText("...and build a tree!", 4.751f).ScaleVector = new Vector2(1f, 1f);
            t.StartTime = 0f;
            Subtitles.Show(0, t);

            tObjCount = new SubtitleText(""); tObjCount.ScaleVector = new Vector2(0.8f, 1f);            
            tObjCount.StartTime = 0f;
            Subtitles.Show(-2, tObjCount);
        }

        protected override bool ScreenBorderHit()
        {
            if (numberOfZoomOuts < 5)
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
                nextBaddieInterval = RandomMath.RandomBetween(0.3f, MaxPauseBetweenBaddies);
                BadPixel b = BadPixel.Create( (int) Math.Round(RandomMath.RandomBetween(-0.49f,23.49f)));
                float x = RandomMath.RandomBetween(-40f,40f);
                float y = RandomMath.RandomBetween(-50f,-40f);
                b.PositionAndTarget = new Vector2(x + pixie.Target.X, y + LevelBackground.ViewPos.Y);
                AddNextUpdate(b);
            }
            if (pixie.Score > 0)
            {
                tObjCount.Text = "Tree-score: " + pixie.Score;
                tObjCount.Motion.Position = new Vector2(Screen.Width-0.25f, 0.035f); 
            }
            // scroll background
            if (SimTime >= SCROLLING_START_TIME)
                Background.Target.Y = BG_STARTING_POS.Y - 3.0f * (SimTime-SCROLLING_START_TIME);

            // resolution scale changing
            if (Background.Target.Y < 720f && numberOfZoomOuts == 0) ScreenBorderHit();
            //if (Background.Target.Y < 710f && numberOfZoomOuts == 1) ScreenBorderHit();
            //if (Background.Target.Y < 700f && numberOfZoomOuts == 2) ScreenBorderHit();

            /*
            if (hasLost)
            {
                Music.Fade( -0.1f * p.Dt);
                if (Music.Volume == 0)
                    PixieGame.Instance.Exit();
            }
             */
        }
    }
}
