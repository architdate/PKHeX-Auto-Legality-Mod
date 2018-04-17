using System;
using System.Windows.Forms;

using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class Main : Form
    {
        private void ExportTrainingData(object sender, EventArgs e)
        {
            string TID = "23456";
            string SID = "34567";
            string OT = "Archit";
            string Gender = "M";
            string Country = "Canada";
            string SubRegion = "Alberta";
            string ConsoleRegion = "Americas (NA/SA)";
            PKM pk = PreparePKM();
            try
            {
                TID = pk.TID.ToString();
                SID = pk.SID.ToString();
                OT = pk.OT_Name;
                if (pk.OT_Gender == 1) Gender = "F";
                Country = PKMConverter.Country.ToString();
                SubRegion = PKMConverter.Region.ToString();
                ConsoleRegion = PKMConverter.ConsoleRegion.ToString();
                writeTxtFile(TID, SID, OT, Gender, Country, SubRegion, ConsoleRegion);
                WinFormsUtil.Alert("trainerdata.txt Successfully Exported in the same directory as PKHeX");
            }
            catch
            {
                writeTxtFile(TID, SID, OT, Gender, Country, SubRegion, ConsoleRegion);
                WinFormsUtil.Alert("Some of the fields were wrongly filled. Exported the default trainerdata.txt");
            }
        }
        private void writeTxtFile(string TID, string SID, string OT, string Gender, string Country, string SubRegion, string ConsoleRegion)
        {
            string[] lines = { "TID:"+TID, "SID:"+SID, "OT:"+OT, "Gender:"+Gender, "Country:"+Country, "SubRegion:"+SubRegion, "3DSRegion:"+ConsoleRegion };
            System.IO.File.WriteAllLines(System.IO.Path.Combine(WorkingDirectory, "trainerdata.txt"), lines);
        }
    }
}
