using Content.Shared.EntityEffects;
using Content.Shared.Adrenaline;
using Robust.Shared.Prototypes;

namespace Content.Server.EntityEffects.Effects;

public sealed partial class Adrenaline : EntityEffect
{
    /// <summary>
    /// Длительность эффекта адреналина в секундах.
    /// </summary>
    [DataField]
    public float Duration = 5f;

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
        => Loc.GetString("reagent-effect-guidebook-adrenaline", ("chance", Probability));

    public override void Effect(EntityEffectBaseArgs args)
    {
        var duration = Duration;

        if (args is EntityEffectReagentArgs reagentArgs)
            duration *= reagentArgs.Scale.Float();


        var adrenalineSys = args.EntityManager.EntitySysManager.GetEntitySystem<SharedAdrenalineSystem>();
        adrenalineSys.TryApplyAdrenaline(args.TargetEntity, duration);
    }
}
