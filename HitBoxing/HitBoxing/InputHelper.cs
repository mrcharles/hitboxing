using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace HitBoxing
{
    class InputHelper
    {
        public class PadHelper
        {
            public PlayerIndex Index;
            GamePadState OldState;
            GamePadState NewState;

            public PadHelper(PlayerIndex index)
            {
                Index = index;
                Update();
            }

            public void Update()
            {
                OldState = NewState = GamePad.GetState(Index);
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
