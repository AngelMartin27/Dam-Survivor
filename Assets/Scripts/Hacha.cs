using UnityEngine;

public class Hacha : MonoBehaviour
{
    [Header("Datos del Hacha")]
    public float speed = 10f;  // Velocidad de movimiento
    public float tiempoVida = 5f; // Tiempo de vida
    public int damage = 25; // Daño

    public void ConfigurarArma(int danioPorNivel)
    {
        damage = danioPorNivel; // Actualizamos el daño
    }

    void Start()
    {
        Destroy(gameObject, tiempoVida); // Destruimos el objeto después de un tiempo
    }
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime; // Mover el objeto hacia adelante
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Enemy"))
        {
            EnemyControler enemy = other.GetComponent<EnemyControler>();
            if (enemy != null)
            {
                enemy.Recibirdano(damage);
            }
        }
    }
  
}