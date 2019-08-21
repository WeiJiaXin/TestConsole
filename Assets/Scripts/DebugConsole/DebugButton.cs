using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lowy.DebugConsole
{
    public class DebugButton : Button, IDragHandler
    {
        private GameObject root;
        private Vector2 offset;
        private bool moved = false;
        
        protected override void Awake()
        {
            base.Awake();
            root = transform.parent.Find("Root").gameObject;
        }

        protected override void Start()
        {
            base.Start();
            onClick.AddListener(OpenWindow);
        }
        
        private void OpenWindow()
        {
            root.SetActive(!root.activeInHierarchy);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            moved = false;
            offset = transform.position - (Vector3) eventData.position;
            base.OnPointerDown(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.selectedObject != gameObject)
                return;
            moved = true;
            transform.position = eventData.position + offset;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (moved)
                return;
            base.OnPointerClick(eventData);
        }
    }
}