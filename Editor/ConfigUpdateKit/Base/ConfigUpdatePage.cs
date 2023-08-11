using Codice.Client.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public string CLASSPATHNAME = "GAME_PROJECT_CLIENT_CLASS_DIR";
        public string CLASSPATH;

        public string JSONPATHNAME = "GAME_PROJECT_CLIENT_JSON_DIR";
        public string JSONPATH;

        [SerializeField]
        VisualTreeAsset VisualTree;

        string NewVerStr;
        string CurVerStr;
        static string CurInstallVerStr;

        Label VerInfoLabel;
        Label CurSelectVerLabel;
        Label ClassPathLabel;
        Label JsonPathLabel;
        ScrollView InfoScrollView;
        ScrollView VerItemScrollView;
        Button UpdateToNew;
        Button UpdateToTarget;
        Button CopyToDic;
        Button SetClassPathBtn;
        Button SetJsonPathBtn;

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
            ClassPathLabel = this.Q<Label>("ClassPathLabel");
            JsonPathLabel = this.Q<Label>("JsonPathLabel");
            NewUpdatePanel = this.Q<VisualElement>("NewUpdatePanel");
            NewVerInfoLabel = this.Q<Label>("NewVerInfoLabel");
            InfoScrollView = this.Q<ScrollView>("InfoScrollView");
            VerItemScrollView = this.Q<ScrollView>("VerItemScrollView");
            UpdateToNew = this.Q<Button>("UpdateToNew");
            UpdateToTarget = this.Q<Button>("UpdateToTarget");
            CopyToDic = this.Q<Button>("CopyToDic");
            SetClassPathBtn = this.Q<Button>("SetClassPathBtn");
            SetJsonPathBtn = this.Q<Button>("SetJsonPathBtn");

            //��ʼ����ť�¼�
            UpdateToNew.clicked += UpdateToNew_clicked;
            UpdateToTarget.clicked += UpdateToTarget_clicked;
            CopyToDic.clicked += CopyToDic_clicked;
            SetClassPathBtn.clicked += SetClassPathBtn_clicked;
            SetJsonPathBtn.clicked += SetJsonPathBtn_clicked;

            //��ϵͳ�����л�ȡgit·��
            GITPATH = Environment.GetEnvironmentVariable(GITPATHNAME, EnvironmentVariableTarget.User);

            //��ϵͳ�����л�ȡ˫��·��
            CLASSPATH = Environment.GetEnvironmentVariable(CLASSPATHNAME, EnvironmentVariableTarget.User);

            ClassPathLabel.text = CLASSPATH;

            //��ϵͳ�����л�ȡ˫��·��
            JSONPATH = Environment.GetEnvironmentVariable(JSONPATHNAME, EnvironmentVariableTarget.User);

            JsonPathLabel.text = JSONPATH;

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
            GetNewVer((newVer) => {
                SetNewVer(newVer);
            });

            //ShellHelper.Run(
            //    "git describe --abbrev=0 --tags",
            //    GITPATH,
            //    (sender, args) => {
            //    if (args.Data != null)
            //    {
            //        SetNewVer(args.Data);
            //    }
            //});

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
        /// ��ȡ�°汾��
        /// </summary>
        /// <param name="action"></param>
        public static void GetNewVer(Action<string> action)
        {
            //��ϵͳ�����л�ȡgit·��
            var path = Environment.GetEnvironmentVariable("GAME_PROJECT_OUTPUTGITFOLDER", EnvironmentVariableTarget.User);

            //��ȡ���°汾��Ϣ
            ShellHelper.Run(
                "git describe --abbrev=0 --tags",
                path,
                (sender, args) => {
                    if (args.Data != null)
                    {
                        action?.Invoke(args.Data);
                    }
                });
        }

        /// <summary>
        /// �������°汾
        /// </summary>
        /// <param name="ver"></param>
        public void SetNewVer(string ver)
        {
            NewVerStr = ver;

            //����״̬�������ǰ���������Ǿ���ʾ��Ϣ

            if (CurInstallVerStr == NewVerStr)
            {
                //NewUpdatePanel.visible = false;
                NewVerInfoLabel.text = $"��ǰ�Ѿ������°棡{NewVerStr}";
                return;
            }

            //NewUpdatePanel.visible = true;
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
        /// ��ȡ��ǰ�汾
        /// </summary>
        /// <param name="packVer"></param>
        /// <returns></returns>
        public static bool GetInstallVersion(out string packVer)
        {
            bool itHas = false;
            packVer = "";

            var request = Client.List(true);

            while (!request.IsCompleted)
            {
                Thread.Sleep(1);
            }

            if (request.Status == StatusCode.Success)
            {
              
                foreach (var package in request.Result)
                {
                    if (package.source != PackageSource.Git)
                    {
                        continue;
                    }
                 
                    if (package.name != "com.wb.gameconfigs")
                    {
                        continue;
                    }

                    packVer = package.version;
                    CurInstallVerStr = packVer;
                    itHas = true;
                }

            }
            else if (request.Status >= StatusCode.Failure)
            {
                Debug.Log(request.Error.message);
            }

            return itHas;
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

        /// <summary>
        /// ���Ƶ�Ĭ���ļ��У�ͨ��path��������
        /// </summary>
        void CopyToDic_clicked()
        {
            if (!UnityEditor.EditorUtility.DisplayDialog("����", "��������Ḳ��ԭ���ļ��������Ƿ������", "ȷ��", "ȡ��"))
            {
                return;
            }

            string[] revFolders = Directory.GetDirectories("./Library/PackageCache/", $"com.wb.gameconfigs@*", SearchOption.AllDirectories);

            foreach (string revFolder in revFolders)
            {
                Debug.Log("New Folder: " + revFolder);

                //var files= Directory.GetFiles(revFolder,"*.cs", SearchOption.AllDirectories);

                //foreach (string file in files)
                //{
                //    Debug.Log("file: " + file);
                //}

                //һ����˵ֻ��һ���ļ���·������������������·�����ֱ��Ӧ�ͻ��˵ĵ�ַ
                Debug.Log(revFolder + "/Assets~/ExportClass/Client");
                Debug.Log(CLASSPATH);
                //�ͻ�����λ��
                //FileHelper.CleanDirectory(SettingClass.ClientClassDir);
                FileHelper.CopyDirectory(revFolder+ "/Assets~/ExportClass/Client", CLASSPATH);

                Debug.Log(revFolder + "/Assets~/ExportJson/c/1");
                Debug.Log(JSONPATH);
                //�ͻ���Jsonλ��
                //FileHelper.CleanDirectory(SettingClass.jsonDirClient);
                FileHelper.CopyDirectory(revFolder + "/Assets~/ExportJson/c/1", JSONPATH);

            }
         

        }
        
        void SetClassPathBtn_clicked()
        {
           
            string Path = EditorUtility.OpenFolderPanel("ѡ�����ļ�����·��",Application.dataPath,"");

            if (!UnityEditor.EditorUtility.DisplayDialog("����", "��ȷ��ѡ������ȷ��·���������Ƿ������", "ȷ��", "ȡ��"))
            {
                return;
            }

            //��ϵͳ�����л�ȡgit·��
            Environment.SetEnvironmentVariable(CLASSPATHNAME, Path, EnvironmentVariableTarget.User);

            //��ϵͳ�����л�ȡ˫��·��
            CLASSPATH = Environment.GetEnvironmentVariable(CLASSPATHNAME, EnvironmentVariableTarget.User);

            ClassPathLabel.text = CLASSPATH;

        }

        void SetJsonPathBtn_clicked()
        {
            string Path = EditorUtility.OpenFolderPanel("ѡ��Json�ļ�����·��", Application.dataPath, "");

            if (!UnityEditor.EditorUtility.DisplayDialog("����", "��ȷ��ѡ������ȷ��·���������Ƿ������", "ȷ��", "ȡ��"))
            {
                return;
            }

            //��ϵͳ�����л�ȡgit·��
            Environment.SetEnvironmentVariable(JSONPATHNAME, Path, EnvironmentVariableTarget.User);

            //��ϵͳ�����л�ȡ˫��·��
            JSONPATH = Environment.GetEnvironmentVariable(JSONPATHNAME, EnvironmentVariableTarget.User);

            JsonPathLabel.text = JSONPATH;
        }
    }

}
