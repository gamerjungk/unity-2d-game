using UnityEngine;
using TMPro;
using System.Collections;

public class PedestrianChat : MonoBehaviour
{
    [Header("대화창 표시 정보")]
    public Sprite genericPortrait; // 모든 보행자가 공통으로 사용할 초상화
    public string genericName = "보행자"; // 공통으로 사용할 이름

    [Header("랜덤 대사 목록")]
    public string[] dialogueOptions = new string[3]; // 랜덤으로 표시할 대사 목록

    private float displayTime = 2.0f;
    private Coroutine runningCoroutine = null;

    private bool canShowDialogue = true;

    private void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트가 "Pedestrian" 태그를 가졌는지 확인
        if (collision.gameObject.CompareTag("Pedestrian"))
        {
            // 이전에 실행되던 코루틴이 있다면 중지
            if (runningCoroutine != null)
            {
                StopCoroutine(runningCoroutine);
            }

            // 새로운 코루틴 시작
            runningCoroutine = StartCoroutine(ShowRandomDialogue());
        }
    }

    IEnumerator ShowRandomDialogue()
    {
        canShowDialogue = false;
        // 대사 목록이 비어있지 않다면
        if (dialogueOptions.Length > 0)
        {
            // 0부터 대사 목록의 개수 -1 사이의 랜덤한 숫자 선택
            int randomIndex = Random.Range(0, dialogueOptions.Length);

            // 랜덤하게 선택된 대사를 변수에 저장
            string randomMessage = dialogueOptions[randomIndex];

            // DialogueUI를 호출해 대화창 표시
            DialogueUI.instance.ShowDialogue(genericName, genericPortrait, randomMessage);

            // 2초 기다리기
            yield return new WaitForSeconds(displayTime);

            // DialogueUI를 호출해 대화창 숨기기
            DialogueUI.instance.HideDialogue();
        }

        yield return new WaitForSeconds(1.0f); 

        canShowDialogue = true;
    }
}