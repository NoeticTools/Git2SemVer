﻿using NoeticTools.Common.ConventionCommits;
using System.Collections.Generic;


#pragma warning disable NUnit2045

namespace NoeticTools.CommonTests.ConventionalCommits;

internal class ConventionalCommitsParserTests
{
    private ConventionalCommitsParser _target;

    [TestCase("feat:")]
    [TestCase("feat:\n")]
    [TestCase("feat: ")]
    [TestCase("feat: \n")]
    [TestCase("feat:  ")]
    [TestCase("fix:  ")]
    [TestCase("fix!:  ")]
    [TestCase("fix(a scope):  ")]
    public void MalformedSingleLineConventionalCommitInfoTest(string commitMessage)
    {
        var result = _target.Parse(commitMessage);

        Assert.That(result.ChangeType, Is.EqualTo(CommitChangeTypeId.None));
    }

    [TestCase("")]
    [TestCase("\n\n\n")]
    [TestCase("This is a commit without conventional commit info")]
    [TestCase("Two lines\n")]
    [TestCase("Three lines\n\n")]
    public void MessageWithoutConventionalCommitInfoTest(string commitMessage)
    {
        var result = _target.Parse(commitMessage);

        Assert.That(result.ChangeType, Is.EqualTo(CommitChangeTypeId.None));
    }

    [TestCase(
                 """
                 feat: Added a real nice feature

                 Body - paragraph1

                 Body - paragraph2

                 Body - paragraph2
                 """,
                 CommitChangeTypeId.Feature,
                 "Added a real nice feature",
                 """
                 Body - paragraph1

                 Body - paragraph2

                 Body - paragraph2
                 """)]
    [TestCase(
                 """
                 feat: Added a real nice feature

                 Body - paragraph1
                 """,
                 CommitChangeTypeId.Feature,
                 "Added a real nice feature",
                 """
                 Body - paragraph1
                 """)]
    public void MultiLineWithFooterTest(string commitMessage,
                                        CommitChangeTypeId expectedChangeTypeId,
                                        string expectedChangeDescription,
                                        string expectedBody)
    {
        var result = _target.Parse(commitMessage);

        Assert.That(result.ChangeType, Is.EqualTo(expectedChangeTypeId));
        Assert.That(result.HasBreakingChange, Is.False);
        Assert.That(result.ChangeDescription, Is.EqualTo(expectedChangeDescription));
        Assert.That(result.Body, Is.EqualTo(expectedBody));
        Assert.That(result.FooterKeyValues, Is.Empty);
    }

    [TestCase(
                 """
                 feat: Added a real nice feature

                 Body - paragraph1

                 Body - paragraph2

                 Body - paragraph2
                 """,
                 "Added a real nice feature",
                 """
                 Body - paragraph1

                 Body - paragraph2

                 Body - paragraph2
                 """,
                 "",
                 false)]
    [TestCase(
                 """
                 feat: Added a real nice feature

                 Body - paragraph1
                 """,
                 "Added a real nice feature",
                 "Body - paragraph1",
                 "",
                 false)]
    [TestCase(
                 """
                 feat: Added a real nice feature

                 Body - paragraph1

                 BREAKING CHANGE: Oops
                 """,
                 "Added a real nice feature",
                 "Body - paragraph1",
                 "BREAKING CHANGE|Oops",
                 true)]
    [TestCase(
                 """
                 feat: Added a real nice feature

                 Body - paragraph1

                 BREAKING CHANGE: Oops very sorry

                 """,
                 "Added a real nice feature",
                 "Body - paragraph1",
                 "BREAKING CHANGE|Oops very sorry",
                 true)]
    [TestCase(
                 """
                 feat: Added a real nice feature

                 Body - paragraph1

                 BREAKING CHANGE: Oops very sorry
                 ref: 1234
                 """,
                 "Added a real nice feature",
                 "Body - paragraph1",
                 """
                 BREAKING CHANGE|Oops very sorry
                 ref|1234
                 """,
                 true)]
    [TestCase(
                 """
                 feat!: Added a real nice feature

                 Body - paragraph1
                 """,
                 "Added a real nice feature",
                 "Body - paragraph1",
                 "",
                 true)]
    public void MultiLineWithoutFooterTest(string commitMessage,
                                           string expectedChangeDescription,
                                           string expectedBody,
                                           string expectedFooter,
                                           bool hasBreakingChange)
    {
        var result = _target.Parse(commitMessage);

        Assert.That(result.ChangeType, Is.EqualTo(CommitChangeTypeId.Feature));
        Assert.That(result.HasBreakingChange, Is.EqualTo(hasBreakingChange));
        Assert.That(result.ChangeDescription, Is.EqualTo(expectedChangeDescription));
        Assert.That(result.Body, Is.EqualTo(expectedBody));

        var expectedFooterLines = expectedFooter.Split('\n');
        var keyValuePairs = new List<(string key, string value)>();
        foreach (var line in expectedFooterLines)
        {
            if (line.Length == 0)
            {
                continue;
            }
            var elements = line.Split('|');
            keyValuePairs.Add((key: elements[0], value: elements[1].Trim()));
        }
        Assert.That(result.FooterKeyValues, Is.EquivalentTo(keyValuePairs.ToLookup(k => k.key, v => v.value)));
    }

    [SetUp]
    public void SetUp()
    {
        _target = new ConventionalCommitsParser();
    }

    [TestCase("feat: Added a real nice feature",
                 CommitChangeTypeId.Feature,
                 "Added a real nice feature")]
    [TestCase("fix: Fixed nasty bug",
                 CommitChangeTypeId.Fix,
                 "Fixed nasty bug")]
    [TestCase("build: Build work",
                 CommitChangeTypeId.Build,
                 "Build work")]
    [TestCase("chore: Did something",
                 CommitChangeTypeId.Chore,
                 "Did something")]
    [TestCase("ci: Did something",
                 CommitChangeTypeId.ContinuousIntegration,
                 "Did something")]
    [TestCase("docs: Did something",
                 CommitChangeTypeId.Documentation,
                 "Did something")]
    [TestCase("style: Did something",
                 CommitChangeTypeId.Style,
                 "Did something")]
    [TestCase("refactor: Did something",
                 CommitChangeTypeId.Refactoring,
                 "Did something")]
    [TestCase("perf: Did something\n",
                 CommitChangeTypeId.Performance,
                 "Did something")]
    [TestCase("test: Did something\n\n",
                 CommitChangeTypeId.Testing,
                 "Did something")]
    public void SingleLineConventionalCommitInfoTest(string commitMessage,
                                                     CommitChangeTypeId expectedChangeTypeId,
                                                     string expectedChangeDescription)
    {
        var result = _target.Parse(commitMessage);

        Assert.That(result.ChangeType, Is.EqualTo(expectedChangeTypeId));
        Assert.That(result.HasBreakingChange, Is.False);
        Assert.That(result.ChangeDescription, Is.EqualTo(expectedChangeDescription));
        Assert.That(result.Body, Is.Empty);
        Assert.That(result.FooterKeyValues, Is.Empty);
    }
}