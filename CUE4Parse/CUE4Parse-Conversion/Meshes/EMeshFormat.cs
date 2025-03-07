using System.ComponentModel;

namespace CUE4Parse_Conversion.Meshes
{
    public enum EMeshFormat
    {
        [Description("ActorX (psk/pskx)")]
        ActorX,
        [Description("glTF2.0 (二进制)")]
        Gltf2,
        [Description("Wavefront OBJ (未实施)")]
        OBJ,
        [Description("UEFormat (uemodel)")]
        UEFormat
    }
}
