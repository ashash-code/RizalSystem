using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace WindowsFormsApp7
{
    public partial class Settings: Form
    {
        private string userEmail;
        public Settings(string email)
        {
            InitializeComponent();
            userEmail = email;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            RegistrationPage v = new RegistrationPage(userEmail);
            v.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            AccountSettings v = new AccountSettings(userEmail);
            v.Show();
            this.Hide();
        }
    }
}
