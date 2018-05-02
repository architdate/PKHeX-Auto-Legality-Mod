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
                DialogResult latestCommit = MessageBox.Show("Download the entire database, which includes past generation events?\nSelecting No will download only the public release of the database.", "Download entire database?", MessageBoxButtons.YesNo);
                if (latestCommit == DialogResult.Yes)
                {
                    string mgdbURL = @"https://github.com/projectpokemon/EventsGallery/archive/master.zip";

                    WebClient client = new WebClient();

                    string mgdbZipPath = @"mgdb.zip";
                    client.DownloadFile(new Uri(mgdbURL), mgdbZipPath);
                    ZipFile.ExtractToDirectory(mgdbZipPath, MGDatabasePath);
                    File.Delete("mgdb.zip");
                    DeleteDirectory(Path.Combine(MGDatabasePath, "EventsGallery-master", "Unreleased"));
                    DeleteDirectory(Path.Combine(MGDatabasePath, "EventsGallery-master", "Extras"));
                    File.Delete(Path.Combine(MGDatabasePath, "EventsGallery-master", ".gitignore"));
                    File.Delete(Path.Combine(MGDatabasePath, "EventsGallery-master", "README.md"));
                    WinFormsUtil.Alert("Download Finished");
                }
                else
                {
                    WebClient client = new WebClient();
                    string json_data = DownloadString("https://api.github.com/repos/projectpokemon/EventsGallery/releases/latest");
                    string mgdbURL = json_data.Split(new string[] { "browser_download_url" }, StringSplitOptions.None)[1].Substring(3).Split('"')[0];
                    Console.WriteLine(mgdbURL);
                    string mgdbZipPath = @"mgdb.zip";
                    client.DownloadFile(new Uri(mgdbURL), mgdbZipPath);
                    ZipFile.ExtractToDirectory(mgdbZipPath, MGDatabasePath);
                    File.Delete("mgdb.zip");
                    WinFormsUtil.Alert("Download Finished");
                }
            }
        }
        
        public static string DownloadString(string address)
        {
            using (WebClient client = new WebClient())
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.github.com/repos/projectpokemon/EventsGallery/releases/latest");
                request.Method = "GET";
                request.UserAgent = "PKHeX-Auto-Legality-Mod";
                request.Accept = "application/json";
                WebResponse response = request.GetResponse(); //Error Here
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string result = reader.ReadToEnd();
                
                return result;
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
