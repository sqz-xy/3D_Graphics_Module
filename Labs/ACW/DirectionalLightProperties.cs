using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Labs.ACW
{
    struct DirectionalLightProperties
    {
        /// <summary>
        /// Direction of the light
        /// </summary>
        public Vector4 Direction { get; set; }

        /// <summary>
        /// Ambient colour variable
        /// </summary>
        public Vector3 AmbientLight { get; set; }

        /// <summary>
        /// Diffuse colour variable
        /// </summary>
        public Vector3 DiffuseLight { get; set; }

        /// <summary>
        /// Specular colour variable
        /// </summary>
        public Vector3 SpecularLight { get; set; }

    }
}
