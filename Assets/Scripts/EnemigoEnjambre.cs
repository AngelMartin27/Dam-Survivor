using UnityEngine;

public class EnemigoEnjambre : MonoBehaviour
{
    [Header("Configuración del Enjambre")]
    public GameObject prefabZangano; // El Enemigo 1 que va a spawnear
    public int cantidadAEngendrar = 10;
    public float radioDispersion = 2f;

    void Start()
    {
        // Al nacer este enemigo, crea a sus súbditos inmediatamente
        SpawnearSubditos();
    }

void SpawnearSubditos()
    {
        for (int i = 0; i < cantidadAEngendrar; i++)
        {
            // 1. Buscamos un punto aleatorio alrededor
            Vector2 puntoCircular = Random.insideUnitCircle * radioDispersion;

            // 2. Convertimos a 3D
            Vector3 posicionDesviada = new Vector3(puntoCircular.x, 0, puntoCircular.y);

            // 3. Calculamos donde aparece el zángano
            Vector3 posicionFinal = transform.position + posicionDesviada;
            
            // 4. Lo creamos
            Instantiate(prefabZangano, posicionFinal, Quaternion.identity);
        }
    }    
}
