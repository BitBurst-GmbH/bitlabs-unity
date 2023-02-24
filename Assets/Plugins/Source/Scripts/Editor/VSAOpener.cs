using UnityEditor;

[InitializeOnLoad]
public static class VSAOpener
{
    static VSAOpener()
    {
        if (!EditorPrefs.GetBool("BitBurstInit", false))
        {
            VSAttributionWindow.Initialize();

            //EditorPrefs.SetBool("BitBurstInit", true);
        }
    }
}
