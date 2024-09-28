using System.Xml.Linq;
using NoeticTools.Git2SemVer.Tool.MSBuild.Projects.Elements;


namespace NoeticTools.Git2SemVer.Tool.MSBuild.Projects.GroupElements;

public sealed class ItemGroup : MsBuildGroupBase<ItemElement>
{
    public ItemGroup(XDocument xmlDocument) : base(xmlDocument, "ItemGroup")
    {
    }

    public void AddNuGetPackage(string name)
    {
        /*
           <ItemGroup>
             <PackageReference Include="System.Text.Json" Version="8.0.4" />
             <PackageReference Include="System.Text.Json" Version="8.0.4" />
           </ItemGroup>
         */
        // todo
    }

    public void AddProjectDependency(string versioningProjectName)
    {
        /*
         <PropertyGroup Condition   MSBuildProjectName


           <ItemGroup>
             <ProjectReference Include="..\ClassLibrary2\ClassLibrary2.csproj" />
           </ItemGroup>
         */
        throw new NotImplementedException();
    }

    protected override ItemElement CreateItem(XElement element)
    {
        return new ItemElement(element);
    }
}