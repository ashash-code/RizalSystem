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
    public partial class AdminHome: Form
    {
        private string userEmail;
        public AdminHome(string email)
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
    }
}
