using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour
{
    public Action<int, int> OnClick;
    public Action<int, int, Tile> OnWanish;
    public int SpriteIndex
    {
        get;
        private set;
    }

    private int _xPos;
    private int _yPos;
    private SpriteRenderer _spriteRenderer;
    private Board _tileBoard;
    private bool _highlighted;

    public void Initialize(int xPos, int yPos)
    {
        ChangePosition(xPos, yPos);

        SpriteIndex = UnityEngine.Random.Range(0, Database.GetDatabase().IconSprites.Length);
        _spriteRenderer.sprite = Database.GetDatabase().IconSprites[SpriteIndex];
    }

    public void InitializeWithoutRandom(int xPos, int yPos, int spriteIndex)
    {
        ChangePosition(xPos, yPos);

        SpriteIndex = spriteIndex;
        _spriteRenderer.sprite = Database.GetDatabase().IconSprites[SpriteIndex];
    }

    public void OnMatched()
    {
        _highlighted = false;
        StartCoroutine(Wanish());
    }

    public void TutorialHighlight()
    {
        _highlighted = true;
        StartCoroutine(Highlight());
    }

    public void ChangePosition(int xPos, int yPos)
    {
        if (xPos < 0 || yPos < 0 || xPos > _tileBoard.Width || yPos > _tileBoard.Height)
            throw new ArgumentOutOfRangeException();

        _xPos = xPos;
        _yPos = yPos;

        Vector2 transformPosition = new Vector2(xPos, yPos);
        transform.position = transformPosition;
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _tileBoard = GetComponentInParent<Board>();
    }

    private void OnEnable()
    {
        GameUI.OnGoToMenuModalWindowOpen += OnGoToMenuModalWindowOpen;
        GameUI.OnGoToMenuModalWindowClose += OnGoToMenuModalWindowClose;
    }

    private void OnMouseDown()
    {
        OnClick?.Invoke(_xPos, _yPos);
    }

    private void OnGoToMenuModalWindowOpen()
    {
        if (_highlighted)
            StopAllCoroutines();
    }

    private void OnGoToMenuModalWindowClose()
    {
        if (_highlighted)
            StartCoroutine(Highlight());
    }

    private IEnumerator Wanish()
    {
        Color color = _spriteRenderer.color;
        for (float alpha = 1; alpha >= 0; alpha -= 0.1f)
        {
            color.a = alpha;
            _spriteRenderer.color = color;
            yield return new WaitForSeconds(0.01f);
        }
        _spriteRenderer.sprite = null;
        color.a = 1;
        _spriteRenderer.color = color;
        OnWanish(_xPos, _yPos, this);
    }

    private IEnumerator Highlight()
    {
        while (_highlighted)
        {
            _spriteRenderer.flipX = _spriteRenderer.flipX ? false : true;
            yield return new WaitForSeconds(0.3f);
        }

        _spriteRenderer.flipX = false;
    }

    private void OnDisable()
    {
        GameUI.OnGoToMenuModalWindowOpen -= OnGoToMenuModalWindowOpen;
        GameUI.OnGoToMenuModalWindowClose -= OnGoToMenuModalWindowClose;
    }
}
