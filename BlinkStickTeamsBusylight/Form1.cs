using BlinkStickDotNet;
using Microsoft.VisualBasic.Logging;
using Ro.Teams.LocalApi;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace BlinkStickTeamsBusylight
{
    public partial class Form1 : Form
    {
        string logFileName;
        NewTeamsStatus teamsStatus;
        BlinkStick led;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            teamsStatus = new NewTeamsStatus();
            led = BlinkStick.FindFirst();


            timer1.Start();

        }


        private void SetBlinkStickStatus(string teamsStatus)
        {
            bool blinking = false;
            string color = "black";


            if (teamsStatus.Contains("Available"))
            {
                blinking = false;
                color = "green";
            }
            else if (teamsStatus.Contains("Away"))
            {
                blinking = false;
                color = "black";
            }
            else if (teamsStatus.Contains("OnThePhone"))
            {
                blinking = true;
                color = "red";
            }
            else if (teamsStatus.Contains("InAMeeting"))
            {
                blinking = false;
                color = "red";
            }
            else if (teamsStatus.Contains("DoNotDisturb"))
            {
                blinking = true;
                color = "red";
            }
            else if (teamsStatus.Contains("Presenting"))
            {
                blinking = true;
                color = "red";
            }
            else if (teamsStatus.Contains("Busy"))
            {
                blinking = false;
                color = "red";
            }
            else if (teamsStatus.Contains("Offline"))
            {
                blinking = false;
                color = "black";
            }
            else if (teamsStatus.Contains("BeRightBack"))
            {
                blinking = true;
                color = "purple";
            }
            else
            {
                blinking = false;
                color = "black";
            }

            listBox1.Items.Add(teamsStatus);
            groupBox1.BackColor = Color.FromName(color);
            led.OpenDevice();
            led.SetColor(color);
            if (blinking)
            {
                //led.Blink(color);
                led.Pulse(color, 5);
            }


        }
    

        private void timer1_Tick(object sender, EventArgs e)
        {
            string status = teamsStatus.GetStatus();
            SetBlinkStickStatus(status);
        }
    }
}