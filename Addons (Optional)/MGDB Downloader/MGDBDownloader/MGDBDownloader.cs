using System;
using System.IO;
using System.Windows.Forms;

using System.Net;
using System.IO.Compression;

namespace PKHeX.WinForms
{
    public partial class Main : Form
    {
        public void DownloadMGDB(object o, EventArgs e)
        {
            if (Directory.Exists(MGDatabasePath))
            {
                DialogResult dialogResult = MessageBox.Show("Update MGDB?", "MGDB already exists!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    DeleteDirectory(MGDatabasePath); // Adding events will be handled by the next conditional
                }
            }
            if (!Directory.Exists(MGDatabasePath))
            {
                string mgdbURL = @"https://github.com/projectpokemon/EventsGallery/archive/master.zip";

                WebClient client = new WebClient();

                string mgdbZipPath = @"mgdb.zip";
                client.DownloadFile(new Uri(mgdbURL), mgdbZipPath);
                ZipFile.ExtractToDirectory(mgdbZipPath, MGDatabasePath);
                WinFormsUtil.Alert("Download Finished");
            }
        }
        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }
    }
}
