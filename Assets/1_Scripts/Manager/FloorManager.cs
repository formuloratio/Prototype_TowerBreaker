using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class FloorManager : MonoBehaviour
{
    [Header("층 세팅")]
    public List<GameObject> activeFloors;
    public float floorHeight = 8f;

    [Header("타임라인 세팅")]
    public PlayableDirector timelineDirector;

    private void OnEnable()
    {
        if (timelineDirector != null)
        {
            timelineDirector.playOnAwake = false; // 자동 재생 방지
            timelineDirector.time = 0;            // 재생 위치를 처음으로 리셋

            StartCoroutine(FloorTransitionRoutine());
        }
    }

    private IEnumerator FloorTransitionRoutine()
    {
        timelineDirector.Play();

        // 타임라인 종료 0.1초 전까지 대기 계산
        float totalDuration = (float)timelineDirector.duration;
        float waitTime = totalDuration - 0.1f;
        if (waitTime < 0) waitTime = 0;

        yield return new WaitForSeconds(waitTime);

        MoveAndResetFloor();

        yield return new WaitForSeconds(0.1f);

        timelineDirector.Stop();

        Debug.Log($"스테이지 {GameManager.Instance.currentStage} 세팅 완료. 매니저를 비활성화합니다.");

        // 매니저 오브젝트를 스스로 비활성화
        this.gameObject.SetActive(false);
    }

    private void MoveAndResetFloor()
    {
        GameObject bottomFloor = activeFloors[0];
        activeFloors.RemoveAt(0);

        foreach (GameObject floor in activeFloors)
        {
            floor.transform.position -= new Vector3(0, floorHeight, 0);
        }

        Vector3 newPosition = activeFloors[activeFloors.Count - 1].transform.position;
        newPosition.y += floorHeight;
        bottomFloor.transform.position = newPosition;

        // 층 초기화 (이 과정에서 새로운 적 스폰)
        bottomFloor.SetActive(false);
        bottomFloor.SetActive(true);

        activeFloors.Add(bottomFloor);
    }
}