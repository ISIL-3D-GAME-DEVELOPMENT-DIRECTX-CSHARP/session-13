using System.Collections.Generic;
using System.Linq;
//using Core.FX;

using SharpDX;
using SharpDX.Direct3D11;
using Sesion2_Lab01.com.isil.utils;
using Assimp;
using Sesion2_Lab01.com.isil.shader.skinnedModel;
using Sesion2_Lab01.com.isil.shader.d3d;
using Sesion2_Lab01;
using Sesion2_Lab01.com.isil.content;
using Sesion2_Lab01.com.isil.graphics;

namespace Core.Model {

    public class SkinnedModelInstance {

        private readonly SkinnedModel mSkinnedModel;
        private readonly Queue<string> mClipQueue;

        private float mTimePosition;
        private string mClipName;
        
        private SkinnedModelProgram mShaderProgram;
        private DeviceContext mDeviceContext;
        private BlendState mBlendState;

        public Matrix World     { get; set; }
        public bool LoopClips   { get; set; }

        // these are the available animation clips
        public IEnumerable<string> Clips { 
            get { return mSkinnedModel.Animator.Animations.Select(a => a.Name); }
        } 
        
        // the bone transforms for the mesh instance
        private List<Matrix> FinalTransforms  { 
            get { return mSkinnedModel.Animator.GetTransforms(mTimePosition); }
        }

        public string ClipName {
            get { return mClipName; }
            set {
                System.Diagnostics.Debug.WriteLine(value);
                mClipName = mSkinnedModel.Animator.Animations.Any(a => a.Name == value) ? value : "Still";
                mSkinnedModel.Animator.SetAnimation(mClipName);
                mTimePosition = 0;
            }
        }
        
        public SkinnedModelInstance(string clipName, Matrix transform, 
            SkinnedModel model, DeviceContext deviceContext) {
            
            World = transform;
            mSkinnedModel = model;
            ClipName = clipName;

            mDeviceContext = deviceContext;

            mClipQueue = new Queue<string>();

            NBlend opaqueBlend = NBlend.Opaque();
            mBlendState = new BlendState(deviceContext.Device, opaqueBlend.BlendStateDescription);
        }

        public void SetShaderProgram(SkinnedModelProgram shaderProgram) {
            mShaderProgram = shaderProgram;
        }

        public void AddClip(string clip) {
            if (mSkinnedModel.Animator.Animations.Any(a => a.Name == clip)) {
                mClipQueue.Enqueue(clip);
            }
        }

        public void ClearClips() {
            mClipQueue.Clear();
        }

        public void Update(float dt) {
            mTimePosition += dt;

            if (mTimePosition > mSkinnedModel.Animator.Duration) {
                if (mClipQueue.Any()) {
                    ClipName = mClipQueue.Dequeue();
                    if (LoopClips) {
                        mClipQueue.Enqueue(ClipName);
                    }
                }
                else {
                    ClipName = "Still";
                }
            }
        }

        public void Draw(Matrix transformation) {
            // ahora seteamos el tipo de blend
            mDeviceContext.OutputMerger.SetBlendState(mBlendState);

            Matrix world = World;
            Matrix wit = NCommon.InverseTranspose(world);

            transformation = world * transformation;
            transformation.Transpose();

            SkinnedModelInputParameters inputParameters = SkinnedModelInputParameters.EMPTY;
            inputParameters.gWorld = world;
            inputParameters.gWorldInvTranspose = wit;
            inputParameters.gWorldViewProj = transformation;
            inputParameters.gTexTransform = Matrix.Identity;

            Matrix[] boneTransforms = this.FinalTransforms.ToArray<Matrix>();

            for (int i = 0; i < mSkinnedModel.SubsetCount; i++) {
                NTexture2D srvDiffuse = null;
                NTexture2D srvNormal = null;

                if (mSkinnedModel.DiffuseMapSRV.Count > 0) { srvDiffuse = mSkinnedModel.DiffuseMapSRV[i]; }
                if (mSkinnedModel.NormalMapSRV.Count > 0) { srvNormal = mSkinnedModel.NormalMapSRV[i]; }
                
                mSkinnedModel.ModelMesh.Draw(mShaderProgram, inputParameters, boneTransforms,
                    srvDiffuse, srvNormal, mDeviceContext, i);
            }
        }
    }
}