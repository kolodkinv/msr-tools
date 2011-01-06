/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;
using Rhino.Mocks;

using MSR.Data.Entities.DSL.Mapping;

namespace MSR.Data.Entities.DSL.Selection
{
	[TestFixture]
	public class ProjectFileSelectionExpressionTest : BaseRepositoryTest
	{
		[Test]
		public void Should_get_files_added_in_specified_commits()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1")
			.Submit()
				.AddCommit("2")
					.File("file1")
					.AddFile("file2")
			.Submit()
				.AddCommit("3")
					.AddFile("file3").CopiedFrom("file1", "1")
			.Submit();
			
			selectionDSL
				.Commits().TillRevision("2")
				.Files().AddedInCommits()
				.Select(x => x.Path).ToArray()
					.Should().Have.SameSequenceAs(new string[] { "file1", "file2" });
			selectionDSL
				.Commits().FromRevision("3")
				.Files().AddedInCommits()
				.Select(x => x.Path).ToArray()
					.Should().Have.SameSequenceAs(new string[] { "file3" });
		}
		[Test]
		public void Should_get_files_deleted_in_specified_commits()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1")
			.Submit()
				.AddCommit("2")
					.File("file1").Delete()
					.AddFile("file2").CopiedFrom("file1", "1")
					.AddFile("file3")
			.Submit()
				.AddCommit("3")
					.File("file3").Delete()
			.Submit();
			
			selectionDSL
				.Commits().TillRevision("2")
				.Files().DeletedInCommits()
				.Select(x => x.Path).ToArray()
					.Should().Have.SameSequenceAs(new string[] { "file1" });
			selectionDSL
				.Commits().FromRevision("3")
				.Files().DeletedInCommits()
				.Select(x => x.Path).ToArray()
					.Should().Have.SameSequenceAs(new string[] { "file3" });
		}
		[Test]
		public void Should_get_files_by_name()
		{
			mappingDSL
				.AddCommit("1").At(DateTime.Today.AddDays(-9))
					.AddFile("file1").Modified()
			.Submit()
				.AddCommit("2").At(DateTime.Today.AddDays(-8))
					.AddFile("file2").Modified()
					.File("file1").Delete()
			.Submit()
				.AddCommit("3").At(DateTime.Today.AddDays(-7))
					.AddFile("file1").Modified()
			.Submit();

			selectionDSL
				.Files().PathIs("file1").Count()
					.Should().Be(2);
			selectionDSL
				.Files().PathIs("file2").Count()
					.Should().Be(1);
		}
		[Test]
		public void Should_get_existent_files()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
			.Submit()
				.AddCommit("2")
					.File("file1").Delete()
					.AddFile("file2").Modified()
			.Submit();
			
			selectionDSL
				.Files().Exist()
				.Select(f => f.Path).ToArray()
					.Should().Have.SameSequenceAs(new string[] { "file2" });
		}
		[Test]
		public void Should_get_existent_files_for_revision()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
			.Submit()
				.AddCommit("2")
					.File("file1").Delete()
					.AddFile("file2").Modified()
					.AddFile("file3").Modified()
			.Submit()
				.AddCommit("3")
					.File("file2").Delete()
					.AddFile("file4").CopiedFrom("file2", "2")
			.Submit();

			selectionDSL
				.Files().ExistInRevision("1")
				.Select(f => f.Path).ToArray()
					.Should().Have.SameSequenceAs(new string[] { "file1" });
			selectionDSL
				.Files().ExistInRevision("2")
				.Select(f => f.Path).ToArray()
					.Should().Have.SameSequenceAs(new string[] { "file2", "file3" });
			selectionDSL
				.Files().ExistInRevision("3")
				.Select(f => f.Path).ToArray()
					.Should().Have.SameSequenceAs(new string[] { "file3", "file4" });
		}
		[Test]
		public void Should_get_distinct_files()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
			.Submit();
			
			selectionDSL
				.Files().Count()
					.Should().Be(1);
		}
		[Test]
		public void Should_get_files_in_directory()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("/trunk/dir1/file1").Modified()
			.Submit()
				.AddCommit("2")
					.AddFile("/trunk/dir2changelog")
					.AddFile("/trunk/dir2/file2").Modified()
			.Submit()
				.AddCommit("3")
					.AddFile("/file3")
			.Submit();
			
			selectionDSL
				.Files().InDirectory("/trunk/dir1").Count()
					.Should().Be(1);
			selectionDSL
				.Files().InDirectory("/trunk/dir2").Count()
					.Should().Be(1);
			selectionDSL
				.Files().InDirectory("/trunk").Count()
					.Should().Be(3);
			selectionDSL
				.Files().InDirectory("/").Count()
					.Should().Be(4);
		}
	}
}
