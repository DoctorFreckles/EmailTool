using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

namespace EmailAddressGenerate
{
    class EmailRank
    {
        public string EmailAddress;
        public int Rank;
    }
    class MainClass
    {
        public static void Main(string[] args)
        {
            //ProcessNPI();

            RankOrderAndGroup();
        }
        private static void RankOrderAndGroup()
        {
            string fileSource =
               @"/home/dan/Desktop/output_emails.txt";

            StreamReader sr = null;

            try
            {
                sr = new StreamReader(fileSource);

                int lineCount = 0;

                Dictionary<string, int> emails = new Dictionary<string, int>();

                string hdr = sr.ReadLine();

                List<EmailRank> emls = new List<EmailRank>();

                while (!sr.EndOfStream)
                {
                    lineCount++;

                    string ln = sr.ReadLine();

                    string[] parts = ln.Split('\t');

                    EmailRank er = new EmailRank();
                    er.EmailAddress = parts[0];
                    er.Rank = int.Parse(parts[1]);

                    emls.Add(er);

                    if (lineCount % 100 == 0)
                    {
                        Console.WriteLine("Processing: " + lineCount.ToString());
                    }

                }


                List<EmailRank> rnks =
                    (from e in emls
                     orderby e.Rank descending
                     where e.Rank > 1 && e.EmailAddress.Length > 4
                     select e).ToList<EmailRank>();


                string fldr = "/home/dan/Desktop/email_groups/";

                List<EmailRank> step2 = new List<EmailRank>();

                int flc = 0;

                for (int i = 0; i < rnks.Count; i++)
                {
                    step2.Add(rnks[i]);

                    if(step2.Count > 199)
                    {
                        flc++;

                        StreamWriter sw = new StreamWriter(fldr + flc.ToString() + "_group.txt");

                        foreach(EmailRank er in step2)
                        {
                            sw.Write(er.EmailAddress);
                            sw.Write('\t');
                            sw.Write(er.Rank.ToString());
                            sw.WriteLine();
                        }

                        sw.Close();

                        step2 = new List<EmailRank>();
                    }

                }

                if (step2.Count > 0)
                {
                    flc++;

                    StreamWriter sw = new StreamWriter(fldr + flc.ToString() + "_group.txt");

                    foreach (EmailRank er in step2)
                    {
                        sw.Write(er.EmailAddress);
                        sw.Write('\t');
                        sw.Write(er.Rank.ToString());
                        sw.WriteLine();
                    }

                    sw.Close();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }
        }
        private static void ProcessNPI()
        {
            string fileSource =
                @"/home/dan/Desktop/NPI/NPPES_Data_Dissemination_November_2018/npidata_pfile_20050523-20181111.csv";

            StreamReader sr = null;

            try
            {
                //StringBuilder sb = new StringBuilder();

                sr = new StreamReader(fileSource);

                int lineCount = 0;

                Dictionary<string, int> emails = new Dictionary<string, int>();

                while(!sr.EndOfStream)
                {
                    lineCount++;
                    string ln = sr.ReadLine();

                    ln = Regex.Replace(ln, "[\"][,][\"]", "|");

                    //sb.Append(ln.Substring(1, ln.Length - 2));
                    //sb.Append('\n');

                    string[] parts = Regex.Split(ln, "[|]");


                    string lastName = parts[5];
                    string firstName = parts[6];
                    string middleName = parts[7];

                    string key1 = firstName.Trim().ToLower() + "." +
                                           lastName.Trim().ToLower();

                    if(!emails.ContainsKey(key1))
                    {
                        emails.Add(key1, 0);
                    }

                    emails[key1]++;

                    //string key2 = null;

                    //if(middleName.Trim().Length > 0)
                    //{
                    //    key2 = firstName.Trim().ToLower() + "." +
                    //                       lastName.Trim().ToLower();
                    //}

                    if(lineCount % 100 == 0)
                    {
                        Console.WriteLine("Processing: " + lineCount.ToString());
                    }



                    //if (lineCount > 10) break;
                }

                //string res = sb.ToString();

                StreamWriter sw = new StreamWriter("output_emails.txt");

                foreach(string key in emails.Keys)
                {
                    sw.Write(key);
                    sw.Write('\t');
                    sw.Write(emails[key].ToString());
                    sw.WriteLine();

                }

                sw.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if(sr != null)
                {
                    sr.Close();
                }
            }
        }
    }
}
