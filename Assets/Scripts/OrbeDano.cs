using UnityEngine;

public class OrbeDano : MonoBehaviour
{
    public int cantidadDano = 20;

    private void OnTriggerEnter(Collider other)
    {
        // 1. Si "atravesamos" a un ENEMIGO
        // Nota: 'other' es directamente el Collider, no hace falta .gameObject en algunos casos,
        // pero lo usamos para acceder al Tag y al Script.
        if (other.CompareTag("Enemy"))
        {
            EnemyControler enemigoScript = other.GetComponent<EnemyControler>();

            if (enemigoScript != null)
            {
                // Llamamos a tu función pública de daño
                enemigoScript.Recibirdano(cantidadDano);
            }
        }
    }
}