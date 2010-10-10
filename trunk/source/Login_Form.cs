using System;
using System.Windows.Forms;
using Google.GData.Calendar;

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
        public System.Uri PostURI { get; set; }

        #endregion

        public Login_Form()
        {
            this.InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.User = this.txtLogin.Text;
            this.Password = this.txtPassword.Text;

            cmbGoogleCalendar.Items.Clear();
            CalendarFeed calFeed = GoogleCalendar.RetrieveCalendars(this.User, this.Password);
            foreach (CalendarEntry centry in calFeed.Entries)
            {
                cmbGoogleCalendar.Items.Add(centry);
            }

            cmbGoogleCalendar.DisplayMember = "Title";
            cmbGoogleCalendar.ValueMember = "Title";
            if (cmbGoogleCalendar.Items.Count > 0)
            {
                cmbGoogleCalendar.SelectedIndex = 0;
            }
        }

        private void LoginTextControls_TextChanged(object sender, EventArgs e)
        {
            TextBoxBase senderTB = (TextBoxBase)sender;

            if (senderTB.Text.Contains(Environment.NewLine))
            {
                this.btnOK_Click(sender, e);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //do we need some kind of validation here?

            this.SetPostURI();
            this.Hide();
        }

        private void cmbGoogleCalendar_Leave(object sender, EventArgs e)
        {
            this.SetPostURI();
        }

        private void SetPostURI()
        {
            this.PostURI = new Uri("http://www.google.com/calendar/feeds/" +
                      ((CalendarEntry)(cmbGoogleCalendar.SelectedItem)).SelfUri.ToString().Substring(((CalendarEntry)(cmbGoogleCalendar.SelectedItem)).SelfUri.ToString().LastIndexOf("/") + 1) +
                      "/private/full");
        }
    }
}
