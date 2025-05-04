using UnityEngine;

public class FireBehaviour : MonoBehaviour
{
    public int health = 1000;

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Extinguish();
        }
    }

    private void Extinguish()
    {
        Destroy(gameObject);
    }
}
