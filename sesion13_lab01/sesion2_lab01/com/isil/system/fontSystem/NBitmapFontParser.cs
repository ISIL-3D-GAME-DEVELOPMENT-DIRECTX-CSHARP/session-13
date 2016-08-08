using System;
using System.IO;

using Sesion2_Lab01.com.isil.utils;

namespace Sesion2_Lab01.com.isil.system.fontSystem {
public class NBitmapFontParser {

	private NBitmapFontCharset mFontCharset;

	// TEMPORAL VARIABLES
	private NBitmapFontPage mNBitmapFontPageTemp;
	private int mNBitmapFontPageIndexTemp;
	private int mNBitmapFontCharIndexTemp;
	private int mNBitmapFontKerningIndexTemp;
	private int[] mASCIIFilter;

	public NBitmapFontCharset fontCharset { get { return mFontCharset; } }

	internal NBitmapFontParser (TextReader data) {
		mNBitmapFontPageIndexTemp = -1;
		mNBitmapFontCharIndexTemp = -1;
		mNBitmapFontKerningIndexTemp = -1;

		bool finish = false;
		string tmpString = "";
		string readedLine = "";

		mASCIIFilter = new int[4];
		mASCIIFilter[0] = EnumNCharacterASCII.NOTHING;
		mASCIIFilter[1] = EnumNCharacterASCII.BACKSLASH;
		mASCIIFilter[2] = EnumNCharacterASCII.QUOTATION_MARK;
		mASCIIFilter[3] = EnumNCharacterASCII.WHITE_SPACE;
		
		while (!finish) {
			readedLine = data.ReadLine ();
			
			if (readedLine != null) {
				bool firstRead = true;
				bool finishInternal = false;
				int lineLength = readedLine.Length;
				int linePosition = 0;
				EnumNBitmapFontRawType lineType = EnumNBitmapFontRawType.NONE;
				
				while (!finishInternal) {
					if (linePosition >= lineLength) {
						finishInternal = true;
						
						if (tmpString.Length > 0) {
							readingAndSettingValues(ref lineType, ref tmpString);
							tmpString = "";
						}
						break;
					} 
					else {
						char letter = readedLine [linePosition];
						bool iss = char.IsSeparator (letter);
						
						if (iss) {
							if (tmpString.Length > 0) {
								if (firstRead) {
									firstRead = false;
									getLineType(ref tmpString, out lineType);
								}
								else { readingAndSettingValues(ref lineType, ref tmpString); }

								tmpString = "";
							}
						} else { tmpString += letter; }
						
						linePosition++;
					}
				}
			} else {
				// FINISH THE READ OF FILE
				finish = true;
				mNBitmapFontPageIndexTemp = 0;
				mNBitmapFontCharIndexTemp = 0;
				mNBitmapFontKerningIndexTemp = 0;
			}
		}

		readedLine = null;

		data.Close();
		data.Dispose();
		data = null;
	}

	private void getLineType (ref string valueType, out EnumNBitmapFontRawType lineType) {
		lineType = EnumNBitmapFontRawType.NONE;

		switch (valueType) {
		case "info": 	lineType = EnumNBitmapFontRawType.INFO; 	break;
		case "common": 	lineType = EnumNBitmapFontRawType.COMMON; 	break;
		case "page": 	lineType = EnumNBitmapFontRawType.PAGE; 	break;
		case "chars": 	lineType = EnumNBitmapFontRawType.CHARS; 	break;
		case "char": 	lineType = EnumNBitmapFontRawType.CHAR; 	break;
		case "kerning": lineType = EnumNBitmapFontRawType.KERNING; 	break;
		case "kernings":lineType = EnumNBitmapFontRawType.KERNINGS; break;
		}
	}

	private void readingAndSettingValues(ref EnumNBitmapFontRawType lineType, ref string tmpString) {
		string propertyType = "";
		string propertyValue = "";

		int charPos = tmpString.IndexOf(EnumNCharacterUnicode.EQUALS_SIGN);
		propertyType = tmpString.Substring(0, charPos);
		propertyValue = tmpString.Substring(charPos + 1);
		propertyValue = propertyValue.Replace(EnumNCharacterUnicode.QUOTATION_MARK,
			EnumNCharacterUnicode.NOTHING);
		propertyValue = propertyValue.Replace(EnumNCharacterUnicode.BACKSLASH,
			EnumNCharacterUnicode.NOTHING);

		// TEMP VARIABLES
		NBitmapFontCharDescriptor charDescTemp = new NBitmapFontCharDescriptor();
		
		switch(lineType) {
		case EnumNBitmapFontRawType.INFO:
			switch(propertyType) {
			case "face": 		mFontCharset.fontName = NCommon.eraseCharsInString(propertyValue, mASCIIFilter); break;
			case "size": 		mFontCharset.size = int.Parse(propertyValue); 							break;
			case "bold": 		mFontCharset.bold = propertyValue == "0" ? false : true; 				break;
			case "italic": 		mFontCharset.italic = propertyValue == "0" ? false : true; 				break;
			case "charset": 	mFontCharset.charset = NCommon.eraseCharsInString(propertyValue, mASCIIFilter);	break;
			case "unicode": 	mFontCharset.unicode = propertyValue == "0" ? false : true; 			break;
			case "stretchH": 	mFontCharset.stretchH = int.Parse(propertyValue); 						break;
			case "smooth": 		mFontCharset.smooth = propertyValue == "0" ? false : true; 				break;
			case "aa": 			mFontCharset.aa = propertyValue == "0" ? false : true; 					break;
			case "padding": 
				mFontCharset.padding = new int[4] { 
					int.Parse(char.ConvertFromUtf32(propertyValue[0])),
					int.Parse(char.ConvertFromUtf32(propertyValue[2])),
					int.Parse(char.ConvertFromUtf32(propertyValue[4])), 
					int.Parse(char.ConvertFromUtf32(propertyValue[6])) };
				break;
			case "spacing":
				mFontCharset.spacing = new int[2] { 
					int.Parse(char.ConvertFromUtf32(propertyValue[0])),
					int.Parse(char.ConvertFromUtf32(propertyValue[2])) };
				break;
			case "outline": mFontCharset.outline = propertyValue == "0" ? false : true; break;
			}
			break;
		case EnumNBitmapFontRawType.COMMON:
			switch(propertyType) {
			case "lineHeight":	mFontCharset.lineHeight = int.Parse(propertyValue); 		break;
			case "base": 		mFontCharset.Base = int.Parse(propertyValue); 				break;
			case "scaleW": 		mFontCharset.scaleWidth = int.Parse(propertyValue); 		break;
			case "scaleH": 		mFontCharset.scaleHeight = int.Parse(propertyValue); 		break;
			case "pages": 		
				mFontCharset.pages = int.Parse(propertyValue); 	
				mFontCharset.fontPages = new NBitmapFontPage[mFontCharset.pages];
				break;
			case "packed": 		mFontCharset.packed = propertyValue == "0" ? false : true; 	break;
			case "alphaChnl": 	mFontCharset.alphaChnl = int.Parse(propertyValue); 			break;
			case "redChnl": 	mFontCharset.redChnl = int.Parse(propertyValue); 			break;
			case "greenChnl": 	mFontCharset.greenChnl = int.Parse(propertyValue); 			break;
			case "blueChnl": 	mFontCharset.blueChnl = int.Parse(propertyValue); 			break;
			}
			break;
		case EnumNBitmapFontRawType.PAGE:
			NBitmapFontPage NBFPage;

			for (int i = 0; i < mFontCharset.fontPages.Length; i++) {
				NBFPage = mFontCharset.fontPages[i];

				if(!NBFPage.isInitialized) {
					switch(propertyType) {
					case "id" : NBFPage.id = int.Parse(propertyValue); break;
					case "file": 	
						NBFPage.path = NCommon.eraseCharsInString(propertyValue, mASCIIFilter);
						NBFPage.isInitialized = true;
						
						// THIS ARE TEMPORAL VARIABLES FOR THE PAGES
						mNBitmapFontCharIndexTemp = 0;
						mNBitmapFontPageIndexTemp = i;
						mNBitmapFontPageTemp = NBFPage;
						break;
					}
					
					mFontCharset.fontPages[i] = NBFPage;
					break;
				}
			}
			break;
		case EnumNBitmapFontRawType.CHARS:
			switch(propertyType) {
			case "count":
				mNBitmapFontCharIndexTemp = -1;
				mNBitmapFontPageTemp.charCount = int.Parse(propertyValue);
				mNBitmapFontPageTemp.chars = new NBitmapFontCharDescriptor[NBitmapFontCharset.CHAR_DESCRIPTOR_SIZE];
				mFontCharset.fontPages[mNBitmapFontPageIndexTemp] = mNBitmapFontPageTemp;
				break;
			}
			break;
		case EnumNBitmapFontRawType.CHAR:
			if(mNBitmapFontCharIndexTemp != -1) {
				charDescTemp = mNBitmapFontPageTemp.chars[mNBitmapFontCharIndexTemp];
			}

			switch (propertyType) {
			case "id": 			
				charDescTemp.id = int.Parse(propertyValue);
				mNBitmapFontCharIndexTemp = charDescTemp.id;
				break;
			case "x": 			charDescTemp.x = int.Parse(propertyValue); 			break;
			case "y": 			charDescTemp.y = int.Parse(propertyValue); 			break;
			case "width": 		
				charDescTemp.width = int.Parse(propertyValue); 
				charDescTemp.textureU = charDescTemp.width; 
				break;
			case "height": 		
				charDescTemp.height = int.Parse(propertyValue);
				charDescTemp.textureV = charDescTemp.height;
				break;
			case "xoffset": 	charDescTemp.xOffset = int.Parse(propertyValue); 	break;
			case "yoffset": 	charDescTemp.yOffset = int.Parse(propertyValue); 	break;
			case "xadvance": 	charDescTemp.xAdvance = int.Parse(propertyValue); 	break;
			case "page": 		charDescTemp.page = int.Parse(propertyValue); 		break;
			case "chnl": 		charDescTemp.chnl = int.Parse(propertyValue);		break;
			}

			mNBitmapFontPageTemp.chars[mNBitmapFontCharIndexTemp] = charDescTemp;
			mFontCharset.fontPages[mNBitmapFontPageIndexTemp] = mNBitmapFontPageTemp;
			break;
		case EnumNBitmapFontRawType.KERNINGS:
			mNBitmapFontKerningIndexTemp = 0;
			mFontCharset.kernings = new NBitmapFontKerning[int.Parse(propertyValue)];
			break;
		case EnumNBitmapFontRawType.KERNING:
			bool incrementKerningReader = false;
			NBitmapFontKerning kerningTemp = mFontCharset.kernings[mNBitmapFontKerningIndexTemp];

			switch(propertyType) {
			case "first": 	kerningTemp.first = int.Parse(propertyValue); 	break;
			case "second": 	kerningTemp.second = int.Parse(propertyValue); 	break;
			case "amount": 	
				incrementKerningReader = true;
				kerningTemp.amount = int.Parse(propertyValue); 	
				break;
			}

			mFontCharset.kernings[mNBitmapFontKerningIndexTemp] = kerningTemp;

			if(incrementKerningReader) {
				// GO TO NEXT KERNING NODE
				mNBitmapFontKerningIndexTemp++;
			}
			break;
		}
	}
}
}

