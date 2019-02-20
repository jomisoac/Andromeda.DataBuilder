using System;
using System.IO;
using Andromeda.Core.Helpers;
using Andromeda.DataBuilder.D2O;
using Andromeda.IoC;
using Andromeda.Logging;

namespace Andromeda.DataBuilder
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Andromeda data builder";
            AppConstants appConstants;
            while (!_tryParseArgs(args, out appConstants)) // todo: implements logic
                _tryParseArgs(AskInput(), out appConstants);

            var container = Startup.Configure(new ContainerBuilder(), appConstants);
            var logger = container.GetInstance<Logger>();

            try
            {
                var files = Directory.GetFiles(appConstants.D2OPath, "*.d2o");
                var elapsed = ActionTimer.Bench(() =>
                {
                    var d2OManager = new D2OManager();
                    d2OManager.Load(files);
                    d2OManager.CompileAllModels();
                });
                logger.Log(LogType.Info, $"Execution took {elapsed:g}");
            }
            catch (Exception e)
            {
                logger.Log(LogType.Fatal, e.ToString());
            }
            finally
            {
                logger.Save();
            }

            Console.WriteLine();
            Console.WriteLine("--- Press any key to exit");
            Console.ReadLine();
        }

        static bool _tryParseArgs(string[] args, out AppConstants constants)
        {
            constants = new AppConstants();
            return true;
        }

        static string[] AskInput()
        {
            var input = Console.ReadLine();
            return input.Length < 1 ? new string[0] : input.Split(' ');
        }
    }
}
