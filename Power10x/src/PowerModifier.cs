using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using System.Reflection;

namespace EvanWard.Power10x;

[HarmonyPatch(typeof(Hook), nameof(Hook.ModifyPowerAmountGiven))]
public static class PowerPatch
{
    static void Postfix(ref decimal __result)
    {
        __result *= 10m;
    }
}
