using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKHeX.WinForms.Misc
{
    internal class GenericRng
    {
        //  This is the generic base that all of the other lcrngs will
        //  use. They will pass in a multiplier, and adder, and a seed.
        protected uint add;
        protected uint mult;

        public GenericRng(uint seed, uint mult, uint add)
        {
            Seed = seed;

            this.mult = mult;
            this.add = add;
        }

        public uint Seed { get; set; }

        public uint GetNext16BitNumber()
        {
            return GetNext32BitNumber() >> 16;
        }

        public virtual uint GetNext32BitNumber()
        {
            Seed = Seed * mult + add;

            return Seed;
        }

        public void GetNext32BitNumber(int num)
        {
            for (int i = 0; i < num; i++)
                Seed = Seed * mult + add;
        }
    }

    internal class PokeRng : GenericRng
    {
        public PokeRng(uint seed)
            : base(seed, 0x41c64e6d, 0x6073)
        {
        }
    }

    internal class PokeRngR : GenericRng
    {
        public PokeRngR(uint seed)
            : base(seed, 0xeeb9eb65, 0xa3561a1)
        {
        }
    }

    internal class XdRng : GenericRng
    {
        public XdRng(uint seed)
            : base(seed, 0x343FD, 0x269EC3)
        {
        }
    }

    internal class XdRngR : GenericRng
    {
        public XdRngR(uint seed)
            : base(seed, 0xB9B33155, 0xA170F641)
        {
        }
    }

    public enum FrameType
    {
        Method1Reverse,
        Method2,
        ColoXD,
        Channel
    };

    internal class Seed
    {
        //  Needs to hold all of the information about 
        //  a seed that we have created from an IV and
        //  nature combo.

        //  Need to come up with a better name for this, as it
        //  cant seem to have the same name as the containing 
        //  class :P
        public uint MonsterSeed { get; set; }

        public uint Pid { get; set; }

        public string Method { get; set; }

        public uint Sid { get; set; }
    }

    internal enum CompareType
    {
        None,
        Equal,
        GtEqual,
        LtEqual,
        NotEqual,
        Even,
        Odd,
        Hidden,
        HiddenEven,
        HiddenOdd,
        HiddenTrickRoom
    };

    internal enum GenderCriteria
    {
        DontCareGenderless,
        Male,
        Female
    }

    public enum EncounterMod
    {
        None,
        Search,
        Synchronize,
        Compoundeyes,
        SuctionCups,
        CuteCharm,
        Everstone,
        CuteCharm50M,
        CuteCharm75M,
        CuteCharm25M,
        CuteCharm875M,
        CuteCharm50F,
        CuteCharm75F,
        CuteCharm25F,
        CuteCharm125F,
        CuteCharmFemale
    };

    internal class GenderFilter
    {

        public string Name { get; protected set; }

        public uint GenderValue { get; protected set; }

        public GenderCriteria GenderCriteria { get; protected set; }

        public override string ToString()
        {
            return Name;
        }

        public bool Filter(uint genderValue)
        {
            if (GenderCriteria == GenderCriteria.Male)
            {
                return genderValue >= GenderValue;
            }

            if (GenderCriteria == GenderCriteria.Female)
            {
                return genderValue < GenderValue;
            }

            return true;
        }

        public bool Filter(EncounterMod encounterMod)
        {
            switch (encounterMod)
            {
                case EncounterMod.CuteCharmFemale:
                    return GenderCriteria != GenderCriteria.Male && GenderValue != 0 && GenderValue != 254;
                case EncounterMod.CuteCharm875M:
                    return GenderValue == 255 || GenderValue == 31;
                case EncounterMod.CuteCharm50M:
                    return GenderValue == 255 || GenderValue == 127;
                case EncounterMod.CuteCharm75M:
                    return GenderValue == 255 || GenderValue == 63;
                case EncounterMod.CuteCharm25M:
                    return GenderValue == 255 || GenderValue == 191;
                case EncounterMod.CuteCharm125F:
                    return GenderValue == 255 || GenderValue == 31;
                case EncounterMod.CuteCharm50F:
                    return GenderValue == 255 || GenderValue == 127;
                case EncounterMod.CuteCharm75F:
                    return GenderValue == 255 || GenderValue == 191;
                case EncounterMod.CuteCharm25F:
                    return GenderValue == 255 || GenderValue == 63;
                default:
                    return true;
            }
        }
    }

    internal class NoGenderFilter : GenderFilter
    {
        public NoGenderFilter()
        {
        }

        public NoGenderFilter(string name)
        {
            Name = name;
        }

        public new bool Filter(uint genderValue)
        {
            return true;
        }
    }

    internal class IVFilter
    {
        public CompareType atkCompare;
        public uint atkValue;
        public CompareType defCompare;
        public uint defValue;
        public CompareType hpCompare;
        public uint hpValue;
        public CompareType spaCompare;
        public uint spaValue;
        public CompareType spdCompare;
        public uint spdValue;
        public CompareType speCompare;
        public uint speValue;

        public IVFilter(uint hpValue, CompareType hpCompare, uint atkValue, CompareType atkCompare, uint defValue,
                        CompareType defCompare, uint spaValue, CompareType spaCompare, uint spdValue,
                        CompareType spdCompare, uint speValue, CompareType speCompare)
        {
            this.hpValue = hpValue;
            this.hpCompare = hpCompare;
            this.atkValue = atkValue;
            this.atkCompare = atkCompare;
            this.defValue = defValue;
            this.defCompare = defCompare;
            this.spaValue = spaValue;
            this.spaCompare = spaCompare;
            this.spdValue = spdValue;
            this.spdCompare = spdCompare;
            this.speValue = speValue;
            this.speCompare = speCompare;
        }

        public IVFilter()
        {
            hpValue = 0;
            hpCompare = CompareType.None;
            atkValue = 0;
            atkCompare = CompareType.None;
            defValue = 0;
            defCompare = CompareType.None;
            spaValue = 0;
            spaCompare = CompareType.None;
            spdValue = 0;
            spdCompare = CompareType.None;
            speValue = 0;
            speCompare = CompareType.None;
        }
    }

    public enum EncounterType
    {
        Wild,
        WildSurfing,
        WildOldRod,
        WildGoodRod,
        WildSuperRod,
        WildWaterSpot,
        WildSwarm,
        WildShakerGrass,
        WildCaveSpot,
        Stationary,
        Roamer,
        Gift,
        Entralink,
        LarvestaEgg,
        AllEncounterShiny,
        BugCatchingContest,
        BugCatchingContestPreDex,
        BugBugCatchingContestTues,
        BugCatchingContestThurs,
        BugCatchingContestSat,
        SafariZone,
        Headbutt,
        Manaphy,
        HiddenGrotto
    };

    public class Frame
    {
        private static readonly uint[] HABCDS = { 0, 1, 2, 5, 3, 4 };
        private static readonly uint[] ABCDS = { 1, 2, 5, 3, 4 };
        private static readonly uint[] ACDS = { 1, 5, 3, 4 };

        /// <summary>
        ///     1 or 2 for the ability number, best we can do since we don't know what the monster is actually going to be.
        /// </summary>
        private uint ability;

        private uint cGearTime;
        private uint[] characteristicIVs;
        private uint coin;
        private bool dreamAbility;
        private uint dv;
        private uint id;
        private int maleOnlySpecies;
        private uint number;
        private uint pid;
        private uint seed;
        private uint sid;
        private bool synchable;

        public Frame()
        {
            Shiny = false;
            EncounterMod = EncounterMod.None;
            Offset = 0;
        }

        internal Frame(FrameType frameType)
        {
            Shiny = false;
            EncounterMod = EncounterMod.None;
            Offset = 0;
            FrameType = frameType;
        }

        public uint RngResult { get; set; }

        public uint MaxSkips { get; set; }

        public uint Seed
        {
            get { return seed; }
            set { seed = value; }
        }

        public uint Number
        {
            get { return number; }
            set { number = value; }
        }

        public uint Offset { get; set; }

        public string Time
        {
            get
            {
                uint minutes = number / 3600;
                uint seconds = (number - (3600 * minutes)) / 60;
                uint milli = ((number % 60) * 100) / 60;

                return minutes.ToString() + ":" + seconds.ToString("D2") + "." + milli.ToString("D2");
            }
        }

        public uint CGearTime
        {
            get { return cGearTime; }
            set { cGearTime = value; }
        }

        public string EntralinkTime
        {
            get
            {
                if (cGearTime == 0)
                    return "skip";
                uint minutes = cGearTime / 3600;
                uint seconds = (cGearTime - (3600 * minutes)) / 60;
                uint milli = ((cGearTime % 60) * 100) / 60;

                return minutes.ToString() + ":" + seconds.ToString("D2") + "." + milli.ToString("D2");
            }
        }

        public bool DreamAbility
        {
            get { return dreamAbility; }
            set { dreamAbility = value; }
        }

        public bool Synchable
        {
            get { return synchable; }
            set { synchable = value; }
        }

        public FrameType FrameType { get; set; }

        public EncounterMod EncounterMod { get; set; }

        public EncounterType EncounterType { get; set; }

        public uint ItemCalc { get; set; }

        //  ID's used for checking if we have a shiny.  If these are
        //  zero'd we will not actually do the check.

        public bool Shiny { get; private set; }

        /// <summary>
        ///     Display member called by the grid control. Saves us from having to actually do anything but return string and not actually store it.
        /// </summary>
        public string ShinyDisplay
        {
            get { return Shiny ? "!!!" : ""; }
        }

        //  The following are cacluated differently based
        //  on the creation method of the pokemon. 

        public uint Pid
        {
            get { return pid; }
            set
            {
                Nature = (value % 25);
                ability = (value & 1);
                coin = (value & 1);

                //  figure out if we are shiny here
                uint tid = (id & 0xffff) | ((sid & 0xffff) << 16);
                uint a = value ^ tid;
                uint b = a & 0xffff;
                uint c = (a >> 16);
                uint d = b ^ c;
                if (d < 8)
                    Shiny = true;

                pid = value;
            }
        }

        public uint Advances { get; set; }

        public uint Dv
        {
            get { return dv; }
            set
            {
                //  Split up our DV
                var dv1 = (ushort)value;
                var dv2 = (ushort)(value >> 16);

                //  Get the actual Values
                Hp = (uint)dv1 & 0x1f;
                Atk = ((uint)dv1 & 0x3E0) >> 5;
                Def = ((uint)dv1 & 0x7C00) >> 10;

                Spe = (uint)dv2 & 0x1f;
                Spa = ((uint)dv2 & 0x3E0) >> 5;
                Spd = ((uint)dv2 & 0x7C00) >> 10;

                //  Set the actual dv value
                dv = value;
            }
        }

        public uint Ability
        {
            get { return ability; }
            set { ability = value; }
        }

        public string Coin
        {
            get { return coin == 0 ? "Tails" : "Heads"; }
        }

        public uint Nature { get; set; }

        public uint Hp { get; set; }

        public uint Atk { get; set; }

        public uint Def { get; set; }

        public uint Spa { get; set; }

        public uint Spd { get; set; }

        public uint Spe { get; set; }

        public string DisplayHp { get; set; }

        public string DisplayAtk { get; set; }

        public string DisplayDef { get; set; }

        public string DisplaySpa { get; set; }

        public string DisplaySpd { get; set; }

        public string DisplaySpe { get; set; }

        // this set of displays is used to display egg IVs
        // instead of labels like "A", "B", "Fe", "Ma"
        public string DisplayHpAlt { get; set; }

        public string DisplayAtkAlt { get; set; }

        public string DisplayDefAlt { get; set; }

        public string DisplaySpaAlt { get; set; }

        public string DisplaySpdAlt { get; set; }

        public string DisplaySpeAlt { get; set; }

        //  Hidden power information that we calculate at the same
        //  we build the DVs.

        public uint HiddenPowerPower
        {
            get
            {
                uint p_hp = (Hp & 3) > 1u ? 1u : 0u;
                uint p_atk = (Atk & 3) > 1u ? 2u : 0u;
                uint p_def = (Def & 3) > 1u ? 4u : 0u;
                uint p_spe = (Spe & 3) > 1u ? 8u : 0u;
                uint p_spa = (Spa & 3) > 1u ? 16u : 0u;
                uint p_spd = (Spd & 3) > 1u ? 32u : 0u;

                uint hp_power = (p_hp + p_atk + p_def + p_spe + p_spa + p_spd) * 40 / 63 + 30;

                return hp_power;
            }
        }

        public uint GenderValue
        {
            get { return pid & 0xFF; }
        }

        public string Female50
        {
            get { return ((pid & 0xFF) >= 127) ? "M" : "F"; }
        }

        public string Female125
        {
            get { return ((pid & 0xFF) >= 31) ? "M" : "F"; }
        }

        public string Female25
        {
            get { return ((pid & 0xFF) >= 63) ? "M" : "F"; }
        }

        public string Female75
        {
            get { return ((pid & 0xFF) >= 191) ? "M" : "F"; }
        }

        public int MaleOnlySpecies
        {
            get { return maleOnlySpecies; }
            set { maleOnlySpecies = value; }
        }

        public string MaleOnly
        {
            get { return maleOnlySpecies == 0 ? "Nidoran-F\\Volbeat" : "Nidoran-M\\Illumise"; }
        }

        public string seedTime { get; set; }

        public int EncounterSlot { get; set; }

        public bool ShakingSpotPossible
        {
            get { return (RngResult >> 28) == 0; }
        }

        public uint[] CharacteristicIVs
        {
            get { return characteristicIVs; }
            set { characteristicIVs = value; }
        }

        /// <summary>
        ///     Generic Frame creation where the values that are to be used for each part are passed in explicitly. There will be other methods to support splitting a list and then passing them to this for creation.
        /// </summary>
        public static Frame GenerateFrame(
            uint seed,
            FrameType frameType,
            uint number,
            uint rngResult,
            uint pid1,
            uint pid2,
            uint dv1,
            uint dv2,
            uint id,
            uint sid,
            uint offset)
        {
            var frame = new Frame(frameType)
            {
                Seed = seed,
                Number = number,
                RngResult = rngResult,
                Offset = offset,
                id = id,
                sid = sid,
                Pid = (pid2 << 16) | pid1,
                Dv = (dv2 << 16) | dv1
            };


            //  Set up the ID and SID before we calculate 
            //  the pid, as we are going to need this.


            return frame;
        }

        // for Methods 1, 2, 4
        public static Frame GenerateFrame(
            uint seed,
            FrameType frameType,
            uint number,
            uint rngResult,
            uint pid1,
            uint pid2,
            uint dv1,
            uint dv2,
            uint id,
            uint sid)
        {
            var frame = new Frame(frameType)
            {
                seed = seed,
                number = number,
                RngResult = rngResult,
                id = id,
                sid = sid,
                Pid = (pid2 << 16) | pid1,
                Dv = (dv2 << 16) | dv1
            };


            //  Set up the ID and SID before we calculate 
            //  the pid, as we are going to need this.


            return frame;
        }
    }

    internal class FrameCompare
    {
        private readonly int ability;
        private readonly CompareType atkCompare;
        private readonly uint atkValue;
        private readonly CompareType defCompare;
        private readonly uint defValue;

        //  Other Values -----------------------------------
        private readonly bool dreamWorld;
        private readonly List<int> encounterSlots;
        private readonly CompareType hpCompare;
        private readonly uint hpValue;
        private readonly bool shinyOnly;
        private readonly CompareType spaCompare;
        private readonly uint spaValue;
        private readonly CompareType spdCompare;
        private readonly uint spdValue;
        private readonly CompareType speCompare;
        private readonly uint speValue;
        private readonly bool synchOnly;

        // We're making this public
        // So they can be accessed when calculating Entralink PIDs

        public FrameCompare(IVFilter ivBase,
                            List<uint> natures,
                            int ability,
                            bool shinyOnly,
                            bool synchOnly,
                            bool dreamWorld,
                            List<int> encounterSlots,
                            GenderFilter genderFilter)
        {
            if (ivBase != null)
            {
                hpValue = ivBase.hpValue;
                hpCompare = ivBase.hpCompare;
                atkValue = ivBase.atkValue;
                atkCompare = ivBase.atkCompare;
                defValue = ivBase.defValue;
                defCompare = ivBase.defCompare;
                spaValue = ivBase.spaValue;
                spaCompare = ivBase.spaCompare;
                spdValue = ivBase.spdValue;
                spdCompare = ivBase.spdCompare;
                speValue = ivBase.speValue;
                speCompare = ivBase.speCompare;
            }
            Natures = natures;
            this.ability = ability;
            this.shinyOnly = shinyOnly;
            this.synchOnly = synchOnly;
            this.dreamWorld = dreamWorld;
            this.encounterSlots = encounterSlots;

            GenderFilter = genderFilter;
        }

        public GenderFilter GenderFilter { get; private set; }

        public List<uint> Natures { get; private set; }

        public bool Compare(Frame frame)
        {
            //  Check the nature first
            if (Natures != null)
            {
                // If the frame can be synchronized, it doesn't need to pass the check
                if (frame.EncounterMod != EncounterMod.Synchronize)
                {
                    bool test = Natures.Any(nature => nature == frame.Nature);
                    if (!test)
                        return false;
                }
            }

            if (synchOnly)
            {
                if (!frame.Synchable)
                {
                    return false;
                }
            }

            if (!GenderFilter.Filter(frame.GenderValue))
                return false;

            if (!GenderFilter.Filter(frame.EncounterMod))
                return false;

            if (shinyOnly)
            {
                if (!frame.Shiny)
                {
                    return false;
                }
            }

            if (dreamWorld)
            {
                if (!frame.DreamAbility)
                {
                    return false;
                }
            }

            if (encounterSlots != null)
            {
                bool test = false;
                foreach (int slot in encounterSlots)
                {
                    if (slot == frame.EncounterSlot)
                    {
                        test = true;
                        break;
                    }
                }

                if (!test)
                    return false;
            }

            if (ability != -1)
            {
                if ((ability == 0) && (frame.Ability != 0))
                    return false;

                if ((ability == 1) && (frame.Ability != 1))
                    return false;
            }

            return true;
        }
    }

    internal class FrameGenerator
    {
        protected Frame frame;
        protected FrameType frameType = FrameType.ColoXD;
        protected List<Frame> frames;
        private uint lastseed;
        protected uint maxResults;
        protected List<uint> rngList;

        public FrameGenerator()
        {
            maxResults = 1000000;
            Compatibility = 20;
            InitialFrame = 1;
            InitialSeed = 0;
            SynchNature = -1;
            EncounterMod = EncounterMod.None;
        }

        public FrameType FrameType
        {
            get { return frameType; }
            set
            {
                frameType = value;
            }
        }

        public EncounterMod EncounterMod { get; set; }

        public int SynchNature { get; set; }

        public ulong InitialSeed { get; set; }

        public uint InitialFrame { get; set; }

        public uint Compatibility { get; set; }

        // by declaring these only once you get a huge performance boost

        // This method ensures that an RNG is only created once,
        // and not every time a Generate function is called

        public List<Frame> Generate(
            FrameCompare frameCompare,
            uint id,
            uint sid)
        {
            frames = new List<Frame>();

            if (frameType == FrameType.ColoXD)
            {
                var rng = new XdRng((uint)InitialSeed);
                rngList = new List<uint>();

                for (uint cnt = 1; cnt < InitialFrame; cnt++)
                    rng.GetNext32BitNumber();

                for (uint cnt = 0; cnt < 5; cnt++)
                    rngList.Add(rng.GetNext16BitNumber());

                for (uint cnt = 0; cnt < maxResults; cnt++, rngList.RemoveAt(0), rngList.Add(rng.GetNext16BitNumber()))
                {
                    frame = Frame.GenerateFrame(
                        0,
                        FrameType.ColoXD,
                        cnt + InitialFrame,
                        rngList[0],
                        rngList[4],
                        rngList[3],
                        rngList[0],
                        rngList[1],
                        id, sid);


                    if (frameCompare.Compare(frame))
                    {
                        frames.Add(frame); break;
                    }
                }
            }
            else
            {
                //  We are going to grab our initial set of rngs here and
                //  then start our loop so that we can iterate as many 
                //  times as we have to.
                var rng = new PokeRng((uint)InitialSeed);
                rngList = new List<uint>();

                for (uint cnt = 1; cnt < InitialFrame; cnt++)
                    rng.GetNext32BitNumber();

                for (uint cnt = 0; cnt < 20; cnt++)
                    rngList.Add(rng.GetNext16BitNumber());

                lastseed = rng.Seed;

                for (uint cnt = 0; cnt < maxResults; cnt++, rngList.RemoveAt(0), rngList.Add(rng.GetNext16BitNumber()))
                {
                    switch (frameType)
                    {
                        case FrameType.Method1Reverse:
                            frame =
                                Frame.GenerateFrame(
                                    0,
                                    FrameType.Method1Reverse,
                                    cnt + InitialFrame,
                                    rngList[0],
                                    rngList[1],
                                    rngList[0],
                                    rngList[2],
                                    rngList[3],
                                    id, sid, cnt);

                            break;

                        case FrameType.Method2:
                            frame =
                                Frame.GenerateFrame(
                                    0,
                                    FrameType.Method2,
                                    cnt + InitialFrame,
                                    rngList[0],
                                    rngList[0],
                                    rngList[1],
                                    rngList[3],
                                    rngList[4],
                                    id, sid, cnt);

                            break;
                    }


                    //  Now we need to filter and decide if we are going
                    //  to add this to our collection for display to the
                    //  user.

                    if (frameCompare.Compare(frame))
                    {
                        frames.Add(frame); break;
                    }
                }
            }

            return frames;
        }
    }

    /// <summary>
    ///     This class is going to do an IV/PID/Seed calculation given a particular method (1, 2 or 3, or 4). Should use the same code to develop candidate IVs.
    /// </summary>
    internal class IVtoSeed
    {
        //  We need a function to return a list of monster seeds,
        //  which will be updated to include a method.

        public static List<Seed> GetSeeds(
            uint hp,
            uint atk,
            uint def,
            uint spa,
            uint spd,
            uint spe,
            uint nature,
            uint id)
        {
            var seeds = new List<Seed>();

            uint ivs2 = spe | (spa << 5) | (spd << 10);
            uint ivs2_2 = ivs2 ^ 0x8000;
            uint ivs1 = hp | (atk << 5) | (def << 10);
            uint ivs1_2 = ivs1 ^ 0x8000;

            uint x_test = ivs2 << 16;
            uint x_testXD = ivs1 << 16;
            uint pid, pidXor, sid;

            //  Now we want to start with IV2 and call the RNG for
            //  values between 0 and FFFF in the low order bits.
            for (uint cnt = 0; cnt <= 0xFFFF; cnt++)
            {
                //Check to see if the iv calls line up
                uint seedXD = x_testXD | cnt;
                var rngXD = new XdRng(seedXD);
                var rngXDR = new XdRngR(seedXD);
                uint rng1XD = rngXD.GetNext16BitNumber();

                if (rng1XD == ivs2 || rng1XD == ivs2_2)
                {
                    //Grab rest of RNG calls for XDColo
                    uint rng2XD = rngXD.GetNext16BitNumber();
                    uint rng3XD = rngXD.GetNext16BitNumber();
                    uint rng4XD = rngXD.GetNext16BitNumber();
                    uint XDColoSeed = rngXDR.GetNext32BitNumber();
                    uint XDColoSeedXor = XDColoSeed ^ 0x80000000;
                    sid = (rng4XD ^ rng3XD ^ id) & 0xFFF8;

                    //  Check Colosseum\XD
                    // [IVs] [IVs] [xxx] [PID] [PID]
                    // [START] [rng1] [rng3]
                    pid = (rng3XD << 16) | rng4XD;
                    if (pid % 25 == nature)
                    {
                        var newSeed = new Seed
                        {
                            Method = "Colosseum/XD",
                            Pid = pid,
                            MonsterSeed = XDColoSeed,
                            Sid = sid
                        };
                        seeds.Add(newSeed);
                    }

                    //  Check Colosseum\XD XOR
                    // [IVs] [IVs] [xxx] [PID] [PID]
                    // [START] [rng1] [rng3]
                    pidXor = pid ^ 0x80008000;
                    if (pidXor % 25 == nature)
                    {
                        var newSeed = new Seed
                        {
                            Method = "Colosseum/XD",
                            Pid = pid,
                            MonsterSeed = XDColoSeedXor,
                            Sid = sid
                        };
                        seeds.Add(newSeed);
                    }
                }

                //  Now test rest of methods
                uint seed = x_test | cnt;
                var rng = new PokeRngR(seed);
                uint rng1 = rng.GetNext16BitNumber();
                //Checks that ivs line up
                if (rng1 == ivs1 || rng1 == ivs1_2)
                {
                    //  We have a max of 5 total RNG calls
                    //  to make a pokemon and we already have
                    //  one so lets go ahead and get 4 more.
                    uint rng2 = rng.GetNext16BitNumber();
                    uint rng3 = rng.GetNext16BitNumber();
                    uint rng4 = rng.GetNext16BitNumber();
                    uint method1Seed = rng.Seed;
                    uint method1SeedXor = method1Seed ^ 0x80000000;
                    sid = (rng2 ^ rng3 ^ id) & 0xFFF8;

                    rng.GetNext16BitNumber();
                    uint method234Seed = rng.Seed;
                    uint method234SeedXor = method234Seed ^ 0x80000000;

                    //  Check Method 1
                    // [PID] [PID] [IVs] [IVs]
                    // [rng3] [rng2] [rng1] [START]
                    pid = (rng2 << 16) + rng3;
                    if (pid % 25 == nature)
                    {
                        //  Build a seed to add to our collection
                        var newSeed = new Seed
                        {
                            Method = "Method 1",
                            Pid = pid,
                            MonsterSeed = method1Seed,
                            Sid = sid
                        };
                        seeds.Add(newSeed);
                    }

                    //  Check Method 1 XOR
                    // [PID] [PID] [IVs] [IVs]
                    // [rng3] [rng2] [rng1] [START]
                    pidXor = pid ^ 0x80008000;
                    if (pidXor % 25 == nature)
                    {
                        //  Build a seed to add to our collection
                        var newSeed = new Seed
                        {
                            Method = "Method 1",
                            Pid = pidXor,
                            MonsterSeed = method1SeedXor,
                            Sid = sid
                        };
                        seeds.Add(newSeed);
                    }

                    //  Check Reverse Method 1
                    // [PID] [PID] [IVs] [IVs]
                    // [rng2] [rng3] [rng1] [START]
                    pid = (rng3 << 16) + rng2;
                    if (pid % 25 == nature)
                    {
                        //  Build a seed to add to our collection
                        var newSeed = new Seed
                        {
                            Method = "Reverse Method 1",
                            Pid = pid,
                            MonsterSeed = method1Seed,
                            Sid = sid
                        };
                        seeds.Add(newSeed);
                    }

                    //  Check Reverse Method 1 XOR
                    // [PID] [PID] [IVs] [IVs]
                    // [rng2] [rng3] [rng1] [START]
                    pid = pid ^ 0x80008000;
                    if (pid % 25 == nature)
                    {
                        //  Build a seed to add to our collection
                        var newSeed = new Seed
                        {
                            Method = "Reverse Method 1",
                            Pid = pid,
                            MonsterSeed = method1SeedXor,
                            Sid = sid
                        };
                        seeds.Add(newSeed);
                    }
                    

                    //  Check Method 2
                    // [PID] [PID] [xxxx] [IVs] [IVs]
                    // [rng4] [rng3] [xxxx] [rng1] [START]
                    pid = (rng3 << 16) + rng4;
                    sid = (rng3 ^ rng4 ^ id) & 0xFFF8;
                    if (pid % 25 == nature)
                    {
                        //  Build a seed to add to our collection
                        var newSeed = new Seed
                        {
                            Method = "Method 2",
                            Pid = pid,
                            MonsterSeed = method234Seed,
                            Sid = sid
                        };
                        seeds.Add(newSeed);
                    }

                    //  Check Method 2
                    // [PID] [PID] [xxxx] [IVs] [IVs]
                    // [rng4] [rng3] [xxxx] [rng1] [START]
                    pidXor = pid ^ 0x80008000;
                    if (pidXor % 25 == nature)
                    {
                        //  Build a seed to add to our collection
                        var newSeed = new Seed
                        {
                            Method = "Method 2",
                            Pid = pidXor,
                            MonsterSeed = method234SeedXor,
                            Sid = sid
                        };
                        seeds.Add(newSeed);
                    }

                    /* Removed because Method 3 doesn't exist in-game
                    //  Check Method 3
                    //  [PID] [xxxx] [PID] [IVs] [IVs]
                    //  [rng4] [xxxx] [rng2] [rng1] [START]
                    if (Check(rng1, rng2, rng4, hp, atk, def, nature))
                    {
                        //  Build a seed to add to our collection
                        Seed newSeed = new Seed();
                        newSeed.Method = "Method 3";
                        newSeed.Pid = (rng2 << 16) + rng4;
                        newSeed.MonsterSeed = method234Seed;
                        newSeed.Sid = (rng2 ^ rng4 ^ id) & 0xFFF8;
                        seeds.Add(newSeed);
                    } */

                    //  Check Method 4
                    //  [PID] [PID] [IVs] [xxxx] [IVs]
                    //  [rng4] [rng3] [rng2] [xxxx] [START]
                    if (pid % 25 == nature)
                    {
                        //  Build a seed to add to our collection
                        var newSeed = new Seed
                        {
                            Method = "Method 4",
                            Pid = pid,
                            MonsterSeed = method234Seed,
                            Sid = sid
                        };
                        seeds.Add(newSeed);
                    }

                    //  Check Method 4 XOR
                    //  [PID] [PID] [IVs] [xxxx] [IVs]
                    //  [rng4] [rng3] [rng2] [xxxx] [START]
                    if (pidXor % 25 == nature)
                    {
                        //  Build a seed to add to our collection
                        var newSeed = new Seed
                        {
                            Method = "Method 4",
                            Pid = pid,
                            MonsterSeed = method234SeedXor,
                            Sid = sid
                        };
                        seeds.Add(newSeed);
                    }
                }
            }
            return seeds;
        }
    }


    public class IVtoPIDGenerator
    {
        public static string[] M1PID(uint hp, uint atk, uint def, uint spa, uint spd, uint spe, uint nature, uint tid)
        {
            List<Seed> seeds =
                IVtoSeed.GetSeeds(
                    hp,
                    atk,
                    def,
                    spa,
                    spd,
                    spe,
                    nature,
                    tid);
            //Console.WriteLine(hp + " " + atk + " " + def + " " + spa + " " + spd + " " + spe + " " + nature + " " + tid);
            Seed chosenOne = new Seed();
            foreach(Seed s in seeds)
            {
                //Console.WriteLine(s.Method);
                if(s.Method == "Method 1")
                {
                    chosenOne = s;
                    break;
                }
            }
            string[] ans = new string[2];
            ans[0] = chosenOne.Pid.ToString("X");
            ans[1] = chosenOne.Sid.ToString();
            return ans;
        }

        public static string[] M2PID(uint hp, uint atk, uint def, uint spa, uint spd, uint spe, uint nature, uint tid)
        {
            List<Seed> seeds =
                IVtoSeed.GetSeeds(
                    hp,
                    atk,
                    def,
                    spa,
                    spd,
                    spe,
                    nature,
                    tid);
            Console.WriteLine(hp + " " + atk + " " + def + " " + spa + " " + spd + " " + spe + " " + nature + " " + tid);
            Seed chosenOne = new Seed();
            foreach (Seed s in seeds)
            {
                Console.WriteLine(s.Method);
                if (s.Method == "Method 2")
                {
                    chosenOne = s;
                    break;
                }
            }
            string[] ans = new string[2];
            ans[0] = chosenOne.Pid.ToString("X");
            ans[1] = chosenOne.Sid.ToString();
            return ans;
        }

        public string[] generateWishmkr(List<uint> natureList)
        {
            uint finalPID = 0;
            uint finalHP = 0;
            uint finalATK = 0;
            uint finalDEF = 0;
            uint finalSPA = 0;
            uint finalSPD = 0;
            uint finalSPE = 0;
            for (uint x = 0; x <= 0xFFFF; x++)
            {
                uint pid1 = forward(x);
                uint pid2 = forward(pid1);
                uint pid = (pid1 & 0xFFFF0000) | (pid2 >> 16);
                uint nature = pid % 25;

                if (natureList == null || natureList.Contains(nature))
                {
                    uint ivs1 = forward(pid2);
                    uint ivs2 = forward(ivs1);
                    ivs1 >>= 16;
                    ivs2 >>= 16;
                    uint[] ivs = createIVs(ivs1, ivs2);
                    if (ivs != null)
                    {
                        finalPID = pid;
                        finalHP = ivs[0];
                        finalATK = ivs[1];
                        finalDEF = ivs[2];
                        finalSPA = ivs[3];
                        finalSPD = ivs[4];
                        finalSPE = ivs[5];
                        break;
                    }
                }
            }
            return new string[] { finalPID.ToString("X"), finalHP.ToString(), finalATK.ToString(), finalDEF.ToString(), finalSPA.ToString(), finalSPD.ToString(), finalSPE.ToString() };
        }

        private uint forward(uint seed)
        {
            return seed * 0x41c64e6d + 0x6073;
        }

        private uint[] createIVs(uint iv1, uint ivs2)
        {
            uint[] ivs = new uint[6];

            for (int x = 0; x < 3; x++)
            {
                int q = x * 5;
                uint iv = (iv1 >> q) & 31;
                if (iv >= 0 && iv <= 31)
                    ivs[x] = iv;
                else
                    return null;
            }

            uint iV = (ivs2 >> 5) & 31;
            if (iV >= 0 && iV <= 31)
                ivs[3] = iV;
            else
                return null;

            iV = (ivs2 >> 10) & 31;
            if (iV >= 0 && iV <= 31)
                ivs[4] = iV;
            else
                return null;

            iV = ivs2 & 31;
            if (iV >= 0 && iV <= 31)
                ivs[5] = iV;
            else
                return null;

            return ivs;
        }

        public static string[] XDPID(uint hp, uint atk, uint def, uint spa, uint spd, uint spe, uint nature, uint tid)
        {
            List<Seed> seeds =
                IVtoSeed.GetSeeds(
                    hp,
                    atk,
                    def,
                    spa,
                    spd,
                    spe,
                    nature,
                    tid);
            //Console.WriteLine("Colo IVS " + hp + " " + atk + " " + def + " " + spa + " " + spd + " " + spe + " " + nature + " " + tid);
            Seed chosenOne = new Seed();
            foreach (Seed s in seeds)
            {
                //Console.WriteLine(s.Method);
                if (s.Method == "Colosseum/XD")
                {
                    //Console.WriteLine("ColoXD");
                    chosenOne = s;
                    break;
                }
            }
            string[] ans = new string[2];
            ans[0] = chosenOne.Pid.ToString("X");
            ans[1] = chosenOne.Sid.ToString();
            return ans;
        }

        private static IVFilter hptofilter(string hiddenpower)
        {
            if(hiddenpower == "dark")
            {
                return new IVFilter(0, CompareType.HiddenOdd, 0, CompareType.HiddenOdd, 0, CompareType.HiddenOdd, 0, CompareType.HiddenOdd, 0, CompareType.HiddenOdd, 0, CompareType.HiddenOdd);
            }
            else if (hiddenpower == "dragon")
            {
                return new IVFilter(0, CompareType.HiddenEven, 0, CompareType.HiddenOdd, 0, CompareType.HiddenOdd, 0, CompareType.HiddenOdd, 0, CompareType.HiddenOdd, 0, CompareType.HiddenOdd);
            }
            else if (hiddenpower == "ice")
            {
                return new IVFilter(0, CompareType.HiddenEven, 0, CompareType.HiddenOdd, 0, CompareType.HiddenEven, 0, CompareType.HiddenOdd, 0, CompareType.HiddenOdd, 0, CompareType.HiddenOdd);
            }
            else if (hiddenpower == "psychic")
            {
                return new IVFilter(0, CompareType.HiddenEven, 0, CompareType.HiddenOdd, 0, CompareType.HiddenOdd, 0, CompareType.HiddenOdd, 0, CompareType.HiddenOdd, 0, CompareType.HiddenEven);
            }
            else if (hiddenpower == "electric")
            {
                return new IVFilter(0, CompareType.HiddenEven, 0, CompareType.HiddenOdd, 0, CompareType.HiddenEven, 0, CompareType.HiddenOdd, 0, CompareType.HiddenOdd, 0, CompareType.HiddenEven);
            }
            else if (hiddenpower == "grass")
            {
                return new IVFilter(0, CompareType.HiddenEven, 0, CompareType.HiddenOdd, 0, CompareType.HiddenOdd, 0, CompareType.HiddenEven, 0, CompareType.HiddenOdd, 0, CompareType.HiddenOdd);
            }
            else if (hiddenpower == "water")
            {
                return new IVFilter(0, CompareType.HiddenOdd, 0, CompareType.HiddenEven, 0, CompareType.HiddenEven, 0, CompareType.HiddenEven, 0, CompareType.HiddenOdd, 0, CompareType.HiddenOdd);
            }
            else if (hiddenpower == "fire")
            {
                return new IVFilter(0, CompareType.HiddenOdd, 0, CompareType.HiddenEven, 0, CompareType.HiddenOdd, 0, CompareType.HiddenEven, 0, CompareType.HiddenOdd, 0, CompareType.HiddenEven);
            }
            else if (hiddenpower == "steel")
            {
                return new IVFilter(0, CompareType.HiddenOdd, 0, CompareType.HiddenEven, 0, CompareType.HiddenEven, 0, CompareType.HiddenEven, 0, CompareType.HiddenOdd, 0, CompareType.HiddenEven);
            }
            else if (hiddenpower == "ghost")
            {
                return new IVFilter(0, CompareType.HiddenOdd, 0, CompareType.HiddenEven, 0, CompareType.HiddenOdd, 0, CompareType.HiddenOdd, 0, CompareType.HiddenEven, 0, CompareType.HiddenOdd);
            }
            else if (hiddenpower == "bug")
            {
                return new IVFilter(0, CompareType.HiddenOdd, 0, CompareType.HiddenEven, 0, CompareType.HiddenEven, 0, CompareType.HiddenOdd, 0, CompareType.HiddenEven, 0, CompareType.HiddenOdd);
            }
            else if (hiddenpower == "rock")
            {
                return new IVFilter(0, CompareType.HiddenEven, 0, CompareType.HiddenEven, 0, CompareType.HiddenOdd, 0, CompareType.HiddenOdd, 0, CompareType.HiddenEven, 0, CompareType.HiddenEven);
            }
            else if (hiddenpower == "ground")
            {
                return new IVFilter(0, CompareType.HiddenEven, 0, CompareType.HiddenEven, 0, CompareType.HiddenEven, 0, CompareType.HiddenOdd, 0, CompareType.HiddenEven, 0, CompareType.HiddenEven);
            }
            else if (hiddenpower == "poison")
            {
                return new IVFilter(0, CompareType.HiddenEven, 0, CompareType.HiddenEven, 0, CompareType.HiddenOdd, 0, CompareType.HiddenEven, 0, CompareType.HiddenEven, 0, CompareType.HiddenOdd);
            }
            else if (hiddenpower == "flying")
            {
                return new IVFilter(0, CompareType.HiddenEven, 0, CompareType.HiddenEven, 0, CompareType.HiddenEven, 0, CompareType.HiddenEven, 0, CompareType.HiddenEven, 0, CompareType.HiddenOdd);
            }
            else if (hiddenpower == "fighting")
            {
                return new IVFilter(0, CompareType.HiddenEven, 0, CompareType.HiddenEven, 0, CompareType.HiddenOdd, 0, CompareType.HiddenEven, 0, CompareType.HiddenEven, 0, CompareType.HiddenEven);
            }
            else
            {
                return new IVFilter();
            }
        }

        public static string[] getIVPID(uint nature, string hiddenpower, bool XD = false, string method = "")
        {
            var generator = new FrameGenerator();
            if (XD)
            {
                generator = new FrameGenerator
                {
                    FrameType = FrameType.ColoXD
                };
            }
            else if (method == "M2")
            {
                generator = new FrameGenerator
                {
                    FrameType = FrameType.Method2
                };
            }
            else if (method == "BACD_R")
            {
                generator = new FrameGenerator
                {
                    FrameType = FrameType.Method1Reverse
                };
                IVtoPIDGenerator bacdr = new IVtoPIDGenerator();
                return bacdr.generateWishmkr(new List<uint> { nature });
            }

            FrameCompare frameCompare = new FrameCompare(
                    hptofilter(hiddenpower),
                    new List<uint> { nature },
                    -1,
                    false,
                    false,
                    false,
                    null,
                    new NoGenderFilter());

            List<Frame> frames = generator.Generate(frameCompare, 0, 0);
            Console.WriteLine("Num frames: " + frames.Count);
            return new string[] { frames[0].Pid.ToString("X"), frames[0].Hp.ToString(), frames[0].Atk.ToString(), frames[0].Def.ToString(), frames[0].Spa.ToString(), frames[0].Spd.ToString(), frames[0].Spe.ToString() };
        }
    }
}
