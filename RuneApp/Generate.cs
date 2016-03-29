﻿using RuneOptim;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace RuneApp
{
    // Generates a bunch of builds to preview the stats
    public partial class Generate : Form
    {
		private static string[] statNames = new string[] { "HP", "ATK", "DEF", "SPD", "CR", "CD", "RES", "ACC" };
        private static string[] extraNames = new string[] { "EHP", "EHPDB", "DPS", "AvD", "MxD" };

        // the build to use
		public Build build = null;

        // if making builds
		bool building = false;
		
		public Generate(Build bb)
        {
            InitializeComponent();
            
            // master has given Gener a Build?
			build = bb;
			Label label = null;
            TextBox textBox = null;

            // cool clicky thing
            var sorter = new ListViewSort();
            // sort decending on POINTS
            sorter.OnColumnClick(0, false);
            listView1.ListViewItemSorter = sorter;
            
            // place controls in a nice grid-like manner
			int x, y;

			y = 20;
			foreach (string stat in statNames)
			{
				x = 25;
				label = new Label();
				label.Name = stat + "Label";
				label.Location = new Point(x, y);
				label.Size = new Size(40, 20);
				label.Text = stat;
				groupBox1.Controls.Add(label);
				x += 45;

				textBox = new TextBox();
				textBox.Name = stat + "Worth";
				textBox.Location = new Point(x, y);
				textBox.Size = new Size(40, 20);
				if (build.Sort[stat] != 0)
					textBox.Text = build.Sort[stat].ToString();
                textBox.TextChanged += textBox_TextChanged;
                groupBox1.Controls.Add(textBox);

				y += 22;

                listView1.Columns.Add(stat).Width = 80;
			}
            foreach (string extra in extraNames)
            {
                x = 25;
                label = new Label();
                label.Name = extra + "Label";
                label.Location = new Point(x, y);
                label.Size = new Size(40, 20);
                label.Text = extra;
                groupBox1.Controls.Add(label);
                x += 45;

                textBox = new TextBox();
                textBox.Name = extra + "Worth";
                textBox.Location = new Point(x, y);
                textBox.Size = new Size(40, 20);
                if (build.Sort.ExtraGet(extra) != 0)
                    textBox.Text = build.Sort.ExtraGet(extra).ToString();
                textBox.TextChanged += textBox_TextChanged;
                groupBox1.Controls.Add(textBox);

                y += 22;

                listView1.Columns.Add(extra).Width = 80;
            }

			toolStripStatusLabel1.Text = "Generating...";
			building = true;

			Task.Factory.StartNew(() =>
            {
                // Allow the window to draw before destroying the CPU
                Thread.Sleep(100);

                // Disregard locked, but honor equippedness checking
				build.GenRunes(Main.data, true, Main.useEquipped);

                // generate 5000 builds
				build.GenBuilds(5000, 0, (s) => { }, (d) =>
				{
					Invoke((MethodInvoker)delegate
					{
						toolStripProgressBar1.Value = (int)(d * 500);
					});
				});
                
                // pick the top 400
                // Believe it or not, putting 400 into the list takes a *lot* longer than making 5000
				foreach (var b in build.loads.Take(400))
				{
					ListViewItem li = new ListViewItem();
					int pts = 0;

					var Cur = b.GetStats();

					foreach (var stat in statNames)
					{
						string str = Cur[stat].ToString();
						if (build.Sort[stat] != 0)
						{
							int p = Cur[stat] / build.Sort[stat];
							if (build.Maximum[stat] != 0)
								p -= Math.Max(0, Cur[stat] - build.Maximum[stat]) / build.Sort[stat] * 2;
							str = p.ToString() + " (" + Cur[stat].ToString() + ")";
							pts += p;
						}
						li.SubItems.Add(str);
					}
                    foreach (var extra in extraNames)
                    {
                        string str = Cur.ExtraValue(extra).ToString();
                        if (build.Sort.ExtraGet(extra) != 0)
                        {
                            int p = Cur.ExtraValue(extra) / build.Sort.ExtraGet(extra);
                            if (build.Maximum.ExtraGet(extra) != 0)
                                p -= Math.Max(0, Cur.ExtraValue(extra) - build.Maximum.ExtraGet(extra)) / build.Sort.ExtraGet(extra) * 2;
                            str = p.ToString() + " (" + Cur.ExtraValue(extra).ToString() + ")";
                            pts += p;
                        }
                        li.SubItems.Add(str);
                    }
                    // put the sum points into the first item
					li.SubItems[0].Text = pts.ToString();
					li.Tag = b;
					Invoke((MethodInvoker)delegate
					{
                        // put the thing in on the main thread and bump the progress bar
						listView1.Items.Add(li);
						toolStripProgressBar1.Value = 500 + (int)(500 * listView1.Items.Count / (double) build.loads.Count());
					});
				}

				Invoke((MethodInvoker)delegate
				{
					toolStripStatusLabel1.Text = "Generated " + listView1.Items.Count + " builds";
					building = false;
				});

			});
        }

        void textBox_TextChanged(object sender, EventArgs e)
        {
            // if we are generating builds, don't recalculate all the builds
			if (building) return;

            // TODO: try to only mangle the column which is changing?

			foreach (string stat in statNames)
            {
                TextBox tb = (TextBox)Controls.Find(stat + "Worth", true).FirstOrDefault();
                int val = 0;
                int.TryParse(tb.Text, out val);
                build.Sort[stat] = val;
            }
            foreach (string extra in extraNames)
            {
                TextBox tb = (TextBox)Controls.Find(extra + "Worth", true).FirstOrDefault();
                int val = 0;
                int.TryParse(tb.Text, out val);
                build.Sort.ExtraSet(extra, val);
            }
            // "sort" as in, recalculate the whole number
            foreach (ListViewItem li in listView1.Items)
            {
                ListItemSort(li);
            }
            var lv = (ListView)listView1;
            var lvs = (ListViewSort)(lv).ListViewItemSorter;
            lvs.OnColumnClick(0, false, true);
            // actually sort the list, on points
			lv.Sort();
		}

        // recalculate all the points for this monster
        // TODO: consider hiding point values in the subitem tags and only recalcing the changed column
        // TODO: pull the scoring algorithm into a neater function
        public void ListItemSort(ListViewItem li)
        {
            Monster load = (Monster)li.Tag;
            int pts = 0;

            var Cur = load.GetStats();
            int i = 1;
            foreach (var stat in statNames)
            {
                string str = Cur[stat].ToString();
                if (build.Sort[stat] != 0)
                {
                    int p = Cur[stat] / build.Sort[stat];
					if (build.Maximum[stat] != 0)
						p -= Math.Max(0, Cur[stat] - build.Maximum[stat]) / build.Sort[stat] * 2;
                    str = p.ToString() + " (" + Cur[stat].ToString() + ")";
                    pts += p;
                }
                li.SubItems[i].Text = str;
                i++;
            }
            foreach (var extra in extraNames)
            {
                string str = Cur.ExtraValue(extra).ToString();
                if (build.Sort.ExtraGet(extra) != 0)
                {
                    int p = Cur.ExtraValue(extra) / build.Sort.ExtraGet(extra);
                    if (build.Maximum.ExtraGet(extra) != 0)
                        p -= Math.Max(0, Cur.ExtraValue(extra) - build.Maximum.ExtraGet(extra)) / build.Sort.ExtraGet(extra) * 2;
                    str = p.ToString() + " (" + Cur.ExtraValue(extra).ToString() + ")";
                    pts += p;
                }
                li.SubItems[i].Text = str;
                i++;
            }
            li.SubItems[0].Text = pts.ToString();

        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
			if (building) return;

            var sorter = (ListViewSort)((ListView)sender).ListViewItemSorter;
            sorter.OnColumnClick(e.Column, false, true);
            ((ListView)sender).Sort();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            // Things went okay
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            // things were :(
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
		}
    }
}
