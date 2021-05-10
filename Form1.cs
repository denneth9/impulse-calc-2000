using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace impulse_calc_2000
{
    public partial class Form1 : Form
    {
        //define motor classifications
        MapValueToUnit[] units =
        {
            new MapValueToUnit(0, 0.3125, "Micro"),
            new MapValueToUnit(0.3125, 0.625, "1/4A"),
            new MapValueToUnit(0.625, 1.25, "1/2A"),
            new MapValueToUnit(1.25, 2.5, "A"),
            new MapValueToUnit(2.5, 5, "B"),
            new MapValueToUnit(5, 10, "C"),
            new MapValueToUnit(10, 20, "D"),
            new MapValueToUnit(20, 40, "E"),
            new MapValueToUnit(40, 80, "F"),
            new MapValueToUnit(80, 160, "G"),
            new MapValueToUnit(160, 320, "H"),
            new MapValueToUnit(320, 640, "I"),
            new MapValueToUnit(640, 1280, "J"),
            new MapValueToUnit(1280, 2560, "K"),
            new MapValueToUnit(2560, 5120, "L"),
            new MapValueToUnit(5120, 10240, "M"),
            new MapValueToUnit(10240, 20480, "N"),
            new MapValueToUnit(20480, 40960, "O"),
            new MapValueToUnit(40960, 81920, "P"),
            new MapValueToUnit(81920, 163840, "Q"),
            new MapValueToUnit(163840, 327680, "R"),
            new MapValueToUnit(327680, 655360, "S"),
            new MapValueToUnit(655360, 1310720, "T")
        };
        //define global variables
        double thrust;
        double thrusttime;
        double previousdouble;
        double ns;


        //define constants
        static double g = 9.80665;
        public Form1()
        {
            InitializeComponent();
        }

        //todo: better error correction, popout graph
        //todo: remove all these useless commented lines
        private void button1_Click(object sender, EventArgs e)
        {
            //define some values

            string nss = Regex.Replace(textBox1.Text, "[^0-9]", ""); //replace all characters other than numbers with nothing
            ns = (double)Int32.Parse(nss) / 1000; //i feel the string to int could be combined into the above line
            //double ns = (double)Int32.Parse(textBox1.Text) / 1000; //convert miliseconds entered to seconds //duplicate of thrusttime
            //double i = 0; //basically a less efficient duplicate of totaltime //removed because i forgot the purpose
            
            bool b = false;

            sumbox(richTextBox1,chart1); //give sumbox the textbox to get the sum of and the chart to display said data

            //Math.DivRem(Int32.Parse(textBox1.Text), 1000, out timeincrement);
            //MessageBox.Show(previousdouble.ToString());
            //double timeincrement = (double) Int32.Parse(textBox1.Text) / 1000; //convert miliseconds entered to seconds
            textBox9.Text = previousdouble.ToString();//print peak thrust to textbox
            
            //math zone
            //double g = 9.80665; //defined as global
            double timeincrement = ns;
            double impulse = thrust * timeincrement * g; //calculate impulse from total thrust times the time increment calculated above together with the gravitational constant (g)
            thrusttime = timeincrement * richTextBox1.Lines.Count(); //calculate total time from amount of values and time increment
            double averagethrust = impulse / thrusttime; //get average thrust by dividing impulse by time
            //textBox2.Text = TruncateToSignificantDigits(impulse, 2).ToString(); //print impulse to textbox
            textBox2.Text = impulse.ToString(); //print impulse to textbox
            textBox3.Text = averagethrust.ToString(); //print average thrust to textbox
            textBox4.Text = thrusttime.ToString(); //print total time to textbox
            if (textBox7.Text.Length > 0) //if textbox is not empty
            {
                try
                {
                    string p = Regex.Replace(textBox7.Text, "[^0-9.]", ""); //replace all characters other than numbers with nothing
                    //double propellant = double.Parse(textBox7.Text); //get propellant weight
                    double propellant = double.Parse(p); //get propellant weight
                    double specificimpulse = impulse / propellant; //get specific impulse from impulse and propelland weight
                    double specificimpulsesec = specificimpulse / g; //get the specific impulse in seconds from specific impulse and g
                    textBox5.Text = specificimpulse.ToString(); //print specific impulse to textbox
                    textBox8.Text = specificimpulsesec.ToString(); //print specific impulse (sec) to textbox
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Error with propellant value"); //if error, error
                    throw;
                }
            }
            motorclass(impulse); //get and print motorclass
        }

        void sumbox(RichTextBox data, Chart chart)
        {
            //reset global values to prevent error
            thrust = 0;
            thrusttime = 0;
            previousdouble = 0;
            chart.Series["Series1"].Points.Clear(); //series1 is and continues to be a stupid name and should be changed
            chart.ChartAreas[0].AxisX.IsMarginVisible = false;
            try
            {
                foreach (var s in data.Lines) //for each line in the textbox, process said line
                {
                    string s3 = Regex.Replace(s, "[^0-9.]", ""); //replace all characters other than numbers with nothing
                    //string line = richTextBox1.Lines[0];
                    if (!string.IsNullOrEmpty(s3)) //if the string is not empty, commence forth
                    {

                        string s2 = s3; //i get the feeling this is unnecessary or inefficient
                        if (s.Contains(" ") == true)
                        {
                            string[] words = s.Split(' ');
                            s2 = words[1];
                            //MessageBox.Show("go go gadget split string!"); //debug to see if it was called
                        }


                        double linedouble = double.Parse(s2); //convert from string to double

                        if (linedouble > previousdouble) //if linedouble is more than previous highest, write as new highest
                        {
                            previousdouble = linedouble; //this should end up as the highest single value in the readings textbox
                        }
                        thrust = thrust + linedouble; //add double to total
                        chart.Series["Series1"].Points.AddXY(thrusttime, linedouble);
                        thrusttime += ns;
                    }
                    else //if the line is empty, delete the line
                    {
                        data.Text = Regex.Replace(data.Text, @"^\s*$(\n|\r|\r\n)", "", RegexOptions.Multiline);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error Parsing Data");
                throw;
            }
        }

        /* //the pseudocode donated to me by a fellow coder which the motorclass remake was built from (Thanks!)
        List<MapValueToUnit> units = new ArrayList<MapValueToUnit>();

        foreach (var unit in units)
        {
            if (ns >= unit.low && ns < unit.high)
        {
            mc = unit.name;
            break;
        }
        */
        void motorclass(double ns) //retrieve motor classification from newton seconds
        {
            string mc = "";

            foreach (var unit in units)
            {
                if (ns >= unit.low && ns < unit.high)
                {
                    mc = unit.name;
                    break;
                }
            }

            textBox6.Text = mc; //print output to textbox
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //if the link is clicked, open denneth.nl in the default browser and display it as visited
            linkLabel1.LinkVisited = true;
            System.Diagnostics.Process.Start("https://denneth.nl");
        }


        private void richTextBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                richTextBox1.Text = Clipboard.GetText();
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                //if (chart1.Series.Count() > 0) //if chart is not empty //removed because i don't give a shit actually
                //{
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        string filepath = saveFileDialog1.FileName;
                        this.chart1.SaveImage(filepath, ChartImageFormat.Png);
                    }
                    else
                    {
                        return;
                    }
                //}

            }
            catch (Exception exception)
            {
                MessageBox.Show("Error trying to save file");
                throw;
            }

        }

        //unused sample functions for truncating, not applicable atm
        static double RoundToSignificantDigits(double d, int digits)
        {
            if (d == 0)
                return 0;

            double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1);
            return scale * Math.Round(d / scale, digits);
        }
        static double TruncateToSignificantDigits(double d, int digits)
        {
            if (d == 0)
                return 0;

            double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1 - digits);
            return scale * Math.Truncate(d / scale);
        }
    }



    class MapValueToUnit //define class for motor classification
    {
        public double low, high;
        public string name;

        public MapValueToUnit(double _low, double _high, string _name)
        {
            low = _low;
            high = _high;
            name = _name;
        }
    }
}
