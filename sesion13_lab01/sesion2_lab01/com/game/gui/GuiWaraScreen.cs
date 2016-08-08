using Sesion2_Lab01.com.isil.content;
using Sesion2_Lab01.com.isil.render.camera;
using Sesion2_Lab01.com.isil.render.components;
using Sesion2_Lab01.com.isil.shader.d2d;
using Sesion2_Lab01.com.isil.system.screenManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sesion2_Lab01.com.game.gui {

    public class GuiWaraScreen : Screen {

        private NBitmapFont mBitmapFont;
        private ShaderBitmapFontTextureProgram mShaderBitmapFontProgram;

        private NTextField2D mTextField;

        private RenderCamera mRenderCamera;

        private int mTimeCounter;

        public GuiWaraScreen() : base() {
            mTimeCounter = 0;

            mRenderCamera = NativeApplication.instance.RenderCamera;
        }

        public override void Initialize() {
            base.Initialize();

            mBitmapFont = new NBitmapFont();
            mBitmapFont.Load("Content/font/kronika/kronika_16.fnt",
                "Content/font/kronika/kronika_16.png");

            mShaderBitmapFontProgram = new ShaderBitmapFontTextureProgram(NativeApplication.instance.Device);
            mShaderBitmapFontProgram.Load("Content/Fx_SimpleBitmapFont.fx");

            mTextField = new NTextField2D(mBitmapFont);
            mTextField.text = "It's me Wara!";
            mTextField.SetShader(mShaderBitmapFontProgram);
        }

        public override void Update(int dt) {
            base.Update(dt);

            if (mTimeCounter > 1500) {
                NativeApplication.instance.ScreenManager.GotoScreen(typeof(GuiStartMenu));
            }
            else {
                mTimeCounter += dt;
            }
        }

        public override void Draw(int dt) {
            base.Draw(dt);

            mRenderCamera.ChangeCameraTo(RenderCamera.ORTHOGRAPHIC);

            mTextField.Draw(mRenderCamera, dt);
        }
    }
}
