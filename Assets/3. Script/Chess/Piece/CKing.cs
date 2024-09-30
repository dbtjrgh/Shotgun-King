using System.Collections;
using UnityEngine;

public class CKing : CChessman
{
    #region 변수
    public int health = 8; // 킹의 체력
    public int currentHealth; // 킹의 현재 체력
    private Rigidbody rb; // 킹의 Rigidbody 참조
    private CCameraTransView cameraTransView;
    private bool isDead = false;
    public GameObject kingStatus;
    public GameObject chessHp;
    public GameObject heartPrefab;
    public GameObject emptyHeartPrefab;
    private CBoardManager boardManager;
    #endregion


    private void Awake()
    {
        cameraTransView = FindObjectOfType<CCameraTransView>();
        damagePool = FindObjectOfType<CUIDamagePool>(); // 데미지 풀 찾기
        boardManager = FindObjectOfType<CBoardManager>();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody 참조
        currentHealth = health;
        if(kingStatus != null)
        {
            kingStatus.SetActive(false);
        }
        if(isWhite)
        {
            UpdateHealthUI();
        }
    }
    private void UpdateHealthUI()
    {
        foreach (Transform child in chessHp.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < currentHealth; i++)
        {
            Instantiate(heartPrefab, chessHp.transform);
        }

        for (int i = 0; i < (health - currentHealth); i++)
        {
            Instantiate(emptyHeartPrefab, chessHp.transform);
        }
    }
    private void OnMouseEnter()
    {
        if (isWhite)
        {
           CChessUIManager.instance.ShowUI(kingStatus);
        }
    }

    private void OnMouseExit()
    {
        if(isWhite)
        {
            CChessUIManager.instance.HideUI(kingStatus);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Projectile 레이어에 속한 오브젝트와 충돌했을 때
        if (collision.gameObject.layer == LayerMask.NameToLayer("Projectile") && !isDead)
        {
            TakeDamage(collision); // 데미지 처리 함수 호출
        }
    }

    // 데미지 처리
    private void TakeDamage(Collision collision)
    {
        GameObject damageUI = damagePool.GetObject();
        if (damagePool != null)
        {
            CUIDamageText damageText = damageUI.GetComponent<CUIDamageText>();
            damageText.Initialize(transform, Vector3.up, damagePool);
        }
        currentHealth--; // 체력 1 감소
        if(isWhite)
        {
            UpdateHealthUI();
        }
        if (currentHealth <= 0)
        {
            StartCoroutine(Die(collision)); // 체력이 0이면 죽는 처리
        }
    }

    // 죽을 때 날아가는 연출과 파괴 처리
    private IEnumerator Die(Collision collision)
    {
        // 백색 킹이 죽었다면 다음 스테이지로 구현


        isDead = true; // 이미 죽은 상태로 표시
        rb.isKinematic = false; // 물리 효과 적용

        // 마지막 충돌 방향으로 날아가는 연출
        Vector3 knockbackDirection = collision.relativeVelocity.normalized; // 총알이 날아온 방향
        rb.AddForce(knockbackDirection * 50f, ForceMode.Impulse); // 힘을 가해 날아가게 함

        yield return new WaitForSeconds(5f); // 5초 대기 후
        Destroy(gameObject, 1.5f);
        boardManager.stageFloor += 1;
        CBoardManager.instance.EndGame(); // 체력이 0이 되면 게임 종료 호출
    }

    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];

        CChessman c;
        int i, j;

        // Top Side
        i = CurrentX - 1;
        j = CurrentY + 1;
        if (CurrentY != 7)
        {
            for (int k = 0; k < 3; k++)
            {
                if (i != -1 && j != 8 && i != 8)
                {
                    c = CBoardManager.instance.Chessmans[i, j];

                    if (c == null)
                    {
                        r[i, j] = true;
                    }
                    else if (isWhite != c.isWhite)
                    {
                        r[i, j] = true;
                    }
                }
                i++;
            }
        }

        // Down Side
        i = CurrentX - 1;
        j = CurrentY - 1;
        if (CurrentY != 0)
        {
            for (int k = 0; k < 3; k++)
            {
                if (i != -1 && j != -1 && i != 8)
                {
                    c = CBoardManager.instance.Chessmans[i, j];
                    if (c == null)
                    {
                        r[i, j] = true;
                    }
                    else if (isWhite != c.isWhite)
                    {
                        r[i, j] = true;
                    }
                }
                i++;
            }
        }

        // Middle Left
        if (CurrentX - 1 != -1)
        {
            c = CBoardManager.instance.Chessmans[CurrentX - 1, CurrentY];
            if (c == null)
            {
                r[CurrentX - 1, CurrentY] = true;
            }
            else if (isWhite != c.isWhite)
            {
                r[CurrentX - 1, CurrentY] = true;
            }
        }

        // Middle Right
        if (CurrentX + 1 != 8)
        {
            c = CBoardManager.instance.Chessmans[CurrentX + 1, CurrentY];
            if (c == null)
            {
                r[CurrentX + 1, CurrentY] = true;
            }
            else if (isWhite != c.isWhite)
            {
                r[CurrentX + 1, CurrentY] = true;
            }
        }

        return r;
    }
}
