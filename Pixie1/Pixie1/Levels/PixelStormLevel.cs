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
        public float MaxPauseBetweenBaddies = 2.5f;

        float timerNewBaddie = 0f;
        float nextBaddieInterval = 1f;
        Vector2 WINNING_POSITION = new Vector2(73f, 7f);
        Color LEVEL_FOREGROUND_COLOR = new Color(231, 231, 248);
        float timeInWinningPos = 0f;
        bool hasWon = false;
        int numberOfZoomOuts = 0;

        public PixelStormLevel()
            : base()
        {           
            // Level settings
            SCREEN_MOTION_SPEED = 8.0f;
            DEFAULT_SCALE = 15f;// 15f;
            PIXIE_STARTING_POS = new Vector2(350f, 681f); // in pixels        
            //PIXIE_STARTING_POS = new Vector2(73f, 10f); // in pixels        
            BG_STARTING_POS = PIXIE_STARTING_POS;
            //PIXIE_STARTING_POS = new Vector2(188f, 0f); // close to win pos
            //BG_STARTING_POS = new Vector2(188f, 0f); 
        }

        protected override void InitLevel()
        {
            base.InitLevel();

            // select bitmap bg            
            Background = new LevelBackground("bg2045.png");
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

            //t = new ZoomOutToy(); p = new Vector2(70f,48f); t.PositionAndTarget = p; Add(t);
        }

        protected override void InitLevelSpecific()
        {
            Music = new GameMusic();
            Add(Music);

            SubtitleText t = new SubtitleText();
            t.AddText("QUEST FOR THE\nPIXEL PRINCESS", 6f);
            t.AddText("You, The Golden Knight, must\nrescue the princess...", 5f);
            t.AddText("...from the clutches of\nthe evil Red Guard.", 5f);
            //t.AddText("...from the clutches of\nthe evil Red Guard.", 5f);
            Subtitles.Show(0, t);

            t = new SubtitleText("My ears! Hark the drums\nof the Red Guard!");
            t.StartTime = 64.0f;
            t.Duration = 10.0f;
            Subtitles.Show(0, t);

            t = new SubtitleText();
            t.AddText("Music 'The Never Ending Quest For The\nPrincess II' by Producer Snafu!\nCC-BY-NC  producersnafu.bandcamp.com", 10f);
            t.AddText("FMOD Audio engine\n(c) Firelight Technologies Pty, Ltd. 2004-2009.", 6f);
            //t.Duration = 10f;
            Parent.Add(t);
            t.ScaleVector = new Vector2(1f, 1f);
            t.Motion.Scale = 0.35f ;/// DEFAULT_SCALE;
            t.Motion.Position = new Vector2(0.35f,0.25f);
            //t.DrawInfo.Center = Vector2.Zero;
            t.StartTime = 17f;
        }

        protected override bool ScreenBorderHit()
        {
            if (numberOfZoomOuts < 0)
            {
                numberOfZoomOuts++;
                Motion.Scale /= 2.0f;
                //Motion.ScaleTarget /= 2.0f;
                //Motion.ScaleSpeed = 0.2f;
                return false;
            }
            return true;
        }

        /// <summary>
        /// called when player wins game
        /// </summary>
        protected void PixieHasWon()
        {
            SubtitleText t = new SubtitleText();
            t.AddText("YOU WIN!", 5f);
            t.AddText("The princess\nis rescued.", 4f);
            t.AddText("", 2f);
            t.AddText("But wait a minute...", 3f);
            t.AddText("How do we get out??", 3f);
            t.AddText("*THE END*", 3f);
            Subtitles.Show(6, t);
            hasWon = true;            
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);
            // adapt scroll speed to how fast pixie goes
            Background.TargetSpeed = SCREEN_MOTION_SPEED * pixie.Velocity;

            if (pixie.Target.Equals(WINNING_POSITION))
            {
                timeInWinningPos += p.Dt;

                if (timeInWinningPos > 0.2f && !hasWon)
                {
                    PixieHasWon();
                }
            }else{
                timeInWinningPos = 0f;
            }

            // create new pixels falling
            timerNewBaddie += p.Dt;
            if (timerNewBaddie >= nextBaddieInterval)
            {
                timerNewBaddie = 0f;
                nextBaddieInterval = RandomMath.RandomBetween(0.5f, MaxPauseBetweenBaddies);
                BadPixel b = BadPixel.Create( (int) Math.Round(RandomMath.RandomBetween(-0.49f,4.49f)));
                float x = RandomMath.RandomBetween(40f,50f);
                float y = RandomMath.RandomBetween(-20f,20f);
                b.PositionAndTarget = pixie.Target + new Vector2(x,y);
                AddNextUpdate(b);
            }

            // scroll background
            Background.Target = BG_STARTING_POS + new Vector2(SimTime,0f);
        }
    }
}
