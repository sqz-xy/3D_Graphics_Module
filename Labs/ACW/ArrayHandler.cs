﻿using Labs.Utility;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using System;

namespace Labs.ACW
{
	public class ArrayHandler
	{
		private int mVBOIndex;
		private int mVAOIndex;

		public ArrayHandler(ref int[] pVAO_IDs, ref int[] pVBO_IDs)
		{
			mVBOIndex = 0;
			mVAOIndex = 0;

            // Generates the Vertex arrays and buffers on handler initialization
            GL.GenVertexArrays(pVAO_IDs.Length, pVAO_IDs);
            GL.GenBuffers(pVBO_IDs.Length, pVBO_IDs);
        }

        /// <summary>
        /// Binds and Buffers data to the graphics card
        /// </summary>
        /// <param name="pVAO_IDs">A reference to the VAOs</param>
        /// <param name="pVBO_IDs">A reference to the VBOs</param>
        /// <param name="pVertices">The vertices to bind</param>
        /// <param name="pIndices">The indices to bind</param>
        /// <param name="pPositionLocation">The vertex position information from the shader</param>
        /// <param name="pNormalLocation">The vertex normal information from the shader</param>
		public void BufferData(ref int[] pVAO_IDs, ref int[] pVBO_IDs, float[] pVertices, int[] pIndices, int pPositionLocation, int pNormalLocation, int pTextureLocation)
        {
            // Bind data to the VAO and VBO
            GL.BindVertexArray(pVAO_IDs[mVAOIndex]);
            mVAOIndex++;

            GL.BindBuffer(BufferTarget.ArrayBuffer, pVBO_IDs[mVBOIndex]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(pVertices.Length * sizeof(float)), pVertices, BufferUsageHint.StaticDraw);
            mVBOIndex++;

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, pVBO_IDs[mVBOIndex]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(pIndices.Length * sizeof(float)), pIndices, BufferUsageHint.StaticDraw);
            mVBOIndex++;

            // Make sure data is buffered correctly
            int size;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
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
            // Issue with stride, models dont have tex coords, thats causing issues with the models
            EnableVertexAttributes(pPositionLocation, pNormalLocation, pTextureLocation);
            
            // Unbind buffer for cleanup purposes
            GL.BindVertexArray(0);
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
        /// <param name="pVAO_IDs">The VAOs to clear</param>
        /// <param name="pVBO_IDs">The VBOs to clear</param>
        public void DeleteBuffers(ref int[] pVAO_IDs, ref int[] pVBO_IDs)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.DeleteBuffers(mVBOIndex, pVBO_IDs);
            GL.DeleteVertexArrays(mVAOIndex, pVAO_IDs);
        }
	}
}

