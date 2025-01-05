using System;
using Autodesk.Revit.UI;
using ResultClickEventArgs = Autodesk.Internal.InfoCenter.ResultClickEventArgs;
using gFil = geeWiz.Utilities.FileUtils;

namespace geeWiz.Forms
{
    internal class BubbleMessage
    {
        private string title;
        private string message;
        private string filePath;
        private string urlPath;

        public BubbleMessage(string title, string message, string urlPath = null, string filePath = null)
        {
            this.title = title;
            this.message = message;
            this.filePath = filePath;
            this.urlPath = urlPath;
        }

        public void Show()
        {
            var ri = new Autodesk.Internal.InfoCenter.ResultItem();

            ri.Category = this.title;
            ri.Title = this.message;

            if (this.urlPath != null && gFil.UrlIsValid(this.urlPath))
            {
                ri.Uri = new System.Uri(this.urlPath);
            }

            ri.IsFavorite = false;
            ri.IsNew = true;

            if (this.filePath != null || this.urlPath != null)
            {
                ri.ResultClicked += new EventHandler<ResultClickEventArgs>(ri_ResultClicked);
            }

            Autodesk.Windows.ComponentManager.InfoCenterPaletteManager.ShowBalloon(ri);
        }

        private void ri_ResultClicked(object sender, ResultClickEventArgs e)
        {
            if (this.filePath != null)
            {
                if (gFil.OpenFile(this.filePath) == Result.Succeeded)
                {
                    return;
                }
            }
            if (this.urlPath != null)
            {
                gFil.OpenUrl(this.urlPath);
            }
        }
    }
}