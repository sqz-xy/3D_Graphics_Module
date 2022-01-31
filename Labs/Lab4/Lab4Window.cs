using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

namespace Labs.Lab4
{
    public class Lab4Window : GameWindow
    {
        public Lab4Window()
            : base(
                800, // Width
                600, // Height
                GraphicsMode.Default,
                "Lab 4 Textures",
                GameWindowFlags.Default,
                DisplayDevice.Default,
                3, // major
                3, // minor
                GraphicsContextFlags.ForwardCompatible
                )
        {
        }

        private int[] mVBO_IDs = new int[2];
        private int mVAO_ID;
        private ShaderUtility mShader;

        protected override void OnLoad(EventArgs e)
        {
            // Set some GL state
            GL.ClearColor(Color4.Firebrick);

            float[] vertices = {-0.5f, -0.5f,
                                -0.25f, -0.5f,
                                0.0f, -0.5f,
                                0.25f, -0.5f,
                                0.5f, -0.5f,
                                -0.5f, 0.0f,
                                -0.25f, 0.0f,
                                0.0f, 0.0f,
                                0.25f, 0.0f,
                                0.5f, 0.0f,
                               -0.5f, 0.5f,
                                -0.25f, 0.5f,
                                0.0f, 0.5f,
                                0.25f, 0.5f,
                                0.5f, 0.5f
                                };

            uint[] indices = { 5, 0, 1,
                               5, 1, 6,
                               6, 1, 2,
                               6, 2, 7,
                               7, 2, 3,
                               7, 3, 8,
                               8, 3, 4,
                               8, 4, 9,
                               10, 5, 6,
                               10, 6, 11,
                               11, 6, 7,
                               11, 7, 12,
                               12, 7, 8,
                               12, 8, 13,
                               13, 8, 9,
                               13, 9, 14
                             };

            GL.Enable(EnableCap.CullFace);

            mShader = new ShaderUtility(@"Lab4/Shaders/vTexture.vert", @"Lab4/Shaders/fTexture.frag");
            GL.UseProgram(mShader.ShaderProgramID);
            int vPositionLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vPosition");

            mVAO_ID = GL.GenVertexArray();
            GL.GenBuffers(mVBO_IDs.Length, mVBO_IDs);

            GL.BindVertexArray(mVAO_ID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVBO_IDs[1]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(uint)), indices, BufferUsageHint.StaticDraw);

            int size;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (indices.Length * sizeof(uint) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

            GL.BindVertexArray(0);

            base.OnLoad(e);

        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindVertexArray(mVAO_ID);
            GL.DrawElements(PrimitiveType.Triangles, 48, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);
            this.SwapBuffers();
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.DeleteBuffers(mVBO_IDs.Length, mVBO_IDs);
            GL.DeleteVertexArray(mVAO_ID);
            mShader.Delete();
            base.OnUnload(e);
        }
    }
}
