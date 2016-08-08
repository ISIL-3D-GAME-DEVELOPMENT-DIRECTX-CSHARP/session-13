using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sesion2_Lab01.com.isil.data_type {
    public struct NColor {
        private static NColor Default = NColor.CreateColor(0, 0, 0, 0);

        public float R;
        public float G;
        public float B;
        public float A;

        public static NColor Gray 									= NColor.CreateColor(0.5f, 0.5f, 0.5f, 1f);
	    public static NColor StrongGray 							= NColor.CreateColor(0.25f, 0.25f, 0.25f, 1f);
	    public static NColor White0P7 								= NColor.CreateColor(0.7f, 0.7f, 0.7f, 1f);
	    public static NColor White 									= NColor.CreateColor(1f, 1f, 1f, 1f);
	    public static NColor White1P2 								= NColor.CreateColor(1.2f, 1.2f, 1.2f, 1f);
	    public static NColor White2 								= NColor.CreateColor(2f, 2f, 2f, 1f);
	    public static NColor White2P2 								= NColor.CreateColor(2.2f, 2.2f, 2.2f, 1f);
	    public static NColor White2P5 								= NColor.CreateColor(2.5f, 2.5f, 2.5f, 1f);
	    public static NColor White3 								= NColor.CreateColor(3f, 3f, 3f, 1f);
	    public static NColor TransparentBlack 						= NColor.CreateColor(0, 0, 0, 122);
	    public static NColor Transparent 							= NColor.CreateColor(0, 0, 0, 0);
	    public static NColor Black 									= NColor.CreateColor(0, 0, 0, 255);
	    public static NColor Red 									= NColor.CreateColor(122, 0, 0, 255);
	    public static NColor Green 									= NColor.CreateColor(0, 122, 0, 255);
	    public static NColor Blue 									= NColor.CreateColor(0, 0, 122, 255);
	    public static NColor StrongRed 								= NColor.CreateColor(255, 0, 0, 255);
	    public static NColor StrongGreen 							= NColor.CreateColor(0, 255, 0, 255);
	    public static NColor StrongBlue 							= NColor.CreateColor(0, 0, 255, 255);
	    public static NColor StrongBlueTransparencyThirdPercent 	= NColor.CreateColor(0, 0, 255, 85);
	    public static NColor StrongGreenTransparencyThirdPercent 	= NColor.CreateColor(255, 0, 0, 85);
	    public static NColor Beige 									= NColor.CreateColor(245, 245, 220, 255);
	    public static NColor AliceBlue 								= NColor.CreateColor(240, 248, 255, 255);
	    public static NColor Brown 									= NColor.CreateColor(165, 42, 42, 255);
	    public static NColor Coral 									= NColor.CreateColor(255, 127, 80, 255);
	    public static NColor Cyan 									= NColor.CreateColor(0, 255, 255, 255);
	    public static NColor DarkOrange 							= NColor.CreateColor(255, 140, 0, 255);
	    public static NColor DeepPink 								= NColor.CreateColor(255, 20, 147, 255);
	    public static NColor DarkViolet 							= NColor.CreateColor(148, 0, 211, 255);
	    public static NColor DarkTurquoise 							= NColor.CreateColor(0, 206, 209, 255);
	    public static NColor DeepSkyBlue 							= NColor.CreateColor(0, 191, 255, 255);
	    public static NColor Fuschia 								= NColor.CreateColor(255, 0, 255, 255);
	    public static NColor Ivory 									= NColor.CreateColor(255, 255, 240, 255);
	    public static NColor LightSalmon 							= NColor.CreateColor(255, 160, 122, 255);
	    public static NColor Lime 									= NColor.CreateColor(0, 255, 0, 255);
	    public static NColor Magenta 								= NColor.CreateColor(255, 0, 255, 255);
	    public static NColor Orange 								= NColor.CreateColor(255, 165, 0, 255);
	    public static NColor Pink 									= NColor.CreateColor(255, 192, 203, 255);
	    public static NColor Purple 								= NColor.CreateColor(128, 0, 128, 255);
	    public static NColor Silver 								= NColor.CreateColor(192, 192, 192, 255);
	    public static NColor Snow 									= NColor.CreateColor(255, 250, 250, 255);
	    public static NColor Teal 									= NColor.CreateColor(0, 128, 128, 255);
	    public static NColor Violet 								= NColor.CreateColor(238, 130, 238, 255);
	    public static NColor Wheat 									= NColor.CreateColor(245, 222, 179, 255);
	    public static NColor Yellow 								= NColor.CreateColor(255, 255, 0, 255);
	    public static NColor Yellow2 								= NColor.CreateColor(2f, 2f, 0, 1f);
	    public static NColor YellowGreen 							= NColor.CreateColor(154, 205, 50, 255);

        private static NColor CreateColor(float nativeR, float nativeG, float nativeB, float nativeA) {
            NColor newColor = NColor.Default;
            newColor.R = (nativeR * 255f);
            newColor.G = (nativeG * 255f);
            newColor.B = (nativeB * 255f);
            newColor.A = (nativeA * 255f);
            return newColor;
        }

        private static NColor CreateColor(float nativeR, float nativeG, float nativeB) {
            NColor newColor = NColor.Default;
            newColor.R = (nativeR * 255f);
            newColor.G = (nativeG * 255f);
            newColor.B = (nativeB * 255f);
            newColor.A = (1.0f * 255f);
            return newColor;
        }

        private static NColor CreateColor(byte r, byte g, byte b, byte a) {
            NColor newColor = NColor.Default;
            newColor.R = (float)r;
            newColor.G = (float)g;
            newColor.B = (float)b;
            newColor.A = (float)a;
            return newColor;
        }

        public static NColor ToNormalize(NColor colorToNormalize) {
            NColor newColor = NColor.White;
            newColor.R = colorToNormalize.R / 255.0f;
            newColor.G = colorToNormalize.G / 255.0f;
            newColor.B = colorToNormalize.B / 255.0f;
            newColor.A = colorToNormalize.A / 255.0f;
            return newColor;
        }
    }
}
