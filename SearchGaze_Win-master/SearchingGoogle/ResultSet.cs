using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchingGoogle
{
    public class ResultSet
    {
        int numElements;
        GroupBox []resultGroupBoxes;
        Label []title;
        LinkLabel[] links;
        RichTextBox[] description;

        public int NumElements
        {
            get
            {
                return numElements;
            }

            set
            {
                numElements = value;
            }
        }

        public GroupBox[] ResultGroupBoxes
        {
            get
            {
                return resultGroupBoxes;
            }

            set
            {
                resultGroupBoxes = value;
            }
        }

        public Label[] Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
            }
        }

        public LinkLabel[] Links
        {
            get
            {
                return links;
            }

            set
            {
                links = value;
            }
        }

        public RichTextBox[] Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
            }
        }

        private void AddDetailsGroupBox()
        {
            for(int cnt = 0; cnt < this.numElements; cnt++)
            {
                this.title[cnt].Parent = this.resultGroupBoxes[cnt];
                this.links[cnt].Parent = this.resultGroupBoxes[cnt];
                this.description[cnt].Parent = this.resultGroupBoxes[cnt];
            }
        }
        public ResultSet(int pNumElements)
        {
            this.numElements = pNumElements;
            this.resultGroupBoxes = new GroupBox[pNumElements];
            this.title = new Label[pNumElements];
            this.links = new LinkLabel[pNumElements];
            this.description = new RichTextBox[pNumElements];
            AddDetailsGroupBox();
        }
    }
}
