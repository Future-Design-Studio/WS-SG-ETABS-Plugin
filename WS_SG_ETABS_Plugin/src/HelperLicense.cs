using ETABSv1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WS_SG_ETABS_Plugin.src
{
    internal class HelperLicense
    {
        public static bool CheckLicense(string version)
        {
            bool licenseValid = true;
            string userComputerName = Environment.MachineName.ToString();

            //Read license number
            try
            {
                StreamReader sr = new StreamReader(@"C:\Program Files\Computers and Structures\ETABS 20\LMHOST.ini");
                string licenseNumber = sr.ReadLine();
                sr.Close();

                if (licenseNumber != "rwcphlice002")
                {
                    MessageBox.Show("Illegal license. Your PC ID " + userComputerName + " has been reported to Ramboll IT as well as Singapore Police.");
                    licenseValid = false;
                    return licenseValid;
                }
            }
            catch
            {
                MessageBox.Show("Illegal license. Your PC ID " + userComputerName + " has been reported to Ramboll IT as well as Singapore Police.");
                licenseValid = false;
                return licenseValid;
            }


            //Read online allowed user
            try
            {

                List<List<string>> licenseCsv = new List<List<string>>();
                licenseCsv = SplitCSV("https://raw.githubusercontent.com/stefano-ramboll/licensing/main/etabs_powerapps_lic.csv");

                foreach (List<string> eachLicenseCsv in licenseCsv)
                {
                    string computerName = eachLicenseCsv[1];
                    string[] expDateMonthYearString = eachLicenseCsv[2].Split('/');

                    int expDate = Convert.ToInt32(expDateMonthYearString[0]);
                    int expMonth = Convert.ToInt32(expDateMonthYearString[1]);
                    int expYear = Convert.ToInt32(expDateMonthYearString[2]);

                    DateTime expDateMonthYear = new DateTime(expYear, expMonth, expDate);

                    DateTime today = DateTime.Today;

                    if (userComputerName == computerName)
                    {
                        if (today < expDateMonthYear)
                        {
                            List<List<string>> versionCsv = new List<List<string>>();
                            versionCsv = SplitCSV("https://raw.githubusercontent.com/stefano-ramboll/licensing/main/etabs_powerapps_allowed_versions.csv");
                            foreach (List<string> eachVersionCsv in versionCsv)
                            {
                                string versionCSV = eachVersionCsv[0];
                                if (versionCSV == version)
                                {
                                    return licenseValid;
                                }
                            }
                            MessageBox.Show("Invalid version of ETABS PowerAPP detected! Please contact FDS for latest version!");
                            licenseValid = false;
                            return licenseValid;
                        }
                        else
                        {
                            MessageBox.Show("License has expired! Please contact FDS to renew / extend your license!");
                            licenseValid = false;
                            return licenseValid;
                        }

                    }

                }

                licenseValid = false;
                return licenseValid;
            }
            catch
            {
                MessageBox.Show("Unable to read license file! Please check Internet connection!");

                licenseValid = false;
                return licenseValid;
            }
        }


        public static List<List<string>> SplitCSV(string url)
        {
            string rawFile = GetCSV(url);

            List<List<string>> splitted = new List<List<string>>();
            string[] tempStr;

            //Split per row
            tempStr = rawFile.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string item in tempStr)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {

                    List<string> splittedRow = new List<string>();
                    string[] tempStrPerRow;

                    tempStrPerRow = item.Split(',');

                    foreach (string itemRow in tempStrPerRow)
                    {
                        if (!string.IsNullOrWhiteSpace(itemRow))
                        {
                            splittedRow.Add(itemRow);
                        }
                    }

                    splitted.Add(splittedRow);

                }
            }

            //Remove first row
            splitted.RemoveAt(0);

            return splitted;
        }

        public static string GetCSV(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            StreamReader sr = new StreamReader(resp.GetResponseStream());
            string results = sr.ReadToEnd();
            sr.Close();

            return results;
        }



    }
}

