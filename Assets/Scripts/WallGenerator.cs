using UnityEngine;

public class ObjectChecker : MonoBehaviour
{
    public GameObject wallBlock_H;
    public GameObject wallBlock_V;

    void Start()
    {
        Set_Wall();
    }

    void Set_Wall()
    {
        // Hierarchy에서 모든 오브젝트를 가져오되 비활성화된 오브젝트도 포함하도록 설정
        Transform[] allTransforms = Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);


        // 가로 블록 체크
        for (int i = 1; i <= 12; i++)
        {
            string horizontalBlockName = $"Horizontal_block{i}";
            GameObject block = FindInactiveObject(horizontalBlockName, allTransforms);

            if (block == null)
            {
                Debug.Log($"Block {horizontalBlockName} is null. Creating new block.");
                Vector2 p = GetHorizontalPosition(i);
                Instantiate(wallBlock_H, p, Quaternion.identity);
            }
            else if (!block.activeSelf)
            {
                Debug.Log($"Block {horizontalBlockName} is inactive. Creating new block.");
                Vector2 p = GetHorizontalPosition(i);
                Instantiate(wallBlock_H, p, Quaternion.identity);
            }
        }

        // 세로 블록 체크
        for (int i = 1; i <= 12; i++)
        {
            string verticalBlockName = $"Vertical_block{i}";
            GameObject block = FindInactiveObject(verticalBlockName, allTransforms);

            if (block == null)
                {
                    Debug.Log($"Block {verticalBlockName} is null. Creating new block.");
                    Vector2 p = GetVerticalPosition(i);
                    Instantiate(wallBlock_V, p, Quaternion.identity);
                }
                else if (!block.activeSelf)
                {
                    Debug.Log($"Block {verticalBlockName} is inactive. Creating new block.");
                    Vector2 p = GetVerticalPosition(i);
                    Instantiate(wallBlock_V, p, Quaternion.identity);
                }
        }
    }

    // 비활성 오브젝트를 찾는 함수
GameObject FindInactiveObject(string name, Transform[] allTransforms)
{
    foreach (var t in allTransforms)
    {
        if (t.name == name)
        {
            Debug.Log($"Found object: {t.name}, activeSelf: {t.gameObject.activeSelf}, activeInHierarchy: {t.gameObject.activeInHierarchy}");
            return t.gameObject;
        }
    }
    Debug.Log($"Object not found: {name}");
    return null;
}

    // 가로 블록 위치 계산
    Vector2 GetHorizontalPosition(int i)
    {
        int row = (i - 1) / 4;
        int col = (i - 1) % 4;

        float x = -75 + col * 50;
        float y = 50 - row * 50;

        return new Vector2(x, y);
    }

    // 세로 블록 위치 계산
    Vector2 GetVerticalPosition(int i)
    {
        int row = (i - 1) / 3;
        int col = (i - 1) % 3;

        float x = -50 + col * 50;
        float y = 75 - row * 50;

        return new Vector2(x, y);
    }
}
