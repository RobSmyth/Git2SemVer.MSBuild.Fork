using System.Reflection;
using NoeticTools.Git2SemVer.Core.Diagnostics;


namespace NoeticTools.DiagnosticCodesDocBuilder;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Diagnostics DocFx pages builder.");

        var destinationDirectory = args[0];
        if (!Directory.Exists(destinationDirectory))
        {
            throw new ArgumentException($"Directory '{destinationDirectory}' does not exist.");
        }
        Console.WriteLine($"Generate pages in directory: {destinationDirectory}");

        var assembly = typeof(DiagnosticCodeBase).Assembly;
        var types = assembly.GetTypes()
                              .Where(t => t.GetCustomAttributes(typeof(DiagnosticCodeAttribute), false).Length > 0)
                              .ToArray();

        Console.WriteLine($"Generating {types.Length} diagnostic code pages.");

        foreach (var type in types)
        {
            var constructor = type.GetConstructors().First();

            var valueArgs = new object[constructor.GetParameters().Length]!;
            for (var index=0; index < valueArgs.Length; index++)
            {
                valueArgs[index] = "value";
            }

            var instance = (DiagnosticCodeBase)constructor.Invoke(valueArgs);
            Console.WriteLine($"  Code: {instance.Code}.");

            var filePath = Path.Combine(destinationDirectory, $"{instance.Code}.md");
            File.WriteAllText(filePath, instance.DocFxPageContents);
        }

    }


}
