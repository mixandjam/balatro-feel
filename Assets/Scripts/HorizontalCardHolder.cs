using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class HorizontalCardHolder : MonoBehaviour
{

    private RectTransform rect;
    private Card[] cards;
    [SerializeField] private Card focusedCard;

    bool isCrossing = false;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        cards = GetComponentsInChildren<Card>();

        foreach (Card card in cards)
        {
            card.SelectEvent.AddListener(CardSelected);
            card.DeselectEvent.AddListener(CardDeselected);
            card.DestroyEvent.AddListener(CardDeselected);
        }

        StartCoroutine(Frame());

        IEnumerator Frame()
        {
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i].cardVisual.UpdateIndex(transform.childCount);
            }
        }
    }

    void CardSelected(Card card)
    {
        focusedCard = card;
    }

    void CardDeselected(Card card)
    {
        focusedCard.transform.DOLocalMove(Vector3.zero, .15f).SetEase(Ease.OutBack);

        rect.sizeDelta += Vector2.right;
        rect.sizeDelta -= Vector2.right;

        focusedCard = null;
    }

    void CardDestroyed(Card card)
    {
        cards = GetComponentsInChildren<Card>();

        foreach (Card c in cards)
        {
            c.SelectEvent.AddListener(CardSelected);
            c.DeselectEvent.AddListener(CardDeselected);
            c.DestroyEvent.AddListener(CardDeselected);
        }
    }

    void Update()
    {
        if (focusedCard == null)
            return;

        if (isCrossing)
            return;

        for (int i = 0; i < cards.Length; i++)
        {

            if (focusedCard.transform.position.x > cards[i].transform.position.x)
            {
                if (focusedCard.ParentIndex() < cards[i].ParentIndex())
                {
                    Swap(i);
                    break;
                }
            }

            if (focusedCard.transform.position.x < cards[i].transform.position.x)
            {
                if (focusedCard.ParentIndex() > cards[i].ParentIndex())
                {
                    Swap(i);
                    break;
                }
            }
        }
    }

    void Swap(int index)
    {
        isCrossing = true;

        Transform focusedParent = focusedCard.transform.parent;
        Transform crossedParent = cards[index].transform.parent;

        cards[index].transform.SetParent(focusedParent);
        cards[index].transform.localPosition = Vector3.zero;
        focusedCard.transform.SetParent(crossedParent);

        isCrossing = false;

        //Updated Visual Indexes
        foreach (Card card in cards)
        {
            card.cardVisual.UpdateIndex(transform.childCount);
        }
    }

}
