using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Assimp;

using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

using D3DBuffer = SharpDX.Direct3D11.Buffer;

using Sesion2_Lab01;
using Sesion2_Lab01.com.isil.content;

namespace CrossXDK.com.digitalkancer.modules.moderlLoaders.assimp {

    public class NModelMesh {

        private float[] mVertices;
        private uint[] mIndices;
        private Node mNode;
        private Mesh mMesh;
        private Material mMaterial;
        private NTexture2D mTexture2D;
        
        private D3DBuffer mIndexBuffer;
        private D3DBuffer mVertexBuffer;
        private VertexBufferBinding mVertexBufferBinding;

        private PrimitiveTopology mPrimitiveTopology;

        private bool mHasColors;
        private bool mHasNormals;
        private bool mHasTangents;
        private bool mHasBitangents;
        private bool mHasTexCoords;

        public bool HasColors       { get { return mHasColors; } }
        public bool HasNormals      { get { return mHasNormals; } }
        public bool HasTangents     { get { return mHasTangents; } }
        public bool HasBitangents   { get { return mHasBitangents; } }
        public bool HasTexCoords    { get { return mHasTexCoords; } }

        public float[] Vertices     { get { return mVertices; } }
        public uint[] Indices       { get { return mIndices; } }
        public int IndicesCount     { get { return mIndices.Length; } }

        public Node Node            { get { return mNode; } }
        public Mesh Mesh            { get { return mMesh; } }
        public Material Material    { get { return mMaterial; } }
        public NTexture2D Texture2D { get { return mTexture2D; } }

        public PrimitiveTopology PrimitiveTopology      { get { return mPrimitiveTopology; } }
        public D3DBuffer IndexBuffer                    { get { return mIndexBuffer; } }
        public D3DBuffer VertexBuffer                   { get { return mVertexBuffer; } }
        public VertexBufferBinding VertexBufferBinding  { get { return mVertexBufferBinding; } }
        
        public NModelMesh() {
            mHasColors = false;
            mHasNormals = false;
            mHasTangents = false;
            mHasBitangents = false;
            mHasTexCoords = false;
        }

        public void AddTextureDiffuse(string name, string path) {
            // here load the textures

            mTexture2D = new NTexture2D(NativeApplication.instance.Device);
            mTexture2D.Load(path);

            //mTexture2D = new NTexture2D(WrapperReference.graphicDevice, name);
            //mTexture2D.PreservedPixels = false;
            //mTexture2D.Load(path.Substring(0, path.Length - 4), path.Substring(path.Length - 4, 4));
        }

        internal void SetVertices(ref float[] vertices, int stride) {
            mVertices = vertices;

            if (mVertexBuffer != null) {
                D3DBuffer.Dispose<D3DBuffer>(ref mVertexBuffer);
                mVertexBuffer = null;
            }

            mVertexBuffer = D3DBuffer.Create<float>(NativeApplication.instance.Device,
                BindFlags.VertexBuffer, mVertices);

            mVertexBufferBinding.Offset = 0;
            mVertexBufferBinding.Stride = stride;
            mVertexBufferBinding.Buffer = mVertexBuffer;
        }

        internal void SetIndices(ref uint[] indices) {
            for (int i = 0; i < indices.Length; i += 3) {
                uint index0 = indices[i + 2];
                uint index1 = indices[i + 1];
                uint index2 = indices[i];

                indices[i] = index0;
                indices[i + 1] = index1;
                indices[i + 2] = index2;
            }

            mIndices = indices;

            if (mIndexBuffer != null) {
                D3DBuffer.Dispose<D3DBuffer>(ref mIndexBuffer);
                mIndexBuffer = null;
            }

            mIndexBuffer = D3DBuffer.Create<uint>(NativeApplication.instance.Device,
                BindFlags.IndexBuffer, mIndices);
        }

        internal void SetNode(Node node) {
            mNode = node;
        }

        internal void SetMesh(Mesh mesh) {
            mMesh = mesh;
            
            switch (mMesh.PrimitiveType) {
            case PrimitiveType.Line:        mPrimitiveTopology = PrimitiveTopology.LineList; break;
            case PrimitiveType.Point:       mPrimitiveTopology = PrimitiveTopology.PointList; break;
            case PrimitiveType.Polygon:     mPrimitiveTopology = PrimitiveTopology.TriangleStrip; break;
            case PrimitiveType.Triangle:    mPrimitiveTopology = PrimitiveTopology.TriangleList; break;
            }
        }

        internal void SetMaterial(Material material) {
            mMaterial = material;
        }

        internal void SetProperties(bool hasColors, bool hasNormals, bool hasTangents,
            bool hasBiTangents, bool hasTextCoords) {

            mHasColors = hasColors;
            mHasNormals = hasNormals;
            mHasTangents = hasTangents;
            mHasBitangents = hasBiTangents;
            mHasTexCoords = hasTextCoords;
        }

        public static void CreateVertexAndIndices(Mesh mesh, ref Matrix invTranspose,
            ref Matrix transform, ref NModelMesh modelMesh) {
            //get pointers to vertex data
            Vector3D[] positions = mesh.Vertices;
            Vector3D[] texCoords = mesh.GetTextureCoords(0);
            Vector3D[] normals = mesh.Normals;
            Vector3D[] tangents = mesh.Tangents;
            Vector3D[] biTangents = mesh.BiTangents;
            Color4D[] colours = mesh.GetVertexColors(0);

            //determine the elements in the vertex
            bool hasColors = mesh.HasVertexColors(0);
            bool hasNormals = mesh.HasNormals;
            bool hasTangents = mesh.Tangents != null;
            bool hasBitangents = mesh.BiTangents != null;
            bool hasTexCoords = mesh.HasTextureCoords(0);

            int vertexArraySize = 0;
            int vertexCount = 0;
            int strideComponents = 0;

            if (positions != null)  { vertexArraySize += positions.Length * 3; strideComponents += 3; }
            if (hasColors)          { vertexArraySize += colours.Length * 4; strideComponents += 4; }
            else {                      vertexArraySize += positions.Length * 4; strideComponents += 4; }
            if (hasNormals)         { vertexArraySize += normals.Length * 3; strideComponents += 3; }
            //if (hasTangents)        { vertexArraySize += tangents.Length * 3; strideComponents += 3; }
            //if (hasBitangents)      { vertexArraySize += biTangents.Length * 3; strideComponents += 3; }
            if (hasTexCoords)       { vertexArraySize += texCoords.Length * 2; strideComponents += 2; }  //{ vertexArraySize += texCoords.Length * 3; strideComponents += 3; }

            // now let's create the vertices array
            float[] vertices = new float[vertexArraySize];
            
            for (int i = 0; i < mesh.VertexCount; i++) {
                //add position, after transforming it with accumulated node transform
                Vector4 out_position = Vector4.Zero;
                Vector3 pos = NModelMesh.FromVector(positions[i]);
                Vector3.Transform(ref pos, ref transform, out out_position);

                vertices[vertexCount]     = out_position.X;
                vertices[vertexCount + 1] = out_position.Y;
                vertices[vertexCount + 2] = out_position.Z;

                vertexCount += 3;

                if (hasColors) {
                    Color4D color_out = colours[i];

                    vertices[vertexCount] = color_out.R;
                    vertices[vertexCount + 1] = color_out.G;
                    vertices[vertexCount + 2] = color_out.B;
                    vertices[vertexCount + 3] = color_out.A;

                    vertexCount += 4;
                }
                else {
                    vertices[vertexCount] = 1f;
                    vertices[vertexCount + 1] = 1f;
                    vertices[vertexCount + 2] = 1f;
                    vertices[vertexCount + 3] = 1f;

                    vertexCount += 4;
                }

                if (hasNormals) {
                    Vector4 out_normals = Vector4.Zero;
                    Vector3 normal = NModelMesh.FromVector(normals[i]);
                    Vector3.Transform(ref normal, ref invTranspose, out out_normals);

                    vertices[vertexCount]     = out_normals.X;
                    vertices[vertexCount + 1] = out_normals.Y;
                    vertices[vertexCount + 2] = out_normals.Z;

                    vertexCount += 3;
                }

                /*if (hasTangents) {
                    Vector4 out_tangents = Vector4.Zero;
                    Vector3 tangent = NModelMesh.FromVector(tangents[i]);
                    Vector3.Transform(ref tangent, ref invTranspose, out out_tangents);

                    vertices[vertexCount]     = out_tangents.X;
                    vertices[vertexCount + 1] = out_tangents.Y;
                    vertices[vertexCount + 2] = out_tangents.Z;

                    vertexCount += 3;
                }

                if (hasBitangents) {
                    Vector4 out_bitangents = Vector4.Zero;
                    Vector3 biTangent = NModelMesh.FromVector(biTangents[i]);
                    Vector3.Transform(ref biTangent, ref invTranspose, out out_bitangents);

                    vertices[vertexCount]     = out_bitangents.X;
                    vertices[vertexCount + 1] = out_bitangents.Y;
                    vertices[vertexCount + 2] = out_bitangents.Z;

                    vertexCount += 3;
                }*/

                if (hasTexCoords) {
                    Vector3D textCoord = texCoords[i];

                    vertices[vertexCount] = textCoord.X;
                    vertices[vertexCount + 1] = textCoord.Y;
                    //vertices[vertexCount + 2] = textCoord.Z;

                    vertexCount += 2;
                    //vertexCount += 3;
                }
            }

            //get pointer to indices data
            uint[] indices = mesh.GetIndices();

            // now set the vertex and indices to the model mesh
            modelMesh.SetVertices(ref vertices, sizeof(float) * strideComponents);
            modelMesh.SetIndices(ref indices);
            modelMesh.SetProperties(hasColors, hasNormals, hasTangents,
                hasBitangents, hasTexCoords);
        }

        private static Vector3 FromVector(Vector3D vec) {
            Vector3 v;
            v.X = vec.X;
            v.Y = vec.Y;
            v.Z = vec.Z;
            return v;
        }
    }
}
