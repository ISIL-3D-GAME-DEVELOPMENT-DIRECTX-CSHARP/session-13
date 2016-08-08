using Sesion2_Lab01.com.isil.content;
using Sesion2_Lab01.com.isil.data_type;
using Sesion2_Lab01.com.isil.render.camera;
using Sesion2_Lab01.com.isil.shader.d2d;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sesion2_Lab01.com.isil.render.batcher {

    struct dtVertexData {
        public float x;
        public float y;
        public float z;
        public float r;
        public float g;
        public float b;
        public float a;
        public float tx;
        public float ty;
    }

    public class NSpriteBatch {

        private ushort[] mIndices;
        private float[] mVertices;

        private ShaderTextureProgram mShader;
        private NTexture2D mTexture;

        private List<dtVertexData> mRenderSprites;

        public NSpriteBatch(NTexture2D texture) {
            mTexture = texture;

            mRenderSprites = new List<dtVertexData>();
        }

        public void SetShader(ShaderTextureProgram shader) {
            mShader = shader;
        }

        private void CreateIndices(int numSprites) {
            mIndices = new ushort[numSprites * 6];

            int offset = 0;
            ushort indexCount = 0;

            for (int i = 0; i < numSprites; i++) {
                mIndices[offset]     = indexCount;
                mIndices[offset + 1] = (ushort)(indexCount + 1);
                mIndices[offset + 2] = (ushort)(indexCount + 2);
                mIndices[offset + 3] = indexCount;
                mIndices[offset + 4] = (ushort)(indexCount + 2);
                mIndices[offset + 5] = (ushort)(indexCount + 3);

                indexCount += 4;
                offset += 6;
			}
        }

        public void AddSprite(int x, int y, int z, int width, int height, NColor color) {
            float _r = color.R / 255f;
            float _g = color.G / 255f;
            float _b = color.B / 255f;
            float _a = color.A / 255f;

            dtVertexData dataSprite_1;
            dataSprite_1.x = x;
            dataSprite_1.y = y;
            dataSprite_1.z = z;
            dataSprite_1.r = _r;
            dataSprite_1.g = _g;
            dataSprite_1.b = _b;
            dataSprite_1.a = _a;
            dataSprite_1.tx = 0;
            dataSprite_1.ty = 0;

            dtVertexData dataSprite_2;
            dataSprite_2.x = x + width;
            dataSprite_2.y = y;
            dataSprite_2.z = z;
            dataSprite_2.r = _r;
            dataSprite_2.g = _g;
            dataSprite_2.b = _b;
            dataSprite_2.a = _a;
            dataSprite_2.tx = 1f;
            dataSprite_2.ty = 0;

            dtVertexData dataSprite_3;
            dataSprite_3.x = x + width;
            dataSprite_3.y = y + height;
            dataSprite_3.z = z;
            dataSprite_3.r = _r;
            dataSprite_3.g = _g;
            dataSprite_3.b = _b;
            dataSprite_3.a = _a;
            dataSprite_3.tx = 1f;
            dataSprite_3.ty = 1f;

            dtVertexData dataSprite_4;
            dataSprite_4.x = x;
            dataSprite_4.y = y + height;
            dataSprite_4.z = z;
            dataSprite_4.r = _r;
            dataSprite_4.g = _g;
            dataSprite_4.b = _b;
            dataSprite_4.a = _a;
            dataSprite_4.tx = 0;
            dataSprite_4.ty = 1f;

            mRenderSprites.Add(dataSprite_1);
            mRenderSprites.Add(dataSprite_2);
            mRenderSprites.Add(dataSprite_3);
            mRenderSprites.Add(dataSprite_4);
        }

        public void Update(int dt) {
            if (mVertices == null && mRenderSprites.Count > 3) {
                mVertices = new float[mRenderSprites.Count * 10];

                // creamos nuestros indices
                this.CreateIndices(mRenderSprites.Count / 4);
            }
            else {
                if (mVertices != null) {
                    if (mVertices.Length != mRenderSprites.Count * 10) {
                        mVertices = null;
                        mVertices = new float[mRenderSprites.Count * 10];

                        // creamos nuestros indices
                        this.CreateIndices(mRenderSprites.Count / 4);
                    }
                }
            }

            int vertexCount = 0;

            for (int i = 0; i < mRenderSprites.Count; i++) {
                dtVertexData vd = mRenderSprites[i];

                mVertices[vertexCount + 0] = vd.x;
                mVertices[vertexCount + 1] = vd.y;
                mVertices[vertexCount + 2] = vd.z;
                mVertices[vertexCount + 3] = 1f;
                mVertices[vertexCount + 4] = vd.r;
                mVertices[vertexCount + 5] = vd.g;
                mVertices[vertexCount + 6] = vd.b;
                mVertices[vertexCount + 7] = vd.a;
                mVertices[vertexCount + 8] = vd.tx;
                mVertices[vertexCount + 9] = vd.ty;

                vertexCount += 10;
            }

            // limpiar el buffer de la lista de dtVertexData
            mRenderSprites.Clear();
        }

        public void Draw(RenderCamera camera, int dt) {
            if (mVertices != null && mIndices != null) {
                mShader.Update(mVertices, mIndices);
                mShader.Draw(camera.transformed, mTexture);
            }
        }
    }
}
