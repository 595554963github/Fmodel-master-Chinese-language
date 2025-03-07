using System.ComponentModel;

namespace CUE4Parse_Conversion.Meshes;

public enum ESocketFormat
{
    [Description("在单独的头部中导出骨骼插孔")]
    Socket,
    [Description("将骨骼插孔导出为骨骼")]
    Bone,
    [Description("不要导出骨骼插孔")]
    None
}
