using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HorizontalCardHolder : MonoBehaviour
{

    private RectTransform rect;
    private NewCard[] cards;
    [SerializeField] private NewCard focusedCard;

    public AnimationCurve handStyle;
    public float handStyleAmount = .2f;

    bool isCrossing = false;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        cards = GetComponentsInChildren<NewCard>();

        foreach (NewCard card in cards)
        {
            card.SelectEvent.AddListener(CardSelected);
            card.DeselectEvent.AddListener(CardDeselected);
        }

        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].cardVisual.UpdateIndex(transform.childCount);
            float yPosition = handStyle.Evaluate(Remap(i, 0, (float)cards.Length - 1, 0, 1));
            cards[i].transform.localPosition = new Vector3(cards[i].transform.localPosition.x, yPosition *handStyleAmount, cards[i].transform.localPosition.z);
        }
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    void CardSelected(NewCard card)
    {
        focusedCard = card;
    }

    void CardDeselected(NewCard card)
    {
        focusedCard.transform.localPosition = Vector3.zero;

        rect.sizeDelta += Vector2.right;
        rect.sizeDelta -= Vector2.right;

        focusedCard = null;
    }

    void Update()
    {
        if (focusedCard == null)
            return;

        if (!isCrossing)

            for (int i = 0; i < cards.Length; i++)
            {
                if (focusedCard.transform.position.x > cards[i].transform.position.x)
                {
                    if (focusedCard.transform.parent.GetSiblingIndex() > cards[i].transform.parent.GetSiblingIndex())
                    {
                        Swap(i);
                        break;
                    }
                }

                if (focusedCard.transform.position.x < cards[i].transform.position.x)
                {
                    if (focusedCard.transform.parent.GetSiblingIndex() < cards[i].transform.parent.GetSiblingIndex())
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
        foreach (NewCard card in cards)
        {
            card.cardVisual.UpdateIndex(transform.childCount);
        }
    }

}
