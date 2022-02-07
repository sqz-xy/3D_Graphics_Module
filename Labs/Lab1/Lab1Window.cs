using System;
using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Labs.Lab1
{
    public class Lab1Window : GameWindow
    {
        private int[] mVertexBufferObjectIDArray = new int[2];
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

            //float[] vertices = new float[] { -0.8f, 0.8f, // Top Left Corner, Triangle 1
            //                                 -0.8f, -0.8f, // Bottom Left corner, Triangle 1
            //                                 0.8f, 0.8f, // Top Right corner, Triangle 1
            //                                  -0.8f, -0.8f, // Bottom Left corner, Triangle 2
            //                                  0.8f, -0.8f, // Bottom Right corner, Triangle 2
            //                                  0.8f, 0.8f }; // Top Right corner, Triangle 2


            // Triforce
            //float[] vertices = new float[] { 
            //                     -0.4f, 0.0f,
            //                      0.4f, 0.0f,
            //                      0.0f, 0.6f,
            //                     -0.8f, -0.6f,
            //                     0.0f, -0.6f,
            //                     0.8f, -0.6f


            //};

            //uint[] indices = new uint[] { 0,1,2,
            //                              3,4,0,
            //                              4,5,1};

            // House
            float[] vertices = new float[] {
                                 -0.8f, 0.2f, //0
                                 -0.4f, 0.2f, //1
                                 -0.4f, 0.6f, //2
                                  0.8f, 0.2f, //3
                                  0.4f, 0.6f, //4
                                  0.4f, 0.2f, //5
                                  0.0f, 0.6f, //6
                                  0.0f, 0.8f, //7
                                 -0.2f, 0.6f, //8
                                 -0.2f, 0.8f, //9                           
                                 -0.4f, -0.6f,//10
                                  -0.6f, 0.2f,//11
                                 -0.6f, -0.6f,//12
                                   0.2f,-0.6f,//13
                                   0.2f,-0.2f,//14
                                 -0.4f, -0.2f,//15
                                  0.0f, -0.2f,//16
                                  0.6f, -0.2f,//17
                                  0.6f, 0.2f, //18
                                  0.0f, 0.2f, //19
                                  0.4f,-0.2f,  //20 
                                  0.4f,-0.6f, //21
                                  0.6f,-0.6f, //22 
                                  0.6f, -0.2f //23
            };

            //The index of the vertices 0 in the indices corresponds to element 0 in the vertices
            //Reusing 0 in the indices array reuses that vertex
            uint[] indices = new uint[] { 0,1,2,
                                          3,4,5,
                                          0,5,2,
                                          6,7,8,
                                          4,2,5,
                                          9,8,7,
                                          1,11,10,
                                          10,11,12,
                                          10,13,14,
                                          10,14,15,
                                          16,17,18,
                                          19,16,18,
                                          20,21,22,
                                          23,20,22
            }; 

            // Triangle Fan Pentagon
            //float[] vertices = new float[] {  
            //                      0.0f, 0.8f,
            //                      0.8f, 0.4f,
            //                      0.6f, -0.6f,
            //                     -0.6f, -0.6f,
            //                     -0.8f, 0.4f};

            //uint[] indices = new uint[] { 
            //                  0,2,1,
            //                  0,3,2,
            //                  0,4,3};

            //uint[] indices = new uint[] { 0,4,3,
            //                  2,
            //                  1};

            //uint[] indices = new uint[] { 0,4,3,
            //                  2,
            //                  1};
            //L2T10 Stuck on triangle strip

            GL.GenBuffers(2, mVertexBufferObjectIDArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, mVertexBufferObjectIDArray[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices,
            BufferUsageHint.StaticDraw);

            int size;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);

            if (vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVertexBufferObjectIDArray[1]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(uint)),
            indices, BufferUsageHint.StaticDraw);

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out
            size);

            if (indices.Length * sizeof(uint) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

            #region Shader Loading Code - Can be ignored for now

            mShader = new ShaderUtility( @"Lab1/Shaders/vSimple.vert", @"Lab1/Shaders/fSimple.frag");

            #endregion

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVertexBufferObjectIDArray[0]);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVertexBufferObjectIDArray[1]);

            // shader linking goes here
            #region Shader linking code - can be ignored for now

            GL.UseProgram(mShader.ShaderProgramID);
            int vPositionLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vPosition");
            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

            #endregion

            // Second argument is where to start the draw, Third argument is the end of the draw
             
            GL.DrawElements(PrimitiveType.TriangleStrip, 5, DrawElementsType.UnsignedInt, 0);

            this.SwapBuffers();
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            GL.DeleteBuffers(2, mVertexBufferObjectIDArray);
            GL.UseProgram(0);
            mShader.Delete();
        }
    }
}
