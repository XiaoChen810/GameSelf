using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{
    public Button ExitBtn;
    public Button RestartBtn;
    public Button ContinueBtn;

    private void Start()
    {
        ExitBtn.onClick.RemoveAllListeners();
        ContinueBtn.onClick.RemoveAllListeners();
        RestartBtn.onClick.RemoveAllListeners();

        ExitBtn.onClick.AddListener(GameManager.Instance.OnExit);
        ContinueBtn.onClick.AddListener(GameManager.Instance.OnContinue);
        RestartBtn.onClick.AddListener(GameManager.Instance.OnRestart);
    }
}
