using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace WB.ConfigUpdate
{
    /// <summary>
    /// �汾Item����
    /// </summary>
    public class VerItemBoxPage : VisualElement
    {
        //���嵽����ȥ
        public new class UxmlFactory : UxmlFactory<VerItemBoxPage, VisualElement.UxmlTraits> { }

        //�������
        Label NewLabel;
        Button VerButton;
        GroupBox VerItemBox;

        [SerializeField]
        VisualTreeAsset VisualTree;

        public VerItemBoxPage()
        {
            //��ʼ��
            Init();
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="action">��ť�¼�</param>
        public VerItemBoxPage(string ver, Action action = null)
        {
            //��ʼ��
            Init(ver, action);
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        public void Init(string ver = null, Action action = null)
        {
            // Import UXML
            VisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ConfigUpdateKit/Uxml/VerItemBox.uxml");

            //���ɽṹ
            VisualTree.CloneTree(this);

            //���ҹؼ�����
            NewLabel = this.Q<Label>("NewLabel");
            VerButton = this.Q<Button>("VerBtn");
            VerItemBox = this.Q<GroupBox>("VerItemBox");

            if (action == null || ver == null)
            {
                return;
            }
            VerButton.text = ver;
            VerButton.clicked += action;
        }

        /// <summary>
        /// �Ƿ�����±��
        /// </summary>
        /// <param name="state"></param>
        public void SetNewLabelState(bool state)
        {
            if (state)
            {
                NewLabel.visible = true;
                return;
            }

            NewLabel.visible = false;
        }

        /// <summary>
        /// ����
        /// </summary>
        public void Dispose()
        {

        }
    }

}
