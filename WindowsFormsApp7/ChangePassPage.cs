using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp7
{
    public partial class ChangePassPage: Form
    {
        private string userEmail;
        private readonly HttpClient httpClient = new HttpClient();

        
        public ChangePassPage(string email)
        {
            InitializeComponent();
            txtNewPassword.PasswordChar = '*';
            checkBox1.Checked = false;

            userEmail = email;

            txtNewPassword.ContextMenu = new ContextMenu();
            txtNewPassword.MouseDown += txtNewPassword_MouseDown;
            txtNewPassword.KeyPress += txtNewPassword_KeyPress;
            txtNewPassword.KeyDown += txtNewPassword_KeyDown;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                MessageBox.Show("Please enter both email and new password.");
                return;
            }

            
            var payload = new
            {
                email = txtEmail.Text,
                newPassword = txtNewPassword.Text
            };
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                MessageBox.Show("Please enter both email and new password.");
                return;
            }

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("https://localhost:7154/api/emails/request-password-reset", content);


            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Verification code sent to your email.");
                new changepassVerify(txtEmail.Text).Show(); // Pass email to next form
                this.Close();
            }
            else
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Status: {response.StatusCode}\nDetails: {responseBody}");

            }

        }

        private void label4_Click(object sender, EventArgs e)
        {
            Login v = new Login(userEmail);
            v.Show();
            this.Hide();
        }

        private async void txtNewPassword_TextChanged(object sender, EventArgs e)
        {
            string password = txtNewPassword.Text.Trim();
            txtNewPassword.ForeColor = Color.Red;


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
            try
            {
                var response = await httpClient.GetAsync($"https://localhost:7154/api/emails/get-password-hash?email={txtEmail.Text}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var records = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);
                    if (records != null && records.Count > 0 && records[0].ContainsKey("PasswordHash"))
                    {
                        string oldHash = records[0]["PasswordHash"];
                        if (BCrypt.Net.BCrypt.Verify(password, oldHash))
                        {
                            lblPasswordError.Text = "You cannot reuse your old password.";
                            txtNewPassword.ForeColor = Color.Red;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblPasswordError.Text = "Error checking password reuse.";
                Console.WriteLine(ex.Message);
                return;
            }


            lblPasswordError.Text = "Ready!";
            txtNewPassword.ForeColor = Color.Green;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                txtNewPassword.PasswordChar = '\0';
            }
            else
            {
                txtNewPassword.PasswordChar = '*';
            }
        }

        private void lblPasswordError_Click(object sender, EventArgs e)
        {

        }

        private void txtNewPassword_KeyDown(object sender, KeyEventArgs e)
        {
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

        private void txtNewPassword_MouseDown(object sender, MouseEventArgs e)
        {
            
        }

        private void txtNewPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }
    }
    
}
