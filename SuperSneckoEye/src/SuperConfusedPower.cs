using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace EvanWard.SuperSneckoEye;

public class SuperConfusedPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner != base.Owner.Player)
        {
            return Task.CompletedTask;
        }
        if (card.EnergyCost.Canonical < 0)
        {
            return Task.CompletedTask;
        }
        int cost = NextEnergyCost();
        card.EnergyCost.SetThisCombat(cost);
        if (card.CanonicalStarCost > 0)
        {
            int starCost = NextEnergyCost();
            card.SetStarCostThisCombat(starCost);
        }
        foreach (var dynamicVar in card.DynamicVars.Values)
        {
            dynamicVar.BaseValue = NextDynamicVar();
        }
        NCard.FindOnTable(card)?.PlayRandomizeCostAnim();
        return Task.CompletedTask;
    }

    private int NextEnergyCost()
    {
        return base.Owner.Player.RunState.Rng.CombatEnergyCosts.NextInt(4);
    }

    private int NextDynamicVar()
    {
        return base.Owner.Player.RunState.Rng.CombatEnergyCosts.NextInt(ModInfo.ModParam.DynamicVarRange + 1);
    }
}
