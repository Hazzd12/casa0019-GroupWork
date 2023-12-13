using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public GameObject canvas; // 引用Canvas
    public Button button;
    void Start()
    {    
        button = FindObjectOfType<Button>();
        canvas.gameObject.SetActive(false);
        Debug.Log(button.gameObject.transform);
        button.onClick.AddListener(ToggleCanvas);
    }

    public void ToggleCanvas()
    {
        Debug.Log("change the pre status："+canvas.gameObject.activeSelf);
        canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
        Debug.Log("change the status："+canvas.gameObject.activeSelf);
    }

    void Update(){
        if(button == null){
            button = FindObjectOfType<Button>();
            Debug.Log(button.gameObject.transform);
            button.onClick.AddListener(ToggleCanvas);
        }
    }
}
