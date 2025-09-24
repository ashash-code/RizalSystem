using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace WindowsFormsApp7
{
    public partial class AdminVerification: Form
    {
        private string userEmail;
        private string sentCode = "";
        private string receptorEmail;
        private string PasswordHash;
        private readonly HttpClient httpClient = new HttpClient();

        public AdminVerification(string code, string email, string password)
        {
            InitializeComponent();
            this.sentCode = code;
            this.receptorEmail = email;
            this.PasswordHash = password;
            userEmail = email;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string userInput = txtVerify.Text.Trim();

            if (userInput == sentCode)
            {
                
                AdminHome l = new AdminHome(userEmail);
                l.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Incorrect code. Please try again.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AdminRegister v = new AdminRegister(userEmail);
            v.Show();
            this.Hide();
        }

        private void button2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Prevent ding sound
                button2.PerformClick();  // Simulate button click
            }
        }

        private void txtVerify_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Prevents the ding sound
                button2.PerformClick();  // Triggers the button click
            }
        }

        private void Registration_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
