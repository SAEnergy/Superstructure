using Core.Interfaces.Components.Logging;
using Core.Logging;
using Core.Logging.LogDestinations;
using Microsoft.Build;
using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectUpdater
{
    public class Program
    {
        private static Settings _settings;
        private static ILogger _logger;
        private const string _searchPath = "*.csproj";
        private const string _globalAssemblyInfo = "GlobalAssemblyInfo.cs";
        private static int _verifyResult;

        public static int Main(string[] args)
        {
            _logger = Logger.CreateInstance();
            _logger.AddLogDestination(new ConsoleLogDestination());

            _logger.Start();

            _settings = new Settings(_logger);

            _verifyResult = 0;

            foreach (var projectFile in Directory.GetFiles(_settings.SourceFolder, _searchPath, SearchOption.AllDirectories))
            {
                UpdateProject(projectFile);
            }

            _logger.Log("Done, bye!");

            _logger.Stop();

            return _verifyResult;
        }

        private static void UpdateProject(string projectFile)
        {
            if (!string.IsNullOrEmpty(projectFile))
            {
                if (File.Exists(projectFile))
                {
                    _logger.Log(string.Format("Working on \"{0}\"", projectFile));

                    var project = new Project(projectFile);

                    var globalAssemblyResult = project.Items.Where(i => i.ItemType.Contains("Compile") && i.UnevaluatedInclude.Contains(_globalAssemblyInfo)).FirstOrDefault();

                    if(globalAssemblyResult == null)
                    {
                        if (!_settings.IsReadOnly)
                        {
                            _logger.Log(string.Format("Adding \"{0}\" link to project \"{1}\"", _globalAssemblyInfo, projectFile));

                            var kvp = new KeyValuePair<string, string>("Link", string.Concat("Properties\\", _globalAssemblyInfo));

                            var rc = project.AddItem("Compile", string.Concat(FindRelativePathFromFolder(projectFile), _globalAssemblyInfo), new[] { kvp });

                            project.Save();
                        }

                        if(_settings.Verify)
                        {
                            _logger.Log(string.Format("Project \"{0}\" did not include link to \"{1}\"", projectFile, _globalAssemblyInfo));
                            _verifyResult--;
                        }
                    }
                    else
                    {
                        _logger.Log(string.Format("Project \"{0}\" already has a link to \"{1}\"", projectFile, _globalAssemblyInfo));
                    }
                }
            }
        }

        private static string FindRelativePathFromFolder(string file)
        {
            string retVal = string.Empty;
            string folder = _settings.SourceFolder;

            int index = file.IndexOf(folder);

            if(index > -1)
            {
                var uncommonPath = file.Substring(index + folder.Length + 1);

                int count = uncommonPath.Count(c => c.Equals(Path.DirectorySeparatorChar));

                for(int i = 0; i < count; i++)
                {
                    retVal = string.Concat(retVal, "..", Path.DirectorySeparatorChar);
                }
            }
            else
            {
                throw new NotSupportedException();
            }

            return retVal;
        }
    }
}
