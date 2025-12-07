using System.Collections;
using UnityEngine;

public class Lanzadordearma : ArmaBase
{
    [Header("Configuración del Spawner")]
    [SerializeField] private GameObject armaPrefab; 
    [SerializeField] private Transform puntoDeSalida; // Opcional: Para afinar desde donde sale
    [SerializeField] private float distanciaSpawn = 1f; 

    [Header("Estadísticas Base")]
    [SerializeField] private float ratioDeDisparoBase = 1f; 
    [SerializeField] private int danoBase = 25;       

    [Header("Progresión por Nivel")]
    [SerializeField] private int incrementoDanoPorNivel = 5;    
    [SerializeField] private float reduccionTiempoPorNivel = 0.05f; // Resta 0.05s cada nivel

    // --- VARIABLES INTERNAS ---
    [SerializeField] private float tiempoEntreDisparosActual;
    [SerializeField] private int danoActual;
    private bool disparando = true;

    private void Start()
    {
        // Nombre por defecto si se te olvidó ponerlo
        if(string.IsNullOrEmpty(nombreArma)) nombreArma = "Arma Lanzadora"; 
        
        // Calcular stats del Nivel 1
        ActualizarStats();

        // Arrancar la ametralladora
        StartCoroutine(RutinaDisparo());
    }

    // ---  LÓGICA MATEMÁTICA DE NIVEL ---
    public override void SubirNivel()
    {
        base.SubirNivel(); // Sube el número de nivel
        ActualizarStats(); // Recalcula el daño y la velocidad
    }

    private void ActualizarStats()
    {
        // 1. CALCULAR DAÑO
        // Fórmula: Base + (Nivel * Bonus)
        danoActual = danoBase + ((nivelActual - 1) * incrementoDanoPorNivel);

        // 2. CALCULAR VELOCIDAD
        // Fórmula: Base - (Nivel * Reducción). Usamos Mathf.Max para que nunca baje de 0.1 segundos
        float reduccionTotal = (nivelActual - 1) * reduccionTiempoPorNivel;
        tiempoEntreDisparosActual = Mathf.Max(0.1f, ratioDeDisparoBase - reduccionTotal);

        Debug.Log($" {nombreArma} MEJORADA: Daño {danoActual} | Tiempo {tiempoEntreDisparosActual:F2}s");
    }

    //  --- LÓGICA DE DISPARO ---
    public IEnumerator RutinaDisparo()
    {
        while (disparando)
        {
            if (armaPrefab != null)
            {
                GenerarProyectil();
            }

            // Esperar el tiempo calculado según el nivel actual
            yield return new WaitForSeconds(tiempoEntreDisparosActual);
        }
    }

    private void GenerarProyectil()
        {
            // Calcular posición de salida
            Vector3 spawnPos = transform.position + (transform.forward * distanciaSpawn);
            
            if (puntoDeSalida != null) 
            {
                spawnPos = puntoDeSalida.position;
            }

            // Crear el objeto
            GameObject nuevaArma = Instantiate(armaPrefab, spawnPos, transform.rotation);

            // Configurar el arma según su tipo (bumerán)
            Bumeran scriptBumeran = nuevaArma.GetComponent<Bumeran>();
            if (scriptBumeran != null)
            {
                // El bumerán pide (Daño, TransformJugador)
                scriptBumeran.ConfigurarArma(danoActual, transform);
                return; // Ya terminamos
            }

            // Configurar el arma según su tipo (hacha)
            Hacha scriptHacha = nuevaArma.GetComponent<Hacha>();
            if (scriptHacha != null)
            {
                // El hacha solo pide (Daño)
                scriptHacha.ConfigurarArma(danoActual);
                return; // Ya terminamos
            }
            // Configurar el arma según su tipo (tajo)
            SlashAtaque scriptTajo = nuevaArma.GetComponent<SlashAtaque>();
            if (scriptTajo != null)
            {
                // El tajo solo pide (Daño)
                scriptTajo.ConfigurarArma(danoActual);
                return; // Ya terminamos
            }
            Debug.LogError("No se ha configurado el arma: " + armaPrefab.name);
        }
}