using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Assimp;
using Assimp.Configs;

using SharpDX;

namespace CrossXDK.com.digitalkancer.modules.moderlLoaders.assimp {
    public class NCrossModelLoader {

        private AssimpImporter mImporter;

        public NCrossModelLoader() {
            mImporter = new AssimpImporter();
            mImporter.SetConfig(new NormalSmoothingAngleConfig(66.0f));
        }

        public NModel Load(string path) {
            string[] splitPath = path.Split(new char[1] { '/' });

            string rootPath = string.Empty;
            for (int i = 0; i < splitPath.Length - 1; i++) { 
                rootPath += splitPath[i] + "/"; 
            }

            //rootPath = rootPath.Substring(0, rootPath.Length - 1);

            //Scene scene = mImporter.ImportFileFromStream(modelData.StreamReader.BaseStream,
            //    PostProcessPreset.TargetRealTimeMaximumQuality, modelData.Format);

            Scene scene = mImporter.ImportFile(path, PostProcessPreset.TargetRealTimeMaximumQuality);

            Matrix transform = Matrix.Identity;

            NModel model = new NModel();

            this.AddVertexData(model, scene, scene.RootNode, ref transform, ref rootPath);

            return model;
        }

        public void AddVertexData(NModel model, Scene scene, Node node, 
            ref Matrix transform, ref string rootPath) {
            
            Matrix previousTransform = transform;
            transform = Matrix.Multiply(previousTransform, NCrossModelLoader.FromMatrix(node.Transform));

            //also calculate inverse transpose matrix for normal/tangent/bitagent transformation
            Matrix invTranspose = transform;
            Matrix.Invert(ref invTranspose, out invTranspose);
            Matrix.Transpose(ref invTranspose, out invTranspose);

            if (node.HasMeshes) {
                foreach (int index in node.MeshIndices) {
                    //get a mesh from the scene
                    Assimp.Mesh mesh = scene.Meshes[index];

                    //create new mesh to add to model
                    NModelMesh modelMesh = new NModelMesh();
                    model.AddMesh(ref modelMesh);

                    //if mesh has a material extract the diffuse texture, if present
                    Material material = scene.Materials[mesh.MaterialIndex];

                    modelMesh.SetNode(node);
                    modelMesh.SetMesh(mesh);
                    modelMesh.SetMaterial(material);

                    if (material != null && material.GetTextureCount(TextureType.Diffuse) > 0) {
                        TextureSlot texture = material.GetTexture(TextureType.Diffuse, 0);
                        //create new texture for mesh
                        

                        modelMesh.AddTextureDiffuse(material.Name, rootPath + texture.FilePath);
                    }

                    NModelMesh.CreateVertexAndIndices(mesh, ref invTranspose, ref transform, ref modelMesh);
                }
            }

            //if node has more children process them as well
            for (int i = 0; i < node.ChildCount; i++) {
                this.AddVertexData(model, scene, node.Children[i], ref transform, ref rootPath);
            }

            transform = previousTransform;
        }

        //some Assimp to SharpDX conversion helpers
        private static Matrix FromMatrix(Matrix4x4 mat) {
            Matrix m = Matrix.Identity;
            m.M11 = mat.A1;
            m.M12 = mat.A2;
            m.M13 = mat.A3;
            m.M14 = mat.A4;
            m.M21 = mat.B1;
            m.M22 = mat.B2;
            m.M23 = mat.B3;
            m.M24 = mat.B4;
            m.M31 = mat.C1;
            m.M32 = mat.C2;
            m.M33 = mat.C3;
            m.M34 = mat.C4;
            m.M41 = mat.D1;
            m.M42 = mat.D2;
            m.M43 = mat.D3;
            m.M44 = mat.D4;
            return m;
        }
    }
}
