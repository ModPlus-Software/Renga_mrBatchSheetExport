using ModPlusNuke;
using Nuke.Common.Execution;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : RengaPluginBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);
}
