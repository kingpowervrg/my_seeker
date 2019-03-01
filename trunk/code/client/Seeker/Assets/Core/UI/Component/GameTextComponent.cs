using System;
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore
{
    public class GameTextComponent : GameUIComponent
    {
        protected UnityEngine.UI.Text label;
        UnityEngine.UI.LayoutElement layout;
        protected Color initialColor;

        protected override void OnInit()
        {
            base.OnInit();
            label = GetComponent<UnityEngine.UI.Text>();
            layout = GetComponent<UnityEngine.UI.LayoutElement>();
            if (label != null)
            {
                initialColor = label.color;
            }
        }

        public virtual string Text
        {
            set
            {
                if (!label || label.text == value)
                    return;
                label.text = value; 
                if (layout)
                {
                    layout.enabled = false;
                    layout.enabled = true;
                }
            }
            get { return label.text; }
        }

        public virtual Color color
        {
            set
            {
                if (!label)
                    return;
                label.color = value;
            }
            get { return label.color; }
        }

        public void ResetColor()
        {
            color = initialColor;
        }

        public int FontSize
        {
            get { return label.fontSize; }
            set { label.fontSize = value; }
        }

        public UnityEngine.UI.Text Label
        {
            get { return label; }
        }

    }

}
