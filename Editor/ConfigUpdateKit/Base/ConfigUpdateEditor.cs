using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Mono.Cecil.Cil;
using Unity.Plastic.Antlr3.Runtime.Misc;
using UnityEditor.PackageManager;
using System;

namespace WB.ConfigUpdate
{

    [InitializeOnLoad]
    public class ConfigUpdateWindow : EditorWindow
    {

        ConfigUpdatePage Page;

        [MenuItem("Tools/���ù��� ConfigTool/���ø��´��� ConfigUpdateWindow")]
        public static void ShowDefaultWindow()
        {
            var wnd = GetWindow<ConfigUpdateWindow>();
            wnd.titleContent = new GUIContent("Config Update");       
        }

        [MenuItem("Tools/���ù��� ConfigTool/�����������ɣ����ز��ԣ�LocalGenTest")]
        public static void LocalGenTest()
        {
            //ʹ��shellִ��Ŀ���ļ�
            ShellHelper.Run(
                 "start %GAME_PROJECT_EXCELTOOL%\\Excel2ClassJsonApp.exe",
                 "c:/",
                 (sender, args) => {
                     if (args.Data != null)
                     {
                       
                     }
                 });
        }

        static ConfigUpdateWindow() 
        {

            //ÿ�ζ�����һ��·��
            //��ϵͳ�����л�ȡgit·��
            Environment.SetEnvironmentVariable("GAME_PROJECT_CLIENT_CLASS_DIR",Application.dataPath+ "/_Codes/Common/Configs/",EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("GAME_PROJECT_CLIENT_JSON_DIR", Application.dataPath + "/Configs/", EnvironmentVariableTarget.User);

            if (ConfigUpdatePage.GetInstallVersion(out string oldVer))
           {
                ConfigUpdatePage.GetNewVer((newVer) => { 
                
                    if (oldVer != newVer)
                    {
                        Debug.Log("��ǰ�������ã�");

                        //if (!UnityEditor.EditorUtility.DisplayDialog("����", "��ȷ��ѡ������ȷ��·���������Ƿ������", "ȷ��", "ȡ��"))
                        //{
                        //    return;
                        //}
                        //ShowDefaultWindow();
                        return;
                    }

                    Debug.Log($"���ñ��������ˣ�{oldVer}->{newVer}");
                });
           }
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