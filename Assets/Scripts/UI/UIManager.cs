using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.UI;
// A singleton class that manages all UI Events
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public UnityEvent OnShowTurnText = new UnityEvent();
    public TextIndicator turnIndicator;
    public TextIndicator messageIndicator;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    public void ShowMessage(string text)
    {
        messageIndicator.AnimateText(text);
    }
    public void ShowTurnText()
    {
        OnShowTurnText?.Invoke();
        turnIndicator.AnimateText("Your Turn");
    }

}
