using System;
using UnityEngine;
using UnityEngine.Events;

public enum ItemType
{
    EmptyCup,
    FilledCup,
    Lid,
    CompletedCoffee
}

public class Take : MonoBehaviour
{
    [SerializeField] float _distance;
    [SerializeField] Transform _holdPoint;
    [SerializeField] private ItemType itemType;

    public static event Action<Take> OnItemPickedUp;
    public static event Action<Take> OnItemPutDown;
    public UnityEvent OnCupFilled;
    public UnityEvent OnLidAttached;

    private Rigidbody _rigidbody;
    private TakeOneItem takeOneItem;
    private Transform _originalParent;
    private bool isHeld = false;
    private bool isInCoffeeMachine = false;
    private bool isAttachedToCup = false;
    private Transform coffeeMachineHoldPoint;
    private Take attachedCup; // Стаканчик к которому прикреплена крышка

    public ItemType CurrentItemType => itemType;
    public bool IsEmptyCup { get; set; } = true;
    public bool HasLid { get; set; } = false;
    public bool IsFilled { get; set; } = false;
    public bool IsInCoffeeMachine => isInCoffeeMachine;
    public bool IsAttachedToCup => isAttachedToCup;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        takeOneItem = GameObject.Find("Player").GetComponent<TakeOneItem>();
        InitializeItem();
    }

    private void InitializeItem()
    {
        if (itemType == ItemType.EmptyCup)
        {
            IsEmptyCup = true;
            IsFilled = false;
            HasLid = false;
        }
        else if (itemType == ItemType.Lid)
        {
            HasLid = true;
        }
    }

    private void Update()
    {
        if (isHeld)
        {
            PutDown();
        }

        // Если стаканчик в кофемашине, обрабатываем специальное взаимодействие
        if (isInCoffeeMachine)
        {
            HandleCoffeeMachineInteraction();
        }
    }

    private void OnMouseDown()
    {
        if (takeOneItem.take) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, _distance))
        {
            // Если предмет прикреплен к чему-то, сначала открепляем
            if (isAttachedToCup)
            {
                TakeFromCup();
            }
            // Если стаканчик в кофемашине, забираем его оттуда
            else if (isInCoffeeMachine)
            {
                TakeFromCoffeeMachine();
            }
            else
            {
                PickUp();
            }
        }
    }

    private void PickUp()
    {
        _originalParent = transform.parent;
        _rigidbody.isKinematic = true;
        takeOneItem.take = true;
        isHeld = true;
        transform.SetParent(_holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        OnItemPickedUp?.Invoke(this);
    }

    private void PutDown()
    {
        if (Input.GetMouseButtonDown(1))
        {
            DropItem();
        }
    }

    private void DropItem()
    {
        transform.SetParent(_originalParent);
        takeOneItem.take = false;
        isHeld = false;
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
        _rigidbody.AddForce(Camera.main.transform.forward * 500);

        OnItemPutDown?.Invoke(this);
    }

    // Метод для помещения в кофемашину
    public void PlaceInCoffeeMachine(Transform machineHoldPoint)
    {
        if (itemType == ItemType.EmptyCup && !isInCoffeeMachine)
        {
            coffeeMachineHoldPoint = machineHoldPoint;
            isInCoffeeMachine = true;

            // Отключаем физику и помещаем в кофемашину
            _rigidbody.isKinematic = true;
            transform.SetParent(coffeeMachineHoldPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            // Если держали в руках - освобождаем руку
            if (isHeld)
            {
                takeOneItem.take = false;
                isHeld = false;
            }

            Debug.Log("Стаканчик помещен в кофемашину");
        }
    }

    // Метод для прикрепления крышки к стаканчику
    public void AttachToCup(Take cup, Transform lidHoldPoint)
    {
        if (itemType == ItemType.Lid && !isAttachedToCup && cup.CurrentItemType == ItemType.FilledCup && !cup.HasLid)
        {
            attachedCup = cup;
            isAttachedToCup = true;

            // Отключаем физику и прикрепляем к стаканчику
            _rigidbody.isKinematic = true;
            transform.SetParent(lidHoldPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            // Если держали в руках - освобождаем руку
            if (isHeld)
            {
                takeOneItem.take = false;
                isHeld = false;
            }

            // Обновляем состояние стаканчика
            cup.HasLid = true;
            cup.itemType = ItemType.CompletedCoffee;

            // Визуальные изменения
            transform.Find("Lid")?.gameObject.SetActive(true);
            OnLidAttached?.Invoke();

            Debug.Log("Крышка прикреплена к стаканчику!");
        }
    }

    // Метод для забора из кофемашины
    private void TakeFromCoffeeMachine()
    {
        if (isInCoffeeMachine)
        {
            isInCoffeeMachine = false;
            PickUp();
        }
    }

    // Метод для снятия крышки со стаканчика
    private void TakeFromCup()
    {
        if (isAttachedToCup && attachedCup != null)
        {
            isAttachedToCup = false;

            // Возвращаем состояние стаканчика
            attachedCup.HasLid = false;
            attachedCup.itemType = ItemType.FilledCup;

            attachedCup = null;
            PickUp(); // Забираем крышку в руку
        }
    }

    // Обработка взаимодействия с кофемашиной
    private void HandleCoffeeMachineInteraction()
    {
        // Автоматическое наполнение когда стаканчик в кофемашине
        if (!IsFilled && IsEmptyCup)
        {
            Invoke(nameof(FillCup), 1f);
        }
    }

    public void FillCup()
    {
        if (itemType == ItemType.EmptyCup && IsEmptyCup && !IsFilled)
        {
            IsEmptyCup = false;
            IsFilled = true;
            itemType = ItemType.FilledCup;

            GetComponent<Renderer>().material.color = new Color(0.4f, 0.2f, 0.1f);
            OnCupFilled?.Invoke();
            Debug.Log("Стаканчик наполнен кофе!");
        }
    }
}