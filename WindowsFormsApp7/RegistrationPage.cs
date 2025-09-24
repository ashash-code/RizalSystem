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
    public partial class RegistrationPage: Form
    {
        private string userEmail;

        public RegistrationPage()
        {
            InitializeComponent();
            

        }
        public RegistrationPage(string email)
        {
            InitializeComponent();
            userEmail = email;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Register v = new Register(userEmail);
            v.Show();
            this.Hide();

           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AdminLogin v = new AdminLogin(userEmail);
            v.Show();
            this.Hide();
        }
    }
}
