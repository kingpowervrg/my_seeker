namespace EngineCore
{
    enum PanelExpandDirection
    {
        Left,
        Right,
        Up,
        Down,
    }
    class GamePanelExpander : GameUIComponent
    {
        TweenScale expandAnim, collapseAnim;
        bool initialized = false;
        public bool Expanded { get; set; }
        public PanelExpandDirection Direction
        {
            set
            {
                if (initialized)
                    return;
                initialized = true;
                TweenScale[] tween = gameObject.GetComponents<TweenScale>();
                switch (value)
                {
                    case PanelExpandDirection.Left:
                        if (tween[0].From.x < tween[1].From.x)
                        {
                            expandAnim = tween[1];
                            collapseAnim = tween[0];
                        }
                        else
                        {
                            expandAnim = tween[0];
                            collapseAnim = tween[1];
                        }
                        break;
                    case PanelExpandDirection.Right:
                        if (tween[0].From.x < tween[1].From.x)
                        {
                            expandAnim = tween[0];
                            collapseAnim = tween[1];
                        }
                        else
                        {
                            expandAnim = tween[1];
                            collapseAnim = tween[0];
                        }
                        break;
                    case PanelExpandDirection.Up:
                        if (tween[0].From.y < tween[1].From.y)
                        {
                            expandAnim = tween[0];
                            collapseAnim = tween[1];
                        }
                        else
                        {
                            expandAnim = tween[1];
                            collapseAnim = tween[0];
                        }
                        break;
                    case PanelExpandDirection.Down:
                        if (tween[0].From.y < tween[1].From.y)
                        {
                            expandAnim = tween[1];
                            collapseAnim = tween[0];
                        }
                        else
                        {
                            expandAnim = tween[0];
                            collapseAnim = tween[1];
                        }
                        break;
                }

                if (Widget.anchoredPosition3D == expandAnim.From)
                    Expanded = false;
                else
                    Expanded = true;
            }
        }

        protected override void OnInit()
        {
            base.OnInit();
        }

        public void Expand()
        {
            Expanded = true;
            //expandAnim.ResetAndPlay();
        }

        public void Collapse()
        {
            Expanded = false;
            //collapseAnim.ResetAndPlay();
        }

        public void Toggle()
        {
            if (Expanded)
                Collapse();
            else
                Expand();
        }
    }

}
