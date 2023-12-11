using UnityEngine;


public class CustomisedText
{
    public string title;
    public float val;
    public CustomisedText(string title, float val){
        this.title = title;
        this.val = val;
    }
}
public class ValEventer : MonoBehaviour
{
    public event OnValueChangedDelegate OnValueChanged;
    public delegate void OnValueChangedDelegate(CustomisedText text);

    public void HandleValChanged(string title, float val){
        OnValueChanged?.Invoke(new CustomisedText(title,val));
    }
}
