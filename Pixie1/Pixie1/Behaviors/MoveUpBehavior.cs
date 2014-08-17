using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pixie1.Levels;

namespace Pixie1.Behaviors
{
    public class MoveUpBehavior: ThingControl
    {
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
            TargetMove = new Vector2(0f, -1f);
        }
    }
}
