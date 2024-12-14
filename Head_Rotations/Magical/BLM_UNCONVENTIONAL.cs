
namespace Head_Rotations.Magical;
[Rotation("Unlimited Paradox-work", CombatType.PvE, GameVersion = "7.11")]
[SourceCode(Path = "main/BasicRotations/Magical/BLM_Default.cs")]
[Api(4)]
public class BLM_Default : BlackMageRotation
{
    public new IBaseAction UmbralSoulPvE { get; } = new BaseAction((ActionID)16506);

    public new IBaseAction ManafontPvE { get; } = new BaseAction((ActionID)158);

    public IBaseAction FixedB4 { get; } = new BaseAction((ActionID)3576);

    /*
    public new IBaseAction LeyLinesPvE { get; } = new BaseAction((ActionID)3573);
    */

    public static bool NextGCDisInstant => Player.HasStatus(true, StatusID.Triplecast, StatusID.Swiftcast);

    public bool CanMakeInstant => TriplecastPvE.Cooldown.CurrentCharges > 0 || !SwiftcastPvE.Cooldown.IsCoolingDown;
    
    
    #region Additional oGCD Logic
    
    protected override IAction? CountDownAction(float remainTime)
    {
        IAction act;
        if (remainTime <= FireIiiPvE.Info.CastTime)
        {
            if (LeyLinesPvE.CanUse(out act)) return act;
            if (FireIiiPvE.CanUse(out act))
                return act;
        }
        return base.CountDownAction(remainTime);
    }
    
    [RotationDesc]
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        if (!NextGCDisInstant && InCombat)
        {
            if (InAstralFire && !HasFire && CurrentMp == 0 && PolyglotStacks == 0)
            {
                if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
                if (SwiftcastPvE.CanUse(out act)) return true;
            }

            if (InUmbralIce)
            {
                if (UmbralIceStacks < 3)
                {
                    if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
                    if (SwiftcastPvE.CanUse(out act)) return true;
                }

                if (UmbralHearts < 3)
                {
                    if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
                    if (SwiftcastPvE.CanUse(out act)) return true;
                }
            }

            if (!InUmbralIce && !InAstralFire)
                if (CanMakeInstant)
                {
                    if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
                    if (SwiftcastPvE.CanUse(out act)) return true;
                }
        }

        return base.EmergencyAbility(nextGCD, out act);
    }
    [RotationDesc(ActionID.AetherialManipulationPvE)]
    protected override bool MoveForwardAbility(IAction nextGCD, out IAction? act)
    {
        if (AetherialManipulationPvE.CanUse(out act)) return true;

        return base.MoveForwardAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.BetweenTheLinesPvE)]
    protected override bool MoveBackAbility(IAction nextGCD, out IAction? act)
    {
        if (BetweenTheLinesPvE.CanUse(out act)) return true;

        return base.MoveBackAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.ManawardPvE)]
    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? act)
    {
        if (ManawardPvE.CanUse(out act)) return true;
        return base.DefenseSingleAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.ManawardPvE, ActionID.AddlePvE)]
    protected sealed override bool DefenseAreaAbility(IAction nextGCD, out IAction? act)
    {
        if (ManawardPvE.CanUse(out act)) return true;
        if (AddlePvE.CanUse(out act)) return true;
        return base.DefenseAreaAbility(nextGCD, out act);
    }
    #endregion
    
    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        if (InCombat)
        {
            if (LeyLinesPvE.CanUse(out act, usedUp: true)) return true;
            
            if (!IsPolyglotStacksMaxed)
                if (AmplifierPvE.CanUse(out act))
                    return true;

            if (!NextGCDisInstant)
            {
                if (InAstralFire && !HasFire && CurrentMp == 0 && PolyglotStacks == 0)
                {
                    if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
                    if (SwiftcastPvE.CanUse(out act)) return true;
                }

                if (InUmbralIce)
                {
                    if (UmbralIceStacks < 3)
                    {
                        if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
                        if (SwiftcastPvE.CanUse(out act)) return true;
                    }

                    if (UmbralHearts < 3)
                    {
                        if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
                        if (SwiftcastPvE.CanUse(out act)) return true;
                    }
                }

                if (!InUmbralIce && !InAstralFire)
                    if (CanMakeInstant)
                    {
                        if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
                        if (SwiftcastPvE.CanUse(out act)) return true;
                    }
            }
        }


        return base.AttackAbility(nextGCD, out act);
    }

    protected override bool GeneralGCD(out IAction? act)
    {
        if (CombatTime > 1.5)
        {
            if (LeyLinesPvE.Cooldown.CurrentCharges == 2)
            {
                if (LeyLinesPvE.CanUse(out act)) return true;
            }
        }
        
        if (!NextGCDisInstant && InCombat)
        {
            if (InAstralFire && !HasFire && CurrentMp == 0 && PolyglotStacks == 0)
            {
                if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
                if (SwiftcastPvE.CanUse(out act)) return true;
            }

            if (InUmbralIce)
            {
                if (UmbralIceStacks < 3)
                {
                    if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
                    if (SwiftcastPvE.CanUse(out act)) return true;
                }

                if (UmbralHearts < 3)
                {
                    if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
                    if (SwiftcastPvE.CanUse(out act)) return true;
                }
            }

            if (!InUmbralIce && !InAstralFire)
                if (CanMakeInstant)
                {
                    if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
                    if (SwiftcastPvE.CanUse(out act)) return true;
                }
        }

        if (NextGCDisInstant)
            if (InUmbralIce)
            {
                if (UmbralIceStacks < 3)
                    if (BlizzardIiiPvE.CanUse(out act))
                        return true;
                if (UmbralHearts < 3)
                    if (BlizzardIvPvE.CanUse(out act))
                        return true;
            }

        if (!NextGCDisInstant)
        {
            if (FoulPvE.CanUse(out act)) return true;
            if (XenoglossyPvE.CanUse(out act)) return true;
        }

        if (IsParadoxActive)
            if (ParadoxPvE.CanUse(out act))
                return true;
        
        if (HostileTarget != null &&
            (!HostileTarget.HasStatus(true, StatusID.Thunder, StatusID.ThunderIi, StatusID.ThunderIii,
                StatusID.ThunderIv, StatusID.HighThunder_3872, StatusID.HighThunder) || HostileTarget.WillStatusEnd(3,
                true, StatusID.Thunder, StatusID.ThunderIi, StatusID.ThunderIii, StatusID.ThunderIv,
                StatusID.HighThunder_3872, StatusID.HighThunder)))
            if (ThunderPvE.CanUse(out act))
                return true;

        if (InAstralFire)
        {
            if (Player.HasStatus(true, StatusID.Firestarter))
                if (FireIiiPvE.CanUse(out act))
                    return true;
            if (CurrentMp < 800)
                if (ManafontPvE.CanUse(out act))
                    return true;
            if (CurrentMp >= 800)
                if (DespairPvE.CanUse(out act))
                    return true;
        }
        
        if (UmbralIceStacks == 3 && UmbralHearts == 3 && InUmbralIce)
            if (TransposePvE.CanUse(out act))
                return true;

        if (InAstralFire && AstralFireStacks == 3 && !Player.HasStatus(true, StatusID.Firestarter) && CurrentMp < 800)
            if (ManafontPvE.CanUse(out act))
                return true;
            else if (TransposePvE.CanUse(out act))
                return true;


        

        if (InUmbralIce)
            if (UmbralSoulPvE.CanUse(out act, skipCastingCheck: true))
                return true;

        if (!InUmbralIce && !InAstralFire)
            if (NextGCDisInstant)
            {
                if (FireIiiPvE.CanUse(out act))
                    return true;
            }
            else
            {
                if (FirePvE.CanUse(out act))
                    return true;
            }
                

        if (ScathePvE.CanUse(out act)) return true;

        return base.GeneralGCD(out act);
    }
}