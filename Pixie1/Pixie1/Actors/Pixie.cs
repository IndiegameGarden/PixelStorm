using System;
using TTengine.Core;
using TTengine.Util;
using Microsoft.Xna.Framework;
using Pixie1;
using Pixie1.Behaviors;

namespace Pixie1.Actors
{
    public class Pixie: Thing
    {
        public int Score = 0;

        public Pixie()
            : base("t-t-2-white")
        {            
            IsCollisionFree = false;
            DrawInfo.DrawColor = new Color(251, 101, 159);
            Velocity = 1.5f;
        }

        protected override void OnDraw(ref DrawParams p)
        {
            base.OnDraw(ref p);
        }

        protected override void OnUpdate(ref UpdateParams p)
        {
            base.OnUpdate(ref p);
        }

    }
}
