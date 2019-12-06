using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using FBSEditor;
using FlatBufferData.Model;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System.Text;
using FlatBufferData.Model.Attributes;
using FlatBufferData.Build;
using System.Windows.Forms;

namespace FlatBufferData.Editor
{
    public sealed class BuildCommand
    {
        public static readonly Guid CommandSet = new Guid("04f51c64-0c0a-412c-818c-57880c441058");

        public const int FBS_ITEM_CHECK_FBS = 0x001;
        public const int FBS_ITEM_CHECK_FBS_ALL = 0x002;
        public const int FBS_ITEM_CHECK_DATA = 0x003;
        public const int FBS_ITEM_CHECK_DATA_ALL = 0x004;
        
        public const int CommandId = 0x0200;

        private readonly BuildCommandPackage package;

        private int errorCount = 0;

        public static BuildCommand Instance { get; private set; }
        private IServiceProvider ServiceProvider { get { return this.package; } }
        public static void Initialize(BuildCommandPackage package) { Instance = new BuildCommand(package); }

        private BuildCommand(BuildCommandPackage package)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));

            var commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                commandService.AddCommand(new OleMenuCommand(OnMenuClick1, OnMenuStateChanged, OnMenuQueryStatus, new CommandID(CommandSet, FBS_ITEM_CHECK_FBS)));
                commandService.AddCommand(new OleMenuCommand(OnMenuClick2, OnMenuStateChanged, OnMenuQueryStatus, new CommandID(CommandSet, FBS_ITEM_CHECK_FBS_ALL)));
                commandService.AddCommand(new OleMenuCommand(OnMenuClick3, OnMenuStateChanged, OnMenuQueryStatus, new CommandID(CommandSet, FBS_ITEM_CHECK_DATA)));
                commandService.AddCommand(new OleMenuCommand(OnMenuClick4, OnMenuStateChanged, OnMenuQueryStatus, new CommandID(CommandSet, FBS_ITEM_CHECK_DATA_ALL)));
            }
        }

        #region single fbs
        private void OnMenuClick1(object sender, EventArgs e)
        {
            var dte = ServiceProvider.GetService(typeof(DTE)) as DTE;
            if (dte != null && dte.SelectedItems.Count == 1 && dte.SelectedItems.Item(1).Name.EndsWith(Constants.ExtName))
            {
                var selectedItem = dte.SelectedItems.Item(1).ProjectItem;
                var selectedProject = selectedItem.ContainingProject;

                errorCount = 0;
                package.ClearError();

                var builder = new FBSProject(selectedProject.Name, GetAllFBSFiles(selectedProject), ErrorHandler);
                builder.Build(selectedItem.FileNames[0]);

                if (errorCount > 0)
                {
                    package.ShowError();
                }
                errorCount = 0;
            }
        }

        private void OnMenuClick2(object sender, EventArgs e)
        {
            var dte = ServiceProvider.GetService(typeof(DTE)) as DTE;
            if (dte != null && dte.SelectedItems.Count == 1 && dte.SelectedItems.Item(1).Name.EndsWith(Constants.ExtName))
            {
                var selectedItem = dte.SelectedItems.Item(1).ProjectItem;
                var selectedProject = selectedItem.ContainingProject;

                errorCount = 0;
                package.ClearError();

                var builder = new FBSProject(selectedProject.Name, GetAllFBSFiles(selectedProject), ErrorHandler);
                builder.BuildAll();

                if (errorCount > 0)
                {
                    package.ShowError();
                }
                errorCount = 0;
            }
        }

        private void OnMenuClick3(object sender, EventArgs e)
        {
            var dte = ServiceProvider.GetService(typeof(DTE)) as DTE;
            if (dte != null && dte.SelectedItems.Count == 1 && dte.SelectedItems.Item(1).Name.EndsWith(Constants.ExtName))
            {
                var selectedItem = dte.SelectedItems.Item(1).ProjectItem;
                var selectedProject = selectedItem.ContainingProject;

                errorCount = 0;
                package.ClearError();

                var builder = new FBSProject(selectedProject.Name, GetAllFBSFiles(selectedProject), ErrorHandler);
                var file = builder.Build(selectedItem.FileNames[0]);
                if (errorCount <= 0)
                {
                    if (file.RootTable != null)
                    {
                        var xls = file.RootTable.Attributes.GetAttribute<XLS>();
                        var json = file.RootTable.Attributes.GetAttribute<JsonFile>();
                        var csv = file.RootTable.Attributes.GetAttribute<CSV>();
                        if (json != null)
                        {
                            List<string> errors = new List<string>();
                            var obj = JsonUtil.ParseJson(json.filePath, file.RootTable, file.RootTable.Attributes, ErrorHandler);
                            foreach (var error in errors)
                            {
                                errorCount++;
                                package.AddError(json.filePath, json.filePath, error, 0, 0);
                            }
                        }
                        else if (csv != null)
                        {
                            CsvUtil.ParseCSV(csv.filePath, file.RootTable, file.RootTable.Attributes, ErrorHandler);
                        }
                        //new DataReaderFactory(selectedProject.Name, ErrorHandler).ReadData(file.RootTable);
                    }
                }
                else
                {
                    package.ShowError();
                    errorCount = 0;
                }
            }
        }

        private void OnMenuClick4(object sender, EventArgs e)
        {
            var dte = ServiceProvider.GetService(typeof(DTE)) as DTE;
            if (dte != null && dte.SelectedItems.Count == 1 && dte.SelectedItems.Item(1).Name.EndsWith(Constants.ExtName))
            {
                var selectedItem = dte.SelectedItems.Item(1).ProjectItem;
                var selectedProject = selectedItem.ContainingProject;

                errorCount = 0;
                package.ClearError();

                var files = new List<Model.FBSFile>();
                var builder = new FBSProject(selectedProject.Name, GetAllFBSFiles(selectedProject), ErrorHandler);

                foreach (var path in GetAllFBSFiles(selectedProject))
                {
                    files.Add(builder.Build(path));
                }

                if (errorCount <= 0)
                {
                    var factory = new DataReaderFactory(selectedProject.Name, ErrorHandler);
                    foreach (var file in files)
                    {
                        if (file.RootTable != null)
                        {
                            var xls = file.RootTable.Attributes.GetAttribute<XLS>();
                            if (xls != null)
                                factory.ReadData(file.RootTable);
                        }
                    }
                }
                else
                {
                    package.ShowError();
                    errorCount = 0;
                }
            }
        }

        private void OnMenuStateChanged(object sender, EventArgs e)
        {
        }

        private void OnMenuQueryStatus(object sender, EventArgs e)
        {
            var command = sender as OleMenuCommand;
            if (null != command)
            {
                var dte = ServiceProvider.GetService(typeof(DTE)) as DTE;
                command.Visible = dte != null && dte.SelectedItems.Count == 1 && dte.SelectedItems.Item(1).Name.EndsWith(Constants.ExtName);
            }
        }
        #endregion

        /// <summary>
        /// 获取所有FBS文件
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        private string[] GetAllFBSFiles(Project project)
        {
            var paths = new List<string>();
            var files = new List<ProjectItem>();
            var folder = new List<ProjectItem>();

            for (var i = 1; i <= project.ProjectItems.Count; i++) { folder.Add(project.ProjectItems.Item(i)); }

            while (folder.Count > 0)
            {
                var first = folder[0];
                var fileName = first.Name;
                var filePath = first.FileNames[0];
                var isFile = File.Exists(filePath) && !Directory.Exists(filePath);

                if (isFile && fileName.ToLower().EndsWith(Constants.ExtName))
                {
                    paths.Add(filePath);
                }
                else if (first.FileCount > 1)
                {
                    for (int i = 1; i <= first.ProjectItems.Count; i++)
                    {
                        folder.Add(first.ProjectItems.Item(i));
                    }
                }

                files.Add(first);
                folder.RemoveAt(0);
            }

            return paths.ToArray();
        }


        private void ErrorHandler(string projectName, string path, string text, int line, int column)
        {
            errorCount++;
            package.AddError(projectName, path, text, line, column);
        }

        private void ErrorHandler(string projectName, string path, string text, int line, int column, int begin, int count)
        {
            errorCount++;
            package.AddError(projectName, path, text, line, column);
        }
        private void ErrorHandler(string path, string text, int line, int column, int begin, int count)
        {
            errorCount++;
            package.AddError("", path, text, line, column);
        }
    }
}
