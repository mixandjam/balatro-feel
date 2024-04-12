
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NewCard : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    private Canvas canvas;

    [Header("Visual")]
    [SerializeField] private GameObject cardVisualPrefab;
    [HideInInspector] public CardVisual cardVisual;
    private VisualCardsHandler? visualHandler;

    [Header("Logic")]
    public bool isHovering;
    public bool isDragging;

    [Header("Events")]
    [HideInInspector] public UnityEvent<NewCard> SelectEvent;
    [HideInInspector] public UnityEvent<NewCard> DeselectEvent;
    [HideInInspector] public UnityEvent<NewCard> PointerEnterEvent;
    [HideInInspector] public UnityEvent<NewCard> PointerExitEvent;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        visualHandler = FindObjectOfType<VisualCardsHandler>();

        cardVisual = Instantiate(cardVisualPrefab, visualHandler ? visualHandler.transform : canvas.transform).GetComponent<CardVisual>();
        cardVisual.Initialize(this);
    }

    void Update()
    {
        ClampPosition();

        if (isDragging)
        {
            Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Calculate the direction towards the target position
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

            // Calculate the limited velocity
            Vector2 velocity = direction * Mathf.Min(30, Vector2.Distance(transform.position, targetPosition) / Time.deltaTime);

            // Update object position based on velocity
            transform.Translate(velocity * Time.deltaTime);
        }
    }

    void ClampPosition()
    {
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -screenBounds.x, screenBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -screenBounds.y, screenBounds.y);
        transform.position = clampedPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //transform.position = new Vector3(mousePos.x, mousePos.y, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
        SelectEvent.Invoke(this);
        GetComponent<Image>().raycastTarget = false;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        DeselectEvent.Invoke(this);
        GetComponent<Image>().raycastTarget = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnterEvent.Invoke(this);

        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExitEvent.Invoke(this);

        isHovering = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Release();
    }

    void Release()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
