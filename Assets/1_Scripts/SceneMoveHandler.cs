using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 반드시 추가해야 함!

public class SceneMoveHandler : MonoBehaviour
{
    // 버튼의 OnClick 이벤트에서 호출할 함수 (반드시 public이어야 함)
    public void ChangeScene(string sceneName)
    {
        // 입력받은 이름의 씬으로 이동합니다.
        SceneManager.LoadScene(sceneName);
    }

    // 혹은 단순히 "다시 시작" 같은 기능을 원할 때
    public void RestartScene()
    {
        // 현재 활성화된 씬의 이름을 가져와 다시 로드합니다.
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}