using UnityEngine;

public class GUIFPS : MonoBehaviour
{
    private float deltaTime = 0.0f;
    private GUIStyle style;
    private Rect rect;
    private float msec;
    private float fps;
    private float worstFps = 100f;
    private string text;
    private Vector2 ScreenInfo;

    private void Awake()
    {
        Application.targetFrameRate = 144;

        int w = Screen.width, h = Screen.height;
        ScreenInfo = new Vector2(Screen.width, Screen.height);
        rect = new Rect(0, 0, w, h * 4 / 100);

        style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = Color.cyan;

    }

    private void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    private void OnGUI()//소스로 GUI 표시.
    {
        msec = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;  //초당 프레임 - 1초에

        if (fps < worstFps)  //새로운 최저 fps가 나왔다면 worstFps 바꿔줌.
            worstFps = fps;
        text = msec.ToString("F1") + "ms (" + fps.ToString("F1") + ") //worst : " + worstFps.ToString("F1") + " Resolu : " + Screen.width + "/" + Screen.height + " Ratio : ";
        GUI.Label(rect, text, style);
    }

}
