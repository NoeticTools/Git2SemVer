using Microsoft.CodeAnalysis.CSharp.Scripting;
using NoeticTools.Git2SemVer.Core;


#pragma warning disable NUnit2045

namespace NoeticTools.Git2SemVer.Framework.IntegrationTests;

[Parallelizable(ParallelScope.Fixtures)]
public class BasicScriptingTests
{
    [Test]
    public async Task EvaluateTest1()
    {
        var result = await CSharpScript.EvaluateAsync("1 + 2");
        Assert.That(result, Is.EqualTo(3));
    }

    [Test]
    public async Task RunEmptyScriptReturnsNullTest()
    {
        using var context = new BasicScriptingTestsContext();

        var state = await CSharpScript.RunAsync("");

        Assert.That(state.ReturnValue, Is.Null);
        Assert.That(context.OutputWriter.ToString(), Is.EqualTo(""));
        Assert.That(context.ErrorWriter.ToString(), Is.EqualTo(""));
    }

    [Test]
    public void RunningScriptThatThrowsExceptionTest()
    {
        var script = GetType().Assembly.GetResourceFileContent("ExceptionScript.csx");

        Assert.CatchAsync<InvalidOperationException>(() => CSharpScript.RunAsync(script));
    }

    [Test]
    public async Task RunScriptWritesToStandardOutAndReturnsValueTest()
    {
        using var context = new BasicScriptingTestsContext();
        var script = GetType().Assembly.GetResourceFileContent("Script01.csx");

        var state = await CSharpScript.RunAsync(script);

        Assert.That(state.ReturnValue, Is.EqualTo(42));
        Assert.That($"Hello World{Environment.NewLine}", Is.EqualTo(context.OutputWriter.ToString()));
        Assert.That(context.ErrorWriter.ToString(), Is.EqualTo(""));
    }

    private sealed class BasicScriptingTestsContext : IDisposable
    {
        public void Dispose()
        {
            ErrorWriter.Dispose();
            OutputWriter.Dispose();
        }

        public StringWriter ErrorWriter { get; }

        public StringWriter OutputWriter { get; }

        public BasicScriptingTestsContext()
        {
            OutputWriter = new StringWriter();
            Console.SetOut(OutputWriter);
            ErrorWriter = new StringWriter();
            Console.SetError(ErrorWriter);
        }
    }
}