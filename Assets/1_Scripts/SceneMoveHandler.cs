using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMoveHandler : MonoBehaviour
{
    public AudioClip confirmSfx;

    public void ChangeScene(string sceneName)
    {
        SoundEvents.NotifySfx(confirmSfx);

        // 입력받은 이름의 씬으로 이동
        SceneManager.LoadScene(sceneName);
    }

    public void RestartScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("게임 종료를 시도합니다.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}