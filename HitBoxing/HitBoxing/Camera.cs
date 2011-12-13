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
    class Camera
    {
        private Vector2 pos;
        public float Zoom;
        public Matrix transform;
        public Vector2 Position
        {
            get
            {
                return pos;
            }
            set
            {
                pos = value;
            }
        }

        public Camera()
        {
            Zoom = 1.0f;
            pos = new Vector2(0,0);
        }

        public Matrix Transform()
        {
            transform =       // Thanks to o KB o for this solution
              Matrix.CreateTranslation(new Vector3(-pos.X, -pos.Y, 0)) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(1280 * 0.5f, 720 * 0.5f, 0));
            return transform;
        }
    }
}
