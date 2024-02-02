using TwentyFiftyMaterialsCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Drawing;
using System.Text;
using TwentyFiftyMaterialsCore.Models;


namespace TwentyFiftyMaterialsUI
{
    public abstract class ConnectorBindings
    {

        public ConnectorBindings()
        {

        }

        public static readonly string AddOnName = "";

        public MainWindow TFMDialog { get; set; }

        public ProjectModel CurrentProject { get; set; }

        public void InitialiseUI(MainWindow dialog)
        {
            TFMDialog = dialog;

            SetUICultureInfo();
            SetCurrentProject();
            SetProjectName();
        }

        public void SetUICultureInfo()
        {
            List<string> projectCultureInfo = GetProjectCultureInfo();
            int[] size = { int.Parse(projectCultureInfo[2]) };
            MainWindow.ProjectCultureInfo.NumberFormat.NumberDecimalSeparator = projectCultureInfo[0];
            MainWindow.ProjectCultureInfo.NumberFormat.NumberGroupSeparator = projectCultureInfo[1];
            MainWindow.ProjectCultureInfo.NumberFormat.NumberGroupSizes = size;
        }

        internal void SetProjectName()
        {

        }

        public void ClickCloseYes_Unwrapped()
        {
            TFMDialog.DialogResult = false; // this determines whether we close directly; set to Cancel to avoid repeating the Request
            TFMDialog.Close();
        }

        public void ClickCloseNo_Unwrapped()
        {
            TFMDialog.DialogResult = true; // this determines whether we close directly; set to Cancel to avoid repeating the Request
            TFMDialog.Close();
        }


        #region abstract methods
        public abstract void SetCurrentProject();
        public abstract void ReadProjectInfoParameters();
        public abstract void UpdateModifiedElements();
        
        /// <summary>
        /// Form closed event handler
        /// </summary>
        public abstract void DisposeExecutor();
        /// <summary>
        /// Gets the current host application name.
        /// </summary>
        /// <returns></returns>
        public abstract string GetHostAppName();

        /// <summary>
        /// Gets the current opened/focused file's name.
        /// Make sure to check regarding unsaved/temporary files.
        /// </summary>
        /// <returns></returns>
        public abstract string GetFileName();

        /// <summary>
        /// Gets the current opened/focused file's id. 
        /// Generate one in here if the host app does not provide one.
        /// </summary>
        /// <returns></returns>
        public abstract string GetDocumentId();

        public abstract string GetProjectNameFromApplication();

        public abstract List<string> GetProjectCultureInfo();

        /// <summary>
        /// Gets the current opened/focused file's locations.
        /// Make sure to check regarding unsaved/temporary files.
        /// </summary>
        /// <returns></returns>
        public abstract string GetDocumentLocation();

        /// <summary>
        /// Gets the current opened/focused file's view, if applicable.
        /// </summary>
        /// <returns></returns>
        public abstract string GetActiveViewName();

        /// <summary>
        /// Gets a list of objects in the currently active view
        /// </summary>
        /// <returns></returns>
        public abstract List<string> GetObjectsInView();
        #endregion
    }
}
