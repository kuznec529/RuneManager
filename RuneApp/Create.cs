﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using RuneOptim;

namespace RuneApp
{
    // Specifying a new build, are we?
    public partial class Create : Form
    {
        public Build build = null;

        // controls builds version numbers
        public static int VERSIONNUM = 1;
        
        // Keep track of the runeset groups for speed swaps
        private ListViewGroup rsInc;
        private ListViewGroup rsExc;
        private ListViewGroup rsReq;

        // keep track of the rune hex controls
        private RuneControl[] runes;

        // the current rune to look at
        private Rune runeTest = null;
        // is the form loading? wouldn't want to trigger any OnChanges, eh?
        private bool loading = false;

        private GenerateLive testWindow = null;

        private readonly LeaderType[] leadTypes = {
            new LeaderType(Attr.Null),
            new LeaderType(Attr.SpeedPercent).AddRange(new int[] { 0, 10, 13, 15, 16, 19, 23, 24, 28, 30, 33 }),
            new LeaderType(Attr.HealthPercent).AddRange(new int[] { 0, 15, 17, 18, 21, 25, 30, 33, 44, 50 }),
            new LeaderType(Attr.AttackPercent).AddRange(new int[] { 0, 15, 18, 20, 21, 22, 25, 30, 33, 35, 38, 40, 44, 50 }),
        };

        static readonly string[] tabNames = { "g", "o", "e", "2", "4", "6", "1", "3", "5" };
        
        private ToolTip tooltipNoSorting = new ToolTip();
        private ToolTip tooltipBadRuneFilter = new ToolTip();
        private ToolTip tooltipSets = new ToolTip();

        #region Leader skills
        public class LeaderType
        {
            public LeaderType(Attr t)
            {
                type = t;
            }

            public Attr type;
            public class LeaderValue
            {
                public LeaderValue(Attr t, int v)
                {
                    type = t;
                    value = v;
                }
                public int value;
                public Attr type;
                public override string ToString()
                {
                    return value.ToString() + ((type == Attr.HealthFlat || type == Attr.DefenseFlat || type == Attr.AttackFlat || type == Attr.Speed) ? "" : "%");
                }
            }

            public void Add(int i)
            {
                values.Add(new LeaderValue(type, i));
            }

            public LeaderType AddRange(int[] ii)
            {
                foreach (int i in ii)
                    Add(i);

                return this;
            }

            public List<LeaderValue> values = new List<LeaderValue>();

            public override string ToString()
            {
                if (type == Attr.Null) return "(None)";
                return type.ToString();
            }
        }
    #endregion

        public Create()
        {
            InitializeComponent();
            // when show, check we have stuff
            Shown += Create_Shown;

            // declare the truthyness of the groups and track them
            setList.Groups[1].Tag = true;
            rsInc = setList.Groups[1];
            setList.Groups[2].Tag = false;
            rsExc = setList.Groups[2];
            setList.Groups[0].Tag = false;
            rsReq = setList.Groups[0];
            
            if (leadTypes != null)
                leaderTypeBox.Items.AddRange(leadTypes);

            // for each runeset, put it in the list as excluded
            foreach (var rs in Enum.GetNames(typeof(RuneSet)))
			{
                if (rs != "Null" && rs != "Broken")
                {
                    ListViewItem li = new ListViewItem(rs);
                    li.Name = rs;
                    li.Tag = Enum.Parse(typeof(RuneSet), rs);
                    li.Group = setList.Groups[2];
                    setList.Items.Add(li);
                }
			}

            // track all the clickable rune things
            runes = new RuneControl[] { runeControl1, runeControl2, runeControl3, runeControl4, runeControl5, runeControl6 };
            for (int i = 0; i < runes.Length; i++)
            {
                runes[i].Tag = i + 1;
            }

            toolStripButton1.Tag = 0;

            // there are lists on the rune filter tabs 2,4, and 6
            var lists = new ListView[]{ priStat2, priStat4, priStat6};
            for(int j = 0; j < lists.Length; j++)
            {
                // mess 'em up
                var lv = lists[j];

                // for all the attributes that may appears as primaries on runes
                for (int i = 0; i < Build.statNames.Length; i++)
                {
					ListViewItem li;

                    string stat = Build.statNames[i];
                    if (i < 3)
                    {
                        li = new ListViewItem(stat);
                        li.Name = stat + "flat";
                        li.Text = stat;
                        li.Tag = stat + "flat";
                        li.Group = lv.Groups[1];
                        
                        lv.Items.Add(li);
                        
                        li = new ListViewItem(stat);
                        li.Name = stat + "perc";
                        li.Text = stat + "%";
                        li.Tag = stat + "perc";
                        li.Group = lv.Groups[1];

                        lv.Items.Add(li);
                        
                    }
                    else
                    {
                        // only allow cool stats on the right slots
                        if (j == 0 && stat != "SPD")
                            continue;
                        if (j == 1 && (stat != "CR" && stat != "CD"))
                            continue;
                        if (j == 2 && (stat != "ACC" && stat != "RES"))
                            continue;

                        li = new ListViewItem(stat);
                        // put the right type on it
                        li.Name = stat + (stat == "SPD" ? "flat" : "perc");
                        li.Text = stat;
                        li.Tag = stat + (stat == "SPD" ? "flat" : "perc");
                        li.Group = lv.Groups[1];
                        
                        lv.Items.Add(li);
                    }
					 
                }
            }

            Control textBox;
            Label label;

            // make a grid for the monsters base, min/max stats and the scoring
            
            int x;
            int y;

            y = 94;

            int colWidth = 50;
            int rowHeight = 24;

            string[] comb = Build.statNames.Concat(Build.extraNames).ToArray();

            int genX = 0;

			int dlCheckX = 0;
			int dlBtnX = 0;

            #region Statbox
            foreach (var stat in comb)
            {
                x = 4;
                groupBox1.Controls.MakeControl<Label>(stat, "Label", x, y, 50, 20, stat);
                x += colWidth;

                groupBox1.Controls.MakeControl<Label>(stat, "Base", x, y, 50, 20, stat);
				dlCheckX = x;
                x += colWidth;

                groupBox1.Controls.MakeControl<Label>(stat, "Bonus", x, y, 50, 20, stat);
                x += colWidth;

                textBox = groupBox1.Controls.MakeControl<TextBox>(stat, "Total", x, y, 40, 20);
                textBox.TextChanged += global_TextChanged;
                textBox.TextChanged += Total_TextChanged;
				dlBtnX = x;
                x += colWidth;



                groupBox1.Controls.MakeControl<Label>(stat, "Current", x, y, 50, 20, stat);
                x += colWidth;

                genX = x;

                textBox = groupBox1.Controls.MakeControl<TextBox>(stat, "Worth", x, y, 40, 20);
                textBox.TextChanged += global_TextChanged;
                x += colWidth;

                groupBox1.Controls.MakeControl<Label>(stat, "CurrentPts", x, y, (int)(50 * 0.8), 20, stat);
                x += (int)(colWidth * 0.8);

                textBox = groupBox1.Controls.MakeControl<TextBox>(stat, "Thresh", x, y, 40, 20);
                textBox.TextChanged += global_TextChanged;
                x += colWidth;

                textBox = groupBox1.Controls.MakeControl<TextBox>(stat, "Max", x, y, 40, 20);
                textBox.TextChanged += global_TextChanged;

                y += rowHeight;
            }
            #endregion

            testBuildButton.Location = new Point(genX, y);

			btnDL6star.Location = new Point(dlBtnX, y);
			checkDL6star.Location = new Point(dlCheckX, y + 2);

			btnDLawake.Location = new Point(dlBtnX, y + rowHeight);
			checkDLawake.Location = new Point(dlCheckX, y + rowHeight + 2);

			// put the grid on all the tabs
			foreach (var tab in tabNames)
            {
                TabPage page = tabControl1.TabPages["tab" + tab];
                page.Tag = tab;

                page.Controls.MakeControl<Label>(tab, "divprompt", 6, 6, 140, 14, "Divide stats into points");
                page.Controls.MakeControl<Label>(tab, "inhprompt", 60, 14, 134, 6, "Inherited");
                page.Controls.MakeControl<Label>(tab, "curprompt", 60, 14, 214, 6, "Current");

                ComboBox filterJoin = new ComboBox();
                filterJoin.DropDownStyle = ComboBoxStyle.DropDownList;
                filterJoin.FormattingEnabled = true;
                filterJoin.Items.AddRange(new object[] {"Or", "And", "Sum"});
                filterJoin.Location = new Point(298, 6);
                filterJoin.Name = tab + "join";
                filterJoin.Size = new Size(72, 21);
                filterJoin.SelectedIndex = 0;

                filterJoin.SelectionChangeCommitted += filterJoin_SelectedIndexChanged;
                filterJoin.Tag = tab;
                page.Controls.Add(filterJoin);

                bool first = true;

                rowHeight = 25;
                colWidth = 42;

                int predX = 0;
                y = 45;
                foreach (var stat in Build.statNames)
                {
                    page.Controls.MakeControl<Label>(tab, stat, 5, y, 30, 20, stat);

                    x = 35;
                    foreach (var pref in new string[] { "", "i", "c" })
                    {
                        foreach (var type in new string[] { "flat", "perc" })
                        {
                            if (first)
                            {
                                label = page.Controls.MakeControl<Label>(tab + pref, type, x, 25, 45, 16, pref + type);
                                if (type == "flat")
                                    label.Text = "Flat";
                                if (type == "perc")
                                    label.Text = "Percent";
                            }
                            
                            if (type == "perc" && stat == "SPD")
                            {
                                x += colWidth;
                                continue;
                            }
                            if (type == "flat" && (stat == "ACC" || stat == "RES" || stat == "CD" || stat == "CR"))
                            {
                                x += colWidth;
                                continue;
                            }

                            if (pref == "")
                            {
                                textBox = page.Controls.MakeControl<TextBox>(tab + pref + stat, type, x, y-2);
                                textBox.TextChanged += global_TextChanged;
                            }
                            else
                                page.Controls.MakeControl<Label>(tab + pref + stat, type, x, y);

                            x += colWidth;
                        }
                    }

                    predX = x;

                    page.Controls.MakeControl<Label>(tab + stat, "gt", x, y, 30, 20, ">=");
                    x += colWidth;

                    textBox = page.Controls.MakeControl<TextBox>(tab + stat, "test", x, y-2);
                    textBox.TextChanged += global_TextChanged;
                    x += colWidth;

                    page.Controls.MakeControl<Label>(tab + "r", stat + "test", x, y, 30, 20, ">=");
                    x += colWidth;

                    y += rowHeight;
                    first = false;
                }

                y += 8;
                x = predX;

                page.Controls.MakeControl<Label>(tab, "testGt", x - 3, y, 45, 20, "Sum >=");
                x += colWidth;

                textBox = page.Controls.MakeControl<TextBox>(tab, "test", x, y - 2);
                textBox.TextChanged += global_TextChanged;
                // default scoring is OR which doesn't need this box
                textBox.Enabled = false;
                x += colWidth;

                page.Controls.MakeControl<Label>(tab, "Check", x, y, 60, 14);

                x = predX;
                y += rowHeight + 8;

                page.Controls.MakeControl<Label>(tab, "raiseLabel", x, y, text: "Make+");
                x += colWidth;

                textBox = page.Controls.MakeControl<TextBox>(tab, "raise", x, y - 2);
                textBox.TextChanged += global_TextChanged;
                x += colWidth;

                page.Controls.MakeControl<Label>(tab, "raiseInherit", x, y, text: "0");
                x += colWidth;

                x = predX;
                y += rowHeight;

                CheckBox check = page.Controls.MakeControl<CheckBox>(tab, "bonus", x, y, 17, 17);
                check.Checked = false;
                check.CheckedChanged += global_CheckChanged;
                x += 17;

                label = page.Controls.MakeControl<Label>(tab, "bonusLabel", x, y, 67, 20, "Predict Subs");
                label.Click += (s,e) => { check.Checked = !check.Checked; };
                x += colWidth;

                x += colWidth - 17;

                page.Controls.MakeControl<Label>(tab, "bonusInherit", x, y, 80, 20, "FT");
                x += colWidth;
            }

        }
        
        private TabPage GetTab(string tabName)
        {
            // figure out which tab it's on
            int tabId;
            if (!int.TryParse(tabName, out tabId))
            {
                if (tabName == "g" || tabName == "Global")
                    tabId = 0;
                else if (tabName == "e" || tabName == "Even")
                    tabId = -2;
                else if (tabName == "o" || tabName == "Odd")
                    tabId = -1;
                else if (tabName == "One")
                    tabId = 1;
                else if (tabName == "Two")
                    tabId = 2;
                else if (tabName == "Three")
                    tabId = 3;
                else if (tabName == "Four")
                    tabId = 4;
                else if (tabName == "Five")
                    tabId = 5;
                else if (tabName == "Six")
                    tabId = 6;
                else
                    throw new Exception();
            }
            //= tabName == "g" ? 0 : tabName == "e" ? -2 : tabName == "o" ? -1 : int.Parse(tabName);

            TabPage tab = null;
            if (tabId <= 0)
                tab = tabControl1.TabPages[-tabId];
            if (tabId > 0)
            {
                if (tabId % 2 == 0)
                    tab = tabControl1.TabPages[2 + tabId / 2];
                else
                    tab = tabControl1.TabPages[6 + tabId / 2];
            }
            return tab;
        }

        // when the scoring type changes
        private void filterJoin_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            string tabName = (string)box.Tag;
            TabPage tab = GetTab(tabName);
            Control ctrl;

            foreach (var stat in Build.statNames)
            {
                foreach (var type in new string[] { "flat", "perc" })
                {
                    if (type == "perc" && stat == "SPD")
                        continue;
                    if (type == "flat" && (stat == "ACC" || stat == "RES" || stat == "CD" || stat == "CR"))
                        continue;
                    ctrl = tab.Controls.Find(tabName + stat + type, false).FirstOrDefault();
                    //ctrl.Enabled = (box.SelectedIndex == 2);
                }
                ctrl = tab.Controls.Find(tabName + stat + "test", false).FirstOrDefault();
                if (ctrl != null) ctrl.Enabled = (box.SelectedIndex != 2);
                ctrl = tab.Controls.Find(tabName + "test", false).FirstOrDefault();
                if (ctrl != null) ctrl.Enabled = (box.SelectedIndex == 2);
            }

            double? test = null;
            double temp;
            ctrl = tab.Controls.Find(tabName + "test", false).FirstOrDefault();
            if (double.TryParse(ctrl?.Text, out temp))
                test = temp;

            if (!build.runeScoring.ContainsKey(ExtensionMethods.GetIndex(tabName)))
                build.runeScoring.Add(ExtensionMethods.GetIndex(tabName), new KeyValuePair<int, double?>(box.SelectedIndex, test));
            build.runeScoring[ExtensionMethods.GetIndex(tabName)] = new KeyValuePair<int, double?>(box.SelectedIndex, test);

            // TODO: trim the ZERO nodes on the tree

            // retest the rune
            TestRune(runeTest);
        }

        // When the window is told to appear (hopefully we have everything)
        void Create_Shown(object sender, EventArgs e)
        {
            if (build == null)
            {
                // don't have. :(
                Close();
                return;
            }
            // warning, now loading
            loading = true;

            Monster mon = build.mon;
            mon.Current.Leader = build.leader;
            Stats cur = mon.GetStats();
            if (mon.ID != -1)
                monLabel.Text = "Build for " + mon.Name + " (" + mon.ID + ")";
            else
                monLabel.Text = "Build for " + build.MonName + " (" + mon.ID + ")";

            build.VERSIONNUM = VERSIONNUM;

			checkDL6star.Checked = build.DownloadStats;
			checkDLawake.Checked = build.DownloadAwake;
			checkDL6star.Enabled = !checkDLawake.Checked;

			check_autoRunes.Checked = build.autoRuneSelect;

            if (build.leader.NonZero())
            {
                Attr t = build.leader.FirstNonZero();
                if (t != Attr.Null)
                {
                    leaderTypeBox.SelectedIndex = leadTypes.ToList().FindIndex(lt => lt.type == t);
                }
            }

            // move the sets around in the list a little
            foreach (RuneSet s in build.BuildSets)
            {
                ListViewItem li = setList.Items.Find(s.ToString(), true).FirstOrDefault();
                //if (li == null)
                //    li.Group = rsExc;
                if (li != null)
                    li.Group = rsInc;
            }
            foreach (RuneSet s in build.RequiredSets)
            {
                ListViewItem li = setList.Items.Find(s.ToString(), true).FirstOrDefault();
                if (li != null)
                    li.Group = rsReq;
				int num = build.RequiredSets.Count(r => r == s);
				if (num > 1)
					li.Text = s.ToString() + " x" + num;
				
            }

            // for 2,4,6 - make sure that the Attrs are set up
            var lists = new ListView[]{ priStat2, priStat4, priStat6};
            for (int j = 0; j < lists.Length; j++)
            {
                var lv = lists[j];

				var attrs = Enum.GetValues(typeof(Attr));
				for (int i = 0; i < Build.statNames.Length; i++)
                {
                    var bl = build.slotStats[(j+1)*2 - 1];
					
                    string stat = Build.statNames[i];
                    if (i < 3)
                    {
                        if (bl.Contains(stat + "flat"))
                            lv.Items.Find(stat + "flat", true).FirstOrDefault().Group = lv.Groups[0];

                        if (bl.Contains(stat + "perc") || build.New)
                            lv.Items.Find(stat + "perc", true).FirstOrDefault().Group = lv.Groups[0];
                    }
                    else
                    {
                        if (j == 0 && stat != "SPD")
                            continue;
                        if (j == 1 && (stat != "CR" && stat != "CD"))
                            continue;
                        if (j == 2 && (stat != "ACC" && stat != "RES"))
                            continue;

                        if (bl.Contains(stat + (stat == "SPD" ? "flat" : "perc")) || build.New)
                            lv.Items.Find(stat + (stat == "SPD" ? "flat" : "perc"), true).FirstOrDefault().Group = lv.Groups[0];
                    }
                }
            }

			refreshStats(mon, cur);

            // do we allow broken sets?
            ChangeBroken(build.AllowBroken);

            // for each tabs filter
            foreach (var rs in build.runeFilters)
            {
                var tab = rs.Key;
                TabPage ctab = GetTab(tab.ToString());
                // for each stats filter
                foreach (var f in rs.Value)
                {
                    var stat = f.Key;
                    // for each stat type
                    foreach (var type in new string[] { "flat", "perc", "test" })
                    {
                        if (type == "perc" && stat == "SPD")
                            continue;
                        if (type == "flat" && (stat == "CR" || stat == "CD" || stat == "RES" || stat == "ACC"))
                            continue;

                        // find the controls and shove the value in it
                        var ctrl = ctab.Controls.Find(ctab.Tag + stat + type, true).FirstOrDefault();
                        double? val = f.Value[type];
                        // unless it's zero, I don't want zeros
                        if (val != null)
                            ctrl.Text = val.Value.ToString("0.##");
                        else
                            ctrl.Text = "";
                    }
                }
            }
            foreach (var tab in build.runePrediction)
            {
                TabPage ctab = GetTab(tab.Key.ToString());
                TextBox tb = (TextBox)ctab.Controls.Find(ctab.Tag + "raise", true).FirstOrDefault();
                if (tab.Value.Key != 0)
                    tb.Text = tab.Value.Key.ToString();
                CheckBox cb = (CheckBox)ctab.Controls.Find(ctab.Tag + "bonus", true).FirstOrDefault();
                cb.Checked = tab.Value.Value;
            }

            // for each tabs scoring
            foreach (var tab in build.runeScoring)
            {
                TabPage ctab = GetTab(tab.Key.ToString());
                // find that box
                ComboBox box = (ComboBox)ctab.Controls.Find(ctab.Tag + "join", true).FirstOrDefault();
                // manually kajigger it
                box.SelectedIndex = tab.Value.Key;
                foreach (var stat in Build.statNames)
                {
                    Control ctrl;
                    foreach (var type in new string[] { "flat", "perc" })
                    {
                        if (type == "perc" && stat == "SPD")
                            continue;
                        if (type == "flat" && (stat == "ACC" || stat == "RES" || stat == "CD" || stat == "CR"))
                            continue;
                        ctrl = ctab.Controls.Find(ctab.Tag + stat + type, true).FirstOrDefault();
                    }
                    ctrl = ctab.Controls.Find(ctab.Tag + stat + "test", true).FirstOrDefault();
                    ctrl.Enabled = (box.SelectedIndex != 2);
                    ctrl = ctab.Controls.Find(ctab.Tag + "test", true).FirstOrDefault();
                    ctrl.Enabled = (box.SelectedIndex == 2);
                }

                var tb = (TextBox)ctab.Controls.Find(ctab.Tag + "test", true).FirstOrDefault();
                if (tab.Value.Value != 0)
                    tb.Text = tab.Value.Value.ToString();
            }
            // done loading!
            loading = false;
            // this build is no longer considered new
            build.New = false;
            // oh yeah, update everything now that it's finally loaded
            UpdateGlobal();
            CalcPerms();
        }

		private void refreshStats(Monster mon, Stats cur)
		{
            statName.Text = mon.Name;
			statID.Text = mon.ID.ToString();
			statLevel.Text = mon.level.ToString();

			// read a bunch of numbers
			foreach (var stat in Build.statNames)
			{
				var ctrlBase = (Label)groupBox1.Controls.Find(stat + "Base", true).FirstOrDefault();
				ctrlBase.Text = mon[stat].ToString();

				var ctrlBonus = (Label)groupBox1.Controls.Find(stat + "Bonus", true).FirstOrDefault();
				var ctrlTotal = (TextBox)groupBox1.Controls.Find(stat + "Total", true).FirstOrDefault();

                ctrlTotal.Tag = new KeyValuePair<Label, Label>(ctrlBase, ctrlBonus);

				var ctrlCurrent = groupBox1.Controls.Find(stat + "Current", true).FirstOrDefault();
                ctrlCurrent.Text = cur[stat].ToString();

				var ctrlWorth = groupBox1.Controls.Find(stat + "Worth", true).FirstOrDefault();

				var ctrlThresh = groupBox1.Controls.Find(stat + "Thresh", true).FirstOrDefault();

				var ctrlMax = groupBox1.Controls.Find(stat + "Max", true).FirstOrDefault();

				if (build.Minimum[stat] > 0)
					ctrlTotal.Text = build.Minimum[stat].ToString();
				if (build.Sort[stat] != 0)
					ctrlWorth.Text = build.Sort[stat].ToString();
				if (build.Maximum[stat] != 0)
					ctrlMax.Text = build.Maximum[stat].ToString();
				if (build.Threshold[stat] != 0)
					ctrlThresh.Text = build.Threshold[stat].ToString();

			}

			foreach (var extra in Build.extraNames)
			{
				var ctrlBase = (Label)groupBox1.Controls.Find(extra + "Base", true).FirstOrDefault();
				ctrlBase.Text = mon.ExtraValue(extra).ToString();

				var ctrlBonus = (Label)groupBox1.Controls.Find(extra + "Bonus", true).FirstOrDefault();
				var ctrlTotal = (TextBox)groupBox1.Controls.Find(extra + "Total", true).FirstOrDefault();

                ctrlTotal.Tag = new KeyValuePair<Label, Label>(ctrlBase, ctrlBonus);

				var ctrlCurrent = groupBox1.Controls.Find(extra + "Current", true).FirstOrDefault();
                ctrlCurrent.Text = cur.ExtraValue(extra).ToString();

				var ctrlWorth = groupBox1.Controls.Find(extra + "Worth", true).FirstOrDefault();

				var ctrlThresh = groupBox1.Controls.Find(extra + "Thresh", true).FirstOrDefault();

				var ctrlMax = groupBox1.Controls.Find(extra + "Max", true).FirstOrDefault();

				if (build.Minimum.ExtraGet(extra) > 0)
					ctrlTotal.Text = build.Minimum.ExtraGet(extra).ToString();
				if (build.Sort.ExtraGet(extra) != 0)
					ctrlWorth.Text = build.Sort.ExtraGet(extra).ToString();
				if (build.Maximum.ExtraGet(extra) != 0)
					ctrlMax.Text = build.Maximum.ExtraGet(extra).ToString();
				if (build.Threshold.ExtraGet(extra) != 0)
					ctrlThresh.Text = build.Threshold.ExtraGet(extra).ToString();
			}

		}

		// switch the cool icon on the button (and the bool in the build)
		private void ChangeBroken(bool state)
        {
            toolStripButton2.Tag = state;
            toolStripButton2.Image = state ? App.broken : App.whole;
            if (build != null)
                build.AllowBroken = state;
        }

        // The minimum value was changed
        void Total_TextChanged(object sender, EventArgs e)
        {
            var total = (TextBox)sender;

            if (total.Tag == null)
                return;

            // pull the BASE and BONUS controls from the tag
            var kv = (KeyValuePair<Label, Label>)total.Tag;
            var bonus = kv.Value;
            var mBase = kv.Key;

            // did someone put a k in?
            if (total.Text.ToLower().Contains("k"))
            {
                total.Text = (double.Parse(total.Text.ToLower().Replace("k", "")) * 1000).ToString();
            }

            // calculate the difference between base and minimum, put it in bonus
            var hasPercent = mBase.Text.Contains("%");
            double val;
			if (double.TryParse(total.Text.Replace("%", ""), out val))
			{
                double sub;
                double.TryParse(mBase.Text.Replace("%", ""), out sub);
				val -= sub;
			}
            if (val < 0)
                val = 0;

            bonus.Text = "+" + (val) + (hasPercent ? "%" : "");
        }

        void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (build == null)
                return;

            var list = (ListView)sender;

            var items = list.SelectedItems;
            if (items.Count == 0)
                return;

            ListViewItem item = items[0];

            ChangeMove(build.BuildSets.Contains((RuneSet)item.Tag));
            ChangeReq(build.RequiredSets.Contains((RuneSet)item.Tag));
        }

        // flip the icon for the INCLUDE set button
        void ChangeMove(bool dir)
        {
            toolStripButton1.Tag = dir;
            if (!dir)
            {
                toolStripButton1.Image = App.add;
                toolStripButton1.Text = "Include selected set";
            }
            else
            {
                toolStripButton1.Image = App.subtract;
                toolStripButton1.Text = "Exclude selected set";
            }
            updatePerms();
        }

        // flip the icon for the REQUIRED set button
        void ChangeReq(bool dir)
        {
            toolStripButton3.Tag = dir;
            if (!dir)
            {
                toolStripButton3.Image = App.up;
                toolStripButton3.Text = "Require selected set";
            }
            else
            {
                toolStripButton3.Image = App.down;
                toolStripButton3.Text = "Option selected set";
            }
            updatePerms();

        }

        void toolStripButton2_Click(object sender, EventArgs e)
        {
            ChangeBroken(!(bool)((ToolStripButton)sender).Tag);
        }

        // shuffle runesets between: included <-> excluded
        void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (build == null)
                return;

            var list = setList;

            var items = list.SelectedItems;
            if (items.Count == 0)
                return;

            var selGrp = items[0].Group;
            var indGrp = selGrp.Items.IndexOf(items[0]);

            // shift all the selected things to the place
            foreach (ListViewItem item in items)
            {
                var set = (RuneSet)item.Tag;
                if (build.BuildSets.Contains(set))
                {
                    build.BuildSets.Remove(set);
                    item.Group = rsExc;
                }
                else
                {
                    build.BuildSets.Add(set);
                    item.Group = rsInc;
                }
            }

            var ind = items[0].Index;
            
            // TODO: maybe try cool reselecting the next entry
            if (selGrp.Items.Count > 0)
            {
                int i = indGrp;
                if (i > 0 && (i == selGrp.Items.Count || !(bool)selGrp.Tag))
                    i -= 1;
                while (selGrp.Items[i].Selected)
                {
                    i++;
                    i %= selGrp.Items.Count;
                    if (i == indGrp)
                        break;

                }

                ind = selGrp.Items[i].Index;
            }
            setList.SelectedIndices.Clear();
            
            setList.SelectedIndices.Add(ind);
            listView1_SelectedIndexChanged(setList, null);
        }

        // shuffle rnuesets between: excluded > required <-> included
        void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (build == null)
                return;

            var list = setList;

            var items = list.SelectedItems;
            if (items.Count == 0)
                return;

            var selGrp = items[0].Group;
            var indGrp = selGrp.Items.IndexOf(items[0]);

            foreach (ListViewItem item in items)
            {
                var set = (RuneSet)item.Tag;
                if (build.RequiredSets.Contains(set))
                {
                    build.RequiredSets.RemoveAll(s => s == set);
                    if (!build.BuildSets.Contains(set))
                        build.BuildSets.Add(set);
                    item.Group = rsInc;
					item.Text = set.ToString();
                }
                else
                {
                    build.RequiredSets.Add(set);
                    if (!build.BuildSets.Contains(set))
                        build.BuildSets.Add(set);
                    item.Group = rsReq;
                }
            }

            var ind = items[0].Index;

            if (selGrp.Items.Count > 0)
            {
                int i = indGrp;
				int selcount = 0;
                if (i > 0 && (i == selGrp.Items.Count || !(bool)selGrp.Tag))
                    i -= 1;
                while (selGrp.Items[i].Selected)
                {
					selcount++;
                    i++;
                    i %= selGrp.Items.Count;
					if (selcount == selGrp.Items.Count)
						break;
                }

                ind = selGrp.Items[i].Index;
            }
            setList.SelectedIndices.Clear();

            setList.SelectedIndices.Add(ind);
            listView1_SelectedIndexChanged(setList, null);
        }

        // if you click on a little rune
        void runeControl_Click(object sender, EventArgs e)
        {
            // reset all the gammas
            foreach (RuneControl t in runes)
            {
                t.Gamma = 1;
                t.Refresh();
            }

            RuneControl tc = ((RuneControl)sender);
            
            // darken? wut
            tc.Gamma = 1.4f;
            // redraw that
            tc.Refresh();

            // good idea, generate right now whenever the user clicks a... whatever
            build.GenRunes(Main.data, false, Main.useEquipped);

            using (RuneSelect rs = new RuneSelect())
            {
                rs.returnedRune = build.mon.Current.Runes[(int)tc.Tag - 1];
                rs.build = build;
                rs.slot = ((int)tc.Tag).ToString();
                rs.runes = build.runes[(int)tc.Tag-1];
                rs.ShowDialog();
            }

        }

        void UpdateGlobal()
        {
            // if the window is loading, try not to save the window
            if (loading)
                return;

            foreach (string tab in tabNames)
            {
                SlotIndex tabdex = ExtensionMethods.GetIndex(tab);
                foreach (string stat in Build.statNames)
                {
                    UpdateStat(tab, stat);
                }
                if (!build.runeScoring.ContainsKey(tabdex) && build.runeFilters.ContainsKey(tabdex))
                {
                    // if there is a non-zero
                    if (build.runeFilters[tabdex].Any(r => r.Value.NonZero))
                    {
                        build.runeScoring.Add(tabdex, new KeyValuePair<int, double?>(0, null));
                    }
                }
                if (build.runeScoring.ContainsKey(tabdex))
                {
                    var kv = build.runeScoring[tabdex];
                    var ctrlTest = Controls.Find(tab + "test", true).FirstOrDefault();

                    //if (!string.IsNullOrWhiteSpace(ctrlTest?.Text))
                    double? testVal = null;
                    double tempval;
                    if (double.TryParse(ctrlTest?.Text, out tempval))
                        testVal = tempval;

                    build.runeScoring[tabdex] = new KeyValuePair<int, double?>(kv.Key, testVal);
                    
                }
                TextBox tb = (TextBox)Controls.Find(tab + "raise", true).FirstOrDefault();
                int raiseLevel;
                if (!int.TryParse(tb.Text, out raiseLevel))
                    raiseLevel = -1;
                CheckBox cb = (CheckBox)Controls.Find(tab + "bonus", true).FirstOrDefault();

                if (raiseLevel == -1 && !cb.Checked)
                {
                    build.runePrediction.Remove(tabdex);
                }
                else
                    build.runePrediction[tabdex] = new KeyValuePair<int?, bool>(raiseLevel, cb.Checked);

                if (build.runePrediction.ContainsKey(tabdex))
                {
                    var kv = build.runePrediction[tabdex];
                    var ctrlRaise = Controls.Find(tab + "raiseInherit", true).FirstOrDefault();
                    ctrlRaise.Text = kv.Key.ToString();
                    var ctrlPred = Controls.Find(tab + "bonusInherit", true).FirstOrDefault();
                    ctrlPred.Text = (kv.Value ? "T" : "F");

                }

            }
            foreach (string stat in Build.statNames)
            {
                double val;
                double total;
                var ctrlTotal = groupBox1.Controls.Find(stat + "Total", true).FirstOrDefault();
                if (double.TryParse(ctrlTotal.Text, out val))
                    total = val;
                build.Minimum[stat] = val;

                var ctrlWorth = groupBox1.Controls.Find(stat + "Worth", true).FirstOrDefault();
                double worth = 0;
                if (double.TryParse(ctrlWorth.Text, out val))
                    worth = val;
                build.Sort[stat] = val;

                var ctrlMax = groupBox1.Controls.Find(stat + "Max", true).FirstOrDefault();
                double max = 0;
                if (double.TryParse(ctrlMax.Text, out val))
                    max = val;
                build.Maximum[stat] = val;

                var ctrlThresh = groupBox1.Controls.Find(stat + "Thresh", true).FirstOrDefault();
                double thr = 0;
                if (double.TryParse(ctrlThresh.Text, out val))
                    thr = val;
                build.Threshold[stat] = val;

                var ctrlCurrent = groupBox1.Controls.Find(stat + "Current", true).FirstOrDefault();
                double current = 0;
                if (double.TryParse(ctrlCurrent.Text, out val))
                    current = val;
                var ctrlWorthPts = groupBox1.Controls.Find(stat + "CurrentPts", true).FirstOrDefault();
                if (worth != 0 && current != 0)
                {
                    double pts = current;
                    if (max != 0)
                        pts = Math.Min(max, current);
                    ctrlWorthPts.Text = (pts / worth).ToString("0.##");
                }
            }

            foreach (string extra in Build.extraNames)
            {
                var ctrlTotal = groupBox1.Controls.Find(extra + "Total", true).FirstOrDefault();
                double val;
                double total = 0;
                if (double.TryParse(ctrlTotal.Text, out val))
                    total = val;
                build.Minimum.ExtraSet(extra, val);
                var ctrlWorth = groupBox1.Controls.Find(extra + "Worth", true).FirstOrDefault();
                double worth = 0;
                if (double.TryParse(ctrlWorth.Text, out val))
                    worth = val;
                build.Sort.ExtraSet(extra, val);

                var ctrlMax = groupBox1.Controls.Find(extra + "Max", true).FirstOrDefault();
                double max = 0;
                if (double.TryParse(ctrlMax.Text, out val))
                    max = val;
                build.Maximum.ExtraSet(extra, val);

                var ctrlThresh = groupBox1.Controls.Find(extra + "Thresh", true).FirstOrDefault();
                double thr = 0;
                if (double.TryParse(ctrlThresh.Text, out val))
                    thr = val;
                build.Threshold.ExtraSet(extra, val);

                var ctrlCurrent = groupBox1.Controls.Find(extra + "Current", true).FirstOrDefault();
                double current = 0;
                if (double.TryParse(ctrlCurrent.Text, out val))
                    current = val;
                var ctrlWorthPts = groupBox1.Controls.Find(extra + "CurrentPts", true).FirstOrDefault();
                if (worth != 0 && current != 0)
                {
                    double pts = current;
                    if (max != 0)
                        pts = Math.Min(max, current);
                    ctrlWorthPts.Text = (pts / worth).ToString("0.##");
                }
                else
                {
                    ctrlWorthPts.Text = "";
                }
            }

            var lists = new ListView[]{ priStat2, priStat4, priStat6};
            for (int j = 0; j < lists.Length; j++)
            {
                var lv = lists[j];
                var bl = build.slotStats[(j + 1) * 2 - 1];
                bl.Clear();

                for (int i = 0; i < Build.statNames.Length; i++)
                {
                    string stat = Build.statNames[i];
                    if (i < 3)
                    {
                        if (lv.Items.Find(stat + "flat", true).FirstOrDefault().Group == lv.Groups[0])
                            bl.Add(stat + "flat");
                        if (lv.Items.Find(stat + "perc", true).FirstOrDefault().Group == lv.Groups[0])
                            bl.Add(stat + "perc");

                    }
                    else
                    {
                        if (j == 0 && stat != "SPD")
                            continue;
                        if (j == 1 && (stat != "CR" && stat != "CD"))
                            continue;
                        if (j == 2 && (stat != "ACC" && stat != "RES"))
                            continue;

                        if (lv.Items.Find(stat + (stat == "SPD" ? "flat" : "perc"), true).FirstOrDefault().Group == lv.Groups[0])
                            bl.Add(stat + (stat == "SPD" ? "flat" : "perc"));
                    }
                }
            }

            TestRune(runeTest);

			updatePerms();

            if (testWindow != null && !testWindow.IsDisposed)
                testWindow.textBox_TextChanged(null, null);
		}

        void UpdateStat(string tab, string stat)
        {
            SlotIndex tabdex = ExtensionMethods.GetIndex(tab);
            TabPage ctab = GetTab(tab);
            var ctest = ctab.Controls.Find(tab + stat + "test", true).First();
            double tt;
            double? test = 0;
            double.TryParse(ctest.Text, out tt);
            test = tt;
            if (ctest.Text.Length == 0)
                test = null;

            if (!build.runeFilters.ContainsKey(tabdex))
            {
                build.runeFilters.Add(tabdex, new Dictionary<string, RuneFilter>());
            }
            var fd = build.runeFilters[tabdex];
            if (!fd.ContainsKey(stat))
            {
                fd.Add(stat, new RuneFilter());
            }
            var fi = fd[stat];
            
            foreach (string type in new string[] { "flat", "perc" })
            {
                if (type == "perc" && stat == "SPD")
                {
                    continue;
                }
                if (type == "flat" && (stat == "ACC" || stat == "RES" || stat == "CD" || stat == "CR"))
                {
                    continue;
                }

                if (tab == "g")
                    ctab.Controls.Find(tab + "i" + stat + type, true).First().Text = "";
                else if (tab == "e" || tab == "o")
                {
                    ctab.Controls.Find(tab + "i" + stat + type, true).First().Text = tabg.Controls.Find("gc" + stat + type, true).First().Text;
                }
                else
                {
                    int s = int.Parse(tab);
                    if (s % 2 == 0)
                        ctab.Controls.Find(tab + "i" + stat + type, true).First().Text = tabe.Controls.Find("ec" + stat + type, true).First().Text;
                    else
                        ctab.Controls.Find(tab + "i" + stat + type, true).First().Text = tabo.Controls.Find("oc" + stat + type, true).First().Text;
                }

                var c = ctab.Controls.Find(tab + "c" + stat + type, true).First();

                double i = 0;
                double t = 0;
                var ip = double.TryParse(ctab.Controls.Find(tab + "i" + stat + type, true).First().Text, out i);
                var tp = double.TryParse(ctab.Controls.Find(tab + stat + type, true).First().Text, out t);

                if (ip)
                {
                    c.Text = tp ? t.ToString() : i.ToString();
                }
                else
                {
                    c.Text = tp ? t.ToString() : "";
                }
                
                fi[type] = t;
                
                if (ctab.Controls.Find(tab + stat + type, true).First().Text.Length == 0)
                {
                    fi[type] = null;
                }

            }

            fi.Test = test;
        }

        void TestRune(Rune rune)
        {
            if (rune == null)
                return;

            // consider moving to the slot tab for the rune
            foreach (var tab in tabNames)
            {
                TestRuneTab(rune, tab);
            }
        }

        double DivCtrl(double val, string tab, string stat, string type)
        {
            var ctrls = Controls.Find(tab + "c" + stat + type, true);
            if (ctrls.Length == 0)
                return 0;

            var ctrl = ctrls[0];
            double num;
            if (double.TryParse(ctrl.Text, out num))
            {
                if (num == 0)
                    return 0;

                return val / num;
            }
            return 0;
        }

        bool GetPts(Rune rune, string tab, string stat, ref double points, int fake, bool pred)
        {
            double pts = 0;

            PropertyInfo[] props = typeof(Rune).GetProperties();
            foreach (var prop in props)
            {
                
            }

            pts += DivCtrl(rune[stat + "flat", fake, pred], tab, stat, "flat");
            pts += DivCtrl(rune[stat + "perc", fake, pred], tab, stat, "perc");
            points += pts;

            var lCtrls = Controls.Find(tab + "r" + stat + "test", true);
            var tbCtrls = Controls.Find(tab + stat + "test", true);
            if (lCtrls.Length != 0 && tbCtrls.Length != 0)
            {
                var tLab = (Label)lCtrls[0];
                var tBox = (TextBox)tbCtrls[0];
                tLab.Text = pts.ToString();
                tLab.ForeColor = Color.Black;
                double vs = 1;
                if (double.TryParse(tBox.Text, out vs))
                {
                    if (pts >= vs)
                    {
                        tLab.ForeColor = Color.Green;
                        return true;
                    }
                    tLab.ForeColor = Color.Red;
                    return false;
                }
                return true;
            }

            return false;
        }

        void TestRuneTab(Rune rune, string tab)
        {
            bool res = false;
            SlotIndex tabdex = ExtensionMethods.GetIndex(tab);
            if (!build.runeScoring.ContainsKey(tabdex))
                return;

            int? fake = 0;
            bool pred = false;
            if (build.runePrediction.ContainsKey(tabdex))
            {
                fake = build.runePrediction[tabdex].Key;
                pred = build.runePrediction[tabdex].Value;
            }

            var kv = build.runeScoring[tabdex];
            int scoring = kv.Key;
            if (scoring == 1)
                res = true;

            double points = 0;
            foreach (var stat in Build.statNames)
            {
                bool s = GetPts(rune, tab, stat, ref points, fake ?? 0, pred);
                if (scoring == 1)
                    res &= s;
                else if (scoring == 0)
                    res |= s;
            }

            var ctrl = Controls.Find(tab + "Check", true).FirstOrDefault();

            if (ctrl != null)
                ctrl.Text = res.ToString();

            if (scoring == 2)
            {
                ctrl.Text = points.ToString("#.##");
                ctrl.ForeColor = Color.Red;
                if (points >= build.runeScoring[tabdex].Value)
                    ctrl.ForeColor = Color.Green;
            }
        }

        void button1_Click(object sender, EventArgs e)
        {
            build.GenRunes(Main.data, false, true);
            using (var ff = new RuneSelect())
            {
                ff.returnedRune = runeTest;
                ff.build = build;

                switch (tabControl1.SelectedTab.Text)
                {
                    case "Evens":
                        ff.runes = Main.data.Runes.Where(r => r.Slot % 2 == 0);
                        List<Rune> fr = new List<Rune>();
                        fr.AddRange(ff.runes.Where(r => r.Slot == 2 && build.slotStats[1].Contains(r.MainType.ToForms())));
                        fr.AddRange(ff.runes.Where(r => r.Slot == 4 && build.slotStats[3].Contains(r.MainType.ToForms())));
                        fr.AddRange(ff.runes.Where(r => r.Slot == 6 && build.slotStats[5].Contains(r.MainType.ToForms())));
                        ff.runes = fr;
                        break;
                    case "Odds":
                        ff.runes = Main.data.Runes.Where(r => r.Slot % 2 == 1);
                        break;
                    case "Global":
                        ff.runes = Main.data.Runes;
                        break;
                    default:
                        int slot = int.Parse(tabControl1.SelectedTab.Text);
                        ff.runes = Main.data.Runes.Where(r => r.Slot == slot);
                        break;
                }

                ff.slot = tabControl1.SelectedTab.Text;

                ff.runes = ff.runes.Where(r => build.BuildSets.Contains(r.Set));

                var res = ff.ShowDialog();
                if (res == DialogResult.OK)
                {
                    Rune rune = ff.returnedRune;
                    if (rune != null)
                    {
                        runeTest = rune;
                        TestRune(rune);
                    }
                }
            }
        }

        void button2_Click(object sender, EventArgs e)
        {
            if (AnnoyUser()) return;
            DialogResult = DialogResult.OK;
            Close();
        }

        void global_TextChanged(object sender, EventArgs e)
        {
            TextBox text = (TextBox)sender;

            if (text.Text.ToLower().Contains("k"))
                text.Text = (double.Parse(text.Text.ToLower().Replace("k", "")) * 1000).ToString();

            UpdateGlobal();
        }

        // Returns if to abort that operation
        bool AnnoyUser()
        {
            if (build == null)
                return true;
            
            foreach (var tbf in build.runeFilters)
            {
                if (build.runeScoring.ContainsKey(tbf.Key))
                {
                    int and = build.runeScoring[tbf.Key].Key;
                    double sum = 0;

                    foreach (var rbf in tbf.Value)
                    {
                        if (rbf.Value.Flat.HasValue)
                            sum += rbf.Value.Flat.Value;
                        if (rbf.Value.Percent.HasValue)
                            sum += rbf.Value.Percent.Value;
                        if (and != 2)
                        {
                            if (rbf.Value.Test == 0)
                            {
                                if (rbf.Value.Flat > 0 || rbf.Value.Percent > 0)
                                {
                                    if (tabControl1.TabPages.ContainsKey("tab" + tbf.Key))
                                    {
                                        var tab = tabControl1.TabPages["tab" + tbf.Key];
                                        tabControl1.SelectTab(tab);
                                        var ctrl = tab.Controls.Find(tbf.Key + rbf.Key + "test", false).FirstOrDefault();
                                        tooltipBadRuneFilter.IsBalloon = true;
                                        tooltipBadRuneFilter.Show(string.Empty, ctrl);
                                        tooltipBadRuneFilter.Show("GEQ how much?", ctrl, 0);
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                if (rbf.Value.Flat + rbf.Value.Percent == 0)
                                {
                                    var tab = tabControl1.TabPages["tab" + tbf.Key];
                                    tabControl1.SelectTab(tab);
                                    var ctrl = tab.Controls.Find(tbf.Key + rbf.Key, false).FirstOrDefault();
                                    tooltipBadRuneFilter.IsBalloon = true;
                                    tooltipBadRuneFilter.Show(string.Empty, ctrl);
                                    tooltipBadRuneFilter.Show("Counts for what?", ctrl, 0);
                                    return true;
                                }
                            }
                        }
                    }
                    if (and == 2 && sum > 0 && build.runeScoring[tbf.Key].Value == 0)
                    {
                        if (tabControl1.TabPages.ContainsKey("tab" + tbf.Key))
                        {
                            var tab = tabControl1.TabPages["tab" + tbf.Key];
                            tabControl1.SelectTab(tab);
                            var ctrl = tab.Controls.Find(tbf.Key + "test", false).FirstOrDefault();
                            tooltipBadRuneFilter.IsBalloon = true;
                            tooltipBadRuneFilter.Show(string.Empty, ctrl);
                            tooltipBadRuneFilter.Show("GEQ how much?", ctrl, 0);
                            return true;
                        }
                    }
                }
            }

            if (!build.Sort.NonZero())
            {
                var ctrl = groupBox1.Controls.Find("SPDWorth", false).FirstOrDefault();
                if (ctrl != null)
                {
                    tooltipNoSorting.IsBalloon = true;
                    tooltipNoSorting.Show(string.Empty, ctrl);
                    tooltipNoSorting.Show("Enter a value somewhere, please.\nLike 1 or 3.14", ctrl, 0);
                    return true;
                }
            }

            if (build.BuildSets == null && build.RequiredSets == null)
            {
                tooltipSets.IsBalloon = true;
                tooltipSets.Show(string.Empty, setList);
                tooltipSets.Show("No sets", setList, 0);
                return true;
            }
            if ((build.BuildSets?.Count ?? 0) + (build.RequiredSets?.Count ?? 0) == 0)
            {
                tooltipSets.IsBalloon = true;
                tooltipSets.Show(string.Empty, setList);
                tooltipSets.Show("No sets", setList, 0);
                return true;
            }

            if (CalcPerms() != 0) return false;

            tooltipBadRuneFilter.IsBalloon = true;
            tooltipBadRuneFilter.Show(string.Empty, runeNums);
            tooltipBadRuneFilter.Show("No builds\nAdd more sets, lax your rune filters, or unlock runes.", runeNums, 0);
            return true;
        }

        void global_CheckChanged(object sender, EventArgs e)
        {
            UpdateGlobal();
        }

        void testBuildClick(object sender, EventArgs e)
        {
            if (build == null)
                return;

            if (AnnoyUser()) return;

            if (MessageBox.Show("This will generate builds", "Continue?", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                // make a new form that generates builds into it
                // have a weight panel near the stats
                // sort live on change
                // copy weights back to here

#if true
                var ff = new Generate(build);
                var res = ff.ShowDialog();
                if (res == DialogResult.OK)
                {
                    loading = true;
                    foreach (var stat in Build.statNames)
                    {
                        var ctrlWorth = groupBox1.Controls.Find(stat + "Worth", true).FirstOrDefault();
                        if (ctrlWorth == null)
                            continue;

                        ctrlWorth.Text = build.Sort[stat] > 0 ? build.Sort[stat].ToString() : "";
                    }
					foreach (var extra in Build.extraNames)
                    {
						var ctrlWorth = groupBox1.Controls.Find(extra + "Worth", true).FirstOrDefault();
                        if (ctrlWorth == null)
                            continue;

                        ctrlWorth.Text = build.Sort.ExtraGet(extra) > 0 ? build.Sort.ExtraGet(extra).ToString() : "";
                    }
					loading = false;
                }
                else
                {
                    foreach (var stat in Build.statNames)
                    {
                        var ctrlWorth = groupBox1.Controls.Find(stat + "Worth", true).FirstOrDefault();
                        if (ctrlWorth == null)
                            continue;

                        int val;
                        int.TryParse(ctrlWorth.Text, out val);
                        build.Sort[stat] = val;
                    }

					foreach (var extra in Build.extraNames)
                    {
						var ctrlWorth = groupBox1.Controls.Find(extra + "Worth", true).FirstOrDefault();
                        if (ctrlWorth == null)
                            continue;

                        int val;
						int.TryParse(ctrlWorth.Text, out val);
						build.Sort.ExtraSet(extra, val);
                    }

                }
                UpdateGlobal();
#else
                if (testWindow == null || testWindow.IsDisposed)
                {
                    testWindow = new GenerateLive(build);
                    testWindow.Owner = this;
                }
                if (!testWindow.Visible)
                    testWindow.Show();
                testWindow.Location = new Point(Location.X + Width, Location.Y);
#endif
            }
        }

        void listView2_DoubleClick(object sender, EventArgs e)
        {
            ListView lv = (ListView)sender;

            if (lv.SelectedItems.Count > 0)
            {
                ListViewItem li = lv.SelectedItems[0];

                if (li.Group == lv.Groups[0])
                    li.Group = lv.Groups[1];
                else
                    li.Group = lv.Groups[0];

                UpdateGlobal();
            }
        }

        void button4_Click(object sender, EventArgs e)
        {
            CalcPerms();
        }

        void updatePerms()
        {
            if (btnPerms.Visible)
                btnPerms.Enabled = true;
            else
                CalcPerms();
        }

        long CalcPerms()
        {
            // good idea, generate right now whenever the user clicks a... whatever
            build.GenRunes(Main.data, false, Main.useEquipped);

            // figure stuff out
            long perms = 0;
            Label ctrl;
            for (int i = 0; i < 6; i++)
            {
                int num = build.runes[i].Length;

                if (i == 0)
                    perms = num;
                else
                    perms *= num;
                if (num == 0)
                    perms = 0;

                ctrl = (Label)Controls.Find("runeNum" + (i + 1).ToString(), true).FirstOrDefault();
                if (ctrl == null) continue;
                ctrl.Text = num.ToString();
                ctrl.ForeColor = Color.Black;

                // arbitrary colours for goodness/badness
                if (num < 12)
                    ctrl.ForeColor = Color.Green;
                if (num > 24)
                    ctrl.ForeColor = Color.Orange;
                if (num > 32)
                    ctrl.ForeColor = Color.Red;
            }
            ctrl = (Label)Controls.Find("runeNums", true).FirstOrDefault();
            if (ctrl != null)
            {
                ctrl.Text = String.Format("{0:#,##0}", perms);
                ctrl.ForeColor = Color.Black;

                // arbitrary colours for goodness/badness
                if (perms < 2*1000*1000) // 2m (0.5s)
                    ctrl.ForeColor = Color.Green;
                if (perms > 10*1000*1000) // 10m (~2s)
                    ctrl.ForeColor = Color.Orange;
                if (perms > 50*1000*1000 || perms == 0) // 50m (>10s)
                    ctrl.ForeColor = Color.Red;
            }

            btnPerms.Enabled = false;

            return perms;
        }

        void leaderTypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            leaderAmountBox.Items.Clear();
            if (leaderTypeBox.SelectedIndex == 0)
            {
                leaderAmountBox.Enabled = false;
                build.leader.SetZero();
            }
            else
            {
                LeaderType lt = (LeaderType)leaderTypeBox.SelectedItem;
                foreach (var o in lt.values)
                    leaderAmountBox.Items.Add(o);
                leaderAmountBox.Enabled = true;
                if (loading)
                    leaderAmountBox.SelectedIndex = lt.values.FindIndex(l => l.value == build.leader.Sum());
                else
                    leaderAmountBox.SelectedIndex = 0;
            }
        }

        void leaderAmountBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loading)
                return;
            build.leader.SetZero();
            var lv = (LeaderType.LeaderValue)leaderAmountBox.SelectedItem;
            switch (lv.type)
            {
                case Attr.SpeedPercent:
                    build.leader.Speed = lv.value; 
                    break;
                case Attr.HealthPercent:
                    build.leader.Health = lv.value;
                    break;
                case Attr.AttackPercent:
                    build.leader.Attack = lv.value;
                    break;
            }
            var mon = build.mon;
            mon.Current.Leader = build.leader;
            refreshStats(mon, mon.GetStats());
        }

        void btnHelp_Click(object sender, EventArgs e)
        {
            if (Main.help != null)
                Main.help.Close();

            Main.help = new Help();
            Main.help.url = Environment.CurrentDirectory + "\\User Manual\\build.html";
            Main.help.Show();
        }

        void button4_Click_1(object sender, EventArgs e)
		{
			btnDL6star.Enabled = false;
			var mref = MonsterStat.FindMon(build.mon);
			if (mref != null)
			{
				var mstat = mref.Download();
				var newmon = mstat.GetMon(build.mon);
				build.mon = newmon;
				refreshStats(newmon, newmon.GetStats());
			}
			btnDL6star.Enabled = true;
		}

        void button5_Click_1(object sender, EventArgs e)
		{
			btnDLawake.Enabled = false;
			var mref = MonsterStat.FindMon(build.mon);
			if (mref != null)
			{
				var mstat = mref.Download();
				if (!mstat.Awakened && mstat.AwakenRef != null)
				{
					mstat = mstat.AwakenRef.Download();
					var newmon = mstat.GetMon(build.mon);
					build.mon = newmon;
					refreshStats(newmon, newmon.GetStats());
				}
			}
			btnDLawake.Enabled = true;
		}

        void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			if (loading) return;
			build.DownloadStats = checkDL6star.Checked;
		}

        void checkBox2_CheckedChanged(object sender, EventArgs e)
		{
			if (loading) return;
			build.DownloadAwake = checkDLawake.Checked;
			checkDL6star.Enabled = !checkDLawake.Checked;
		}

        void listView1_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
				toolStripButton4_Click(sender, e);
		}

        void toolStripButton4_Click(object sender, EventArgs e)
		{
			if (setList.SelectedItems.Count == 1)
			{
				var li = setList.SelectedItems[0];
				RuneSet set = (RuneSet)li.Tag;
				// whatever, just add it to required if not
				if (li.Group != rsReq)
				{
					li.Group = rsReq;
					build.RequiredSets.Add(set);
					if (!build.BuildSets.Contains(set))
						build.BuildSets.Add(set);
				}
				else
				{
					build.RequiredSets.Add(set);
					int num = build.RequiredSets.Count(s => s == set);
					if (num > 3)
					{
						build.RequiredSets.RemoveAll(s => s == set);
						build.RequiredSets.Add(set);
					}
					if (num > 1)
						li.Text = set.ToString() + " x" + num;
					if (num > 3 || num <= 1)
						li.Text = set.ToString();
				}
			}
		}

        void monLabel_Click(object sender, EventArgs e)
        {
            using (MonSelect ms = new MonSelect(build.mon))
            {
                if (ms.ShowDialog() == DialogResult.OK)
                {
                    build.mon = ms.retMon;
                    monLabel.Text = "Build for " + build.mon.Name + " (" + build.mon.ID + ")";
                    refreshStats(build.mon, build.mon.GetStats());
                }
            }
        }

        void check_magic_CheckedChanged(object sender, EventArgs e)
		{
			btnPerms.Visible = check_autoRunes.Checked;
			build.autoRuneSelect = check_autoRunes.Checked;
			updatePerms();
		}

        void check_autoBuild_CheckedChanged(object sender, EventArgs e)
        {
            MessageBox.Show("NYI");
           // sampletext
        }

        private void Create_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (testWindow != null && !testWindow.IsDisposed)
                testWindow.Close();
        }
    }
}
