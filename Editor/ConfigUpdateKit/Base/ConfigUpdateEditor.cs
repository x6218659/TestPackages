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

        [MenuItem("Tools/配置工具 ConfigTool/配置更新窗口 ConfigUpdateWindow")]
        public static void ShowDefaultWindow()
        {
            var wnd = GetWindow<ConfigUpdateWindow>();
            wnd.titleContent = new GUIContent("Config Update");       
        }

        [MenuItem("Tools/配置工具 ConfigTool/本地配置生成（本地测试）LocalGenTest")]
        public static void LocalGenTest()
        {
            //使用shell执行目标文件
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

            //每次都设置一下路径
            //从系统配置中获取git路径
            Environment.SetEnvironmentVariable("GAME_PROJECT_CLIENT_CLASS_DIR",Application.dataPath+ "/_Codes/Common/Configs/",EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("GAME_PROJECT_CLIENT_JSON_DIR", Application.dataPath + "/Configs/", EnvironmentVariableTarget.User);

            if (ConfigUpdatePage.GetInstallVersion(out string oldVer))
           {
                ConfigUpdatePage.GetNewVer((newVer) => { 
                
                    if (oldVer != newVer)
                    {
                        Debug.Log("当前有新配置！");

                        //if (!UnityEditor.EditorUtility.DisplayDialog("警告", "请确定选择了正确的路径！！！是否继续？", "确认", "取消"))
                        //{
                        //    return;
                        //}
                        //ShowDefaultWindow();
                        return;
                    }

                    Debug.Log($"配置保持最新了！{oldVer}->{newVer}");
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