using Sesion2_Lab01.com.isil.content;
using Sesion2_Lab01.com.isil.data_type;
using Sesion2_Lab01.com.isil.render.camera;
using Sesion2_Lab01.com.isil.render.data;
using Sesion2_Lab01.com.isil.shader.d2d;
using Sesion2_Lab01.com.isil.system.fontSystem;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sesion2_Lab01.com.isil.render.components {
    public class NTextField2D {

        public const string JUMP_LINE_COMMAND = "\\n";

        private NBitmapFont mBitmapFontRef;

        private ShaderBitmapFontTextureProgram mShader;

        private dtNQuad[] mCustomQuads;
        private float[] mVertices;
        private ushort[] mIndices;

        private int mWidth;
        private int mHeight;

        private float mTextureWidth;
        private float mTextureHeight;
        private bool mDrawEnable;
        private int mOldNumQuads;

        private int mTextQuadCounter;
        private float mTextPositionRenderX;
        private float mTextPositionRenderY;
        private bool mJumpLineTextInNextUpdate;

        // TEXTFIELD VARIABLES
        private string mText;
        private bool mHaveShadow;
        private Color mShadowColor;
        private Color mTextColor;
        private Vector2 mTextSize;

        public virtual string text {
            get { return mText; }
            set {
                setText(value == null ? "" : value);
            }
        }

        public NTextField2D(NBitmapFont bitmapFont) {
            mBitmapFontRef = bitmapFont;

            mTextureWidth = mBitmapFontRef.texture.Width;
            mTextureHeight = mBitmapFontRef.texture.Height;

            mTextColor = Color.Black;
        }

        protected virtual bool preValidationText(ref string phrase) {
            return phrase != null && phrase.Length > 0 && mText != phrase;
        }

        private void setText(string phrase) {
            if (preValidationText(ref phrase)) {
                mText = phrase;
                mTextQuadCounter = 0;
                mTextPositionRenderX = 0;
                mTextPositionRenderY = 0;
                mJumpLineTextInNextUpdate = false;
                mTextSize = Vector2.Zero;

                mCustomQuads = new dtNQuad[phrase.Length];

                int oldUnicodeChar = -1;
                int phraseLength = mText.Length;
                int oldIndexAfterJumpLine = 0;
                int oldWidthAfterJumpLine = 0;
                string commandInsideString = string.Empty;
                NBitmapFontCharDescriptor charDescriptor;

                for (int i = 0; i < phraseLength; i++) {
                    int kerningAmount = 0;
                    int unicodeCharFirst = char.ConvertToUtf32(mText, i);

                    if (unicodeCharFirst == EnumNCharacterASCII.BACKSLASH ||
                        unicodeCharFirst == EnumNCharacterASCII.JUMPLINE ||
                        commandInsideString.Length >= 1) {

                        commandInsideString += char.ConvertFromUtf32(unicodeCharFirst);

                        bool makeJumpLine = false;

                        if (unicodeCharFirst == EnumNCharacterASCII.JUMPLINE) {
                            makeJumpLine = true;
                        }
                        else {
                            if (commandInsideString.Length >= 2) {
                                switch (commandInsideString) {
                                    case NTextField2D.JUMP_LINE_COMMAND:
                                        makeJumpLine = true;
                                        break;
                                }
                            }
                        }

                        if (makeJumpLine) {
                            commandInsideString = string.Empty;
                            mJumpLineTextInNextUpdate = true;

                            string newText = mText.Substring(oldIndexAfterJumpLine, (i - oldIndexAfterJumpLine));
                            oldIndexAfterJumpLine = i + 1;
                            int newTextLength = (int)mBitmapFontRef.measureString(newText).X;

                            if (newTextLength > oldWidthAfterJumpLine) {
                                oldWidthAfterJumpLine = newTextLength;
                                mTextSize.X = oldWidthAfterJumpLine;
                            }

                            mTextSize.Y += mBitmapFontRef.Base;
                        }
                    }
                    else {
                        // RETRIEVE KERNING
                        if (oldUnicodeChar != -1) {
                            mBitmapFontRef.getKerning(oldUnicodeChar,
                                unicodeCharFirst, out kerningAmount);
                        }

                        if (mBitmapFontRef.existChar(unicodeCharFirst)) {
                            // RETRIEVE CHAR DESCRIPTOR
                            mBitmapFontRef.getCharDescriptor(unicodeCharFirst,
                                out charDescriptor);

                            // ADD CHAR QUAD TO RENDER
                            addTextQuad(ref charDescriptor, kerningAmount);

                            // STORING OLD VARIABLES
                            oldUnicodeChar = unicodeCharFirst;
                        }
                    }
                }

                // IF NO JUMP LINE IS MADE, SET TEXT SIZE BY DEFAULT
                if (mTextSize.X == 0) {
                    Vector2 sizeText = mBitmapFontRef.measureString(mText);
                    mTextSize.X = sizeText.X;
                    mWidth = (int)sizeText.X;
                }
                if (mTextSize.Y == 0) {
                    mTextSize.Y = mBitmapFontRef.Base;
                    mHeight = mBitmapFontRef.Base;
                }

                // TRANSFORM QUAD DATA TO RENDER ENABLE DATA
                transformToRender();
            }
        }

        protected void preCalculateIndices(int numQuads) {
            if (mOldNumQuads != numQuads) {
                short indicesCount = 0;
                int indexCounterArray = 0;

                mIndices = new ushort[numQuads * 6];

                for (int i = 0; i < numQuads; ++i) {
                    mIndices[indexCounterArray] = (ushort)(indicesCount + 2);
                    mIndices[indexCounterArray + 1] = (ushort)(indicesCount + 3);
                    mIndices[indexCounterArray + 2] = (ushort)(indicesCount + 1);
                    mIndices[indexCounterArray + 3] = (ushort)(indicesCount + 2);
                    mIndices[indexCounterArray + 4] = (ushort)(indicesCount + 1);
                    mIndices[indexCounterArray + 5] = (ushort)indicesCount;
                    indicesCount += 4;
                    indexCounterArray += 6;
                }
            }
        }

        protected void transformToRender() {
            int mVertexCount = 0;

            int indexVDCounter = 0;
            int numTilesToDraw = 0;
            dtNQuad tempDSTQ;

            int w = 0;
            int textLenght = mCustomQuads.Length;

            for (; w < textLenght; w++) {
                if (mCustomQuads[w].created) {
                    numTilesToDraw++;
                }
            }

            preCalculateIndices(numTilesToDraw);

            mVertexCount = numTilesToDraw * 4;

            mVertices = new float[mVertexCount * 5];

            w = 0;

            for (; w < textLenght; w++) {
                tempDSTQ = mCustomQuads[w];

                if (tempDSTQ.created) {
                    mVertices[indexVDCounter + 0] = tempDSTQ.VPTTopRight.positionX;
                    mVertices[indexVDCounter + 1] = tempDSTQ.VPTTopRight.positionY;
                    mVertices[indexVDCounter + 2] = tempDSTQ.VPTTopRight.positionZ;
                    mVertices[indexVDCounter + 3] = tempDSTQ.VPTTopRight.textureU;
                    mVertices[indexVDCounter + 4] = tempDSTQ.VPTTopRight.textureV;

                    mVertices[indexVDCounter + 5] = tempDSTQ.VPTTopLeft.positionX;
                    mVertices[indexVDCounter + 6] = tempDSTQ.VPTTopLeft.positionY;
                    mVertices[indexVDCounter + 7] = tempDSTQ.VPTTopLeft.positionZ;
                    mVertices[indexVDCounter + 8] = tempDSTQ.VPTTopLeft.textureU;
                    mVertices[indexVDCounter + 9] = tempDSTQ.VPTTopLeft.textureV;

                    mVertices[indexVDCounter + 10] = tempDSTQ.VPTBottomRight.positionX;
                    mVertices[indexVDCounter + 11] = tempDSTQ.VPTBottomRight.positionY;
                    mVertices[indexVDCounter + 12] = tempDSTQ.VPTBottomRight.positionZ;
                    mVertices[indexVDCounter + 13] = tempDSTQ.VPTBottomRight.textureU;
                    mVertices[indexVDCounter + 14] = tempDSTQ.VPTBottomRight.textureV;

                    mVertices[indexVDCounter + 15] = tempDSTQ.VPTBottomLeft.positionX;
                    mVertices[indexVDCounter + 16] = tempDSTQ.VPTBottomLeft.positionY;
                    mVertices[indexVDCounter + 17] = tempDSTQ.VPTBottomLeft.positionZ;
                    mVertices[indexVDCounter + 18] = tempDSTQ.VPTBottomLeft.textureU;
                    mVertices[indexVDCounter + 19] = tempDSTQ.VPTBottomLeft.textureV;

                    indexVDCounter += 20;
                }
            }

            if (numTilesToDraw > 0) { mDrawEnable = true; }
            else { mDrawEnable = false; }
        }

        protected void addTextQuad(ref NBitmapFontCharDescriptor charDesc, int kerningAmount) {
            float tileX = charDesc.x;
            float tileY = charDesc.y;
            float tileWidth = charDesc.textureU;
            float tileHeight = charDesc.textureV;

            mCustomQuads[mTextQuadCounter].created = true;

            dtNQuad dynSTQuadTemp = mCustomQuads[mTextQuadCounter];

            if (mJumpLineTextInNextUpdate) {
                mJumpLineTextInNextUpdate = false;
                mTextPositionRenderX = 0;
                mTextPositionRenderY += mBitmapFontRef.Base;
            }

            float px = (mTextPositionRenderX + charDesc.xOffset + kerningAmount);
            float py = (mTextPositionRenderY + charDesc.yOffset);

            mTextPositionRenderX += (charDesc.xAdvance + kerningAmount);

            // VERTEX
            dynSTQuadTemp.VPTTopLeft.positionX = px;
            dynSTQuadTemp.VPTTopLeft.positionY = py;
            dynSTQuadTemp.VPTTopLeft.positionZ = 0;

            dynSTQuadTemp.VPTTopRight.positionX = (px + charDesc.width);
            dynSTQuadTemp.VPTTopRight.positionY = py;
            dynSTQuadTemp.VPTTopRight.positionZ = 0;

            dynSTQuadTemp.VPTBottomLeft.positionX = px;
            dynSTQuadTemp.VPTBottomLeft.positionY = (py + charDesc.height);
            dynSTQuadTemp.VPTBottomLeft.positionZ = 0;

            dynSTQuadTemp.VPTBottomRight.positionX = (px + charDesc.width);
            dynSTQuadTemp.VPTBottomRight.positionY = (py + charDesc.height);
            dynSTQuadTemp.VPTBottomRight.positionZ = 0;

            // TEXTURE COORDINATES
            dynSTQuadTemp.VPTTopLeft.textureU = tileX / mTextureWidth;
            dynSTQuadTemp.VPTTopLeft.textureV = tileY / mTextureHeight;
            dynSTQuadTemp.VPTTopRight.textureU = (tileX + tileWidth) / mTextureWidth;
            dynSTQuadTemp.VPTTopRight.textureV = tileY / mTextureHeight;
            dynSTQuadTemp.VPTBottomLeft.textureU = tileX / mTextureWidth;
            dynSTQuadTemp.VPTBottomLeft.textureV = (tileY + tileHeight) / mTextureHeight;
            dynSTQuadTemp.VPTBottomRight.textureU = (tileX + tileWidth) / mTextureWidth;
            dynSTQuadTemp.VPTBottomRight.textureV = (tileY + tileHeight) / mTextureHeight;

            // adding to buffer
            mCustomQuads[mTextQuadCounter] = dynSTQuadTemp;
            mTextQuadCounter++;
        }

        public void SetShader(ShaderBitmapFontTextureProgram shader) {
            mShader = shader;
        }

        public void Draw(RenderCamera camera, int dt) {
            mShader.X = 300;
            mShader.Y = 20;

            mShader.Update(mVertices, mIndices);
            mShader.Draw(camera.transformed, NColor.Red, mBitmapFontRef.texture);
        }
    }
}
