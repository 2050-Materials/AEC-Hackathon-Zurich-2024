using Autodesk.Revit.UI;
using TwentyFiftyMaterialsRevit.RevitUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TwentyFiftyMaterialsRevit
{
    public class ExternalEventHandler : IExternalEventHandler
    {
        public ConnectorBindingsRevit RevitBindings { get; set; }
        public bool Running = false;

        public ExternalEventHandler(ConnectorBindingsRevit revitBindings)
        {
            RevitBindings = revitBindings;
        }

        public string GetName()
        {
            return "ConnectorRevit";
        }

        public void Execute(UIApplication app)
        {
            Debug.WriteLine($"Current queue length is: {RevitBindings.Queue.Count}");

            if (Running)
            { 
                // queue will run itself through
                return; 
            } 
            Running = true;

            CheckIfQueueIsCyclic();




            Debug.WriteLine($"Current method name is: {RevitBindings.Queue[0].Method.Name}");
            try
            {
                RevitBindings.Queue[0]();
            }
            catch (Exception e)
            {
                string exception = e.Message;
                string innerException = e.InnerException == null ? "" : e.InnerException.Message;
                string stackTrace = e.StackTrace;
                TaskDialog messageDialog = new TaskDialog("Error");
                messageDialog.MainIcon = TaskDialogIcon.TaskDialogIconError;
                messageDialog.MainInstruction = "Beim Ausführen des Befehls ist ein Fehler aufgetreten";
                messageDialog.ExpandedContent = "Something went wrong at " + RevitBindings.Queue[0].Method.Name + "\n" + exception + "\n" + innerException + "\n" + stackTrace;
                messageDialog.Show();

            }

            RevitBindings.Queue.RemoveAt(0);
            Running = false;

            if (RevitBindings.Queue.Count != 0 && RevitBindings.Executor != null)
                RevitBindings.Executor.Raise();

        }

        private void CheckIfQueueIsCyclic()
        {

            if(RevitBindings.Queue.Count == 0)
            {
                return;
            }

            string firstName = RevitBindings.Queue.First().Method.Name;
            List<Action> safeQueue = new List<Action>();

            foreach (Action action in RevitBindings.Queue)
            {
                if (action.Method.Name == firstName && safeQueue.Count > 0)
                {
                    break;
                }

                safeQueue.Add(action);
            }
            RevitBindings.Queue = safeQueue;
        }
    }

}
