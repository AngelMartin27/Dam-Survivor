using UnityEngine;
using System.Collections;

public class Hacha : MonoBehaviour
{
    [Header("Datos del Hacha Proyectil")]
    public float speed = 15f;            
    public float tiempoIda = 1.0f;     
    public int damage = 25;            


    private Transform player;
    private Rigidbody rb; // Referencia para configurarlo

    public void ConfigurarArma(int danioPorNivel, Transform playerTransform)
    {
        damage = danioPorNivel;
    }

    void Start()
    {
        ConfigurarFisicas();

        // Buscamos al Player si no lo tenemos ya
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        if (player != null)
        {
            // Orientamos el bumerán igual que el jugador
            transform.rotation = player.rotation;

            // Posicionamos un poco delante para no chocarnos con nosotros mismos
            transform.position = player.position + transform.forward + (Vector3.up * 1.0f);

            StartCoroutine(RutinaVuelo());
        }
        else
        {
            Destroy(gameObject); // Si no hay jugador, el bumerán no tiene sentido
        }
    }
    private void ConfigurarFisicas()
    {
        rb = GetComponent<Rigidbody>();
    }

    IEnumerator RutinaVuelo()
    {
        float timer = 0f;

        // --- FASE 1: IDA (Sale disparado recto) ---
        while (timer < tiempoIda)
        {
            // Movemos hacia adelante
            transform.position += transform.forward * (speed * Time.deltaTime);

            timer += Time.deltaTime;
            yield return null; 
        }
        // --- LLEGADA ---
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignoramos al jugador y otros bumeranes
        if (other.CompareTag("Player")) return;

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