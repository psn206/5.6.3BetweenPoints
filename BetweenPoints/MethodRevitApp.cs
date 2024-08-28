using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BetweenPoints
{
    internal class MethodRevitApp
    {
        UIApplication uiApp;
        UIDocument uiDoc;
        Document doc;

        public UIApplication UiApp { get => uiApp; }
        public UIDocument UiDoc { get => uiDoc; }
        public Document Doc { get => doc; }

        public MethodRevitApp(ExternalCommandData commandData)
        {
            uiApp = commandData.Application;
            uiDoc = UiApp.ActiveUIDocument;
            doc = UiDoc.Document;
        }

        public List<FamilySymbol> GetListElement()
        {
            var familySymbols = new FilteredElementCollector(doc)
                 .OfClass(typeof(FamilySymbol))
                 .Cast<FamilySymbol>()
                 .ToList();
            return familySymbols;
        }

        public List<XYZ> GetPoints()
        {
            string[] message = { "Начальная точка", "Конечная точка" };

            List<XYZ> points = new List<XYZ>();
            for (int i = 0; i < 2; i++)
            {
                XYZ pickedPoint = null;
                try
                {
                    pickedPoint = UiDoc.Selection.PickPoint(ObjectSnapTypes.Endpoints, message[i]);
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
                {
                    break;
                }
                points.Add(pickedPoint);
            }
            return points;
        }

        public void СreateElements(List<XYZ> points, FamilySymbol selectedElementInList, string step)
        {

            if (!selectedElementInList.IsActive)
            {
                selectedElementInList.Activate();
                Doc.Regenerate();
            }

            int stepInt = Convert.ToInt32(step) + 1;
            Line line = Line.CreateBound(points[0], points[1]);
            double distance = line.Length / stepInt;
            XYZ direction = (points[1] - points[0]).Normalize();

            using (var ts = new Autodesk.Revit.DB.Transaction(Doc, "Create family instance"))
            {
                ts.Start();

                for (int i = 1; i < stepInt; i++)
                {
                    Doc.Create.NewFamilyInstance((points[0] + distance * direction * i), selectedElementInList, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                }
                ts.Commit();
            }
        }
    }
}
