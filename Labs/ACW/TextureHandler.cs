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
        private TextureUnit mCurrentTextureUnit;
        private List<string> mTextureUnitsAsString;

        public TextureHandler()
        {
            mTextureIndex = 0;
            mCurrentTextureUnit = TextureUnit.Texture0;
            mTextureUnitsAsString = TextureUnitsToString();
        }

        /// <summary>
        /// Buffers and Binds texture data
        /// </summary>
        /// <param name="pFilePath">The path of the texture to bind</param>
        /// <param name="pTexture_IDs">The array of texture IDs</param>
        public void BindTextureData(string pFilePath, ref int[] pTexture_IDs)
        {
            string filepath = @pFilePath;
            if (System.IO.File.Exists(filepath))
            {
                Bitmap TextureBitmap = new Bitmap(filepath);
                BitmapData TextureData = TextureBitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, TextureBitmap.Width,
                TextureBitmap.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppRgb);

                GL.ActiveTexture(mCurrentTextureUnit);
                GL.GenTextures(1, out pTexture_IDs[mTextureIndex]);
                GL.BindTexture(TextureTarget.Texture2D, pTexture_IDs[mTextureIndex]);
                mTextureIndex++;
                IncrementTextureUnit();

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
        }

        /// <summary>
        /// Increments the current texture unit to the next one
        /// </summary>
        private void IncrementTextureUnit()
        {
            string nextUnitString = mTextureUnitsAsString[mTextureIndex];
            TextureUnit nextUnit;
            Enum.TryParse<TextureUnit>(nextUnitString, out nextUnit);
            mCurrentTextureUnit = nextUnit;
        }

        /// <summary>
        /// Creates a list of strings pertaining to textureUnit enum values
        /// </summary>
        /// <returns>A list of strings</returns>
        private List<string> TextureUnitsToString()
        {
            List<string> textureUnits = new List<string>();
            foreach (TextureUnit textureUnit in Enum.GetValues(typeof(TextureUnit)))
            {
                textureUnits.Add(textureUnit.ToString());
            }
            return textureUnits;
        }

        /// <summary>
        /// Deletes the textures
        /// </summary>
        /// <param name="pTexture_IDs">The array of texture ids</param>
        public void DeleteTextures(ref int[] pTexture_IDs)
        {
            foreach (int Texture in pTexture_IDs)
            {
                GL.DeleteTexture(Texture);
            }
        }

    }
}
