using UnityEditor;
using UnityEngine;
using UnityEditor.VSAttribution.BitLabs;

public class StaticScriptSample : EditorWindow
{
    static readonly Vector2 s_WindowSize = new(320, 100);

    public string customerUid;

    [MenuItem("BitLabs/Initialise")]
    public static void Initialize()
    {
        var window = GetWindow<StaticScriptSample>();

        window.titleContent = new GUIContent("VS Attribution");
        window.minSize = s_WindowSize;
        window.maxSize = s_WindowSize;
    }

    public void OnGUI()
    {
        customerUid = EditorGUILayout.TextField("Your API Key", customerUid);

        GUILayout.Space(20f);

        if (GUILayout.Button("Send Attribution Event"))
        {
            var result = VSAttribution.SendAttributionEvent("VSA", "BitLabs Publisher", customerUid);
            Debug.Log($"[VS Attribution] Attribution Event returned status: {result}!");
        }
    }
}