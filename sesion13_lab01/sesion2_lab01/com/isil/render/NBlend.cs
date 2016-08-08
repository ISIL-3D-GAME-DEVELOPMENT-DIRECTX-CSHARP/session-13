using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX.Direct3D11;

namespace Sesion2_Lab01.com.isil.graphics {
    public struct NBlend {

        private static NBlend M_Default = new NBlend();

        public int RenderTargetIndex;
        public BlendStateDescription BlendStateDescription;

        public bool BlendEnable;

        public static NBlend Opaque() {
            NBlend defaultBlend = NBlend.M_Default;

            defaultBlend.BlendEnable = false;
            defaultBlend.RenderTargetIndex = 0;

            BlendStateDescription bddesc = BlendStateDescription.Default();
            RenderTargetBlendDescription rtbd = bddesc.RenderTarget[defaultBlend.RenderTargetIndex];

            rtbd.IsBlendEnabled = false;
            rtbd.SourceBlend = BlendOption.One;
            rtbd.DestinationBlend = BlendOption.Zero;
            rtbd.BlendOperation = BlendOperation.Add;
            rtbd.SourceAlphaBlend = BlendOption.One;
            rtbd.DestinationAlphaBlend = BlendOption.Zero;
            rtbd.AlphaBlendOperation = BlendOperation.Add;
            rtbd.RenderTargetWriteMask = ColorWriteMaskFlags.All;

            bddesc.RenderTarget[defaultBlend.RenderTargetIndex] = rtbd;
            defaultBlend.BlendStateDescription = bddesc;

            return defaultBlend;
        }

        public static NBlend AlphaBlend() {
            NBlend defaultBlend = NBlend.M_Default;

            defaultBlend.BlendEnable = true;
            defaultBlend.RenderTargetIndex = 0;

            BlendStateDescription bddesc = BlendStateDescription.Default();
            RenderTargetBlendDescription rtbd = bddesc.RenderTarget[defaultBlend.RenderTargetIndex];

            rtbd.IsBlendEnabled = true;
            rtbd.SourceBlend = BlendOption.SourceAlpha;
            rtbd.DestinationBlend = BlendOption.InverseSourceAlpha;
            rtbd.BlendOperation = BlendOperation.Add;
            rtbd.SourceAlphaBlend = BlendOption.One;
            rtbd.DestinationAlphaBlend = BlendOption.Zero;
            rtbd.AlphaBlendOperation = BlendOperation.Add;
            rtbd.RenderTargetWriteMask = ColorWriteMaskFlags.All;

            bddesc.RenderTarget[defaultBlend.RenderTargetIndex] = rtbd;
            defaultBlend.BlendStateDescription = bddesc;

            return defaultBlend;
        }
    }
}
