using System;
using System.Collections.Generic;
using System.Text;
using Terminal;

namespace Terminal.MessageBox
{
    public class OkCancel : IConsoleWindow
    {
        bool debugMode = true;

        private OkCancel() { }

        public static void Show(string Title, string Contents, string OkText = null, string CancelText = null)
        {
            var msg = new OkCancel();
            msg.Draw(Title, Contents, OkText, CancelText);
        }

        public DialogResult Draw(string Title, string Contents)
        {
            return Draw(Title, Contents, null, null);
        }

        public DialogResult Draw(string Title, string Contents, string OkText = null, string CancelText = null)
        {
            while(true)
            {
                if(debugMode) Console.Clear(); // mustn't leave this here
                Helpers.DrawCorner(Corner.TopLeft, 0, 0);
                Helpers.DrawHorizontalLine(5, 1, 0);
                Helpers.DrawCorner(Corner.TopRight, 6, 0);
                Helpers.DrawCorner(Corner.LowerLeft, 0, 1);
                Helpers.DrawHorizontalLine(5, 1, 1);
                Helpers.DrawCorner(Corner.LowerRight, 6, 1);

                Contents = Contents;
            }
        }
    }
}
