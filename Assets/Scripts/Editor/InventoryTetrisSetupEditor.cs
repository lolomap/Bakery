#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(InventoryTetrisSetup))]
public class InventoryTetrisSetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        InventoryTetrisSetup setup = (InventoryTetrisSetup)target;

        GUILayout.Space(10);

        EditorGUI.BeginDisabledGroup(Application.isPlaying);
        if (GUILayout.Button("Создать / Обновить Инвентарь"))
        {
            setup.SetupInventory();
            EditorUtility.SetDirty(setup);
            EditorUtility.SetDirty(setup.inventoryTetris);
            EditorUtility.SetDirty(setup.background);
        }
        EditorGUI.EndDisabledGroup();

        if (Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Редактирование инвентаря недоступно во время игры", MessageType.Warning);
        }
    }
}
#endif