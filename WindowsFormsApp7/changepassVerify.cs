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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace WindowsFormsApp7
{
    public partial class changepassVerify: Form
    {
        private readonly HttpClient httpClient = new HttpClient();
        private string userEmail;
        public changepassVerify(string email)
        {
            InitializeComponent();
            
            userEmail = email;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            var payload = new
            {
                Email = userEmail,
                Code = txtVerify.Text
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("https://localhost:7154/api/emails/verify", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("✅ Password updated successfully. You can now log in.");
                new Login(userEmail).Show();
                this.Close();
            }
            else
            {
                MessageBox.Show($"❌ Verification failed: {responseBody}");
            }
        }
    }
    
}
