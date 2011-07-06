/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace MSR.Data.Entities.Mapping.PathSelectors
{
	public class TakePathByExtension : SelectPathByExtension
	{
		public TakePathByExtension(string[] extensionsToTake)
			: base(extensionsToTake)
		{
		}
		protected override bool SelectMatchedPath()
		{
			return true;
		}
	}
}
