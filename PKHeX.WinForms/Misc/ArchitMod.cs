using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using PKHeX.Core;
using PKHeX.WinForms.Controls;
using PKHeX.WinForms.Properties;

namespace PKHeX.WinForms.Controls
{
    public partial class PKMEditor : UserControl
    {
        private Controls.SAVEditor C_SAV;

        public void LoadShowdownSetModded(ShowdownSet Set)
        {
            try
            {
                if (Main.HaX)
                {
                    WinFormsUtil.Alert("NOTE: PKHaX enabled, auto legality usage is not necessary for Hackmons.");
                }
                openQuickMod(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\DONOTTOUCH\\reset.pk7");
                TB_OT.Text = "Archit (TCD)";
                TB_TID.Text = "24521";
                TB_SID.Text = "42312";
                CB_Species.SelectedValue = Set.Species;
                CHK_Fateful.Checked = false;
                CB_Ball.SelectedIndex = 0;
                CB_GameOrigin.SelectedIndex = 0;
                CB_MetLocation.SelectedIndex = 4;
                TB_MetLevel.Text = "1";
                CB_3DSReg.SelectedIndex = 2;
                CHK_Nicknamed.Checked = Set.Nickname != null;
                CHK_AsEgg.Checked = true;
                GB_EggConditions.Visible = true;
                CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                if (Set.Nickname != null)
                    TB_Nickname.Text = Set.Nickname;
                if (Set.Gender != null)
                {
                    int Gender = PKX.GetGender(Set.Gender);
                    Label_Gender.Text = gendersymbols[Gender];
                    Label_Gender.ForeColor = GetGenderColor(Gender);
                }

                // Set Form
                string[] formStrings = PKX.GetFormList(Set.Species,
                    Util.GetTypesList("en"),
                    Util.GetFormsList("en"), gendersymbols, pkm.Format);
                int form = 0;
                for (int i = 0; i < formStrings.Length; i++)
                    if (formStrings[i].Contains(Set.Form ?? ""))
                    { form = i; break; }
                CB_Form.SelectedIndex = Math.Min(CB_Form.Items.Count - 1, form);

                // Error Checking for Mega/Busted
                if (CB_Form.Text.Contains("Mega") || CB_Form.Text == "Busted")
                {
                    CB_Form.SelectedIndex = 0;
                }

                // Set Ability and Moves
                CB_Ability.SelectedIndex = Math.Max(0, Array.IndexOf(pkm.PersonalInfo.Abilities, Set.Ability));
                ComboBox[] m = { CB_Move1, CB_Move2, CB_Move3, CB_Move4 };
                for (int i = 0; i < 4; i++) m[i].SelectedValue = Set.Moves[i];

                // Set Item and Nature
                CB_HeldItem.SelectedValue = Set.HeldItem < 0 ? 0 : Set.HeldItem;
                CB_Nature.SelectedValue = Set.Nature < 0 ? 0 : Set.Nature;

                // Set IVs
                TB_HPIV.Text = Set.IVs[0].ToString();
                TB_ATKIV.Text = Set.IVs[1].ToString();
                TB_DEFIV.Text = Set.IVs[2].ToString();
                TB_SPAIV.Text = Set.IVs[4].ToString();
                TB_SPDIV.Text = Set.IVs[5].ToString();
                TB_SPEIV.Text = Set.IVs[3].ToString();

                // Set EVs
                TB_HPEV.Text = Set.EVs[0].ToString();
                TB_ATKEV.Text = Set.EVs[1].ToString();
                TB_DEFEV.Text = Set.EVs[2].ToString();
                TB_SPAEV.Text = Set.EVs[4].ToString();
                TB_SPDEV.Text = Set.EVs[5].ToString();
                TB_SPEEV.Text = Set.EVs[3].ToString();

                // Set Level and Friendship
                TB_Level.Text = Set.Level.ToString();
                TB_Friendship.Text = Set.Friendship.ToString();

                // Reset IV/EVs
                UpdateRandomPID(null, null);
                UpdateRandomEC(null, null);
                ComboBox[] p = { CB_PPu1, CB_PPu2, CB_PPu3, CB_PPu4 };
                for (int i = 0; i < 4; i++)
                    p[i].SelectedIndex = m[i].SelectedIndex != 0 ? 3 : 0; // max PP

                if (Set.Shiny) UpdateShiny(true);
                eggMovesUpdate(null);
                pkm = PreparePKM();
                UpdateLegality();

                if (Legality.Info.Relearn.Any(z => !z.Valid))
                    SetSuggestedRelearnMoves(silent: true);


                if (!Legality.Valid)
                {
                    CB_Ball.SelectedIndex = 0;
                    CHK_AsEgg.Checked = false;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    clickMetLocationMod(null, null);
                    clickLegality1();
                    UpdateLegality();
                    if (!Legality.Valid)
                    {
                        int abindex = CB_Ability.SelectedIndex;
                        CB_Ability.SelectedIndex = 1;
                        clickLegality1();
                        UpdateLegality();
                        CB_Ability.SelectedIndex = 0;
                        clickLegality1();
                        UpdateLegality();
                        if (!Legality.Valid)
                        {
                            CB_Ability.SelectedIndex = abindex;
                            CHK_AsEgg.Checked = true;
                            CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0;
                        } // daycare : none
                    }
                }

                if (!Legality.Valid)
                {
                    CB_Ball.SelectedIndex = 0;
                    CHK_AsEgg.Checked = false;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    CB_GameOrigin.SelectedIndex = 1;
                    clickMetLocationMod(null, null);
                    clickLegality1();
                    UpdateLegality();
                    if (!Legality.Valid)
                    {
                        int abindex = CB_Ability.SelectedIndex;
                        CB_Ability.SelectedIndex = 1;
                        clickLegality1();
                        UpdateLegality();
                        CB_Ability.SelectedIndex = 0;
                        clickLegality1();
                        UpdateLegality();
                        if (!Legality.Valid)
                        {
                            CB_Ability.SelectedIndex = abindex;
                            CHK_AsEgg.Checked = true;
                            CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0;
                        } // daycare : none
                    }
                }


                if (!Legality.Valid)
                {
                    CB_Ball.SelectedIndex = 0;
                    CHK_AsEgg.Checked = true;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    TB_MetLevel.Text = "1";
                    CB_GameOrigin.SelectedIndex = 2;
                    CB_MetLocation.SelectedIndex = 5;
                    CB_RelearnMove1.SelectedIndex = 0;
                    CB_RelearnMove2.SelectedIndex = 0;
                    CB_RelearnMove3.SelectedIndex = 0;
                    CB_RelearnMove4.SelectedIndex = 0;
                    eggMovesUpdate(null);
                    pkm = PreparePKM();
                    pkm.CurrentHandler = 1;
                    TB_OTt2.Text = "Archit";
                    UpdateRandomPID(BTN_RerollPID, null);
                    if (Set.Shiny) UpdateShiny(true);
                    clickLegality1();
                    UpdateLegality();
                }

                if (!Legality.Valid)
                {
                    CB_Ball.SelectedIndex = 0;
                    CHK_AsEgg.Checked = false;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    clickMetLocationMod(null, null);
                    clickLegality1();
                    UpdateLegality();
                    if (!Legality.Valid)
                    {
                        CHK_AsEgg.Checked = true;
                        CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    }
                }

                if (!Legality.Valid)
                {
                    CB_Ball.SelectedIndex = 0;
                    CHK_AsEgg.Checked = false;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    CB_GameOrigin.SelectedIndex = 3;
                    clickMetLocationMod(null, null);
                    clickLegality1();
                    UpdateLegality();
                    if (!Legality.Valid)
                    {
                        CHK_AsEgg.Checked = true;
                        CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    }
                }

                if (!Legality.Valid)
                {
                    CB_Ball.SelectedIndex = 0;
                    CHK_AsEgg.Checked = false;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    CB_GameOrigin.SelectedIndex = 4;
                    clickMetLocationMod(null, null);
                    clickLegality1();
                    UpdateLegality();
                    if (!Legality.Valid)
                    {
                        CHK_AsEgg.Checked = true;
                        CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    }
                }

                if (!Legality.Valid)
                {
                    CB_Ball.SelectedIndex = 0;
                    CHK_AsEgg.Checked = false;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    CB_GameOrigin.SelectedIndex = 5;
                    clickMetLocationMod(null, null);
                    clickLegality1();
                    UpdateLegality();
                    if (!Legality.Valid)
                    {
                        CHK_AsEgg.Checked = true;
                        CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    }
                }

                if (!Legality.Valid)
                {
                    CB_Ball.SelectedIndex = 0;
                    CHK_AsEgg.Checked = false;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    CB_GameOrigin.SelectedIndex = 6;
                    clickMetLocationMod(null, null);
                    clickLegality1();
                    UpdateLegality();
                    if (!Legality.Valid)
                    {
                        CHK_AsEgg.Checked = true;
                        CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    }
                }

                if (!Legality.Valid)
                {
                    CB_Ball.SelectedIndex = 0;
                    CHK_AsEgg.Checked = false;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    CB_GameOrigin.SelectedIndex = 7;
                    clickMetLocationMod(null, null);
                    clickLegality1();
                    UpdateLegality();
                    if (!Legality.Valid)
                    {
                        CB_Ball.SelectedIndex = 6;
                        clickLegality1();
                        UpdateLegality();
                        if (!Legality.Valid)
                        {
                            CHK_AsEgg.Checked = true;
                            CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                        }
                    }
                }

                if (!Legality.Valid)
                {
                    CB_Ball.SelectedIndex = 0;
                    CHK_AsEgg.Checked = false;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    CB_GameOrigin.SelectedIndex = 8;
                    clickMetLocationMod(null, null);
                    clickLegality1();
                    UpdateLegality();
                    if (!Legality.Valid)
                    {
                        CHK_AsEgg.Checked = true;
                        CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    }
                }

                if (!Legality.Valid)
                {
                    CB_Ball.SelectedIndex = 0;
                    CHK_AsEgg.Checked = false;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    CB_GameOrigin.SelectedIndex = 9;
                    clickMetLocationMod(null, null);
                    clickLegality1();
                    UpdateLegality();
                    if (!Legality.Valid)
                    {
                        CB_Ball.SelectedIndex = 6;
                        clickLegality1();
                        UpdateLegality();
                        if (!Legality.Valid)
                        {
                            CHK_AsEgg.Checked = true;
                            CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                        }
                    }
                }


                if (!Legality.Valid)
                {
                    CB_Ball.SelectedIndex = 0;
                    CHK_AsEgg.Checked = true;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    CB_GameOrigin.SelectedIndex = 10;
                    CB_MetLocation.SelectedIndex = 0;
                    CB_RelearnMove1.SelectedIndex = 0;
                    CB_RelearnMove2.SelectedIndex = 0;
                    CB_RelearnMove3.SelectedIndex = 0;
                    CB_RelearnMove4.SelectedIndex = 0;
                    eggMovesUpdate(null);
                    pkm = PreparePKM();
                    pkm.CurrentHandler = 1;
                    TB_OTt2.Text = "Archit";
                    UpdateRandomPID(BTN_RerollPID, null);
                    if (Set.Shiny) UpdateShiny(true);
                    clickLegality1();
                    UpdateLegality();
                }

                if (!Legality.Valid)
                {
                    CB_Ball.SelectedIndex = 0;
                    CHK_AsEgg.Checked = false;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    CB_GameOrigin.SelectedIndex = 10;
                    clickMetLocationMod(null, null);
                    CB_MetLocation.SelectedIndex = 0;
                    UpdateRandomPID(BTN_RerollPID, null);
                    if (Set.Shiny) UpdateShiny(true);
                    clickLegality1();
                    UpdateLegality();
                    if (!Legality.Valid)
                    {
                        CHK_Fateful.Checked = true;
                        clickLegality1();
                        UpdateLegality();
                        if (!Legality.Valid)
                        {
                            CHK_Fateful.Checked = false;
                            CHK_AsEgg.Checked = true;
                            CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                        }
                    }
                }

                if (!Legality.Valid)
                {
                    CB_Ball.SelectedIndex = 0;
                    CHK_AsEgg.Checked = false;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    CB_GameOrigin.SelectedIndex = 11;
                    clickMetLocationMod(null, null);
                    CB_MetLocation.SelectedIndex = 0;
                    UpdateRandomPID(BTN_RerollPID, null);
                    if (Set.Shiny) UpdateShiny(true);
                    clickLegality1();
                    UpdateLegality();
                    if (!Legality.Valid)
                    {
                        CHK_Fateful.Checked = true;
                        clickLegality1();
                        UpdateLegality();
                        if (!Legality.Valid)
                        {
                            CHK_Fateful.Checked = false;
                            CHK_AsEgg.Checked = true;
                            CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                        }
                    }
                }

                if (!Legality.Valid)
                {
                    CB_Ball.SelectedIndex = 0;
                    CHK_AsEgg.Checked = false;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    CB_GameOrigin.SelectedIndex = 12;
                    clickMetLocationMod(null, null);
                    CB_MetLocation.SelectedIndex = 0;
                    UpdateRandomPID(BTN_RerollPID, null);
                    if (Set.Shiny) UpdateShiny(true);
                    clickLegality1();
                    UpdateLegality();
                    if (!Legality.Valid)
                    {
                        CHK_Fateful.Checked = true;
                        clickLegality1();
                        UpdateLegality();
                        if (!Legality.Valid)
                        {
                            CHK_Fateful.Checked = false;
                            CHK_AsEgg.Checked = true;
                            CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                        }
                    }
                }

                if (!Legality.Valid)
                {
                    CB_Ball.SelectedIndex = 0;
                    CHK_AsEgg.Checked = false;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    CB_GameOrigin.SelectedIndex = 13;
                    clickMetLocationMod(null, null);
                    CB_MetLocation.SelectedIndex = 0;
                    UpdateRandomPID(BTN_RerollPID, null);
                    if (Set.Shiny) UpdateShiny(true);
                    clickLegality1();
                    UpdateLegality();
                    if (!Legality.Valid)
                    {
                        CHK_Fateful.Checked = true;
                        clickLegality1();
                        UpdateLegality();
                        if (!Legality.Valid)
                        {
                            CHK_Fateful.Checked = false;
                            CHK_AsEgg.Checked = true;
                            CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                        }
                    }
                }

                if (!Legality.Valid)
                {
                    CB_Ball.SelectedIndex = 0;
                    CHK_AsEgg.Checked = false;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    CB_GameOrigin.SelectedIndex = 14;
                    clickMetLocationMod(null, null);
                    CB_MetLocation.SelectedIndex = 0;
                    UpdateRandomPID(BTN_RerollPID, null);
                    if (Set.Shiny) UpdateShiny(true);
                    clickLegality1();
                    UpdateLegality();
                    if (!Legality.Valid)
                    {
                        CHK_Fateful.Checked = true;
                        clickLegality1();
                        UpdateLegality();
                        if (!Legality.Valid)
                        {
                            CHK_Fateful.Checked = false;
                            CHK_AsEgg.Checked = true;
                            CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                        }
                    }
                }

                if (!Legality.Valid)
                    CHK_Fateful.Checked = false;

                if (!Legality.Valid)
                {
                    CB_Ball.SelectedIndex = 0;
                    CHK_AsEgg.Checked = true;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    CB_GameOrigin.SelectedIndex = 15;
                    CB_MetLocation.SelectedIndex = 0;
                    CB_RelearnMove1.SelectedIndex = 0;
                    CB_RelearnMove2.SelectedIndex = 0;
                    CB_RelearnMove3.SelectedIndex = 0;
                    CB_RelearnMove4.SelectedIndex = 0;
                    TB_MetLevel.Text = "5";
                    CHK_AsEgg.Checked = false;
                    eggMovesUpdate(null);
                    pkm = PreparePKM();
                    pkm.CurrentHandler = 1;
                    TB_OTt2.Text = "Archit";
                    UpdateRandomPID(BTN_RerollPID, null);
                    if (Set.Shiny) UpdateShiny(true);
                    clickLegality1();
                    UpdateLegality();
                }

                if (!Legality.Valid)
                {
                    CB_Ball.SelectedIndex = 0;
                    CHK_AsEgg.Checked = false;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    CB_GameOrigin.SelectedIndex = 15;
                    clickMetLocationMod(null, null);
                    CB_MetLocation.SelectedIndex = 0;
                    UpdateRandomPID(BTN_RerollPID, null);
                    if (Set.Shiny) UpdateShiny(true);
                    clickLegality1();
                    UpdateLegality();
                    if (!Legality.Valid)
                    {
                        CHK_Fateful.Checked = true;
                        clickLegality1();
                        UpdateLegality();
                        if (!Legality.Valid)
                        {
                            CHK_Fateful.Checked = false;
                            CHK_AsEgg.Checked = true;
                            CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                        }
                    }
                }

                if (!Legality.Valid)
                {
                    CB_Ball.SelectedIndex = 0;
                    CHK_AsEgg.Checked = false;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    CB_GameOrigin.SelectedIndex = 16;
                    clickMetLocationMod(null, null);
                    CB_MetLocation.SelectedIndex = 0;
                    UpdateRandomPID(BTN_RerollPID, null);
                    if (Set.Shiny) UpdateShiny(true);
                    clickLegality1();
                    UpdateLegality();
                    if (!Legality.Valid)
                    {
                        CHK_Fateful.Checked = true;
                        clickLegality1();
                        UpdateLegality();
                        if (!Legality.Valid)
                        {
                            CHK_Fateful.Checked = false;
                            CHK_AsEgg.Checked = true;
                            CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                        }
                    }
                }

                if (!Legality.Valid)
                {
                    CB_Ball.SelectedIndex = 0;
                    CHK_AsEgg.Checked = false;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    CB_GameOrigin.SelectedIndex = 17;
                    clickMetLocationMod(null, null);
                    CB_MetLocation.SelectedIndex = 0;
                    UpdateRandomPID(BTN_RerollPID, null);
                    if (Set.Shiny) UpdateShiny(true);
                    clickLegality1();
                    UpdateLegality();
                    if (!Legality.Valid)
                    {
                        CHK_Fateful.Checked = true;
                        clickLegality1();
                        UpdateLegality();
                        if (!Legality.Valid)
                        {
                            CHK_Fateful.Checked = false;
                            CHK_AsEgg.Checked = true;
                            CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                        }
                    }
                }

                if (!Legality.Valid)
                {
                    CB_Ball.SelectedIndex = 0;
                    CHK_AsEgg.Checked = false;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    CB_GameOrigin.SelectedIndex = 18;
                    clickMetLocationMod(null, null);
                    CB_MetLocation.SelectedIndex = 0;
                    UpdateRandomPID(BTN_RerollPID, null);
                    if (Set.Shiny) UpdateShiny(true);
                    clickLegality1();
                    UpdateLegality();
                    if (!Legality.Valid)
                    {
                        CHK_Fateful.Checked = true;
                        clickLegality1();
                        UpdateLegality();
                        if (!Legality.Valid)
                        {
                            CHK_Fateful.Checked = false;
                            CHK_AsEgg.Checked = true;
                            CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                        }
                    }
                }

                if (!Legality.Valid)
                {
                    CB_Ball.SelectedIndex = 0;
                    CHK_AsEgg.Checked = false;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    CB_GameOrigin.SelectedIndex = 19;
                    clickMetLocationMod(null, null);
                    CB_MetLocation.SelectedIndex = 0;
                    UpdateRandomPID(BTN_RerollPID, null);
                    if (Set.Shiny) UpdateShiny(true);
                    clickLegality1();
                    UpdateLegality();
                    if (!Legality.Valid)
                    {
                        CHK_Fateful.Checked = true;
                        clickLegality1();
                        UpdateLegality();
                        if (!Legality.Valid)
                        {
                            CHK_Fateful.Checked = false;
                            CHK_AsEgg.Checked = true;
                            CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                        }
                    }
                }

                if (!Legality.Valid) CHK_Fateful.Checked = false;

                if (!Legality.Valid)
                {
                    CB_Ball.SelectedIndex = 0;
                    CHK_AsEgg.Checked = true;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    CB_GameOrigin.SelectedIndex = 24;
                    CB_MetLocation.SelectedIndex = 107;
                    CB_RelearnMove1.SelectedIndex = 0;
                    CB_RelearnMove2.SelectedIndex = 0;
                    CB_RelearnMove3.SelectedIndex = 0;
                    CB_RelearnMove4.SelectedIndex = 0;
                    TB_MetLevel.Text = "100";
                    CHK_AsEgg.Checked = false;
                    eggMovesUpdate(null);
                    pkm = PreparePKM();
                    pkm.CurrentHandler = 1;
                    TB_OTt2.Text = "Archit";
                    TB_SID.Text = "00000";
                    pkm.Geo1_Country = 64;
                    UpdateRandomPID(BTN_RerollPID, null);
                    if (Set.Shiny) UpdateShiny(true);
                    clickLegality1();
                    UpdateLegality();
                }

                if (TB_PID.Text == "00000000")
                {
                    UpdateRandomPID(BTN_RerollPID, null);
                    if (Set.Shiny) UpdateShiny(true);
                }

                string pkmnName = CB_Species.Text;
                string move1 = CB_Move1.Text;
                string move2 = CB_Move2.Text;
                string move3 = CB_Move3.Text;
                string move4 = CB_Move4.Text;
                if (!Legality.Valid)
                {
                    string folderpath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\mgdb";
                    foreach (string file in Directory.GetFiles(folderpath, "*.*", SearchOption.AllDirectories))
                    {
                        openQuickMod(file);
                        if (pkmnName == CB_Species.Text)
                        {

                                Console.WriteLine("In here for pokemon: " + CB_Species.Text);
                                readPSData(Set);
                                //UpdateRandomPID(BTN_RerollPID, null);
                                clickLegality1();
                                UpdateLegality();


                        }
                        if (Legality.Valid && pkmnName == CB_Species.Text) break;
                    }
                    readPSData(Set);
                }
            }
            catch (Exception e)
            {
                WinFormsUtil.Alert("Decoded data not a valid PKM. If you believe it is a valid Pokemon, Open an issue in the GitHub repository or contact thecommondude on Discord");
            }
        }

        private void eggMovesUpdate(EventArgs e)
        {
            UpdateLegality(skipMoveRepop: true);


            int[] m = Legality.GetSuggestedRelearn();
            if (!pkm.WasEgg && !pkm.WasEvent && !pkm.WasEventEgg && !pkm.WasLink)
            {
                var encounter = Legality.GetSuggestedMetInfo();
                if (encounter != null)
                    m = encounter.Relearn;
            }

            if (pkm.RelearnMoves.SequenceEqual(m))
                return;

            string r = string.Join(Environment.NewLine, m.Select(v => v >= GameInfo.Strings.movelist.Length ? "ERROR" : GameInfo.Strings.movelist[v]));


            CB_RelearnMove1.SelectedValue = m[0];
            CB_RelearnMove2.SelectedValue = m[1];
            CB_RelearnMove3.SelectedValue = m[2];
            CB_RelearnMove4.SelectedValue = m[3];


            UpdateLegality();
        }

        private void clickLegality1()
        {
            if (!VerifiedPKM())
            { SystemSounds.Asterisk.Play(); return; }

            var pk = PreparePKM();

            if (pk.Species == 0 || !pk.ChecksumValid)
            { SystemSounds.Asterisk.Play(); return; }

            //ShowLegality(sender, e, pk);
        }

        private void ShowLegality(PKM pk)
        {
            LegalityAnalysis la = new LegalityAnalysis(pk);
            if (pk.Slot < 0)
                UpdateLegality(la);
            bool verbose = ModifierKeys == Keys.Control;
            var report = la.Report(verbose);
            if (verbose)
            {
                var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, report, "Copy report to Clipboard?");
                if (dr == DialogResult.Yes)
                    Clipboard.SetText(report);
            }
            else
                WinFormsUtil.Alert(report);
        }

        private void readPSData(ShowdownSet Set)
        {
            if (Set.Nickname != null)
                TB_Nickname.Text = Set.Nickname;
            if (Set.Gender != null)
            {
                int Gender = PKX.GetGender(Set.Gender);
                Label_Gender.Text = gendersymbols[Gender];
                Label_Gender.ForeColor = GetGenderColor(Gender);
            }

            // Set Form
            string[] formStrings = PKX.GetFormList(Set.Species,
                Util.GetTypesList("en"),
                Util.GetFormsList("en"), gendersymbols, pkm.Format);
            int form = 0;
            for (int i = 0; i < formStrings.Length; i++)
                if (formStrings[i].Contains(Set.Form ?? ""))
                { form = i; break; }
            CB_Form.SelectedIndex = Math.Min(CB_Form.Items.Count - 1, form);

            // Set Ability and Moves
            CB_Ability.SelectedIndex = Math.Max(0, Array.IndexOf(pkm.PersonalInfo.Abilities, Set.Ability));
            ComboBox[] m = { CB_Move1, CB_Move2, CB_Move3, CB_Move4 };
            for (int i = 0; i < 4; i++) m[i].SelectedValue = Set.Moves[i];

            // Set Item and Nature
            CB_HeldItem.SelectedValue = Set.HeldItem < 0 ? 0 : Set.HeldItem;
            CB_Nature.SelectedValue = Set.Nature < 0 ? 0 : Set.Nature;

            // Set IVs
            TB_HPIV.Text = Set.IVs[0].ToString();
            TB_ATKIV.Text = Set.IVs[1].ToString();
            TB_DEFIV.Text = Set.IVs[2].ToString();
            TB_SPAIV.Text = Set.IVs[4].ToString();
            TB_SPDIV.Text = Set.IVs[5].ToString();
            TB_SPEIV.Text = Set.IVs[3].ToString();

            // Set EVs
            TB_HPEV.Text = Set.EVs[0].ToString();
            TB_ATKEV.Text = Set.EVs[1].ToString();
            TB_DEFEV.Text = Set.EVs[2].ToString();
            TB_SPAEV.Text = Set.EVs[4].ToString();
            TB_SPDEV.Text = Set.EVs[5].ToString();
            TB_SPEEV.Text = Set.EVs[3].ToString();

            // Set Level and Friendship
            TB_Level.Text = Set.Level.ToString();
            TB_Friendship.Text = Set.Friendship.ToString();

            // Reset IV/EVs
            UpdateRandomPID(null, null);
            UpdateRandomEC(null, null);
            ComboBox[] p = { CB_PPu1, CB_PPu2, CB_PPu3, CB_PPu4 };
            for (int i = 0; i < 4; i++)
                p[i].SelectedIndex = m[i].SelectedIndex != 0 ? 3 : 0; // max PP

            if (Set.Shiny) UpdateShiny(true);
        }

        private void clickMetLocationMod(object sender, EventArgs e)
        {
            if (HaX)
                return;

            pkm = PreparePKM();
            UpdateLegality(skipMoveRepop: true);
            if (Legality.Valid)
                return;

            var encounter = Legality.GetSuggestedMetInfo();
            if (encounter == null || (pkm.Format >= 3 && encounter.Location < 0))
            {
                //WinFormsUtil.Alert("Unable to provide a suggestion.");
                return;
            }

            int level = encounter.Level;
            int location = encounter.Location;
            int minlvl = Legal.GetLowestLevel(pkm, encounter.Species);
            if (minlvl == 0)
                minlvl = level;

            if (pkm.CurrentLevel >= minlvl && pkm.Met_Level == level && pkm.Met_Location == location)
                return;
            if (minlvl < level)
                minlvl = level;

            var suggestion = new List<string> { "Suggested:" };
            if (pkm.Format >= 3)
            {
                var met_list = GameInfo.GetLocationList((GameVersion)pkm.Version, pkm.Format, egg: false);
                var locstr = met_list.FirstOrDefault(loc => loc.Value == location).Text;
                suggestion.Add($"Met Location: {locstr}");
                suggestion.Add($"Met Level: {level}");
            }
            if (pkm.CurrentLevel < minlvl)
                suggestion.Add($"Current Level: {minlvl}");

            if (suggestion.Count == 1) // no suggestion
                return;

            string suggest = string.Join(Environment.NewLine, suggestion);
            //if (WinFormsUtil.Prompt(MessageBoxButtons.YesNo, suggest) != DialogResult.Yes)
            //return;

            if (pkm.Format >= 3)
            {
                TB_MetLevel.Text = level.ToString();
                CB_MetLocation.SelectedValue = location;
            }

            if (pkm.CurrentLevel < minlvl)
                TB_Level.Text = minlvl.ToString();

            pkm = PreparePKM();
            UpdateLegality();
        }

        private void OpenFile(byte[] input, string path, string ext, SaveFile currentSaveFile)
        {

            if (TryLoadPKM(input, path, ext, currentSaveFile))
                return;
            if (TryLoadMysteryGift(input, path, ext, currentSaveFile))
                return;

            //WinFormsUtil.Error("Attempted to load an unsupported file type/size.",
            //  $"File Loaded:{Environment.NewLine}{path}",
            //$"File Size:{Environment.NewLine}{input.Length} bytes (0x{input.Length:X4})");
        }

        private bool TryLoadPKM(byte[] input, string path, string ext, SaveFile SAV)
        {
            var temp = PKMConverter.GetPKMfromBytes(input, prefer: ext.Length > 0 ? (ext.Last() - 0x30) & 7 : SAV.Generation);
            if (temp == null)
                return false;

            var type = CurrentPKM.GetType();
            PKM pk = PKMConverter.ConvertToType(temp, type, out string c);
            if (pk == null)
            {
                //WinFormsUtil.Alert("Conversion failed.", c);
                return false;
            }
            if (SAV.Generation < 3 && ((pk as PK1)?.Japanese ?? ((PK2)pk).Japanese) != SAV.Japanese)
            {
                var strs = new[] { "International", "Japanese" };
                var val = SAV.Japanese ? 0 : 1;
                WinFormsUtil.Alert($"Cannot load {strs[val]} {pk.GetType().Name}s to {strs[val ^ 1]} saves.");
                return false;
            }

            PopulateFields(pk);
            Console.WriteLine(c);
            return true;
        }

        private bool TryLoadMysteryGift(byte[] input, string path, string ext, SaveFile SAV)
        {
            var tg = MysteryGift.GetMysteryGift(input, ext);
            if (tg == null)
                return false;
            if (!tg.IsPokémon)
            {
                //WinFormsUtil.Alert("Mystery Gift is not a Pokémon.", path);
                return true;
            }

            var temp = tg.ConvertToPKM(SAV);
            PKM pk = PKMConverter.ConvertToType(temp, SAV.PKMType, out string c);

            if (pk == null)
            {
                //WinFormsUtil.Alert("Conversion failed.", c);
                return true;
            }

            PopulateFields(pk);
            Console.WriteLine(c);
            return true;
        }

        private void openQuickMod(string path, bool force = false)
        {
            this.C_SAV = new PKHeX.WinForms.Controls.SAVEditor();
            this.C_SAV.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.C_SAV.Location = new System.Drawing.Point(292, 26);
            this.C_SAV.Name = "C_SAV";
            this.C_SAV.Size = new System.Drawing.Size(310, 326);
            this.C_SAV.TabIndex = 104;

            if (!(CanFocus || force))
            {
                SystemSounds.Asterisk.Play();
                return;
            }

            string ext = Path.GetExtension(path);
            FileInfo fi = new FileInfo(path);
            if (fi.Length > 0x10009C && fi.Length != 0x380000 && !SAV3GCMemoryCard.IsMemoryCardSize(fi.Length))
                WinFormsUtil.Error("Input file is too large." + Environment.NewLine + $"Size: {fi.Length} bytes", path);
            else if (fi.Length < 32)
                WinFormsUtil.Error("Input file is too small." + Environment.NewLine + $"Size: {fi.Length} bytes", path);
            else
            {
                byte[] input; try { input = File.ReadAllBytes(path); }
                catch (Exception e) { WinFormsUtil.Error("Unable to load file.  It could be in use by another program.\nPath: " + path, e); return; }

#if DEBUG
                OpenFile(input, path, ext, C_SAV.SAV);
#else
                try { openFile(input, path, ext); }
                catch (Exception e) { WinFormsUtil.Error("Unable to load file.\nPath: " + path, e); }
#endif
            }
        }

    }


}
