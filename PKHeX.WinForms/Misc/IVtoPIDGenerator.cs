using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKHeX.WinForms.Misc
{
    internal interface IRNG
    {
        //  All of our RNG functions must be able to return
        //  a simple unsigned 32 bit item, and that is it.
        uint Next();
        uint Nextuint();
        void Reseed(uint seed);
    }

    internal class GenericRng : IRNG
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

        #region IRNG Members

        public void Reseed(uint seed)
        {
            Seed = seed;
        }

        // Interface call
        public uint Next()
        {
            return GetNext32BitNumber();
        }

        // Interface call
        public uint Nextuint()
        {
            return GetNext32BitNumber();
        }

        #endregion

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

    internal class ARng : GenericRng
    {
        public ARng(uint seed)
            : base(seed, 0x6c078965, 0x01)
        {
        }
    }

    internal class ARngR : GenericRng
    {
        public ARngR(uint seed)
            : base(seed, 0x9638806d, 0x69c77f93)
        {
        }
    }

    internal class GRng : GenericRng
    {
        public GRng(uint seed)
            : base(seed, 0x45, 0x1111)
        {
        }

        public override uint GetNext32BitNumber()
        {
            Seed = (Seed * mult + add) & 0x7fffffff;

            return Seed;
        }
    }

    internal class GRngR : GenericRng
    {
        public GRngR(uint seed)
            : base(seed, 0x233f128d, 0x789467a3)
        {
        }

        public override uint GetNext32BitNumber()
        {
            Seed = (Seed * mult + add) & 0x7fffffff;

            return Seed;
        }
    }

    internal class EncounterRng : GenericRng
    {
        public EncounterRng(uint seed)
            : base(seed, 0x41c64e6d, 0x3039)
        {
        }
    }

    internal class EncounterRngR : GenericRng
    {
        public EncounterRngR(uint seed)
            : base(seed, 0xeeb9eb65, 0xfc77a683)
        {
        }
    }

    internal class MersenneTwisterTable : GenericRng
    {
        public MersenneTwisterTable(uint seed)
            : base(seed, 0x6c078965, 0x01)
        {
        }

        public override uint GetNext32BitNumber()
        {
            Seed = (Seed ^ (Seed >> 30)) * mult + add;
            add = (add + 1) % 624;

            return Seed;
        }
    }

    public enum FrameType
    {
        Method1,
        Method1Reverse,
        Method2,
        Method3,
        Method4,
        MethodH1,
        MethodH2,
        MethodH4,
        MethodJ,
        MethodK,
        Method5Standard,
        Method5CGear,
        Method5Natures,
        ChainedShiny,
        Gen4Normal,
        Gen4International,
        Bred,
        BredSplit,
        BredAlternate,
        DPPtBred,
        HGSSBred,
        BWBred,
        BWBredInternational,
        WondercardIVs,
        Wondercard5thGen,
        Wondercard5thGenFixed,
        ColoXD,
        Channel,
        RSBredLower,
        RSBredUpper,
        FRLGBredLower,
        FRLGBredUpper,
        EBredPID,
        BW2Bred,
        BW2BredInternational
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

        //  Both of the below are based on the PID, so we're
        //  not actually going to store anything for these
        //  guys.

        public uint Ability
        {
            get { return Pid & 1; }
        }

        //  gender number
        public string Female50
        {
            get { return ((Pid & 0xFF) > 126) ? "M" : "F"; }
        }

        public string Female125
        {
            get { return ((Pid & 0xFF) > 30) ? "M" : "F"; }
        }

        public string Female25
        {
            get { return ((Pid & 0xFF) > 63) ? "M" : "F"; }
        }

        public string Female75
        {
            get { return ((Pid & 0xFF) > 190) ? "M" : "F"; }
        }

        public string Method { get; set; }

        public FrameType FrameType { get; set; }

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
        public GenderFilter(string name, uint genderValue, GenderCriteria genderCriteria)
        {
            Name = name;
            GenderValue = genderValue;
            GenderCriteria = genderCriteria;
        }

        public GenderFilter()
        {
            // blank constructor for the NoFilter class
        }

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

        public static List<GenderFilter> GenderFilterCollection()
        {
            var GenderFilterCollection =
                new List<GenderFilter>
                    {
                        new NoGenderFilter("Don't Care / Genderless / Fixed Gender"),
                        new GenderFilter("Male (50% Male / 50% Female)", 127, GenderCriteria.Male),
                        new GenderFilter("Female (50% Male / 50% Female)", 127, GenderCriteria.Female),
                        new GenderFilter("Male (25% Male / 75% Female)", 191, GenderCriteria.Male),
                        new GenderFilter("Female (25% Male / 75% Female)", 191, GenderCriteria.Female),
                        new GenderFilter("Male (75% Male / 25% Female)", 63, GenderCriteria.Male),
                        new GenderFilter("Female (75% Male / 25% Female)", 63, GenderCriteria.Female),
                        new GenderFilter("Male (87.5% Male / 12.5% Female)", 31, GenderCriteria.Male),
                        new GenderFilter("Female (87.5% Male / 12.5% Female)", 31, GenderCriteria.Female)
                    };

            return GenderFilterCollection;
        }

        // This is the collection we use for the main window
        // Entralink PIDs need to know gender values
        // So we added Genderless and 100% male\female entries
        public static List<GenderFilter> GenderFilterCollectionMain()
        {
            var GenderFilterCollection =
                new List<GenderFilter>
                    {
                        new GenderFilter("Don't Care / Genderless", 255, GenderCriteria.DontCareGenderless),
                        new GenderFilter("Male (50% Male / 50% Female)", 127, GenderCriteria.Male),
                        new GenderFilter("Female (50% Male / 50% Female)", 127, GenderCriteria.Female),
                        new GenderFilter("Male (25% Male / 75% Female)", 191, GenderCriteria.Male),
                        new GenderFilter("Female (25% Male / 75% Female)", 191, GenderCriteria.Female),
                        new GenderFilter("Male (75% Male / 25% Female)", 63, GenderCriteria.Male),
                        new GenderFilter("Female (75% Male / 25% Female)", 63, GenderCriteria.Female),
                        new GenderFilter("Male (87.5% Male / 12.5% Female)", 31, GenderCriteria.Male),
                        new GenderFilter("Female (87.5% Male / 12.5% Female)", 31, GenderCriteria.Female),
                        new GenderFilter("Male (100% Male)", 0, GenderCriteria.DontCareGenderless),
                        new GenderFilter("Female (100% Female)", 254, GenderCriteria.DontCareGenderless)
                    };

            return GenderFilterCollection;
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

    public enum Language
    {
        English,
        Japanese,
        German,
        Spanish,
        French,
        Italian,
        Korean
    }

    public enum DSType
    {
        DS_Lite,
        DS_DSi,
        DS_3DS
    };

    public enum ButtonComboType
    {
        None,
        Start,
        Select,
        A,
        B,
        Right,
        Left,
        Up,
        Down,
        R,
        L,
        X,
        Y
    };

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

    public class Profile
    {
        private uint _timer0Max;
        private uint _timer0Min;
        public string Name { get; set; }

        public ushort ID { get; set; }

        public ushort SID { get; set; }

        public ulong MAC_Address { get; set; }

        public Version Version { get; set; }

        public string VersionStr
        {
            get { return (Version).ToString(); }
        }

        public Language Language { get; set; }

        public DSType DSType { get; set; }

        public uint VCount { get; set; }

        public uint VFrame { get; set; }

        public uint Timer0Min
        {
            get { return _timer0Min; }
            set
            {
                _timer0Min = value;
                if (value > _timer0Max)
                    _timer0Max = value;
            }
        }

        public uint Timer0Max
        {
            get { return _timer0Max; }
            set
            {
                _timer0Max = value;
                if (value < _timer0Min)
                    _timer0Min = value;
            }
        }

        public uint GxStat { get; set; }

        public string KeyString
        {
            get
            {
                if (Keypresses == 0) return "None";
                string keyString = "";
                byte b = 0x1;
                for (int i = 0; i < 4; ++i)
                {
                    if ((Keypresses & b) != 0)
                    {
                        if (keyString.Length > 0)
                            keyString += ", ";
                        keyString += i == 0 ? "None" : i.ToString();
                    }
                    b <<= 1;
                }
                return keyString;
            }
        }

        public byte Keypresses { get; set; }

        public bool SoftReset { get; set; }

        public bool SkipLR { get; set; }

        // note: BW2 only
        public bool MemoryLink { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public string ProfileInformation()
        {
            return
                string.Format(
                    "{0} {1} Version: {2} ID: {3} SID: {4} Timer0: {5} - {6} VCount: {7} VFrame: {8} GxStat: {9} Keypresses: {10}",
                    DSType.ToString().Replace('_', ' '), MAC_Address.ToString("X"),
                    Language + " " + Version, ID, SID,
                    Timer0Min.ToString("X"), Timer0Max.ToString("X"), VCount.ToString("X"), VFrame.ToString("X"),
                    GxStat.ToString("X"), KeyString) + (SkipLR ? " (Skip L\\R)" : "");
        }

        public string ProfileInformationShort()
        {
            return string.Format("{0} {1} Version: {2} Timer0: {3} - {4} VCount: {5} VFrame: {6} GxStat: {7}",
                                 DSType.ToString().Replace('_', ' '), MAC_Address.ToString("X"),
                                 Language + " " + Version,
                                 Timer0Min.ToString("X"), Timer0Max.ToString("X"), VCount.ToString("X"),
                                 VFrame.ToString("X"), GxStat.ToString("X"));
        }
    }

    internal class Responses
    {
        public static string ElmResponses(uint seed, uint count, uint skips)
        {
            string responses = "";

            var rng = new PokeRng(seed);

            uint rngCalls = count + skips;

            if (skips > 0)
            {
                responses += "(";
            }

            for (int n = 0; n < rngCalls; n++)
            {
                uint response = rng.GetNext16BitNumber();

                response %= 3;

                if (response == 0)
                    responses += "E";
                if (response == 1)
                    responses += "K";
                if (response == 2)
                    responses += "P";

                //  Skip the last item
                if (n != rngCalls - 1)
                {
                    if (skips != 0 && skips == n + 1)
                    {
                        responses += " skipped)   ";
                    }
                    else
                    {
                        responses += ", ";
                    }
                }
            }

            return responses;
        }

        public static string ElmResponse(uint rngResult)
        {
            uint response = rngResult % 3;

            string responses = "";

            if (response == 0)
                responses += "E";
            if (response == 1)
                responses += "K";
            if (response == 2)
                responses += "P";

            return responses;
        }

        public static string ChatotResponse(uint rngResult)
        {
            uint result = ((rngResult & 0x1FFF) * 100) >> 13;
            if (result < 20)
                return "Low (" + result.ToString() + ")";
            if (result < 40)
                return "Mid-Low (" + result.ToString() + ")";
            if (result < 60)
                return "Mid (" + result.ToString() + ")";
            if (result < 80)
                return "Mid-High (" + result.ToString() + ")";
            return "High (" + result.ToString() + ")";
        }

        public static string ChatotResponse64(uint rngResult)
        {
            uint result = (uint)(((ulong)rngResult * 0x1FFF) >> 32) / 82;
            if (result < 20)
                return "Low (" + result.ToString() + ")";
            if (result < 40)
                return "Mid-Low (" + result.ToString() + ")";
            if (result < 60)
                return "Mid (" + result.ToString() + ")";
            if (result < 80)
                return "Mid-High (" + result.ToString() + ")";
            return "High (" + result.ToString() + ")";
        }

        public static string ChatotResponse64Short(uint rngResult)
        {
            uint result = (uint)(((ulong)rngResult * 0x1FFF) >> 32) / 82;
            if (result < 20)
                return "L (" + result.ToString() + ")";
            if (result < 40)
                return "ML (" + result.ToString() + ")";
            if (result < 60)
                return "M (" + result.ToString() + ")";
            if (result < 80)
                return "MH (" + result.ToString() + ")";
            return "H (" + result.ToString() + ")";
        }

    }

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
        private uint inh1;
        private uint inh2;
        private uint inh3;
        private int maleOnlySpecies;
        private uint number;
        private uint par1;
        private uint par2;
        private uint par3;
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

        public string Elm
        {
            get { return Responses.ElmResponse(RngResult); }
        }

        public string CaveSpotting
        {
            get { return RngResult >> 31 == 1 ? "Possible" : ""; }
        }

        // Chatot response for 4th Gen Games
        public string Chatot
        {
            get { return Responses.ChatotResponse(RngResult); }
        }

        // Chatot response for 5th Gen Games (64-bit RNG)
        public string Chatot64
        {
            get { return Responses.ChatotResponse64(RngResult); }
        }

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

        /// <summary>
        ///     Friendly display name since we want to show the S as a postfix to the split spreads right now.
        /// </summary>
        public string Name
        {
            get
            {
                string name = number.ToString();

                if (FrameType == FrameType.BredSplit)
                    name += "S";

                return name;
            }
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


                //  Calculate the inheretence for this frame
                if (FrameType == FrameType.Bred ||
                    FrameType == FrameType.BredSplit ||
                    FrameType == FrameType.DPPtBred)
                {
                    DisplayHp = Hp.ToString();
                    DisplayAtk = Atk.ToString();
                    DisplayDef = Def.ToString();
                    DisplaySpa = Spa.ToString();
                    DisplaySpd = Spd.ToString();
                    DisplaySpe = Spe.ToString();

                    uint inherited1 = HABCDS[inh1 % 6];
                    switch (inherited1)
                    {
                        case 0:
                            DisplayHp = (par1 & 1) == 0 ? "A" : "B";
                            break;
                        case 1:
                            DisplayAtk = (par1 & 1) == 0 ? "A" : "B";
                            break;
                        case 2:
                            DisplayDef = (par1 & 1) == 0 ? "A" : "B";
                            break;
                        case 3:
                            DisplaySpa = (par1 & 1) == 0 ? "A" : "B";
                            break;
                        case 4:
                            DisplaySpd = (par1 & 1) == 0 ? "A" : "B";
                            break;
                        case 5:
                            DisplaySpe = (par1 & 1) == 0 ? "A" : "B";
                            break;
                    }

                    uint inherited2 = ABCDS[inh2 % 5];
                    switch (inherited2)
                    {
                        case 1:
                            DisplayAtk = (par2 & 1) == 0 ? "A" : "B";
                            break;
                        case 2:
                            DisplayDef = (par2 & 1) == 0 ? "A" : "B";
                            break;
                        case 3:
                            DisplaySpa = (par2 & 1) == 0 ? "A" : "B";
                            break;
                        case 4:
                            DisplaySpd = (par2 & 1) == 0 ? "A" : "B";
                            break;
                        case 5:
                            DisplaySpe = (par2 & 1) == 0 ? "A" : "B";
                            break;
                    }

                    uint inherited3 = ACDS[inh3 & 3];
                    switch (inherited3)
                    {
                        case 1:
                            DisplayAtk = (par3 & 1) == 0 ? "A" : "B";
                            break;
                        case 3:
                            DisplaySpa = (par3 & 1) == 0 ? "A" : "B";
                            break;
                        case 4:
                            DisplaySpd = (par3 & 1) == 0 ? "A" : "B";
                            break;
                        case 5:
                            DisplaySpe = (par3 & 1) == 0 ? "A" : "B";
                            break;
                    }
                }
                if (FrameType == FrameType.HGSSBred ||
                    FrameType == FrameType.RSBredUpper ||
                    FrameType == FrameType.FRLGBredUpper)
                {
                    DisplayHp = Hp.ToString();
                    DisplayAtk = Atk.ToString();
                    DisplayDef = Def.ToString();
                    DisplaySpa = Spa.ToString();
                    DisplaySpd = Spd.ToString();
                    DisplaySpe = Spe.ToString();

                    uint[] available = { 0, 1, 2, 3, 4, 5 };

                    // Dumb that we have to do this, but we really
                    // need these guys in an array for things to 
                    // work correctly.
                    var rngArray = new uint[6];
                    rngArray[0] = inh1;
                    rngArray[1] = inh2;
                    rngArray[2] = inh3;
                    rngArray[3] = par1;
                    rngArray[4] = par2;
                    rngArray[5] = par3;

                    for (uint cnt = 0; cnt < 3; cnt++)
                    {
                        // Decide which parent (A or B) from which we'll pick an IV
                        uint parent = rngArray[3 + cnt] & 1;

                        // Decide which stat to pick for IV inheritance
                        uint ivslot = available[rngArray[0 + cnt] % (6 - cnt)];
                        //  We have our parent and we have our slot, so lets 
                        //  put them in the correct place here 
                        string parentString = (parent == 0 ? "A" : "B");

                        switch (ivslot)
                        {
                            case 0:
                                DisplayHp = parentString;
                                break;
                            case 1:
                                DisplayAtk = parentString;
                                break;
                            case 2:
                                DisplayDef = parentString;
                                break;
                            case 3:
                                DisplaySpe = parentString;
                                break;
                            case 4:
                                DisplaySpa = parentString;
                                break;
                            case 5:
                                DisplaySpd = parentString;
                                break;
                        }

                        // Find out where taking an item from
                        //  so that we know where to start doing
                        //  doing our shift.

                        // Avoids repeat IV inheritance
                        for (uint j = 0; j < 5 - cnt; j++)
                        {
                            if (ivslot <= available[j])
                            {
                                available[j] = available[j + 1];
                            }
                        }
                    }
                }

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
            uint inh1,
            uint inh2,
            uint inh3,
            uint par1,
            uint par2,
            uint par3,
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
                inh1 = inh1,
                inh2 = inh2,
                inh3 = inh3,
                par1 = par1,
                par2 = par2,
                par3 = par3,
                Dv = (dv2 << 16) | dv1
            };


            //  Set up the ID and SID before we calculate 
            //  the pid, as we are going to need this.


            return frame;
        }

        //Emerald Eggs
        public static Frame GenerateFrame(
            FrameType frameType,
            uint number,
            uint rngResult,
            uint pid,
            uint dv1,
            uint dv2,
            uint inh1,
            uint inh2,
            uint inh3,
            uint par1,
            uint par2,
            uint par3,
            uint[] parentA,
            uint[] parentB,
            uint id,
            uint sid,
            uint offset)
        {
            var frame = new Frame(frameType)
            {
                Number = number,
                RngResult = rngResult,
                Offset = offset,
                id = id,
                sid = sid,
                Pid = pid,
                inh1 = inh1,
                inh2 = inh2,
                inh3 = inh3,
                par1 = par1,
                par2 = par2,
                par3 = par3,
                Dv = (dv2 << 16) | dv1
            };

            //punch in the inheritence values
            if (parentA != null && parentB != null && parentA.Length == 6 && parentB.Length == 6)
            {
                frame.DisplayHpAlt = GetInheritence(frame.DisplayHp, parentA[0], parentB[0]);
                frame.DisplayAtkAlt = GetInheritence(frame.DisplayAtk, parentA[1], parentB[1]);
                frame.DisplayDefAlt = GetInheritence(frame.DisplayDef, parentA[2], parentB[2]);
                frame.DisplaySpaAlt = GetInheritence(frame.DisplaySpa, parentA[3], parentB[3]);
                frame.DisplaySpdAlt = GetInheritence(frame.DisplaySpd, parentA[4], parentB[4]);
                frame.DisplaySpeAlt = GetInheritence(frame.DisplaySpe, parentA[5], parentB[5]);
            }

            return frame;
        }

        private static string GetInheritence(string display, uint parenta, uint parentb)
        {
            return display == "A"
                       ? parenta.ToString()
                       : display == "B" ? parentb.ToString() : display;
        }

        public static Frame GenerateFrame(
            FrameType frameType,
            uint number,
            uint rngResult,
            uint dv1,
            uint dv2,
            uint inh1,
            uint inh2,
            uint inh3,
            uint par1,
            uint par2,
            uint par3,
            uint[] parentA,
            uint[] parentB,
            uint id,
            uint sid,
            uint offset)
        {
            var frame = new Frame(frameType)
            {
                Number = number,
                RngResult = rngResult,
                id = id,
                sid = sid,
                inh1 = inh1,
                inh2 = inh2,
                inh3 = inh3,
                par1 = par1,
                par2 = par2,
                par3 = par3
            };


            //  Set up the ID and SID before we calculate 
            //  the pid, as we are going to need this.


            var rngArray = new uint[6];
            rngArray[0] = inh1;
            rngArray[1] = inh2;
            rngArray[2] = inh3;
            rngArray[3] = par1;
            rngArray[4] = par2;
            rngArray[5] = par3;

            frame.Dv = (dv2 << 16) + dv1;

            if (parentA != null && parentB != null)
            {
                uint[] available = { 0, 1, 2, 3, 4, 5 };
                if (frameType == FrameType.HGSSBred)
                {
                    for (uint cnt = 0; cnt < 3; cnt++)
                    {
                        uint parent = rngArray[3 + cnt] & 1;
                        uint ivslot = available[rngArray[0 + cnt] % (6 - cnt)];
                        //  We have our parent and we have our slot, so lets 
                        //  put them in the correct place here 

                        switch (ivslot)
                        {
                            case 0:
                                frame.Hp = (parent == 0 ? parentA[0] : parentB[0]);
                                break;
                            case 1:
                                frame.Atk = (parent == 0 ? parentA[1] : parentB[1]);
                                break;
                            case 2:
                                frame.Def = (parent == 0 ? parentA[2] : parentB[2]);
                                break;
                            case 3:
                                //parrents speed = 5, egg speed = 3
                                frame.Spe = (parent == 0 ? parentA[5] : parentB[5]);
                                break;
                            case 4:
                                //parrents spa = 3, egg spa = 4
                                frame.Spa = (parent == 0 ? parentA[3] : parentB[3]);
                                break;
                            case 5:
                                //parrents spd = 4, egg spd = 6
                                frame.Spd = (parent == 0 ? parentA[4] : parentB[4]);
                                break;
                        }

                        // Find out where taking an item from
                        //  so that we know where to start doing
                        //  doing our shift.
                        for (uint j = 0; j < 5 - cnt; j++)
                        {
                            if (ivslot <= available[j])
                            {
                                available[j] = available[j + 1];
                            }
                        }
                    }
                }
                else
                {
                    uint inherited1 = HABCDS[inh1 % 6];
                    switch (inherited1)
                    {
                        case 0:
                            frame.Hp = (par1 & 1) == 0 ? parentA[0] : parentB[0];
                            break;
                        case 1:
                            frame.Atk = (par1 & 1) == 0 ? parentA[1] : parentB[1];
                            break;
                        case 2:
                            frame.Def = (par1 & 1) == 0 ? parentA[2] : parentB[2];
                            break;
                        case 3:
                            frame.Spa = (par1 & 1) == 0 ? parentA[3] : parentB[3];
                            break;
                        case 4:
                            frame.Spd = (par1 & 1) == 0 ? parentA[4] : parentB[4];
                            break;
                        case 5:
                            frame.Spe = (par1 & 1) == 0 ? parentA[5] : parentB[5];
                            break;
                    }

                    uint inherited2 = ABCDS[inh2 % 5];
                    switch (inherited2)
                    {
                        case 1:
                            frame.Atk = (par2 & 1) == 0 ? parentA[1] : parentB[1];
                            break;
                        case 2:
                            frame.Def = (par2 & 1) == 0 ? parentA[2] : parentB[2];
                            break;
                        case 3:
                            frame.Spa = (par2 & 1) == 0 ? parentA[3] : parentB[3];
                            break;
                        case 4:
                            frame.Spd = (par2 & 1) == 0 ? parentA[4] : parentB[4];
                            break;
                        case 5:
                            frame.Spe = (par2 & 1) == 0 ? parentA[5] : parentB[5];
                            break;
                    }

                    uint inherited3 = ACDS[inh3 & 3];
                    switch (inherited3)
                    {
                        case 1:
                            frame.Atk = (par3 & 1) == 0 ? parentA[1] : parentB[1];
                            break;
                        case 3:
                            frame.Spa = (par3 & 1) == 0 ? parentA[3] : parentB[3];
                            break;
                        case 4:
                            frame.Spd = (par3 & 1) == 0 ? parentA[4] : parentB[4];
                            break;
                        case 5:
                            frame.Spe = (par3 & 1) == 0 ? parentA[5] : parentB[5];
                            break;
                    }
                }

                frame.DisplayHpAlt = frame.Hp.ToString();
                frame.DisplayAtkAlt = frame.Atk.ToString();
                frame.DisplayDefAlt = frame.Def.ToString();
                frame.DisplaySpaAlt = frame.Spa.ToString();
                frame.DisplaySpdAlt = frame.Spd.ToString();
                frame.DisplaySpeAlt = frame.Spe.ToString();
            }

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



        //for Channel Method
        public static Frame GenerateChannel(
            uint seed,
            FrameType frameType,
            uint number,
            uint rngResult,
            uint pid1,
            uint pid2,
            uint dv1,
            uint dv2,
            uint dv3,
            uint dv4,
            uint dv5,
            uint dv6,
            uint id,
            uint sid)
        {
            if ((pid2 > 7 ? 0 : 1) != (pid1 ^ 40122 ^ sid))
                pid1 ^= 0x8000;

            var frame = new Frame(frameType)
            {
                seed = seed,
                number = number,
                RngResult = rngResult,
                id = id,
                sid = sid,
                Pid = (pid1 << 16) | pid2
            };

            frame.Hp = dv1;
            frame.Atk = dv2;
            frame.Def = dv3;
            frame.Spa = dv4;
            frame.Spd = dv5;
            frame.Spe = dv6;

            return frame;
        }

        // for Methods H, J, K
        public static Frame GenerateFrame(
            uint seed,
            FrameType frameType,
            EncounterType encounterType,
            uint number,
            uint rngResult,
            uint pid1,
            uint pid2,
            uint dv1,
            uint dv2,
            uint id,
            uint sid,
            uint offset,
            int encounterSlot)
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
                Dv = (dv2 << 16) | dv1,
                EncounterType = encounterType,
                EncounterSlot = encounterSlot
            };


            //  Set up the ID and SID before we calculate 
            //  the pid, as we are going to need this.


            return frame;
        }

        public static Frame GenerateFrame(
            FrameType frameType,
            uint number,
            uint rngResult,
            uint pid,
            uint id,
            uint sid)
        {
            var frame = new Frame(frameType)
            { Number = number, RngResult = rngResult, id = id, sid = sid, Pid = pid, Dv = 0 };


            //  Set up the ID and SID before we calculate 
            //  the pid, as we are going to need this.
            //  frame.Pid = pid;

            return frame;
        }

        // for Ruby\Sapphire lower egg PID half
        public static Frame GenerateFrame(
            FrameType frameType,
            uint number,
            uint rngResult,
            uint pid,
            uint compatibility)
        {
            var frame = new Frame(frameType) { RngResult = rngResult };


            // if upper 8 bits is less than 51
            // Day-Care Man holds an egg
            if ((rngResult * 100) / 0xFFFF < compatibility)
            {
                // If valid, assign the frame a non-zero number so it doesn't get filtered
                frame.Number = number;

                if (pid > 0xFFFD)
                {
                    frame.Pid = (pid + 3) % 0xFFFF;
                }
                else
                    frame.Pid = (pid & 0xFFFF) + 1;
            }

            return frame;
        }

        // for Ruby\Sapphire full egg PID
        public static Frame GenerateFrame(
            FrameType frameType,
            uint number,
            uint rngResult,
            uint lowerPID,
            uint upperPID,
            uint dv1,
            uint dv2,
            uint inh1,
            uint inh2,
            uint inh3,
            uint par1,
            uint par2,
            uint par3,
            uint[] parentA,
            uint[] parentB,
            uint id,
            uint sid)
        {
            var frame = new Frame(frameType)
            {
                Number = number,
                RngResult = rngResult,
                id = id,
                sid = sid,
                inh1 = inh1,
                inh2 = inh2,
                inh3 = inh3,
                par1 = par1,
                par2 = par2,
                par3 = par3,
                Pid = (upperPID << 16) | lowerPID,
                Dv = (dv2 << 16) | dv1
            };


            uint[] available = { 0, 1, 2, 3, 4, 5 };

            var rngArray = new uint[6];
            rngArray[0] = inh1;
            rngArray[1] = inh2;
            rngArray[2] = inh3;
            rngArray[3] = par1;
            rngArray[4] = par2;
            rngArray[5] = par3;

            if (parentA != null && parentB != null)
            {
                for (uint cnt = 0; cnt < 3; cnt++)
                {
                    uint parent = rngArray[3 + cnt] & 1;
                    uint ivslot = available[rngArray[0 + cnt] % (6 - cnt)];
                    //  We have our parent and we have our slot, so lets 
                    //  put them in the correct place here 

                    switch (ivslot)
                    {
                        case 0:
                            frame.Hp = (parent == 0 ? parentA[0] : parentB[0]);
                            break;
                        case 1:
                            frame.Atk = (parent == 0 ? parentA[1] : parentB[1]);
                            break;
                        case 2:
                            frame.Def = (parent == 0 ? parentA[2] : parentB[2]);
                            break;
                        case 3:
                            frame.Spe = (parent == 0 ? parentA[5] : parentB[5]);
                            break;
                        case 4:
                            frame.Spa = (parent == 0 ? parentA[3] : parentB[3]);
                            break;
                        case 5:
                            frame.Spd = (parent == 0 ? parentA[4] : parentB[4]);
                            break;
                    }

                    // Find out where taking an item from
                    //  so that we know where to start doing
                    //  doing our shift.
                    for (uint j = 0; j < 5 - cnt; j++)
                    {
                        if (ivslot <= available[j])
                        {
                            available[j] = available[j + 1];
                        }
                    }
                }
            }

            frame.DisplayHpAlt = frame.Hp.ToString();
            frame.DisplayAtkAlt = frame.Atk.ToString();
            frame.DisplayDefAlt = frame.Def.ToString();
            frame.DisplaySpaAlt = frame.Spa.ToString();
            frame.DisplaySpdAlt = frame.Spd.ToString();
            frame.DisplaySpeAlt = frame.Spe.ToString();

            return frame;
        }

        // used for 5th Gen nature generation
        public static Frame GenerateFrame(
            FrameType frameType,
            EncounterType encounterType,
            uint number,
            uint rngResult,
            uint pid,
            uint id,
            uint sid,
            uint natureValue,
            bool synch,
            int encounterSlot,
            uint itemCalc)
        {
            var frame = new Frame(frameType)
            {
                Number = number,
                RngResult = rngResult,
                id = id,
                sid = sid,
                Pid = pid,
                Nature = natureValue,
                ability = (pid >> 16) & 1,
                EncounterType = encounterType,
                EncounterSlot = encounterSlot,
                ItemCalc = itemCalc,
                synchable = synch
            };


            //  Set up the ID and SID before we calculate 
            //  the pid, as we are going to need this.


            return frame;
        }

        public static Frame GenerateFrame(
            FrameType frameType,
            EncounterType encounterType,
            uint number,
            uint rngResult,
            uint pid,
            uint id,
            uint sid,
            uint natureValue,
            bool synch,
            int encounterSlot,
            uint itemCalc,
            uint[] rngIVs)
        {
            var frame = new Frame(frameType)
            {
                Number = number,
                RngResult = rngResult,
                id = id,
                sid = sid,
                Pid = pid,
                Nature = natureValue,
                ability = (pid >> 16) & 1,
                EncounterType = encounterType,
                EncounterSlot = encounterSlot,
                ItemCalc = itemCalc,
                synchable = synch,
                Hp = rngIVs[0],
                Atk = rngIVs[1],
                Def = rngIVs[2],
                Spa = rngIVs[3],
                Spd = rngIVs[4],
                Spe = rngIVs[5]
            };


            //  Set up the ID and SID before we calculate 
            //  the pid, as we are going to need this.


            return frame;
        }

        // for 5th Gen eggs
        public static Frame GenerateFrame(
            FrameType frameType,
            uint number,
            uint rngResult,
            int maleOnlySpecies,
            uint inh1,
            uint inh2,
            uint inh3,
            uint par1,
            uint par2,
            uint par3,
            uint pid,
            uint id,
            uint sid,
            bool dream,
            bool everstone,
            uint natureValue,
            uint maxSkips)
        {
            var frame = new Frame(frameType)
            {
                Number = number,
                RngResult = rngResult,
                id = id,
                sid = sid,
                Pid = pid,
                Ability = (pid >> 16) & 1,
                Nature = natureValue,
                dreamAbility = dream,
                synchable = everstone,
                maleOnlySpecies = maleOnlySpecies,
                MaxSkips = maxSkips,
                inh1 = inh1,
                inh2 = inh2,
                inh3 = inh3,
                par1 = par1,
                par2 = par2,
                par3 = par3
            };


            return frame;
        }

        // used for 5th Gen eggs when parent IVs are known
        public static Frame GenerateFrame(
            FrameType frameType,
            uint number,
            uint rngResult,
            int maleOnlySpecies,
            uint inh1,
            uint inh2,
            uint inh3,
            uint par1,
            uint par2,
            uint par3,
            uint[] parentA,
            uint[] parentB,
            uint[] rngIVs,
            uint pid,
            uint id,
            uint sid,
            bool dream,
            bool everstone,
            uint natureValue,
            uint maxSkips)
        {
            var frame = new Frame(frameType)
            {
                Number = number,
                RngResult = rngResult,
                id = id,
                sid = sid,
                Pid = pid,
                Ability = (pid >> 16) & 1,
                Nature = natureValue,
                dreamAbility = dream,
                synchable = everstone,
                maleOnlySpecies = maleOnlySpecies,
                MaxSkips = maxSkips,
                inh1 = inh1,
                inh2 = inh2,
                inh3 = inh3,
                par1 = par1,
                par2 = par2,
                par3 = par3,
                Hp = rngIVs[0],
                Atk = rngIVs[1],
                Def = rngIVs[2],
                Spa = rngIVs[3],
                Spd = rngIVs[4],
                Spe = rngIVs[5]
            };


            var rngArray = new uint[6];
            rngArray[0] = inh1;
            rngArray[1] = inh2;
            rngArray[2] = inh3;
            rngArray[3] = par1;
            rngArray[4] = par2;
            rngArray[5] = par3;

            for (uint cnt = 0; cnt < 3; cnt++)
            {
                uint parent = rngArray[3 + cnt] & 1;

                //  We have our parent and we have our slot, so lets 
                //  put them in the correct place here 
                uint parentIV = (parent == 1 ? parentA[rngArray[cnt]] : parentB[rngArray[cnt]]);

                switch (rngArray[cnt])
                {
                    case 0:
                        frame.Hp = parentIV;
                        break;
                    case 1:
                        frame.Atk = parentIV;
                        break;
                    case 2:
                        frame.Def = parentIV;
                        break;
                    case 3:
                        frame.Spa = parentIV;
                        break;
                    case 4:
                        frame.Spd = parentIV;
                        break;
                    case 5:
                        frame.Spe = parentIV;
                        break;
                }
            }

            return frame;
        }


        // for 5th Gen Wild Pokémon
        public static Frame GenerateFrame(
            FrameType frameType,
            uint number,
            uint rngResult,
            uint hp,
            uint atk,
            uint def,
            uint spa,
            uint spd,
            uint spe)
        {
            var frame = new Frame(frameType)
            {
                Number = number,
                seed = rngResult,
                Hp = hp,
                Atk = atk,
                Def = def,
                Spa = spa,
                Spd = spd,
                Spe = spe
            };


            return frame;
        }


        // 5th Gen Wondercards
        public static Frame GenerateFrame(
            FrameType frameType,
            uint id,
            uint sid,
            uint number,
            uint rngResult,
            uint hp,
            uint atk,
            uint def,
            uint spa,
            uint spd,
            uint spe,
            uint natureValue,
            uint pid)
        {
            var frame = new Frame(frameType)
            {
                Number = number,
                RngResult = rngResult,
                id = id,
                sid = sid,
                Hp = hp,
                Atk = atk,
                Def = def,
                Spa = spa,
                Spd = spd,
                Spe = spe,
                Pid = pid ^ 0x10000
            };


            var nature = (uint)((ulong)natureValue * 25 >> 32);
            frame.Nature = nature;
            frame.Ability = (pid >> 16) & 1;

            return frame;
        }

        // This method is only called when a frame is going to be displayed
        // Avoids unnecessary costly functions
        public void DisplayPrep()
        {
            if (FrameType != FrameType.Method5Natures &&
                FrameType != FrameType.BredAlternate &&
                FrameType != FrameType.BredSplit &&
                FrameType != FrameType.Bred &&
                FrameType != FrameType.DPPtBred &&
                FrameType != FrameType.HGSSBred &&
                FrameType != FrameType.BWBred &&
                FrameType != FrameType.BWBredInternational &&
                FrameType != FrameType.RSBredUpper)
            {
                DisplayHp = Hp.ToString();
                DisplayAtk = Atk.ToString();
                DisplayDef = Def.ToString();
                DisplaySpa = Spa.ToString();
                DisplaySpd = Spd.ToString();
                DisplaySpe = Spe.ToString();
            }

            if (FrameType == FrameType.BWBred ||
                FrameType == FrameType.BWBredInternational)
            {
                var rngArray = new uint[6];
                rngArray[0] = inh1;
                rngArray[1] = inh2;
                rngArray[2] = inh3;
                rngArray[3] = par1;
                rngArray[4] = par2;
                rngArray[5] = par3;

                DisplayHpAlt = Hp.ToString();
                DisplayAtkAlt = Atk.ToString();
                DisplayDefAlt = Def.ToString();
                DisplaySpaAlt = Spa.ToString();
                DisplaySpdAlt = Spd.ToString();
                DisplaySpeAlt = Spe.ToString();

                for (uint cnt = 0; cnt < 3; cnt++)
                {
                    uint parent = rngArray[3 + cnt] & 1;

                    //  We have our parent and we have our slot, so lets 
                    //  put them in the correct place here 
                    string parentString = (parent == 1 ? "Fe" : "Ma");

                    switch (rngArray[cnt])
                    {
                        case 0:
                            DisplayHp = parentString;
                            break;
                        case 1:
                            DisplayAtk = parentString;
                            break;
                        case 2:
                            DisplayDef = parentString;
                            break;
                        case 3:
                            DisplaySpa = parentString;
                            break;
                        case 4:
                            DisplaySpd = parentString;
                            break;
                        case 5:
                            DisplaySpe = parentString;
                            break;
                    }
                }
            }
        }

        public static Frame Clone(Frame source)
        {
            // We're implementing this only for Gen 5 IVs
            // Because they can have multiple nearby shiny frames

            var clone = new Frame
            {
                FrameType = source.FrameType,
                number = source.number,
                seed = source.seed,
                Hp = source.Hp,
                Atk = source.Atk,
                Def = source.Def,
                Spa = source.Spa,
                Spd = source.Spd,
                Spe = source.Spe,
                DisplayHp = source.DisplayHp,
                DisplayAtk = source.DisplayAtk,
                DisplayDef = source.DisplayDef,
                DisplaySpa = source.DisplaySpa,
                DisplaySpd = source.DisplaySpd,
                DisplaySpe = source.DisplaySpe,
                EncounterMod = source.EncounterMod,
                EncounterSlot = source.EncounterSlot,
                id = source.id,
                sid = source.sid,
                Pid = source.Pid
            };


            return clone;
        }
    }

    internal class FrameCompare
    {
        private readonly bool ABcheck;
        private readonly int ability;
        private readonly CompareType atkCompare;
        private readonly uint atkValue;
        private readonly uint compareAtkA;
        private readonly uint compareAtkB;
        private readonly uint compareDefA;
        private readonly uint compareDefB;
        private readonly uint compareHpA;
        private readonly uint compareHpB;
        private readonly uint compareSpaA;
        private readonly uint compareSpaB;
        private readonly uint compareSpdA;
        private readonly uint compareSpdB;
        private readonly uint compareSpeA;
        private readonly uint compareSpeB;
        private readonly List<FrameCompare> comparers;
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

        public FrameCompare(
            uint compareHpA,
            uint compareAtkA,
            uint compareDefA,
            uint compareSpaA,
            uint compareSpdA,
            uint compareSpeA,
            uint compareHpB,
            uint compareAtkB,
            uint compareDefB,
            uint compareSpaB,
            uint compareSpdB,
            uint compareSpeB,
            uint hpValue,
            CompareType hpCompare,
            uint atkValue,
            CompareType atkCompare,
            uint defValue,
            CompareType defCompare,
            uint spaValue,
            CompareType spaCompare,
            uint spdValue,
            CompareType spdCompare,
            uint speValue,
            CompareType speCompare,
            List<uint> natures,
            int ability,
            bool shinyOnly,
            bool checkparents,
            GenderFilter genderFilter)
        {
            comparers = new List<FrameCompare>();

            this.compareHpA = compareHpA;
            this.compareAtkA = compareAtkA;
            this.compareDefA = compareDefA;
            this.compareSpaA = compareSpaA;
            this.compareSpdA = compareSpdA;
            this.compareSpeA = compareSpeA;

            this.compareHpB = compareHpB;
            this.compareAtkB = compareAtkB;
            this.compareDefB = compareDefB;
            this.compareSpaB = compareSpaB;
            this.compareSpdB = compareSpdB;
            this.compareSpeB = compareSpeB;

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
            Natures = natures;
            this.ability = ability;
            this.shinyOnly = shinyOnly;
            ABcheck = checkparents;
            GenderFilter = genderFilter;
        }

        public FrameCompare(
            uint compareHpA,
            uint compareAtkA,
            uint compareDefA,
            uint compareSpaA,
            uint compareSpdA,
            uint compareSpeA,
            uint compareHpB,
            uint compareAtkB,
            uint compareDefB,
            uint compareSpaB,
            uint compareSpdB,
            uint compareSpeB,
            IVFilter ivBase,
            List<uint> natures,
            int ability,
            bool shinyOnly,
            bool checkparents,
            GenderFilter genderFilter)
        {
            comparers = new List<FrameCompare>();

            this.compareHpA = compareHpA;
            this.compareAtkA = compareAtkA;
            this.compareDefA = compareDefA;
            this.compareSpaA = compareSpaA;
            this.compareSpdA = compareSpdA;
            this.compareSpeA = compareSpeA;

            this.compareHpB = compareHpB;
            this.compareAtkB = compareAtkB;
            this.compareDefB = compareDefB;
            this.compareSpaB = compareSpaB;
            this.compareSpdB = compareSpdB;
            this.compareSpeB = compareSpeB;

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
            Natures = natures;
            this.ability = ability;
            this.shinyOnly = shinyOnly;
            ABcheck = checkparents;
            GenderFilter = genderFilter;
        }

        public FrameCompare(
            uint hpValue,
            CompareType hpCompare,
            uint atkValue,
            CompareType atkCompare,
            uint defValue,
            CompareType defCompare,
            uint spaValue,
            CompareType spaCompare,
            uint spdValue,
            CompareType spdCompare,
            uint speValue,
            CompareType speCompare,
            List<uint> natures,
            int ability,
            bool shinyOnly,
            bool synchOnly,
            bool dreamWorld,
            List<int> encounterSlots,
            GenderFilter genderFilter)
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
            Natures = natures;
            this.ability = ability;
            this.shinyOnly = shinyOnly;
            this.synchOnly = synchOnly;
            this.dreamWorld = dreamWorld;
            this.encounterSlots = encounterSlots;

            GenderFilter = genderFilter;
        }

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

        // someday, dynamic frame comparers will be added
        public FrameCompare()
        {
            comparers = new List<FrameCompare>();
        }

        public GenderFilter GenderFilter { get; private set; }

        public List<uint> Natures { get; private set; }

        public void Add(FrameCompare comparer)
        {
            comparers.Add(comparer);
        }

        public void Remove(FrameCompare comparer)
        {
            comparers.Remove(comparer);
        }

        public bool CompareNature(uint testNature)
        {
            //  Check the nature first
            return Natures == null || Natures.Any(nature => nature == testNature);
        }

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

            // For 3rd Gen eggs - if an egg is not generated on that frame, ignore it
            if (frame.FrameType == FrameType.RSBredLower || frame.FrameType == FrameType.FRLGBredLower)
            {
                // need to replace this now that we're using numbered natures
                if (frame.Number == 0)
                    return false;
            }

            //  Go through and check each IV against what the user has required.
            //  Skip if it's a FrameType that doesn't use IVs
            if (frame.FrameType != FrameType.Method5Natures &&
                frame.FrameType != FrameType.Gen4Normal &&
                frame.FrameType != FrameType.Gen4International &&
                frame.FrameType != FrameType.RSBredLower)
            {
                if (ABcheck)
                {
                    uint frameIv = frame.Hp;

                    if (frame.DisplayHp == "A") frameIv = compareHpA;
                    if (frame.DisplayHp == "B") frameIv = compareHpB;

                    if (!CompareIV(hpCompare, frameIv, hpValue))
                        return false;


                    frameIv = frame.Atk;
                    if (frame.DisplayAtk == "A") frameIv = compareAtkA;
                    if (frame.DisplayAtk == "B") frameIv = compareAtkB;

                    if (!CompareIV(atkCompare, frameIv, atkValue))
                        return false;


                    frameIv = frame.Def;
                    if (frame.DisplayDef == "A") frameIv = compareDefA;
                    if (frame.DisplayDef == "B") frameIv = compareDefB;


                    if (!CompareIV(defCompare, frameIv, defValue))
                        return false;


                    frameIv = frame.Spa;
                    if (frame.DisplaySpa == "A") frameIv = compareSpaA;
                    if (frame.DisplaySpa == "B") frameIv = compareSpaB;


                    if (!CompareIV(spaCompare, frameIv, spaValue))
                        return false;


                    frameIv = frame.Spd;
                    if (frame.DisplaySpd == "A") frameIv = compareSpdA;
                    if (frame.DisplaySpd == "B") frameIv = compareSpdB;


                    if (!CompareIV(spdCompare, frameIv, spdValue))
                        return false;


                    frameIv = frame.Spe;
                    if (frame.DisplaySpe == "A") frameIv = compareSpeA;
                    if (frame.DisplaySpe == "B") frameIv = compareSpeB;


                    if (!CompareIV(speCompare, frameIv, speValue))
                        return false;
                }

                else
                {
                    if (frame.DisplayHp != "A" && frame.DisplayHp != "B")
                    {
                        if (!CompareIV(hpCompare, frame.Hp, hpValue))
                            return false;
                    }

                    if (frame.DisplayAtk != "A" && frame.DisplayAtk != "B")
                    {
                        if (!CompareIV(atkCompare, frame.Atk, atkValue))
                            return false;
                    }

                    if (frame.DisplayDef != "A" && frame.DisplayDef != "B")
                    {
                        if (!CompareIV(defCompare, frame.Def, defValue))
                            return false;
                    }

                    if (frame.DisplaySpa != "A" && frame.DisplaySpa != "B")
                    {
                        if (!CompareIV(spaCompare, frame.Spa, spaValue))
                            return false;
                    }

                    if (frame.DisplaySpd != "A" && frame.DisplaySpd != "B")
                    {
                        if (!CompareIV(spdCompare, frame.Spd, spdValue))
                            return false;
                    }

                    if (frame.DisplaySpe != "A" && frame.DisplaySpe != "B")
                    {
                        if (!CompareIV(speCompare, frame.Spe, speValue))
                            return false;
                    }
                }
            }
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

        public bool CompareEggIVs(Frame frame)
        {
            // Checks to see if the frame has at least three IVs that fit criteria
            // For 5th Gen Eggs

            int passCount = 0;

            if (CompareIV(hpCompare, frame.Hp, hpValue))
                passCount++;

            if (CompareIV(atkCompare, frame.Atk, atkValue))
                passCount++;

            if (CompareIV(defCompare, frame.Def, defValue))
                passCount++;

            if (CompareIV(spaCompare, frame.Spa, spaValue))
                passCount++;

            if (CompareIV(spdCompare, frame.Spd, spdValue))
                passCount++;

            if (CompareIV(speCompare, frame.Spe, speValue))
                passCount++;

            return passCount >= 3;
        }


        public bool CompareIV(CompareType compare, uint frameIv, uint testIv)
        {
            bool passed = true;

            //  Anything set not to compare is considered pass
            if (compare != CompareType.None)
            {
                switch (compare)
                {
                    case CompareType.Equal:
                        if (frameIv != testIv)
                            passed = false;
                        break;

                    case CompareType.GtEqual:
                        if (frameIv < testIv)
                            passed = false;
                        break;

                    case CompareType.LtEqual:
                        if (frameIv > testIv)
                            passed = false;
                        break;

                    case CompareType.NotEqual:
                        if (frameIv == testIv)
                            passed = false;
                        break;

                    case CompareType.Even:
                        if ((frameIv & 1) != 0)
                            passed = false;

                        break;

                    case CompareType.Odd:
                        if ((frameIv & 1) == 0)
                            passed = false;

                        break;

                    case CompareType.Hidden:
                        if ((((frameIv + 2) & 3) != 0) && (((frameIv + 5) & 3) != 0))
                            passed = false;
                        break;

                    case CompareType.HiddenEven:
                        if (((frameIv + 2) & 3) != 0)
                            passed = false;
                        break;

                    case CompareType.HiddenOdd:
                        if (((frameIv + 5) & 3) != 0)
                            passed = false;
                        break;
                }
            }

            return passed;
        }

        public bool CompareIV(int index, uint frameIv)
        {
            bool passed = true;

            uint testIv;
            CompareType compare;
            switch (index)
            {
                case 0:
                    testIv = hpValue;
                    compare = hpCompare;
                    break;
                case 1:
                    testIv = atkValue;
                    compare = atkCompare;
                    break;
                case 2:
                    testIv = defValue;
                    compare = defCompare;
                    break;
                case 3:
                    testIv = spaValue;
                    compare = spaCompare;
                    break;
                case 4:
                    testIv = spdValue;
                    compare = spdCompare;
                    break;
                case 5:
                    testIv = speValue;
                    compare = speCompare;
                    break;
                default:
                    testIv = hpValue;
                    compare = hpCompare;
                    break;
            }

            //  Anything set not to compare is considered pass
            if (compare != CompareType.None)
            {
                switch (compare)
                {
                    case CompareType.Equal:
                        if (frameIv != testIv)
                            passed = false;
                        break;

                    case CompareType.GtEqual:
                        if (frameIv < testIv)
                            passed = false;
                        break;

                    case CompareType.LtEqual:
                        if (frameIv > testIv)
                            passed = false;
                        break;

                    case CompareType.NotEqual:
                        if (frameIv == testIv)
                            passed = false;
                        break;

                    case CompareType.Even:
                        if ((frameIv & 1) != 0)
                            passed = false;

                        break;

                    case CompareType.Odd:
                        if ((frameIv & 1) == 0)
                            passed = false;

                        break;

                    case CompareType.Hidden:
                        if ((((frameIv + 2) & 3) != 0) && (((frameIv + 5) & 3) != 0))
                            passed = false;
                        break;

                    case CompareType.HiddenEven:
                        if (((frameIv + 2) & 3) != 0)
                            passed = false;
                        break;

                    case CompareType.HiddenOdd:
                        if (((frameIv + 5) & 3) != 0)
                            passed = false;
                        break;

                    case CompareType.HiddenTrickRoom:
                        if (frameIv != 2 && frameIv != 3)
                            passed = false;
                        break;
                }
            }

            return passed;
        }
    }

    internal class FrameGenerator
    {
        protected Frame frame;
        protected FrameType frameType = FrameType.Method1;
        protected List<Frame> frames;
        private uint lastseed;
        protected uint maxResults;
        protected IRNG mt;
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

        public uint Calibration { get; set; }

        public FrameType FrameType
        {
            get { return frameType; }
            set
            {
                frameType = value;
            }
        }

        public EncounterType EncounterType { get; set; }

        public EncounterMod EncounterMod { get; set; }

        public bool Everstone { get; set; }

        public int SynchNature { get; set; }

        public ulong InitialSeed { get; set; }

        public uint InitialFrame { get; set; }

        public uint MaxResults
        {
            get { return maxResults; }
            set
            {
                maxResults = value;
            }
        }

        public byte MotherAbility { get; set; }

        public bool DittoUsed { get; set; }

        public bool MaleOnlySpecies { get; set; }

        public bool ShinyCharm { get; set; }

        public uint[] ParentA { get; set; }

        public uint[] ParentB { get; set; }

        public uint[] RNGIVs { get; set; }

        public bool isBW2 { get; set; }

        //a static portion of the PID
        //either lower (R/S eggs) or the entire PID (E eggs)
        public uint StaticPID { get; set; }

        public uint Compatibility { get; set; }

        // by declaring these only once you get a huge performance boost

        // This method ensures that an RNG is only created once,
        // and not every time a Generate function is called


        public List<Frame> Generate(
            FrameCompare frameCompare,
            uint hp,
            uint atk,
            uint def,
            uint spa,
            uint spd,
            uint spe,
            List<uint> natures,
            uint minEfgh,
            uint maxEfgh,
            uint id,
            uint sid)
        {
            frames = new List<Frame>();
            var candidates = new List<Frame>();

            var rng = new PokeRngR(0);

            uint x_test = spe | (spa << 5) | (spd << 10);
            uint y_test = hp | (atk << 5) | (def << 10);

            #region

            // Experimentally derived
            // Any possible test seed will have at most
            // a difference of 0x31 from the target seed.
            // If it's close enough, we can then modify it
            // to match.


            /*
            for (uint cnt = 0xFFFF; cnt > 0xF2CC; cnt--)
            {
                uint seed = (x_test << 16) | cnt;                

                // Do a quick search for matching seeds
                // with a lower 16-bits between 0xFFFF and 0xF2CD.
                // We'll take the closest matches and subtract 0xD33
                // until it produces the correct seed (or doesn't).

                // Best we can do until we find a way to
                // calculate them directly.

                rng.Seed = seed;
                uint rng1 = rng.GetNext16BitNumber();

                // We don't have to worry about unsigned overflow
                // because y_test is never more than 0x7FFF
                if (y_test < 0x31)
                {
                    if (rng1 <= (y_test - 0x31))
                    {
                        while ((seed & 0xFFFF) > 0xD32 && (rng1 & 0x7FFF) < y_test)
                        {
                            seed = seed - 0xD33;
                            rng.Seed = seed;
                            rng1 = rng.GetNext16BitNumber();
                        }
                    }
                }
                else
                {
                    if (rng1 >= (y_test - 0x31))
                    {
                        while ((seed & 0xFFFF) > 0xD32 && (rng1 & 0x7FFF) < y_test)
                        {
                            seed = seed - 0xD33;
                            rng.Seed = seed;
                            rng1 = rng.GetNext16BitNumber();
                        }
                    }
                }
                */

            #endregion

            for (uint cnt = 0x0; cnt < 0xFFFF; cnt++)
            {
                uint seed = (x_test << 16) | cnt;

                rng.Seed = seed;
                uint rng1 = rng.GetNext16BitNumber();
                // Check to see if the next frame yields
                // the HP, Attack, and Defense IVs we're searching for
                // If not, skip 'em.
                if ((rng1 & 0x7FFF) != y_test)
                    continue;

                //  We have a max of 5 total RNG calls
                //  to make a pokemon and we already have
                //  one so lets go ahead and get 4 more.
                uint seedWondercard = rng.GetNext32BitNumber();
                var rng2 = seedWondercard >> 16;
                uint rng3 = rng.GetNext16BitNumber();
                uint rng4 = rng.GetNext16BitNumber();

                uint method1Seed = rng.Seed;

                // Instead of re-searching the entire space for seeds that are
                // basically identical except for the upper bit, we'll
                // just flip the upper seed bits instead.
                for (int upperBit = 0; upperBit < 2; upperBit++)
                {
                    rng2 = (ushort)(rng2 ^ 0x8000);
                    rng3 = (ushort)(rng3 ^ 0x8000);
                    rng4 = (ushort)(rng4 ^ 0x8000);
                    method1Seed = method1Seed ^ 0x80000000;
                    rng.Seed = rng.Seed ^ 0x80000000;

                    if (frameType == FrameType.WondercardIVs)
                    {
                        seedWondercard = seedWondercard ^ 0x80000000;
                        frame = Frame.GenerateFrame(seedWondercard,
                                                    frameType, EncounterType,
                                                    0,
                                                    seedWondercard,
                                                    0, 0,
                                                    rng1, x_test,
                                                    id, sid,
                                                    0, 0);

                        candidates.Add(frame);
                    }

                    foreach (uint nature in natures)
                    {
                        

                        if (frameType == FrameType.Method1)
                        {
                            //  Check Method 1
                            // [PID] [PID] [IVs] [IVs]
                            // [rng3] [rng2] [rng1] [START]

                            if (IVtoSeed.CheckPID(rng2, rng3, nature))
                            {
                                frame = Frame.GenerateFrame(method1Seed,
                                                            frameType, EncounterType,
                                                            0,
                                                            method1Seed,
                                                            rng3, rng2,
                                                            rng1, x_test,
                                                            id, sid,
                                                            0, 0);

                                candidates.Add(frame);
                            }
                        }
                    }
                }
            }

            // Now that we have some possibilities for frames,
            // We'll filter out ones that don't meet user criteria
            // Then roll back the RNG for each of them to make sure
            // each is within the user-specified maximum frames
            // from a DPPtHGSS-style seed.
            foreach (Frame candidate in candidates)
            {
                if (frameCompare.Compare(candidate))
                {
                    // start backing up frames until the user-specified max
                    rng.Seed = candidate.Seed;

                    const uint start = 1;

                    for (uint backCount = start; backCount <= MaxResults; backCount++)
                    {
                        uint testSeed = rng.Seed;

                        //uint seedAB = testSeed >> 24;
                        uint seedCD = (testSeed & 0x00FF0000) >> 16;
                        uint seedEFGH = testSeed & 0x0000FFFF;

                        // Check to see if seed follows ABCDEFGH format
                        // And matches-user specified delay
                        if (seedEFGH > minEfgh && seedEFGH < maxEfgh)
                        {
                            // CD must be between 0-23
                            if (seedCD < 23)
                            {
                                if (backCount >= InitialFrame)
                                {
                                    Frame frameCopy = Frame.Clone(candidate);

                                    frameCopy.Seed = testSeed;
                                    frameCopy.Number = backCount;
                                    frames.Add(frameCopy);
                                }
                            }
                        }

                        rng.GetNext32BitNumber();
                    }
                }
            }

            return frames;
        }

        // This is the smarter way of generating spreads
        // Takes a desired spread and derives the seed
        // Rather than searching all spreads for a match
        public List<Frame> Generate(
            uint minhp,
            uint maxhp,
            uint minatk,
            uint maxatk,
            uint mindef,
            uint maxdef,
            uint minspa,
            uint maxspa,
            uint minspd,
            uint maxspd,
            uint minspe,
            uint maxspe,
            uint nature)
        {
            var frames = new List<Frame>();
            var rngXD = new XdRng(0);

            for (uint hp = minhp; hp <= maxhp; hp++)
            {
                for (uint atk = minatk; atk <= maxatk; atk++)
                {
                    for (uint def = mindef; def <= maxdef; def++)
                    {
                        for (uint spa = minspa; spa <= maxspa; spa++)
                        {
                            for (uint spd = minspd; spd <= maxspd; spd++)
                            {
                                for (uint spe = minspe; spe <= maxspe; spe++)
                                {
                                    frame = null;

                                    uint x_test = hp | (atk << 5) | (def << 10);
                                    uint y_test = spe | (spa << 5) | (spd << 10);

                                    //  Now we want to start with IV2 and call the RNG for
                                    //  values between 0 and FFFF in the low order bits.
                                    for (uint cnt = 0; cnt <= 0xFFFF; cnt++)
                                    {
                                        //  Set our test seed here so we can start
                                        //  working backwards to see if the rest
                                        //  of the information we were provided 
                                        //  is a match.

                                        uint seed = (x_test << 16) | cnt;

                                        rngXD.Seed = seed;

                                        //  We have a max of 5 total RNG calls
                                        //  to make a pokemon and we already have
                                        //  one so lets go ahead and get 4 more.
                                        uint rng1XD = rngXD.GetNext16BitNumber();
                                        if ((rng1XD & 0x7FFF) != y_test)
                                            continue;

                                        // this second call isn't used for anything we know of
                                        uint rng2XD = rngXD.GetNext16BitNumber();
                                        uint rng3XD = rngXD.GetNext16BitNumber();
                                        uint rng4XD = rngXD.GetNext16BitNumber();

                                        uint XDColoSeed = seed * 0xB9B33155 + 0xA170F641;

                                        //  Check Colosseum\XD
                                        // [IVs] [IVs] [xxx] [PID] [PID]
                                        // [START] [rng1] [rng3] [rng4]

                                        for (int highBit = 0; highBit < 2; highBit++)
                                        {
                                            XDColoSeed = XDColoSeed ^ 0x80000000;
                                            rng3XD = rng3XD ^ 0x8000;
                                            rng4XD = rng4XD ^ 0x8000;

                                            if (((rng3XD << 16) | rng4XD) % 25 == nature)
                                            {
                                                frame = Frame.GenerateFrame
                                                    (XDColoSeed,
                                                     FrameType.ColoXD,
                                                     0,
                                                     XDColoSeed,
                                                     rng4XD,
                                                     rng3XD,
                                                     x_test,
                                                     rng1XD,
                                                     0, 0, 0, 0, 0, 0, 0, 0, 0);
                                                frame.DisplayPrep();

                                                frames.Add(frame); break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return frames;
        }

        // This generates a single frame, using IVs recalled from a stored hashtable
        public List<Frame> Generate(FrameCompare frameCompare, uint seed, uint IVHash, uint frameNumber)
        {
            frames = new List<Frame>();

            frame = Frame.GenerateFrame(
                FrameType.Method5Standard,
                frameNumber,
                seed,
                (IVHash & 0x1F),
                (IVHash & 0x3E0) >> 5,
                (IVHash & 0x7C00) >> 10,
                ((IVHash >> 16) & 0x3E0) >> 5,
                ((IVHash >> 16) & 0x7C00) >> 10,
                ((IVHash >> 16) & 0x1F));

            if (frameCompare.Compare(frame))
            {
                frames.Add(frame);
            }

            return frames;
        }

        public List<Frame> Generate(
            FrameCompare frameCompare,
            uint id,
            uint sid)
        {
            frames = new List<Frame>();

            //  The first thing we need to do is check for
            //  whether we are using the LCRNG or MTRNG
            if (frameType == FrameType.Gen4Normal ||
                frameType == FrameType.Gen4International)
            {
                mt.Reseed((uint)InitialSeed);
                frame = null;

                for (uint cnt = 1; cnt < InitialFrame + maxResults; cnt++)
                {
                    if (cnt < InitialFrame)
                    {
                        mt.Nextuint();
                        continue;
                    }

                    switch (frameType)
                    {
                        case FrameType.Gen4Normal:
                            uint mtResult = mt.Nextuint();

                            frame =
                                Frame.GenerateFrame(
                                    FrameType.Gen4Normal,
                                    cnt,
                                    mtResult,
                                    mtResult,
                                    id, sid);

                            break;

                        case FrameType.Gen4International:

                            //  We want to get our random number
                            //  first and then go through and check
                            //  to see if it is shiny.
                            uint pid = mt.Nextuint();

                            for (int n = 0; n <= 3; n++)
                            {
                                uint tid = (id & 0xffff) | ((sid & 0xffff) << 16);

                                uint a = pid ^ tid;
                                uint b = a & 0xffff;
                                uint c = (a >> 16) & 0xffff;
                                uint d = b ^ c;

                                if (d < 8)
                                {
                                    break;
                                }

                                // ARNG
                                pid = pid * 0x6c078965 + 1;
                            }

                            frame =
                                Frame.GenerateFrame(
                                    FrameType.Gen4International,
                                    cnt,
                                    pid,
                                    pid,
                                    id, sid);

                            break;
                    }


                    if (frameCompare.Compare(frame))
                    {
                        frames.Add(frame); break;
                    }
                }
            }
            else if (frameType == FrameType.Method5Standard)
            {
                mt.Reseed((uint)InitialSeed);

                for (uint cnt = 1; cnt < InitialFrame; cnt++)
                    mt.Nextuint();

                for (int i = 0; i < 7; i++)
                    rngList.Add(mt.Nextuint() >> 27);

                for (uint cnt = 0; cnt < maxResults; cnt++, rngList.RemoveAt(0), rngList.Add(mt.Nextuint() >> 27))
                {
                    if (EncounterType == EncounterType.Roamer)
                    {
                        if (!frameCompare.CompareIV(0, rngList[1]))
                            continue;
                        if (!frameCompare.CompareIV(1, rngList[2]))
                            continue;
                        if (!frameCompare.CompareIV(2, rngList[3]))
                            continue;
                        if (!frameCompare.CompareIV(3, rngList[6]))
                            continue;
                        if (!frameCompare.CompareIV(4, rngList[4]))
                            continue;
                        if (!frameCompare.CompareIV(5, rngList[5]))
                            continue;

                        frame =
                            Frame.GenerateFrame(
                                FrameType.Method5Standard,
                                cnt + InitialFrame,
                                (uint)InitialSeed,
                                rngList[1],
                                rngList[2],
                                rngList[3],
                                rngList[6],
                                rngList[4],
                                rngList[5]);
                    }
                    else
                    {
                        if (!frameCompare.CompareIV(0, rngList[0]))
                            continue;
                        if (!frameCompare.CompareIV(1, rngList[1]))
                            continue;
                        if (!frameCompare.CompareIV(2, rngList[2]))
                            continue;
                        if (!frameCompare.CompareIV(3, rngList[3]))
                            continue;
                        if (!frameCompare.CompareIV(4, rngList[4]))
                            continue;
                        if (!frameCompare.CompareIV(5, rngList[5]))
                            continue;

                        frame =
                            Frame.GenerateFrame(
                                FrameType.Method5Standard,
                                cnt + InitialFrame,
                                (uint)InitialSeed,
                                rngList[0],
                                rngList[1],
                                rngList[2],
                                rngList[3],
                                rngList[4],
                                rngList[5]);
                    }

                    frames.Add(frame); break;
                }
            }
            else if (frameType == FrameType.Method5CGear)
            {
                mt.Reseed((uint)InitialSeed);

                // first two frames are skipped
                mt.Nextuint();
                mt.Nextuint();

                for (uint cnt = 1; cnt < InitialFrame; cnt++)
                    mt.Nextuint();

                for (int i = 0; i < 7; i++)
                    rngList.Add(mt.Nextuint() >> 27);

                for (uint cnt = 0; cnt < maxResults; cnt++, rngList.RemoveAt(0), rngList.Add(mt.Nextuint() >> 27))
                {
                    frame = null;

                    if (EncounterType == EncounterType.Roamer)
                    {
                        if (!frameCompare.CompareIV(0, rngList[1]))
                            continue;
                        if (!frameCompare.CompareIV(1, rngList[2]))
                            continue;
                        if (!frameCompare.CompareIV(2, rngList[3]))
                            continue;
                        if (!frameCompare.CompareIV(3, rngList[6]))
                            continue;
                        if (!frameCompare.CompareIV(4, rngList[4]))
                            continue;
                        if (!frameCompare.CompareIV(5, rngList[5]))
                            continue;

                        frame =
                            Frame.GenerateFrame(
                                FrameType.Method5CGear,
                                cnt + InitialFrame,
                                (uint)InitialSeed,
                                rngList[1],
                                rngList[2],
                                rngList[3],
                                rngList[6],
                                rngList[4],
                                rngList[5]);
                    }
                    else
                    {
                        if (!frameCompare.CompareIV(0, rngList[0]))
                            continue;
                        if (!frameCompare.CompareIV(1, rngList[1]))
                            continue;
                        if (!frameCompare.CompareIV(2, rngList[2]))
                            continue;
                        if (!frameCompare.CompareIV(3, rngList[3]))
                            continue;
                        if (!frameCompare.CompareIV(4, rngList[4]))
                            continue;
                        if (!frameCompare.CompareIV(5, rngList[5]))
                            continue;

                        frame =
                            Frame.GenerateFrame(
                                FrameType.Method5CGear,
                                cnt + InitialFrame,
                                (uint)InitialSeed,
                                rngList[0],
                                rngList[1],
                                rngList[2],
                                rngList[3],
                                rngList[4],
                                rngList[5]);
                    }

                    frames.Add(frame); break;
                }
            }
            else if (frameType == FrameType.ColoXD)
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
                    Frame frameSplit = null;

                    switch (frameType)
                    {
                        case FrameType.Method1:
                            frame =
                                Frame.GenerateFrame(
                                    0,
                                    FrameType.Method1,
                                    cnt + InitialFrame,
                                    rngList[0],
                                    rngList[0],
                                    rngList[1],
                                    rngList[2],
                                    rngList[3],
                                    0, 0, 0, 0, 0, 0,
                                    id, sid, cnt);

                            break;

                        case FrameType.Method1Reverse:
                            frame =
                                Frame.GenerateFrame(
                                    0,
                                    FrameType.Method1,
                                    cnt + InitialFrame,
                                    rngList[0],
                                    rngList[1],
                                    rngList[0],
                                    rngList[2],
                                    rngList[3],
                                    0, 0, 0, 0, 0, 0,
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
                                    0, 0, 0, 0, 0, 0,
                                    id, sid, cnt);

                            break;

                        case FrameType.Method3:
                            frame =
                                Frame.GenerateFrame(
                                    0,
                                    FrameType.Method3,
                                    cnt + InitialFrame,
                                    rngList[0],
                                    rngList[0],
                                    rngList[2],
                                    rngList[3],
                                    rngList[4],
                                    0, 0, 0, 0, 0, 0,
                                    id, sid, cnt);

                            break;

                        case FrameType.Method4:
                            frame =
                                Frame.GenerateFrame(
                                    0,
                                    FrameType.Method4,
                                    cnt + InitialFrame,
                                    rngList[0],
                                    rngList[0],
                                    rngList[1],
                                    rngList[2],
                                    rngList[4],
                                    0, 0, 0, 0, 0, 0,
                                    id, sid, cnt);

                            break;

                        
                        case FrameType.EBredPID:
                            uint pid = GetEPid(cnt, out uint total);
                            if (pid == 0)
                            {
                                continue;
                            }
                            //generate frame with bogus RNG result
                            frame = Frame.GenerateFrame(FrameType.EBredPID, cnt + InitialFrame, 0, pid, id, sid);
                            frame.Advances = total;
                            //new Frame {FrameType = FrameType.EBredPID, Number = cnt + InitialFrame, Pid = pid};
                            break;

                        case FrameType.RSBredLower:
                            frame =
                                Frame.GenerateFrame(
                                    FrameType.RSBredLower,
                                    cnt + InitialFrame,
                                    rngList[18],
                                    rngList[19],
                                    Compatibility
                                    );
                            break;

                        case FrameType.RSBredUpper:
                            frame =
                                Frame.GenerateFrame(
                                    FrameType.RSBredUpper,
                                    cnt + InitialFrame,
                                    rngList[0],
                                    StaticPID,
                                    rngList[3],
                                    rngList[5],
                                    rngList[6],
                                    // vblank
                                    rngList[8],
                                    rngList[9],
                                    rngList[10],
                                    rngList[11],
                                    rngList[12],
                                    rngList[13],
                                    ParentA,
                                    ParentB,
                                    id, sid);
                            break;

                        case FrameType.FRLGBredLower:
                            frame =
                                Frame.GenerateFrame(
                                    FrameType.FRLGBredLower,
                                    cnt + InitialFrame,
                                    rngList[18],
                                    rngList[19],
                                    Compatibility
                                    );
                            break;

                        case FrameType.FRLGBredUpper:
                            frame =
                                Frame.GenerateFrame(
                                    FrameType.FRLGBredUpper,
                                    cnt + InitialFrame,
                                    rngList[0],
                                    StaticPID,
                                    rngList[3],
                                    rngList[5],
                                    rngList[6],
                                    // vblank
                                    rngList[8],
                                    rngList[9],
                                    rngList[10],
                                    rngList[11],
                                    rngList[12],
                                    rngList[13],
                                    ParentA,
                                    ParentB,
                                    id, sid);
                            break;
                        case FrameType.Bred:

                            frame =
                                Frame.GenerateFrame(
                                    FrameType.Bred,
                                    cnt + InitialFrame,
                                    rngList[0],
                                    StaticPID,
                                    rngList[4],
                                    rngList[5],
                                    rngList[7],
                                    rngList[8],
                                    rngList[9],
                                    rngList[10],
                                    rngList[11],
                                    rngList[12],
                                    ParentA,
                                    ParentB,
                                    id, sid, cnt);

                            break;

                        case FrameType.BredSplit:
                            //  This is going to add both of the frames and 
                            //  the logic below will decide whether to add 
                            //  it to the output.

                            frame =
                                Frame.GenerateFrame(
                                    FrameType.Bred,
                                    cnt + InitialFrame,
                                    rngList[0],
                                    StaticPID,
                                    rngList[4],
                                    rngList[5],
                                    //  Garbage
                                    rngList[7],
                                    rngList[8],
                                    rngList[9],
                                    rngList[10],
                                    rngList[11],
                                    rngList[12],
                                    ParentA,
                                    ParentB,
                                    id, sid, cnt);

                            frameSplit =
                                Frame.GenerateFrame(
                                    FrameType.BredSplit,
                                    cnt + InitialFrame,
                                    rngList[0],
                                    StaticPID,
                                    rngList[4],
                                    rngList[6],
                                    //  Garbage
                                    rngList[8],
                                    rngList[9],
                                    rngList[10],
                                    rngList[11],
                                    rngList[12],
                                    rngList[13],
                                    ParentA,
                                    ParentB,
                                    id, sid, cnt);

                            break;

                        case FrameType.BredAlternate:

                            frame =
                                Frame.GenerateFrame(
                                    FrameType.Bred,
                                    cnt + InitialFrame,
                                    rngList[0],
                                    StaticPID,
                                    rngList[4],
                                    rngList[5],
                                    rngList[8],
                                    rngList[9],
                                    rngList[10],
                                    rngList[11],
                                    rngList[12],
                                    rngList[13],
                                    ParentA,
                                    ParentB,
                                    id, sid, cnt);

                            break;

                        case FrameType.DPPtBred:

                            frame =
                                Frame.GenerateFrame(
                                    FrameType.DPPtBred,
                                    cnt + InitialFrame,
                                    rngList[0],
                                    rngList[0],
                                    rngList[1],
                                    rngList[2],
                                    rngList[3],
                                    rngList[4],
                                    rngList[5],
                                    rngList[6],
                                    rngList[7],
                                    ParentA,
                                    ParentB,
                                    id, sid, cnt);

                            break;

                        case FrameType.HGSSBred:

                            frame =
                                Frame.GenerateFrame(
                                    FrameType.HGSSBred,
                                    cnt + InitialFrame,
                                    rngList[0],
                                    rngList[0],
                                    rngList[1],
                                    rngList[2],
                                    rngList[3],
                                    rngList[4],
                                    rngList[5],
                                    rngList[6],
                                    rngList[7],
                                    ParentA,
                                    ParentB,
                                    id, sid, cnt);

                            break;

                        case FrameType.WondercardIVs:

                            if (EncounterType == EncounterType.Manaphy)
                            {
                                uint pid1 = rngList[0];
                                uint pid2 = rngList[1];

                                while ((pid1 ^ pid2 ^ id ^ sid) < 8)
                                {
                                    uint testPID = pid1 | (pid2 << 16);

                                    // Call the ARNG to change the PID
                                    testPID = testPID * 0x6c078965 + 1;

                                    pid1 = testPID & 0xFFFF;
                                    pid2 = testPID >> 16;
                                }

                                frame =
                                    Frame.GenerateFrame(
                                        0,
                                        FrameType.WondercardIVs,
                                        cnt + InitialFrame,
                                        rngList[0],
                                        pid1,
                                        pid2,
                                        rngList[2],
                                        rngList[3],
                                        0, 0, 0, 0, 0, 0,
                                        id, sid, cnt);
                            }
                            else
                            {
                                frame =
                                    Frame.GenerateFrame(
                                        0,
                                        FrameType.WondercardIVs,
                                        cnt + InitialFrame,
                                        rngList[0],
                                        0,
                                        0,
                                        rngList[0],
                                        rngList[1],
                                        rngList[2],
                                        rngList[3],
                                        rngList[4],
                                        rngList[5],
                                        rngList[6],
                                        rngList[7],
                                        id, sid, cnt);
                            }

                            break;
                    }


                    //  Now we need to filter and decide if we are going
                    //  to add this to our collection for display to the
                    //  user.

                    if (frameCompare.Compare(frame))
                    {
                        frames.Add(frame); break;
                    }

                    if (frameType == FrameType.BredSplit)
                    {
                        if (frameCompare.Compare(frameSplit))
                        {
                            frames.Add(frameSplit);
                        }
                    }
                }
            }

            return frames;
        }

        private uint GetEPid(uint cnt, out uint total)
        {
            total = 0;
            int i = 0;
            uint pid;
            // check for compatibility
            if ((rngList[i++] * 100) / 0xFFFF >= Compatibility) return 0;

            //check the everstone
            bool useEverstone = Everstone ? (rngList[i++] >> 15) == 0 : false;

            // set up the TRNG
            var trng = new PokeRng((cnt + InitialFrame - Calibration) & 0xFFFF);

            if (!useEverstone)
            {
                // generate lower
                if (rngList[i] > 0xFFFD)
                    pid = (rngList[i] + 3) % 0xFFFF;
                else
                    pid = (rngList[i] & 0xFFFF) + 1;

                // generate upper
                pid += trng.GetNext16BitNumber() * 0x10000;

                return pid;
            }

            do
            {
                //always appears to vblank at 17
                if (total == 17)
                    ++i;

                // check if we need to add to the rngArray
                // if we do add another 20 elements
                if (i >= rngList.Count)
                    AddToRngList();

                // generate lower
                pid = (rngList[i++] & 0xFFFF);

                // generate upper
                pid += trng.GetNext16BitNumber() * 0x10000;
                ++total;
            } while (pid % 0x19 != SynchNature);

            return pid;
        }

        private void AddToRngList()
        {
            int i = rngList.Count;

            // seed the new RNG with the last seed
            var rng = new PokeRng(lastseed);
            // add in the new elements
            for (; i < rngList.Count + 20; ++i)
                rngList.Add(rng.GetNext16BitNumber());

            lastseed = rng.Seed;
        }


        private static uint ForceShiny(uint pid, uint tid, uint sid)
        {
            uint lowByte = pid & 0x000000ff;
            return ((lowByte ^ tid ^ sid) << 16) | lowByte;
        }

        private static uint ForceNonShiny(uint pid, uint tid, uint sid)
        {
            if (((pid >> 16) ^ (pid & 0xffff) ^ sid ^ tid) < 8)
                pid = pid ^ 0x10000000;

            return pid;
        }

        #region Nested type: Compare

        protected delegate bool Compare(uint x);

        #endregion
    }

    /// <summary>
    ///     This class is going to do an IV/PID/Seed calculation given a particular method (1, 2 or 3, or 4). Should use the same code to develop candidate IVs.
    /// </summary>
    internal class IVtoSeed
    {
        //  We need a function to return a list of monster seeds,
        //  which will be updated to include a method.  


        public static List<Seed> GetXDSeeds(
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

            uint ivs1 = hp + (atk << 5) + (def << 10);
            uint ivs1_2 = ivs1 ^ 0x8000;
            uint ivs2 = spe | (spa << 5) | (spd << 10);
            uint ivs2_2 = ivs2 ^ 0x8000;
            uint x_testXD = ivs1 << 16;

            //  Now we want to start with IV2 and call the RNG for
            //  values between 0 and FFFF in the low order bits.
            for (uint cnt = 0; cnt <= 0xFFFF; cnt++)
            {
                //  Set our test seed here so we can start
                //  working backwards to see if the rest
                //  of the information we were provided 
                //  is a match.

                uint seed = x_testXD | cnt;
                var rngXD = new XdRng(seed);
                uint rng1XD = rngXD.GetNext16BitNumber();

                //Check if ivs line up
                if (rng1XD == ivs2 || rng1XD == ivs2_2)
                {

                    var rngXDR = new XdRngR(seed);
                    uint XDColoSeed = rngXDR.GetNext32BitNumber();

                    //  Right now, this simply assumes method
                    //  1 and gets the value previous to check
                    //  for  match.  We need a clean way to do
                    //  this for all of our methods.

                    //  We have a max of 5 total RNG calls
                    //  to make a pokemon and we already have
                    //  one so lets go ahead and get 4 more.
                    uint rng2XD = rngXD.GetNext16BitNumber();
                    uint rng3XD = rngXD.GetNext16BitNumber();
                    uint rng4XD = rngXD.GetNext16BitNumber();
                    uint pid = (rng3XD << 16) + rng4XD;

                    //  Check Colosseum\XD
                    // [IVs] [IVs] [xxx] [PID] [PID]
                    // [START] [rng1] [rng3] [rng4]

                    if (pid % 25 == nature)
                    {
                        var newSeed = new Seed
                        {
                            Method = "Colosseum/XD",
                            Pid = pid,
                            MonsterSeed = XDColoSeed
                        };
                        seeds.Add(newSeed);
                    }

                    //  Check Colosseum\XD XOR
                    // [IVs] [IVs] [xxx] [PID] [PID]
                    // [START] [rng1] [rng3] [rng4]
                    pid ^= 0x80008000;
                    if (pid % 25 == nature)
                    {
                        var newSeed = new Seed
                        {
                            Method = "Colosseum/XD",
                            Pid = pid,
                            MonsterSeed = XDColoSeed ^ 0x80000000
                        };
                        seeds.Add(newSeed);
                    }
                }
            }
            return seeds;
        }

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
            bool pass = false;

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
                    uint choppedPID;

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

                    //  DPPt Cute Charm
                    if (rng3 / 0x5556 != 0)
                    {
                        //  Check DPPt Cute Charm
                        //  [CC Check] [PID] [IVs] [IVs]
                        //  [rng3] [rng2] [rng1] [START]

                        choppedPID = rng2 / 0xA3E;
                        pass = choppedPID % 25 == nature;
                        if (pass)
                        {
                            var newSeed = new Seed
                            {
                                Method = "Cute Charm (DPPt)",
                                Pid = choppedPID,
                                MonsterSeed = method1Seed,
                                Sid = (choppedPID ^ id) & 0xFFF8
                            };
                            seeds.Add(newSeed);
                        }

                        //  Check DPPt Cute Charm (50% male)
                        //  [CC Check] [PID] [IVs] [IVs]
                        //  [rng3] [rng2] [rng1] [START]

                        //choppedPID = rng2 / 0xA3E + 0x96;
                        if (pass)
                        {
                            choppedPID += 0x96;
                            var newSeed = new Seed
                            {
                                Method = "Cute Charm (DPPt)",
                                Pid = choppedPID,
                                MonsterSeed = method1Seed,
                                Sid = (choppedPID ^ id) & 0xFFF8
                            };
                            seeds.Add(newSeed);
                        }

                        //  Check DPPt Cute Charm (25% male)
                        //  [CC Check] [PID] [IVs] [IVs]
                        //  [rng3] [rng2] [rng1] [START]

                        //choppedPID = rng2 / 0xA3E + 0xC8;
                        if (pass)
                        {
                            choppedPID += 0x32;
                            var newSeed = new Seed
                            {
                                Method = "Cute Charm (DPPt)",
                                Pid = choppedPID,
                                MonsterSeed = method1Seed,
                                Sid = (choppedPID ^ id) & 0xFFF8
                            };
                            seeds.Add(newSeed);
                        }

                        //  Check DPPt Cute Charm (75% male)
                        //  [CC Check] [PID] [IVs] [IVs]
                        //  [rng3] [rng2] [rng1] [START]

                        //choppedPID = rng2 / 0xA3E + 0x4B;
                        if (pass)
                        {
                            choppedPID -= 0x7D;
                            var newSeed = new Seed
                            {
                                Method = "Cute Charm (DPPt)",
                                Pid = choppedPID,
                                MonsterSeed = method1Seed,
                                Sid = (choppedPID ^ id) & 0xFFF8
                            };
                            seeds.Add(newSeed);
                        }

                        //  Check DPPt Cute Charm (87.5% male)
                        //  [CC Check] [PID] [IVs] [IVs]
                        //  [rng3] [rng2] [rng1] [START]

                        //choppedPID = rng2 / 0xA3E + 0x32;
                        if (pass)
                        {
                            choppedPID -= 0x19;
                            var newSeed = new Seed
                            {
                                Method = "Cute Charm (DPPt)",
                                Pid = choppedPID,
                                MonsterSeed = method1Seed,
                                Sid = (choppedPID ^ id) & 0xFFF8
                            };
                            seeds.Add(newSeed);
                        }
                    }

                    //  HGSS Cute Charm
                    if (rng3 % 3 != 0)
                    {
                        //  Check HGSS Cute Charm
                        //  [CC Check] [PID] [IVs] [IVs]
                        //  [rng3] [rng2] [rng1] [START]

                        choppedPID = rng2 % 25;
                        pass = choppedPID == nature;
                        if (pass)
                        {
                            var newSeed = new Seed
                            {
                                Method = "Cute Charm (HGSS)",
                                Pid = choppedPID,
                                MonsterSeed = method1Seed,
                                Sid = (choppedPID ^ id) & 0xFFF8
                            };
                            seeds.Add(newSeed);
                        }

                        //  Check HGSS Cute Charm (50% male)
                        //  [CC Check] [PID] [IVs] [IVs]
                        //  [rng3] [rng2] [rng1] [START]

                        //choppedPID = rng2 % 25 + 0x96;
                        if (pass)
                        {
                            choppedPID += 0x96;
                            var newSeed = new Seed
                            {
                                Method = "Cute Charm (HGSS)",
                                Pid = choppedPID,
                                MonsterSeed = method1Seed,
                                Sid = (choppedPID ^ id) & 0xFFF8
                            };
                            seeds.Add(newSeed);
                        }

                        //  Check HGSS Cute Charm (25% male)
                        //  [CC Check] [PID] [IVs] [IVs]
                        //  [rng3] [rng2] [rng1] [START]

                        //choppedPID = rng2 % 25 + 0xC8;
                        if (pass)
                        {
                            choppedPID += 0x32;
                            var newSeed = new Seed
                            {
                                Method = "Cute Charm (HGSS)",
                                Pid = choppedPID,
                                MonsterSeed = method1Seed,
                                Sid = (choppedPID ^ id) & 0xFFF8
                            };
                            seeds.Add(newSeed);
                        }

                        //  Check HGSS Cute Charm (75% male)
                        //  [CC Check] [PID] [IVs] [IVs]
                        //  [rng3] [rng2] [rng1] [START]

                        //choppedPID = rng2 % 25 + 0x4B;
                        if (pass)
                        {
                            choppedPID -= 0x7D;
                            var newSeed = new Seed
                            {
                                Method = "Cute Charm (HGSS)",
                                Pid = choppedPID,
                                MonsterSeed = method1Seed,
                                Sid = (choppedPID ^ id) & 0xFFF8
                            };
                            seeds.Add(newSeed);
                        }

                        //  Check HGSS Cute Charm (87.5% male)
                        //  [CC Check] [PID] [IVs] [IVs]
                        //  [rng3] [rng2] [rng1] [START]

                        //choppedPID = rng2 % 25 + 0x32;
                        if (pass)
                        {
                            choppedPID -= 0x19;
                            var newSeed = new Seed
                            {
                                Method = "Cute Charm (HGSS)",
                                Pid = choppedPID,
                                MonsterSeed = method1Seed,
                                Sid = (choppedPID ^ id) & 0xFFF8
                            };
                            seeds.Add(newSeed);
                        }
                    }

                    //  DPPt Cute Charm XOR
                    rng3 ^= 0x8000;
                    rng2 ^= 0x8000;
                    if (rng3 / 0x5556 != 0)
                    {
                        //  Check DPPt Cute Charm
                        //  [CC Check] [PID] [IVs] [IVs]
                        //  [rng3] [rng2] [rng1] [START]

                        choppedPID = rng2 / 0xA3E;
                        pass = choppedPID % 25 == nature;
                        if (pass)
                        {
                            var newSeed = new Seed
                            {
                                Method = "Cute Charm (DPPt)",
                                Pid = choppedPID,
                                MonsterSeed = method1SeedXor,
                                Sid = (choppedPID ^ id) & 0xFFF8
                            };
                            seeds.Add(newSeed);
                        }

                        //  Check DPPt Cute Charm (50% male)
                        //  [CC Check] [PID] [IVs] [IVs]
                        //  [rng3] [rng2] [rng1] [START]

                        //choppedPID = rng2 / 0xA3E + 0x96;
                        if (pass)
                        {
                            choppedPID += 0x96;
                            var newSeed = new Seed
                            {
                                Method = "Cute Charm (DPPt)",
                                Pid = choppedPID,
                                MonsterSeed = method1SeedXor,
                                Sid = (choppedPID ^ id) & 0xFFF8
                            };
                            seeds.Add(newSeed);
                        }

                        //  Check DPPt Cute Charm (25% male)
                        //  [CC Check] [PID] [IVs] [IVs]
                        //  [rng3] [rng2] [rng1] [START]

                        //choppedPID = rng2 / 0xA3E + 0xC8;
                        if (pass)
                        {
                            choppedPID += 0x32;
                            var newSeed = new Seed
                            {
                                Method = "Cute Charm (DPPt)",
                                Pid = choppedPID,
                                MonsterSeed = method1SeedXor,
                                Sid = (choppedPID ^ id) & 0xFFF8
                            };
                            seeds.Add(newSeed);
                        }

                        //  Check DPPt Cute Charm (75% male)
                        //  [CC Check] [PID] [IVs] [IVs]
                        //  [rng3] [rng2] [rng1] [START]

                        //choppedPID = rng2 / 0xA3E + 0x4B;
                        if (pass)
                        {
                            choppedPID -= 0x7D;
                            var newSeed = new Seed
                            {
                                Method = "Cute Charm (DPPt)",
                                Pid = choppedPID,
                                MonsterSeed = method1SeedXor,
                                Sid = (choppedPID ^ id) & 0xFFF8
                            };
                            seeds.Add(newSeed);
                        }

                        //  Check DPPt Cute Charm (87.5% male)
                        //  [CC Check] [PID] [IVs] [IVs]
                        //  [rng3] [rng2] [rng1] [START]

                        //choppedPID = rng2 / 0xA3E + 0x32;
                        if (pass)
                        {
                            choppedPID -= 0x19;
                            var newSeed = new Seed
                            {
                                Method = "Cute Charm (DPPt)",
                                Pid = choppedPID,
                                MonsterSeed = method1SeedXor,
                                Sid = (choppedPID ^ id) & 0xFFF8
                            };
                            seeds.Add(newSeed);
                        }
                    }

                    //  HGSS Cute Charm XOR
                    if (rng3 % 3 != 0)
                    {
                        //  Check HGSS Cute Charm
                        //  [CC Check] [PID] [IVs] [IVs]
                        //  [rng3] [rng2] [rng1] [START]

                        choppedPID = rng2 % 25;
                        pass = choppedPID == nature;
                        if (pass)
                        {
                            var newSeed = new Seed
                            {
                                Method = "Cute Charm (HGSS)",
                                Pid = choppedPID,
                                MonsterSeed = method1SeedXor,
                                Sid = (choppedPID ^ id) & 0xFFF8
                            };
                            seeds.Add(newSeed);
                        }

                        //  Check HGSS Cute Charm (50% male)
                        //  [CC Check] [PID] [IVs] [IVs]
                        //  [rng3] [rng2] [rng1] [START]

                        //choppedPID = rng2 % 25 + 0x96;
                        if (pass)
                        {
                            choppedPID += 0x96;
                            var newSeed = new Seed
                            {
                                Method = "Cute Charm (HGSS)",
                                Pid = choppedPID,
                                MonsterSeed = method1SeedXor,
                                Sid = (choppedPID ^ id) & 0xFFF8
                            };
                            seeds.Add(newSeed);
                        }

                        //  Check HGSS Cute Charm (25% male)
                        //  [CC Check] [PID] [IVs] [IVs]
                        //  [rng3] [rng2] [rng1] [START]

                        //choppedPID = rng2 % 25 + 0xC8;
                        if (pass)
                        {
                            choppedPID += 0x32;
                            var newSeed = new Seed
                            {
                                Method = "Cute Charm (HGSS)",
                                Pid = choppedPID,
                                MonsterSeed = method1SeedXor,
                                Sid = (choppedPID ^ id) & 0xFFF8
                            };
                            seeds.Add(newSeed);
                        }

                        //  Check HGSS Cute Charm (75% male)
                        //  [CC Check] [PID] [IVs] [IVs]
                        //  [rng3] [rng2] [rng1] [START]

                        //choppedPID = rng2 % 25 + 0x4B;
                        if (pass)
                        {
                            choppedPID -= 0x7D;
                            var newSeed = new Seed
                            {
                                Method = "Cute Charm (HGSS)",
                                Pid = choppedPID,
                                MonsterSeed = method1SeedXor,
                                Sid = (choppedPID ^ id) & 0xFFF8
                            };
                            seeds.Add(newSeed);
                        }

                        //  Check HGSS Cute Charm (87.5% male)
                        //  [CC Check] [PID] [IVs] [IVs]
                        //  [rng3] [rng2] [rng1] [START]

                        //choppedPID = rng2 % 25 + 0x32;
                        if (pass)
                        {
                            choppedPID -= 0x19;
                            var newSeed = new Seed
                            {
                                Method = "Cute Charm (HGSS)",
                                Pid = choppedPID,
                                MonsterSeed = method1SeedXor,
                                Sid = (choppedPID ^ id) & 0xFFF8
                            };
                            seeds.Add(newSeed);
                        }
                    }
                }
            }
            return seeds;
        }


        // Overloaded method for SeedFinder's Open Search
        public static List<Seed> GetSeeds(
            uint hp,
            uint atk,
            uint def,
            uint spa,
            uint spd,
            uint spe,
            uint nature)
        {
            var seeds = new List<Seed>();

            uint ivs2 = spe | (spa << 5) | (spd << 10);
            uint ivs2_2 = ivs2 ^ 0x8000;

            uint ivs1 = hp | (atk << 5) | (def << 10);
            uint ivs1_2 = ivs1 ^ 0x8000;

            uint x_test = ivs2 << 16;

            //  Now we want to start with IV2 and call the RNG for
            //  values between 0 and FFFF in the low order bits.
            for (uint cnt = 0; cnt <= 0xFFFF; cnt++)
            {
                //  Set our test seed here so we can start
                //  working backwards to see if the rest
                //  of the information we were provided 
                //  is a match.

                uint seed = x_test | (cnt & 0xFFFF);

                var rng = new PokeRngR(seed);

                //  Right now, this simply assumes method
                //  1 and gets the value previous to check
                //  for  match.  We need a clean way to do
                //  this for all of our methods.

                uint rng1 = rng.GetNext16BitNumber();

                //Check if ivs line up
                if (rng1 == ivs1 || rng1 == ivs1_2)
                {
                    //  We have a max of 5 total RNG calls
                    //  to make a pokemon and we already have
                    //  one so lets go ahead and get 4 more.
                    uint rng2 = rng.GetNext16BitNumber();
                    uint rng3 = rng.GetNext16BitNumber();
                    uint rng4 = rng.GetNext16BitNumber();
                    uint pid = (rng2 << 16) + rng3;
                    uint method1Seed = rng.Seed;

                    //  Check Method 1
                    // [PID] [PID] [IVs] [IVs]
                    // [rng3] [rng2] [rng1] [START]
                    if (pid % 25 == nature)
                    {
                        //  Build a seed to add to our collection
                        var newSeed = new Seed
                        {
                            Method = "Method 1",
                            Pid = pid,
                            MonsterSeed = method1Seed
                        };
                        seeds.Add(newSeed);
                    }

                    // Check Method 1 XOR
                    // [PID] [PID] [IVs] [IVs]
                    // [rng3] [rng2] [rng1] [START]
                    pid ^= 0x80008000;
                    if (pid % 25 == nature)
                    {
                        //  Build a seed to add to our collection
                        var newSeed = new Seed
                        {
                            Method = "Method 1",
                            Pid = pid,
                            MonsterSeed = method1Seed ^ 0x80000000
                        };
                        seeds.Add(newSeed);
                    }
                }
            }
            return seeds;
        }

        public static bool CheckPID(uint pid2, uint pid1, uint nature)
        {
            //  Do a nature comparison with what we have selected
            //  in the dropdown and if we have a good match we can
            //  go ahead and add this to our starting seeds.
            return nature == ((pid2 << 16) | pid1) % 25;
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

        public static string[] getIVPID(uint nature, string hiddenpower, bool XD = false)
        {
            var generator = new FrameGenerator();
            if (XD) generator = new FrameGenerator
            {
                FrameType = FrameType.ColoXD
            };
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
