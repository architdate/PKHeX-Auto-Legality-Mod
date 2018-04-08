using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace PKHeX.WinForms
{
    public partial class Main : Form
    {
        private void URLGen(object sender, EventArgs e)
        {
            string url = Clipboard.GetText().Trim();
            string initURL = url;
            bool isUri = Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute);
            if (!isUri)
            {
                WinFormsUtil.Alert("The text in the clipboard is not a valid URL");
                return;
            }
            if (!CheckPokePaste(url) && !CheckPasteBin(url))
            {
                WinFormsUtil.Alert("The URL provided is not a pokepast.es or a pastebin.com URL");
                return;
            }
            url = FixURL(url);
            string sets = GetText(url).TrimStart().TrimEnd();
            if (sets.StartsWith("Error :")) return;
            Clipboard.SetText(sets);
            try { ClickShowdownImportPKMModded(sender, e); }
            catch { WinFormsUtil.Alert("The data inside the URL are not valid Showdown Sets"); }
            WinFormsUtil.Alert("All sets genned from the following URL: " + initURL);
            Clipboard.SetText(initURL);
        }

        private string FixURL(string url)
        {
            if (CheckPokePaste(url) && url.EndsWith("/raw")) return url;
            else if (CheckPasteBin(url) && url.Contains("/raw/")) return url;
            else if (CheckPokePaste(url)) return url + "/raw";
            else if (CheckPasteBin(url)) return url.Replace("pastebin.com/", "pastebin.com/raw/");
            else return url; // This should never happen
        }

        private bool CheckPokePaste(string url)
        {
            if (url.Contains("pokepast.es/")) return true;
            return false;
        }

        private bool CheckPasteBin(string url)
        {
            if (url.Contains("pastebin.com/")) return true;
            return false;
        }

        private string GetText(string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseString;
            }
            catch(Exception e)
            {
                WinFormsUtil.Alert("An error occured while trying to obtain the contents of the URL. This is most likely an issue with your Internet Connection. The exact error is as follows: " + e.ToString());
                return "Error :" + e.ToString();
            }
        }
    }
}