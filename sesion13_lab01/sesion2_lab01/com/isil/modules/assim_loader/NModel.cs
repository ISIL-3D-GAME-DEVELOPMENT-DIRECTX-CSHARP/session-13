using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sesion2_Lab01;

using SharpDX;
using Sesion2_Lab01.com.isil.shader.d3d;

namespace CrossXDK.com.digitalkancer.modules.moderlLoaders.assimp {
    public class NModel {

        private List<NModelMesh> mModelMeshes;

        private Shader3DProgram mShader;

        public NModel() {
            mModelMeshes = new List<NModelMesh>();
        }

        internal void AddMesh(ref NModelMesh modelMesh) {
            mModelMeshes.Add(modelMesh);
        }

        public void SetShader(Shader3DProgram shader) {
            mShader = shader;
        }

        public void Draw(Matrix transformation, int dt) {
            for (int i = 0; i < mModelMeshes.Count; i++) {
                NModelMesh mesh = mModelMeshes[i];

                mShader.Update(mesh.IndexBuffer, mesh.VertexBufferBinding);
                mShader.Draw(mesh.IndicesCount, transformation, mesh.Texture2D, mesh.PrimitiveTopology);
            }
        }
    }
}
