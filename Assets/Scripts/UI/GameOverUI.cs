using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameOverUI : BaseUI
{
    [SerializeField] private GameObject highlightBackground; // 선택된 버튼 강조 효과
    [SerializeField] private Button restartButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    [SerializeField] private Button selectedButton;
    [SerializeField] private float selectedIndex = 0;

    private void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        // 버튼 클릭 이벤트 설정
        restartButton.onClick.AddListener(RestartGame);
        settingsButton.onClick.AddListener(OpenSettings);
        exitButton.onClick.AddListener(ExitGame);

        // 기본 선택 버튼을 재시작 버튼으로 설정
        SetSelectedButton(restartButton);

        // 게임오버 UI 등장 (페이드 인)
        PlayShowAnimation();
    }

    private void RestartGame()
    {
        DOTween.KillAll();

        // 현재 씬 다시 불러오기 (타임스케일 원상복구)
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OpenSettings()
    {
        // UIManager를 통해 설정 UI 열기
        UIManager.Instance.ShowUI<SettingsUI>();
    }

    private void ExitGame()
    {
        DOTween.KillAll();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void SetSelectedButton(Button button)
    {
        selectedButton = button;
        selectedButton.Select();

        // 하이라이트 배경을 현재 버튼 위치로 부드럽게 이동
        RectTransform rectTransform = highlightBackground.GetComponent<RectTransform>();
        rectTransform.DOAnchorPosY(selectedButton.GetComponent<RectTransform>().anchoredPosition.y, 0.5f)
            .SetEase(Ease.OutExpo)
            .SetUpdate(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            switch(selectedIndex)
            {
                case 0:
                    SetSelectedButton(settingsButton);
                    selectedIndex = 1;
                    break;
                case 1:
                    SetSelectedButton(exitButton);
                    selectedIndex = 2;
                    break;
                default:
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            switch (selectedIndex)
            {
                case 1:
                    SetSelectedButton(restartButton);
                    selectedIndex = 0;
                    break;
                case 2:
                    SetSelectedButton(settingsButton);
                    selectedIndex = 1;
                    break;
                default:
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            selectedButton.onClick.Invoke(); // 엔터 키를 누르면 선택된 버튼 클릭
        }
    }
}
