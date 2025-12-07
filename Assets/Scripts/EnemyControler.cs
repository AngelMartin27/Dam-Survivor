using UnityEngine;

public class EnemyControler : MonoBehaviour
{
    // ---------------------- REFERENCIAS Y STATS ----------------------
    [Header("Configuración Principal")]
    public EnemyStats Stats; // Tu ScriptableObject
    private Rigidbody rb;
    private GameObject player;

    // Variables internas de stats
    private int maxHP;
    private int currentHP;
    private int damage;
    private int defense;
    private float speed;
    private float velocidadOriginal;

    [Header("Combate")]
    public float cooldownAtaque = 1.0f;
    private float tiempoSiguienteAtaque = 0f;

    // ---------------------- LOOT (RECOMPENSAS) ----------------------
    [Header("Configuración de EXP")]
    public int CantidadExp = 1; // Cuántos orbes suelta al morir

    [Header("Referencias de Orbes")]
    public GameObject orbeVerde; // Común (Lo que sobra)
    public GameObject orbeAzul;  // Raro
    public GameObject orbeDorado;// Legendario

    [Header("Probabilidades (%)")]
    
    [Range(0, 100)] public float chanceDorado = 5f; 
    [Range(0, 100)] public float chanceAzul = 20f;  

    // ---------------------- INICIO ----------------------
    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (Stats != null)
        {
            maxHP = Stats.MaxHP;
            currentHP = maxHP;
            damage = Stats.Damage;
            defense = Stats.Defense;
            speed = Stats.Speed;
            velocidadOriginal = speed;
        }
        else
        {
            Debug.LogError("¡Falta asignar el archivo Stats en el enemigo: " + gameObject.name + "!");
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // ---------------------- MOVIMIENTO ----------------------
    void FixedUpdate()
    {
        if (player != null)
        {
            Vector3 direccion = (player.transform.position - transform.position).normalized;
            Vector3 nuevaPosicion = transform.position + (direccion * speed * Time.fixedDeltaTime);
            
            rb.MovePosition(nuevaPosicion);
            transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
        }
    }

    // ---------------------- COMBATE ----------------------
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= tiempoSiguienteAtaque)
            {
                // Usamos TryGetComponent que es más seguro y eficiente
                if (collision.gameObject.TryGetComponent(out PlayerStats playerScript))
                {
                    playerScript.RecibirDmg(damage); 
                    tiempoSiguienteAtaque = Time.time + cooldownAtaque; 
                }
            }
        }
    }

    public void Recibirdano(int danio)
    {
        int danioFinal = Mathf.Max(0, danio - defense); // Evita curar si la defensa es alta
        currentHP -= danioFinal;

        if (currentHP <= 0)
        {
            Morir();
        }
    }

    // ---------------------- LÓGICA DE DROP (MÉTODO RESTA) ----------------------
    private void Morir()
    {
        // Repetimos el proceso según la cantidad de EXP que deba soltar
        for (int i = 0; i < CantidadExp; i++)
        {
            GenerarLootIndividual();
        }

        Destroy(gameObject);
    }

    private void GenerarLootIndividual()
    {
        // 1. Tiramos el dado (0 a 100)
        float dado = Random.Range(0f, 100f);

        // --- INTENTO DORADO ---
        // Comparamos directamente con tu variable.
        // Ejemplo: Si chanceDorado es 5, y dado sale 3 -> Entra.
        if (dado < chanceDorado)
        {
            CrearOrbe(orbeDorado);
            return; // Ya soltó el orbe, terminamos esta función.
        }

        // Si no salió Dorado, restamos su probabilidad al dado para "resetear" la regla
        dado = dado - chanceDorado;

        // --- INTENTO AZUL ---
        // Ahora comparamos directamente con tu variable Azul.
        // Ejemplo: Si chanceAzul es 20, y el dado (ya restado) es 15 -> Entra.
        if (dado < chanceAzul)
        {
            CrearOrbe(orbeAzul);
            return; // Ya soltó el orbe, terminamos.
        }

        // --- PASO 3: RELLENO (VERDE) ---
        // Si no fue ni Dorado ni Azul, automáticamente es Verde.
        CrearOrbe(orbeVerde);
    }

    // Función auxiliar para instanciar limpio
    private void CrearOrbe(GameObject prefab)
    {
        if (prefab != null)
        {
            // Posición aleatoria pequeña para que no salgan todos en el mismo punto exacto
            Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), 0.5f, Random.Range(-0.5f, 0.5f));
            Instantiate(prefab, transform.position + offset, Quaternion.identity);
        }
    }

    // ---------------------- UTILIDADES ----------------------
    public void Ralentizar(float porcentaje)
    {
        speed = velocidadOriginal * (1.0f - porcentaje);
    }

    public void RestaurarVelocidad()
    {
        speed = velocidadOriginal;
    }
}