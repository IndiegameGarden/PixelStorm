using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pixie1.Levels;
using Pixie1.Actors;

namespace Pixie1.Behaviors
{
    public class MoveUpBehavior: ThingControl
    {
        public Vector2 UP_VECTOR = new Vector2(0f, -1f);

        protected override void OnInit()
        {
            base.OnInit();
            MoveSpeed = PixelStormLevel.SCROLL_SPEED_PIXELS_PER_SEC * 0.2f;
        }

        protected override void OnUpdate(ref TTengine.Core.UpdateParams p)
        {
            base.OnUpdate(ref p);
            IsTargetMoveDefined = true;
            AllowNextMove();
        }

        protected override void OnNextMove()
        {
            base.OnNextMove();
            Pixie p = (ParentThing as Pixie);
            if (!p.CollidesWithBackground(UP_VECTOR))
                TargetMove = UP_VECTOR;
        }
    }
}
