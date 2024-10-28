using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;
// A singleton class that manages all UI Events
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public UnityEvent OnShowTurnText = new UnityEvent();

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

    public void ShowTurnText()
    {
        OnShowTurnText?.Invoke();
    }

}
