using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Build.Utilities;

public static class PatchNoteTools
{
    public static string GetPatchNote()
    {
        var patchNote = File.ReadAllText(BuildEnvironment.PatchNotePath);

        patchNote = patchNote.Replace("$(Version)", ProductInfo.Version);
        patchNote = patchNote.Replace("$(ReleaseDate)", DateTime.Now.ToString("yyyy-MM-dd"));
        patchNote = patchNote.Replace("$(DocumentUrl)", ProductInfoExtended.DocumentUrl);

        return patchNote;
    }
}
