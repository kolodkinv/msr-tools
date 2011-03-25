﻿/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MSR.Tools.Visualizer
{
	public interface IVisualizerView
	{
		event Action<string> OnOpenConfigFile;
		event Action<int> OnVisualizationActivate;
		event Action<bool> OnChengeCleanUpOption;
		
		void Show();
		void ShowError(string text);
		void SetVisualizationList(IEnumerable<string> visualizations);
		bool ConfigureVisualization(IVisualization visualization);

		string Title { get; set; }
		string Status { get; set; }
		IGraphView Graph { get; }
		bool AutomaticallyCleanUp { get; set; }
	}
	
	public partial class VisualizerView : Form, IVisualizerView
	{
		private List<Color> differentColors = new List<Color>()
		{
			Color.Black,
			Color.Blue,
			Color.Red,
			Color.Green,
			Color.Brown,
			Color.Gray,
			Color.Lime,
			Color.Orange,
			Color.Pink,
			Color.Purple,
			Color.Silver
		};

		public event Action<string> OnOpenConfigFile;
		public event Action<int> OnVisualizationActivate;
		public event Action<bool> OnChengeCleanUpOption;
		
		public VisualizerView()
		{
			InitializeComponent();

			Graph = new GraphView(panel1);
		}
		public new void Show()
		{
			Application.Run(this);
		}
		public void ShowError(string text)
		{
			MessageBox.Show(text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		public void SetVisualizationList(IEnumerable<string> visualizations)
		{
			visualizationsToolStripMenuItem.DropDownItems.Clear();
			int i = 0;
			foreach (var v in visualizations)
			{
				var menuItem = visualizationsToolStripMenuItem.DropDownItems.Add(v);
				menuItem.Tag = i;
				menuItem.Click += (s,e) =>
				{
					OnVisualizationActivate((int)(s as ToolStripMenuItem).Tag);
				};
				i++;
			}
		}
		public bool ConfigureVisualization(IVisualization visualization)
		{
			var config = new VisualizationConfigView();
			return config.ShowDialog(visualization);
		}
		public string Title
		{
			get { return Text; }
			set { Text = value; }
		}
		public string Status
		{
			get { return statusText.Text; }
			set { statusText.Text = value; }
		}
		public IGraphView Graph
		{
			get; private set;
		}
		public bool AutomaticallyCleanUp
		{
			get { return automaticallyToolStripMenuItem.Checked; }
			set { automaticallyToolStripMenuItem.Checked = value; }
		}

		private void openConfigToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				OnOpenConfigFile(dialog.FileName);
			}
		}

		private void logXToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var item = (sender as ToolStripMenuItem);
			item.Checked = ! item.Checked;
			Graph.XAxisLogScale = item.Checked;
		}
		private void logYToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var item = (sender as ToolStripMenuItem);
			item.Checked = ! item.Checked;
			Graph.YAxisLogScale = item.Checked;
		}
		private void automaticallyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var item = (sender as ToolStripMenuItem);
			item.Checked = ! item.Checked;
			OnChengeCleanUpOption(item.Checked);
		}
		private void cleanUpNowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Graph.CleanUp();
		}
	}
}
