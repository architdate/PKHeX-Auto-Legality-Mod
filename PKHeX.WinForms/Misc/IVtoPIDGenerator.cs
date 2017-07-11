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
            Console.WriteLine(hp + " " + atk + " " + def + " " + spa + " " + spd + " " + spe + " " + nature + " " + tid);
            Seed chosenOne = new Seed();
            foreach(Seed s in seeds)
            {
                Console.WriteLine(s.Method);
                if(s.Method == "Method 1")
                {
                    Console.WriteLine("here");
                    chosenOne = s;
                    break;
                }
            }
            string[] ans = new string[2];
            ans[0] = chosenOne.Pid.ToString("X");
            ans[1] = chosenOne.Sid.ToString();
            return ans;
        }
        
    }
}
