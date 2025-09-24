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

namespace WindowsFormsApp7
{
    public partial class AdminLogin : Form
    {
        private string userEmail;
        private string sentCode = "";
        public static string LoggedInUser = "";
        private readonly HttpClient httpClient = new HttpClient();
        private readonly string supabaseUrl = "https://cyugvmkmbjwyjsnutaph.supabase.co/rest/v1/Userss";
        private readonly string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImN5dWd2bWttYmp3eWpzbnV0YXBoIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc1ODQzNDg0NSwiZXhwIjoyMDc0MDEwODQ1fQ.Fua0y9qZc7vW-Wpa-nD2JE8AYX6Wxab0Vgkuq5kGpXs";


        public AdminLogin(string email)
        {
            InitializeComponent();
            txtPassword.PasswordChar = '*';
            checkBox1.Checked = false;

            userEmail = email;

            txtPassword.ContextMenu = new ContextMenu();
            txtPassword.MouseDown += txtPassword_MouseDown;
            txtPassword.KeyPress += txtPassword_KeyPress;
            txtPassword.KeyDown += txtPassword_KeyDown;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string Email = txtEmail.Text.Trim();

            string enteredPassword = txtPassword.Text;
            string receptorEmail = txtEmail.Text.Trim();



            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(enteredPassword))
            {
                MessageBox.Show("Please enter a password and a valid email address.");
                return;
            }

            if (txtEmail.Text == "ashley.m.ebreo@gmail.com" && txtPassword.Text == "Admin123!")
            {
                try
                {
                    var emailResponse = await httpClient.PostAsync(
                    $"https://localhost:7154/api/emails?receptor={Uri.EscapeDataString(receptorEmail)}",
                    null
                    );


                    if (emailResponse.IsSuccessStatusCode)
                    {
                        var json = await emailResponse.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                        sentCode = result["code"]; // Store the code
                        MessageBox.Show($"Verification code sent to {receptorEmail}");

                        AdminVerification v = new AdminVerification(sentCode, receptorEmail, enteredPassword);
                        v.Show();
                        this.Hide();

                    }
                    else
                    {
                        MessageBox.Show("Failed to send email.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }

            }
        }
           
               

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                txtPassword.PasswordChar = '\0';
            }
            else
            {
                txtPassword.PasswordChar = '*';
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            
        }

        private void label4_Click(object sender, EventArgs e)
        {
            RegistrationPage r = new RegistrationPage(userEmail);
            r.Show();
            this.Close();
        }

        private void button1_HelpRequested(object sender, HelpEventArgs hlpevent)
        {

        }

        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void txtEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Prevents the ding sound
                button1.PerformClick();  // Triggers the button click
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Prevents the ding sound
                button1.PerformClick();  // Triggers the button click
            }
            if ((e.Control && e.KeyCode == Keys.C))
            {
                MessageBox.Show("Copying and pasting is not allowed in this field.");
                e.SuppressKeyPress = true; // Prevent the action
            }
            if ((e.Control && e.KeyCode == Keys.V))
            {
                MessageBox.Show("Copying and pasting is not allowed in this field.");
                e.SuppressKeyPress = true; // Prevent the action
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            string password = txtPassword.Text.Trim();
            txtPassword.ForeColor = Color.Red;

            if (password.Length < 8)
            {
                lblPasswordError.Text = "Minimum 8 characters required.";
                return;
            }

            if (!password.Any(char.IsUpper))
            {
                lblPasswordError.Text = "Add at least one uppercase letter.";
                return;
            }

            if (!password.Any(char.IsLower))
            {
                lblPasswordError.Text = "Add at least one lowercase letter.";
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"\d"))
            {
                lblPasswordError.Text = "Add at least one number.";
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[\W_]"))
            {
                lblPasswordError.Text = "Add at least one special character.";
                return;
            }

            lblPasswordError.Text = "Ready!";
            txtPassword.ForeColor = Color.Green;
        }

        private void txtPassword_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                MessageBox.Show("Right-click copying is disabled.");
            }

        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (e.KeyChar == 3) // Ctrl+C
                {
                    MessageBox.Show("Copying is not allowed.");
                    e.Handled = true;
                }
            }
        }
    }
    
}
