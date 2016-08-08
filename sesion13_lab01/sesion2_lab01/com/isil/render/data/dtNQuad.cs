using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sesion2_Lab01.com.isil.render.data {
    public struct dtNQuad {
        public static dtNQuad EMPTY = new dtNQuad();

        public bool created;
        public float initX;
        public float initY;

        public NVertexPositionTexture VPTTopLeft;
        public NVertexPositionTexture VPTTopRight;
        public NVertexPositionTexture VPTBottomLeft;
        public NVertexPositionTexture VPTBottomRight;
    }
}
