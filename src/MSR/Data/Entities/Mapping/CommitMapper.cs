/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using MSR.Data.VersionControl;
using MSR.Data.Entities.DSL.Mapping;

namespace MSR.Data.Entities.Mapping
{
	public class CommitMapper : EntityMapper<RepositoryMappingExpression, CommitMappingExpression>
	{
		public CommitMapper(IScmData scmData)
			: base(scmData)
		{
		}
		public override IEnumerable<CommitMappingExpression> Map(RepositoryMappingExpression expression)
		{
			ILog log = scmData.Log(expression.Revision);
			
			return new CommitMappingExpression[]
			{
				expression.AddCommit(log.Revision)
					.By(log.Author)
					.At(log.Date)
					.WithMessage(log.Message)
			};
		}
	}
}
