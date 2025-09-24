using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp7
{
    public partial class Home : Form
    {
        private Timer inactivityTimer;
        private TimeSpan timeout = TimeSpan.FromMinutes(5); // Set your desired timeout
        private DateTime lastActivityTime;
        private string userEmail;
        public Home(string email)
        {
            InitializeComponent();
            userEmail = email;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Settings v = new Settings(userEmail);
            v.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RegistrationPage v = new RegistrationPage();
            v.Show();
            this.Hide();
        }

        private void Home_Load(object sender, EventArgs e)
        {
            lastActivityTime = DateTime.Now;

            inactivityTimer = new Timer();
            inactivityTimer.Interval = 1000; // Check every second
            inactivityTimer.Tick += InactivityTimer_Tick;
            inactivityTimer.Start();

            // Hook into user activity
            this.MouseMove += ResetInactivityTimer;
            this.KeyPress += ResetInactivityTimer;

        }
        private void ResetInactivityTimer(object sender, EventArgs e)
        {
            lastActivityTime = DateTime.Now;
        }
        private void InactivityTimer_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now - lastActivityTime > timeout)
            {
                inactivityTimer.Stop();
                MessageBox.Show("You've been logged out due to inactivity.", "Session Expired", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Redirect to login or close app
                new Login(userEmail).Show();
                this.Close();
            }
        }
    }
}
