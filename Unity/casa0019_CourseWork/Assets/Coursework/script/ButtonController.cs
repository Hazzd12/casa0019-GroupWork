using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public GameObject canvas; // 引用Canvas
    void Start()
    {
        Debug.Log("Start method is called.");
        // 默认情况下隐藏Canvas
        canvas.gameObject.SetActive(false);
    }

    public void ToggleCanvas()
    {
        Debug.Log("TTTTTTTTTTTTTTTTTTTT");
        // 切换Canvas的显示状态
        canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
    }
}
