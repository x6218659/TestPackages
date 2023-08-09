using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.VersionControl;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.Burst.Intrinsics.X86.Avx;
using static UnityEngine.Rendering.ReloadAttribute;

namespace WB.ConfigUpdate
{
    /// <summary>
    /// 配置更新页面
    /// </summary>
    public class ConfigUpdatePage : VisualElement
    {

        //定义到库中去
        public new class UxmlFactory : UxmlFactory<ConfigUpdatePage, VisualElement.UxmlTraits> { }

        public string GITPATHNAME = "GAME_PROJECT_OUTPUTGITFOLDER";
        public string GITPATH;

        [SerializeField]
        VisualTreeAsset VisualTree;

        string NewVerStr;
        string CurVerStr;

        Label VerInfoLabel;
        Label CurSelectVerLabel;
        ScrollView InfoScrollView;
        ScrollView VerItemScrollView;
        Button UpdateToNew;
        Button UpdateToTarget;

        VisualElement NewUpdatePanel;
        Label NewVerInfoLabel;

        List<string> VerList;
        Dictionary<string, VisualElement> VerItemDic;

     

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="action">按钮事件</param>
        public ConfigUpdatePage()
        {
            //初始化
            Init();
        }



        public void Init()
        {

            // Import UXML
            VisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ConfigUpdateKit/Uxml/ConfigUpdatePage.uxml");

            //生成结构
            VisualTree.CloneTree(this);

            //初始化管理
            VerItemDic = new Dictionary<string, VisualElement>();

            //管理版本信息
            VerList = new List<string>();

            //查找关键对象
            VerInfoLabel = this.Q<Label>("VerInfoLabel");
            CurSelectVerLabel = this.Q<Label>("CurSelectVerLabel");
            NewUpdatePanel = this.Q<VisualElement>("NewUpdatePanel");
            NewVerInfoLabel = this.Q<Label>("NewVerInfoLabel");
            InfoScrollView = this.Q<ScrollView>("InfoScrollView");
            VerItemScrollView = this.Q<ScrollView>("VerItemScrollView");
            UpdateToNew = this.Q<Button>("UpdateToNew");
            UpdateToTarget = this.Q<Button>("UpdateToTarget");

            //初始化按钮事件
            UpdateToNew.clicked += UpdateToNew_clicked;
            UpdateToTarget.clicked += UpdateToTarget_clicked; ;

            //从系统配置中获取双端路径
            GITPATH = Environment.GetEnvironmentVariable(GITPATHNAME, EnvironmentVariableTarget.User);

            //配置信息
            ReSet();
        }

        /// <summary>
        /// 配置
        /// </summary>
        public void ReSet()
        {
            //清理
            ClearItems();

            //设置信息
            SetInfo();

           
        }

        /// <summary>
        /// 设置信息.目前做测试用
        /// </summary>
        public void SetInfo()
        {
         
            //判断路径
            if (GITPATH == null || GITPATH == "")
            {
                UnityEngine.Debug.LogError("配置git文件路径未注册！");
                return;
            }

            //获取最新版本信息
            ShellHelper.Run(
                "git describe --abbrev=0 --tags",
                GITPATH,
                (sender, args) => {
                if (args.Data != null)
                {
                    SetNewVer(args.Data);                  
                }
            });

            //获取所有版本号
            ShellHelper.Run(
                "git tag --sort=taggerdate",
                GITPATH,
                (sender, args) => {
                    if (args.Data != null)
                    {
                        VerList.Add(args.Data);                      
                    }
            });
            //初始化列表
            CheckUpdateInfo();

            //设置最新版本
            SetCurVer(NewVerStr);
        }

        /// <summary>
        /// 设置最新版本
        /// </summary>
        /// <param name="ver"></param>
        public void SetNewVer(string ver)
        {
            NewVerStr = ver;

            //设置状态，如果当前不是最新那就显示信息
            NewUpdatePanel.visible = true;
            NewVerInfoLabel.text = $"有新的配置版本！{NewVerStr}";
        }

        /// <summary>
        /// 设置当前选择版本
        /// </summary>
        /// <param name="ver"></param>
        public void SetCurVer(string ver)
        {
            CurSelectVerLabel.text = ver;
            CurVerStr = ver;
            SetUpdateInfo(ver);
        }

        /// <summary>
        /// 设置更新信息显示
        /// </summary>
        /// <param name="ver"></param>
        public void SetUpdateInfo(string ver)
        {
            StringBuilder stringBuilder = new StringBuilder();

            //显示版本信息
            ShellHelper.Run(
               $"git show {ver}",
               GITPATH,
               (sender, args) => {
                   if (args.Data != null)
                   {
                       stringBuilder.AppendLine(args.Data);
                   }
               });

            //根据版本获取更新信息
            string info = stringBuilder.ToString();///后续应该改成信息获取

            if (ver == null)
            {
                info = "版本错误！";
                return;
            }

            if (info == null)
            {
                info = "信息获取错误！";
                return;
            }

            VerInfoLabel.text = info;
        }

        /// <summary>
        /// 初始化信息
        /// </summary>
        public void CheckUpdateInfo()
        {
            VerList.Reverse();

            //循环添加列表
            VerList.ForEach(ver => {
                //添加按钮
                AddVerBoxItem(ver, () => {
                    if (ver == null)
                    {
                        return;
                    }
                    SetCurVer(ver);
                });

            });

            if (VerList.Count > 0)
            {
                SetCurVer(VerList[0]);
            }
        }

        /// <summary>
        /// 添加版本按钮
        /// </summary>
        /// <param name="ver"></param>
        /// <param name="action"></param>
        public void AddVerBoxItem(string ver, Action action = null)
        {
            if (VerItemDic.ContainsKey(ver))
            {
                return;
            }
            var elm = new VerItemBoxPage(ver, action);

            if (ver != NewVerStr)
            {
                elm.SetNewLabelState(false);
            }

            VerItemDic.Add(ver, elm);
            VerItemScrollView.Add(elm);
        }

        /// <summary>
        /// 清理
        /// </summary>
        public void ClearItems()
        {
            foreach (var keyValuePair in VerItemDic)
            {
                VerItemScrollView.Remove(keyValuePair.Value);
            }
            VerList.Clear();
            VerItemDic.Clear();
        }

        /// <summary>
        /// 设置到目标版本
        /// </summary>
        /// <param name="ver"></param>
        public void SetManifestVersion(string ver)
        {
          

           var request =Client.List(true);

            while (!request.IsCompleted)
            {
                Thread.Sleep(1);
            }

            string packVer = "";

            if (request.Status == StatusCode.Success)
            {
                bool itHas = false;
               

                foreach (var package in request.Result)
                {
                    if (package.source != PackageSource.Git)
                    {
                        continue;
                    }
                    Debug.Log("Package Name: " + package.name);
                    Debug.Log("Package Source: " + package.source);
                    //Debug.Log("Package Hash: " + package.git.hash);
                    
                    if (package.name != "com.wb.gameconfigs")
                    {
                        continue;
                    }

                    packVer = package.version;
                    itHas = true;
                }

                //移除先
                if (itHas)
                {
                    //先移除 再添加
                   // Client.Remove(packName);
                }

                var filePath = "./Packages/manifest.json";

                if (File.Exists(filePath))
                {
                    StringBuilder sb=new StringBuilder();

                    string str = File.ReadAllText(filePath);
                    var strlist = str.Split('\n');
                    for (int i = 0; i < strlist.Length; i++)
                    {
                        if (strlist[i].Contains("com.wb.gameconfigs"))
                        {
                            strlist[i]= strlist[i].Replace(packVer, ver);                        
                        }

                        sb.Append(strlist[i]);
                    }

                    File.WriteAllText(filePath, sb.ToString());
                }

                //AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                ////添加
                //Client.Add($"https://github.com/x6218659/TestPackages.git#{ver}");
                Client.Resolve();
            }
            else if (request.Status >= StatusCode.Failure)
            {
                Debug.Log(request.Error.message);
            }


        }

        /// <summary>
        /// 更新到最新版本按钮事件
        /// </summary>
        void UpdateToNew_clicked()
        {
            if (NewVerStr == null || NewVerStr == "")
            {
                Debug.LogWarning("没有最新版本信息！");
                return;
            }
            //实际要做的事情
            //更改package.json的版本号内容
            Debug.Log($"更新到最新版本！{NewVerStr}");

            SetCurVer(NewVerStr);

            //刷新设置版本
            SetManifestVersion(CurVerStr);

            //AssetDatabase.Refresh();
        }

        /// <summary>
        /// 更新到选择版本按钮事件
        /// </summary>
        void UpdateToTarget_clicked()
        {
            if (CurVerStr == null || CurVerStr == "")
            {
                Debug.LogWarning("没有选择目标版本！");
                return;
            }
            //实际要做的事情
            //更改package.json的版本号内容
            Debug.Log($"更新到目标版本！{CurVerStr}");

            //刷新设置版本     
            SetManifestVersion(CurVerStr);

            //AssetDatabase.Refresh();
        }
    }

}
