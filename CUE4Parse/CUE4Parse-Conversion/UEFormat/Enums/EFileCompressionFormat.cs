using System.ComponentModel;

namespace CUE4Parse_Conversion.UEFormat.Enums;

public enum EFileCompressionFormat
{
    [Description("未压缩的")]
    None,
    
    [Description("Gzip压缩")]
    GZIP,
    
    [Description("ZStandard压缩")]
    ZSTD
}
