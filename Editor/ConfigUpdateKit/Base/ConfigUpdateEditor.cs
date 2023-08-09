using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Mono.Cecil.Cil;
using Unity.Plastic.Antlr3.Runtime.Misc;

namespace WB.ConfigUpdate
{
    public class ConfigUpdateWindow : EditorWindow
    {

        ConfigUpdatePage Page;

         [MenuItem("Tools/ÅäÖÃ¸üÐÂ´°¿Ú ConfigUpdateWindow")]
        public static void ShowDefaultWindow()
        {
            var wnd = GetWindow<ConfigUpdateWindow>();
            wnd.titleContent = new GUIContent("Config Update");       
        }

        public void CreateGUI()
        {
            Page = new ConfigUpdatePage();
            rootVisualElement.Add(Page);
        }

     

        public void OnSelectionChange()
        {
            GameObject selectedObject = Selection.activeObject as GameObject;
            if (selectedObject != null)
            {
                //// Create the SerializedObject from the current selection
                //SerializedObject so = new SerializedObject(selectedObject);
                //// Bind it to the root of the hierarchy. It will find the right object to bind to.
                //rootVisualElement.Bind(so);
            }
            else
            {
                //// Unbind the object from the actual visual element
                //rootVisualElement.Unbind();

                //// Clear the TextField after the binding is removed
                //var textField = rootVisualElement.Q<TextField>("GameObjectName");
                //if (textField != null)
                //{
                //    textField.value = string.Empty;
                //}
            }
        }
    }
}