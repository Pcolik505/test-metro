using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.StatusEffect;

namespace Content.Shared.Adrenaline;

public sealed class SharedAdrenalineSystem : EntitySystem
{
    [ValidatePrototypeId<StatusEffectPrototype>]
    public const string AdrenalineKey = "NoCrit";

    [Dependency] private readonly StatusEffectsSystem _statusEffectsSystem = default!;
    [Dependency] private readonly MobStateSystem _mobStateSystem = default!;

    /// <summary>
    /// Применяет эффект адреналина на моба: оживляет из крита и добавляет статус на время.
    /// </summary>
    public void TryApplyAdrenaline(EntityUid uid, float duration,
        StatusEffectsComponent? status = null, MobStateComponent? mobState = null)
    {
        if (!Resolve(uid, ref status, false))
            return;

        // Добавляем или продлеваем статус
        if (!_statusEffectsSystem.HasStatusEffect(uid, AdrenalineKey, status))
        {
            _statusEffectsSystem.TryAddStatusEffect<AdrenalineStatusComponent>(
				uid, AdrenalineKey, TimeSpan.FromSeconds(duration), true, status);
        }
        else
        {
            _statusEffectsSystem.TryAddTime(uid, AdrenalineKey, TimeSpan.FromSeconds(duration), status);
        }

        // Если моб в критическом состоянии — поднимаем его
        if (Resolve(uid, ref mobState, false))
        {
            if (mobState.CurrentState == MobState.Critical)
                _mobStateSystem.ChangeMobState(uid, MobState.Alive, mobState);
			
        }
    }

    /// <summary>
    /// Удаляет эффект адреналина.
    /// </summary>
    public void TryRemoveAdrenaline(EntityUid uid)
    {
        _statusEffectsSystem.TryRemoveStatusEffect(uid, AdrenalineKey);
    }

    /// <summary>
    /// Удаляет часть времени эффекта.
    /// </summary>
    public void TryRemoveAdrenalineTime(EntityUid uid, double timeRemoved)
    {
        _statusEffectsSystem.TryRemoveTime(uid, AdrenalineKey, TimeSpan.FromSeconds(timeRemoved));
    }
}
