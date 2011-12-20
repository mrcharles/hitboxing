using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace HitBoxing
{
    public class CamManual : Camera
    {
        InputHelper.PadHelper pad;
        float ZoomFactor = 1.0f;
        public CamManual(InputHelper.PadHelper control)
        {
            pad = control;
        }

        public override void Update()
        {
            Vector2 vLeft = pad.LeftStick();
            vLeft.Y *= -1;
            Position = Position + vLeft * 5.0f;
            //ZoomFactor = ZoomFactor + pad.RightStick().Y;

            //Zoom = (float)Math.Log(ZoomFactor) / 3.0f + 1.0f;
            float yRight = pad.RightStick().Y;
            //if(yRight > 0.0f)
                Zoom += yRight;
            Position = GetClampedPos();



            base.Update();
        }
    }
}
