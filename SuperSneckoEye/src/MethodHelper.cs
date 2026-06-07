using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using System.Reflection;
using System.Text.Json.Nodes;

namespace EvanWard.SuperSneckoEye;

public static class HoverTipCompat
{
    private static readonly MethodInfo? FromPowerMethod;

    static HoverTipCompat()
    {
        Type factoryType = typeof(HoverTipFactory); // 改成 FromPower 所在类

        FromPowerMethod = factoryType
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m =>
                m.Name == "FromPower"
                && m.IsGenericMethodDefinition
                && m.GetParameters().Length == 0)
            ?? factoryType
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m =>
                m.Name == "FromPower"
                && m.IsGenericMethodDefinition
                && m.GetParameters().Length == 1
                && m.GetParameters()[0].ParameterType == typeof(int?));
    }

    public static IHoverTip FromPower<T>()
        where T : PowerModel
    {
        if (FromPowerMethod == null)
            throw new MissingMethodException("No compatible FromPower<T> method found.");

        MethodInfo generic = FromPowerMethod.MakeGenericMethod(typeof(T));

        object?[]? args = FromPowerMethod.GetParameters().Length == 0
            ? null
            : new object?[] { null };

        return (IHoverTip)generic.Invoke(null, args)!;
    }
}

public static class PowerCmdCompat
{
    private static readonly MethodInfo ApplyMethod;
    private static readonly bool HasChoiceContext;

    static PowerCmdCompat()
    {
        Type type = typeof(PowerCmd);

        ApplyMethod =
            FindApplyWithChoiceContext(type)
            ?? FindApplyWithoutChoiceContext(type)
            ?? throw new MissingMethodException(type.FullName, "Apply<T>");

        HasChoiceContext = ApplyMethod.GetParameters().Length == 6;
    }

    public static Task<T?> Apply<T>(
        Creature target,
        decimal amount,
        Creature? applier,
        CardModel? cardSource,
        bool silent = false)
        where T : PowerModel
    {
        MethodInfo generic = ApplyMethod.MakeGenericMethod(typeof(T));

        object?[] args = HasChoiceContext
            ? new object?[]
            {
                new ThrowingPlayerChoiceContext(),
                target,
                amount,
                applier,
                cardSource,
                silent
            }
            : new object?[]
            {
                target,
                amount,
                applier,
                cardSource,
                silent
            };

        try
        {
            return (Task<T?>)generic.Invoke(null, args)!;
        }
        catch (TargetInvocationException ex)
        {
            throw ex.InnerException ?? ex;
        }
    }

    private static MethodInfo? FindApplyWithChoiceContext(Type type)
    {
        return type
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m =>
                m.Name == "Apply"
                && m.IsGenericMethodDefinition
                && HasParameterTypes(m,
                    typeof(PlayerChoiceContext),
                    typeof(Creature),
                    typeof(decimal),
                    typeof(Creature),
                    typeof(CardModel),
                    typeof(bool)));
    }

    private static MethodInfo? FindApplyWithoutChoiceContext(Type type)
    {
        return type
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m =>
                m.Name == "Apply"
                && m.IsGenericMethodDefinition
                && HasParameterTypes(m,
                    typeof(Creature),
                    typeof(decimal),
                    typeof(Creature),
                    typeof(CardModel),
                    typeof(bool)));
    }

    private static bool HasParameterTypes(MethodInfo method, params Type[] expected)
    {
        ParameterInfo[] parameters = method.GetParameters();

        if (parameters.Length != expected.Length)
            return false;

        for (int i = 0; i < expected.Length; i++)
        {
            if (parameters[i].ParameterType != expected[i])
                return false;
        }

        return true;
    }
}
