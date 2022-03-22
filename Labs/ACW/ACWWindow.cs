using Labs.Utility;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;

namespace Labs.ACW
{
    public class ACWWindow : GameWindow
    {
        // It now takes 3 Lines to draw an object:
            // Buffer using Vertex Data handler and retain the VAO index
            // Bind to the VAO index of the object
            // Draw Elements

        // TODO,
        // Extend the data handler to handle textures // DONE
        // add second texture for back wall and seperate the textures // DONE

        // Upgrade lighting
        // Add a final primitive

        private ShaderUtility mShader;
        private ModelUtility mCylinder;
        private ModelUtility mCreature;
        private Matrix4 mView, mStaticView, mCreatureModel, mGroundModel, mLeftCylinder, mMiddleCylinder, mRightCylinder, mCubeModel;
        private bool mStaticViewEnabled = false;

        private VertexDataHandler mVertexDataHandler;
        private TextureHandler mTextureHandler;

        private float mCreatureAngle = 0.1f;
        private bool mIsUpOrDown = true;

        private int mFloorIndex, mWallIndex, mCreatureIndex, mCylinderIndex, mCubeIndex;

        // Removed tex coords from cube as they dont work due to reduced number of triangles
        float[] mCubeVertices = new float[]
        {
                -0.5f, -0.5f,  0.5f, 0, 0, 1,
                -0.5f,  0.5f,  0.5f, 1, 0, 0,
                 0.5f,  0.5f,  0.5f, 0, 1, 0,
                 0.5f, -0.5f,  0.5f, 1, 1, 0,
                -0.5f, -0.5f, -0.5f, 1, 1, 1,
                -0.5f,  0.5f, -0.5f, 1, 0, 0,
                 0.5f,  0.5f, -0.5f, 1, 0, 1, 
                 0.5f, -0.5f, -0.5f, 0, 0, 1
        };

        int[] mCubeIndices = new int[]
        {
                0,2,1,  0,3,2,
                4,3,0,  4,7,3,
                4,1,5,  4,0,1,
                3,6,2,  3,7,6,
                1,6,5,  1,2,6,
                7,5,6,  7,4,5
        };

        float[] mFloorVertices = new float[] 
        {
            -10, 0,-10, 0, 1, 0, 0.0f, 0.0f, 1.0f,
            -10, 0, 10, 0, 1, 0, 0.0f, 1.0f, 1.0f,
             10, 0, 10, 0, 1, 0, 1.0f, 1.0f, 1.0f,
             10, 0,-10, 0, 1, 0, 1.0f, 0.0f, 1.0f
        };

        int[] mFloorIndices = new int[]
        {
            0, 1, 2, 3
        };

        float[] mBackWallVertices = new float[]
        {
            -10, 10,-10, 0, 1, 0, 0.0f, 0.0f, 1.0f,
            -10, 0, -10, 0, 1, 0, 0.0f, 1.0f, 1.0f,
             10, 0, -10, 0, 1, 0, 1.0f, 1.0f, 1.0f,
             10, 10,-10, 0, 1, 0, 1.0f, 0.0f, 1.0f
        };

        int[] mBackWallIndices = new int[]
        {
            0, 1, 2, 3
        };

        public ACWWindow()
            : base(
                800, // Width
                600, // Height
                GraphicsMode.Default,
                "Assessed Coursework",
                GameWindowFlags.Default,
                DisplayDevice.Default,
                3, // major
                3, // minor
                GraphicsContextFlags.ForwardCompatible
                )
        {
            mVertexDataHandler = new VertexDataHandler(10, 5);
            mTextureHandler = new TextureHandler(2);
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(Color4.Black);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            mShader = new ShaderUtility(@"ACW/Shaders/vVertexShader.vert", @"ACW/Shaders/fFragmentShader.frag");
            GL.UseProgram(mShader.ShaderProgramID);

            int vPositionLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vPosition");
            int vNormalLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vNormal");
            int vTexCoordsLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vTexCoords");

            int uTextureSamplerLocation1 = GL.GetUniformLocation(mShader.ShaderProgramID,"uTextureSampler1");
            GL.Uniform1(uTextureSamplerLocation1, 0);

            int uTextureSamplerLocation2 = GL.GetUniformLocation(mShader.ShaderProgramID, "uTextureSampler2");
            GL.Uniform1(uTextureSamplerLocation2, 1);

            // Bind Texture Data:
            // Floor
            mTextureHandler.BindTextureData("ACW/Textures/texture.png");

            // Wall
            mTextureHandler.BindTextureData("ACW/Textures/texture2.png");

            // Bind Vertex Data:
            // Floor
            mFloorIndex = mVertexDataHandler.BindVertexData(mFloorVertices, mFloorIndices, vPositionLocation, vNormalLocation, vTexCoordsLocation);

            // Creature
            mCreature = ModelUtility.LoadModel(@"Utility/Models/model.bin");
            mCreatureIndex = mVertexDataHandler.BindVertexData(mCreature.Vertices, mCreature.Indices, vPositionLocation, vNormalLocation, -1);

            // Cylinder
            mCylinder = ModelUtility.LoadModel(@"Utility/Models/cylinder.bin");
            mCylinderIndex = mVertexDataHandler.BindVertexData(mCylinder.Vertices, mCylinder.Indices, vPositionLocation, vNormalLocation, -1);
           
            // Cube
            mCubeIndex = mVertexDataHandler.BindVertexData(mCubeVertices, mCubeIndices, vPositionLocation, vNormalLocation, -1);

            // Back wall
            mWallIndex = mVertexDataHandler.BindVertexData(mBackWallVertices, mBackWallIndices, vPositionLocation, vNormalLocation, vTexCoordsLocation);

            mView = Matrix4.CreateTranslation(0, -1.5f, 0);
            int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
            GL.UniformMatrix4(uView, true, ref mView);

            Vector3 eye = new Vector3(0f, 5f, 5f); // Should be 0.5f temp fix, ask in lab, concerning mView initialisation
            Vector3 lookAt = new Vector3(0, 0, -5f);
            Vector3 up = new Vector3(0, 1, 0);
            mStaticView = Matrix4.LookAt(eye, lookAt, up);

            mGroundModel = Matrix4.CreateTranslation(0, 0, -5f);
            mCreatureModel = Matrix4.CreateTranslation(0, 2, -5f);
            mLeftCylinder = Matrix4.CreateTranslation(-5, 0, -5f);
            mMiddleCylinder = Matrix4.CreateTranslation(0, 0, -5f);
            mRightCylinder = Matrix4.CreateTranslation(5, 0, -5f);
            mCubeModel = Matrix4.CreateTranslation(-5, 2, -5f);

            // Lighting, Currently up to directional

            int uLightDirectionLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLightDirection");
            Vector3 normalisedLightDirection, lightDirection = new Vector3(-1, -1, -1);
            Vector3.Normalize(ref lightDirection, out normalisedLightDirection);
            GL.Uniform3(uLightDirectionLocation, normalisedLightDirection);

        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (e.KeyChar == 'p')
            {
                mStaticViewEnabled = !mStaticViewEnabled;

                if(mStaticViewEnabled)
                {
                    int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
                    GL.UniformMatrix4(uView, true, ref mStaticView);
                }
                else
                {
                    int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
                    GL.UniformMatrix4(uView, true, ref mView);
                }
                
            }

            if (!mStaticViewEnabled)
            {
                if (e.KeyChar == 'a')
                {
                    mView = mView * Matrix4.CreateTranslation(0.1f, 0, 0);
                    MoveCamera();
                }
                if (e.KeyChar == 'd')
                {
                    mView = mView * Matrix4.CreateTranslation(-0.1f, 0, 0);
                    MoveCamera();
                }
                if (e.KeyChar == 'w')
                {
                    mView = mView * Matrix4.CreateTranslation(0, 0, 0.1f);
                    MoveCamera();
                }
                if (e.KeyChar == 's')
                {
                    mView = mView * Matrix4.CreateTranslation(0, 0, -0.1f);
                    MoveCamera();
                }
                if (e.KeyChar == ' ')
                {
                    mView = mView * Matrix4.CreateTranslation(0, -0.1f, 0);
                    MoveCamera();
                }
                if (e.KeyChar == 'c')
                {
                    mView = mView * Matrix4.CreateTranslation(0, 0.1f, 0);
                    MoveCamera();
                }
                if (e.KeyChar == 'q')
                {
                    mView = mView * Matrix4.CreateRotationY(-0.05f);
                    MoveCamera();
                }
                if (e.KeyChar == 'e')
                {
                    mView = mView * Matrix4.CreateRotationY(0.05f);
                    MoveCamera();
                }
            }          
        }
        private void MoveCamera()
        {
            int uViewLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
            GL.UniformMatrix4(uViewLocation, true, ref mView);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(this.ClientRectangle);
            if (mShader != null)
            {
                int uProjectionLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uProjection");
                Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(1, (float)ClientRectangle.Width / ClientRectangle.Height, 0.5f, 25);
                GL.UniformMatrix4(uProjectionLocation, true, ref projection);
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            // Delta time for accurate updates, independent of framerate
            float deltaTime = (float)e.Time;

            // Cube moving up and down
            Matrix4 cubeTranslationUp = Matrix4.CreateTranslation(0 , 4f * deltaTime, 0);
            Matrix4 cubeTranslationDown = Matrix4.CreateTranslation(0, -4f * deltaTime, 0);

            Vector3 cubePos = mCubeModel.ExtractTranslation();
            
            if (mIsUpOrDown)
            {
                mCubeModel *= cubeTranslationUp;
            }
            if (!mIsUpOrDown)
            {
                mCubeModel *= cubeTranslationDown;
            }

            if (cubePos.Y > 8)
            {
                mIsUpOrDown = false;
            }
            if (cubePos.Y < 2)
            {
                mIsUpOrDown = true;
            }

            // Creature rotating
            Matrix4 creatureRotation = Matrix4.CreateRotationY(mCreatureAngle);
            mCreatureModel = creatureRotation;
            mCreatureModel *= Matrix4.CreateTranslation(0f, 2f, -5f);
            mCreatureAngle += (4f * deltaTime);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            int uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref mGroundModel);

            // For multiple textures
            int uTextureIndexLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uTextureIndex");
            
            // Floor
            GL.Uniform1(uTextureIndexLocation, 0);
            GL.BindVertexArray(mVertexDataHandler.GetVAOAtIndex(mFloorIndex));
            GL.DrawElements(PrimitiveType.TriangleFan, mFloorIndices.Length, DrawElementsType.UnsignedInt, 0);

            // Back wall
            GL.Uniform1(uTextureIndexLocation, 1);
            GL.BindVertexArray(mVertexDataHandler.GetVAOAtIndex(mWallIndex));
            GL.DrawElements(PrimitiveType.TriangleFan, mBackWallIndices.Length, DrawElementsType.UnsignedInt, 0);

            // Creature
            Matrix4 m = mCreatureModel * mGroundModel;
            uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref m);

            GL.BindVertexArray(mVertexDataHandler.GetVAOAtIndex(mCreatureIndex));
            GL.DrawElements(PrimitiveType.Triangles, mCreature.Indices.Length, DrawElementsType.UnsignedInt, 0);

            // Left Cylinder
            Matrix4 m2 = mLeftCylinder * mGroundModel;
            uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref m2);

            GL.BindVertexArray(mVertexDataHandler.GetVAOAtIndex(mCylinderIndex));
            GL.DrawElements(PrimitiveType.Triangles, mCylinder.Indices.Length, DrawElementsType.UnsignedInt, 0);

            // Middle Cylinder
            Matrix4 m3 = mMiddleCylinder * mGroundModel;
            uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref m3);

            GL.BindVertexArray(mVertexDataHandler.GetVAOAtIndex(mCylinderIndex));
            GL.DrawElements(PrimitiveType.Triangles, mCylinder.Indices.Length, DrawElementsType.UnsignedInt, 0);

            // Right Cylinder
            Matrix4 m4 = mRightCylinder * mGroundModel;
            uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref m4);

            GL.BindVertexArray(mVertexDataHandler.GetVAOAtIndex(mCylinderIndex));
            GL.DrawElements(PrimitiveType.Triangles, mCylinder.Indices.Length, DrawElementsType.UnsignedInt, 0);

            // Cube
            Matrix4 m5 = mCubeModel * mGroundModel;
            uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref m5);

            GL.BindVertexArray(mVertexDataHandler.GetVAOAtIndex(mCubeIndex));
            GL.DrawElements(PrimitiveType.Triangles, mCubeIndices.Length, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);
            this.SwapBuffers();
        }

        protected override void OnUnload(EventArgs e)
        {
            // Delete buffered vertex data and textures
            mVertexDataHandler.DeleteBuffers();
            mTextureHandler.DeleteTextures();
            mShader.Delete();
            base.OnUnload(e);
        }
    }
}
