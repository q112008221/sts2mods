using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace EvanWard.CardStats10x;

[HarmonyPatch(typeof(DynamicVar), nameof(DynamicVar.SetOwner))]
public static class DynamicVarSetOwnerPatch
{
    static void Prefix(ref AbstractModel ____owner, out bool __state)
    {
        __state = ____owner is CardModel;
    }

    static void Postfix(DynamicVar __instance, AbstractModel owner, bool __state)
    {
        bool wasCard = __state;
        bool isCard = owner is CardModel;

        if (!wasCard && isCard)
        {
            __instance.BaseValue *= ModInfo.ModParam.Multiplier;
        }
        else if (wasCard && !isCard)
        {
            __instance.BaseValue /= ModInfo.ModParam.Multiplier;
        }
    }
}

[HarmonyPatch(typeof(DynamicVar), nameof(DynamicVar.UpgradeValueBy))]
public static class DynamicVarUpgradePatch
{
    static void Prefix(ref AbstractModel ____owner, ref decimal addend)
    {
        if (____owner is CardModel)
        {
            addend *= ModInfo.ModParam.Multiplier;
        }
    }
}
