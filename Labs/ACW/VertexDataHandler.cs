using Labs.Utility;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using System;

namespace Labs.ACW
{
	public class VertexDataHandler
	{
        private readonly int[] mVBO_IDs;
        private readonly int[] mVAO_IDs;

        private int mVBOIndex;
		private int mVAOIndex;

		public VertexDataHandler(int pVBOSize, int pVAOSize)
		{
            mVBO_IDs = new int[pVBOSize];
            mVAO_IDs = new int[pVAOSize];

            mVBOIndex = 0;
			mVAOIndex = 0;

            // Generates the Vertex arrays and buffers on handler initialization
            GL.GenVertexArrays(mVAO_IDs.Length, mVAO_IDs);
            GL.GenBuffers(mVBO_IDs.Length, mVBO_IDs);
        }

        /// <summary>
        /// Returns the VAO At a specified Index
        /// </summary>
        /// <param name="pVAOIndex">The index to return</param>
        /// <returns>An integer Value</returns>
        public int GetVAOAtIndex(int pVAOIndex)
        {
            return mVAO_IDs[pVAOIndex];
        }

        /// <summary>
        /// Binds and Buffers data to the graphics card
        /// </summary>
        /// <param name="pVertices">The vertices to bind</param>
        /// <param name="pIndices">The indices to bind</param>
        /// <param name="pPositionLocation">The vertex position information from the shader</param>
        /// <param name="pNormalLocation">The vertex normal information from the shader</param>
        /// <returns>The VAO index of the bound vertices</returns>
        public int BindVertexData(float[] pVertices, int[] pIndices, int pPositionLocation, int pNormalLocation, int pTextureLocation)
        {
            // Bind data to the VAO and VBO
            GL.BindVertexArray(mVAO_IDs[mVAOIndex]);
            mVAOIndex++;

            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[mVBOIndex]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(pVertices.Length * sizeof(float)), pVertices, BufferUsageHint.StaticDraw);
            mVBOIndex++;

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVBO_IDs[mVBOIndex]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(pIndices.Length * sizeof(float)), pIndices, BufferUsageHint.StaticDraw);
            mVBOIndex++;

            // Make sure data is buffered correctly
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int size);
            if (pVertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (pIndices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

            // Enable Position, Normal and Texture coordinate vertex attributes for the shader
            // Issue with stride, models don't have tex coords, that's causing issues with the models
            EnableVertexAttributes(pPositionLocation, pNormalLocation, pTextureLocation);
            
            // Unbind buffer for cleanup purposes
            GL.BindVertexArray(0);

            // Returns the index of the VAO That was just bound 2, -1 due to previous increments
            return mVAOIndex - 1;
        }

        /// <summary>
        /// Enables vertex attributes for the shapes and shaders
        /// </summary>
        /// <param name="pPositionLocation">Position location in the shader</param>
        /// <param name="pNormalLocation">Normal location in the shader</param>
        /// <param name="pTextureLocation">Texture location in the shader</param>
        private void EnableVertexAttributes(int pPositionLocation, int pNormalLocation, int pTextureLocation) 
        { 
            // Account for textures using increased stride
            if (pTextureLocation != -1)
            {
                GL.EnableVertexAttribArray(pPositionLocation);
                GL.VertexAttribPointer(pPositionLocation, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 0);

                GL.EnableVertexAttribArray(pNormalLocation);
                GL.VertexAttribPointer(pNormalLocation, 3, VertexAttribPointerType.Float, true, 9 * sizeof(float), 3 * sizeof(float));

                GL.EnableVertexAttribArray(pTextureLocation);
                GL.VertexAttribPointer(pTextureLocation, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 6 * sizeof(float));
            }
            // No texture coordinates available
            else
            {
                GL.EnableVertexAttribArray(pPositionLocation);
                GL.VertexAttribPointer(pPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

                GL.EnableVertexAttribArray(pNormalLocation);
                GL.VertexAttribPointer(pNormalLocation, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 3 * sizeof(float));
            }       
        }

        /// <summary>
        /// Deletes all buffered data
        /// </summary>
        public void DeleteBuffers()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.DeleteBuffers(mVBOIndex, mVBO_IDs);
            GL.DeleteVertexArrays(mVAOIndex, mVAO_IDs);
        }
	}
}

