using System;
using System.Drawing;
using System.Windows.Forms;

namespace BipartiteProject
{
    public partial class BipartiteMatchingForm : Form
    {
        private readonly DrawPanel _panel;
        
        public BipartiteMatchingForm()
        {
            InitializeComponent();
            _panel = new DrawPanel();
            _panel.Location = new Point(55, 10);
            _panel.Size = new Size(400, 400);
            _panel.BackColor = Color.Yellow;
            _panel.FirstLineDrawn += (o, e) => findMatching.Enabled = true;
            Controls.Add(_panel);
        }

        private void FindMatching_Click(object sender, EventArgs e)
        {
            _panel.FindMatching();
        }
    }
    
}