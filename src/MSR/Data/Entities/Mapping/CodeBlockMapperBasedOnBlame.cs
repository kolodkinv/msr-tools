/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

using MSR.Data.VersionControl;
using MSR.Data.Entities.DSL.Mapping;
using MSR.Data.Entities.DSL.Selection;
using ABCMetrics;
using System.Threading.Tasks;

namespace MSR.Data.Entities.Mapping
{
    public class CodeBlockMapperBasedOnBlame : EntityMapper<CodeBlock, ModificationMappingExpression, CodeBlockMappingExpression>
    {
        public CodeBlockMapperBasedOnBlame(IScmData scmData)
            : base(scmData)
        {
        }
        public override IEnumerable<CodeBlockMappingExpression> Map(ModificationMappingExpression expression)
        {
            List<CodeBlockMappingExpression> codeBlockExpressions = new List<CodeBlockMappingExpression>();
            string revision = expression.CurrentEntity<Commit>().Revision;
            ProjectFile file = expression.CurrentEntity<ProjectFile>();
            bool fileIsNew = (file.AddedInCommit != null) && (file.AddedInCommit.Revision == revision);
            bool fileCopied = fileIsNew && (file.SourceFile != null);
            bool fileDeleted = file.DeletedInCommit != null;
            IMetricsLanguage metricsCalculator = new MetricsLanguageC(new KeyTokensSetCPlus());

            if (fileDeleted)
            {
                codeBlockExpressions.Add(
                    expression.DeleteCode()
                );
            }
            else
            {
                IBlame blame = scmData.Blame(revision, file.Path);
                var linesByRevision = from l in blame group l.Key by l.Value;

                IBodyFile bodyFile = scmData.Show(revision, file.Path);

                if (fileCopied)
                {
                    foreach (var linesForRevision in linesByRevision)
                    {
                        double totalValueMetric = 0;
                        string totalCode = "";
                        foreach (int indexLineCode in linesForRevision)
                        {
                            var lineCode = bodyFile.ElementAt(indexLineCode - 1);
                            totalCode += lineCode + "\n";
                        }
                        totalValueMetric = metricsCalculator.CalculateMetrics(totalCode);
                        codeBlockExpressions.Add(
                            expression.Code(totalValueMetric)
                        );

                        if (linesForRevision.Key != revision)
                        {
                            codeBlockExpressions.Last().CopiedFrom(linesForRevision.Key);
                        }
                    }               
                }
                else
                {
                    var addedCode = linesByRevision.SingleOrDefault(x => x.Key == revision);
                    if (addedCode != null)
                    {
                        double totalValueMetric = 0;
                        string totalCode = "";
                        foreach (int indexLineCode in addedCode)
                        {
                            var lineCode = bodyFile.ElementAt(indexLineCode - 1);
                            totalCode += lineCode + "\n";
                        }
                        totalValueMetric = metricsCalculator.CalculateMetrics(totalCode);
                        codeBlockExpressions.Add(
                            expression.Code(totalValueMetric)
                        );
                    }

                    foreach (var existentCode in (
                        from cb in expression.Queryable<CodeBlock>()
                        join m in expression.Queryable<Modification>() on cb.ModificationID equals m.ID
                        join f in expression.Queryable<ProjectFile>() on m.FileID equals f.ID
                        join c in expression.Queryable<Commit>() on m.CommitID equals c.ID
                        let addedCodeID = cb.Size < 0 ? cb.TargetCodeBlockID : cb.ID
                        let addedCodeRevision = expression.Queryable<Commit>()
                            .Single(x => x.ID == expression.Queryable<CodeBlock>()
                                .Single(y => y.ID == addedCodeID).AddedInitiallyInCommitID
                            ).Revision
                        where
                            f.ID == file.ID
                        group cb.Size by addedCodeRevision into g
                        select new
                        {
                            Revision = g.Key,
                            CodeSize = g.Sum()
                        }
                    ))
                    {
                        var linesForRevision = linesByRevision.SingleOrDefault(x => x.Key == existentCode.Revision);

                        IBodyFile bodyFileForRevision = scmData.Show(revision, file.Path);

                        string totalCodeForRevision = "";
                        try
                        {
                            foreach (int indexLineCodeForRevision in linesForRevision)
                            {
                                var lineCode = bodyFileForRevision.ElementAt(indexLineCodeForRevision - 1);
                                totalCodeForRevision += lineCode + "\n";
                            }
                        }
                        catch { }

                        double realCodeSize = linesForRevision == null ? 0 : metricsCalculator.CalculateMetrics(totalCodeForRevision);
                        if (existentCode.CodeSize > realCodeSize)
                        {
                            codeBlockExpressions.Add(
                                expression.Code(realCodeSize - existentCode.CodeSize)
                            );
                            codeBlockExpressions.Last().ForCodeAddedInitiallyInRevision(existentCode.Revision);
                        }
                    }
                }
            }

            return codeBlockExpressions;
        }
    }
}
