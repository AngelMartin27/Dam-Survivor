using UnityEngine;

public class SlashAtaque : MonoBehaviour
{
    [Header("Configuración")]
    public float duracion = 0.2f;   // Duración del tajo
    public int damage = 10; // Esta es tu variable de daño

    public void ConfigurarArma(int danioPorNivel) 
    {
        damage = danioPorNivel; 
    }   

    void Start()
    {
        // El tajo desaparece rápido
        Destroy(gameObject, duracion);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemigo = other.GetComponent<EnemyControler>();
            
            if (enemigo != null)
            {
                // Usamos la variable 'damage' que ya fue actualizada por el lanzador
                enemigo.Recibirdano(damage);
            }
        }
    }
}
