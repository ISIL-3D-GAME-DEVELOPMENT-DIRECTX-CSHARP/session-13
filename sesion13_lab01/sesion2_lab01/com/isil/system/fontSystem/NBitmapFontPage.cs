using System;

namespace Sesion2_Lab01.com.isil.system.fontSystem {
public struct NBitmapFontPage {
	public int id;
	public int charCount;
	public string path;
	public bool isInitialized;
	public NBitmapFontCharDescriptor[] chars;
}
}

