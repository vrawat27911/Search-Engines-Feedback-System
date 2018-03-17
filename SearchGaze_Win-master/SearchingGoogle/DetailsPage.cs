using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchingGoogle
{
    public partial class DetailsPage : Form
    {
        private int genCode;
        public UserData userData;

        public DetailsPage()
        {
            InitializeComponent();
            this.ActiveControl = txtUserId;
        }

        private void RaiseWarning()
        {
            // Create a GroupBox and add a TextBox to it.
            Label label1 = new Label();
            label1.Location = new Point(15, 15);
            groupBox1.Controls.Add(label1);

            // Set the Text and Dock properties of the GroupBox.
            label1.Text = "Please ensure all details are filled and correct.";
            label1.ForeColor = Color.Red;
            label1.AutoSize = true;
            // groupBox1.Dock = DockStyle.Top

            // Add the Groupbox to the form.
            this.Controls.Add(groupBox1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            userData = new UserData();

            try
            {
                userData.UserID = int.Parse(txtUserId.Text);
                userData.MFirstName = txtFName.Text;
                userData.MLastName = txtLName.Text;
                userData.MAge = int.Parse(txtAge.Text);
                userData.MGenderCode = genCode;
                Utility.writeFile(userData);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch
            {
                RaiseWarning();
            }
        }

        private void rdbtnMale_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbtnMale.Checked)
            {
                genCode = 1;
            }
        }

        private void rdbtnFemale_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbtnFemale.Checked)
            {
                genCode = 2;
            }

        }

        private void rdbtnOther_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbtnOther.Checked)
            {
                genCode = 3;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtUserId.Text = "";
            txtFName.Text = "";
            txtLName.Text = "";
            txtAge.Text = "";
            txtUserId.Focus();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
