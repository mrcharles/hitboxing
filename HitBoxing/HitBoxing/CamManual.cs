using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HitBoxing
{
    public class CamManual : Camera
    {
        InputHelper.PadHelper pad;
        public CamManual(InputHelper.PadHelper control)
        {
            pad = control;
        }

        public override void Update()
        {
            Position = Position + pad.RightStick() * 5.0f;
            base.Update();
        }
    }
}
