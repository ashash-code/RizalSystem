using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Net.Http;
using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WindowsFormsApp7
{
    public partial class Register : Form
    {
        private string userEmail;
        private string sentCode = "";
        private readonly HttpClient httpClient = new HttpClient();
        public Register(string email)
        {
            InitializeComponent();
            txtPassword.PasswordChar = '*';
            checkBox1.Checked = false;

            userEmail = email;

            txtPassword.ContextMenu = new ContextMenu();
            
            txtPassword.KeyDown += txtPassword_KeyDown;



        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string password = txtPassword.Text;
            
            string receptorEmail = txtEmail.Text.Trim();

            if (string.IsNullOrEmpty(receptorEmail) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter a password and a valid email address.");
                return;
            }
            

            string supabaseUrl = "https://cyugvmkmbjwyjsnutaph.supabase.co/rest/v1/Userss"; // Replace with your actual Supabase URL
            string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImN5dWd2bWttYmp3eWpzbnV0YXBoIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc1ODQzNDg0NSwiZXhwIjoyMDc0MDEwODQ1fQ.Fua0y9qZc7vW-Wpa-nD2JE8AYX6Wxab0Vgkuq5kGpXs"; // Replace with your actual anon key

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {supabaseKey}");

            var response = await httpClient.GetAsync($"{supabaseUrl}?Email=eq.{Uri.EscapeDataString(receptorEmail)}&select=Email");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var results = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);

                if (results.Count > 0)
                {
                    MessageBox.Show("This email is already registered. Please proceed to login.");
                    Login loginForm = new Login(userEmail);
                    loginForm.Show();
                    this.Hide(); // Optional: hide the register form
                    return;
                }
            }
            else
            {
                MessageBox.Show("Failed to connect to Supabase. Please check your API key and URL.");
                return;
            }
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

                        Verification v = new Verification(sentCode, receptorEmail, password);
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
            Login v = new Login(userEmail);
            v.Show();
            this.Hide();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            RegistrationPage r = new RegistrationPage();
            r.Show();
            this.Close();
        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
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

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }




            

        private void txtPassword_MouseDown(object sender, MouseEventArgs e)
        {
            
        }
    }
    
    
}
