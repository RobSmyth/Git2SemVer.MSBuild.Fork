using Microsoft.CodeAnalysis.CSharp.Scripting;
using NoeticTools.Common;
using NoeticTools.Git2SemVer.MSBuild.IntegrationTests.Framework;


namespace NoeticTools.Git2SemVer.MSBuild.IntegrationTests;

public class BasicScriptingTests
{
    private StringWriter _errorWriter = null!;
    private StringWriter _outputWriter = null!;

    [Test]
    public async Task EvaluateTest1()
    {
        var result = await CSharpScript.EvaluateAsync("1 + 2");
        Assert.That(result, Is.EqualTo(3));
    }

    [Test]
    public async Task RunEmptyScriptReturnsNullTest()
    {
        var state = await CSharpScript.RunAsync("");

        Assert.That(state.ReturnValue, Is.Null);
        Assert.That(_outputWriter.ToString(), Is.EqualTo(""));
        Assert.That(_errorWriter.ToString(), Is.EqualTo(""));
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
        var script = GetType().Assembly.GetResourceFileContent("Script01.csx");

        var state = await CSharpScript.RunAsync(script);

        Assert.That(state.ReturnValue, Is.EqualTo(42));
        Assert.That($"Hello World{Environment.NewLine}", Is.EqualTo(_outputWriter.ToString()));
        Assert.That(_errorWriter.ToString(), Is.EqualTo(""));
    }

    [SetUp]
    public void SetUp()
    {
        _outputWriter = new StringWriter();
        Console.SetOut(_outputWriter);
        _errorWriter = new StringWriter();
        Console.SetError(_errorWriter);
    }

    [TearDown]
    public void TearDown()
    {
        _outputWriter.Dispose();
        _errorWriter.Dispose();
    }
}