using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using FBSEditor;

namespace FlatBufferData.Editor
{
    internal sealed class BuildCommand
    {
        public const int CommandId = 0x0200;
        public static readonly Guid CommandSet = new Guid("04f51c64-0c0a-412c-818c-57880c441058");

        private readonly BuildCommandPackage package;

        private int errorCount = 0;

        public static BuildCommand Instance
        {
            get;
            private set;
        }
        private IServiceProvider ServiceProvider { get { return this.package; } }

        public static void Initialize(BuildCommandPackage package)
        {
            Instance = new BuildCommand(package);
        }

        private BuildCommand(BuildCommandPackage package)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));

            var commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                commandService.AddCommand(new OleMenuCommand(OnMenuClick, OnMenuStateChanged, OnMenuQueryStatus, new CommandID(CommandSet, CommandId)));
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

        private void OnMenuClick(object sender, EventArgs e)
        {
            var dte = ServiceProvider.GetService(typeof(DTE)) as DTE;
            if(dte != null && dte.SelectedItems.Count == 1 && dte.SelectedItems.Item(1).Name.EndsWith(Constants.ExtName))
            {
                var a = dte.SelectedItems.Item(1);
                var b = a.ProjectItem;
                var c = a.Project;
                BuildProject(dte.SelectedItems.Item(1).ProjectItem.ContainingProject);

                /*
                var selected = dte.SelectedItems.Item(1);
                var selectedPath = selected.ProjectItem.FileNames[0];
                var outputPath = selectedPath + ".txt";

                var task = new BuildTask(selectedPath);
                var text = task.Build();
                if (text != null)
                {
                    var list = new List<string>();
                    for (int i = 2; i < selected.ProjectItem.ProjectItems.Count; i++)
                    {
                        list.Add(selected.ProjectItem.ProjectItems.Item(i).FileNames[0]);
                    }
                    for(int i=0;i<list.Count;i++)
                    {
                        File.Delete(list[i]);
                    }
                    if (!File.Exists(outputPath))
                    {
                        var writer = File.CreateText(outputPath);
                        writer.WriteLine(text);
                        writer.Flush();
                        writer.Close();
                    }
                    else
                    {
                        File.WriteAllText(outputPath, text);
                    }
                    selected.ProjectItem.ProjectItems.AddFromFile(outputPath);
                }*/
            }
        }


        private void BuildProject(Project project)
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

            errorCount = 0;
            package.ClearError();
            Build.FBSBuilder.Build(project.Name, paths.ToArray(), ErrorHandler);
            if(errorCount>0)
            {
                package.ShowError();
            }
        }

        private void ErrorHandler(string projectName,string path,string text,int line,int column)
        {
            errorCount++;
            package.AddError(projectName, path, text, line, column);
        }
    }
}
