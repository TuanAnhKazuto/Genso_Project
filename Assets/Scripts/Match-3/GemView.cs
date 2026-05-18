using System.Timers;
using Unity.Properties;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class GemView : MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public ElementType Element { get; private set; }

    private BoardManager boardManager;
    private SpriteRenderer spriteRenderer;

    public void Init(
        int x,
        int y,
        ElementType element,
        BoardManager board,
        Sprite sprite
    )
    {
        X = x;
        Y = y;
        Element = element;
        boardManager = board;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;

        name = $"Gem_{element}_{x}_{y}";
    }

    public void SetGridPosition(int x, int y)
    {
        X = x;
        Y = y;
        name = $"Gem_{Element}_{x}_{y}";
    }

    public void SetSelected(bool selected)
    {
        transform.localScale = selected ? Vector3.one * 1.12f : Vector3.one;
    }
}
