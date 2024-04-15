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
    private Transform handRotationChild;
    private Transform firstChild;
    private Transform secondChild;

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
    private float yOffset;

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
        firstChild = transform.GetChild(0);
        secondChild = firstChild.GetChild(0);
        handRotationChild = secondChild.GetChild(0);


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

        yOffset = positionCurve.Evaluate(parentCard.NormalizedPosition()) * positionCurveEffect;

        float test = rotationCurve.Evaluate(parentCard.NormalizedPosition());

        //Smooth Follow
        transform.position = Vector3.Lerp(transform.position, cardTransform.position + (Vector3.up * yOffset), followSpeed * Time.deltaTime);
        //Smooth Rotate
        rotationDelta = Vector3.Lerp(rotationDelta, (transform.position - cardTransform.position) * rotationAmount, rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, rotationDelta.x);
        //Tilt Logic

        //Hand Logic

        if (parentCard.isDragging)
            return;
        //handRotationChild.localEulerAngles = new Vector3(handRotationChild.localEulerAngles.x, handRotationChild.localEulerAngles.y,parentCard.isDragging ? 0 : test * -rotationCurveEffect);
        handRotationChild.localEulerAngles = new Vector3(handRotationChild.localEulerAngles.x, handRotationChild.localEulerAngles.y,test * -rotationCurveEffect);

        Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float tiltX = parentCard.isHovering ? ((offset.y*-1) * tiltAmount) : 0;
        float tiltY = parentCard.isHovering ? ((offset.x) * (tiltAmount/1.5f)) : 0;
        float tiltZ = parentCard.isDragging ? secondChild.eulerAngles.z : 0;

        float lerpX = Mathf.LerpAngle(secondChild.eulerAngles.x, tiltX, tiltSpeed * Time.deltaTime);
        float lerpY = Mathf.LerpAngle(secondChild.eulerAngles.y, tiltY, tiltSpeed * Time.deltaTime);
        float lerpZ = Mathf.LerpAngle(secondChild.eulerAngles.z,tiltZ, tiltSpeed/2 * Time.deltaTime);
        secondChild.eulerAngles = new Vector3(lerpX, lerpY, lerpZ);

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
        firstChild.DOPunchRotation(Vector3.forward * 5, .15f, 20, 1).SetId(2);
    }
    private void PointerExit(Card card)
    {
        if(EventSystem.current.currentSelectedGameObject != parentCard.gameObject)
        transform.DOScale(1, .15f).SetEase(Ease.OutBack);
    }
}
