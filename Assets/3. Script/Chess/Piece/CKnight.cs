using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CKnight : CChessman
{
    public int health = 3;
    public int CurrentHealth;
    private Rigidbody rb;
    private bool isDead = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody 참조
        CurrentHealth = health;
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
        CurrentHealth--; // 체력 1 감소

        if (CurrentHealth <= 0)
        {
            Die(collision); // 체력이 0이면 죽는 처리
        }
    }

    // 죽을 때 날아가는 연출과 파괴 처리
    private void Die(Collision collision)
    {
        isDead = true; // 이미 죽은 상태로 표시
        rb.isKinematic = false; // 물리 효과 적용

        // 마지막 충돌 방향으로 날아가는 연출
        Vector3 knockbackDirection = collision.contacts[0].normal * -1; // 충돌 방향의 반대 방향
        rb.AddForce(knockbackDirection * 500f); // 힘을 주어 날아가게 함
        Destroy(gameObject, 1f);
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
