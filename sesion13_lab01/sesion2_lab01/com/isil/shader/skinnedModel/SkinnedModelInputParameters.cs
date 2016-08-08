using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

using Assimp;
using System.Runtime.InteropServices;

namespace Sesion2_Lab01.com.isil.shader.d3d {
    public struct SkinnedModelInputParameters {
        public static SkinnedModelInputParameters EMPTY = new SkinnedModelInputParameters();
        public Matrix gWorld;
        public Matrix gWorldInvTranspose;
        public Matrix gWorldViewProj;
        public Matrix gTexTransform;
    }
}
