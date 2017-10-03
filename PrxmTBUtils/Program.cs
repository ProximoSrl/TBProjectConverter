using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TBProjectConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: dotnet run -- path\\to\\Application.Config");
                return;
            }

            var pathToAppConfig = args[0];

            if (!pathToAppConfig.ToLowerInvariant().Contains("application.config"))
            {
                pathToAppConfig = Path.Combine(pathToAppConfig, "Application.Config");
            }

            if (!File.Exists(pathToAppConfig))
            {
                Console.WriteLine("Application.Config File not found!");
                return;
            }

            ProcessFolder(Path.GetDirectoryName(pathToAppConfig));
        }

        static void ProcessFolder(string pathToFolder)
        {
            Console.WriteLine("Starting");
            var projects = Directory.GetFiles(pathToFolder, "*.vcxproj", SearchOption.AllDirectories);
            var updater = new ProjectUpdater();

            Parallel.ForEach
            (
                projects, 
                new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount },
                project =>
                {
                    // backup
                    var backupFile = project + ".bak";
                    if (File.Exists(backupFile))
                    {
                        File.Delete(backupFile);
                    }

                    File.Copy(project, backupFile);

                    // process
                    var upgraded = updater.ConvertFromFile(project);
                    File.WriteAllText(project, upgraded);
                    Console.WriteLine($"Upgraded {project}");
                }
            );
        }
    }
}
