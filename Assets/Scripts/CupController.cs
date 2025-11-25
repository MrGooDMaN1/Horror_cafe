using UnityEngine;

public class CupController : MonoBehaviour
{
    [SerializeField] private Transform lidHoldPoint; // Точка куда прикрепляется крышка
    [SerializeField] private float interactionDistance = 1.5f;

    private Take cupTakeComponent;

    private void Start()
    {
        cupTakeComponent = GetComponent<Take>();
    }

    private void OnEnable()
    {
        Take.OnItemPutDown += HandleItemPutDown;
    }

    private void OnDisable()
    {
        Take.OnItemPutDown -= HandleItemPutDown;
    }

    private void HandleItemPutDown(Take putDownItem)
    {
        // Когда крышку бросили, проверяем можно ли её прикрепить к стаканчику
        if (putDownItem.CurrentItemType == ItemType.Lid &&
            !putDownItem.IsAttachedToCup &&
            cupTakeComponent.CurrentItemType == ItemType.FilledCup &&
            !cupTakeComponent.HasLid)
        {
            float distance = Vector3.Distance(transform.position, putDownItem.transform.position);

            if (distance < interactionDistance)
            {
                // Прикрепляем крышку к стаканчику
                putDownItem.AttachToCup(cupTakeComponent, lidHoldPoint);
            }
        }
    }

    // Визуализация зоны взаимодействия в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);

        if (lidHoldPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(lidHoldPoint.position, Vector3.one * 0.05f);
        }
    }
}