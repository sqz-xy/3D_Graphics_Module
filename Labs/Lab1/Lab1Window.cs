﻿using System;
using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Labs.Lab1
{
    public class Lab1Window : GameWindow
    {
        private int mVertexBufferObjectID;
        private ShaderUtility mShader;

        //TODO: TAKE NOTE OF CULLING, WINDING 

        public Lab1Window()
            : base(
                800, // Width
                600, // Height
                GraphicsMode.Default,
                "Lab 1 Hello, Triangle",
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
            GL.ClearColor(Color4.Green);
            GL.Enable(EnableCap.CullFace);

            float[] vertices = new float[] { -0.8f, 0.8f, // Top Left Corner, Triangle 1
                                             -0.8f, -0.8f, // Bottom Left corner, Triangle 1
                                             0.8f, 0.8f, // Top Right corner, Triangle 1
                                              -0.8f, -0.8f, // Bottom Left corner, Triangle 2
                                              0.8f, -0.8f, // Bottom Right corner, Triangle 2
                                              0.8f, 0.8f }; // Top Right corner, Triangle 2

            GL.GenBuffers(1, out mVertexBufferObjectID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVertexBufferObjectID);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, BufferUsageHint.StaticDraw);

            int size;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);

            if (vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            #region Shader Loading Code - Can be ignored for now

            mShader = new ShaderUtility( @"Lab1/Shaders/vSimple.vert", @"Lab1/Shaders/fSimple.frag");

            #endregion

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVertexBufferObjectID);

            // shader linking goes here
            #region Shader linking code - can be ignored for now

            GL.UseProgram(mShader.ShaderProgramID);
            int vPositionLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vPosition");
            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

            #endregion

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            this.SwapBuffers();
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            GL.DeleteBuffers(1, ref mVertexBufferObjectID);
            GL.UseProgram(0);
            mShader.Delete();
        }
    }
}
