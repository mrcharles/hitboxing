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
    class CamKeyboard : Camera
    {
        InputHelper input;
        float PanSpeed = 5.0f;
        float ZoomSpeed = 0.1f;

        public CamKeyboard(InputHelper _input)
        {
            input = _input;
        }
        public override void Update()
        {
            Vector2 vPan = new Vector2();
            float fZoom = 0.0f;
            if (input.Pressed(Keys.Left))
            {
                vPan.X -= PanSpeed;
            }
            if (input.Pressed(Keys.Right))
            {
                vPan.X += PanSpeed;
            }
            if (input.Pressed(Keys.Up))
            {
                if(input.Pressed(Keys.LeftShift))
                    fZoom += ZoomSpeed;
                else
                    vPan.Y -= PanSpeed;
            }
            if (input.Pressed(Keys.Down))
            {
                if(input.Pressed(Keys.LeftShift))
                    fZoom -= ZoomSpeed;
                else
                    vPan.Y += PanSpeed;
            }

            Position = Position + vPan;
            Zoom += fZoom;

            Position = GetClampedPos();

            base.Update();
        }
    }
}
