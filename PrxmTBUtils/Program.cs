using System;
using Microsoft.Extensions.CommandLineUtils;

namespace TBProjectConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "TBProjectConverter",
                Description = "TaskBuilder command line utils"
            };
            app.HelpOption("-?|-h|--help");

            var recursive = app.Option
            (
                "-r|--recursive",
                "Recursive folder scan",
                CommandOptionType.NoValue
            );

            var backup = app.Option
            (
                "-b|--backup",
                "Backup original files",
                CommandOptionType.SingleValue
            );

            app.OnExecute(() =>
            {
                if (recursive.HasValue())
                {
                    // todo
                }

                if (backup.HasValue())
                {
                    // todo
                }

                return 0;
            });

            app.Execute(args);
        }
    }
}
