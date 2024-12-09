using geeWiz.Commands;
using Nice3point.Revit.Toolkit.External;

namespace geeWiz
{
    /// <summary>
    ///     Application entry point
    /// </summary>
    public class Application : ExternalApplication
    {
        public override void OnStartup()
        {
            CreateRibbon();
        }

        private void CreateRibbon()
        {
            var panel = Application.CreatePanel("Commands", "geeWiz");

            panel.AddPushButton<StartupCommand>("Execute")
                .SetImage("/geeWiz;component/Resources/Icons/RibbonIcon16.png")
                .SetLargeImage("/geeWiz;component/Resources/Icons/RibbonIcon32.png");
        }
    }
}