using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pixie1.Levels;

namespace Pixie1.Behaviors
{
    public class TetrisFallBehavior: ThingControl
    {
        protected override void OnNextMove()
        {
            base.OnNextMove();
            TargetMove = new Vector2(0f, 1f);
        }

        protected override void OnUpdate(ref TTengine.Core.UpdateParams p)
        {
            base.OnUpdate(ref p);
            IsTargetMoveDefined = true;
            AllowNextMove();
            if (SimTime >= PixelStormLevel.SCROLLING_START_TIME)
            {
                ParentThing.AddNextUpdate(new MoveUpBehavior());
                Delete = true;
            }
        }
    }
}
