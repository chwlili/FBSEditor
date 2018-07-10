using System;
using System.ComponentModel.Design;
using System.Globalization;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace FBSEditor
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class BuildCommand
    {
        public const int CommandId = 0x0100;
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

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                CommandID menuCommandID = new CommandID(CommandSet, CommandId);
                OleMenuCommand menuItem = new OleMenuCommand(MenuItemCallback, menuCommandID);
                menuItem.BeforeQueryStatus += new EventHandler(OnBeforeQueryStatus);

                commandService.AddCommand(menuItem);
            }
        }
        private void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            var myCommand = sender as OleMenuCommand;
            if (null != myCommand)
            {
                DTE dte = ServiceProvider.GetService(typeof(DTE)) as DTE;

                if(dte.SelectedItems.Count==1)
                {
                    var t = dte.SelectedItems.Item(1);
                    var project1 = dte.SelectedItems.Item(1).Project;
                    var project2 = dte.SelectedItems.Item(1).ProjectItem;

                    for (int i = 1; i <= project1.ProjectItems.Count; i++)
                    {
                        var item = project1.ProjectItems.Item(i);
                        var tttt = item.GetType();
                        if(item.Properties==project1.Properties)
                        {
                            if(item.ProjectItems == project1.ProjectItems)
                            {
                                Console.WriteLine("...");
                            }
                        }
                    }
                    for (int i = 1; i <= project1.Properties.Count; i++)
                    {
                        var prop = project1.Properties.Item(i);
                    }
                    if (project1 != null)
                    {
                        myCommand.Visible = true;
                    }
                }
                else
                {

                    myCommand.Visible = false;
                }
            }
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
            string title = "BuildCommand";

            //VsShellUtilities.ShowMessageBox(this.ServiceProvider, message, title, OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

            DTE dte = ServiceProvider.GetService(typeof(DTE)) as DTE;
            if (dte == null)
                return;

            var solution = dte.Solution;
            var projects = dte.Solution.Projects;
            var select = dte.SelectedItems;

            //var txt = select.Item(0);

            //TextSelection ts = dte.ActiveDocument.Selection as TextSelection;
            //防止用户在</mappings>结束符后进行操作，在结束符后操作的的FindText方法返回的结果为false
            //ts.MoveToLineAndOffset(1, 1);
            /*
            bool result = ts.FindText("</mappings>", (int)vsFindOptions.vsFindOptionsMatchWholeWord);
            if (!result)
            {
                if (ts.ActivePoint.Line == 1)
                {
                    ts.EndOfLine();
                    ts.NewLine();
                }
                string str = "<mappings>\r\n" + sb.ToString();
                ts.Insert(str);
            }
            else
            {
                //需要添加此操作，否则不会替换成功
                ts.SelectAll();
                ts.ReplacePattern("</mappings>", sb.ToString(), (int)vsFindOptions.vsFindOptionsMatchWholeWord);
            }*/
        }

        private void BuildEvents_OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
        {
            throw new NotImplementedException();
        }
    }
}
