using UnityEditor;
using UnityEngine;

namespace Roulette.Editor
{
    [CustomEditor(typeof(RouletteSlotLabelGenerator))]
    public class RouletteSlotLabelGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var myScript = (RouletteSlotLabelGenerator)target;

            if (GUILayout.Button("Generate and align reward amount text"))
            {
                myScript.AlignText();
            }
        }
    }
}