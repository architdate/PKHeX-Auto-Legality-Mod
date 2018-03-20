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
        /// <summary>
        ///     1 or 2 for the ability number, best we can do since we don't know what the monster is actually going to be.
        /// </summary>
        private uint ability;

        private uint dv;
        private uint id;
        private uint number;
        private uint pid;
        private uint seed;
        private uint sid;

        internal Frame(FrameType frameType)
        {
            Shiny = false;
            Offset = 0;
            FrameType = frameType;
        }

        public uint RngResult { get; set; }

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

        public FrameType FrameType { get; set; }

        public bool Shiny { get; private set; }

        //  The following are cacluated differently based
        //  on the creation method of the pokemon. 

        public uint Pid
        {
            get { return pid; }
            set
            {
                Nature = (value % 25);
                ability = (value & 1);

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

        public uint Nature { get; set; }

        public uint Hp { get; set; }

        public uint Atk { get; set; }

        public uint Def { get; set; }

        public uint Spa { get; set; }

        public uint Spd { get; set; }

        public uint Spe { get; set; }

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
        private readonly CompareType atkCompare;
        private readonly uint atkValue;
        private readonly CompareType defCompare;
        private readonly uint defValue;
        private readonly CompareType hpCompare;
        private readonly uint hpValue;
        private readonly CompareType spaCompare;
        private readonly uint spaValue;
        private readonly CompareType spdCompare;
        private readonly uint spdValue;
        private readonly CompareType speCompare;
        private readonly uint speValue;

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

        public List<uint> Natures { get; private set; }

        public bool Compare(Frame frame)
        {
            bool test = Natures.Any(nature => nature == frame.Nature);
            if (!test)
                return false;

            if (!CompareIV(hpCompare, frame.Hp, hpValue))
                return false;

            if (!CompareIV(atkCompare, frame.Atk, atkValue))
                return false;

            if (!CompareIV(defCompare, frame.Def, defValue))
                return false;

            if (!CompareIV(spaCompare, frame.Spa, spaValue))
                return false;

            if (!CompareIV(spdCompare, frame.Spd, spdValue))
                return false;

            if (!CompareIV(speCompare, frame.Spe, speValue))
                return false;

            return true;
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