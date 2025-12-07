using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FrostZone : MonoBehaviour
{
    [Header("Tiempos del Ciclo")]
    public bool esPermanente = false; 
    public float tiempoActivo = 5.0f;    
    public float tiempoDescanso = 3.0f;  

    [Header("Daño y Ralentización")]
    public float intervaloTick = 0.5f;    
    public int damagePorTick = 10;          
    public float porcentajeLentitud = 0.3f;  
    
    [Header("Posición")]
    // Si el personaje mide 2 metros, la mitad es 1.
    public float ajusteAltura = -1.0f; 

    // Referencias Componentes
    private CapsuleCollider colisionador;
    private MeshRenderer renderizador;    
    
    // Lista de enemigos
    private List<EnemyControler> enemigosDentro = new List<EnemyControler>(); 
    private bool zonaActiva = true; 

    void Start()
    {
        colisionador = GetComponent<CapsuleCollider>();
        renderizador = GetComponent<MeshRenderer>();

        colisionador.isTrigger = true;

        // Mantenemos la X y Z en 0 (centro del jugador), pero bajamos la Y.
        transform.localPosition = new Vector3(0, ajusteAltura, 0);

        // Iniciamos las rutinas
        StartCoroutine(CicloDeZona());
        StartCoroutine(AplicarDañoPeriodico());
    }

    // ENCENDER Y APAGAR LA ZONA 
    IEnumerator CicloDeZona()
    {
        ActivarZona(true);
        
        while (true)
        {
            if (esPermanente) yield break; 

            yield return new WaitForSeconds(tiempoActivo);

            ActivarZona(false);
            yield return new WaitForSeconds(tiempoDescanso);
            
            ActivarZona(true);
        }
    }

    void ActivarZona(bool estado)
    {
        zonaActiva = estado;
        
        if(colisionador != null) colisionador.enabled = estado;
        if(renderizador != null) renderizador.enabled = estado;

        if (!estado)
        {
            LiberarTodosLosEnemigos();
        }
    }

    // -- DAÑO PERIÓDICO ---
    IEnumerator AplicarDañoPeriodico()
    {
        while (true)
        {
            yield return new WaitForSeconds(intervaloTick);

            if (zonaActiva)
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
            }
        }
    }

    // --- DETECCIÓN DE ENEMIGOS ---
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Nota: Asegúrate que tu script de enemigo se llama exactamente así
            EnemyControler enemigo = other.GetComponent<EnemyControler>();
            
            if (enemigo != null && !enemigosDentro.Contains(enemigo))
            {
                enemigosDentro.Add(enemigo);
                enemigo.Ralentizar(porcentajeLentitud);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyControler enemigo = other.GetComponent<EnemyControler>();
            if (enemigo != null && enemigosDentro.Contains(enemigo))
            {
                enemigosDentro.Remove(enemigo);
                enemigo.RestaurarVelocidad();
            }
        }
    }

    void LiberarTodosLosEnemigos()
    {
        foreach (var enemigo in enemigosDentro)
        {
            if (enemigo != null) enemigo.RestaurarVelocidad();
        }
        enemigosDentro.Clear();
    }
}