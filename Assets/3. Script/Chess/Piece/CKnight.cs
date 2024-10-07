using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CKnight : CChessman
{
    #region 변수
    public int health = 3;
    public int currentHealth;
    private Rigidbody rb;
    private bool isDead = false;
    public GameObject knightStatus;
    public GameObject chessHp;
    public GameObject heartPrefab;
    public GameObject emptyHeartPrefab;
    #endregion
    private void Awake()
    {
        damagePool = FindObjectOfType<CUIDamagePool>(); // 데미지 풀 찾기
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody 참조
        currentHealth = health;
        if (knightStatus != null)
        {
            knightStatus.SetActive(false);
        }
        UpdateHealthUI();
        cameraTransView = FindObjectOfType<CCameraTransView>();
    }

    private void Update()
    {
        if (cameraTransView == null)
        {
            return;
        }
        if (!cameraTransView.isInTopView)
        {
            Vector3 targetPosition = cameraTransView.playerCinemachine.transform.position;
            targetPosition.y = transform.position.y;  // y축은 고정된 상태로 LookAt 적용

            transform.LookAt(targetPosition);
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
        CChessUIManager.instance.ShowUI(knightStatus);
    }

    private void OnMouseExit()
    {
        CChessUIManager.instance.HideUI(knightStatus);
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
        UpdateHealthUI();
        if (currentHealth <= 0)
        {
            Die(collision); // 체력이 0이면 죽는 처리
        }
    }

    // 죽을 때 날아가는 연출과 파괴 처리
    private void Die(Collision collision)
    {
        CSoundManager.Instance.PlaySfx(8);
        isDead = true; // 이미 죽은 상태로 표시
        rb.isKinematic = false; // 물리 효과 적용

        // 충돌한 총알의 방향과 속도를 기반으로 날아가는 연출
        Vector3 knockbackDirection = collision.relativeVelocity.normalized; // 총알이 날아온 방향
        rb.AddForce(knockbackDirection * 50f, ForceMode.Impulse); // 힘을 가해 날아가게 함

        Destroy(gameObject, 1.5f); // 1초 후 오브젝트 파괴
    }
    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];

        // UpLeft
        KnightMove(CurrentX - 1, CurrentY + 2, ref r);
        // UpRight
        KnightMove(CurrentX + 1, CurrentY + 2, ref r);

        // RightUp
        KnightMove(CurrentX + 2, CurrentY + 1, ref r);
        // RightDown
        KnightMove(CurrentX + 2, CurrentY - 1, ref r);

        // DownLeft
        KnightMove(CurrentX - 1, CurrentY - 2, ref r);
        // DownRight
        KnightMove(CurrentX + 1, CurrentY - 2, ref r);

        // LeftUp
        KnightMove(CurrentX - 2, CurrentY + 1, ref r);
        // LeftDown
        KnightMove(CurrentX - 2, CurrentY - 1, ref r);
        return r;
    }

    public void KnightMove(int x, int y, ref bool[,] r)
    {
        CChessman c;
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            c = CBoardManager.instance.Chessmans[x, y];
            if (c == null)
            {
                r[x, y] = true;
            }
            else if (isWhite != c.isWhite)
            {
                r[x, y] = true;
            }
        }
    }

}
