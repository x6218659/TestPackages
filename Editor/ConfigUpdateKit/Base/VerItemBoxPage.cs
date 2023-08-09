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
    /// 版本Item对象
    /// </summary>
    public class VerItemBoxPage : VisualElement
    {
        //定义到库中去
        public new class UxmlFactory : UxmlFactory<VerItemBoxPage, VisualElement.UxmlTraits> { }

        //组件定义
        Label NewLabel;
        Button VerButton;
        GroupBox VerItemBox;

        [SerializeField]
        VisualTreeAsset VisualTree;

        public VerItemBoxPage()
        {
            //初始化
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="action">按钮事件</param>
        public VerItemBoxPage(string ver, Action action = null)
        {
            //初始化
            Init(ver, action);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(string ver = null, Action action = null)
        {
            // Import UXML
            VisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ConfigUpdateKit/Uxml/VerItemBox.uxml");

            //生成结构
            VisualTree.CloneTree(this);

            //查找关键对象
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
        /// 是否打开最新标记
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
        /// 清理
        /// </summary>
        public void Dispose()
        {

        }
    }

}
