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
        Method1,
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

        public string CaveSpotting
        {
            get { return RngResult >> 31 == 1 ? "Possible" : ""; }
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

        public FrameCompare(IVFilter ivBase, List<uint> natures)
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
            bool checkparents)
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
            bool checkparents)
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
            List<int> encounterSlots)
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
            
        }

        public FrameCompare(IVFilter ivBase,
                            List<uint> natures,
                            int ability,
                            bool shinyOnly,
                            bool synchOnly,
                            bool dreamWorld,
                            List<int> encounterSlots)
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
            
        }

        // someday, dynamic frame comparers will be added
        public FrameCompare()
        {
            comparers = new List<FrameCompare>();
        }
        

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

           
            //  Go through and check each IV against what the user has required.
            //  Skip if it's a FrameType that doesn't use IVs
            
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
            foreach (Seed s in seeds)
            {
                //Console.WriteLine(s.Method);
                if (s.Method == "Method 1")
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
            if (hiddenpower == "dark")
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
            if (XD) generator = new FrameGenerator
            {
                FrameType = FrameType.ColoXD
            };
            if (method == "M2") generator = new FrameGenerator
            {
                FrameType = FrameType.Method2
            };
            if (method == "BACD_R")
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
                    new List<uint> { nature });
            List<Frame> frames = generator.Generate(frameCompare, 0, 0);
            Console.WriteLine("Num frames: " + frames.Count);
            return new string[] { frames[0].Pid.ToString("X"), frames[0].Hp.ToString(), frames[0].Atk.ToString(), frames[0].Def.ToString(), frames[0].Spa.ToString(), frames[0].Spd.ToString(), frames[0].Spe.ToString() };
        }
    }
}