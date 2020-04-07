using CsvHelper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GBOM
{
    
    public partial class Form1 : Form
    {
        private static CsvHelper.Configuration.Configuration CsvConf = new CsvHelper.Configuration.Configuration { Delimiter = ";" };
        List<string> listQte = new List<string>();
        List<string> listValue = new List<string>();
        List<string> listDesc = new List<string>();
        List<string> listRef = new List<string>();
        List<string> listMfg = new List<string>();
        List<string> listParts = new List<string>();

        List<string> listQteDispoMouser = new List<string>();
        List<string> listRefDispoMouser = new List<string>();

        List<string> listQteDispoDigi = new List<string>();
        List<string> listRefDispoDigi = new List<string>();

        RootObject response;
        public Form1()
        {
            InitializeComponent();

            
        }

        void GetResult(string mftPart)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.mouser.com/api/v1/search/partnumber?apiKey=xxxxxxxxxxxxxxxxxx");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {

                /*{
                      "SearchByPartRequest": {
                        "mouserPartNumber": "C3640C153KGGACAUTO",
                        "partSearchOptions": ""
                      }
                    }*/
                string json = "{\"SearchByPartRequest\":" +
                                "{\"mouserPartNumber\":\"" + mftPart + "\"," +
                              "\"partSearchOptions\":\"2\"}" +
                              "}";

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = streamReader.ReadToEnd();
                response = JToken.Parse(result).ToObject<RootObject>();
                if (response.Errors.Count != 0)
                {
                    response = null;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            listQte.Clear();
            listValue.Clear();
            listDesc.Clear();
            listRef.Clear();
            listMfg.Clear();
            listParts.Clear();

            listQteDispoMouser.Clear();
            listRefDispoMouser.Clear();

            listQteDispoMouser.Add("Mouser Qte");
            listRefDispoMouser.Add("Mouser Ref");

            listQteDispoDigi.Add("DigiKey Qte");
            listRefDispoDigi.Add("DigiKey Ref");

            using (var reader = new StreamReader(textBox1.Text))
            {
            
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');
                    
                    listQte.Add(values[0].Replace("\"", string.Empty));
                    listValue.Add(values[1].Replace("\"", string.Empty));
                    listDesc.Add(values[2].Replace("\"", string.Empty));
                    listRef.Add(values[3].Replace("\"", string.Empty));
                    listMfg.Add(values[4].Replace("\"", string.Empty));
                    listParts.Add(values[5].Replace("\"", string.Empty));
                }
            }
            DateTime start = DateTime.Now;
            int nbrequest = 1;
            for (int i = 1; i < listRef.Count; i++)
            {
                /* vérifie correspondance avant la requette */
                GetResult(listRef[i]);
                nbrequest++;
                if (nbrequest == 29) // maximum 30 requettes à la minute
                {
                    start = start.AddMinutes(1);
                    while (DateTime.Now < start)
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                    System.Threading.Thread.Sleep(1000);
                }
                string valqteM = "";
                string valrefM = "";
                if (response != null)
                {
                    if (response.SearchResults.NumberOfResult > 0)
                    {
                        if (response.SearchResults.Parts[0].Availability != null)
                        {
                            valqteM = response.SearchResults.Parts[0].Availability.ToString();
                            if (valqteM.Contains("Sur commande"))
                            {
                                valqteM = "0";
                            }
                            else
                            {
                                if (valqteM.Contains("En stock"))
                                {
                                    try
                                    {
                                        int qte = int.Parse(valqteM.Substring(0, valqteM.IndexOf(" ")));
                                        if (qte > int.Parse(listQte[i]))
                                        {
                                            valqteM = (int.Parse(listQte[i]) * (int)numericUpDown1.Value).ToString();
                                            valrefM = response.SearchResults.Parts[0].MouserPartNumber;
                                        }
                                        else
                                        {
                                            valqteM = "";
                                            valrefM = "";
                                        }
                                    }
                                    catch
                                    {
                                        valqteM = "";
                                        valrefM = "";
                                    }
                                }
                            }
                        }
                        else
                        {
                            valqteM = "";
                            Remplacement frm = new Remplacement("Le produit n'est plus disponible : " + listRef[i]);
                            frm.ShowDialog();
                            if (frm.new_Ref != "")
                            {
                                listRef[i] = frm.new_Ref;

                                /* Ajout fichier de correspondance*/
                                i--; // reboucle avec nouvelle valeur
                                continue;
                            }
                            
                        }
                    }
                    else
                        valqteM = "-1";
                }
                else
                {
                    valqteM = "-2";
                }
                listQteDispoMouser.Add(valqteM);
                listRefDispoMouser.Add(valrefM);
            }
            string sortie = Path.GetDirectoryName(textBox1.Text) + "\\Export_Auto_BOM.csv";
            WriteDumpFile(sortie);
        }

        private bool WriteDumpFile(string _path)
        {
            try
            {
                using (System.IO.TextWriter tr = new System.IO.StreamWriter(_path, false, Encoding.GetEncoding(1252)))
                {
                    var csv = new CsvWriter(tr, CsvConf);
                    for (int i = 0; i < listRef.Count; i++)
                    {
                        csv.WriteField(listQte[i]);
                        csv.WriteField(listValue[i]);
                        csv.WriteField(listDesc[i]);
                        csv.WriteField(listRef[i]);
                        csv.WriteField(listMfg[i]);
                        csv.WriteField(listParts[i]);
                        csv.WriteField(listRefDispoMouser[i]);
                        csv.WriteField(listQteDispoMouser[i]);
                        csv.NextRecord();

                        
                    }
                    //csv.WriteRecords(_MyList);
                    tr.Flush();
                }
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
                return true;
            }
        }

    }
}
