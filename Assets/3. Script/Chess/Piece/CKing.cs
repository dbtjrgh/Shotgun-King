using System.Collections;
using UnityEngine;

public class CKing : CChessman
{
    public int health = 8; // 킹의 체력

    private Rigidbody rb; // 킹의 Rigidbody 참조
    private CCameraTransView cameraTransView;
    private bool isDead = false;

    private void Awake()
    {
        cameraTransView = FindObjectOfType<CCameraTransView>();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody 참조
    }

    private void Update()
    {
        if (!isWhite && cameraTransView.isInTopView)
        {
            return;
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
        health--; // 체력 1 감소

        if (health <= 0)
        {
            StartCoroutine(Die(collision)); // 체력이 0이면 죽는 처리
        }
    }

    // 죽을 때 날아가는 연출과 파괴 처리
    private IEnumerator Die(Collision collision)
    {
        isDead = true; // 이미 죽은 상태로 표시
        rb.isKinematic = false; // 물리 효과 적용

        // 마지막 충돌 방향으로 날아가는 연출
        Vector3 knockbackDirection = collision.contacts[0].normal * -1; // 충돌 방향의 반대 방향
        rb.AddForce(knockbackDirection * 500f); // 힘을 주어 날아가게 함

        yield return new WaitForSeconds(5f); // 5초 대기 후

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
