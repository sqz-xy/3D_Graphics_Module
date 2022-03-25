using Labs.Utility;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

namespace Labs.ACW
{
    class TextureHandler
    {
        private int mTextureIndex;
        private readonly int[] mTexture_IDs;

        private TextureUnit mCurrentTextureUnit;
        private readonly List<string> mTextureUnitsAsString;

        // FBOs not fully implemented
        private readonly int[] mFBO_IDs;

        public TextureHandler(int pTextureCount)
        {
            mTexture_IDs = new int[pTextureCount];
            mTextureIndex = 0;
            mCurrentTextureUnit = TextureUnit.Texture0;
            mTextureUnitsAsString = TextureUnitsToString();

            mFBO_IDs = new int[pTextureCount];

            GL.GenFramebuffers(pTextureCount, mFBO_IDs);
        }

        /// <summary>
        /// Buffers and Binds texture data
        /// </summary>
        /// <param name="pFilePath">The path of the texture to bind</param>
        public int BindTextureData(string pFilePath)
        {
            var filepath = @pFilePath;
            if (System.IO.File.Exists(filepath))
            {            
                var TextureBitmap = new Bitmap(filepath);
                var TextureData = TextureBitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, TextureBitmap.Width,
                TextureBitmap.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppRgb);

                GL.ActiveTexture(mCurrentTextureUnit);
                GL.GenTextures(1, out mTexture_IDs[mTextureIndex]);
                GL.BindTexture(TextureTarget.Texture2D, mTexture_IDs[mTextureIndex]);
  
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

                mTextureIndex++;
                IncrementTextureUnit();

                return mTextureIndex - 1;
            }
            else
            {
                throw new Exception("Could not find file " + filepath);
            }
        }

        /// <summary>
        /// Increments the current texture unit to the next one
        /// </summary>
        private void IncrementTextureUnit()
        {
            var nextUnitString = mTextureUnitsAsString[mTextureIndex];
            Enum.TryParse<TextureUnit>(nextUnitString, out var nextUnit);
            mCurrentTextureUnit = nextUnit;
        }

        /// <summary>
        /// Creates a list of strings pertaining to textureUnit enum values
        /// </summary>
        /// <returns>A list of strings</returns>
        private List<string> TextureUnitsToString()
        {
            var textureUnits = new List<string>();
            foreach (TextureUnit textureUnit in Enum.GetValues(typeof(TextureUnit)))
            {
                textureUnits.Add(textureUnit.ToString());
            }
            return textureUnits;
        }

        /// <summary>
        /// Deletes the textures
        /// </summary>
        public void DeleteTextures()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            foreach (var texture in mTexture_IDs)
            {
                GL.DeleteTexture(texture);
            }
        }
    }
}
