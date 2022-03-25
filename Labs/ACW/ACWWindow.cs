// TODO: Get Lighting working with textures, Add frame buffer objects, Add spotlights

using Labs.Utility;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using System;

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

        // Bool to indicate which view type is enabled
        private bool mStaticViewEnabled;

        // Lighting Properties
        private LightingProperties mLightingProperties;

        // Handlers
        private readonly VertexDataHandler mVertexDataHandler;
        private readonly TextureHandler mTextureHandler;

        // Constants
        private const int mVAOSize = 6;
        private const int mVBOSize = 12;
        private const int mTextureSize = 2;

        private const float mDirectionalSpeed = 0.4f;
        private const float mRotationalSpeed = 0.1f;

        private const float mCubeRotationRate = 10f;
        private const float mConeScaleRate = 1f;
        private const float mCreatureMoveRate = 4f;

        // Indexes for VAOs and Textures
        private int mFloorIndex, mWallIndex, mCreatureIndex, mCylinderIndex, mCubeIndex, mConeIndex;
        private int mTexture1Index, mTexture2Index;

        // Misc Variables
        private float mCubeAngle;
        private float mConeScale;

        private bool mCreatureUp;
        private bool mConeBigOrSmall;

        // Vertices and Indices
        #region Vertices and Indices Initialization

        // Removed tex coords from cube as they don't work due to reduced number of triangles
        private readonly float[] mCubeVertices = new float[]
        {
            -0.5f,  0.5f,  0.5f, 1, 0, 0,
            0.5f,  0.5f,  0.5f, 1, 0, 0,
            -0.5f, -0.5f,  0.5f, 1, 0, 0,
            0.5f, -0.5f,  0.5f, 1, 0, 0,
            -0.5f, -0.5f, -0.5f, 0, 0, -1,
            0.5f, -0.5f, -0.5f, 0, 0, -1,
            -0.5f,  0.5f, -0.5f, 0, 0, -1,
            0.5f,  0.5f, -0.5f, 0, 0, -1,
            -0.5f,  0.5f,  0.5f, 1, 1, 0,
            -0.5f,  0.5f, -0.5f, 0, 1, -1,
            0.5f,  0.5f,  0.5f, 1, 1, 0,
            0.5f,  0.5f, -0.5f, 0, 1, -1
        };

        private readonly int[] mCubeIndices = new int[]
        {
            0, 2, 1, 2, 3, 1,
            8, 9, 2, 9, 4, 2,
            2, 4, 3, 4, 5, 3,
            3, 5, 10, 5, 11, 10,
            4, 6, 5, 6, 7, 5,
            6, 0, 7, 0, 1, 7
        };

        private readonly float[] mFloorVertices = new float[] 
        {
            -10, 0,-10, 0, 1, 0, 0.0f, 0.0f,
            -10, 0, 10, 0, 1, 0, 0.0f, 1.0f,
             10, 0, 10, 0, 1, 0, 1.0f, 1.0f,
             10, 0,-10, 0, 1, 0, 1.0f, 0.0f
        };

        private readonly int[] mFloorIndices = new int[]
        {
            0, 1, 2, 3
        };

        private readonly float[] mBackWallVertices = new float[]
        {
            -10, 10,-10, 0, 0, 1, 0.0f, 0.0f,
            -10, 0, -10, 0, 0, 1, 0.0f, 1.0f,
             10, 0, -10, 0, 0, 1, 1.0f, 1.0f,
             10, 10,-10, 0, 0, 1, 1.0f, 0.0f
        };

        private readonly int[] mBackWallIndices = new int[]
        {
            0, 1, 2, 3
        };

        private readonly float[] mConeVertices = new float[]
        {
                0f,    4f,  0f,    0, 1, 0,
             0.25f,-0.25f,  0.75f, 1, 0, 1,
             0.75f,-0.25f,  0.25f, 1, 0, 1,
                1f,-0.25f, -0.25f, 1, 0,-1,
             0.25f,-0.25f, -0.75f, 1, 0,-1,
            -0.25f,-0.25f, -0.75f, 1, 0,-1,
            -0.75f,-0.25f,  0.25f,-1, 0,-1,
            -0.75f,-0.25f,  0.25f,-1, 0, 1,
            -0.25f,-0.25f,  0.75f,-1, 0, 1,
        };

        private readonly int[] mConeIndices = new int[]
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
            // Static view is off by default
            mStaticViewEnabled = false;

            // Default Values for transformations
            mCubeAngle = 0.1f;
            mConeScale = 0.1f;
            mCreatureUp = true;
            mConeBigOrSmall = true;

            // Handlers
            mVertexDataHandler = new VertexDataHandler(mVBOSize, mVAOSize);
            mTextureHandler = new TextureHandler(mTextureSize);

            // Light properties struct
            mLightingProperties = new LightingProperties(4);
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

            /*
            * Didn't implement a second shader because I didn't need too.
            * In order to do it and use it, I would declare a ShaderUtility
            * similar to below, then call GL.UseProgram(shader);
            * between draw calls where I want to use the different shader
            */

            mLightingShader = new ShaderUtility(@"ACW/Shaders/vVertexShader.vert", @"ACW/Shaders/fFragmentShader.frag");
            GL.UseProgram(mLightingShader.ShaderProgramID);

            var vPositionLocation = GL.GetAttribLocation(mLightingShader.ShaderProgramID, "vPosition");
            var vNormalLocation = GL.GetAttribLocation(mLightingShader.ShaderProgramID, "vNormal");
            var vTexCoordsLocation = GL.GetAttribLocation(mLightingShader.ShaderProgramID, "vTexCoords");

            // Bind Texture Data:
            // Floor
            mTexture1Index = mTextureHandler.BindTextureData("ACW/Textures/FloorTexture.jpg");

            // Wall
            mTexture2Index = mTextureHandler.BindTextureData("ACW/Textures/WallTexture.jpg");

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

            // Initialize the matrices for the models and view
            InitializeMatrices();

            // Initialize light properties
            InitializeLightProperties();

            // Bind the light properties and other values to the fragment shader
            InitializeFragValues();

            // Transform the light positions relative to the camera on initial launch
            TransformLightPos(mNonStaticView);
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
                var uProjectionLocation = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uProjection");
                var projection = Matrix4.CreatePerspectiveFieldOfView(1, (float)ClientRectangle.Width / ClientRectangle.Height, 0.5f, 25);
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
            var deltaTime = (float)e.Time;

            // Creature moving up and down
            TransformCreature(deltaTime);

            // Cube rotating
            TransformCube(deltaTime);

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
            var uModel = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref mGroundModel);

            // For multiple textures
            var uTextureIndexLocation = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uTextureIndex");

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
        /// Initializes values for the light properties struct
        /// </summary>
        private void InitializeLightProperties()
        {
            // The three light positions
            mLightingProperties.LightPositions[0] = new Vector4(3, 0.5f, -9.5f, 1);
            mLightingProperties.LightPositions[1] = new Vector4(-3, 0.5f, -9.5f, 1);
            mLightingProperties.LightPositions[2] = new Vector4(0, 0.5f, -12.5f, 1);
            mLightingProperties.LightPositions[3] = new Vector4(0, 0.5f, -6.5f, 1);

            // The three light colours
            mLightingProperties.LightColours[0] = new Vector3(0.5f, 0, 0);
            mLightingProperties.LightColours[1] = new Vector3(0, 0.5f, 0);
            mLightingProperties.LightColours[2] = new Vector3(0, 0, 0.5f);
            mLightingProperties.LightColours[3] = new Vector3(0.25f, 0, 0.25f);

            // The three light reflectivity pro
            mLightingProperties.AmbientReflectivity = new Vector3(0.1f, 0.1f, 0.1f);
            mLightingProperties.DiffuseReflectivity = new Vector3(0.5f, 0.5f, 0.5f);
            mLightingProperties.SpecularReflectivity = new Vector3(0.7f, 0.7f, 0.7f);

            mLightingProperties.Shininess = 15f;
        }

        /// <summary>
        /// Initializes the view matrices and translation matrices for models
        /// </summary>
        private void InitializeMatrices()
        {
            mNonStaticView = Matrix4.CreateTranslation(0, -1.5f, 0);
            var uView = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uView");
            GL.UniformMatrix4(uView, true, ref mNonStaticView);

            var eye = new Vector3(0f, 5f, 5f);
            var lookAt = new Vector3(0, 0, -5f);
            var up = new Vector3(0, 1, 0);
            mStaticView = Matrix4.LookAt(eye, lookAt, up);

            mGroundModel = Matrix4.CreateTranslation(0, 0, -5f);
            mCreatureModel = Matrix4.CreateTranslation(0, 2, -5f);
            mLeftCylinder = Matrix4.CreateTranslation(-5, 0, -5f);
            mMiddleCylinder = Matrix4.CreateTranslation(0, 0, -5f);
            mRightCylinder = Matrix4.CreateTranslation(5, 0, -5f);
            mCubeModel = Matrix4.CreateTranslation(-4.82f, 2, -4.82f);
            mConeModel = Matrix4.CreateTranslation(5, 1f, -5f);
        }

        /// <summary>
        /// Initializes the lighting values for diffuse, ambient and specular light
        /// </summary>
        private void InitializeFragValues()
        {
            // Sets the eye position in the fragment shader using the views
            var uEyePosition = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uEyePosition");
            var eyePosition = new Vector4(mNonStaticView.ExtractTranslation(), 1);
            GL.Uniform4(uEyePosition, eyePosition);

            // Sets texture indexes in the fragment shader
            var uTextureSamplerLocation1 = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uTextureSampler1");
            GL.Uniform1(uTextureSamplerLocation1, mTexture1Index);

            var uTextureSamplerLocation2 = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uTextureSampler2");
            GL.Uniform1(uTextureSamplerLocation2, mTexture2Index);

            // Positional Lighting, per fragment
            var uAmbientReflectivity = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uMaterial.AmbientReflectivity");
            GL.Uniform3(uAmbientReflectivity, mLightingProperties.AmbientReflectivity);

            var uDiffuseReflectivity = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uMaterial.DiffuseReflectivity");
            GL.Uniform3(uDiffuseReflectivity, mLightingProperties.DiffuseReflectivity);

            var uSpecularReflectivity = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uMaterial.SpecularReflectivity");
            GL.Uniform3(uSpecularReflectivity, mLightingProperties.SpecularReflectivity);

            var uShininess = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uMaterial.Shininess");
            GL.Uniform1(uShininess, mLightingProperties.Shininess);
        }

        #endregion

        #region Update Utility Functions

        /// <summary>
        /// Transforms the cube, rotates constantly
        /// </summary>
        /// <param name="deltaTime">The current time step</param>
        private void TransformCube(float deltaTime)
        {
            var cubeRotation = Matrix4.CreateRotationY(mCubeAngle);
            mCubeModel = cubeRotation;
            mCubeModel *= Matrix4.CreateTranslation(-4.92f, 2f, -4.92f);
            mCubeAngle -= mCubeRotationRate * deltaTime;
        }

        /// <summary>
        /// Transforms the creature, it moves up and then down depending on the bounds
        /// </summary>
        /// <param name="deltaTime">The current time step</param>
        private void TransformCreature(float deltaTime)
        {
            var cubeTranslationUp = Matrix4.CreateTranslation(0, mCreatureMoveRate * deltaTime, 0);
            var cubeTranslationDown = Matrix4.CreateTranslation(0, -mCreatureMoveRate * deltaTime, 0);

            var creaturePos = mCreatureModel.ExtractTranslation();

            switch (mCreatureUp)
            {
                case true:
                    mCreatureModel *= cubeTranslationUp;
                    break;
                case false:
                    mCreatureModel *= cubeTranslationDown;
                    break;
            }

            if (creaturePos.Y > 5)
            {
                mCreatureUp = false;
            }
            if (creaturePos.Y < 2)
            {
                mCreatureUp = true;
            }
        }

        /// <summary>
        /// Transforms the Cone, scales up and down depending on the bounds
        /// </summary>
        /// <param name="pDeltaTime">The current time step</param>
        private void TransformCone(float pDeltaTime)
        {
            var newConeScale = Matrix4.CreateScale(mConeScale);
            var currentConeScale = mConeModel.ExtractScale();

            switch (mConeBigOrSmall)
            {
                case true:
                    mConeModel = newConeScale;
                    mConeModel *= Matrix4.CreateTranslation(5, 1.25f, -5f);
                    mConeScale += mConeScaleRate * pDeltaTime;
                    break;
                case false:
                    mConeModel = newConeScale;
                    mConeModel *= Matrix4.CreateTranslation(5, 1.25f, -5f);
                    mConeScale -= mConeScaleRate * pDeltaTime;
                    break;
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
        /// Iterates through the light colours and sets them in the fragment shader appropriately, called before drawing
        /// </summary>
        private void SetLightColours()
        {
            for (var lightIndex = 0; lightIndex < mLightingProperties.LightCount; lightIndex++)
            {
                var uAmbientLightLocation = GL.GetUniformLocation(mLightingShader.ShaderProgramID, $"uLight[{lightIndex}].AmbientLight");
                GL.Uniform3(uAmbientLightLocation, mLightingProperties.LightColours[lightIndex]);

                var uDiffuseLightLocation = GL.GetUniformLocation(mLightingShader.ShaderProgramID, $"uLight[{lightIndex}].DiffuseLight");
                GL.Uniform3(uDiffuseLightLocation, mLightingProperties.LightColours[lightIndex]);

                var uSpecularLightLocation = GL.GetUniformLocation(mLightingShader.ShaderProgramID, $"uLight[{lightIndex}].SpecularLight");
                GL.Uniform3(uSpecularLightLocation, mLightingProperties.LightColours[lightIndex]);
            }
        }

        /// <summary>
        /// Handles draw calls to each VAO
        /// </summary>
        /// <param name="uModel">The model link within the fragment shader</param>
        /// <param name="uTextureIndexLocation">The texture index</param>
        private void DrawGeometry(int uModel, int uTextureIndexLocation)
        {
            SetLightColours();

            // Floor
            GL.Uniform1(uTextureIndexLocation, mTexture1Index);
            GL.BindVertexArray(mVertexDataHandler.GetVAOAtIndex(mFloorIndex));
            GL.DrawElements(PrimitiveType.TriangleFan, mFloorIndices.Length, DrawElementsType.UnsignedInt, 0);

            // Back wall
            GL.Uniform1(uTextureIndexLocation, mTexture2Index);
            GL.BindVertexArray(mVertexDataHandler.GetVAOAtIndex(mWallIndex));
            GL.DrawElements(PrimitiveType.TriangleFan, mBackWallIndices.Length, DrawElementsType.UnsignedInt, 0);

            // Creature
            var m = mCreatureModel * mGroundModel;
            GL.UniformMatrix4(uModel, true, ref m);

            GL.BindVertexArray(mVertexDataHandler.GetVAOAtIndex(mCreatureIndex));
            GL.DrawElements(PrimitiveType.Triangles, mCreature.Indices.Length, DrawElementsType.UnsignedInt, 0);

            // Left Cylinder
            var m2 = mLeftCylinder * mGroundModel;
            GL.UniformMatrix4(uModel, true, ref m2);

            GL.BindVertexArray(mVertexDataHandler.GetVAOAtIndex(mCylinderIndex));
            GL.DrawElements(PrimitiveType.Triangles, mCylinder.Indices.Length, DrawElementsType.UnsignedInt, 0);

            // Middle Cylinder
            var m3 = mMiddleCylinder * mGroundModel;
            GL.UniformMatrix4(uModel, true, ref m3);

            GL.BindVertexArray(mVertexDataHandler.GetVAOAtIndex(mCylinderIndex));
            GL.DrawElements(PrimitiveType.Triangles, mCylinder.Indices.Length, DrawElementsType.UnsignedInt, 0);

            // Right Cylinder
            var m4 = mRightCylinder * mGroundModel;
            GL.UniformMatrix4(uModel, true, ref m4);

            GL.BindVertexArray(mVertexDataHandler.GetVAOAtIndex(mCylinderIndex));
            GL.DrawElements(PrimitiveType.Triangles, mCylinder.Indices.Length, DrawElementsType.UnsignedInt, 0);

            // Cube
            var m5 = mCubeModel * mGroundModel;
            GL.UniformMatrix4(uModel, true, ref m5);

            GL.BindVertexArray(mVertexDataHandler.GetVAOAtIndex(mCubeIndex));
            GL.DrawElements(PrimitiveType.Triangles, mCubeIndices.Length, DrawElementsType.UnsignedInt, 0);

            // Cone
            var m6 = mConeModel * mGroundModel;
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
        private void CameraMovementOnPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == 'p')
            {
                mStaticViewEnabled = !mStaticViewEnabled;

                if (mStaticViewEnabled)
                {
                    var uView = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uView");
                    GL.UniformMatrix4(uView, true, ref mStaticView);
                    MoveCamera(mStaticView);

                }
                else
                {
                    var uView = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uView");
                    GL.UniformMatrix4(uView, true, ref mNonStaticView);
                    MoveCamera(mNonStaticView);
                }

            }

            if (mStaticViewEnabled) return;
            switch (e.KeyChar)
            {
                case 'a':
                    mNonStaticView *= Matrix4.CreateTranslation(mDirectionalSpeed, 0, 0);
                    MoveCamera(mNonStaticView);
                    break;
                case 'd':
                    mNonStaticView *= Matrix4.CreateTranslation(-mDirectionalSpeed, 0, 0);
                    MoveCamera(mNonStaticView);
                    break;
                case 'w':
                    mNonStaticView *= Matrix4.CreateTranslation(0, 0, mDirectionalSpeed);
                    MoveCamera(mNonStaticView);
                    break;
                case 's':
                    mNonStaticView *= Matrix4.CreateTranslation(0, 0, -mDirectionalSpeed);
                    MoveCamera(mNonStaticView);
                    break;
                case ' ':
                    mNonStaticView *= Matrix4.CreateTranslation(0, -mDirectionalSpeed, 0);
                    MoveCamera(mNonStaticView);
                    break;
                case 'c':
                    mNonStaticView *= Matrix4.CreateTranslation(0, mDirectionalSpeed, 0);
                    MoveCamera(mNonStaticView);
                    break;
                case 'q':
                    mNonStaticView *= Matrix4.CreateRotationY(-mRotationalSpeed);
                    MoveCamera(mNonStaticView);
                    break;
                case 'e':
                    mNonStaticView *= Matrix4.CreateRotationY(mRotationalSpeed);
                    MoveCamera(mNonStaticView);
                    break;
            }
        }

        /// <summary>
        /// Updates the view matrix when the camera is moved
        /// </summary>
        private void MoveCamera(Matrix4 pView)
        {
            var uViewLocation = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uView");
            GL.UniformMatrix4(uViewLocation, true, ref pView);

            var uEyePosition = GL.GetUniformLocation(mLightingShader.ShaderProgramID, "uEyePosition");
            var eyePosition = new Vector4(pView.ExtractTranslation(), 1);
            GL.Uniform4(uEyePosition, eyePosition);

            TransformLightPos(pView);
        }


        /// <summary>
        /// Transforms the light positions so they stay stationary relative to the current view
        /// </summary>
        /// <param name="pView"></param>
        private void TransformLightPos(Matrix4 pView)
        {
            for (var lightIndex = 0; lightIndex < mLightingProperties.LightCount; lightIndex++)
            {
                mTransformedLightPos = Vector4.Transform(mLightingProperties.LightPositions[lightIndex], pView);
                var uLightPositionLocation = GL.GetUniformLocation(mLightingShader.ShaderProgramID, $"uLight[{lightIndex}].Position");
                GL.Uniform4(uLightPositionLocation, mTransformedLightPos);
            }
        }

        #endregion
    }
}
