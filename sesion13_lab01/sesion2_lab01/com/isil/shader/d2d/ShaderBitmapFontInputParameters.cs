using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using Sesion2_Lab01.com.isil.data_type;

namespace Sesion2_Lab01.com.isil.shader.d2d {
    public struct ShaderBitmapFontInputParameters {
        public static ShaderBitmapFontInputParameters EMPTY = new ShaderBitmapFontInputParameters();

        public NColor textColor;
        public Matrix transformation;
    }
}
