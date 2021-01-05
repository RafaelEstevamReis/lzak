using System;
using System.Collections.Generic;
using System.Text;

namespace Terminal
{
    public enum DialogResult
    {
        Ok,
        Cancel
    }

    public interface IConsoleWindow
    {
        public abstract DialogResult Draw(string Title, 
                                          string Contents);

        public abstract DialogResult Draw(string Title, 
                                          string Contents, 
                                          string OkText = null, 
                                          string CancelText = null);
    }
}
