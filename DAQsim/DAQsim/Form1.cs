using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;


namespace DAQsim
{
    public partial class Form1 : Form
    {
        /* Global variables */
        static int sam_mS_init = 1700; // milliseconds
        static int log_mS_init = 30000; // milliseconds

        int sam_mS = sam_mS_init; // Counter for next samling timer
        int log_mS = log_mS_init; // Counter for next logging timer

        int numberOfWritings;
        public Form1()
        {
            InitializeComponent();

            // Initializing variables
            int numberOfSensors = Convert.ToInt16(txtSamplingDev.Text);
            string initTextSens;
            string[] dVal = new String[numberOfSensors];


            // Initializing the sampling textbox with a header
            for (int i = 1; i <= numberOfSensors; i++)
            {
                initTextSens = String.Format("nr. {0}\t\t", i);
                txtSensorVal.AppendText(initTextSens);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        /*
         * EXERCISE 7
         * 
         * Click-event for Sampling button,
         * 1) is disabled if no valid sampling time
         * 2) reads all the sensor values
         * 3) displays the sensor values in txtSensorVal textbox
         * 4) updates the "time left" field, by using the nextSampling tick event handler
         */

        private void btnSampling_Click(object sender, EventArgs e)
        {
            int num = Convert.ToInt16(this.txtSamplingDev.Text);
            string textSens = "";

            // Creating array of 8 sensor objects
            Sensor[] mySensor = new Sensor[num];
            for (int i = 0; i < num; i++)
            {
                mySensor[i] = new Sensor(i);
            }

            // Getting the sensor values and saving them to a string
            txtSensorVal.AppendText("\n");
            for (int i = 0; i < num; i++)
            {
                textSens = mySensor[i].GetValue().ToString("F4"); // Resolution set to 4 decimals for cleaner display
                txtSensorVal.AppendText(textSens + "\t\t");
            }


            nextSamling.Start();
        }

        private void nextSamling_Tick(object sender, EventArgs e)
        {
            if (sam_mS > 0)
            {
                sam_mS -= 10;
                txtNextSampling.Text = Convert.ToString(sam_mS);
                btnSampling.Enabled = false; 
            }
            else
            {
                txtNextSampling.Text = "0";
                btnSampling.Enabled = true;
                sam_mS = sam_mS_init;
                nextSamling.Stop();
            }
        }




        /*
         * EXERCISE 9
         * 
         * Help About Information box, explaining to the user what 
         * this application does. 
         */
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This application simulates data acquisition from a DAQ device reading 8 sensors. Samples and logfiles can be made hitting the buttons. Samples can be taken with minimum 1,7 seconds interval and logfiles can be made with minimum 30 seconds interval!", "Help!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }

        /*
         * EXERCISE 10
         * 
         * Logging button
         * 1) is disabled if no valid logging time 
         * 2) write the last read sensor values to a text file, include the time and append to the file contents if the file already exists          * 3) include a field showing the file name and number of writings         * 4) update the Next Logging Time field.
         */
        private void btnLogging_Click(object sender, EventArgs e)
        {
            /* Get date and time */
            DateTime myCurrentDate = DateTime.Today ; // Get current date
            string currentDate = myCurrentDate.ToString("yyyy/MM/dd");

            DateTime myCurrentTime = DateTime.Now; // Get current time
            string currentTime = myCurrentTime.ToString("hh:mm:ss");



            if (txtSensorVal.Lines.Length > 1) // If samples are made
            {
                /* Get last samples */
                string lastSamples = "";
                string header = "";
                header = txtSensorVal.Lines[0];
                lastSamples = txtSensorVal.Lines[txtSensorVal.Lines.Length - 1];
                nextLogging.Start();
                

                /* Compose filename and path */
                string filename = Convert.ToString(currentDate + ".txt"); // Name the files with the current date
                string path = Convert.ToString(@"C:\Users\Rafael Johansen\Dropbox\Master - HSN\2nd Semester\Object-oriented Analysis, Design and Programming\Assignment 1 - DAQ simulator\Logging\" + filename);

                /* Write/append data to .txt file */
                if (!File.Exists(path)) // If file doesn't exist -> initialize with header and write first line
                {
                    File.Create(path).Dispose();
                    using (TextWriter tw = new StreamWriter(path))
                    {
                        numberOfWritings = 1; // first writing
                        tw.WriteLine("Filename: " + filename); // Header
                        tw.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------------------");
                        tw.WriteLine("Writing\t\tCurrent time\t" + header);
                        tw.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------------------");
                        tw.WriteLine("nr. " + numberOfWritings + "\t\t" + currentTime + "\t" + lastSamples);
                        tw.Close();
                    }

                }

                else if (File.Exists(path)) // If file exists -> append new line
                {
                    using (TextWriter tw = new StreamWriter(path, true))
                    {
                        numberOfWritings++; // increase number of writings
                        tw.WriteLine("nr. " + numberOfWritings + "\t\t" + currentTime + "\t" + lastSamples);
                        tw.Close();

                    }
                }

            }
            else // If no samples are made
            {
                MessageBox.Show("No samples found! First retrieve a sample.");
            }

        }

        private void nextLogging_Tick(object sender, EventArgs e)
        {
            if (log_mS > 0)
            {
                log_mS -= 10;
                txtNextLogging.Text = Convert.ToString(log_mS);
                btnLogging.Enabled = false;
            }
            else
            {
                txtNextLogging.Text = "0";
                btnLogging.Enabled = true;
                log_mS = log_mS_init;
                nextLogging.Stop();
            }
        }
    }

    /*
     * EXERCISE 6
     * 
     * Sensor class, simulating measurement values
     * with the following specifications
     * 
     * DAQ input voltage: -5V to 5V
     * DAQ resolution: 20 bits
     */
    class Sensor
    {
        double dVal;
        static Random randSensVal;
        public Sensor(int id)
        {
            randSensVal = new Random();
            dVal = 0.0F;
        }
        public double GetValue()
        {
            dVal = (randSensVal.NextDouble() * 10) - 5; // Range -5 to 5 volts
            return dVal;
        }
    }
}