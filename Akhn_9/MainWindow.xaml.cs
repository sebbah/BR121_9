using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AkhnGear
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainWindowName.Title = "AkhnGear v9.0 build " + DateTime.Now;
            FileNameTXT.Text = "BackupM 20160805 1258.sql";
            StartNumberTXT.Text = "02066575";
            EndNumberTXT.Text = "02066586";
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OutputTxt1.Text = "";
            // TODO: OutputTxt2 = "";
            
            string snozzle = "2";
            string sconcl = "GOOD";
            string stestname = "No index 1";
           

            string fileNameTxt = FileNameTXT.Text;
            string startNumberTxt = StartNumberTXT.Text;
            string endNumberTxt = EndNumberTXT.Text;
            int errors = 0;
            int startNumberInt, endNumberInt;
            bool startNumberValid = Int32.TryParse(startNumberTxt, out startNumberInt);
            bool endNumberValid = Int32.TryParse(endNumberTxt, out endNumberInt);

            string line;
            string fileNameGearTxt = "geartype.txt";
            List<GearTable> nGear = new List<GearTable>();

            StringBuilder ErrorsText = new StringBuilder();
            if (startNumberTxt == "") { errors++; ErrorsText.AppendLine("Brak początkowego numeru!"); } else if (startNumberTxt.Length != 8) { errors++; ErrorsText.AppendLine("Sprawdz ilość cyfr w polu OD!"); } else if (startNumberValid == false) { errors++; ErrorsText.AppendLine("Sprawdź numer seryjny w polu OD!"); }
            if (endNumberTxt == "") { errors++; ErrorsText.AppendLine("Brak końcowego numeru!"); } else if (endNumberTxt.Length != 8) { errors++; ErrorsText.AppendLine("Sprawdz ilość cyfr w polu DO!"); } else if (endNumberValid == false) { errors++; ErrorsText.AppendLine("Sprawdź numer seryjny w polu DO!"); }
            if (errors==0) if (startNumberTxt[0] != endNumberTxt[0] || startNumberTxt[1] != endNumberTxt[1]) { errors++; ErrorsText.AppendLine("Pola OD i DO nie są w jednej grupie!"); } else if (startNumberInt > endNumberInt) { errors++; ErrorsText.AppendLine("Numer seryjny w polu OD jest większy od numeru DO!"); }
            
            if (fileNameTxt == "") { errors++; ErrorsText.AppendLine("Brak pliku do analizy!"); };

            if (!System.IO.File.Exists(fileNameGearTxt))
            { errors++; ErrorsText.AppendLine("Brak dostępu do pliku " + fileNameGearTxt + " !"); }
            else
            {
                // Read the file it line by line.  
                System.IO.StreamReader file = new System.IO.StreamReader(fileNameGearTxt);
                while ((line = file.ReadLine()) != null)
                {
                   if (line[0] != '/') nGear.Add(new GearTable(line, 0));
                }
                file.Close();
                OutputTxt1.Text += "Załadowano " + nGear.Count + " wzorców!\n";
            }

            if (!System.IO.File.Exists(fileNameTxt)) { errors++; ErrorsText.AppendLine("Brak dostępu do pliku " + fileNameTxt + "!"); }

            if (errors>0) {OutputTxt1.Text += ErrorsText + "\nW sumie " + errors.ToString() + " błędy!"; };

            // TODO: ?
            
            if (errors == 0)

            {
                int sumAllGears = 0;
                int counter = 0;
                int rekord; // nr zestawu danych w linii
                char char1 = ','; // znak rozdzielajacy dane
                char char2 = '\''; // char 39 - znak ktory pomijamy '
                string nozzle, windex, serial, concl, testname; // dane otrzymane po analizie linii
                int serialInt;
                bool calculate = true;
                bool calculatethis = true;

                // Read the file it line by line.  
                System.IO.StreamReader file = new System.IO.StreamReader(@fileNameTxt);
                while ((line = file.ReadLine()) != null)
                {

                    counter++;
                    //TODO: textBox8.Text = counter.ToString();
                    rekord = 1;
                    nozzle = "";
                    windex = "";
                    serial = "";
                    concl = "";
                    testname = "";

                    if (line == "XXXYYY") calculate = false;
                    for (int i = 0; i <= line.Length - 1; i++)
                    {
                        if (line[i] == char1) rekord++;
                        // `ProdN`,`POS`,`Tnum`,`TType`,`dP`,`Tx1`,`Tx2`,`T1`,`H1`,`P2`,`dPx0`,`Nozzle`,`MUTimp`,`TTime`,`PulseT`,`PulseR`,`Vref`,`Vmut`,`Error`,`Cerror`,`wIndex`,`Concl`,`Reason`,`Serial`,`Protocol`,`Year`,`MUT`,`Flow`,`Test_name`,`T_id`,`dP_ave`
                        if (rekord == 12) if (line[i] != char1) nozzle = nozzle + line[i]; // Nozzle
                        if (rekord == 21) if (line[i] != char1) windex = windex + line[i]; // wIndex
                        if (rekord == 22) if (line[i] != char1 && line[i] != char2) concl = concl + line[i]; // Concl
                        if (rekord == 24) if (line[i] != char1 && line[i] != char2) serial = serial + line[i]; // Serial
                        if (rekord == 29) if (line[i] != char1 && line[i] != char2) testname = testname + line[i]; // Test_name
                    }
                    // spr czy serial jest poprawny
                    calculatethis = Int32.TryParse(serial, out serialInt);

                    if (calculate == true && calculatethis == true && concl == sconcl && testname == stestname && nozzle == snozzle)
                        if (serialInt >= startNumberInt && serialInt <= endNumberInt)
                        {
                            //if (windex == "1") k1++;
                            foreach (GearTable newItems in nGear)
                            {
                                if (windex == newItems.namePos)
                                {
                                    newItems.sumPos++;
                                    break;
                                }
                            }
                        }
                }
                file.Close();

                OutputTxt2.AppendText("GEAR\tPCS\n");

                // if ((k1 == 0 && checkBox2.Checked) || k1 > 0) richTextBox1.Text += "1,0\t" + k1.ToString() + "\n";
                foreach (GearTable nowy in nGear)
                {
                    if (nowy.sumPos >= 0) // || checkBox2.Checked)
                    {
                        OutputTxt2.AppendText(nowy.namePos + "\t" + nowy.sumPos.ToString() + "\n");
                        sumAllGears += nowy.sumPos;
                        // suma = k1 + k1p + k2 + ...;
                    }
                }
                OutputTxt2.AppendText("SUMA:\t" + sumAllGears.ToString() + "\n");
                //textBox8.Text = counter.ToString();
                OutputTxt1.Text += "Sprawdzono " + counter.ToString() + " linii.\nZnaleziono " + sumAllGears.ToString() + " dopasowań.\n";
            }
            else
            {
                OutputTxt2.AppendText("Błędy :  " + errors.ToString() + "\n");
            }
        }

        // Print RichTextBox content
        private void PrintCommand()
        {
            PrintDialog pd = new PrintDialog();
            if ((pd.ShowDialog() == true))
            {
                //use either one of the below      
                pd.PrintVisual(OutputTxt2 as Visual, "printing as visual");
                pd.PrintDocument((((IDocumentPaginatorSource)OutputTxt2.Document).DocumentPaginator), "printing as paginator");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            PrintCommand();
        }

        public class GearTable
        {
            public string namePos { get; set; }
            public int sumPos { get; set; }

            public GearTable(string snamePos, int nsumPos)
            {
                namePos = snamePos;
                sumPos = nsumPos;
            }
        }
    }
}
