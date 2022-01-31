using OpenTK;
using OpenTK.Graphics;
using System;

namespace Labs.Challenge
{
    class ChallengeWindow : GameWindow
    {
        public ChallengeWindow()
            : base(
                800, // Width
                600, // Height
                GraphicsMode.Default,
                "Challenge Window",
                GameWindowFlags.Default,
                DisplayDevice.Default,
                3, // major
                3, // minor
                GraphicsContextFlags.ForwardCompatible
                )
        {
        }

        protected override void OnLoad(EventArgs e)
        {
 	            base.OnLoad(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
 	        base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
        }
    }
}
