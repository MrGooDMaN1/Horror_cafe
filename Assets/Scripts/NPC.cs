using UnityEngine;

public class NPC : MonoBehaviour
{
    //[SerializeField] private AudioClip thanksSound;

    private bool wantsCoffee = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (!wantsCoffee) return;

        Take coffeeItem = collision.gameObject.GetComponent<Take>();
        if (coffeeItem != null && coffeeItem.CurrentItemType == ItemType.CompletedCoffee)
        {
            ReceiveCoffee(coffeeItem);
        }
    }

    private void ReceiveCoffee(Take coffeeItem)
    {
        wantsCoffee = false;

        // Звук и визуальный эффект
        //AudioSource.PlayClipAtPoint(thanksSound, transform.position);
        Debug.Log("NPC: Ура! Кофе!");

        // Можно добавить анимацию
        //GetComponent<Animator>()?.SetTrigger("Happy");

        // Уничтожаем кофе
        Destroy(coffeeItem.gameObject);

        // Через время снова хотим кофе
        //Invoke(nameof(ResetCoffeeWant), 5f);
    }

    private void ResetCoffeeWant()
    {
        wantsCoffee = true;
        Debug.Log("NPC: Давайте еще кофе!");
    }
}