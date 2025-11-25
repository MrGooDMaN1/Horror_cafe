using UnityEngine;

public class CoffeeMachine : MonoBehaviour
{
    //[SerializeField] private AudioClip fillSound;
    [SerializeField] private Transform cupHoldPoint;
    [SerializeField] private float interactionDistance = 2f;

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
        if (putDownItem.CurrentItemType == ItemType.EmptyCup && !putDownItem.IsInCoffeeMachine)
        {
            float distance = Vector3.Distance(transform.position, putDownItem.transform.position);

            if (distance < interactionDistance)
            {
                putDownItem.PlaceInCoffeeMachine(cupHoldPoint);
                //AudioSource.PlayClipAtPoint(fillSound, transform.position);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);

        if (cupHoldPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(cupHoldPoint.position, Vector3.one * 0.1f);
        }
    }
}