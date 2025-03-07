using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Readers;
using CUE4Parse.UE4.Versions;

namespace CUE4Parse.GameTypes.InfinityNikki;

public static class FX6GameCustomVersion
{
    public enum Type
    {
        None = 0,
        WorldTileChanges1 = 2,
        StaticMeshSectionChanges1 = 4,
        SkelMeshRenderSectionChanges1 = 8,
        WorldTileChanges2 = 10,
        SkelMeshRenderAndStaticMeshSectionChanges2 = 12,
        LatestVersionPlusOne
    }

    public static readonly FGuid GUID = new(0x30EF2F0B, 0xE46646A5, 0xB16F28E8, 0xA0FA9A7D);

    public static Type Get(FArchive Ar)
    {
        var ver = Ar.CustomVer(GUID);
        if (ver >= 0)
            return (Type) ver;

        return Type.LatestVersionPlusOne;
    }
}
