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
    public partial class MainWindow : Form
    {
        int UserId =1;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool isTrain = true;
            SearchWin obj = new SearchWin(UserId, isTrain);

            obj.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DetailsPage detailsDialog = new DetailsPage();

            // Show testDialog as a modal dialog and determine if DialogResult = OK.
            if (detailsDialog.ShowDialog(this) == DialogResult.OK)
            {

                // Read the contents of detailsDialog's TextBox.
                this.label3.Text = "Hello " + detailsDialog.userData.MFirstName + " " + detailsDialog.userData.MLastName;
                // Set User ID
                UserId = detailsDialog.userData.UserID; 
            }
            else
            {
                this.label3.Text = "Cancelled";
            }
            detailsDialog.Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bool isTrain = false;
            SearchWin obj = new SearchWin(UserId, isTrain);
            obj.Show();
        }
    }
}
