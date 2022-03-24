﻿// It now takes 3 Lines to draw an object:
// Buffer using Vertex Data handler and retain the VAO index
// Bind to the VAO index of the object
// Draw Elements

// TODO:

// Extend the data handler to handle textures // DONE
// add second texture for back wall and seperate the textures // DONE

// Upgrade lighting
// Add a final primitive

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
        // Shader and model Utility
        private ShaderUtility mLightingShader;

        private ModelUtility mCylinder;
        private ModelUtility mCreature;

        // Transformation Matrices for models
        private Matrix4 mNonStaticView, mStaticView, mCreatureModel, mGroundModel, mLeftCylinder, mMiddleCylinder, mRightCylinder, mCubeModel, mConeModel;

        private Vector4 mTransformedLightPos;
        private Vector4 mLightPos;

        // Bool to indicate which view type is enabled
        private bool mStaticViewEnabled = false;

        // Handlers
        private VertexDataHandler mVertexDataHandler;
        private TextureHandler mTextureHandler;

        // Constants
        private const int mVAOSize = 6;
        private const int mVBOSize = 12;
        private const int mTextureSize = 2;

        private const float mDirectionalSpeed = 0.4f;
        private const float mRotationalSpeed = 0.1f;

        private const float mCreatureRotationRate = 10f;
        private const float mConeScaleRate = 1f;
        private const float mCubeTranslationRate = 4f;

        // Indexes for VAOs and Textures
        private int mFloorIndex, mWallIndex, mCreatureIndex, mCylinderIndex, mCubeIndex, mConeIndex;
        private int mTexture1Index, mTexture2Index;

        // Misc Variables
        private float mCreatureAngle;
        private float mConeScale;

        private bool mCubeUpOrDown;
        private bool mConeBigOrSmall;

        // Vertices and Indices
        #region Vertices and Indices Initialization

        // Removed tex coords from cube as they dont work due to reduced number of triangles
        readonly float[] mCubeVertices = new float[]
        {
                    -0.5f,  0.5f,  0.5f, 0, 0, 1,
                     0.5f,  0.5f,  0.5f, 1, 0, 1,
                    -0.5f, -0.5f,  0.5f, 0, 1, 1,
                     0.5f, -0.5f,  0.5f, 1, 1, 1,
                    -0.5f, -0.5f, -0.5f, 0, 0, 1,
                     0.5f, -0.5f, -0.5f, 1, 0, 1, 
                    -0.5f,  0.5f, -0.5f, 0, 1, 1, 
                     0.5f,  0.5f, -0.5f, 1, 1, 1, 
                    -0.5f,  0.5f,  0.5f, 1, 1, 1, 
                    -0.5f,  0.5f, -0.5f, 1, 0, 1, 
                     0.5f,  0.5f,  0.5f, 0, 1, 1, 
                     0.5f,  0.5f, -0.5f, 0, 0, 1
        };


        readonly int[] mCubeIndices = new int[]
        {
            0, 2, 1, 2, 3, 1,
            8, 9, 2, 9, 4, 2,
            2, 4, 3, 4, 5, 3,
            3, 5, 10, 5, 11, 10,
            4, 6, 5, 6, 7, 5,
            6, 0, 7, 0, 1, 7
        };

        readonly float[] mFloorVertices = new float[] 
        {
            -10, 0,-10, 0, 1, 0, 0.0f, 0.0f, 1.0f,
            -10, 0, 10, 0, 1, 0, 0.0f, 1.0f, 1.0f,
             10, 0, 10, 0, 1, 0, 1.0f, 1.0f, 1.0f,
             10, 0,-10, 0, 1, 0, 1.0f, 0.0f, 1.0f
        };

        readonly int[] mFloorIndices = new int[]
        {
            0, 1, 2, 3
        };

        readonly float[] mBackWallVertices = new float[]
        {
            -10, 10,-10, 0, 1, 0, 0.0f, 0.0f, 1.0f,
            -10, 0, -10, 0, 1, 0, 0.0f, 1.0f, 1.0f,
             10, 0, -10, 0, 1, 0, 1.0f, 1.0f, 1.0f,
             10, 10,-10, 0, 1, 0, 1.0f, 0.0f, 1.0f
        };

        readonly int[] mBackWallIndices = new int[]
        {
            0, 1, 2, 3
        };

        readonly float[] mConeVertices = new float[]
        {
             0f,  2f,  0f, 0, 0, 1,
             0.5f,  0f,  1f, 1, 0, 1,
            1f, 0f,  0.5f, 0, 1, 1,
             1f, 0f,  -0.5f, 1, 1, 1,
            0.5f, 0f, -1f, 0, 0, 1,
             -0.5f, 0f, -1f, 1, 0, 1,
            -1f,  0f, 0.5f, 0, 1, 1,
             -1f,  0f, 0.5f, 1, 1, 1,
            -0.5f,  0f,  1f, 1, 1, 1,
        };

        readonly int[] mConeIndices = new int[]
        {
            0, 1, 2, 
            0, 2, 3,
            0, 3, 4,
            0, 4, 5,
            0, 5, 6,
            0, 6, 7,
            0, 7, 8,
            0, 8, 1
        };

        #endregion

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
            mCubeUpOrDown = true;
            mConeBigOrSmall = true;

            // Default Values
            mCreatureAngle = 0.1f;
            mConeScale = 0.1f;

            mLightPos = new Vector4(0, 10, 0, 1);

            // Handlers
            mVertexDataHandler = new VertexDataHandler(mVBOSize, mVAOSize);
            mTextureHandler = new TextureHandler(mTextureSize);
        }

        /// <summary>
        /// Called once when the program is run
        /// </summary>
        /// <param name="e">The on load event arguments</param>
        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(Color4.Black);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            /* Didn't implement a second shader because I didn't need too.
            * In order to do it and use it, I would declare a ShaderUtility
            * similar to below, then call GL.UseProgram();
            * between draw calls where I want to use the different shader
            */
            mLightingShader = new ShaderUtility(@"ACW/Shaders/vVertexShader.vert", @"ACW/Shaders/fFragmentShader.frag");
            GL.UseProgram(mLightingShader.ShaderProgramID);

            int vPositionLocation = GL.GetAttribLocation(mLightingShader.ShaderProgramID, "vPosition");
            int vNormalLocation = GL.GetAttribLocation(mLightingShader.ShaderProgramID, "vNormal");
            int vTexCoordsLocation = GL.GetAttribLocation(mLightingShader.ShaderProgramID, "vTexCoords");

            // Bind Texture Data:
            // Floor
            mTexture1Index = mTextureHandler.BindTextureData("ACW/Textures/texture.png");

            // Wall
            mTexture2Index = mTextureHandler.BindTextureData("ACW/Textures/texture2.png");

            // Send them to the fragment shader using their indexes
            int uTextureSamplerLocation1 = GL.GetUniformLocation(mLightingShader.ShaderProgramID,"uTextureSampler1");
            GL.Uniform1(uTextureSamplerLocation1, mTexture1Index);

            int uTextureSamplerLocation2 = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uTextureSampler2");
            GL.Uniform1(uTextureSamplerLocation2, mTexture2Index);

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

            // Cone
            mConeIndex = mVertexDataHandler.BindVertexData(mConeVertices, mConeIndices, vPositionLocation, vNormalLocation, -1);

            InitializeMatrices();

            int uEyePosition = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uEyePosition");
            Vector4 eyePosition = new Vector4(mNonStaticView.ExtractTranslation(), 1);
            GL.Uniform4(uEyePosition, eyePosition);

            // Positional Lighting, per fragment
            Vector4 lightPosition = mLightPos;
            mTransformedLightPos = Vector4.Transform(lightPosition, mNonStaticView);

            int uLightPositionLocation = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uLight.Position");
            GL.Uniform4(uLightPositionLocation, lightPosition);

            int uAmbientReflectivity = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uMaterial.AmbientReflectivity");
            Vector3 colour4 = new Vector3(0.5f, 0.5f, 0.5f);
            GL.Uniform3(uAmbientReflectivity, colour4);

            int uDiffuseReflectivity = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uMaterial.DiffuseReflectivity");
            Vector3 colour5 = new Vector3(0.5f, 0.5f, 0.5f);
            GL.Uniform3(uDiffuseReflectivity, colour5);

            int uSpecularReflectivity = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uMaterial.SpecularReflectivity");
            Vector3 colour6 = new Vector3(0.5f, 0.5f, 0.5f);
            GL.Uniform3(uSpecularReflectivity, colour6);

            int uShininess = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uMaterial.Shininess");
            float shininess = 0.25f;
            GL.Uniform1(uShininess, shininess);
        }

        /// <summary>
        /// Called whenever a key is pressed
        /// </summary>
        /// <param name="e">The key press event arguments</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            CameraMovementOnPress(e);
        }

        /// <summary>
        /// Called whenever the screen is resized, re-transforms the projection
        /// </summary>
        /// <param name="e">The resize event arguments</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(this.ClientRectangle);
            if (mLightingShader != null)
            {
                int uProjectionLocation = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uProjection");
                Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(1, (float)ClientRectangle.Width / ClientRectangle.Height, 0.5f, 25);
                GL.UniformMatrix4(uProjectionLocation, true, ref projection);
            }
        }

        /// <summary>
        /// Called per frame, contains transformations used for animation
        /// </summary>
        /// <param name="e">The frame event arguments</param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            // Delta time for accurate updates, independent of frame rate
            float deltaTime = (float)e.Time;

            // Cube moving up and down
            TransformCube(deltaTime);

            // Creature rotating
            TransformCreature(deltaTime);

            // Cone scaling
            TransformCone(deltaTime);
        }

        /// <summary>
        /// Called per frame, renders objects on screen
        /// </summary>
        /// <param name="e">The frame event arguments</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // For the models
            int uModel = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref mGroundModel);

            // For multiple textures
            int uTextureIndexLocation = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uTextureIndex");

            // Handles geometry drawing
            DrawGeometry(uModel, uTextureIndexLocation);

            GL.BindVertexArray(0);
            this.SwapBuffers();
        }
    
        /// <summary>
        /// On unload, when the window is closed, the code executes
        /// </summary>
        /// <param name="e">The unload event args</param>
        protected override void OnUnload(EventArgs e)
        {
            // Delete buffered vertex data and textures
            mVertexDataHandler.DeleteBuffers();
            mTextureHandler.DeleteTextures();
            mLightingShader.Delete();
            base.OnUnload(e);
        }

        #region Loading Utility Functions

        /// <summary>
        /// Initializes the view matrices and translation matrices for models
        /// </summary>
        private void InitializeMatrices()
        {
            mNonStaticView = Matrix4.CreateTranslation(0, -1.5f, 0);
            int uView = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uView");
            GL.UniformMatrix4(uView, true, ref mNonStaticView);

            Vector3 eye = new Vector3(0f, 5f, 5f);
            Vector3 lookAt = new Vector3(0, 0, -5f);
            Vector3 up = new Vector3(0, 1, 0);
            mStaticView = Matrix4.LookAt(eye, lookAt, up);

            mGroundModel = Matrix4.CreateTranslation(0, 0, -5f);
            mCreatureModel = Matrix4.CreateTranslation(0, 2, -5f);
            mLeftCylinder = Matrix4.CreateTranslation(-5, 0, -5f);
            mMiddleCylinder = Matrix4.CreateTranslation(0, 0, -5f);
            mRightCylinder = Matrix4.CreateTranslation(5, 0, -5f);
            mCubeModel = Matrix4.CreateTranslation(-5, 2, -5f);
            mConeModel = Matrix4.CreateTranslation(5, 2, -5f);
        }

        #endregion

        #region Lighting Utility Functions

        private void ChangeLightColour(Vector3 pAmbientColour, Vector3 pDiffuseColour, Vector3 pSpecularColour)
        {
            int uAmbientLightLocation = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uLight.AmbientLight");
            Vector3 colour = new Vector3(pAmbientColour);
            GL.Uniform3(uAmbientLightLocation, colour);

            int uDiffuseLightLocation = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uLight.DiffuseLight");
            Vector3 colour2 = new Vector3(pDiffuseColour);
            GL.Uniform3(uDiffuseLightLocation, colour2);

            int uSpecularLightLocation = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uLight.SpecularLight");
            Vector3 colour3 = new Vector3(pSpecularColour);
            GL.Uniform3(uSpecularLightLocation, colour3);
        }

        #endregion

        #region Update Utility Functions

        /// <summary>
        /// Transforms the creature, rotates constantly
        /// </summary>
        /// <param name="deltaTime">The current timestep</param>
        private void TransformCreature(float deltaTime)
        {
            Matrix4 creatureRotation = Matrix4.CreateRotationY(mCreatureAngle);
            mCreatureModel = creatureRotation;
            mCreatureModel *= Matrix4.CreateTranslation(0f, 2f, -5f);
            mCreatureAngle += mCreatureRotationRate * deltaTime;
        }

        /// <summary>
        /// Transforms the Cube, it moves up and then down depending on the bounds
        /// </summary>
        /// <param name="deltaTime">The current timestep</param>
        private void TransformCube(float deltaTime)
        {
            Matrix4 cubeTranslationUp = Matrix4.CreateTranslation(0, mCubeTranslationRate * deltaTime, 0);
            Matrix4 cubeTranslationDown = Matrix4.CreateTranslation(0, -mCubeTranslationRate * deltaTime, 0);

            Vector3 cubePos = mCubeModel.ExtractTranslation();

            if (mCubeUpOrDown)
            {
                mCubeModel *= cubeTranslationUp;
            }
            if (!mCubeUpOrDown)
            {
                mCubeModel *= cubeTranslationDown;
            }

            if (cubePos.Y > 8)
            {
                mCubeUpOrDown = false;
            }
            if (cubePos.Y < 2)
            {
                mCubeUpOrDown = true;
            }
        }

        /// <summary>
        /// Transforms the Cone, scales up and down depending on the bounds
        /// </summary>
        /// <param name="pDeltaTime">The current timestep</param>
        private void TransformCone(float pDeltaTime)
        {
            Matrix4 newConeScale = Matrix4.CreateScale(mConeScale);
            Vector3 currentConeScale = mConeModel.ExtractScale();

            if (mConeBigOrSmall)
            {
                mConeModel = newConeScale;
                mConeModel *= Matrix4.CreateTranslation(5, 2, -5f);
                mConeScale += mConeScaleRate * pDeltaTime;
            }
            if (!mConeBigOrSmall)
            {
                mConeModel = newConeScale;
                mConeModel *= Matrix4.CreateTranslation(5, 2, -5f);
                mConeScale -= mConeScaleRate * pDeltaTime;
            }

            if (currentConeScale.Y > 2)
            {
                mConeBigOrSmall = false;
            }
            if (currentConeScale.Y < 0.2)
            {
                mConeBigOrSmall = true;
            }
        }

        #endregion

        #region Drawing Utility Functions

        /// <summary>
        /// Handles draw calls to each VAO
        /// </summary>
        /// <param name="uModel">The model link within the fragment shader</param>
        /// <param name="uTextureIndexLocation">The texture index</param>
        private void DrawGeometry(int uModel, int uTextureIndexLocation)
        {
            // Floor
            GL.Uniform1(uTextureIndexLocation, mTexture1Index);
            GL.BindVertexArray(mVertexDataHandler.GetVAOAtIndex(mFloorIndex));
            ChangeLightColour(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.55f, 0.55f, 0.55f), new Vector3(0.7f, 0.7f, 0.7f));
            GL.DrawElements(PrimitiveType.TriangleFan, mFloorIndices.Length, DrawElementsType.UnsignedInt, 0);

            // Back wall
            GL.Uniform1(uTextureIndexLocation, mTexture2Index);
            GL.BindVertexArray(mVertexDataHandler.GetVAOAtIndex(mWallIndex));
            GL.DrawElements(PrimitiveType.TriangleFan, mBackWallIndices.Length, DrawElementsType.UnsignedInt, 0);

            // Creature
            Matrix4 m = mCreatureModel * mGroundModel;
            GL.UniformMatrix4(uModel, true, ref m);

            GL.BindVertexArray(mVertexDataHandler.GetVAOAtIndex(mCreatureIndex));
            GL.DrawElements(PrimitiveType.Triangles, mCreature.Indices.Length, DrawElementsType.UnsignedInt, 0);

            // Left Cylinder
            Matrix4 m2 = mLeftCylinder * mGroundModel;
            GL.UniformMatrix4(uModel, true, ref m2);

            GL.BindVertexArray(mVertexDataHandler.GetVAOAtIndex(mCylinderIndex));
            GL.DrawElements(PrimitiveType.Triangles, mCylinder.Indices.Length, DrawElementsType.UnsignedInt, 0);

            // Middle Cylinder
            Matrix4 m3 = mMiddleCylinder * mGroundModel;
            GL.UniformMatrix4(uModel, true, ref m3);

            GL.BindVertexArray(mVertexDataHandler.GetVAOAtIndex(mCylinderIndex));
            GL.DrawElements(PrimitiveType.Triangles, mCylinder.Indices.Length, DrawElementsType.UnsignedInt, 0);

            // Right Cylinder
            Matrix4 m4 = mRightCylinder * mGroundModel;
            GL.UniformMatrix4(uModel, true, ref m4);

            GL.BindVertexArray(mVertexDataHandler.GetVAOAtIndex(mCylinderIndex));
            GL.DrawElements(PrimitiveType.Triangles, mCylinder.Indices.Length, DrawElementsType.UnsignedInt, 0);

            // Cube
            Matrix4 m5 = mCubeModel * mGroundModel;
            GL.UniformMatrix4(uModel, true, ref m5);

            GL.BindVertexArray(mVertexDataHandler.GetVAOAtIndex(mCubeIndex));
            GL.DrawElements(PrimitiveType.Triangles, mCubeIndices.Length, DrawElementsType.UnsignedInt, 0);

            // Cone
            Matrix4 m6 = mConeModel * mGroundModel;
            GL.UniformMatrix4(uModel, true, ref m6);

            GL.BindVertexArray(mVertexDataHandler.GetVAOAtIndex(mConeIndex));
            GL.DrawElements(PrimitiveType.TriangleFan, mCubeIndices.Length, DrawElementsType.UnsignedInt, 0);
        }

        #endregion

        #region Camera Utility Functions

        /// <summary>
        /// Manages camera movement based on key press
        /// </summary>
        /// <param name="e">Key press event arguments</param>
        /// <param name="pDirectionalSpeed">The speed of cardinal movement</param>
        /// <param name="pRotationalSpeed">The speed of rotational movement</param>
        private void CameraMovementOnPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == 'p')
            {
                mStaticViewEnabled = !mStaticViewEnabled;

                if (mStaticViewEnabled)
                {
                    int uView = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uView");
                    GL.UniformMatrix4(uView, true, ref mStaticView);

                    int uEyePosition = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uEyePosition");
                    Vector4 EyePosition = new Vector4(mStaticView.ExtractTranslation(), 1);
                    GL.Uniform4(uEyePosition, EyePosition);
                }
                else
                {
                    int uView = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uView");
                    GL.UniformMatrix4(uView, true, ref mNonStaticView);

                    int uEyePosition = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uEyePosition");
                    Vector4 EyePosition = new Vector4(mNonStaticView.ExtractTranslation(), 1);
                    GL.Uniform4(uEyePosition, EyePosition);
                }

            }

            if (!mStaticViewEnabled)
            {
                if (e.KeyChar == 'a')
                {
                    mNonStaticView = mNonStaticView * Matrix4.CreateTranslation(mDirectionalSpeed, 0, 0);
                    MoveCamera();
                }
                if (e.KeyChar == 'd')
                {
                    mNonStaticView = mNonStaticView * Matrix4.CreateTranslation(-mDirectionalSpeed, 0, 0);
                    MoveCamera();
                }
                if (e.KeyChar == 'w')
                {
                    mNonStaticView = mNonStaticView * Matrix4.CreateTranslation(0, 0, mDirectionalSpeed);
                    MoveCamera();
                }
                if (e.KeyChar == 's')
                {
                    mNonStaticView = mNonStaticView * Matrix4.CreateTranslation(0, 0, -mDirectionalSpeed);
                    MoveCamera();
                }
                if (e.KeyChar == ' ')
                {
                    mNonStaticView = mNonStaticView * Matrix4.CreateTranslation(0, -mDirectionalSpeed, 0);
                    MoveCamera();
                }
                if (e.KeyChar == 'c')
                {
                    mNonStaticView = mNonStaticView * Matrix4.CreateTranslation(0, mDirectionalSpeed, 0);
                    MoveCamera();
                }
                if (e.KeyChar == 'q')
                {
                    mNonStaticView = mNonStaticView * Matrix4.CreateRotationY(-mRotationalSpeed);
                    MoveCamera();
                }
                if (e.KeyChar == 'e')
                {
                    mNonStaticView = mNonStaticView * Matrix4.CreateRotationY(mRotationalSpeed);
                    MoveCamera();
                }
            }
        }

        /// <summary>
        /// Updates the view matrix when the camera is moved
        /// </summary>
        private void MoveCamera()
        {
            int uViewLocation = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uView");
            GL.UniformMatrix4(uViewLocation, true, ref mNonStaticView);

            mTransformedLightPos = Vector4.Transform(mLightPos, mNonStaticView);
            int uLightPositionLocation = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uLight.Position");
            GL.Uniform4(uLightPositionLocation, mTransformedLightPos);

            int uEyePosition = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uEyePosition");
            Vector4 EyePosition = new Vector4(mNonStaticView.ExtractTranslation(), 1);
            GL.Uniform4(uEyePosition, EyePosition);
        }

        #endregion
    }
}
