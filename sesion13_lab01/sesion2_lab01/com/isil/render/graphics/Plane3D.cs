using SharpDX;
using SharpDX.Direct3D11;
using D3DBuffer = SharpDX.Direct3D11.Buffer;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sesion2_Lab01.com.isil.shader.d3d;
using Sesion2_Lab01.com.isil.render.camera;
using Sesion2_Lab01.com.isil.content;

namespace Sesion2_Lab01.com.isil.render.graphics {
    public class Plane3D {

        private Shader3DProgram mShader;

        protected float[] mVertices;
        protected uint[] mIndices;

        private int mIndicesCount;

        private D3DBuffer mIndexBuffer;
        private D3DBuffer mVertexBuffer;
        private VertexBufferBinding mVertexBufferBinding;

        private NTexture2D mTexture;

        private Vector3 mPosition;

        public Plane3D(NTexture2D texture, float x, float y, float z, float size) {
            mTexture = texture;

            mPosition.X = x;
            mPosition.Y = y;
            mPosition.Z = z;

            mVertices = new float[4 * 12]; // 6 faces, 4 vertex, 3 position and 3 normals

            int vertexOffset = 0;

            Vector3 normal = Vector3.UnitZ;

            // Get two vectors perpendicular to the face normal and to each other.
            Vector3 side1 = Vector3.Zero;
            side1.X = normal.Y;
            side1.Y = normal.Z;
            side1.Z = normal.X;

            Vector3 side2 = Vector3.Zero;
            Vector3.Cross(ref normal, ref side1, out side2);

            Vector3 vertex_1 = (normal - side1 - side2) * size / 2;
            Vector3 vertex_2 = (normal - side1 + side2) * size / 2;
            Vector3 vertex_3 = (normal + side1 + side2) * size / 2;
            Vector3 vertex_4 = (normal + side1 - side2) * size / 2;

            mVertices[vertexOffset + 0] = vertex_1.X;
            mVertices[vertexOffset + 1] = vertex_1.Y;
            mVertices[vertexOffset + 2] = vertex_1.Z;
            mVertices[vertexOffset + 3] = 1f;
            mVertices[vertexOffset + 4] = 1f;
            mVertices[vertexOffset + 5] = 1f;
            mVertices[vertexOffset + 6] = 1f;
            mVertices[vertexOffset + 7] = normal.X;
            mVertices[vertexOffset + 8] = normal.Y;
            mVertices[vertexOffset + 9] = normal.Z;
            mVertices[vertexOffset + 10] = 0f;
            mVertices[vertexOffset + 11] = 0f;

            mVertices[vertexOffset + 12] = vertex_2.X;
            mVertices[vertexOffset + 13] = vertex_2.Y;
            mVertices[vertexOffset + 14] = vertex_2.Z;
            mVertices[vertexOffset + 15] = 1f;
            mVertices[vertexOffset + 16] = 1f;
            mVertices[vertexOffset + 17] = 1f;
            mVertices[vertexOffset + 18] = 1f;
            mVertices[vertexOffset + 19] = normal.X;
            mVertices[vertexOffset + 20] = normal.Y;
            mVertices[vertexOffset + 21] = normal.Z;
            mVertices[vertexOffset + 22] = 0f;
            mVertices[vertexOffset + 23] = 1f;

            mVertices[vertexOffset + 24] = vertex_3.X;
            mVertices[vertexOffset + 25] = vertex_3.Y;
            mVertices[vertexOffset + 26] = vertex_3.Z;
            mVertices[vertexOffset + 27] = 1f;
            mVertices[vertexOffset + 28] = 1f;
            mVertices[vertexOffset + 29] = 1f;
            mVertices[vertexOffset + 30] = 1f;
            mVertices[vertexOffset + 31] = normal.X;
            mVertices[vertexOffset + 32] = normal.Y;
            mVertices[vertexOffset + 33] = normal.Z;
            mVertices[vertexOffset + 34] = 1f;
            mVertices[vertexOffset + 35] = 1f;

            mVertices[vertexOffset + 36] = vertex_4.X;
            mVertices[vertexOffset + 37] = vertex_4.Y;
            mVertices[vertexOffset + 38] = vertex_4.Z;
            mVertices[vertexOffset + 39] = 1f;
            mVertices[vertexOffset + 40] = 1f;
            mVertices[vertexOffset + 41] = 1f;
            mVertices[vertexOffset + 42] = 1f;
            mVertices[vertexOffset + 43] = normal.X;
            mVertices[vertexOffset + 44] = normal.Y;
            mVertices[vertexOffset + 45] = normal.Z;
            mVertices[vertexOffset + 46] = 1f;
            mVertices[vertexOffset + 47] = 0f;

            if (mVertexBuffer != null) {
                D3DBuffer.Dispose<D3DBuffer>(ref mVertexBuffer);
                mVertexBuffer = null;
            }

            mVertexBuffer = D3DBuffer.Create<float>(NativeApplication.instance.Device,
                BindFlags.VertexBuffer, mVertices);

            mVertexBufferBinding.Offset = 0;
            mVertexBufferBinding.Stride = sizeof(float) * 12;
            mVertexBufferBinding.Buffer = mVertexBuffer;

            //creamos los indices para el cubo
            CreateIndices();
        }

        private void CreateIndices() {
            int indexOffset = 0;

            mIndicesCount = 1 * 6; // 1 faces, 6 quads
            mIndices = new uint[mIndicesCount];

            for (int i = 0; i < mIndicesCount; i += 6) {
                mIndices[i] = (uint)indexOffset;
                mIndices[i + 1] = (uint)(indexOffset + 1);
                mIndices[i + 2] = (uint)(indexOffset + 2);
                mIndices[i + 3] = (uint)indexOffset;
                mIndices[i + 4] = (uint)(indexOffset + 2);
                mIndices[i + 5] = (uint)(indexOffset + 3);

                indexOffset += 4;
            }

            if (mIndexBuffer != null) {
                D3DBuffer.Dispose<D3DBuffer>(ref mIndexBuffer);
                mIndexBuffer = null;
            }

            mIndexBuffer = D3DBuffer.Create<uint>(NativeApplication.instance.Device,
                BindFlags.IndexBuffer, mIndices);
        }

        public void SetShader(Shader3DProgram shader) {
            mShader = shader;
        }

        public void Draw(RenderCamera camera, int dt) {
            Matrix billboard = Matrix.Billboard(mPosition, camera.Position,
                camera.CameraUpVector, camera.CameraForwardVector);

            Matrix newTransformation = billboard * camera.transformed;

            mShader.Update(mIndexBuffer, mVertexBufferBinding);
            mShader.Draw(mIndicesCount, newTransformation, mTexture,
                SharpDX.Direct3D.PrimitiveTopology.TriangleList);
        }
    }
}
