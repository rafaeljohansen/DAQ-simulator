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


namespace DAQsim
{
    public partial class Form1 : Form
    {
        static int sam_mS_init = 1700; // milliseconds
        static int log_mS_init = 30000; // milliseconds

        int sam_mS = sam_mS_init; // Counter for next samling timer
        int log_mS = log_mS_init; // Counter for next logging timer

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
            for (int i = 0; i < num; i++)
            {
                textSens = mySensor[i].GetValue().ToString("F5"); // Resolution set to 5 decimals
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