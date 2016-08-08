

using System.Collections.Generic;
using System.Runtime.InteropServices;

using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D11;
using Device = SharpDX.Direct3D11.Device;
using Sesion2_Lab01.com.isil.shader.skinnedModel;
using Sesion2_Lab01.com.isil.shader.d3d;
using Sesion2_Lab01;
using SharpDX.Direct3D;
using Core.Vertex;
using Sesion2_Lab01.com.isil.content;

namespace Core.Model {
    public class MeshGeometry {
        public class Subset {
            public int VertexStart;
            public int VertexCount;
            public int FaceStart;
            public int FaceCount;
        }

        private Buffer mIndexBuffer;

        private Buffer mVertexBuffer;
        private BufferDescription mVertexBDesc;
        private VertexBufferBinding mBoneVertexBufferBinding;

        private int mVertexStride;

        private List<Subset> mSubsetTable;
        private PosNormalTexTanSkinned[] mVertex;

        public void SetVertices(Device device, List<PosNormalTexTanSkinned> vertices) {
            //Util.ReleaseCom(ref _vb);
            mVertexStride = Marshal.SizeOf(typeof(PosNormalTexTanSkinned));

            BufferDescription vbd = new BufferDescription(
                mVertexStride * vertices.Count,
                ResourceUsage.Immutable,
                BindFlags.VertexBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0
            );

            mVertex = (PosNormalTexTanSkinned[])vertices.ToArray();

            // definimos la descripcion de nuestro Buffer de vertices
            mVertexBDesc.SizeInBytes = mVertexStride * mVertex.Length;
            mVertexBDesc.Usage = ResourceUsage.Immutable;
            mVertexBDesc.BindFlags = BindFlags.VertexBuffer;
            mVertexBDesc.CpuAccessFlags = CpuAccessFlags.None;
            mVertexBDesc.OptionFlags = ResourceOptionFlags.None;
            mVertexBDesc.StructureByteStride = 0;
        }

        public void SetIndices(Device device, List<short> indices) {
            BufferDescription ibd = new BufferDescription(
                sizeof(short) * indices.Count,
                ResourceUsage.Immutable,
                BindFlags.IndexBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0
            );

            short[] arrayIndices = indices.ToArray();
            mIndexBuffer = new Buffer(device, DataStream.Create<short>(arrayIndices, false, false), ibd);
        }

        public void SetSubsetTable(List<Subset> subsets) {
            mSubsetTable = subsets;
        }

        private void ProcessBones(ref Matrix[] boneTransforms) {
            PosNormalTexTanSkinned[] newVertex = new PosNormalTexTanSkinned[mVertex.Length];

            // aqui procesamos los Huesos (Bones) individualmente con los vertices para poder
            // calcular las animaciones :D
            for (int i = 0; i < newVertex.Length; i++) {
                PosNormalTexTanSkinned pnt = mVertex[i];

                float weight0 = pnt.Weight;
                float weight1 = 1.0f - weight0;

                Vector4 position = new Vector4(pnt.Pos, 1f);

                Matrix transBone_1 = boneTransforms[(int)pnt.BoneIndices.B0];
                Matrix transBone_2 = boneTransforms[(int)pnt.BoneIndices.B1];

                Vector4 newPosition = weight0 * Vector4.Transform(position, transBone_1);
                newPosition += weight1 * Vector4.Transform(position, transBone_2);
                newPosition.W = 1f;

                pnt.Pos = new Vector3(newPosition.X, newPosition.Y, newPosition.Z);

                newVertex[i] = pnt;
            }

            // ahora creamos los buffers para mandar al Shader
            if (mVertexBuffer != null) {
                mVertexBuffer.Dispose();
                mVertexBuffer = null;
            }
            
            mVertexBuffer = new Buffer(NativeApplication.instance.Device,
                DataStream.Create<PosNormalTexTanSkinned>(newVertex,
                false, false), mVertexBDesc);

            mBoneVertexBufferBinding.Buffer = mVertexBuffer;
            mBoneVertexBufferBinding.Stride = mVertexStride;
            mBoneVertexBufferBinding.Offset = 0;
        }

        public void Draw(SkinnedModelProgram shaderProgram, SkinnedModelInputParameters input,
            Matrix[] boneTransforms, NTexture2D diffuseSRV, NTexture2D normalSRV, 
            DeviceContext dc, int subsetId) {

            // procesamos los vertices segun las nuevas transformaciones de los huesos
            this.ProcessBones(ref boneTransforms);

            shaderProgram.Update(mIndexBuffer, mBoneVertexBufferBinding);

            shaderProgram.Draw(mSubsetTable[subsetId].FaceCount * 3,
                mSubsetTable[subsetId].FaceStart * 3, input, 
                diffuseSRV, PrimitiveTopology.TriangleList); 
        }
    }
}
