using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing.Imaging;
using System.Drawing;

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

        private readonly int[] mVBO_IDs = new int[2];
        private int mVAO_ID;
        private ShaderUtility mShader;
        private int mTexture_ID;
        private int mTexture_ID_2;
        private int mRateOfDissolve = 1;
        private float mThreshold = 0.1f;

        protected override void OnLoad(EventArgs e)
        {
            // ASK ABOUT MULTIPLE TEXTURE LOADING
            // ASK ABOUT TEXTURE VERTICES
            // Set some GL state
            GL.ClearColor(Color4.Firebrick);

            float[] vertices = {-0.5f, -0.5f, -0.5f, -0.5f,
                                -0.25f, -0.5f, -0.25f, -0.5f,
                                0.0f, -0.5f, 0.0f, -0.5f,
                                0.25f, -0.5f,  0.25f, -0.5f,
                                0.5f, -0.5f, 0.5f, -0.5f,
                                -0.5f, 0.0f, -0.5f, 0.0f,
                                -0.25f, 0.0f, -0.25f, 0.0f,
                                0.0f, 0.0f, 0.0f, 0.0f,
                                0.25f, 0.0f, 0.25f, 0.0f,
                                0.5f, 0.0f, 0.5f, 0.0f,
                               -0.5f, 0.5f, -0.5f, 0.5f,
                                -0.25f, 0.5f, -0.25f, 0.5f,
                                0.0f, 0.5f, 0.0f, 0.5f,
                                0.25f, 0.5f, 0.25f, 0.5f,
                                0.5f, 0.5f, -0.5f, 0.5f
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
            var vPositionLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vPosition");

            var uThresholdLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uThreshold");
            GL.Uniform1(uThresholdLocation, 0.05f);

            var filepath = @"Lab4/Textures/texture.png";
            if (System.IO.File.Exists(filepath))
            {
                var TextureBitmap = new Bitmap(filepath);
                var TextureData = TextureBitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, TextureBitmap.Width,
                TextureBitmap.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppRgb);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.GenTextures(1, out mTexture_ID);
                GL.BindTexture(TextureTarget.Texture2D, mTexture_ID);
                GL.TexImage2D(TextureTarget.Texture2D,
                0, PixelInternalFormat.Rgba, TextureData.Width, TextureData.Height,
                0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, TextureData.Scan0);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);
                TextureBitmap.UnlockBits(TextureData);
                TextureBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            }
            else
            {
                throw new Exception("Could not find file " + filepath);
            }

            filepath = @"Lab4/Textures/texture2.png";
            if (System.IO.File.Exists(filepath))
            {
                var TextureBitmap = new Bitmap(filepath);
                var TextureData = TextureBitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, TextureBitmap.Width,
                TextureBitmap.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppRgb);

                GL.ActiveTexture(TextureUnit.Texture1);
                GL.GenTextures(1, out mTexture_ID_2);
                GL.BindTexture(TextureTarget.Texture2D, mTexture_ID_2);
                GL.TexImage2D(TextureTarget.Texture2D,
                0, PixelInternalFormat.Rgba, TextureData.Width, TextureData.Height,
                0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, TextureData.Scan0);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);
                TextureBitmap.UnlockBits(TextureData);
                TextureBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            }
            else
            {
                throw new Exception("Could not find file " + filepath);
            }

            var vTexCoordsLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vTexCoords");

            var uTextureSamplerLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uTextureSampler");
            GL.Uniform1(uTextureSamplerLocation, 0);

            var uTextureSamplerLocation2 = GL.GetUniformLocation(mShader.ShaderProgramID, "uTextureSampler2");
            GL.Uniform1(uTextureSamplerLocation2, 0);

            var uTextureIndexLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uTextureIndex");
            GL.Uniform1(uTextureIndexLocation, 0);
         
            mVAO_ID = GL.GenVertexArray();
            GL.GenBuffers(mVBO_IDs.Length, mVBO_IDs);

            GL.BindVertexArray(mVAO_ID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVBO_IDs[1]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(uint)), indices, BufferUsageHint.StaticDraw);

            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int size);
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
            GL.VertexAttribPointer(vPositionLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);

            GL.EnableVertexAttribArray(vTexCoordsLocation);
            GL.VertexAttribPointer(vTexCoordsLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

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

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var timestep = (float)e.Time;
            var thresholdChange = mRateOfDissolve * timestep;
            if (mThreshold + thresholdChange < 0 || mThreshold + thresholdChange > 1)
            {
                mRateOfDissolve = -mRateOfDissolve;
            }
            mThreshold += mRateOfDissolve * timestep;
            var uThresholdLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uThreshold");
            GL.Uniform1(uThresholdLocation, mThreshold);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.DeleteBuffers(mVBO_IDs.Length, mVBO_IDs);
            GL.DeleteVertexArray(mVAO_ID);
            GL.DeleteTexture(mTexture_ID);
            GL.DeleteTexture(mTexture_ID_2);
            mShader.Delete();
            base.OnUnload(e);
        }
    }
}
