using System.Collections;
using Microsoft.Build.Framework;


namespace NoeticTools.Git2SemVer.IntegrationTests.Framework;

internal class BuildEngine9Stub : IBuildEngine6
{
    private readonly IReadOnlyDictionary<string, string> _globalProperties;

    public BuildEngine9Stub(IReadOnlyDictionary<string, string> globalProperties)
    {
        _globalProperties = globalProperties;
    }

    public bool AllowFailureWithoutError { get; set; } = false;

    public int ColumnNumberOfTaskNode { get; } = 0;

    public bool ContinueOnError { get; } = false;

    public bool IsRunningMultipleNodes { get; } = false;

    public int LineNumberOfTaskNode { get; } = 0;

    public string ProjectFileOfTaskNode { get; } = "";

    public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
    {
        return false;
    }

    public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs,
                                 string toolsVersion)
    {
        return false;
    }

    public bool BuildProjectFilesInParallel(string[] projectFileNames, string[] targetNames, IDictionary[] globalProperties,
                                            IDictionary[] targetOutputsPerProject, string[] toolsVersion, bool useResultsCache,
                                            bool unloadProjectsOnCompletion)
    {
        return false;
    }

    public BuildEngineResult BuildProjectFilesInParallel(string[] projectFileNames, string[] targetNames, IDictionary[] globalProperties,
                                                         IList<string>[] removeGlobalProperties, string[] toolsVersion, bool returnTargetOutputs)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyDictionary<string, string> GetGlobalProperties()
    {
        return _globalProperties;
    }

    public object GetRegisteredTaskObject(object key, RegisteredTaskObjectLifetime lifetime)
    {
        throw new NotImplementedException();
    }

    public void LogCustomEvent(CustomBuildEventArgs e)
    {
    }

    public void LogErrorEvent(BuildErrorEventArgs e)
    {
    }

    public void LogMessageEvent(BuildMessageEventArgs e)
    {
    }

    public void LogTelemetry(string eventName, IDictionary<string, string> properties)
    {
        throw new NotImplementedException();
    }

    public void LogWarningEvent(BuildWarningEventArgs e)
    {
    }

    public void Reacquire()
    {
        throw new NotImplementedException();
    }

    public void RegisterTaskObject(object key, object obj, RegisteredTaskObjectLifetime lifetime, bool allowEarlyCollection)
    {
        throw new NotImplementedException();
    }

    public void ReleaseCores(int coresToRelease)
    {
        throw new NotImplementedException();
    }

    public int RequestCores(int requestedCores)
    {
        throw new NotImplementedException();
    }

    public bool ShouldTreatWarningAsError(string warningCode)
    {
        throw new NotImplementedException();
    }

    public object UnregisterTaskObject(object key, RegisteredTaskObjectLifetime lifetime)
    {
        throw new NotImplementedException();
    }

    public void Yield()
    {
        throw new NotImplementedException();
    }
}