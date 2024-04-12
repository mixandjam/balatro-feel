using System;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.EventSystems;
using Unity.Collections;
using UnityEngine.UI;

public class CardVisual : MonoBehaviour
{
    private bool initalize = false;
    private NewCard targetCard;
    private Transform cardTransform;
    private Vector3 rotationDelta;
    private Transform firstChild;
    private Transform secondChild;

    [Header("References")]
    public Shadow visualShadow;
    private Vector2 shadowDistance;

    [Header("Parameters")]
    [SerializeField] float followSpeed = 20;
    [SerializeField] float rotationAmount = 20;
    [SerializeField] float rotationSpeed = 20;
    [SerializeField] float tiltAmount = 20;
    [SerializeField] float tiltSpeed = 20;

    private void Start()
    {
        shadowDistance = visualShadow.effectDistance;
    }

    public void Initialize(NewCard target, int index=0)
    {
        //Declarations
        targetCard = target;
        cardTransform = target.transform;

        //Event Listening
        targetCard.SelectEvent.AddListener(Select);
        targetCard.DeselectEvent.AddListener(Deselect);
        targetCard.PointerEnterEvent.AddListener(PointerEnter);
        targetCard.PointerExitEvent.AddListener(PointerExit);

        //Initialization
        initalize = true;

        //Rotation Stuff
        firstChild = transform.GetChild(0);
        secondChild = firstChild.GetChild(0);
    }

    public void UpdateIndex(int length)
    {
        transform.SetSiblingIndex(GetInverseIndex(targetCard.transform.parent.GetSiblingIndex(), length));
    }

    // Function to calculate the inverse index
    int GetInverseIndex(int index, int length)
    {
        // Calculate the inverse index
        int inverseIndex = length - index - 1;
        return inverseIndex;
    }

    void Update()
    {
        if (!initalize) return;

        //Smooth Follow
        transform.position = Vector3.Lerp(transform.position, cardTransform.position, followSpeed * Time.deltaTime);
        //Smooth Rotate
        rotationDelta = Vector3.Lerp(rotationDelta, (transform.position - cardTransform.position) * rotationAmount, rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, rotationDelta.x);
        //Tilt Logic
        Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float tiltX = targetCard.isHovering ? (-offset.y * tiltAmount) : 0;
        float tiltY = targetCard.isHovering ? (offset.x * (tiltAmount/1.5f)) : 0;
        float tiltZ = targetCard.isDragging ? secondChild.eulerAngles.z : 0;
        float lerpX = Mathf.LerpAngle(secondChild.eulerAngles.x, tiltX, tiltSpeed * Time.deltaTime);
        float lerpY = Mathf.LerpAngle(secondChild.eulerAngles.y, tiltY, tiltSpeed * Time.deltaTime);
        float lerpZ = Mathf.LerpAngle(secondChild.eulerAngles.z,tiltZ, tiltSpeed/2 * Time.deltaTime);
        secondChild.eulerAngles = new Vector3(lerpX, lerpY, lerpZ);
    }

    private void Select(NewCard card)
    {
        GetComponent<Canvas>().overrideSorting = true;
        transform.DOScale(1.3f, .13f).SetEase(Ease.OutBack);

        visualShadow.effectDistance += (-Vector2.up * 20);
    }

    private void Deselect(NewCard card)
    {
        GetComponent<Canvas>().overrideSorting = false;
        transform.DOScale(1, .4f).SetEase(Ease.OutBack);

        visualShadow.effectDistance = shadowDistance;
    }
    private void PointerEnter(NewCard card)
    {
        transform.DOScale(1.2f, .15f).SetEase(Ease.OutBack);
        DOTween.Kill(2, true);
        firstChild.DOPunchRotation(Vector3.forward * 5, .15f, 20, 1).SetId(2);
    }
    private void PointerExit(NewCard card)
    {
        if(EventSystem.current.currentSelectedGameObject != targetCard.gameObject)
        transform.DOScale(1, .15f).SetEase(Ease.OutBack);
    }
}
