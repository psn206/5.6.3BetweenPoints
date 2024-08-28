using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetweenPoints
{
    internal class MainViewViewModel
    {

        MethodRevitApp methodRevitApp;
        public DelegateCommand SaveCommand { get; }
        List<XYZ> Points { get; } = new List<XYZ>();
        public List<FamilySymbol> ListElement { get; set; } = new List<FamilySymbol>();
        public FamilySymbol SelectedElementInList { get; set; }
        public string Step { get; set; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            SaveCommand = new DelegateCommand(OnSaveCommand);
            methodRevitApp = new MethodRevitApp(commandData);
            Points  = methodRevitApp.GetPoints();
            ListElement = methodRevitApp.GetListElement();
        }

        private void OnSaveCommand()
        {
            methodRevitApp.СreateElements(Points,SelectedElementInList,Step);
            RaiseCloseRecuest();
        }

        public event EventHandler CloseRecuest;

        public void RaiseCloseRecuest()
        {
            CloseRecuest?.Invoke(this, EventArgs.Empty);
        }
    }
}
