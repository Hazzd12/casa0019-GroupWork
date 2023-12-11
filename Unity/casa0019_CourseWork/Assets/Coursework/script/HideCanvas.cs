using UnityEngine;

public class HideCanvas : MonoBehaviour
{
    public Canvas canvas;

    void Start()
    {
        // 在Start方法中将Canvas设置为不可见
        canvas.enabled = false;
    }

    // 在其他地方根据需要调用该方法来隐藏Canvas
    public void HideUI()
    {
        canvas.enabled = false;
    }

    // 在其他地方根据需要调用该方法来显示Canvas
    public void ShowUI()
    {
        canvas.enabled = true;
    }
}