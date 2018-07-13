using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FBSEditor;
using FBSEditor.Generator;

namespace FBSEditor
{
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(BuildCommandPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class BuildCommandPackage : Package
    {
        public const string PackageGuidString = "912e1fad-78c1-4ba3-8e6d-92e9fc3dc91e";

        public const string OutputPanelGuidString = "FAF9C7D3-FD49-4CBC-9F2B-8E1CBADA4299";
        public Guid OutputPanelGuid = new Guid(OutputPanelGuidString);

        private DTE dte;
        private ErrorListProvider ErrorList;
        private IVsOutputWindow outputWindow;

        protected override void Initialize()
        {
            BuildCommand.Initialize(this);

            dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE;
            dte.Events.BuildEvents.OnBuildBegin += BuildEvents_OnBuildBegin;

            ErrorList = new ErrorListProvider(this);

            outputWindow = ServiceProvider.GlobalProvider.GetService(typeof(IVsOutputWindow)) as IVsOutputWindow;

            base.Initialize();
        }

        #region 输出面板管理

        public void Output(string text)
        {
            IVsOutputWindowPane outputPanel = null;
            outputWindow.GetPane(VSConstants.OutputWindowPaneGuid.BuildOutputPane_guid, out outputPanel);
            outputPanel.OutputString(text + "\n");
        }

        #endregion

        #region 错误列表管理

        public void ClearError()
        {
            ErrorList.Tasks.Clear();
        }

        public void AddError(string projectFileName,string filePath,string text,int line,int column)
        {
            var ivsSolution = (IVsSolution)Package.GetGlobalService(typeof(IVsSolution));
            IVsHierarchy hierarchyItem;
            ivsSolution.GetProjectOfUniqueName(projectFileName, out hierarchyItem);

            var error = new ErrorTask();
            error.Line = line - 1;
            error.Column = column;
            error.Text = text;
            error.ErrorCategory = TaskErrorCategory.Error;
            error.Category = TaskCategory.BuildCompile;
            error.Document = filePath;
            error.HierarchyItem = hierarchyItem;

            error.Navigate += (sender, e) =>
            {
                error.Line++;
                ErrorList.Navigate(error, new Guid(EnvDTE.Constants.vsViewKindCode));
                error.Line--;
            };
            ErrorList.Tasks.Add(error);
        }

        public void ShowError()
        {
            ErrorList.Show();
        }

        #endregion

        private void BuildEvents_OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
        {
            var projectList = new List<Project>();

            var edition = dte.Edition;
            var fileName = dte.FileName;
            var fullName = dte.FullName;
            var LocalID = dte.LocaleID;
            var name = dte.Name;
            var mode = dte.Mode;

            if (Scope == vsBuildScope.vsBuildScopeSolution)
            {
                var projects = dte.Solution.Projects;
                for (int i = 1; i <= projects.Count; i++)
                {
                    projectList.Add(projects.Item(i));
                }
            }
            else if(Scope == vsBuildScope.vsBuildScopeProject)
            {
                var names = new HashSet<string>();
                for(int i=1;i<=dte.SelectedItems.Count;i++)
                {
                    var project = dte.SelectedItems.Item(i).Project;
                    if(!names.Contains(project.Name))
                    {
                        names.Add(project.Name);
                        projectList.Add(project);
                    }
                }
            }

            foreach(var project in projectList)
            {
                BuildProject(project);
            }
        }

        private void BuildProject(Project project)
        {
            //project.Globals["tttttttttttt"] = "xxxxxx";
            //project.Globals.VariablePersists["tttttttttttt"] = true;

            var paths = new List<string>();
            var files = new List<ProjectItem>();
            var folder = new List<ProjectItem>();

            for (var i = 1; i <= project.ProjectItems.Count; i++) { folder.Add(project.ProjectItems.Item(i)); }

            while(folder.Count>0)
            {
                var first = folder[0];
                var fileName = first.Name;
                var filePath = first.FileNames[0];
                var isFile = File.Exists(filePath) && !Directory.Exists(filePath);

                if(isFile && fileName.ToLower().EndsWith(Constants.FBSExtName))
                {
                    var csFilePath = filePath.Substring(0, filePath.Length - Constants.FBSExtName.Length) + ".cs";

                    var csFileCount = 0;
                    var invalidateFiles = new List<ProjectItem>();
                    for (int i = 1; i <= first.ProjectItems.Count; i++)
                    {
                        var current = first.ProjectItems.Item(i);
                        if(!current.FileNames[0].Equals(csFilePath))
                        {
                            invalidateFiles.Add(current);
                        }
                        else
                        {
                            csFileCount ++;
                        }
                    }
                    foreach(var invalidateFile in invalidateFiles)
                    {
                        invalidateFile.Remove();
                    }

                    if(csFileCount==0)
                    {
                        if (!File.Exists(csFilePath))
                        {
                            var writer = File.CreateText(csFilePath);
                            writer.WriteLine("class " + fileName.Substring(0, fileName.Length - Constants.FBSExtName.Length) + " \n{\n}");
                            writer.Flush();
                            writer.Close();
                        }
                        first.ProjectItems.AddFromFile(csFilePath);
                    }

                    paths.Add(filePath);
                }
                else if (first.FileCount >1)
                {
                    for (int i = 1; i <= first.ProjectItems.Count; i++)
                    {
                        folder.Add(first.ProjectItems.Item(i));
                    }
                }

                files.Add(first);
                folder.RemoveAt(0);
            }

            ErrorList.Tasks.Clear();

            var builder = new Build.FBSBuilder(this, project.FileName, paths.ToArray());
            var trees = builder.Build();

            if (ErrorList.Tasks.Count > 0)
            {
                ErrorList.Show();
            }
            else
            {
                CSGenerator.Generate(this, trees);
            }
        }
    }
}
