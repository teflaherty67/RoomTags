#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace RoomTags
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            string pathToSharedParameterFile = @"C:\MySharedParameters.txt";
            app.OpenSharedParameterFile = pathToSharedParameterFile;


            DefinitionFile sharedParameterFile = app.OpenSharedParameterFile();

            ExternalDefinition externalDefinition = null;
            try
            {
                externalDefinition = sharedParameterFile.Groups.get_Item("My Group").Definitions.get_Item("My Parameter");
            }
            catch
            {
                DefinitionGroup group = sharedParameterFile.Groups.Create("My Group");
                ExternalDefinitionCreationOptions options = new ExternalDefinitionCreationOptions("My Parameter", ParameterType.Text);
                externalDefinition = group.Definitions.Create(options) as ExternalDefinition;
            }

            Binding binding = app.Create.NewTypeBinding(externalDefinition);
            CategorySet categories = app.Create.NewCategorySet();
            categories.Insert(doc.Settings.Categories.get_Item(BuiltInCategory.OST_Walls));
            doc.ParameterBindings.Insert(externalDefinition, binding, categories);






            return Result.Succeeded;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
