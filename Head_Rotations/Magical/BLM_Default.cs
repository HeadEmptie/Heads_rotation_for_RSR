using FFXIVClientStructs.FFXIV.Client.Game;

namespace RebornRotations.Magical;

[Rotation("Optimized_Beta", CombatType.PvE, GameVersion = "7.11")]
[SourceCode(Path = "main/BasicRotations/Magical/BLM_Default.cs")]
[Api(4)]
public class BLM_Default : BlackMageRotation
{
    #region Config Options

    [RotationConfig(CombatType.PvE,
        Name = "Use Retrace when out of Ley Lines and standing still (Dangerous and Experimental)")]
    private bool UseRetrace { get; set; } = false;

    [RotationConfig(CombatType.PvE, Name = "Extend Astral Fire time more conservatively (3 GCDs) (Default is 2 GCDs)")]
    private bool ExtendTimeSafely { get; set; } = false;

    #endregion

    #region Additional oGCD Logic

    protected override IAction? CountDownAction(float remainTime)
    {
        IAction act;
        if (remainTime < FireIiiPvE.Info.CastTime + CountDownAhead)
        {
            if (FireIiiPvE.CanUse(out act)) return act;
        }
        return base.CountDownAction(remainTime);
    }

    [RotationDesc]
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        act = null;
        if (!HasHostilesInRange || NotInCombatDelay) return false;
        if (UseRetrace && RetracePvE.CanUse(out act)) return true;
        //To Fire
        if (CurrentMp >= 7200 && UmbralIceStacks == 2 && ParadoxPvE.EnoughLevel)
        {
            if ((HasFire || HasSwift) && TransposePvE.CanUse(out act)) return true;
        }

        //Using Manafont
        if (InAstralFire)
        {
            if (CurrentMp == 0 && ManafontPvE.CanUse(out act)) return true;
            //To Ice
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

    #region oGCD Logic

    [RotationDesc(ActionID.ManafontPvE, ActionID.TransposePvE)]
    protected override bool GeneralAbility(IAction nextGCD, out IAction? act)
    {
        if (IsMoving && HasHostilesInRange && (TriplecastPvE.CanUse(out act, usedUp: true) || SwiftcastPvE.CanUse(out act))) return true;
        

        return base.GeneralAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.RetracePvE, ActionID.SwiftcastPvE, ActionID.TriplecastPvE, ActionID.AmplifierPvE)]
    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        act = null;
        if (!InCombat) return false;
        if (InUmbralIce)
        {
            if (UmbralIceStacks == 2 && !HasFire && !IsLastGCD(ActionID.ParadoxPvE))
            {
                if (SwiftcastPvE.CanUse(out act)) return true;
                if (TriplecastPvE.CanUse(out act, usedUp: true)) return true;
            }

            if (UmbralIceStacks < 3 && LucidDreamingPvE.CanUse(out act)) return true;
        }

        if (InAstralFire)
        {
            if (AstralFireStacks == 6)
            {
                if (SwiftcastPvE.CanUse(out act) ||  TriplecastPvE.CanUse(out act)) return true;
            }
            if (TriplecastPvE.CanUse(out act, gcdCountForAbility: 5)) return true;
        }

        if (AmplifierPvE.CanUse(out act)) return true;
        return base.AttackAbility(nextGCD, out act);
    }

    #endregion

    #region GCD Logic

    protected override bool GeneralGCD(out IAction? act)
    {
        if (AddThunder(out act)) return true;
        if (PolyglotDump(out act)) return true;
        if (FirePhase(out act)) return true;
        if (IcePhase(out act)) return true;
        if (NeedsElement(out act)) return true;
        
        if (MaintainStatus(out act)) return true;
        return  base.GeneralGCD(out act);
    }

    private bool IcePhase(out IAction? act)
    {
        act = null;
        if (!InUmbralIce) return false;
        if (Player.CurrentMp >= 9700)
        {
            if (Player.Level <= 58)
            {
                if (FireIiPvE.CanUse(out act) || FireIiiPvE.CanUse(out act)) return true;
            }
            
            //fallback because paradox is not always being pressed
            if (IsParadoxActive)
            {
                if (ParadoxPvE.CanUse(out act) || BlizzardPvE.CanUse(out act)) return true;
            }

            if (Player.HasStatus(true, StatusID.Firestarter))
            {
                if (TransposePvE.CanUse(out act) && InCombat) return true;
            }

            if (FireIiPvE.CanUse(out act) || HighFireIiPvE.CanUse(out act) || FireIiiPvE.CanUse(out act)) return true;
        }
        
        if (UmbralIceStacks < 3)
        {
            if (BlizzardIiPvE.CanUse(out act) || UmbralSoulPvE.CanUse(out act) || BlizzardIiiPvE.CanUse(out act)) return true;
        }
        if (Player is { Level: > 58})
        {
            if (FreezePvE.CanUse(out act) || BlizzardIvPvE.CanUse(out act) || UmbralSoulPvE.CanUse(out act) || BlizzardPvE.CanUse(out act)) return true;
        }

        if (ElementTime < 3u)
        {
            if (ParadoxPvE.CanUse(out act) || BlizzardPvE.CanUse(out act)) return true;
        }

        if (UmbralSoulPvE.CanUse(out act)) return true;

        return false;
    }

    private bool FirePhase(out IAction? act)
    {
        act = null;
        if (!InAstralFire) return false;
        // Finisher 
        if (CurrentMp < FireIvPvE.Info.MPNeed + 800)
        {
            if (FlarePvE.CanUse(out act) || DespairPvE.CanUse(out act)) return true;
            
        }

        //Aoe Rotation
        if (UmbralHearts <= 3)
        {
            // if player is level 100 just flare after getting into fire phase
            if (Player.Level == 100)
            {
                if (FlarePvE.CanUse(out act)) return true;
            }

            // use flare once 1 umbral heart is left
            if (Player.Level >= 58 && UmbralHearts == 1)
            {
                if (FlarePvE.CanUse(out act)) return true;
            }
        }
        if ( ElementTime >= FlareStarPvE.Info.CastTime * 1.7 && FlareStarPvE.CanUse(out act)) return true;

        // Fire Rotation
        if (ElementTimeEndAfter((float)(ExtendTimeSafely ? 5.22: 3.22)))
        {
            if (ParadoxPvE.CanUse(out act)) return true;
            if (Player.HasStatus(true, StatusID.Firestarter))
            {
                if (FireIiPvE.CanUse(out act) || HighFireIiPvE.CanUse(out act) || FireIiiPvE.CanUse(out act))
                    return true;
            }

            if (FireIiPvE.CanUse(out act) || HighFireIiPvE.CanUse(out act) || FirePvE.CanUse(out act)) return true;
        }

        if (AstralFireStacks < 3)
        {
            if (Player.HasStatus(true, StatusID.Firestarter))
            {
                if (FireIiPvE.CanUse(out act) || HighFireIiPvE.CanUse(out act) || FireIiiPvE.CanUse(out act))
                    return true;
            }
            else
            {
                if (SwiftcastPvE.CanUse(out act) || TriplecastPvE.CanUse(out act)) return true;
                if (FireIiPvE.CanUse(out act) || HighFireIiPvE.CanUse(out act) || FireIiiPvE.CanUse(out act) ||
                    FirePvE.CanUse(out act))
                    return true;
            }
        }

        if (CurrentMp >= FireIvPvE.Info.MPNeed + 800 && AstralFireStacks >= 3)
        {
            if (FireIiPvE.CanUse(out act) || HighFireIiPvE.CanUse(out act) || FireIvPvE.CanUse(out act) ||
                FirePvE.CanUse(out act))
                return true;
        }

        // Change to Ice Phase
        if (!FlareStarPvE.CanUse(out act))
        {
            if (ManafontPvE.CanUse(out act)) return true;
            if (BlizzardIiPvE.CanUse(out act) || BlizzardIiiPvE.CanUse(out act)) return true;
        }

        return false;
    }

    private bool AddThunder(out IAction? act, uint gcdCount = 3)
    {
        act = null;
        //Return if just used.
        if (IsLastGCD(ActionID.ThunderPvE, ActionID.ThunderIiPvE, ActionID.ThunderIiiPvE, ActionID.ThunderIvPvE))
            return false;

        //So long for thunder.
        if (ThunderPvE.CanUse(out _) && (!ThunderPvE.Target.Target?.WillStatusEndGCD(gcdCount, 2, true,
                                             StatusID.Thunder, StatusID.ThunderIi, StatusID.ThunderIii,
                                             StatusID.ThunderIv, StatusID.HighThunder) ??
                                         false))
            return false;

        return ThunderIiPvE.CanUse(out act) || HighThunderIiPvE.CanUse(out act) || ThunderPvE.CanUse(out act);
    }

    private bool PolyglotDump(out IAction? act)
    {
        act = null;
        if (ElementTimeEndAfterGCD(3u) || HasSwift) return false;
        if ((IsPolyglotStacksMaxed && EnochianEndAfterGCD(2)) ||
            AmplifierPvE.Cooldown.WillHaveOneChargeGCD(1, 2) || IsMoving)
        {
            if (FoulPvE.CanUse(out act) || XenoglossyPvE.CanUse(out act)) return true;
        }
        return false;
    }

    private bool NeedsElement(out IAction? act)
    {
        act = null;
        if (InAstralFire || InUmbralIce || !InCombat) return false;
        if (SwiftcastPvE.CanUse(out act) || TriplecastPvE.CanUse(out act)) return true;
        // fallback if mana is 800mp or less
        if (BlizzardIiPvE.CanUse(out act) || BlizzardIiiPvE.CanUse(out act) || BlizzardPvE.CanUse(out act)) return true;
        // fallback if there is no mana
        return LucidDreamingPvE.CanUse(out act) || ManafontPvE.CanUse(out act);
    }
    
    private bool MaintainStatus(out IAction? act)
    {
        act = null;
        if (HasHostilesInRange) return false;
        if (UmbralSoulPvE.CanUse(out act)) return true;
        if (InAstralFire && TransposePvE.CanUse(out act) && !InCombat) return true;

        return false;
    }

    #endregion
}