﻿using System;
using System.Windows.Forms;

namespace RuneApp
{
    public partial class Help : Form
    {
        public string url = null;

        public Help()
        {
            InitializeComponent();
            webBrowser1.Navigated += WebBrowser1_Navigated;

            Shown += Help_Shown;
        }

        private void Help_Shown(object sender, EventArgs e)
        {
            if (url == null)
                url = Environment.CurrentDirectory + "\\User Manual\\index.html";

            webBrowser1.Navigate(url);
        }

        private void WebBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            toolStripButton1.Enabled = webBrowser1.CanGoBack;
            toolStripButton2.Enabled = webBrowser1.CanGoForward;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
