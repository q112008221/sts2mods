using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using System.Text.Json.Nodes;

namespace EvanWard.SuperSneckoEye;

public static class ModInfo
{
    public const string AuthorName = "EvanWard";
    public const string ModName = "SuperSneckoEye";

    public static ModParam ModParam => _modParam;
    private static ModParam _modParam = new()
    {
        DynamicVarRange = 15,
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
            JsonNode? dynamicVarRangeNode = configNode["dynamic_var_range"];
            if (dynamicVarRangeNode == null)
            {
                return;
            }
            int dynamicVarRange = dynamicVarRangeNode.GetValue<int>();
            _modParam.DynamicVarRange = dynamicVarRange;
            ModLogger.Info($"Config loaded successfully, dynamicVarRange: {_modParam.DynamicVarRange}");
        }
        catch (Exception e)
        {
            ModLogger.Warn($"Failed to read {configPath}, using default multiplier. Error: " + e.Message);
        }
    }
}

public struct ModParam
{
    public int DynamicVarRange;
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

