using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using System.Text.Json.Nodes;

namespace evanward.cardstats10x;

public static class ModInfo
{
    public const string AuthorName = "EvanWard";
    public const string ModName = "CardStats10x";

    public static ModParam ModParam => _modParam;
    private static ModParam _modParam = new()
    {
        Multiplier = 10
    };

    public static void InitModParam()
    {
        var mod = ModManager.Mods.First(m => m.manifest?.id == ModName);
        if (mod == null)
        {
            return;
        }
        var modPath = mod.path;
        var configPath = Path.Combine(modPath, $"{ModName}.json");
        if (!File.Exists(configPath))
        {
            return;
        }
        try
        {
            using Stream stream = File.OpenRead(configPath);
            JsonNode? jsonNode = JsonNode.Parse(stream);
            if (jsonNode == null)
            {
                return;
            }
            JsonNode? configNode = jsonNode["config"];
            if (configNode == null)
            {
                return;
            }
            JsonNode? multiplierNode = configNode["multiplier"];
            if (multiplierNode == null)
            {
                return;
            }
            int multiplier = multiplierNode.GetValue<int>();
            _modParam.Multiplier = multiplier;
            ModLogger.Info($"Config loaded successfully, multiplier: {_modParam.Multiplier}");
        }
        catch (Exception e)
        {
            ModLogger.Warn($"Failed to read {configPath}, using default multiplier. Error: " + e.Message);
        }
    }
}

public struct ModParam
{
    public int Multiplier;
}

public static class ModLogger
{
    public static void Info(string text)
    {
        Log.Info($"[{ModInfo.ModName}]{text}");
    }
    public static void Warn(string text)
    {
        Log.Warn($"[{ModInfo.ModName}]{text}");
    }
    public static void Error(string text)
    {
        Log.Error($"[{ModInfo.ModName}]{text}");
    }
}

