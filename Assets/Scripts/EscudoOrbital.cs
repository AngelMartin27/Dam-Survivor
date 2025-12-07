using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EscudoOrbital : ArmaBase
{
    [Header("Configuración Básica")]
    public GameObject orbePrefab;
    [SerializeField] private float radio = 3.0f;      
    [SerializeField] private float velocidadRotacion = 180.0f; 
    [SerializeField] private float alturaDelSuelo = 1.0f;     
    
    private float velocidadInicial;  

    [Header("Stats del Escudo")]
    [SerializeField] private int danoBase = 10;          
    [SerializeField] private int incrementoDano = 5;     
    [SerializeField] private int orbesBase = 3;          
    [SerializeField] private int incrementoVelocidad = 5; 

    [Header("Temporalidad")]
    [SerializeField] private float duracionBase = 5.0f;   
    [SerializeField] private float cooldownBase = 2.0f;   

    // Variables internas
    private List<GameObject> orbesActivos = new List<GameObject>(); 
    private float anguloActual = 0f; 
    private bool estaActivo = false;  

    private Coroutine corrutinaCiclo;

    // Valores calculados
    private float duracionActual;
    private float cooldownActual;
    private int danoActualCalculado;
    private int cantidadOrbesCalculada;

    // Referencia al collider del propio jugador para ignorar colisiones extra
    private Collider miColliderJugador;

    void Start()
    {
        if (string.IsNullOrEmpty(nombreArma)) nombreArma = "Escudo Orbital";
        velocidadInicial = velocidadRotacion;

        // Buscamos el collider del jugador (asumiendo que este script está en el Player o un hijo)
        miColliderJugador = GetComponentInParent<Collider>();

        CalcularEstadisticas();
        corrutinaCiclo = StartCoroutine(RutinaCicloEscudo());
    }

    void OnDisable()
    {
        if (corrutinaCiclo != null) StopCoroutine(corrutinaCiclo);
    }

    IEnumerator RutinaCicloEscudo()
    {
        while (true)
        {
            ActivarEscudo();
            yield return new WaitForSeconds(duracionActual);

            DesactivarEscudo();
            yield return new WaitForSeconds(cooldownActual);
        }
    }

    void LateUpdate()
    {
        if (estaActivo && orbesActivos.Count > 0)
        {
            anguloActual += velocidadRotacion * Time.deltaTime;
            if (anguloActual >= 360f) anguloActual -= 360f;
            MoverOrbes();
        }
    }

    public override void SubirNivel()
    {
        base.SubirNivel();
        CalcularEstadisticas();

        if (corrutinaCiclo != null) StopCoroutine(corrutinaCiclo);
        
        DesactivarEscudo(); 
        corrutinaCiclo = StartCoroutine(RutinaCicloEscudo()); 
    }

    void CalcularEstadisticas()
    {
        danoActualCalculado = danoBase + ((nivelActual - 1) * incrementoDano);
        cantidadOrbesCalculada = orbesBase + ((nivelActual - 1) / 2);
        
        float velocidadTotal = velocidadInicial + ((nivelActual - 1) * incrementoVelocidad);
        velocidadRotacion = velocidadTotal;

        duracionActual = duracionBase;
        cooldownActual = cooldownBase;
    }

    void ActivarEscudo()
    {
        estaActivo = true;
        GenerarOrbesFisicos();
    }

    void DesactivarEscudo()
    {
        estaActivo = false;
        LimpiarOrbes();
    }

    void LimpiarOrbes()
    {
        foreach (var orbe in orbesActivos)
        {
            if (orbe != null) Destroy(orbe);
        }
        orbesActivos.Clear();
    }

    void GenerarOrbesFisicos()
    {
        LimpiarOrbes();

        for (int i = 0; i < cantidadOrbesCalculada; i++)
        {
            GameObject nuevoOrbe = Instantiate(orbePrefab, transform.position, Quaternion.identity);

            // Configurar daño
            OrbeDano scriptDano = nuevoOrbe.GetComponent<OrbeDano>();
            if (scriptDano != null) scriptDano.cantidadDano = danoActualCalculado;

            // --- CORRECCIÓN CRÍTICA DE FÍSICAS ---
            
            // Configurar Rigidbody
            Rigidbody rb = nuevoOrbe.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;  // IMPORTANTE: Lo hace imparable
                rb.useGravity = false;
            }

            // Configurar Collider como TRIGGER (Fantasma)
            // Esto evita que el orbe empuje al jugador, pero sigue detectando choques.
            Collider colOrbe = nuevoOrbe.GetComponent<Collider>();
            if (colOrbe != null)
            {
                colOrbe.isTrigger = true; 
                
                // Seguridad Extra: Ignorar colisión explícita con el jugador si no fuera trigger
                if (miColliderJugador != null)
                {
                    Physics.IgnoreCollision(miColliderJugador, colOrbe);
                }
            }

            orbesActivos.Add(nuevoOrbe);
        }

        MoverOrbes();
    }

    void MoverOrbes()
    {
        if (orbesActivos.Count == 0) return;

        float separacionAngular = 360f / orbesActivos.Count;
        for (int i = 0; i < orbesActivos.Count; i++)
        {
            if (orbesActivos[i] != null)
            {
                float radianes = (anguloActual + (separacionAngular * i)) * Mathf.Deg2Rad;
                float x = Mathf.Cos(radianes) * radio;
                float z = Mathf.Sin(radianes) * radio;

                Vector3 nuevaPos = transform.position + new Vector3(x, alturaDelSuelo, z);
                
                Rigidbody rb = orbesActivos[i].GetComponent<Rigidbody>();
                if(rb) rb.MovePosition(nuevaPos);
                else orbesActivos[i].transform.position = nuevaPos;
            }
        }
    }
}