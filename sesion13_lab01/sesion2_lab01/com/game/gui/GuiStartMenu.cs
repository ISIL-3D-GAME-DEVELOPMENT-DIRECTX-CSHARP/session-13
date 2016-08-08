using Core.Model;
using CrossXDK.com.digitalkancer.modules.moderlLoaders.assimp;
using Sesion2_Lab01.com.isil.content;
using Sesion2_Lab01.com.isil.data_type;
using Sesion2_Lab01.com.isil.render.batcher;
using Sesion2_Lab01.com.isil.render.camera;
using Sesion2_Lab01.com.isil.render.components;
using Sesion2_Lab01.com.isil.render.graphics;
using Sesion2_Lab01.com.isil.shader.d2d;
using Sesion2_Lab01.com.isil.shader.d3d;
using Sesion2_Lab01.com.isil.shader.skinnedModel;
using Sesion2_Lab01.com.isil.system.screenManager;
using Sesion2_Lab01.com.isil.system.soundSystem;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sesion2_Lab01.com.game.gui {

    public class GuiStartMenu : Screen {

        private Shader3DProgram mShaderProgram;
        private ShaderTextureProgram mShaderSpriteProgram;
        private ShaderBitmapFontTextureProgram mShaderBitmapFontProgram;
        private SkinnedModelProgram mShaderSkinnedModel;

        private SkinnedModelInstance mSkinnedModel;
        private NModel mModel3DBetter;
        private Plane3D mPlane3D;
        private NSoundInstance mSoundInstance;

        private NSound mSoundNode;
        private NBitmapFont mBitmapFont;
        private NTextField2D mTextField;

        private RenderCamera mRenderCamera;

        private NSpriteBatch mSpriteBatch;

        private int mTimeCounter;

        public GuiStartMenu() : base() {
            mTimeCounter = 0;

            mRenderCamera = NativeApplication.instance.RenderCamera;
        }

        public override void Initialize() {
            base.Initialize();

            // cargamos y construimos nuestro Shader
            mShaderProgram = new Shader3DProgram(NativeApplication.instance.Device);
            mShaderProgram.Load("Content/Fx_PrimitiveTexture3D.fx");

            mShaderBitmapFontProgram = new ShaderBitmapFontTextureProgram(NativeApplication.instance.Device);
            mShaderBitmapFontProgram.Load("Content/Fx_SimpleBitmapFont.fx");

            mShaderSpriteProgram = new ShaderTextureProgram(NativeApplication.instance.Device);
            mShaderSpriteProgram.Load("Content/Fx_TexturePrimitive.fx");

            mShaderSkinnedModel = new SkinnedModelProgram(NativeApplication.instance.Device);
            mShaderSkinnedModel.Load("Content/Fx_SkinnedModel.fx");

            NTexture2D texture = new NTexture2D(NativeApplication.instance.Device);
            texture.Load("Content/spMario.png");

            mPlane3D = new Plane3D(texture, 0, 0, 0, 10f);
            mPlane3D.SetShader(mShaderProgram);

            SkinnedModel mageSkinnedModel = new SkinnedModel(NativeApplication.instance.Device,
                "Content/magician/magician.X", "Content/magician/", true);

            Matrix offsetTransform = Matrix.RotationY((float)Math.PI) * Matrix.Translation(4.0f, 0, 0);

            mSkinnedModel = new SkinnedModelInstance("Attack", offsetTransform,
                mageSkinnedModel, NativeApplication.instance.Device.ImmediateContext);
            mSkinnedModel.SetShaderProgram(mShaderSkinnedModel);

            foreach (var clip in mSkinnedModel.Clips) { mSkinnedModel.AddClip(clip); }
            mSkinnedModel.LoopClips = true;

            //mSprite = new NSprite2D("Content/spMario.png", 0, 0);
            //mSprite.SetShader(mShaderSpriteProgram);

            mBitmapFont = new NBitmapFont();
            mBitmapFont.Load("Content/font/kronika/kronika_16.fnt", 
                "Content/font/kronika/kronika_16.png");

            mTextField = new NTextField2D(mBitmapFont);
            mTextField.text = "Hola, soy una prueba en 3D =)";
            mTextField.SetShader(mShaderBitmapFontProgram);

            mSoundNode = new NSound("Content/sound/music_play_button.wav");

            mSoundInstance = new NSoundInstance(NativeApplication.instance.SoundDevice);
            mSoundInstance.BindDataBuffer(mSoundNode, 0);
            mSoundInstance.Play(() => {
                mSoundInstance = null;  
            });
            
            mSpriteBatch = new NSpriteBatch(texture);
            mSpriteBatch.SetShader(mShaderSpriteProgram);
        }

        public override void Update(int dt) {
            base.Update(dt);

            if (mSoundInstance != null) {
                mSoundInstance.Update(dt);
            }

            //if (mTimeCounter > 1500) {
            //    NativeApplication.instance.ScreenManager.GotoScreen(typeof(GuiWaraScreen));
            //}
            //else {
            //    mTimeCounter += dt;
            //}

            mSpriteBatch.AddSprite(50, 50, 0, 60, 60, NColor.White);
            mSpriteBatch.AddSprite(150, 0, 0, 120, 120, NColor.Blue);
            mSpriteBatch.Update(dt);
        }

        public override void Draw(int dt) {
            base.Draw(dt);

            /////////////////// EMPEZAMOS A DIBUJAR NUESTRO PRIMITIVO ///////////////
            mPlane3D.Draw(mRenderCamera, dt);

            mRenderCamera.ChangeCameraTo(RenderCamera.ORTHOGRAPHIC);
            
            //mSprite.Draw(mRenderCamera, dt);
            //mSprite.X += 1;
            
            mSpriteBatch.Draw(mRenderCamera, dt);


            mTextField.Draw(mRenderCamera, dt);

            mRenderCamera.ChangeCameraTo(RenderCamera.PERSPECTIVE);

            mSkinnedModel.Update(dt / 500f);
            mSkinnedModel.Draw(mRenderCamera.transformed);
        }
    }
}
