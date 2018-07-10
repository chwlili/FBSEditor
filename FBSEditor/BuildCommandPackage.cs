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

        public BuildCommandPackage()
        {
        }

        #region Package Members

        protected override void Initialize()
        {
            BuildCommand.Initialize(this);

            var dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE;
            
            dte.Events.BuildEvents.OnBuildBegin += BuildEvents_OnBuildBegin;

            base.Initialize();
        }

        private void BuildEvents_OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
        {
            var projectList = new List<Project>();

            var dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE;
            var panel = ServiceProvider.GlobalProvider.GetService(typeof(IVsOutputWindow)) as IVsOutputWindow;

            IVsOutputWindowPane aa = null;
            panel.GetPane(VSConstants.OutputWindowPaneGuid.BuildOutputPane_guid,out aa);
            aa.OutputString(".............................");


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
            project.Globals["tttttttttttt"] = "xxxxxx";
            project.Globals.VariablePersists["tttttttttttt"] = true;

            var paths = new List<string>();
            var names = new List<string>();

            var files = new List<ProjectItem>();
            var folder = new List<ProjectItem>();
            for(var i=1;i<=project.ProjectItems.Count;i++)
            {
                folder.Add(project.ProjectItems.Item(i));
            }
            while(folder.Count>0)
            {
                var first = folder[0];
                folder.RemoveAt(0);

                for (int i = 1; i <= first.ProjectItems.Count; i++)
                {
                    folder.Add(first.ProjectItems.Item(i));
                }
                for(short i=0;i<first.FileCount;i++)
                {
                    paths.Add(first.FileNames[i]);
                }

                files.Add(first);
                names.Add(first.Name);
            }

            var builder = new Build.FBSBuilder(project, paths.ToArray());
            builder.Build();
        }

        #endregion
    }
}
