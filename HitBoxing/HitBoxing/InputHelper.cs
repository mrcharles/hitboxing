using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace HitBoxing
{
    public class InputHelper
    {
        public class PadHelper
        {
            public PlayerIndex Index;
            GamePadState OldState;
            GamePadState NewState;

            public PadHelper(PlayerIndex index)
            {
                Index = index;
                OldState = NewState = GamePad.GetState(Index);
            }

            public bool IsConnected()
            {
                if(OldState == null)
                    throw new Exception("No state to test.");

                return OldState.IsConnected;

            }

            public void Update()
            {
                OldState = NewState;
                NewState = GamePad.GetState(Index);
            }

            public Vector2 RightStick()
            {
                return NewState.ThumbSticks.Right;
            }

            public Vector2 LeftStick()
            {
                return NewState.ThumbSticks.Left;
            }

            public bool Pressed(Buttons b)
            {
                return NewState.IsButtonDown(b);
            }

            public bool JustPressed(Buttons b)
            {
                return NewState.IsButtonDown(b) && OldState.IsButtonUp(b);
            }

            public bool JustReleased(Buttons b)
            {
                return NewState.IsButtonUp(b) && OldState.IsButtonDown(b);
            }
        }
        KeyboardState OldKeyState;
        KeyboardState NewKeyState;

        PadHelper [] Pads = new PadHelper[4];

        bool[] PadAssigned = new bool[] { false, false, false, false };

        public InputHelper()
        {
            OldKeyState = NewKeyState = Keyboard.GetState();
            for (int i = 0; i < 4; i++)
            {
                Pads[i] = new PadHelper((PlayerIndex)i);
            }
        }

        public void Update()
        {
            for (int i = 0; i < 4; i++)
            {
                Pads[i].Update();
            }

            OldKeyState = NewKeyState;
            NewKeyState = Keyboard.GetState();
        }

        public PadHelper AcquireNewPad()
        {
            for (int i = 0; i < 4; i++)
            {
                if (!PadAssigned[i] && Pads[i].IsConnected())
                {
                    PadAssigned[i] = true;
                    return Pads[i];
                }
            }
            throw new Exception("No pad available.");
        }

        public void ReleasePad(PadHelper pad)
        {
            for (int i = 0; i < 4; i++)
            {
                if (Pads[i].Index == pad.Index)
                {
                    PadAssigned[i] = false;
                    return;
                }
            }

        }

        public bool Pressed(Keys k)
        {
            return NewKeyState.IsKeyDown(k);
        }

        public bool JustPressed(Keys k)
        {
            return NewKeyState.IsKeyDown(k) && OldKeyState.IsKeyUp(k);
        }

        public bool JustReleased(Keys k)
        {
            return NewKeyState.IsKeyUp(k) && OldKeyState.IsKeyDown(k);
        }
    }
}
