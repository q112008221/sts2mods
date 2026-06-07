using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Reflection;

namespace EvanWard.SuperSneckoEye;

[HarmonyPatch]
public static class StartingRelics_AllOverrides_Patch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        var baseType = typeof(CharacterModel);
        var assembly = baseType.Assembly;

        return assembly.GetTypes()
            .Where(t => t != baseType)
            .Where(t => baseType.IsAssignableFrom(t))
            .Select(t => AccessTools.PropertyGetter(t, "StartingRelics"))
            .Where(m => m != null)
            .Distinct();
    }

    static void Postfix(ref IReadOnlyList<RelicModel> __result)
    {
        if (__result == null)
            __result = new List<RelicModel>();

        var list = __result.ToList();
        list.Add(ModelDb.Relic<SuperSneckoEye>());
        __result = list.AsReadOnly();
    }
}
