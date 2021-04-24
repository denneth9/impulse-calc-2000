using System;
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
        public Form1()
        {
            InitializeComponent();
        }
        //todo: better error correction, popout graph
        private void button1_Click(object sender, EventArgs e)
        {
            //define some values
            double thrust = 0;
            double thrusttime;
            string nss = Regex.Replace(textBox1.Text, "[^0-9]", ""); //replace all characters other than numbers with nothing
            //double ns = (double)Int32.Parse(textBox1.Text) / 1000; //convert miliseconds entered to seconds //duplicate of thrusttime
            double ns = (double) Int32.Parse(nss);
            double i = 0; //basically a less efficient duplicate of totaltime
            bool b = false;
            double previousdouble = 0;
            chart1.Series["Series1"].Points.Clear();
            chart1.ChartAreas[0].AxisX.IsMarginVisible = false;
            try
            {
                foreach (var s in richTextBox1.Lines) //for each line in the textbox, process said line
                {
                    string s3 = Regex.Replace(s, "[^0-9.]", ""); //replace all characters other than numbers with nothing
                    //string line = richTextBox1.Lines[0];
                    if (!string.IsNullOrEmpty(s3)) //if the string is not empty, commence forth
                    {
                        
                        string s2 = s3;
                        if (s.Contains(" ") == true)
                        {
                            string[] words = s.Split(' ');
                            s2 = words[1];
                            //MessageBox.Show("go go gadget split string!");
                        }


                        double linedouble = double.Parse(s2); //convert from string to double

                        if (linedouble > previousdouble) //if linedouble is more than previous highest, write as new highest
                        {
                            previousdouble = linedouble; //this should end up as the highest single value in the readings textbox
                        }
                        thrust = thrust + linedouble; //add double to total
                        chart1.Series["Series1"].Points.AddXY(i, linedouble);
                        i += ns;
                    }
                    else //if the line is empty, delete the line
                    {
                        richTextBox1.Text = Regex.Replace(richTextBox1.Text, @"^\s*$(\n|\r|\r\n)", "", RegexOptions.Multiline);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error Parsing Data");
                throw;
            }

            //Math.DivRem(Int32.Parse(textBox1.Text), 1000, out timeincrement);
            //MessageBox.Show(previousdouble.ToString());
            
            textBox9.Text = previousdouble.ToString();//print peak thrust to textbox
            
            //math zone
            double g = 9.80665;
            double timeincrement = ns;
            //double timeincrement = (double) Int32.Parse(textBox1.Text) / 1000; //convert miliseconds entered to seconds
            double impulse = thrust * timeincrement * g; //calculate impulse from total thrust times the time increment calculated above together with the gravitational constant (g)
            thrusttime = timeincrement * richTextBox1.Lines.Count(); //calculate total time from amount of values and time increment
            double averagethrust = impulse / thrusttime; //get average thrust by dividing impulse by time
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

        void motorclass(double ns)
        {
            //calculate motor classification from newton seconds
            //i would use a switch statement, but i don't know if / how you can use and compare float values
            string mc = "";
            if (ns > 0 && ns < 0.3125)
            {
                mc = "Micro";
            }
            else if (ns >= 0.3125 && ns < 0.625)
            {
                mc = "1/4A";
            }
            else if (ns >= 0.625 && ns < 1.25)
            {
                mc = "1/2A";
            }
            /*else if (ns < 1.26 )
            {
                mc = "N/A";
            }*/
            else if (ns >= 1.25 && ns < 2.5)
            {
                mc = "A";
            }
            else if (ns >= 2.5 && ns < 5)
            {
                mc = "B";
            }
            else if (ns >= 5 && ns < 10)
            {
                mc = "C";
            }
            else if (ns >= 10 && ns < 20)
            {
                mc = "D";
            }
            else if (ns >= 20 && ns < 40)
            {
                mc = "E";
            }
            else if (ns >= 40 && ns < 80)
            {
                mc = "F";
            }
            else if (ns >= 80 && ns < 160)
            {
                mc = "G";
            }
            else if (ns >= 160 && ns < 320)
            {
                mc = "H";
            }
            else if (ns >= 320 && ns < 640)
            {
                mc = "I";
            }
            else if (ns >= 640 && ns < 1280)
            {
                mc = "J";
            }
            else if (ns >= 1280 && ns < 2560)
            {
                mc = "K";
            }
            else if (ns >= 2560 && ns < 5120)
            {
                mc = "L";
            }
            else if (ns >= 5120 && ns < 10240)
            {
                mc = "M";
            }
            else if (ns >= 10240 && ns < 20480)
            {
                mc = "N";
            }
            else if (ns >= 20480 && ns < 40960)
            {
                mc = "O";
            }
            else if (ns >= 40960 && ns < 81920)
            {
                mc = "P";
            }
            else if (ns >= 81920 && ns < 163840)
            {
                mc = "Q";
            }
            else if (ns >= 163840 && ns < 327680)
            {
                mc = "R";
            }
            else if (ns >= 327680 && ns < 655360)
            {
                mc = "S";
            }
            else if (ns >= 655360 && ns < 1310720)
            {
                mc = "T";
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
    }
}
