/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

using MSR.Models.Prediction.PostReleaseDefectFiles;

namespace MSR.Tools.Predictor
{
	public class PredictorPresenter
	{
		private IPredictorModel model;
		private IPredictorView view;
		
		public PredictorPresenter(IPredictorModel model, IPredictorView view)
		{
			this.model = model;
			this.view = view;
			model.OnTitleUpdated += x => view.Title = x;
			model.OnClearReport += () => view.ClearReport();
			model.OnAddReport += x => view.AddReport(x);
			model.OnReadyStateChanged += x => view.Ready = x;
			model.OnError += x => view.ShowError(x);
			view.OnOpenConfigFile += OpenConfigFile;
			view.OnPredict += () => Predict(false);
			view.OnPredictAndEvaluate += () => Predict(true);
		}
		public void Run()
		{
			view.Show();
		}
		private void ReadOptions()
		{
			view.SetReleaseList(model.Releases.Keys);
			view.SetModelList(model.Models.Select(x => x.Title));
			view.ClearReport();
			view.CommandMenuAvailable = true;
			view.ShowFiles = model.ShowFiles;
			view.MaxReleaseSetSize = model.MaxReleaseSetSize;
		}
		private void UpdateOptions()
		{
			List<PostReleaseDefectFilesPrediction> models = new List<PostReleaseDefectFilesPrediction>();
			foreach (var n in view.SelectedModels)
			{
				models.Add(model.Models[n]);
			}
			model.SelectedModels = models;
			
			model.SelectedReleases = model.Releases
				.Where(x => view.SelectedReleases.Contains(x.Key))
				.ToDictionary(x => x.Key, x => x.Value);
			
			model.ShowFiles = view.ShowFiles;
			model.MaxReleaseSetSize = view.MaxReleaseSetSize;
		}
		private void OpenConfigFile(string fileName)
		{
			try
			{
				model.OpenConfig(fileName);
				ReadOptions();
			}
			catch (Exception e)
			{
				view.ShowError(e.Message);
			}
		}
		private void Predict(bool evaluate)
		{
			try
			{
				UpdateOptions();
				model.Evaluate = evaluate;
				model.Predict();
			}
			catch (Exception e)
			{
				view.ShowError(e.Message);
			}
		}
	}
}