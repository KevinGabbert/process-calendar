using System;
using System.Windows.Forms;

namespace ProcessCalendar
{
    /// <summary>
    /// This form is designed to pop up whenever login information is not known (and pre-populated)
    /// normally, the user would never see this form, and if they do, it would be rare.
    /// </summary>
    public partial class Login_Form : Form
    {
        #region Properties

        public string User { get; set; }
        public string Password { get; set; }

        #endregion

        public Login_Form()
        {
            this.InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.User = this.txtLogin.Text;
            this.Password = this.txtPassword.Text;

            this.Hide();
        }

        private void LoginTextControls_TextChanged(object sender, EventArgs e)
        {
            TextBoxBase senderTB = (TextBoxBase)sender;

            if (senderTB.Text.Contains(Environment.NewLine))
            {
                this.btnOK_Click(sender, e);
            }
        }
    }
}
