using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UiPathTeam.FindWfInvocations;

namespace FindWfInvocations.Tests
{
    [TestClass]
    public class Tests
    {
        public Manager TestManager { get; set; }

        public Tests()
        {
            var current = Directory.GetCurrentDirectory();
            var newPath = Path.GetFullPath(Path.Combine(current, @"..\..\"));
            var testsFolder = @"TestProcess\WorkflowInvocations";
            var searchedWorkflowName = "InvokedWorkflow.xaml";
            var testProcessFolder = Path.GetFullPath(Path.Combine(newPath, testsFolder));

            TestManager = new Manager(searchedWorkflowName, testProcessFolder);
        }

        [TestMethod]
        public void CheckIfWorkflowExists_ReturnsTrueForExistingWf()
        {
            var expected = true;
            var allFiles = Directory.GetFiles(TestManager.ProjectPath, "*.xaml", SearchOption.AllDirectories);
            var actual = TestManager.CheckIfWorkflowExists(allFiles);
            Assert.IsTrue(actual == expected);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckIfWorkflowExists_ThrowsExceptionForNonExistingWf()
        {
            TestManager.WorkflowName = "InvokedWorkflow1.xaml";
            var allFiles = Directory.GetFiles(TestManager.ProjectPath, "*.xaml", SearchOption.AllDirectories);
            TestManager.CheckIfWorkflowExists(allFiles);
            //TestManager.WorkflowName = "InvokedWorkflow.xaml";
        }

        [TestMethod]
        public void GetWorkflowInvocations_ReturnsCorrectFiles()
        {
            var expected = 2;
            var result = TestManager.GetListOfWorkflowInvocations();
            var actual = result.Count;
            Assert.IsTrue(expected == actual);
        }

        [TestMethod]
        public void CompareSequentialAndParallelFileProcessing()
        {
            var seqStopwatch = new Stopwatch();
            var parallelStopwatch = new Stopwatch();
            var allFiles = Directory.GetFiles(TestManager.ProjectPath, "*.xaml", SearchOption.AllDirectories);

            seqStopwatch.Start();
            TestManager.ProcessSequentially(allFiles);
            seqStopwatch.Stop();
            var seqElapsed = seqStopwatch.Elapsed;

            parallelStopwatch.Start();
            TestManager.ProcessInParallel(allFiles);
            parallelStopwatch.Stop();
            var parallelElapsed = parallelStopwatch.Elapsed;

            Assert.IsTrue(parallelElapsed < seqElapsed);
        }
    }
}
