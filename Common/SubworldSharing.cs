using Divergency.Content.Biomes;
using MonoMod.Cil;
using SubworldLibrary;
using System.IO;
using System.Reflection;
using Terraria.IO;

namespace Divergency.Common;

#if DEBUG
public class LocalSubworldSaving : ModSystem
{
    public override void OnModLoad()
    {
        MonoModHooks.Modify(
            typeof(SubworldSystem).GetMethod("LoadWorld",
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public
            ),
            LoadWorld_LoadLocalFile
        );
    }

    private static void LoadWorld_LoadLocalFile(ILContext il)
    {
        ILCursor c = new(il);

        int inSubworldIndex = -1; // loc
        int pathIndex = -1; // loc

        if (
            !c.TryGotoNext(
                MoveType.After,
                i => i.MatchLdloc(out inSubworldIndex),
                i => i.MatchBrtrue(out _),
                i => i.MatchLdsfld<SubworldSystem>("main"),
                i => i.MatchCallvirt<FileData>($"get_{nameof(FileData.Path)}"),
                i => i.MatchBr(out _),
                i => i.MatchCall<SubworldSystem>($"get_{nameof(SubworldSystem.CurrentPath)}"),
                i => i.MatchStloc(out pathIndex)
            )
        )
        {
            return;
        }

        c.EmitLdloca(pathIndex);
        c.EmitLdloc(inSubworldIndex);

        c.EmitDelegate(
            static (ref string path, bool flag) =>
            {
                if (SubworldSystem.Current is LivingCoreSubworld && flag)
                {
                    path = Path.Combine(ModContent.GetInstance<Divergency>().SourceFolder, "SubworldSave", SubworldSystem.Current.FileName + ".wld");
                }
            }
        );
    }
}
#endif
