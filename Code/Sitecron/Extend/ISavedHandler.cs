using System;

namespace Sitecron.Extend
{
    public interface ISavedHandler
    {
        void OnItemSaved(object sender, EventArgs args);
    }
}
