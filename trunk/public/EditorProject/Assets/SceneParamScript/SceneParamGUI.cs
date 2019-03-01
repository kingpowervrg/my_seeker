using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using HedgehogTeam.EasyTouch;
using SeekerGame;
using UnityEngine.UI;

public class SceneParamGUI : MonoBehaviour {

    public Texture2D tex2d;
    public Texture2D inputTex2d;
    public Button btnBack;
    private string[] sceneName = new string[] { "法院外", "物证调查科" ,"单身公寓","女明星家"
        ,"女明星家-夜晚","医院护士站","死者病房","更衣室"
        ,"医院休息室","嫌疑人家","储藏室","神秘人家"
        ,"偷窥者家","偷窥者家-监控","温妮的车","手提箱"
        ,"报纸收发室","假面舞会现场","湖边","郊外森林-白天"
        ,"保罗工作室","郊外森林-夜晚","纳尔婶儿子房间","监控街道"
        ,"汽车修理厂","心理医生工作室","咖啡厅","便利店"
        ,"皮尔森公寓","厕所","杀手家","马路边"
        ,"厕所-男厕"};
    private string[] sceneID = new string[] { "FaYuanWai_01", "WuZhengDiaoChaKe_01", "DanShenGongYu_01", "NvMingXingJia_02"
        ,"NvMingXingJia_02-yewan","YiYuanHuShiZhan_01","SiZheBingFang_01","GengYiShi_01"
        ,"YiYuanXiuXiShi_01","XianYiRenJia_01","ChuCangShi_01","ShenMiRenJia_01"
        ,"TouKuiZheJia_01","TouKuiZheJia_02_jiankong","WenNiDeChe_01","ShouTiXiang_01"
        ,"BaoZhiShouFaShi_01","JiaMianWuHuiXianChang_01","HuBian_01","JiaoWai_SenLin_01_baitian"
        ,"BaoLuoGongZuoShi_01","JiaoWai_SenLin_01_yewan","NaErSenErZiFangJian_01","JianKongJieDao_01"
        ,"QiCheXiuLiChang_01","XinLiYiShengGongZuoShi_01","KaFeiTing_01","BianLiDian_01"
        ,"PiErSenGongYu_01","CeSuo_01","ShaShouJia_01","MaLuBian_01"
        ,"CeSuo_02_lanse"};
    GUIStyle fontStyle;
    GUIStyle inputFontStyle;

    GUIStyle labelFontStyle;
    GameSceneCameraSystem cameraSystem;
    // Use this for initialization
    void Start() {
        max = Mathf.FloorToInt(Screen.height / (btnHei + spaceHei));
        fontStyle = new GUIStyle();
        fontStyle.alignment = TextAnchor.MiddleCenter;
        fontStyle.fontSize = 30;
        fontStyle.normal.background = tex2d;

        inputFontStyle = new GUIStyle();
        inputFontStyle.alignment = TextAnchor.MiddleCenter;
        inputFontStyle.normal.textColor = Color.white;
        inputFontStyle.fontSize = 30;
        inputFontStyle.normal.background = inputTex2d;

        labelFontStyle = new GUIStyle();
        labelFontStyle.normal.textColor = Color.white;
        labelFontStyle.alignment = TextAnchor.MiddleLeft;
        labelFontStyle.fontSize = 30;

        
        //DontDestroyOnLoad(this);
    }

    private float btnWidth = 300f;
    private float btnHei = 100f;
    private float spaceWidth = 20f;
    private float spaceHei = 20f;
    private int max = 6;

    private int currentScene = -1;
    AsyncOperation operation = null;
    private bool m_isLoadScene = false;

    private string dragDeltaStr = "1";
    private string pinInDeltaStr = "1";
    private string MoveTimeStr = "0.2";
    private string easeStr = "3";
    private string zoomKickStr = "1";

    void OnGUI()
    {
        if (currentScene < 0)
        {
            for (int i = 0; i < sceneName.Length; i++)
            {
                float x = (i / max) * (btnWidth + spaceWidth);
                float y = (i % max) * (btnHei + spaceHei);
                if (GUI.Button(new Rect(x, y, btnWidth, btnHei), sceneName[i], fontStyle))
                {
                    currentScene = i;
                    operation = SceneManager.LoadSceneAsync(sceneID[i], LoadSceneMode.Additive);
                    m_isLoadScene = true;
                }
            }
        }

        if (currentScene >= 0 && !m_startDelay)
        {
            GUI.Button(new Rect((Screen.width / 2f) - 300, 0, 600, 100), "当前场景:" + sceneID[currentScene], fontStyle);
            if (GUI.Button(new Rect(Screen.width - btnWidth, 0, btnWidth, btnHei), "退出", fontStyle))
            {
                SceneManager.UnloadSceneAsync(sceneID[currentScene]);
                currentScene = -1;
            }
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUI.Label(new Rect(0, 100, 200, 100), "滑动:", labelFontStyle);
            dragDeltaStr = GUI.TextField(new Rect(200, 100,300,100), dragDeltaStr, inputFontStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.Label(new Rect(0, 220, 200, 100), "缩放:", labelFontStyle);
            pinInDeltaStr = GUI.TextField(new Rect(200, 220, 300, 100),pinInDeltaStr, inputFontStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.Label(new Rect(0, 340, 200, 100), "移动时间:", labelFontStyle);
            MoveTimeStr = GUI.TextField(new Rect(200, 340, 300, 100), MoveTimeStr, inputFontStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.Label(new Rect(0, 460, 200, 100), "曲线类型:", labelFontStyle);
            easeStr = GUI.TextField(new Rect(200, 460, 300, 100), easeStr, inputFontStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.Label(new Rect(0, 560, 200, 100), "缩放回弹:", labelFontStyle);
            zoomKickStr = GUI.TextField(new Rect(200, 560, 300, 100), zoomKickStr, inputFontStyle);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            float.TryParse(dragDeltaStr,out CameraManager.DragDelta);
            float.TryParse(pinInDeltaStr, out CameraManager.PinInDelta);
            float.TryParse(MoveTimeStr, out CameraManager.moveTime);
            int.TryParse(easeStr, out CameraManager.ease);
            float.TryParse(zoomKickStr, out CameraManager.ZoomKick);
        }

    }

    private void LoadSceneComplete()
    {
        Camera.main.gameObject.AddComponent<GameCamera_New>();
        EasyTouch.instance.touchCameras[0].camera = Camera.main;
        cameraSystem = new GameSceneCameraSystem(this.btnBack);

    }

    private float delayTime = 2f;
    private bool m_startDelay = false;
    private float timesection = 0f;
    void Update()
    {
        if (m_isLoadScene && operation != null)
        {
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
                m_isLoadScene = false;
                m_startDelay = true;
                timesection = 0f;
                
            }
        }
        if (m_startDelay)
        {
            timesection += Time.deltaTime;
            if (timesection >= delayTime)
            {
                LoadSceneComplete();
                m_startDelay = false;
            }
        }
    }

	
}
