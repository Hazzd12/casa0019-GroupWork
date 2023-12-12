using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public Canvas canvas; // 引用Canvas
    public Button toggleButton; // 引用Button

    void Start()
    {
        // 添加按钮点击事件监听器
        toggleButton.onClick.AddListener(ToggleCanvas);
        
        // 默认情况下隐藏Canvas
        canvas.gameObject.SetActive(false);
    }

    void ToggleCanvas()
    {
        // 切换Canvas的显示状态
        canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
    }
}
