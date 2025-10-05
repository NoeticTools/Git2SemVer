using System.Text.RegularExpressions;
using NoeticTools.Git2SemVer.Core.ConventionCommits;


#pragma warning disable NUnit2045

namespace NoeticTools.Git2SemVer.Core.Tests.ConventionalCommits;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal class ConventionalCommitsParserTests
{
    private ConventionalCommitsParser _target;

    [TestCase(
                 """
                 Body - paragraph1

                 Body - paragraph2

                 Body - paragraph2
                 """,
                 """
                 Body - paragraph1

                 Body - paragraph2

                 Body - paragraph2
                 """,
                 new string[0],
                 false)]
    [TestCase(
                 """
                 Body - paragraph1
                 """,
                 "Body - paragraph1",
                 new string[0],
                 false)]
    [TestCase(
                 """
                 Body - paragraph1

                 BREAKING CHANGE: Oops
                 """,
                 "Body - paragraph1",
                 new[] { "BREAKING CHANGE|Oops" },
                 true)]
    [TestCase(
                 """
                 Body - paragraph1

                 BREAKING CHANGE: Oops very sorry

                 """,
                 "Body - paragraph1",
                 new[] { "BREAKING CHANGE|Oops very sorry" },
                 true)]
    [TestCase(
                 """
                 Body - paragraph1

                 BREAKING CHANGE: Oops very sorry
                 ref: 1234
                 """,
                 "Body - paragraph1",
                 new[]
                 {
                     "BREAKING CHANGE|Oops very sorry",
                     "ref|1234"
                 },
                 true)]
    [TestCase(
                 "Body - paragraph1",
                 "Body - paragraph1",
                 new string[0],
                 false)]
    public void BodyMultiLineBodyAndFooterTest(string messageBody,
                                               string expectedBody,
                                               string[] expectedTopicValues,
                                               bool hasBreakingChange)
    {
        var result = _target.Parse("feat: Added a real nice feature", messageBody);

        Assert.That(result.ChangeType, Is.EqualTo("feat"));
        Assert.That(result.ApiChangeFlags.BreakingChange, Is.EqualTo(hasBreakingChange));
        Assert.That(result.Description, Is.EqualTo("Added a real nice feature"));
        Assert.That(result.Body, Is.EqualTo(expectedBody));
        var keyValuePairs = GetExpectedFooterKeyValues(expectedTopicValues);

        Assert.That(result.FooterKeyValues, Is.EquivalentTo(keyValuePairs));
    }

    [TestCase(
                 """
                 Body - paragraph1

                 Body - paragraph2

                 Body - paragraph2
                 """,
                 """
                 Body - paragraph1

                 Body - paragraph2

                 Body - paragraph2
                 """)]
    [TestCase(
                 "Body - paragraph1",
                 "Body - paragraph1")]
    public void BodyWithFooterTest(string messageBody,
                                   string expectedBody)
    {
        var result = _target.Parse("feat: Added a real nice feature", messageBody);

        Assert.That(result.ChangeType, Is.EqualTo("feat"));
        Assert.That(result.ApiChangeFlags.BreakingChange, Is.False);
        Assert.That(result.Description, Is.EqualTo("Added a real nice feature"));
        Assert.That(result.Body, Is.EqualTo(expectedBody));
        Assert.That(result.FooterKeyValues, Is.Empty);
    }

    [TestCase(
                 """

                 BREAKING CHANGE: Oops1 very sorry
                 issue #35
                 """,
                 new[]
                 {
                     "BREAKING CHANGE|Oops1 very sorry",
                     "issue|35"
                 },
                 true)]
    [TestCase(
                 """

                 BREAKING CHANGE: Oops2 very sorry
                 refs: #12345
                 """,
                 new[]
                 {
                     "BREAKING CHANGE|Oops2 very sorry",
                     "refs|#12345"
                 },
                 true)]
    // Test extra footer topic value lines can start with <space><space>.
    // to ensure compatible with git footer features.
    [TestCase(
                 """

                 BREAKING CHANGE: Oops3
                   very sorry
                 refs: username/projectName#12345
                 """,
                 new[]
                 {
                     """
                     BREAKING CHANGE|Oops3
                       very sorry
                     """,
                     "refs|username/projectName#12345"
                 },
                 true)]
    // Test
    //  - footer topic <space># delimiter
    //  - Return after topic delimiter.
    //  - Empty line in topic value.
    [TestCase(
                 """

                 BREAKING CHANGE #
                 Oops4
                 very sorry

                 really
                 refs: username/projectName#12345
                 """,
                 new[]
                 {
                     """
                     BREAKING CHANGE|
                     Oops4
                     very sorry

                     really
                     """,
                     "refs|username/projectName#12345"
                 },
                 true)]
    public void FooterWithoutBodyTest(string messageBody,
                                      string[] expectedTopicValues,
                                      bool hasBreakingChange)
    {
        var result = _target.Parse("feat: Added a real nice feature", "\n" + messageBody);

        Assert.That(result.ChangeType, Is.EqualTo("feat"));
        Assert.That(result.ApiChangeFlags.BreakingChange, Is.EqualTo(hasBreakingChange));
        Assert.That(result.Description, Is.EqualTo("Added a real nice feature"));
        Assert.That(result.Body, Is.EqualTo(""));
        var keyValuePairs = GetExpectedFooterKeyValues(expectedTopicValues);

        Assert.That(result.FooterKeyValues, Is.EquivalentTo(keyValuePairs));
    }

    [TestCase("feat:")]
    [TestCase("feat:\n")]
    [TestCase("feat: ")]
    [TestCase("feat: \n")]
    [TestCase("feat:  ")]
    [TestCase("fix:  ")]
    [TestCase("fix!:  ")]
    [TestCase("fix(a scope):  ")]
    public void MalformedSubjectLineConventionalCommitInfoTest(string commitSubject)
    {
        var result = _target.Parse(commitSubject, "");

        Assert.That(result.ChangeType, Is.EqualTo(""));
    }

    //[TestCase("""
    //          fail body ddd

    //          dd

    //          """)]
    [TestCase("""
              body1 ddd

              dd

              token1(scope1): value1
              token2: value2
              """)]
    [TestCase("""
              body2 ddd

              dd

              token1(scope1): value1
              token2: value2
              """)]
    [TestCase("""

              token1: value1
              token2: value2
              """)]
    [TestCase("""

              token1: value1
                aaa
              token2: value2
              """)]
    public void RegexExperimentalTest(string input)
    {
        var regex = new Regex("""
                              \A
                              (
                                (?<body>\S ((\n|\r\n)|.)*? )
                                (\Z | (?: (\n|\r\n) ) )
                              )?
                              (

                                (?<footer>
                                  (
                                    (?: (\n|\r\n) )
                                    (?<token> (BREAKING\sCHANGE) | ( \w[\w-]+ ) )
                                    ( \( (?<scope>\w[\w-]+) \) )?
                                    :\s
                                    (?<value>
                                      \S.*
                                      (?:
                                        (\n|\r\n) \s\s \S.*
                                      )*
                                    )
                                  )+
                                )?

                                \Z
                              )
                              """,
                              RegexOptions.IgnorePatternWhitespace |
                              RegexOptions.Multiline);

        var match = regex.Match(input);

        var body = match.Groups["body"].Value;
        System.Console.WriteLine($"|{body}|");

        var keywords = match.Groups["token"].Captures;
        var values = match.Groups["value"].Captures;
        Assert.That(keywords[0].Value, Is.EqualTo("token1"));
        Assert.That(values[0].Value.Trim().StartsWith("value1"), Is.True);
        Assert.That(keywords[1].Value, Is.EqualTo("token2"));
        Assert.That(values[1].Value, Is.EqualTo("value2"));
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase("feat")]
    [TestCase("feat This is a commit without conventional commit info")]
    [TestCase("This is a commit without conventional commit info")]
    public void SubjectLineWithoutConventionalCommitInfoTest(string commitSubject)
    {
        var result = _target.Parse(commitSubject, "");

        Assert.That(result.ChangeType, Is.EqualTo(""));
    }

    [TestCase("feat: Added a real nice feature", "feat", "Added a real nice feature", false)]
    [TestCase("feat: Added a real nice feature (#24)", "feat", "Added a real nice feature (#24)", false)]
    [TestCase("feat!: Added a real nice feature", "feat", "Added a real nice feature", true)]
    [TestCase("feat(apples)!: Added a real nice feature", "feat", "Added a real nice feature", true)]
    [TestCase("Fix: Fixed nasty bug", "fix", "Fixed nasty bug", false)]
    [TestCase("build: Build work", "build", "Build work", false)]
    [TestCase("build(my-scope): Build work", "build", "Build work", false)]
    [TestCase("custom-type: Build work", "custom-type", "Build work", false)]
    public void SubjectWithConventionalCommitInfoTest(string messageSubject,
                                                      string expectedChangeType,
                                                      string expectedChangeDescription,
                                                      bool hasBreakingChange)
    {
        var result = _target.Parse(messageSubject, "");

        Assert.That(result.ChangeType, Is.EqualTo(expectedChangeType));
        Assert.That(result.ApiChangeFlags.BreakingChange, Is.EqualTo(hasBreakingChange));
        Assert.That(result.Description, Is.EqualTo(expectedChangeDescription));
        Assert.That(result.Body, Is.Empty);
        Assert.That(result.FooterKeyValues, Is.Empty);
    }

    [SetUp]
    public void SetUp()
    {
        _target = new ConventionalCommitsParser(new ConventionalCommitsSettings());
    }

    private static FooterKeyValues GetExpectedFooterKeyValues(string[] expectedTopicValues)
    {
        var footerKeyValues = new FooterKeyValues();
        foreach (var line in expectedTopicValues)
        {
            if (line.Length == 0)
            {
                continue;
            }

            var elements = line.Split('|');
            var key = elements[0];
            var value = elements[1];
            footerKeyValues.Add(key, value);
        }

        return footerKeyValues;
    }
}