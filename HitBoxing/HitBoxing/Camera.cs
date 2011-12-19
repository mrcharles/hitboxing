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
    public class Camera
    {
        private Vector2 pos;
        public float Zoom;
        Matrix transform;
        public Rectangle Bounds;
        public Vector2 Viewport;
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

        Vector2 GetClampedPos()
        {
            if (Bounds.IsEmpty)
                return pos;

            Vector2 ret = pos;

            float minx = Bounds.Left + Viewport.X / 2;
            float maxx = Bounds.Right - Viewport.X / 2;
            float miny = Bounds.Top + Viewport.Y / 2;
            float maxy = Bounds.Bottom - Viewport.Y / 2;

            ret.X = Math.Min(Math.Max(pos.X, minx), maxx);
            ret.Y = Math.Min(Math.Max(pos.Y, miny), maxy);

            return ret;

        }

        public virtual void Update()
        {
            Vector2 position = GetClampedPos();
            transform = Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) *
                        Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                        Matrix.CreateTranslation(new Vector3(Viewport.X * 0.5f, Viewport.Y * 0.5f, 0));
        }

        public Matrix Transform()
        {
            return transform;
        }
    }
}
