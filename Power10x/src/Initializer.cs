using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace EvanWard.Power10x;

[ModInitializer(nameof(Initialize))]
public static class Initializer
{
    public static void Initialize()
    {
        ModInfo.InitModParam();
        try
        {
            var harmony = new Harmony($"{ModInfo.AuthorName}.{ModInfo.ModName}");
            harmony.PatchAll();
        }
        catch (Exception e)
        {
            ModLogger.Error(e.Message);
            return;
        }
        ModLogger.Info("Mod Loaded");
    }
}

