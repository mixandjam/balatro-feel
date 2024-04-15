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

    private Card parentCard;
    private Transform cardTransform;
    private Vector3 rotationDelta;

    //Different rotation parents
    private Transform shakeParent;
    private Transform tiltParent;
    private Transform curveParent;

    [Header("References")]
    public Shadow visualShadow;
    private Vector2 shadowDistance;

    [Header("Parameters")]
    [SerializeField] float followSpeed = 30;
    [SerializeField] float rotationAmount = 20;
    [SerializeField] float rotationSpeed = 20;
    [SerializeField] float tiltAmount = 20;
    [SerializeField] float tiltSpeed = 20;

    [Header("Curve")]
    [SerializeField] private AnimationCurve positionCurve;
    [SerializeField] private float positionCurveEffect = .1f;
    [SerializeField] private AnimationCurve rotationCurve;
    [SerializeField] private float rotationCurveEffect = 10f;

    private float curveYOffset;
    private float curveRotationOffset;

    private void Start()
    {
        shadowDistance = visualShadow.effectDistance;
    }

    public void Initialize(Card target, int index=0)
    {
        //Declarations
        parentCard = target;
        cardTransform = target.transform;

        //Event Listening
        parentCard.SelectEvent.AddListener(Select);
        parentCard.DeselectEvent.AddListener(Deselect);
        parentCard.PointerEnterEvent.AddListener(PointerEnter);
        parentCard.PointerExitEvent.AddListener(PointerExit);

        //Initialization
        initalize = true;

        //Rotation Stuff
        shakeParent = transform.GetChild(0);
        tiltParent = shakeParent.GetChild(0);
        curveParent = tiltParent.GetChild(0);


    }

    public void UpdateIndex(int length)
    {
        transform.SetSiblingIndex(GetInverseIndex(parentCard.transform.parent.GetSiblingIndex(), length));
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

        curveYOffset = positionCurve.Evaluate(parentCard.NormalizedPosition()) * positionCurveEffect;
        curveRotationOffset = rotationCurve.Evaluate(parentCard.NormalizedPosition());

        //Smooth Follow
        transform.position = Vector3.Lerp(transform.position, cardTransform.position + (Vector3.up * curveYOffset), followSpeed * Time.deltaTime);

        //Smooth Rotate
        rotationDelta = Vector3.Lerp(rotationDelta, (transform.position - cardTransform.position) * rotationAmount, rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, rotationDelta.x);

        //Hand Logic
        curveParent.localEulerAngles = new Vector3(curveParent.localEulerAngles.x, curveParent.localEulerAngles.y, Mathf.LerpAngle(curveParent.localEulerAngles.z,parentCard.isDragging ? 0 : curveRotationOffset * -rotationCurveEffect, 5*Time.deltaTime));

        //Tilt Logic
        float sine =  Mathf.Sin(Time.time + parentCard.ParentIndex());
        float cosine = Mathf.Cos(Time.time + parentCard.ParentIndex());

        Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float tiltX = parentCard.isHovering ? ((offset.y*-1) * tiltAmount) : 0;
        float tiltY = parentCard.isHovering ? ((offset.x) * (tiltAmount/1.5f)) : 0;
        float tiltZ = parentCard.isDragging ? tiltParent.eulerAngles.z : 0;

        float lerpX = Mathf.LerpAngle(tiltParent.eulerAngles.x, tiltX + (sine*15), tiltSpeed * Time.deltaTime);
        float lerpY = Mathf.LerpAngle(tiltParent.eulerAngles.y, tiltY + (cosine*15), tiltSpeed * Time.deltaTime);
        float lerpZ = Mathf.LerpAngle(tiltParent.eulerAngles.z,tiltZ, tiltSpeed/2 * Time.deltaTime);
        tiltParent.eulerAngles = new Vector3(lerpX, lerpY, lerpZ);

    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    private void Select(Card card)
    {
        GetComponent<Canvas>().overrideSorting = true;
        transform.DOScale(1.25f, .13f).SetEase(Ease.OutBack);

        visualShadow.effectDistance += (-Vector2.up * 20);
    }

    private void Deselect(Card card)
    {
        GetComponent<Canvas>().overrideSorting = false;
        transform.DOScale(1, .4f).SetEase(Ease.OutBack);

        visualShadow.effectDistance = shadowDistance;
    }
    private void PointerEnter(Card card)
    {
        transform.DOScale(1.15f, .15f).SetEase(Ease.OutBack);
        DOTween.Kill(2, true);
        shakeParent.DOPunchRotation(Vector3.forward * 5, .15f, 20, 1).SetId(2);
    }
    private void PointerExit(Card card)
    {
        if(EventSystem.current.currentSelectedGameObject != parentCard.gameObject)
        transform.DOScale(1, .15f).SetEase(Ease.OutBack);
    }
}
