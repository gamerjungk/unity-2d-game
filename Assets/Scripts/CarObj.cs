using UnityEngine;
using UnityEngine.SceneManagement;

public class CarObj : MonoBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void MoveRandomly()
    {
        float randomDistance = Random.Range(1f,2f);
        rb.MovePosition(rb.position +(Vector2)transform.right * randomDistance);
    }
    void crash()
    {
        Debug.Log("Crash!");
    }

    void OnCollisionEnter2D(Collision2D other)
    {  // ✅ 수정 (Collider2D → Collision2D)
        crash();
        Debug.Log("LoadSceneManager.Instance: " + LoadSceneManager.Instance);
        if (other.gameObject.CompareTag("Player")){
                Debug.Log("플레이어와 충돌 감지됨! Die() 실행");

                //GameManager.inst.Stop();
                SceneManager.LoadScene("GameOverScene");

    }
}


    