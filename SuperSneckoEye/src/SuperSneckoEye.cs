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

namespace EvanWard.SuperSneckoEye;

public class SuperSneckoEye : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new List<IHoverTip> { HoverTipCompat.FromPower<SuperConfusedPower>() };
    public override async Task AfterObtained()
    {
        if (CombatManager.Instance.IsInProgress)
        {
            await ApplyPower();
        }
    }
    public override async Task BeforeCombatStart()
    {
        await ApplyPower();
    }
    private async Task ApplyPower()
    {
        await PowerCmdCompat.Apply<SuperConfusedPower>(base.Owner.Creature, 1m, base.Owner.Creature, null);
    }
}
