using System;

namespace Sesion2_Lab01.com.isil.system.fontSystem {
public struct NBitmapFontCharset {
	public const int CHAR_DESCRIPTOR_SIZE = 256;

	// info
	public string fontName;
	public int size;
	public bool bold;
	public bool italic;
	public string charset;
	public bool unicode;
	public int stretchH;
	public bool smooth;
	public bool aa;
	public int[] padding;
	public int[] spacing;
	public bool outline;
	
	// common
	public int lineHeight;
	public int Base;
	public int scaleWidth;
	public int scaleHeight;
	public int pages;
	public bool packed;
	public int alphaChnl;
	public int redChnl;
	public int greenChnl;
	public int blueChnl;

	// pages
	public NBitmapFontPage[] fontPages;

	// kerning
	public NBitmapFontKerning[] kernings;

	public void getCharDescriptor (int charUnicode, out NBitmapFontCharDescriptor charDescriptor) {
		charDescriptor = fontPages[0].chars[charUnicode];
	}

	public bool existsChar(int charUnicode) {
		if ((charUnicode - 1) < fontPages[0].chars.Length) {
			return true;
		}
		
		return false;
	}

	public void getKerning (int unicodeFirst, int unicodeSecond, out int amount) {
		int kerningCount = kernings.Length;
		amount = 0;

		for (int i = 0; i < kerningCount; i++) {
			NBitmapFontKerning _kerning = kernings[i];
			
			if(_kerning.first == unicodeFirst && _kerning.second == unicodeSecond) {
				amount = _kerning.amount;
				break;
			}
		}
	}
}
}

