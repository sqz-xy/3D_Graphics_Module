using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

namespace Labs.Lab3
{
    public class Lab3Window : GameWindow
    {
        public Lab3Window()
            : base(
                800, // Width
                600, // Height
                GraphicsMode.Default,
                "Lab 3 Lighting and Material Properties",
                GameWindowFlags.Default,
                DisplayDevice.Default,
                3, // major
                3, // minor
                GraphicsContextFlags.ForwardCompatible
                )
        {
        }

        private readonly int[] mVBO_IDs = new int[5];
        private readonly int[] mVAO_IDs = new int[3];
        private ShaderUtility mShader;
        private ModelUtility mCylinder;
        private ModelUtility mCreature;
        private Vector4 mLightPosition = new Vector4(2, 1, -8.5f, 1);
        private Matrix4 mView, mCreatureModel, mGroundModel, mCylinderModel;

        protected override void OnLoad(EventArgs e)
        {
            // Set some GL state
            GL.ClearColor(Color4.CornflowerBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            mShader = new ShaderUtility(@"Lab3/Shaders/vPassThrough.vert", @"Lab3/Shaders/fLighting.frag");
            GL.UseProgram(mShader.ShaderProgramID);
            var vPositionLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vPosition");
            var vNormalLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vNormal");

            var lightPosition = new Vector4(2, 4, -8.5f, 1);
            lightPosition = Vector4.Transform(lightPosition, mView);

            //Vector4 lightPosition2 = new Vector4(4, 2, -13f, 1);
            //lightPosition2 = Vector4.Transform(lightPosition2, mView);

            //Vector4 lightPosition3 = new Vector4(8, 10, 0f, 1);
            //lightPosition3 = Vector4.Transform(lightPosition3, mView);

            var uLightPositionLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight.Position");
            GL.Uniform4(uLightPositionLocation, lightPosition);

            //int uLightPositionLocation2 = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[1].Position");
            //GL.Uniform4(uLightPositionLocation2, lightPosition2);

            //int uLightPositionLocation3 = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[2].Position");
            //GL.Uniform4(uLightPositionLocation3, lightPosition3);

            var uAmbientReflectivity = GL.GetUniformLocation(mShader.ShaderProgramID, "uMaterial.AmbientReflectivity");
            var colour4 = new Vector3(0.5f, 0.5f, 0.5f);
            GL.Uniform3(uAmbientReflectivity, colour4);

            var uDiffuseReflectivity = GL.GetUniformLocation(mShader.ShaderProgramID, "uMaterial.DiffuseReflectivity");
            var colour5 = new Vector3(0.5f, 0.5f, 0.5f);
            GL.Uniform3(uDiffuseReflectivity, colour5);

            var uSpecularReflectivity = GL.GetUniformLocation(mShader.ShaderProgramID, "uMaterial.SpecularReflectivity");
            var colour6 = new Vector3(0.5f, 0.5f, 0.5f);
            GL.Uniform3(uSpecularReflectivity, colour6);

            var uShininess = GL.GetUniformLocation(mShader.ShaderProgramID, "uMaterial.Shininess");
            var shininess = 10f;
            GL.Uniform1(uShininess, shininess);

            GL.GenVertexArrays(mVAO_IDs.Length, mVAO_IDs);
            GL.GenBuffers(mVBO_IDs.Length, mVBO_IDs);

            var vertices = new float[] {-10, 0, -10,0,1,0,
                                             -10, 0, 10,0,1,0,
                                             10, 0, 10,0,1,0,
                                             10, 0, -10,0,1,0,};

            GL.BindVertexArray(mVAO_IDs[0]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, BufferUsageHint.StaticDraw);

            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int size);
            if (vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            GL.EnableVertexAttribArray(vNormalLocation);
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 3 * sizeof(float));

            mCreature = ModelUtility.LoadModel(@"Utility/Models/model.bin");

            GL.BindVertexArray(mVAO_IDs[1]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[1]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(mCreature.Vertices.Length * sizeof(float)), mCreature.Vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVBO_IDs[2]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(mCreature.Indices.Length * sizeof(float)), mCreature.Indices, BufferUsageHint.StaticDraw);

            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (mCreature.Vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (mCreature.Indices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            GL.EnableVertexAttribArray(vNormalLocation);
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 3 * sizeof(float));

            // Cylinder
            mCylinder = ModelUtility.LoadModel(@"Utility/Models/cylinder.bin");

            GL.BindVertexArray(mVAO_IDs[2]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[3]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(mCylinder.Vertices.Length * sizeof(float)), mCylinder.Vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVBO_IDs[4]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(mCylinder.Indices.Length * sizeof(float)), mCylinder.Indices, BufferUsageHint.StaticDraw);

            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (mCylinder.Vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (mCylinder.Indices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            GL.EnableVertexAttribArray(vNormalLocation);
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 3 * sizeof(float));

            GL.BindVertexArray(0);

            mView = Matrix4.CreateTranslation(0, -1.5f, 0);
            var uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
            GL.UniformMatrix4(uView, true, ref mView);

            var uEyePosition = GL.GetUniformLocation(mShader.ShaderProgramID, "uEyePosition");
            var EyePosition = new Vector4(mView.ExtractTranslation(), 1);
            GL.Uniform4(uEyePosition, EyePosition);

            mGroundModel = Matrix4.CreateTranslation(0, 0, -5f);
            mCreatureModel = Matrix4.CreateTranslation(0, 2, -5f);
            mCylinderModel = Matrix4.CreateTranslation(0, 0, -5f);

            base.OnLoad(e);

        }

        private void ChangeLightColour(Vector3 pAmbientColour, Vector3 pDiffuseColour, Vector3 pSpecularColour)
        {
            var uAmbientLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight.AmbientLight");
            var colour = new Vector3(pAmbientColour);
            GL.Uniform3(uAmbientLightLocation, colour);

            var uDiffuseLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight.DiffuseLight");
            var colour2 = new Vector3(pDiffuseColour);
            GL.Uniform3(uDiffuseLightLocation, colour2);

            var uSpecularLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight.SpecularLight");
            var colour3 = new Vector3(pSpecularColour);
            GL.Uniform3(uSpecularLightLocation, colour3);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(this.ClientRectangle);
            if (mShader != null)
            {
                var uProjectionLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uProjection");
                var projection = Matrix4.CreatePerspectiveFieldOfView(1, (float)ClientRectangle.Width / ClientRectangle.Height, 0.5f, 25);
                GL.UniformMatrix4(uProjectionLocation, true, ref projection);
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            // Moving Camera
            base.OnKeyPress(e);
            switch (e.KeyChar)
            {
                case 'w':
                {
                    mView *= Matrix4.CreateTranslation(0.0f, 0.0f, 0.05f);
                    var uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
                    GL.UniformMatrix4(uView, true, ref mView);
                    UpdateLightPos();
                    break;
                }
                case 'a':
                {
                    mView *= Matrix4.CreateRotationY(-0.025f);
                    var uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
                    GL.UniformMatrix4(uView, true, ref mView);
                    UpdateLightPos();
                    break;
                }
                // Moving Floor
                case 'q':
                {
                    var t = mGroundModel.ExtractTranslation();
                    var translation = Matrix4.CreateTranslation(t);
                    var inverseTranslation = Matrix4.CreateTranslation(-t);
                    mGroundModel = mGroundModel * inverseTranslation * Matrix4.CreateRotationY(-0.025f) *
                                   translation;
                    UpdateLightPos();
                    break;
                }
                case 'e':
                {
                    var t = mGroundModel.ExtractTranslation();
                    var translation = Matrix4.CreateTranslation(t);
                    var inverseTranslation = Matrix4.CreateTranslation(-t);
                    mGroundModel = mGroundModel * inverseTranslation * Matrix4.CreateRotationY(0.025f) *
                                   translation;
                    UpdateLightPos();
                    break;
                }
                // Rotating Creature
                case 'c':
                {
                    var t = mCreatureModel.ExtractTranslation();
                    var translation = Matrix4.CreateTranslation(t);
                    var inverseTranslation = Matrix4.CreateTranslation(-t);
                    mCreatureModel = mCreatureModel * inverseTranslation * Matrix4.CreateRotationY(-0.025f) *
                                     translation;
                    UpdateLightPos();
                    break;
                }
                case 'v':
                {
                    var t = mCreatureModel.ExtractTranslation();
                    var translation = Matrix4.CreateTranslation(t);
                    var inverseTranslation = Matrix4.CreateTranslation(-t);
                    mCreatureModel = mCreatureModel * inverseTranslation * Matrix4.CreateRotationY(0.025f) *
                                     translation;
                    UpdateLightPos();
                    break;
                }
            }
        }

        private void UpdateLightPos()
        {       
            var uLightPositionLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLightPosition");
            var transformedLightPos = Vector4.Transform(mLightPosition, mView);
            GL.Uniform4(uLightPositionLocation, transformedLightPos);

            UpdateEyePos();
        }

        private void UpdateEyePos()
        {
            var uEyePosition = GL.GetUniformLocation(mShader.ShaderProgramID, "uEyePosition");
            var EyePosition = new Vector4(mView.ExtractTranslation(), 1);
            GL.Uniform4(uEyePosition, EyePosition);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            var uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref mGroundModel);

            GL.BindVertexArray(mVAO_IDs[0]);
            ChangeLightColour(new Vector3(0.0215f, 0.1745f, 0.0215f), new Vector3(0.07568f, 0.61424f, 0.07568f), new Vector3(0.07568f, 0.61424f, 0.07568f));
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);

            var m = mCreatureModel * mGroundModel;
            uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref m);
           
            GL.BindVertexArray(mVAO_IDs[1]);
            ChangeLightColour(new Vector3(0.25f, 0.20725f, 0.20725f), new Vector3(1f, 0.829f, 0.829f), new Vector3(0.296648f, 0.296648f, 0.296648f));
            GL.DrawElements(PrimitiveType.Triangles, mCreature.Indices.Length, DrawElementsType.UnsignedInt, 0);

            var m2 = mCylinderModel * mGroundModel;
            uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref m2);

            GL.BindVertexArray(mVAO_IDs[2]);
            ChangeLightColour(new Vector3(0.5f, 0f, 0f), new Vector3(0.4f, 0.4f, 0.7f), new Vector3(0.7f, 0.4f, 0.4f));
            GL.DrawElements(PrimitiveType.Triangles, mCylinder.Indices.Length, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);
            this.SwapBuffers();
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.DeleteBuffers(mVBO_IDs.Length, mVBO_IDs);
            GL.DeleteVertexArrays(mVAO_IDs.Length, mVAO_IDs);
            mShader.Delete();
            base.OnUnload(e);
        }
    }
}
