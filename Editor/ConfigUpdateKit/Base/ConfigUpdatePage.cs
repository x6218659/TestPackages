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
    /// ���ø���ҳ��
    /// </summary>
    public class ConfigUpdatePage : VisualElement
    {

        //���嵽����ȥ
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
        /// ��ʼ��
        /// </summary>
        /// <param name="action">��ť�¼�</param>
        public ConfigUpdatePage()
        {
            //��ʼ��
            Init();
        }



        public void Init()
        {

            // Import UXML
            VisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ConfigUpdateKit/Uxml/ConfigUpdatePage.uxml");

            //���ɽṹ
            VisualTree.CloneTree(this);

            //��ʼ������
            VerItemDic = new Dictionary<string, VisualElement>();

            //����汾��Ϣ
            VerList = new List<string>();

            //���ҹؼ�����
            VerInfoLabel = this.Q<Label>("VerInfoLabel");
            CurSelectVerLabel = this.Q<Label>("CurSelectVerLabel");
            NewUpdatePanel = this.Q<VisualElement>("NewUpdatePanel");
            NewVerInfoLabel = this.Q<Label>("NewVerInfoLabel");
            InfoScrollView = this.Q<ScrollView>("InfoScrollView");
            VerItemScrollView = this.Q<ScrollView>("VerItemScrollView");
            UpdateToNew = this.Q<Button>("UpdateToNew");
            UpdateToTarget = this.Q<Button>("UpdateToTarget");

            //��ʼ����ť�¼�
            UpdateToNew.clicked += UpdateToNew_clicked;
            UpdateToTarget.clicked += UpdateToTarget_clicked; ;

            //��ϵͳ�����л�ȡ˫��·��
            GITPATH = Environment.GetEnvironmentVariable(GITPATHNAME, EnvironmentVariableTarget.User);

            //������Ϣ
            ReSet();
        }

        /// <summary>
        /// ����
        /// </summary>
        public void ReSet()
        {
            //����
            ClearItems();

            //������Ϣ
            SetInfo();

           
        }

        /// <summary>
        /// ������Ϣ.Ŀǰ��������
        /// </summary>
        public void SetInfo()
        {
         
            //�ж�·��
            if (GITPATH == null || GITPATH == "")
            {
                UnityEngine.Debug.LogError("����git�ļ�·��δע�ᣡ");
                return;
            }

            //��ȡ���°汾��Ϣ
            ShellHelper.Run(
                "git describe --abbrev=0 --tags",
                GITPATH,
                (sender, args) => {
                if (args.Data != null)
                {
                    SetNewVer(args.Data);                  
                }
            });

            //��ȡ���а汾��
            ShellHelper.Run(
                "git tag --sort=taggerdate",
                GITPATH,
                (sender, args) => {
                    if (args.Data != null)
                    {
                        VerList.Add(args.Data);                      
                    }
            });
            //��ʼ���б�
            CheckUpdateInfo();

            //�������°汾
            SetCurVer(NewVerStr);
        }

        /// <summary>
        /// �������°汾
        /// </summary>
        /// <param name="ver"></param>
        public void SetNewVer(string ver)
        {
            NewVerStr = ver;

            //����״̬�������ǰ���������Ǿ���ʾ��Ϣ
            NewUpdatePanel.visible = true;
            NewVerInfoLabel.text = $"���µ����ð汾��{NewVerStr}";
        }

        /// <summary>
        /// ���õ�ǰѡ��汾
        /// </summary>
        /// <param name="ver"></param>
        public void SetCurVer(string ver)
        {
            CurSelectVerLabel.text = ver;
            CurVerStr = ver;
            SetUpdateInfo(ver);
        }

        /// <summary>
        /// ���ø�����Ϣ��ʾ
        /// </summary>
        /// <param name="ver"></param>
        public void SetUpdateInfo(string ver)
        {
            StringBuilder stringBuilder = new StringBuilder();

            //��ʾ�汾��Ϣ
            ShellHelper.Run(
               $"git show {ver}",
               GITPATH,
               (sender, args) => {
                   if (args.Data != null)
                   {
                       stringBuilder.AppendLine(args.Data);
                   }
               });

            //���ݰ汾��ȡ������Ϣ
            string info = stringBuilder.ToString();///����Ӧ�øĳ���Ϣ��ȡ

            if (ver == null)
            {
                info = "�汾����";
                return;
            }

            if (info == null)
            {
                info = "��Ϣ��ȡ����";
                return;
            }

            VerInfoLabel.text = info;
        }

        /// <summary>
        /// ��ʼ����Ϣ
        /// </summary>
        public void CheckUpdateInfo()
        {
            VerList.Reverse();

            //ѭ������б�
            VerList.ForEach(ver => {
                //��Ӱ�ť
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
        /// ��Ӱ汾��ť
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
        /// ����
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
        /// ���õ�Ŀ��汾
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

                //�Ƴ���
                if (itHas)
                {
                    //���Ƴ� �����
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
                ////���
                //Client.Add($"https://github.com/x6218659/TestPackages.git#{ver}");
                Client.Resolve();
            }
            else if (request.Status >= StatusCode.Failure)
            {
                Debug.Log(request.Error.message);
            }


        }

        /// <summary>
        /// ���µ����°汾��ť�¼�
        /// </summary>
        void UpdateToNew_clicked()
        {
            if (NewVerStr == null || NewVerStr == "")
            {
                Debug.LogWarning("û�����°汾��Ϣ��");
                return;
            }
            //ʵ��Ҫ��������
            //����package.json�İ汾������
            Debug.Log($"���µ����°汾��{NewVerStr}");

            SetCurVer(NewVerStr);

            //ˢ�����ð汾
            SetManifestVersion(CurVerStr);

            //AssetDatabase.Refresh();
        }

        /// <summary>
        /// ���µ�ѡ��汾��ť�¼�
        /// </summary>
        void UpdateToTarget_clicked()
        {
            if (CurVerStr == null || CurVerStr == "")
            {
                Debug.LogWarning("û��ѡ��Ŀ��汾��");
                return;
            }
            //ʵ��Ҫ��������
            //����package.json�İ汾������
            Debug.Log($"���µ�Ŀ��汾��{CurVerStr}");

            //ˢ�����ð汾     
            SetManifestVersion(CurVerStr);

            //AssetDatabase.Refresh();
        }
    }

}
