using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using System.Reflection;

namespace EvanWard.CardStats10x;

[HarmonyPatch]
public static class CardCostCanonicalPatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        var cardModelType = typeof(CardModel);
        var assembly = cardModelType.Assembly;
        string[] propertyNames =
        {
            "CanonicalEnergyCost",
            "CanonicalStarCost"
        };

        return assembly.GetTypes()
            .Where(t => !t.IsAbstract)
            .Where(t => cardModelType.IsAssignableFrom(t))
            .SelectMany(t => propertyNames
                .Select(propertyName => AccessTools.PropertyGetter(t, propertyName)))
            .Where(m => m != null);
    }

    private static void Postfix(ref int __result)
    {
        __result *= ModInfo.ModParam.Multiplier;
    }
}

[HarmonyPatch(typeof(CardEnergyCost), nameof(CardEnergyCost.UpgradeBy))]
public static class CardCostUpgradePatch
{
    private static void Prefix(ref int addend)
    {
        addend *= ModInfo.ModParam.Multiplier;
    }
}
