using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;



namespace WindowsFormsApp7
{
    public partial class Login: Form
    {
        private string userEmail;
        public static string LoggedInUser = "";
        private readonly HttpClient httpClient = new HttpClient();

        private readonly string supabaseUrl = "https://cyugvmkmbjwyjsnutaph.supabase.co/rest/v1/Userss";
        private readonly string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImN5dWd2bWttYmp3eWpzbnV0YXBoIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc1ODQzNDg0NSwiZXhwIjoyMDc0MDEwODQ1fQ.Fua0y9qZc7vW-Wpa-nD2JE8AYX6Wxab0Vgkuq5kGpXs"; 

        public Login(string email)
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

        private  async void button1_Click(object sender, EventArgs e)
        {
            string Email = txtEmail.Text.Trim();
           
            string enteredPassword = txtPassword.Text;

            

            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(enteredPassword))
            {
                MessageBox.Show("Please enter a password and a valid email address.");
                return;
            }


            try
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {supabaseKey}");

                string queryUrl = $"{supabaseUrl}?Email=eq.{Email}&select=id,PasswordHash,failedattempts,islocked";


                var response = await httpClient.GetAsync(queryUrl);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Supabase error: {response.StatusCode}\n{responseBody}");
                    return;
                }
            

                dynamic users = JsonConvert.DeserializeObject(responseBody);
                if (users.Count == 0)
                {
                    MessageBox.Show("This email is not registered. Please sign up first.");
                    new Register(userEmail).Show();
                    this.Hide();
                    return;
                }
                var user = users[0];
                string storedHash = user.PasswordHash;
                int failedAttempts = user.failedattempts;
                bool isLocked = user.islocked;

                if (isLocked)
                {
                    MessageBox.Show("Your account is locked due to multiple failed login attempts. Please reset your password via email.");
                    button1.Enabled = false;

                    ChangePassPage resetForm = new ChangePassPage(Email);
                    resetForm.Show();
                    this.Hide(); 
                    
                    return;
                }

                bool isValid = BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);

                if (isValid)
                {
                    // Reset FailedAttempts
                    var resetPayload = new { failedattempts = 0 };
                    var resetContent = new StringContent(JsonConvert.SerializeObject(resetPayload), Encoding.UTF8, "application/json");
                    var resetRequest = new HttpRequestMessage(new HttpMethod("PATCH"), $"{supabaseUrl}?Email=eq.{Email}")
                    {
                        Content = resetContent
                    };
                    await httpClient.SendAsync(resetRequest);

                    LoggedInUser = Email;
                    AccountSettings settingsForm = new AccountSettings(LoggedInUser);
                    try
                    {
                        string receptorEmail = Email;
                        string password = enteredPassword;
                        string sentCode = "";

                        var emailResponse = await httpClient.PostAsync(
                            $"https://localhost:7154/api/emails?receptor={Uri.EscapeDataString(receptorEmail)}",
                            null
                        );

                        if (emailResponse.IsSuccessStatusCode)
                        {
                            var json = await emailResponse.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                            if (result.ContainsKey("code"))
                            {
                                sentCode = result["code"];
                                MessageBox.Show($"Verification code sent to {receptorEmail}");

                                Verification v = new Verification(sentCode, receptorEmail, password, isLoginFlow: true);
                                v.Show();
                                this.Hide();
                            }
                            else
                           
                                {
                                    MessageBox.Show("Email sent, but no verification code received.");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Failed to send email.");
                            }
                        }
                         catch (Exception ex)
                        {
                        MessageBox.Show($"Error sending verification code: {ex.Message}");
                        }
                     }
                else
                {
                    failedAttempts++;

                    if (failedAttempts >= 3)
                    {
                        var lockPayload = new { failedattempts = failedAttempts, islocked = true };
                        var lockContent = new StringContent(JsonConvert.SerializeObject(lockPayload), Encoding.UTF8, "application/json");
                        var lockRequest = new HttpRequestMessage(new HttpMethod("PATCH"), $"{supabaseUrl}?Email=eq.{Email}")
                        {
                            Content = lockContent
                        };
                        await httpClient.SendAsync(lockRequest);

                        MessageBox.Show("Your account has been locked after 3 failed attempts. Please reset your password via email.", "Account Locked", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        button1.Enabled = false;

                    }
                    else
                    {
                        var updatePayload = new { failedattempts = failedAttempts };
                        var updateContent = new StringContent(JsonConvert.SerializeObject(updatePayload), Encoding.UTF8, "application/json");
                        var updateRequest = new HttpRequestMessage(new HttpMethod("PATCH"), $"{supabaseUrl}?Email=eq.{Email}")
                        {
                            Content = updateContent
                        };
                        await httpClient.SendAsync(updateRequest);

                        MessageBox.Show($"Incorrect password. {3 - failedAttempts} attempt(s) left.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login error: " + ex.Message);
            }
        }



               

        private void label3_Click(object sender, EventArgs e)
        {
            Register v = new Register(userEmail);
            v.Show();
            this.Hide();
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

        private void label4_Click(object sender, EventArgs e)
        {
            RegistrationPage r = new RegistrationPage(userEmail);
            r.Show();
            this.Close();
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

        private void label5_Click(object sender, EventArgs e)
        {
            ChangePassPage v = new ChangePassPage(userEmail);
            v.Show();
            this.Hide();

        }
    }
    
}
