using Game;
using UnityEngine;

public class GlowEffectHelper : MonoBehaviour
{
    [SerializeField] private Bubble bubble;
    [SerializeField] private SpriteRenderer spriteRenderer;

    void Update()
    {
        spriteRenderer.color = bubble.GetColor();
    }
}
