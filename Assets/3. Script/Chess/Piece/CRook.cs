using I18N.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRook : CChessman
{
    public int health = 5;
    public int CurrentHealth;
    private Rigidbody rb;
    private bool isDead = false;
    public GameObject rookUI;
    public GameObject ChessHp;              // Reference to the ChessHp container with GridLayoutGroup
    public GameObject coloredHeartPrefab;   // Prefab for colored heart (full health)
    public GameObject emptyHeartPrefab;

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody 참조
        CurrentHealth = health;
        if (rookUI != null)
        {
            rookUI.SetActive(false);  // Make sure the UI is initially hidden
        }
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        // Clear any existing hearts in the ChessHp container
        foreach (Transform child in ChessHp.transform)
        {
            Destroy(child.gameObject);
        }

        // Add colored hearts for current health
        for (int i = 0; i < CurrentHealth; i++)
        {
            Instantiate(coloredHeartPrefab, ChessHp.transform);  // Create colored heart
        }

        // Add empty hearts for lost health
        for (int i = 0; i < (health - CurrentHealth); i++)
        {
            Instantiate(emptyHeartPrefab, ChessHp.transform);  // Create empty heart
        }
    }
    private void OnMouseEnter()
    {
        CChessUIManager.instance.ShowUI(rookUI);
    }

    private void OnMouseExit()
    {
        CChessUIManager.instance.HideUI(rookUI);
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
        UpdateHealthUI();

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
        CChessman c;
        int i;

        // Right
        i = CurrentX;
        while (true)
        {
            i++;
            if (i >= 8)
                break;

            c = CBoardManager.instance.Chessmans[i, CurrentY];
            if (c == null)
            {
                r[i, CurrentY] = true;  // 빈 칸
            }
            else
            {
                if (isWhite != c.isWhite) // 다른 팀의 말일 때
                {
                    r[i, CurrentY] = true;
                }
                break; // 말이 있으면 경로 차단
            }
        }

        // Left
        i = CurrentX;
        while (true)
        {
            i--;
            if (i < 0)
                break;

            c = CBoardManager.instance.Chessmans[i, CurrentY];
            if (c == null)
            {
                r[i, CurrentY] = true;
            }
            else
            {
                if (isWhite != c.isWhite)
                {
                    r[i, CurrentY] = true;
                }
                break;
            }
        }

        // Up
        i = CurrentY;
        while (true)
        {
            i++;
            if (i >= 8)
                break;

            c = CBoardManager.instance.Chessmans[CurrentX, i];
            if (c == null)
            {
                r[CurrentX, i] = true;
            }
            else
            {
                if (isWhite != c.isWhite)
                {
                    r[CurrentX, i] = true;
                }
                break;
            }
        }

        // Down
        i = CurrentY;
        while (true)
        {
            i--;
            if (i < 0)
                break;

            c = CBoardManager.instance.Chessmans[CurrentX, i];
            if (c == null)
            {
                r[CurrentX, i] = true;
            }
            else
            {
                if (isWhite != c.isWhite)
                {
                    r[CurrentX, i] = true;
                }
                break;
            }
        }

        return r;
    }
}

