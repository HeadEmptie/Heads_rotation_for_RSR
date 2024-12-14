using System.ComponentModel;

namespace RebornRotations.Magical;

[Rotation("Beta_needs_Tweaks", CombatType.PvE, GameVersion = "7.11",
    Description = "Kindly created and donated by Rabbs and further update made by IcWa")]
[SourceCode(Path = "main/BasicRotations/Magical/ICWA_PCT_BETA.cs")]
[Api(4)]
public sealed class IcWaPctBeta : PictomancerRotation
{
    #region Config Options

    public override MedicineType MedicineType => MedicineType.Intelligence;
    public static IBaseAction RainbowPrePull { get; } = new BaseAction((ActionID)34688);

    [RotationConfig(CombatType.PvE, Name = "Use HolyInWhite or CometInBlack while moving")]
    public bool HolyCometMoving { get; set; } = true;

    [RotationConfig(CombatType.PvE,
        Name =
            "Use swifcast on (would advise weapon only - Creature can delay timings and f opener and reopener and landscape doesn't bring any bonus on dps.)")]
    public MotifSwift MotifSwiftCast { get; set; } = MotifSwift.WeaponMotif;

    [Range(1, 5, ConfigUnitType.None, 1)]
    [RotationConfig(CombatType.PvE,
        Name = "Paint overcap protection. How many paint do you need to be at before using a paint?")]
    public bool UseCapCometHoly { get; set; } = true;

    [RotationConfig(CombatType.PvE,
        Name = "Use the paint overcap protection (will still use comet while moving if the setup is on)")]
    public bool UseCapCometOnly { get; set; } = false;

    [RotationConfig(CombatType.PvE,
        Name =
            "Use the paint overcap protection for comet only (will still use comet while moving if the setup is on)")]
    public int HolyCometMax { get; set; } = 5;

    public enum MotifSwift : byte
    {
        [Description("CreatureMotif")] CreatureMotif,
        [Description("WeaponMotif")] WeaponMotif,
        [Description("LandscapeMotif")] LandscapeMotif,
        [Description("AllMotif")] AllMotif,

        [Description("NoMotif(ManualSwifcast")]
        NoMotif
    }

    #endregion

    #region Countdown logic

    // Defines logic for actions to take during the countdown before combat starts.
    protected override IAction? CountDownAction(float remainTime)
    {
        IAction? act;
        if (!InCombat)
        {
            if (!CreatureMotifDrawn)
            {
                if (CreaturePaintings(out act)) return act;
            }

            if (!WeaponMotifDrawn)
            {
                if (HammerMotifPvE.CanUse(out act)) return act;
            }

            if (!LandscapeMotifDrawn)
            {
                if (StarrySkyMotifPvE.CanUse(out act) && !Player.HasStatus(true, StatusID.Hyperphantasia)) return act;
            }
        }

        if (remainTime < RainbowDripPvE.Info.CastTime + CountDownAhead)
        {
            if (StrikingMusePvE.CanUse(out act, skipCastingCheck: true, skipStatusProvideCheck: true,
                    skipComboCheck: true, skipAoeCheck: true, usedUp: true) && WeaponMotifDrawn) return act;
        }

        if (remainTime < RainbowDripPvE.Info.CastTime + 0.4f + CountDownAhead)
        {
            if (RainbowPrePull.CanUse(out act, skipAoeCheck: true, skipCastingCheck: true,
                    skipStatusProvideCheck: true)) return act;
        }

        if (remainTime < FireInRedPvE.Info.CastTime + CountDownAhead && Player.Level < 92)
        {
            if (FireInRedPvE.CanUse(out act, skipAoeCheck: true, skipCastingCheck: true,
                    skipStatusProvideCheck: true)) return act;
        }

        return base.CountDownAction(remainTime);
    }

    #endregion

    #region Additional oGCD Logic

    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        if (InCombat)
        {
            switch (MotifSwiftCast)
            {
                case MotifSwift.CreatureMotif:
                    if (nextGCD == PomMotifPvE || nextGCD == WingMotifPvE || nextGCD == MawMotifPvE ||
                        nextGCD == ClawMotifPvE)
                    {
                        if (SwiftcastPvE.CanUse(out act)) return true;
                    }

                    break;
                case MotifSwift.WeaponMotif:
                    if (nextGCD == HammerMotifPvE)
                    {
                        if (SwiftcastPvE.CanUse(out act)) return true;
                    }

                    break;
                case MotifSwift.LandscapeMotif:
                    if (nextGCD == StarrySkyMotifPvE)
                    {
                        if (SwiftcastPvE.CanUse(out act)) return true;
                    }

                    break;
                case MotifSwift.AllMotif:
                    if (nextGCD == PomMotifPvE || nextGCD == WingMotifPvE || nextGCD == MawMotifPvE ||
                        nextGCD == ClawMotifPvE)
                    {
                        if (SwiftcastPvE.CanUse(out act)) return true;
                    }
                    else if (nextGCD == HammerMotifPvE || nextGCD == StarrySkyMotifPvE)
                        if (SwiftcastPvE.CanUse(out act)) return true;
                    

                    break;
                case MotifSwift.NoMotif:
                    break;
            }
        }

        return base.EmergencyAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.SmudgePvE)]
    protected override bool MoveForwardAbility(IAction nextGCD, out IAction? act)
    {
        if (SmudgePvE.CanUse(out act)) return true;
        return base.AttackAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.AddlePvE, ActionID.TemperaCoatPvE, ActionID.TemperaGrassaPvE)]
    protected override bool DefenseAreaAbility(IAction nextGCD, out IAction? act)
    {
        if (AddlePvE.CanUse(out act)) return true;
        if (TemperaCoatPvE.CanUse(out act)) return true;
        if (TemperaGrassaPvE.CanUse(out act)) return true;
        return base.DefenseAreaAbility(nextGCD, out act);
    }

    #endregion

    #region oGCD Logic

    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        if (Player.HasStatus(true, StatusID.StarryMuse))
        {
            if (FangedMusePvE.CanUse(out act, skipCastingCheck: true, skipStatusProvideCheck: true,
                    skipComboCheck: true, skipAoeCheck: true, usedUp: true)) return true;
            if (RetributionOfTheMadeenPvE.CanUse(out act, skipCastingCheck: true, skipStatusProvideCheck: true,
                    skipComboCheck: true, skipAoeCheck: true, usedUp: true)) return true;
        }

        bool burstTimingCheckerStriking =
            !ScenicMusePvE.Cooldown.WillHaveOneCharge(60) || Player.HasStatus(true, StatusID.StarryMuse);
        int adjustCombatTimeForOpener = Player.Level < 92 ? 2 : 5;
        if (StarryMusePvE.CanUse(out act, skipCastingCheck: true, skipStatusProvideCheck: true, skipComboCheck: true,
                skipAoeCheck: true, usedUp: true) && CombatTime > adjustCombatTimeForOpener && IsBurst) return true;
        if (CombatTime > adjustCombatTimeForOpener && StrikingMusePvE.CanUse(out act, skipCastingCheck: true,
                skipStatusProvideCheck: true, skipComboCheck: true, skipAoeCheck: true, usedUp: true) &&
            burstTimingCheckerStriking) return true;
        if (SubtractivePalettePvE.CanUse(out act) && !Player.HasStatus(true, StatusID.SubtractivePalette)) return true;
        
        
        
        if (Player.HasStatus(true, StatusID.StarryMuse)
            || (BurstMuses(out act) &&
                LivingMusePvE.Cooldown.WillHaveXCharges((uint)LivingMusePvE.Cooldown.MaxCharges - 1,Player.BaseCastTime * 3)))
        {
            if (MogOfTheAgesPvE.CanUse(out act)) return true;
            if (RetributionOfTheMadeenPvE.CanUse(out act, skipCastingCheck: true, skipStatusProvideCheck: true,
                    skipComboCheck: true, skipAoeCheck: true, usedUp: true)) return true;
            if (AllMuses(out act)) return true;
        }

        if (LivingMusePvE.Cooldown.WillHaveXCharges(
                (uint)((LivingMusePvE.Cooldown.MaxCharges - 1) == 0 ? 1 : LivingMusePvE.Cooldown.MaxCharges - 1),
                Player.BaseCastTime * 3) || Player.HasStatus(true, StatusID.StarryMuse))
        {
            if (AllMuses(out act)) return true;
        }
        
        if (ScenicMusePvE.Cooldown.RecastTimeRemainOneCharge > 60 || ScenicMusePvE.Cooldown.WillHaveXChargesGCD(ScenicMusePvE.Cooldown.MaxCharges, 1, 0))
        {
            if (StrikingMusePvE.CanUse(out act, skipCastingCheck: true, skipStatusProvideCheck: true,
                    skipComboCheck: true,
                    skipAoeCheck: true, usedUp: true)) return true;
        }
        
        
        return base.AttackAbility(nextGCD, out act);
    }

    #endregion

    #region GCD Logic

    protected override bool GeneralGCD(out IAction? act)
    {
        //Opener requirements
        if (CombatTime < 5)
        {
            if (HolyInWhitePvE.CanUse(out act, skipCastingCheck: true, skipAoeCheck: true) && Paint > 0) return true;
            if (CreaturePaintings(out act)) return true;
        }

        // some gcd priority
        if (RainbowDripPvE.CanUse(out act, skipAoeCheck: true) &&
            Player.HasStatus(true, StatusID.RainbowBright)) return true;
        if (Player.HasStatus(true, StatusID.StarryMuse))
        {
            if (CometInBlackPvE.CanUse(out act, skipCastingCheck: true, skipAoeCheck: true) && Paint > 0) return true;
        }

        if (StarPrismPvE.CanUse(out act, skipAoeCheck: true) && Player.HasStatus(true, StatusID.Starstruck))
            return true;
        
        if ( NumberOfHostilesInRange >= 2 || IsMoving || SteelMusePvE.Cooldown.WillHaveXCharges(2, Player.CurrentCastTime * 5) ||
            Player.HasStatus(true, StatusID.StarryMuse) || Player.WillStatusEndGCD((uint)HammerStacks + 1,  0 ,true, StatusID.HammerTime))
        {
            if (PolishingHammerPvE.CanUse(out act, skipComboCheck: true) ||
                HammerBrushPvE.CanUse(out act, skipComboCheck: true) ||
                HammerStampPvE.CanUse(out act, skipComboCheck: true))
                return true;
        }

        //Cast when not in fight or no target available
        if (NotInCombat(out act)) return true;
        

        // timings for motif casting
        if (ScenicMusePvE.Cooldown.RecastTimeRemainOneCharge <= 15 && !Player.HasStatus(true, StatusID.StarryMuse) &&
            !Player.HasStatus(true, StatusID.Hyperphantasia))
        {
            if (StarrySkyMotifPvE.CanUse(out act) && !Player.HasStatus(true, StatusID.Hyperphantasia)) return true;
        }

        if ((LivingMusePvE.Cooldown.HasOneCharge ||
             LivingMusePvE.Cooldown.RecastTimeRemainOneCharge <= CreatureMotifPvE.Info.CastTime * 1.7) &&
            !Player.HasStatus(true, StatusID.StarryMuse) && !Player.HasStatus(true, StatusID.Hyperphantasia))
        {
            if (CreaturePaintings(out act)) return true;
        }

        if ((SteelMusePvE.Cooldown.HasOneCharge ||
             SteelMusePvE.Cooldown.RecastTimeRemainOneCharge <= WeaponMotifPvE.Info.CastTime) &&
            !Player.HasStatus(true, StatusID.StarryMuse) && !Player.HasStatus(true, StatusID.Hyperphantasia))
        {
            if (HammerMotifPvE.CanUse(out act)) return true;
        }

        bool isMovingAndSwift = IsMoving && !Player.HasStatus(true, StatusID.Swiftcast);
        // white/black paint use while moving
        if (isMovingAndSwift)
        {
            if (PolishingHammerPvE.CanUse(out act, skipComboCheck: true)) return true;
            if (HammerBrushPvE.CanUse(out act, skipComboCheck: true)) return true;
            if (HammerStampPvE.CanUse(out act, skipComboCheck: true)) return true;
            if (HolyCometMoving)
            {
                if (CometInBlackPvE.CanUse(out act, skipCastingCheck: true, skipAoeCheck: true)) return true;
                if (HolyInWhitePvE.CanUse(out act, skipCastingCheck: true, skipAoeCheck: true)) return true;
            }
        }

        // When in swift management
        if (WhenInSwift(out act)) return true;

        //white paint over cap protection
        if ((Paint == HolyCometMax && !Player.HasStatus(true, StatusID.StarryMuse)) &&
            (UseCapCometHoly || UseCapCometOnly))
        {
            if (CometInBlackPvE.CanUse(out act, skipCastingCheck: true, skipAoeCheck: true)) return true;
            if (HolyInWhitePvE.CanUse(out act, skipCastingCheck: true, skipAoeCheck: true) &&
                !UseCapCometOnly) return true;
        }

        //aoe sub
        if (ThunderIiInMagentaPvE.CanUse(out act) || StoneIiInYellowPvE.CanUse(out act) || BlizzardIiInCyanPvE.CanUse(out act)) return true;
        //aoe normal
        if (WaterIiInBluePvE.CanUse(out act) || AeroIiInGreenPvE.CanUse(out act) || FireIiInRedPvE.CanUse(out act)) return true;
        //single target sub
        if (ThunderInMagentaPvE.CanUse(out act) || StoneInYellowPvE.CanUse(out act) || BlizzardInCyanPvE.CanUse(out act)) return true;
        //single target normal
        if (WaterInBluePvE.CanUse(out act) || AeroInGreenPvE.CanUse(out act) || FireInRedPvE.CanUse(out act)) return true;
        
        return base.GeneralGCD(out act);
    }

    private bool NotInCombat(out IAction? act)
    {
        act = null;
        if (InCombat || !HasHostilesInRange) return false;
        if (CreaturePaintings(out act)) return true;
        if (HammerMotifPvE.CanUse(out act)) return true;
        if (StarrySkyMotifPvE.CanUse(out act) && !Player.HasStatus(true, StatusID.Hyperphantasia)) return true;
        if (RainbowDripPvE.CanUse(out act)) return true;

        return false;
    }

    private bool CreaturePaintings(out IAction? act)
    {
        act = null;
        if (PomMotifPvE.CanUse(out act) && CreatureMotifPvE.AdjustedID == PomMotifPvE.ID) return true;
        if (WingMotifPvE.CanUse(out act) && CreatureMotifPvE.AdjustedID == WingMotifPvE.ID) return true;
        if (ClawMotifPvE.CanUse(out act) && CreatureMotifPvE.AdjustedID == ClawMotifPvE.ID) return true;
        if (MawMotifPvE.CanUse(out act) && CreatureMotifPvE.AdjustedID == MawMotifPvE.ID) return true;
        return false;
    }

    private bool WhenInSwift(out IAction? act)
    {
        act = null;

        if (!Player.HasStatus(true, StatusID.Swiftcast) ||
            (LandscapeMotifDrawn && CreatureMotifDrawn && WeaponMotifDrawn)) return false;
        
        bool creature = MotifSwiftCast is MotifSwift.CreatureMotif or MotifSwift.AllMotif;
        bool weapon = MotifSwiftCast is MotifSwift.WeaponMotif or MotifSwift.AllMotif;
        bool landscape = MotifSwiftCast is MotifSwift.LandscapeMotif or MotifSwift.AllMotif;
        if (PomMotifPvE.CanUse(out act, skipCastingCheck: creature) &&
            CreatureMotifPvE.AdjustedID == PomMotifPvE.ID && creature) return true;
        if (WingMotifPvE.CanUse(out act, skipCastingCheck: creature) &&
            CreatureMotifPvE.AdjustedID == WingMotifPvE.ID && creature) return true;
        if (ClawMotifPvE.CanUse(out act, skipCastingCheck: creature) &&
            CreatureMotifPvE.AdjustedID == ClawMotifPvE.ID && creature) return true;
        if (MawMotifPvE.CanUse(out act, skipCastingCheck: creature) &&
            CreatureMotifPvE.AdjustedID == MawMotifPvE.ID && creature) return true;
        if (HammerMotifPvE.CanUse(out act, skipCastingCheck: weapon) && weapon) return true;
        if (StarrySkyMotifPvE.CanUse(out act, skipCastingCheck: landscape) &&
            !Player.HasStatus(true, StatusID.Hyperphantasia) && landscape) return true;

        return false;
    }

    private bool BurstMuses(out IAction? act)
    {
        act = null;
        if (WingedMusePvE.CanUse(out act, skipCastingCheck: true, skipStatusProvideCheck: true, skipComboCheck: true,
                skipAoeCheck: true, usedUp: true) && LivingMusePvE.AdjustedID == WingedMusePvE.ID) return true;
        if (FangedMusePvE.CanUse(out act, skipCastingCheck: true, skipStatusProvideCheck: true, skipComboCheck: true,
                skipAoeCheck: true, usedUp: true) && LivingMusePvE.AdjustedID == FangedMusePvE.ID) return true;
        return false;
    }

    private bool AllMuses(out IAction? act)
    {
        act = null;
        if (PomMusePvE.CanUse(out act, skipCastingCheck: true, skipStatusProvideCheck: true, skipComboCheck: true,
                skipAoeCheck: true, usedUp: true) && LivingMusePvE.AdjustedID == PomMusePvE.ID) return true;
        if (ClawedMusePvE.CanUse(out act, skipCastingCheck: true, skipStatusProvideCheck: true, skipComboCheck: true,
                skipAoeCheck: true, usedUp: true) && LivingMusePvE.AdjustedID == ClawedMusePvE.ID) return true;
        BurstMuses(out act);
        return false;
    }
    #endregion
}