using System.Collections.Generic;
using System.IO;
using System.Linq;

using Assimp;

using Core.Vertex;

using SharpDX.Direct3D11;
using SharpDX;
using Sesion2_Lab01.com.isil.utils;
using Sesion2_Lab01;
using Sesion2_Lab01.com.isil.content;

namespace Core.Model {
    public class SkinnedModel {
        private MeshGeometry _modelMesh;
        public MeshGeometry ModelMesh { get { return _modelMesh; } }

        private readonly List<MeshGeometry.Subset> _subsets;
        public int SubsetCount { get { return _subsets.Count; } }

        private readonly List<PosNormalTexTanSkinned> _vertices;
        private readonly List<short> _indices;

        protected internal SceneAnimator Animator { get; private set; }

        public List<Material> Materials { get; private set; }
        public List<NTexture2D> DiffuseMapSRV { get; private set; }
        public List<NTexture2D> NormalMapSRV { get; private set; }

        public BoundingBox BoundingBox { get; private set; }
        private Vector3 _min;
        private Vector3 _max;

        private bool _disposed;

        public SkinnedModel(Device device, string filename, string texturePath, bool flipTexY = false) {
            // initialize collections
            _subsets = new List<MeshGeometry.Subset>();
            _vertices = new List<PosNormalTexTanSkinned>();
            _indices = new List<short>();
            DiffuseMapSRV = new List<NTexture2D>();
            NormalMapSRV = new List<NTexture2D>();
            Materials = new List<Material>();
            
            var importer = new AssimpImporter();
        #if DEBUG
            importer.AttachLogStream(new ConsoleLogStream());
            importer.VerboseLoggingEnabled = true;
        #endif
            var model = importer.ImportFile(filename, PostProcessSteps.GenerateSmoothNormals | PostProcessSteps.CalculateTangentSpace );
    
            // Load animation data
            Animator = new SceneAnimator();
            Animator.Init(model);
            
            // create our vertex-to-boneweights lookup
            var vertToBoneWeight = new Dictionary<uint, List<VertexWeight>>();
            // create bounding box extents
            _min = new Vector3(float.MaxValue);
            _max = new Vector3(float.MinValue);

            foreach (var mesh in model.Meshes) {
                ExtractBoneWeightsFromMesh(mesh, vertToBoneWeight);
                var subset = new MeshGeometry.Subset {
                    VertexCount = mesh.VertexCount,
                    VertexStart = _vertices.Count,
                    FaceStart = _indices.Count / 3,
                    FaceCount = mesh.FaceCount
                };
                _subsets.Add(subset);

                IEnumerable<PosNormalTexTanSkinned> verts = ExtractVertices(mesh, vertToBoneWeight, flipTexY);
                _vertices.AddRange(verts);
                // extract indices and shift them to the proper offset into the combined vertex buffer
                List<short> indices = mesh.GetIndices().Select(i => (short)(i + (uint)subset.VertexStart)).ToList();
                _indices.AddRange(indices);

                // extract materials
                Material mat = model.Materials[mesh.MaterialIndex];
                //var material = mat.ToMaterial();
                Materials.Add(mat);

                // extract material textures
                var diffusePath = mat.GetTexture(TextureType.Diffuse, 0).FilePath;
                if (!string.IsNullOrEmpty(diffusePath)) {
                    NTexture2D texture1 = new NTexture2D(NativeApplication.instance.Device);
                    texture1.ChangeSamplerStateAddress(TextureAddressMode.Wrap);
                    texture1.Load(texturePath + diffusePath);

                    DiffuseMapSRV.Add(texture1);
                }
                var normalPath = mat.GetTexture(TextureType.Normals, 0).FilePath;
                if (!string.IsNullOrEmpty(normalPath)) {
                    NTexture2D texture2 = new NTexture2D(NativeApplication.instance.Device);
                    texture2.Load(texturePath + normalPath);

                    NormalMapSRV.Add(texture2);
                } else {
                    // for models created without a normal map baked, we'll check for a texture with the same 
                    // filename as the diffure texture, and _nmap suffixed
                    // this lets us add our own normal maps easily
                    var normalExt = Path.GetExtension(diffusePath);
                    normalPath = Path.GetFileNameWithoutExtension(diffusePath) + "_nmap" + normalExt;
                    if (File.Exists(Path.Combine(texturePath, normalPath))) {
                        NTexture2D texture3 = new NTexture2D(NativeApplication.instance.Device);
                        texture3.Load(texturePath + normalPath);

                        NormalMapSRV.Add(texture3);
                    }
                }
            }
            
            BoundingBox = new BoundingBox(_min, _max);

            _modelMesh = new MeshGeometry();
            _modelMesh.SetSubsetTable(_subsets);
            _modelMesh.SetVertices(device, _vertices);
            _modelMesh.SetIndices(device, _indices);
        }

        private IEnumerable<PosNormalTexTanSkinned> ExtractVertices(Mesh mesh, IReadOnlyDictionary<uint, List<VertexWeight>> vertToBoneWeights, bool flipTexY) {
            var verts = new List<PosNormalTexTanSkinned>();
            for (var i = 0; i < mesh.VertexCount; i++) {
                Vector3 pos = mesh.HasVertices ? NCommon.ConvertVector3DToVector3(mesh.Vertices[i]) : new Vector3();
                _min = Vector3.Min(_min, pos);
                _max = Vector3.Max(_max, pos);
                
                var norm = mesh.HasNormals ? mesh.Normals[i] : new Vector3D();

                var tan = mesh.HasTangentBasis ? mesh.Tangents[i] : new Vector3D();
                var texC = new Vector3D();
                if (mesh.HasTextureCoords(0)) {
                    var coord = mesh.GetTextureCoords(0)[i];
                    if (flipTexY) {
                        float tcx = coord.X;
                        float tcy = coord.Y;

                        coord.X = tcx;
                        coord.Y = 1f - tcy;
                    }
                    texC = coord;
                }


                var weights = vertToBoneWeights[(uint) i].Select(w => w.Weight).ToArray();
                var boneIndices = vertToBoneWeights[(uint) i].Select(w => (byte) w.VertexID).ToArray();

                PosNormalTexTanSkinned v = new PosNormalTexTanSkinned(pos,
                    NCommon.ConvertVector3DToVector3(norm),
                    NCommon.ConvertVector3DToVector2(texC),
                    NCommon.ConvertVector3DToVector3(tan), 
                    weights.First(),
                    boneIndices);
                verts.Add(v);
            }
            return verts;
        }

        private void ExtractBoneWeightsFromMesh(Mesh mesh, IDictionary<uint, List<VertexWeight>> vertToBoneWeight) {
            foreach (var bone in mesh.Bones) {
                var boneIndex = Animator.GetBoneIndex(bone.Name);
                // bone weights are recorded per bone in assimp, with each bone containing a list of the vertices influenced by it
                // we really want the reverse mapping, i.e. lookup the vertexID and get the bone id and weight
                // We'll support up to 4 bones per vertex, so we need a list of weights for each vertex
                foreach (var weight in bone.VertexWeights) {
                    if (vertToBoneWeight.ContainsKey(weight.VertexID)) {
                        vertToBoneWeight[weight.VertexID].Add(new VertexWeight((uint) boneIndex, weight.Weight));
                    } else {
                        vertToBoneWeight[weight.VertexID] = new List<VertexWeight>(
                            new[] {new VertexWeight((uint) boneIndex, weight.Weight)}
                        );
                    }
                }
            }
        }
    }
}