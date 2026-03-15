using System.Collections;
using UnityEngine;
using UnityEngine.UI; // 버튼 컴포넌트 사용을 위해 필요

public class StageTransitionHandler : MonoBehaviour
{
    [Header("연결 설정")]
    [SerializeField] private GameObject objectFreezing;      // 1초 동안 켜질 하위 오브젝트
    [SerializeField] private Button actionButton;    // 누르면 자신을 끌 버튼

    private void OnEnable()
    {
        // 1. 초기 상태 설정
        if (objectFreezing != null)
        {
            objectFreezing.SetActive(true);
            StartCoroutine(DisableObjectARoutine());
        }

        // 2. 버튼 리스너 등록
        if (actionButton != null)
        {
            actionButton.onClick.AddListener(OnButtonClick);
        }
    }

    private void OnDisable()
    {
        // ⭐ 중요: 오브젝트가 꺼질 때 리스너를 제거하여 메모리 누수 방지
        if (actionButton != null)
        {
            actionButton.onClick.RemoveListener(OnButtonClick);
        }
    }

    // 1초 뒤에 A 오브젝트를 끄는 코루틴
    private IEnumerator DisableObjectARoutine()
    {
        yield return new WaitForSeconds(1.0f);

        if (objectFreezing != null)
        {
            objectFreezing.SetActive(false);
        }
    }

    // 버튼이 눌렸을 때 실행될 함수
    private void OnButtonClick()
    {
        Debug.Log("버튼 클릭됨: 전환 오브젝트를 비활성화합니다.");

        // 자신(상위 오브젝트)을 비활성화
        this.gameObject.SetActive(false);
    }
}