using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Windows;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;

using Device = SharpDX.Direct3D11.Device;
using System.Diagnostics;
using Sesion2_Lab01.com.isil.utils;
using CrossXDK.com.digitalkancer.modules.moderlLoaders.assimp;
using Sesion2_Lab01.com.isil.shader.d3d;
using Sesion2_Lab01.com.isil.graphics;
using Sesion2_Lab01.com.isil.shader.d2d;
using Sesion2_Lab01.com.isil.system;
using Sesion2_Lab01.com.isil.render.camera;
using Sesion2_Lab01.com.isil.render.graphics;
using Sesion2_Lab01.com.isil.content;
using Sesion2_Lab01.com.isil.render.components;
using Sesion2_Lab01.com.isil.system.soundSystem;
using System.Threading;
using Sesion2_Lab01.com.isil.system.screenManager;
using Sesion2_Lab01.com.game.gui;

namespace Sesion2_Lab01 {
    public class NativeApplication {

        public static NativeApplication instance = null;

        public const int App_Width = 800;
        public const int App_Height = 600;

        // Es como el Windows.Forms, pero este se utiliza nativamente para el DirectX
        // Se necesita las librerias:
        //  1. System.Windows.Forms
        //  2. SharpDX.Windows
        private RenderForm mRenderForm;

        private Texture2D mBackBufferFBO;
        private DepthStencilView mDepthStencilView;
        private RenderTargetView mRenderTargetView;
        private Device mDevice;
        private DeviceContext mDeviceContext;

        private Factory mFactory;

        private NViewport mViewport;
        private RenderCamera mRenderCamera;

        private ScreenManager mScreenManager;

        private SwapChain mSwapChain;
        private SwapChainDescription mSwapChainDescription;

        private Stopwatch mStopWatch;
        private NKeyboardHandler mKeyboardHandler;
        private NMouseHandler mMouseHandler;
        private NSoundDevice mSoundDevice;

        private int mOldTimeDelta;
        private int mOldTimeTimeCounter;
        private int mNewTime;
        private int mDeltaTime;
        private int mTimeCounter;
        private int mFrameRate;

        public NKeyboardHandler KeyboardHandler { get { return mKeyboardHandler; } }
        public NMouseHandler MouseHandler { get { return mMouseHandler; } }

        public Device Device                { get { return mDevice; } }
        public NSoundDevice SoundDevice     { get { return mSoundDevice; } }
        public RenderCamera RenderCamera    { get { return mRenderCamera; } }
        public ScreenManager ScreenManager  { get { return mScreenManager; } }

        public NativeApplication() {
            NativeApplication.instance = this;

            mOldTimeDelta = 0;
            mOldTimeTimeCounter = 0;
            mNewTime = 0;
            mDeltaTime = 0;
            mTimeCounter = 0;
            mFrameRate = 1000 / 30;

            mStopWatch = new Stopwatch();
            mStopWatch.Start();

            mRenderForm = new RenderForm("Sesion 7::Aplicacion Nativa DirectX 11");

            // ahora le damos la refernecia del formulario asi sabra de donde viene los
            // eventos del teclado y de mouse
            mMouseHandler = new NMouseHandler(mRenderForm);

            mKeyboardHandler = new NKeyboardHandler(mRenderForm, OnKeyDownHandler, OnKeyUpHandler);

            mSwapChainDescription = new SwapChainDescription(); // es una estructura, no es necesario construirlo
            mSwapChainDescription.BufferCount = 1;
            mSwapChainDescription.IsWindowed = true; // es ventana
            mSwapChainDescription.SwapEffect = SwapEffect.Discard;
            mSwapChainDescription.Usage = Usage.RenderTargetOutput;
            mSwapChainDescription.OutputHandle = mRenderForm.Handle; // le pasamos el puntero de nuestro RenderForm
            mSwapChainDescription.SampleDescription.Count = 1;
            mSwapChainDescription.SampleDescription.Quality = 0;
            mSwapChainDescription.ModeDescription.Width = NativeApplication.App_Width;  // aqui definimos el ancho de la ventana
            mSwapChainDescription.ModeDescription.Height = NativeApplication.App_Height; // aqui definimos el alto de la ventana
            mSwapChainDescription.ModeDescription.RefreshRate.Numerator = 60; // aqui definimos el Frame Rate
            mSwapChainDescription.ModeDescription.RefreshRate.Denominator = 1; // siempre por defecto 1... la matematica es Denominator / Numerator
            mSwapChainDescription.ModeDescription.Format = Format.R8G8B8A8_UNorm; // se define el formato del color de la ventana

            // Creamos la tarjeta grafica usando el SwapChainDescription
            // Esto nos devuelve:
            //  1. Device -> La tarjeta grafica
            //  2. SwapChain -> Control del Frame Rate
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, mSwapChainDescription,
                out mDevice, out mSwapChain);

            // recogemos el contexto de la tarjeta de video
            mDeviceContext = mDevice.ImmediateContext;

            RasterizerStateDescription rsd = new RasterizerStateDescription();
            rsd.CullMode = CullMode.None;
            rsd.FillMode = FillMode.Solid;
            rsd.IsFrontCounterClockwise = true;
            rsd.DepthBias = 0;
            rsd.DepthBiasClamp = 0;
            rsd.SlopeScaledDepthBias = 0;
            rsd.IsDepthClipEnabled = true;
            rsd.IsMultisampleEnabled = false;

            RasterizerState rs = new RasterizerState(mDevice, rsd);

            // Set the rasterizer state to the D3D Context
            mDeviceContext.Rasterizer.State = rs;

            // recogemos el padre constructor de la ventana
            mFactory = mSwapChain.GetParent<Factory>();
            // ignoramos todos los eventos de la ventana
            mFactory.MakeWindowAssociation(mRenderForm.Handle, WindowAssociationFlags.IgnoreAll);

            // creamos un frame buffer object, y usamos este back buffer para almacenar la data del render
            mBackBufferFBO = Texture2D.FromSwapChain<Texture2D>(mSwapChain, 0);

            // creamos un render target view para manejar el render usando nuestro frame buffer object
            mRenderTargetView = new RenderTargetView(mDevice, mBackBufferFBO);
            // ahora creamos el depth stencil para el Z
            mDepthStencilView = CreateDepthStencilBuffer();

            // creamos el sound device
            mSoundDevice = new NSoundDevice();

            // Ahora inicializamos algunos datos para poder dibujar
            PreConfiguration();

            // Ahora creamos algo muy importante! Nuestro Render Loop, donde ira nuestro Draw y Update!
            RenderLoop.Run(mRenderForm, OnRenderLoop);
        }

        private DepthStencilView CreateDepthStencilBuffer() {
            Texture2DDescription depthTextDesc = new Texture2DDescription();
            depthTextDesc.Format = Format.D32_Float_S8X24_UInt;
            depthTextDesc.ArraySize = 1;
            depthTextDesc.MipLevels = 1;
            depthTextDesc.Width = NativeApplication.App_Width;
            depthTextDesc.Height = NativeApplication.App_Height;
            depthTextDesc.SampleDescription = new SampleDescription(1, 0);
            depthTextDesc.Usage = ResourceUsage.Default;
            depthTextDesc.BindFlags = BindFlags.DepthStencil;
            depthTextDesc.CpuAccessFlags = CpuAccessFlags.None;
            depthTextDesc.OptionFlags = ResourceOptionFlags.None;

            Texture2D depthBuffer = new Texture2D(mDevice, depthTextDesc);
            return new DepthStencilView(mDevice, depthBuffer);
        }

        private void PreConfiguration() {
            // preparamos nuestros parametros para dibujar
            // creamos una camara para el escenario
            mViewport = new NViewport(NativeApplication.App_Width, NativeApplication.App_Height);

            mRenderCamera = new RenderCamera(mViewport, -5f);

            mScreenManager = new ScreenManager();
            mScreenManager.GotoScreen(typeof(GuiStartMenu));
        }

        private void OnRenderLoop() {
            bool canRender = false;

            mNewTime = (int)mStopWatch.ElapsedMilliseconds;
            mTimeCounter += mNewTime - mOldTimeTimeCounter;

            if (mTimeCounter >= mFrameRate) {
                mTimeCounter = 0;
                canRender = true;

                mDeltaTime = mNewTime - mOldTimeDelta;
                mOldTimeDelta = mNewTime;
            }

            mOldTimeTimeCounter = mNewTime;

            if (canRender) {
                Update(mDeltaTime);
                Draw(mDeltaTime);
            }
        }

        private void OnKeyDownHandler(int keyCode) {
            if (mRenderCamera != null) {
                mRenderCamera.OnKeyDown(keyCode);
            }
        }

        private void OnKeyUpHandler(int keyCode) {
            if (mRenderCamera != null) {
                mRenderCamera.OnKeyUp(keyCode);
            }
        }

        private void Update(int dt) {
            mMouseHandler.Update(dt);

            // actualizamos nuestra camara
            mRenderCamera.Update();

            mScreenManager.Update(dt);
        }

        private void Draw(int dt) {
            // ahora pasamos nuestro Frame Buffer Object
            mDeviceContext.OutputMerger.SetTargets(mDepthStencilView, mRenderTargetView);

            // Creamos nuestro Viewport que define el tamanio de nuestras dimension de dibujo para
            // el DirectX
            mDeviceContext.Rasterizer.SetViewports(new Viewport(0, 0, mViewport.Width,
                mViewport.Height, 0.0f, 1.0f));

            // aqui definimos nuestro color usando Color4
            Color4 clearColor = Color4.Black;
            clearColor.Red = 0f;
            clearColor.Green = 1f;
            clearColor.Blue = 0f;

            // aqui limpiamos nuestra ventana con un color solido
            mDeviceContext.ClearRenderTargetView(mRenderTargetView, clearColor);

            // ahora limpiamos nuestro depth buffer
            mDeviceContext.ClearDepthStencilView(mDepthStencilView,
                DepthStencilClearFlags.Depth, 1.0f, 0);

            mScreenManager.Draw(dt);

            // hacemos un swap de buffers
            mSwapChain.Present(0, PresentFlags.None);
        }
    }
}
