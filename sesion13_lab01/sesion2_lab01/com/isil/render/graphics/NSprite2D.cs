using Sesion2_Lab01.com.isil.content;
using Sesion2_Lab01.com.isil.render.camera;
using Sesion2_Lab01.com.isil.shader.d2d;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sesion2_Lab01.com.isil.render.graphics {

    public class NSprite2D {

        // variables para dibujar nuestro primitivo
        private ushort[] mIndices;
        private float[] mVertices;

        private ShaderTextureProgram mShader;
        private NTexture2D mTexture2D;

        public float X {
            get { return mShader.X; }
            set { mShader.X = value; }
        }

        public float Y {
            get { return mShader.Y; }
            set { mShader.Y = value; }
        }

        public NSprite2D (string path, int x, int y) {
            mTexture2D = new NTexture2D(NativeApplication.instance.Device);
            mTexture2D.Load(path);

            // creamos nuestro indices
            mIndices = new ushort[6];
            mIndices[0] = 0;
            mIndices[1] = 1;
            mIndices[2] = 2;
            mIndices[3] = 0;
            mIndices[4] = 2;
            mIndices[5] = 3;

            // creamos nustros vertices
            mVertices = new float[10 * 4];
            // nuestro primer vertice
            mVertices[0] = 0f; mVertices[1] = 0f; mVertices[2] = 0f; mVertices[3] = 1f; // vertex
            mVertices[4] = 1f; mVertices[5] = 1f; mVertices[6] = 1f; mVertices[7] = 1f; // color
            mVertices[8] = 0f; mVertices[9] = 0f; // texture coordinate
            // nuestro segundo vertice
            mVertices[10] = 150f; mVertices[11] = 0f; mVertices[12] = 0f; mVertices[13] = 1f; // vertex
            mVertices[14] = 1f; mVertices[15] = 1f; mVertices[16] = 1f; mVertices[17] = 1f; // color
            mVertices[18] = 1f; mVertices[19] = 0f; // texture coordinate
            // nuestro tercer vertice
            mVertices[20] = 150f; mVertices[21] = 150f; mVertices[22] = 0f; mVertices[23] = 1f; // vertex
            mVertices[24] = 1f; mVertices[25] = 1f; mVertices[26] = 1f; mVertices[27] = 1f; // color
            mVertices[28] = 1f; mVertices[29] = 1f; // texture coordinate
            // nuestro cuarto vertice
            mVertices[30] = 0f; mVertices[31] = 150f; mVertices[32] = 0f; mVertices[33] = 1f; // vertex
            mVertices[34] = 1f; mVertices[35] = 1f; mVertices[36] = 1f; mVertices[37] = 1f; // color
            mVertices[38] = 0f; mVertices[39] = 1f; // texture coordinate
        }

        public void SetShader(ShaderTextureProgram shader) {
            mShader = shader;
        }

        public void Draw(RenderCamera camera, int dt) {
            mShader.Update(mVertices, mIndices);
            mShader.Draw(camera.transformed, mTexture2D);
        }
    }
}
