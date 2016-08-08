using Sesion2_Lab01.com.isil.utils;

using System;

namespace Sesion2_Lab01.com.isil.system.fontSystem {
public sealed class EnumNCharacterUnicode {
	public static readonly char DOT 			= '.';
	public static readonly char WHITE_SPACE 	= ' ';
	public static readonly char EQUALS_SIGN 	= '=';
	public static readonly char SLASH 			= '/';


    public static readonly char QUOTATION_MARK  = NCommon.Char_Parse_Win8(char.ConvertFromUtf32(EnumNCharacterASCII.QUOTATION_MARK));
    public static readonly char BACKSLASH       = NCommon.Char_Parse_Win8(char.ConvertFromUtf32(EnumNCharacterASCII.BACKSLASH));
    public static readonly char NOTHING         = NCommon.Char_Parse_Win8(char.ConvertFromUtf32(EnumNCharacterASCII.NOTHING));
}
}

