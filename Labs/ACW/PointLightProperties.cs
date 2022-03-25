using OpenTK;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labs.ACW
{
    struct PointLightProperties
    {
        /// <summary>
        /// The light positions
        /// </summary>
        public Vector4[] LightPositions { get; set; }

        /// <summary>
        /// The light colours
        /// </summary>
        public Vector3[] LightColours { get; set; }

        /// <summary>
        /// Reflectivity of ambient light
        /// </summary>
        public Vector3 AmbientReflectivity { get; set; }

        /// <summary>
        /// Reflectivity of diffuse light
        /// </summary>
        public Vector3 DiffuseReflectivity { get; set; }

        /// <summary>
        /// Reflectivity of specular light
        /// </summary>
        public Vector3 SpecularReflectivity { get; set; }

        /// <summary>
        /// The shininess of all the lighting
        /// </summary>
        public float Shininess { get; set; }

        /// <summary>
        /// The number of lights
        /// </summary>
        public int LightCount { get; set; }

        /// <summary>
        /// Struct must be initialized with a count pertaining to the number of lights and colours
        /// </summary>
        /// <param name="pLightCount">The number of desired lights</param>
        public PointLightProperties(int pLightCount)
        {
            LightCount = pLightCount;
            LightPositions = new Vector4[pLightCount];
            LightColours = new Vector3[pLightCount];
            AmbientReflectivity = new Vector3();
            DiffuseReflectivity = new Vector3();
            SpecularReflectivity = new Vector3();
            Shininess = new float();
        }
    } 
}
