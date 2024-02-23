using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public int curHp;
    public int maxHp;
    public int coins;
    public bool hasKey;
    public SpriteRenderer sr;
    public LayerMask moveLayerMask;

    private void Move(Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 1.0f, moveLayerMask);

        if (hit.collider == null)
        {
            transform.position += new Vector3(dir.x, dir.y, 0);
            EnemyManager.instance.OnPlayerMove();
            Generation.instance.OnPlayerMove();
        }
    }

    public void OnMoveUp (InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            Move(Vector2.up);
    }

    public void OnMoveDown(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            Move(Vector2.down);
    }

    public void OnMoveLeft(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            Move(Vector2.left);
    }

    public void OnMoveRight(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            Move(Vector2.right);
    }

    public void OnAttackUp(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            TryAttack(Vector2.up);
    }

    public void OnAttackDown(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            TryAttack(Vector2.down);
    }

    public void OnAttackLeft(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            TryAttack(Vector2.left);
    }

    public void OnAttackRight(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            TryAttack(Vector2.right);
    }

    void TryAttack(Vector2 dir)
    {                                                                       //only detect layer 9 (enemy layer)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 1.0f, 1 << 9);

        if (hit.collider != null)
            hit.transform.GetComponent<Enemy>().TakeDamage(1);
    }

    public void TakeDamage (int damageToTake)
    {
        curHp -= damageToTake;

        UI.instance.UpdateHealth(curHp);

        StartCoroutine(DamageFlash());

        if (curHp <= 0)
            SceneManager.LoadScene(0);
    }

    IEnumerator DamageFlash()
    {
        Color defaultColor = sr.color;
        sr.color = Color.white;

        yield return new WaitForSeconds(0.05f);

        sr.color = defaultColor;
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UI.instance.UpdateCoinText(coins);
    }

    public bool AddHealth(int amount)
    {
        if (curHp + amount <= maxHp)
        {
            curHp += amount;

            UI.instance.UpdateHealth(curHp);

            return true;
        }

        return false;
    }
}
