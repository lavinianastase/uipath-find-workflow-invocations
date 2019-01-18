using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UiPathTeam.FindWfInvocations
{
    public class Manager
    {
        public string WorkflowName { get; set; }
        public string ProjectPath { get; set; }

        private readonly string _regexPattern;
        private readonly bool _processInParallel;

        public Manager(string workflowName, string projectPath, bool processInParallel = true)
        {
            WorkflowName = workflowName;
            ProjectPath = projectPath;

            _processInParallel = processInParallel;
            _regexPattern = @"\<{1}(ui:InvokeWorkflowFile)(?s).*\<{1}(\/ui:InvokeWorkflowFile)";
        }

        public List<string> GetListOfWorkflowInvocations()
        {
            var result = new List<string>();

            var allFiles = Directory.GetFiles(ProjectPath, "*.xaml", SearchOption.AllDirectories);

            CheckIfWorkflowExists(allFiles);

            if (_processInParallel)
            {
                result = ProcessInParallel(allFiles);
            }
            else
            {
                result = ProcessSequentially(allFiles);
            }

            return result;
        }

        public List<string> ProcessInParallel(string[] allFiles)
        {
            var result = new List<string>();

            Parallel.ForEach(allFiles, (file) =>
            {
                if (!file.Contains(WorkflowName))
                {
                    var text = File.ReadAllText(file);

                    var matches = Regex.Match(text, _regexPattern, RegexOptions.IgnoreCase);

                    if (matches.Value.Contains(WorkflowName))
                    {
                        result.Add(file);
                    }
                }
            });

            return result;
        }

        public List<string> ProcessSequentially( string[] allFiles)
        {
            var result = new List<string>();

            foreach (var file in allFiles)
            {
                if (!file.Contains(WorkflowName))
                {
                    var text = File.ReadAllText(file);

                    var matches = Regex.Match(text, _regexPattern, RegexOptions.IgnoreCase);

                    if (matches.Value.Contains(WorkflowName))
                    {
                        result.Add(file);
                    }
                }
            }

            return result;
        }

        public bool CheckIfWorkflowExists(string[] allFiles)
        {
            var workflowExistsInProject = allFiles.FirstOrDefault(s => s.Contains(WorkflowName));

            if (string.IsNullOrEmpty(workflowExistsInProject))
            {
                throw new ArgumentException("The workflow provided as parameter doesn't belong to the project. Please check the name and try again.");
            }
            return true;
        }
    }
}
