using OpenTK;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labs.ACW
{
    struct LightingProperties
    {
        public Vector4[] LightPositions { get; set; }

        public Vector3[] LightColours { get; set; }

        public Vector3 AmbientReflectivity { get; set; }

        public Vector3 DiffuseReflectivity { get; set; }

        public Vector3 SpecularReflectivity { get; set; }

        public float Shininess { get; set; }

        public LightingProperties(int pLightCount)
        {
            LightPositions = new Vector4[pLightCount];
            LightColours = new Vector3[pLightCount];
            AmbientReflectivity = new Vector3();
            DiffuseReflectivity = new Vector3();
            SpecularReflectivity = new Vector3();
            Shininess = new float();
        }
    } 
}
