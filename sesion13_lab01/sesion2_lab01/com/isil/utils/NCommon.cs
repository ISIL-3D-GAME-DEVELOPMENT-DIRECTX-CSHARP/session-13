using Assimp;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sesion2_Lab01.com.isil.utils {
    public class NCommon {

        public static Matrix InverseTranspose(Matrix m) {

            var a = m;
            a.M41 = a.M42 = a.M43 = 0;
            a.M44 = 1;

            return Matrix.Transpose(Matrix.Invert(a));
        }

        public static Vector3 ConvertVector3DToVector3(Vector3D inVector) {
            Vector3 outVector = Vector3.Zero;
            outVector.X = inVector.X;
            outVector.Y = inVector.Y;
            outVector.Z = inVector.Z;

            return outVector;
        }

        public static Vector2 ConvertVector3DToVector2(Vector3D inVector) {
            Vector2 outVector = Vector2.Zero;
            outVector.X = inVector.X;
            outVector.Y = inVector.Y;

            return outVector;
        }

        public static Matrix ConvertMatrix4x4ToMatrix(Matrix4x4 inMatrix) {
            //inMatrix.Transpose();

            Matrix outMatrix = Matrix.Identity;

            outMatrix.M11 = inMatrix.A1;
            outMatrix.M12 = inMatrix.A2;
            outMatrix.M13 = inMatrix.A3;
            outMatrix.M14 = inMatrix.A4;

            outMatrix.M21 = inMatrix.B1;
            outMatrix.M22 = inMatrix.B2;
            outMatrix.M23 = inMatrix.B3;
            outMatrix.M24 = inMatrix.B4;

            outMatrix.M31 = inMatrix.C1;
            outMatrix.M32 = inMatrix.C2;
            outMatrix.M33 = inMatrix.C3;
            outMatrix.M34 = inMatrix.C4;

            outMatrix.M41 = inMatrix.D1;
            outMatrix.M42 = inMatrix.D2;
            outMatrix.M43 = inMatrix.D3;
            outMatrix.M44 = inMatrix.D4;

            return outMatrix;
        }

        public static char Char_Parse_Win8(string inParameter) {
            bool wasSuccesfull = false;
            return Char_Parse_Win8(inParameter, out wasSuccesfull);
        }

        public static char Char_Parse_Win8(string inParameter, out bool wasSuccesfull) {
            char output = char.MinValue;
            wasSuccesfull = char.TryParse(inParameter, out output);
            return output;
        }

        public static string eraseCharsInString(string baseString, int[] asciiChars) {
            string newStrings = "";
            int charsLength = asciiChars.Length;
            int stringLength = baseString.Length;

            for (int i = 0; i < stringLength; i++) {
                int unicodeChar = char.ConvertToUtf32(baseString, i);
                bool comparisionIsTrue = false;

                for (int j = 0; j < charsLength; j++) {
                    if (asciiChars[j] == unicodeChar) {
                        comparisionIsTrue = true;
                        break;
                    }
                }

                if (!comparisionIsTrue) {
                    newStrings += baseString[i];
                }
            }

            return newStrings;
        }
    }
}
