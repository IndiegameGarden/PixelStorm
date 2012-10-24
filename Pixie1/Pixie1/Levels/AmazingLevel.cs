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
    public class AmazingLevel : Level
    {
        int numberOfZoomOuts = 0;
        Vector2 WINNING_POSITION = new Vector2(192f, 2f);
        float timeInWinningPos = 0f;
        bool hasWon = false;

        public AmazingLevel()
            : base()
        {           
            // Level settings
            SCREEN_MOTION_SPEED = 8.0f;
            DEFAULT_SCALE = 20f;
            PIXIE_STARTING_POS = new Vector2(3f, 51f); // in pixels        
            PIXIE_STARTING_POS = new Vector2(188f, 0f); // close to win pos
            BG_STARTING_POS = new Vector2(3f, 51f); // in pixels; bg=background            
            BG_STARTING_POS = new Vector2(188f, 0f); // in pixels; bg=background            
        }

        protected override void InitLevel()
        {
            base.InitLevel();

            // select bitmap bg
            Background = new LevelBackground("amazing1.png");
            Background.TargetSpeed = SCREEN_MOTION_SPEED;
            Add(Background);
            Background.Target = PIXIE_STARTING_POS;
            Background.Position = BG_STARTING_POS;

        }

        protected override void InitBadPixels()
        {
            base.InitBadPixels();

            BadPixel bp = BadPixel.Create(); // Cloaky();
            bp.PositionAndTarget = new Vector2(72f, 34f);
            //bp.TargetSpeed = 18.0f; // TODO
            Add(bp);

            bp = BadPixel.Create();
            bp.PositionAndTarget = new Vector2(37f, 44f);
            Add(bp);
        }

        protected override void InitToys()
        {
            base.InitToys();

            for (int i = 0; i < 20; i++)
            {
                Toy test = new SpeedModifyToy(2f);
                Vector2 p = PIXIE_STARTING_POS + new Vector2(RandomMath.RandomBetween(10f, 50f), RandomMath.RandomBetween(-40f, 40f));
                test.PositionAndTarget = p;
                Add(test);
            }

            Toy invisToy = new InvisibilityToy();
            invisToy.PositionAndTarget = PIXIE_STARTING_POS + new Vector2(20f, 0f);
            Add(invisToy);

            // attach test
            //test.AttachmentPosition = new Vector2(3f, 0f);
            //pixie.Add(test);
        }

        class MySubtitle: SubtitleText
        {
            public MySubtitle()
            {
                AddText("", 1f);
                AddText("Oh no.", 3f);
                AddText("", 1f);
                AddText("Where am I?", 3f);
                AddText("I'm lost.", 3f);
                AddText("Can you help me\nget back home?", 3f);
                AddText("Wanne be there.", 3f);
                AddText("Always.", 3f);
                AddText("What? If I ever\nstop talking?", 3f);
                AddText("Hardly.", 3f);              
            }

            protected override void OnUpdate(ref UpdateParams p)
            {
                base.OnUpdate(ref p);
            }
        }

        protected override void InitLevelSpecific()
        {
            Music = new GameMusic();
            Add(Music);

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

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);
            // adapt scroll speed to how fast pixie goes
            Background.TargetSpeed = SCREEN_MOTION_SPEED * pixie.Velocity;

            if (pixie.Target.Equals(WINNING_POSITION))
            {
                timeInWinningPos += p.Dt;

                if (timeInWinningPos > 2f && !hasWon)
                {
                    SubtitleText t = new SubtitleText();
                    t.AddText("We DID IT!", 3f);
                    t.AddText("I found my friends!", 3f);
                    t.AddText("Hi, Trixie!", 3f);
                    t.AddText("Hi, Dixie!", 3f);
                    t.AddText("Hi, Fixie!", 3f);
                    t.AddText("Hi, everyone!", 3f);
                    t.AddText("This is...", 3f);
                    t.AddText("AMAZING!!!", 7f);
                    Subtitles.Show(10, t);
                    Motion.ScaleTarget = 3f;
                    Motion.ScaleSpeed = 0.004f;
                    Background.Target = new Vector2(Background.Texture.Width/2, Background.Texture.Height/2);
                    Background.TargetSpeed = 0.005f;
                    hasWon = true;
                    isBackgroundScrollingOn = false;
                }
            }else{
                timeInWinningPos = 0f;
            }
        }
    }
}
