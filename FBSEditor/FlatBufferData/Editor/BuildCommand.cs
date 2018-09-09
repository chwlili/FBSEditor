using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;

namespace FlatBufferData.Editor
{
    internal sealed class BuildCommand
    {
        public const int CommandId = 0x0200;
        public static readonly Guid CommandSet = new Guid("04f51c64-0c0a-412c-818c-57880c441058");

        private readonly Package package;

        public static BuildCommand Instance
        {
            get;
            private set;
        }
        private IServiceProvider ServiceProvider { get { return this.package; } }

        public static void Initialize(Package package)
        {
            Instance = new BuildCommand(package);
        }

        private BuildCommand(Package package)
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
                var selected = dte.SelectedItems.Item(1);
                var selectedPath = selected.ProjectItem.FileNames[0];
                var outputPath = selectedPath + ".txt";
                /*
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
    }
}
