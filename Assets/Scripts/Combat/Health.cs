using System;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] CharacterStats stats;
    [SerializeField] int maxHp = 100;
    [SerializeField] int blinkCount = 3;
    [SerializeField] float blinkInterval = 0.05f;
    [SerializeField] Renderer[] renderers;
    [SerializeField] SpriteRenderer[] spriteRenderers;

    public int CurrentHp { get; private set; }
    public int MaxHp => maxHp;

    public event Action<int> DamageTaken;
    public event Action<int, int> HealthChanged;
    public event Action<Health> Died;

    Coroutine blinkRoutine;
    bool isDead;

    void Awake()
    {
        if (stats != null)
        {
            maxHp = stats.MaxHp;
        }

        if (renderers == null || renderers.Length == 0)
        {
            renderers = GetComponentsInChildren<Renderer>();
        }

        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }

        CurrentHp = maxHp;
        isDead = CurrentHp <= 0;
        HealthChanged?.Invoke(CurrentHp, maxHp);
    }

    public void TakeDamage(int amount)
    {
        if (CurrentHp <= 0)
        {
            return;
        }

        CurrentHp = Mathf.Max(CurrentHp - amount, 0);
        DamageTaken?.Invoke(amount);
        HealthChanged?.Invoke(CurrentHp, maxHp);
        StartBlink();

        if (!isDead && CurrentHp <= 0)
        {
            isDead = true;
            Died?.Invoke(this);
        }
    }

    void StartBlink()
    {
        if (blinkCount <= 0 || blinkInterval <= 0f)
        {
            return;
        }

        if ((renderers == null || renderers.Length == 0) && (spriteRenderers == null || spriteRenderers.Length == 0))
        {
            return;
        }

        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
        }

        blinkRoutine = StartCoroutine(BlinkRoutine());
    }

    IEnumerator BlinkRoutine()
    {
        bool visible = false;
        int toggles = blinkCount * 2;

        for (int i = 0; i < toggles; i++)
        {
            SetRenderersEnabled(visible);
            visible = !visible;
            yield return new WaitForSeconds(blinkInterval);
        }

        SetRenderersEnabled(true);
        blinkRoutine = null;
    }

    void SetRenderersEnabled(bool enabled)
    {
        if (renderers != null)
        {
            foreach (Renderer rendererComponent in renderers)
            {
                if (rendererComponent != null)
                {
                    rendererComponent.enabled = enabled;
                }
            }
        }

        if (spriteRenderers != null)
        {
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = enabled;
                }
            }
        }
    }
}
