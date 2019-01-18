using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace UiPathTeam.FindWfInvocations
{
    public class FindWfInvocationsActivity : CodeActivity
    {
        [RequiredArgument, Category("Input"), Description("The name of the workflow you want to find references for."), DisplayName("Workflow Name")]
        public InArgument<string> WorkflowName { get; set; }

        [Category("Output"), Description("The list of workflow invocations in the process."), DisplayName("Workflow Invocations")]
        public OutArgument<List<string>> WorkflowInvocations { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var workflowName = WorkflowName.Get(context);

            var projectPath = Directory.GetCurrentDirectory();

            var manager = new Manager(workflowName, projectPath);

            var result = manager.GetListOfWorkflowInvocations();

            WorkflowInvocations.Set(context, result);
        }
    }
}
