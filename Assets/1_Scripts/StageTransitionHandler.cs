using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StageTransitionHandler : MonoBehaviour
{
    [Header("연결 설정")]
    [SerializeField] private GameObject objectFreezing; // 1초 동안 켜질 하위 오브젝트
    [SerializeField] private Button actionButton; // 누르면 자신을 끌 버튼

    private void OnEnable()
    {
        // 초기 상태 설정
        if (objectFreezing != null)
        {
            objectFreezing.SetActive(true);
            StartCoroutine(DisableObjectARoutine());
        }

        // 버튼 리스너 등록
        if (actionButton != null)
        {
            actionButton.onClick.AddListener(OnButtonClick);
        }
    }

    private void OnDisable()
    {
        // 메모리 누수 방지
        if (actionButton != null)
        {
            actionButton.onClick.RemoveListener(OnButtonClick);
        }
    }

    private IEnumerator DisableObjectARoutine()
    {
        yield return new WaitForSeconds(1.0f);

        if (objectFreezing != null)
        {
            objectFreezing.SetActive(false);
        }
    }

    private void OnButtonClick()
    {
        Debug.Log("버튼 클릭됨: 전환 오브젝트를 비활성화합니다.");
        this.gameObject.SetActive(false);
    }
}