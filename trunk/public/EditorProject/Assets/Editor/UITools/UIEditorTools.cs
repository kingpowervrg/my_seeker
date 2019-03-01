using EngineCore;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIEditorTools
{
    [MenuItem("Tools/UI/替换Tweener")]
    public static void ReplaceOldTweener()
    {/*
        Scene currentScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        GameObject[] sceneRootObjects = currentScene.GetRootGameObjects();

        for (int i = 0; i < sceneRootObjects.Length; ++i)
        {
            if (PrefabUtility.GetPrefabParent(sceneRootObjects[i]) == null)
                continue;

            GOGUI.UITweener[] oldTweeners = sceneRootObjects[i].GetComponentsInChildren<GOGUI.UITweener>(true);

            for (int j = 0; j < oldTweeners.Length; ++j)
            {
                GOGUI.UITweener oldTweener = oldTweeners[j];
                EngineCore.UITweenerBase newTweener = null;

                if (oldTweener is GOGUI.TweenAlpha)
                {
                    GOGUI.TweenAlpha alphaTweener = oldTweener as GOGUI.TweenAlpha;

                    newTweener = oldTweener.gameObject.AddComponent<EngineCore.TweenAlpha>();
                    EngineCore.TweenAlpha newTweenAlpha = newTweener as EngineCore.TweenAlpha;
                    newTweenAlpha.From = alphaTweener.from;
                    newTweenAlpha.To = alphaTweener.to;
                }
                else if (oldTweener is GOGUI.TweenPosition)
                {
                    GOGUI.TweenPosition positionTweener = oldTweener as GOGUI.TweenPosition;
                    newTweener = oldTweener.gameObject.AddComponent<EngineCore.TweenPosition>();
                    EngineCore.TweenPosition newTweenPosition = newTweener as EngineCore.TweenPosition;
                    newTweenPosition.From = positionTweener.from;
                    newTweenPosition.To = positionTweener.to;
                }
                else if (oldTweener is GOGUI.TweenScale)
                {
                    GOGUI.TweenScale scaleTweener = oldTweener as GOGUI.TweenScale;
                    newTweener = oldTweener.gameObject.AddComponent<EngineCore.TweenScale>();
                    EngineCore.TweenScale newscaleTweener = newTweener as EngineCore.TweenScale;
                    newscaleTweener.From = scaleTweener.from;
                    newscaleTweener.To = scaleTweener.to;
                }
                else if (oldTweener is GOGUI.TweenRotation)
                {
                    GOGUI.TweenRotation rotationTweener = oldTweener as GOGUI.TweenRotation;
                    newTweener = oldTweener.gameObject.AddComponent<EngineCore.TweenRotationEuler>();
                    EngineCore.TweenRotationEuler newscaleTweener = newTweener as EngineCore.TweenRotationEuler;
                    newscaleTweener.From = rotationTweener.from;
                    newscaleTweener.To = rotationTweener.to;
                }
                else if (oldTweener is GOGUI.TweenColor)
                {
                    GOGUI.TweenColor colorTweener = oldTweener as GOGUI.TweenColor;
                    newTweener = oldTweener.gameObject.AddComponent<EngineCore.TweenColor>();
                    EngineCore.TweenColor newscaleTweener = newTweener as EngineCore.TweenColor;
                    newscaleTweener.From = colorTweener.from;
                    newscaleTweener.To = colorTweener.to;
                }
                else if (oldTweener is GOGUI.TweenFillAmount)
                {
                    GOGUI.TweenFillAmount fillAmountTweener = oldTweener as GOGUI.TweenFillAmount;
                    newTweener = oldTweener.gameObject.AddComponent<EngineCore.TweenFillAmount>();
                    EngineCore.TweenFillAmount newscaleTweener = newTweener as EngineCore.TweenFillAmount;
                    newscaleTweener.From = fillAmountTweener.from;
                    newscaleTweener.To = fillAmountTweener.to;
                }
                else if (oldTweener is GOGUI.TweenSlider)
                {
                    GOGUI.TweenSlider sliderTweener = oldTweener as GOGUI.TweenSlider;
                    newTweener = oldTweener.gameObject.AddComponent<EngineCore.TweenSlider>();
                    EngineCore.TweenSlider newSliderTweener = newTweener as EngineCore.TweenSlider;
                    newSliderTweener.From = sliderTweener.from;
                    newSliderTweener.To = sliderTweener.to;
                }
                else
                {
                    Debug.Log(oldTweener.GetType().ToString());
                }


                if (newTweener)
                {
                    newTweener.Duration = oldTweener.duration;
                    newTweener.Delay = oldTweener.delay;
                    newTweener.animationCurve = oldTweener.animationCurve;

                    switch (oldTweener.style)
                    {
                        case GOGUI.UITweener.Style.OnShow:
                            newTweener.m_triggerType = EngineCore.UITweenerBase.TweenTriggerType.OnShow;
                            break;
                        case GOGUI.UITweener.Style.OnHide:
                            newTweener.m_triggerType = EngineCore.UITweenerBase.TweenTriggerType.OnHide;
                            break;
                        case GOGUI.UITweener.Style.Once:
                            newTweener.m_triggerType = EngineCore.UITweenerBase.TweenTriggerType.Manual;
                            break;
                        case GOGUI.UITweener.Style.OnPress:
                            newTweener.m_triggerType = EngineCore.UITweenerBase.TweenTriggerType.OnPressTrue;
                            break;
                        case GOGUI.UITweener.Style.PingPong:
                            newTweener.m_tweenStyle = EngineCore.UITweenerBase.TweenStyle.PingPong;
                            break;
                        case GOGUI.UITweener.Style.Loop:
                            newTweener.m_tweenStyle = EngineCore.UITweenerBase.TweenStyle.Loop;
                            break;
                    }

                    GameObject.DestroyImmediate(oldTweener);
                }
            }

            //Replace Old UIButtonScale
            GOGUI.UIButtonScale[] oldUIButtonScaleComponents = sceneRootObjects[i].GetComponentsInChildren<GOGUI.UIButtonScale>(true);
            for (int j = 0; j < oldUIButtonScaleComponents.Length; ++j)
            {
                GOGUI.UIButtonScale oldUIButtonScaleComponent = oldUIButtonScaleComponents[j];

                UIButtonEffect newButtonEffect = oldUIButtonScaleComponent.gameObject.GetOrAddComponent<UIButtonEffect>();
                newButtonEffect.InteractiveEffectType = ButtonEffectType.ScaleEffect;
                newButtonEffect.MaxSizeScale = oldUIButtonScaleComponent.maxSize;
                newButtonEffect.OnPressedScale = oldUIButtonScaleComponent.pressed;

                newButtonEffect.NormalToPressedScaleDuration = oldUIButtonScaleComponent.duration;
                newButtonEffect.PressedToMaxSizeDuration = oldUIButtonScaleComponent.PressedToMaxDuration;
                newButtonEffect.MaxSizeToNormalDuration = oldUIButtonScaleComponent.MaxToNormalDuration;

                GameObject.DestroyImmediate(oldUIButtonScaleComponent);
            }


            PrefabUtility.ReplacePrefab(sceneRootObjects[i], PrefabUtility.GetPrefabParent(sceneRootObjects[i]), ReplacePrefabOptions.ConnectToPrefab);
        }
*/

    }
}
