using UnityEngine;

public class Hacha : MonoBehaviour
{
    [Header("Datos del Hacha Proyectil")]
    public float speed = 10f; 	    // Velocidad de movimiento
    public float tiempoVida = 5f; 	// Tiempo de vida máximo
    public int damage = 25; 	    // Daño base

    public void ConfigurarArma(int danioPorNivel)
    {
        damage = danioPorNivel; // Actualizamos el daño
    }

    void Start()
    {
        // Se autodestruye después de 'tiempoVida' si no golpea nada.
        Destroy(gameObject, tiempoVida); 
    }
    
    void Update()
    {
        // 1. Movimiento hacia adelante
        // Mover el objeto en su propia dirección frontal (transform.forward)
        transform.position += transform.forward * speed * Time.deltaTime; 
    }

    private void OnTriggerEnter(Collider other) 
    {
        // 1. Ignorar al jugador (opcional, pero buena práctica)
        if (other.CompareTag("Player"))
        {
            return;
        }

        // 2. Impacto en enemigo
        if (other.CompareTag("Enemy"))
        {
            EnemyControler enemy = other.GetComponent<EnemyControler>();
            if (enemy != null)
            {
                enemy.Recibirdano(damage);
                
                // IMPORTANTE: Destruir el proyectil después de golpear al enemigo para evitar daño repetido.
                Destroy(gameObject); 
                return; // Salir después de destruir.
            }
        }
    }
}