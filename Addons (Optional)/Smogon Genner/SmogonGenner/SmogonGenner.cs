using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Collections.Generic;
using PKHeX.Core;
using System.Linq;

namespace PKHeX.WinForms
{
    public partial class Main : Form
    {
        private void SmogonGenner(object sender, EventArgs e)
        {
            string speciesName = PKME_Tabs.CB_Species.Text;
            string form = new ShowdownSet(PKME_Tabs.PreparePKM().ShowdownText).Form;
            string url = "";
            if (form != null)
            {
                if (form != "Mega" || form != "") url = String.Format("https://www.smogon.com/dex/sm/pokemon/{0}-{1}/", speciesName.ToLower(), form.ToLower());
            }
            else url = String.Format("https://www.smogon.com/dex/sm/pokemon/{0}/", speciesName.ToLower());
            string smogonPage = GetSmogonPage(url);
            string[] split1 = smogonPage.Split(new string[] { "\",\"abilities\":" }, StringSplitOptions.None);
            List<string> sets = new List<string>();
            for(int i = 1; i<split1.Length; i++)
            {
                sets.Add(split1[i].Split(new string[] { "\"]}" }, StringSplitOptions.None)[0]);
            }
            string showdownSpec = speciesName;
            if (form != null)
            {
                if (form != "Mega" || form != "") showdownSpec += ("-" + form);
            }
            string showdownsets = "";
            foreach(string set in sets)
            {
                showdownsets = showdownsets + ConvertSetToShowdown(set, showdownSpec) + "\n\r";
            }
            showdownsets.TrimEnd();
            if (showdownsets == "")
            {
                WinFormsUtil.Alert("No movesets available. Perhaps you could help out? Check the Contributions & Corrections forum.\n\nForum: https://www.smogon.com/forums/forums/contributions-corrections.388/");
                return;
            }
            Clipboard.SetText(showdownsets);
            try { ClickShowdownImportPKMModded(sender, e); }
            catch { WinFormsUtil.Alert("Something went wrong"); }
            WinFormsUtil.Alert(sets.Count.ToString() + " sets genned for " + showdownSpec);
        }

        private string GetSmogonPage(string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseString;
            }
            catch (Exception e)
            {
                WinFormsUtil.Alert("An error occured while trying to obtain the contents of the URL. This is most likely an issue with your Internet Connection. The exact error is as follows: " + e.ToString());
                return "Error :" + e.ToString();
            }
        }

        private string ConvertSetToShowdown(string set, string species)
        {
            string ability = set.Split('\"')[1];
            string item = set.Split(new string[] { "\"items\":[\"" }, StringSplitOptions.None)[1].Split('"')[0];
            string movesets = set.Split(new string[] { "\"moveslots\":[" }, StringSplitOptions.None)[1].Split(new string[] { ",\"evconfigs\"" }, StringSplitOptions.None)[0];
            List<string> moves = new List<string>();
            string[] splitmoves = movesets.Split('"');
            if (splitmoves.Length > 1) moves.Add(splitmoves[1]);
            if (splitmoves.Length > 3) moves.Add(splitmoves[3]);
            if (splitmoves.Length > 5) moves.Add(splitmoves[5]);
            if (splitmoves.Length > 7) moves.Add(splitmoves[7]);
            string nature = set.Split(new string[] { "\"natures\":[\"" }, StringSplitOptions.None)[1].Split('"')[0];
            string[] evs = parseEVIVs(set.Split(new string[] { "\"evconfigs\":" }, StringSplitOptions.None)[1].Split(new string[] { ",\"ivconfigs\":" }, StringSplitOptions.None)[0], false);
            string[] ivs = parseEVIVs(set.Split(new string[] { "\"ivconfigs\":" }, StringSplitOptions.None)[1].Split(new string[] { ",\"natures\":" }, StringSplitOptions.None)[0], true);
            var result = new List<string>();
            result.Add(species + " @ " + item);
            result.Add("Ability: " + ability);
            result.Add("EVs: " + evs[0] + " HP / " + evs[1] + " Atk / " + evs[2] + " Def / " + evs[3] + " SpA / " + evs[4] + " SpD / " + evs[5] + " Spe");
            result.Add("IVs: " + ivs[0] + " HP / " + ivs[1] + " Atk / " + ivs[2] + " Def / " + ivs[3] + " SpA / " + ivs[4] + " SpD / " + ivs[5] + " Spe");
            result.Add(nature + " Nature");
            foreach (string move in moves)
            {
                result.Add("- " + move);
            }
            return string.Join(Environment.NewLine, result);
        }
        
        private string[] parseEVIVs(string liststring, bool iv)
        {
            string[] ivdefault = new string[] { "31", "31", "31", "31", "31", "31" };
            string[] evdefault = new string[] { "0", "0", "0", "0", "0", "0" };
            if (!liststring.Contains("{"))
            {
                if (iv) return ivdefault;
                return evdefault;
            }
            string[] val = evdefault;
            if (iv) val = ivdefault;
            string hpstat = liststring.Split(new string[] { "\"hp\":" }, StringSplitOptions.None)[1].Split(',')[0];
            string atkstat = liststring.Split(new string[] { "\"atk\":" }, StringSplitOptions.None)[1].Split(',')[0];
            string defstat = liststring.Split(new string[] { "\"def\":" }, StringSplitOptions.None)[1].Split(',')[0];
            string spastat = liststring.Split(new string[] { "\"spa\":" }, StringSplitOptions.None)[1].Split(',')[0];
            string spdstat = liststring.Split(new string[] { "\"spd\":" }, StringSplitOptions.None)[1].Split(',')[0];
            string spestat = liststring.Split(new string[] { "\"spe\":" }, StringSplitOptions.None)[1].Split('}')[0];
            val[0] = hpstat;
            val[1] = atkstat;
            val[2] = defstat;
            val[3] = spastat;
            val[4] = spdstat;
            val[5] = spestat;
            return val;
        }
    }
}
