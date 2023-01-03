using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EA;

namespace EAAddin_ExportSenML
{
    public class Main
    {
        ExportSenMLUserControl addinDialog;
        //Runs when opening a repository connection
        public String EA_Connect(EA.Repository Repository)
        {
            return "SenML Export";
        }

        //Runs when closing a repository connection
        public virtual void EA_Disconnect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        //Creates addin-specific entries in the Extensions/<addin name> menu
        public object EA_GetMenuItems(EA.Repository Repository, string Location, string MenuName)
        {
            switch (MenuName)
            {
                case "":
                    return "-&SenML Export";
                case "-&SenML Export":
                    string[] ar = { "&Load add-in window", "&Show info" };
                    return ar;
            }
            return "";
        }

        public void EA_GetMenuState(EA.Repository Repository, string Location, string MenuName, string ItemName, ref bool IsEnabled, ref bool IsChecked)
        {
            //Menu is always enabled
            IsEnabled = true;
        }
        //Handling menu clicks
        public void EA_MenuClick(EA.Repository Repository, string Location, string MenuName, string ItemName)
        {
            switch (ItemName)
            {
                case "&Load add-in window":
                    if (this.addinDialog == null)
                    {
                        LoadAddin(Repository);
                    }
                    else
                    {
                        MessageBox.Show("Already loaded.");
                    }
                    break;
                case "&Show info":
                    ExportSenMLHelp helpForm = new ExportSenMLHelp();
                    helpForm.Show();
                    break;
            }
        }

        //Runs when navigating in the project browser (GUID: EA guid of the selected object)
        public void EA_OnContextItemChanged(EA.Repository Repository, string GUID, EA.ObjectType ot)
        {
            //Addin is automatically loaded if not running yet - optional
            if (this.addinDialog == null)
            {
                LoadAddin(Repository);
            }
        }

        //Show AddinDialogUserControl in the Add-in Window of EA
        private void LoadAddin(EA.Repository Repository)
        {
            this.addinDialog = Repository.AddWindow("SenML Export", "EAAddin_ExportSenML.ExportSenMLUserControl") as ExportSenMLUserControl;
            this.addinDialog.Repository = Repository;
        }
    }
}
