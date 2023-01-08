using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitData))]
public class UnitDataEditor : Editor
{
    [SerializeField] private UnitData unitData;
    public UnitData UnitData => unitData;

    private void OnEnable() => unitData = target as UnitData;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Texture2D texture = AssetPreview.GetAssetPreview(unitData.mySprite);
        GUILayout.Label("", GUILayout.Height(80), GUILayout.Width(80));
        GUI.DrawTexture(GUILayoutUtility.GetLastRect(), texture);
    }
}
