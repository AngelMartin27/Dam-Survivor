using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RayoDeEnergia : MonoBehaviour
{
    [Header("Configuración del Rayo")]
    [SerializeField] private float duracion = 3.0f;
    [SerializeField] private float intervaloTick = 0.25f;
    [SerializeField] private int damagePorTick = 25;

    private GameObject player;
    private List<EnemyControler> enemigosDentro = new List<EnemyControler>();

    // --- Configuración (Nivel) ---
    public void ConfigurarRayo(int nuevoDanio)
    {
        damagePorTick = nuevoDanio;
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player");

        Destroy(gameObject, duracion);

        // Arrancamos las dos rutinas: una para el movimiento y otra para el daño
        StartCoroutine(RutinaMovimiento());
        StartCoroutine(AplicarDañoPeriodico());
    }

    private IEnumerator RutinaMovimiento()
    {
        while (player != null)
        {
            // El rayo se mueve y rota para seguir al jugador
            transform.position = player.transform.position + player.transform.forward * 2f;
            transform.rotation = player.transform.rotation;

            // Espera un frame (funciona como el Update)
            yield return null;
        }
    }
    
    // --- Lógica de Daño y Trigger (Igual que antes) ---
    private void OnTriggerEnter(Collider other)
    {
        EnemyControler enemigo = other.GetComponent<EnemyControler>();
        if (enemigo != null)
        {
            enemigosDentro.Add(enemigo);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        EnemyControler enemigo = other.GetComponent<EnemyControler>();
        if (enemigo != null)
        {
            enemigosDentro.Remove(enemigo);
        }
    }

    private IEnumerator AplicarDañoPeriodico()
    {
        while (true)
        {
            for (int i = enemigosDentro.Count - 1; i >= 0; i--)
            {
                if (enemigosDentro[i] != null)
                {
                    enemigosDentro[i].Recibirdano(damagePorTick);
                }
                else
                {
                    enemigosDentro.RemoveAt(i);
                }
            }
            yield return new WaitForSeconds(intervaloTick);
        }
    }
}