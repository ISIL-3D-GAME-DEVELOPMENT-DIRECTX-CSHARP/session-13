using System;
using System.IO;
using System.Collections.Generic;

using Sesion2_Lab01.com.isil.system.fontSystem;
using SharpDX.IO;
using SharpDX;

namespace Sesion2_Lab01.com.isil.content {
public class NBitmapFont {

	private string mNativePath;
	private int mFontSize;
	private NTexture2D mTexture;
	private NBitmapFontParser mFontParser;

	private Dictionary<string, int> mFontSizeRepository;

	public int fontSize 				{ get { return mFontSize; } }
	public int originalFontSize 		{ get { return mFontParser.fontCharset.size; } }
	public string nativePath 			{ get { return mNativePath; } }
	public NTexture2D texture 			{ get { return mTexture; } }

	public int Base { 
		get { 
			float scaleFactor = (float)mFontSize / (float)mFontParser.fontCharset.size;
			return (int)Math.Ceiling(((float)mFontParser.fontCharset.Base * scaleFactor)); 
		} 
	}

	public NBitmapFont () {
		mNativePath = string.Empty;
		mFontSize = 0;
		mTexture = null;
		mFontParser = null;
		mFontSizeRepository = new Dictionary<string, int>();
	}

	public void Load (string fontPath, string texturePath) {
        byte[] rawData = NativeFile.ReadAllBytes(fontPath);
        StreamReader streamReader = new StreamReader(new MemoryStream(rawData));

        mFontParser = new NBitmapFontParser(streamReader);

		mTexture = new NTexture2D(NativeApplication.instance.Device);
        mTexture.Load(texturePath);
	}

	public void addFontSize (string fontNameID, int fontSize) {
		fontNameID = fontNameID.ToLower();

		if (!mFontSizeRepository.ContainsKey (fontNameID)) {
			mFontSizeRepository [fontNameID] = fontSize;
			mFontSize = fontSize;
		}
	}

	public bool existFont (string fontNameID) {
		fontNameID = fontNameID.ToLower();
		return mFontSizeRepository.ContainsKey (fontNameID);
	}

	public int getFontSize (string fontNameID) {
		fontNameID = fontNameID.ToLower();
		return mFontSizeRepository[fontNameID];
	}

	public void switchFontSize (int fontSize) {
		mFontSize = fontSize;
	}

	public void setNativePath (string path) {
		if (mNativePath == string.Empty) {
			mNativePath = path;
		}
	}

	public Vector2 measureString (string text) { 
		Vector2 textSize = Vector2.Zero;

		int oldUnicodeChar = -1;
		int phraseLength = text.Length;
		NBitmapFontCharDescriptor charDescriptor;

		for (int i = 0; i < phraseLength; i++) {
			int kerningAmount = 0;
			int unicodeCharFirst = char.ConvertToUtf32(text, i);
			
			if (existChar(unicodeCharFirst)) {
				// RETRIEVE KERNING
				if(oldUnicodeChar != -1) {
					getKerning(oldUnicodeChar, unicodeCharFirst, out kerningAmount);
				}
				
				// RETRIEVE CHAR DESCRIPTOR
				getCharDescriptor(unicodeCharFirst, out charDescriptor);
				
				textSize.X += (charDescriptor.xAdvance + kerningAmount);
				
				// STORING OLD VARIABLES
				oldUnicodeChar = unicodeCharFirst;
			}
		}
		
		return textSize;
	}

	public void getKerning (int unicodeFirst, int unicodeSecond, out int amount) {
		mFontParser.fontCharset.getKerning(unicodeFirst, unicodeSecond, out amount);
		amount = (int)((float)amount * ((float)mFontSize / (float)mFontParser.fontCharset.size));
	}

	public bool existChar(int charUnicode) {
		return mFontParser.fontCharset.existsChar(charUnicode);
	}

	public void getCharDescriptor (int charUnicode, out NBitmapFontCharDescriptor charDescriptor) {
		NBitmapFontCharDescriptor _charDescriptor;

        mFontSize = mFontParser.fontCharset.size;
		float scaleFactor = (float)mFontSize / (float)mFontParser.fontCharset.size;

		mFontParser.fontCharset.getCharDescriptor(charUnicode, out _charDescriptor);

		_charDescriptor.width = (int)((float)_charDescriptor.textureU * scaleFactor);
		_charDescriptor.height = (int)((float)_charDescriptor.textureV * scaleFactor);
		_charDescriptor.xAdvance = (int)((float)_charDescriptor.xAdvance * scaleFactor);
		_charDescriptor.xOffset = (int)((float)_charDescriptor.xOffset * scaleFactor);
		_charDescriptor.yOffset = (int)((float)_charDescriptor.yOffset * scaleFactor);
		charDescriptor = _charDescriptor;
	}
}
}

