using System;

namespace Sesion2_Lab01.com.isil.system.fontSystem {
public struct NBitmapFontCharDescriptor {
	public int id;	 
	public int x;	 
	public int y;
	public int width;
	public int height;
	public int textureU;
	public int textureV;
	public int xOffset;
	public int yOffset;
	public int xAdvance;
	public int page;
	public int chnl;

	public static readonly NBitmapFontCharDescriptor EMPTY = new NBitmapFontCharDescriptor();
}
}

