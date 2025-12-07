using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración")]
    public Transform player;          // Arrastra al jugador
    public float radioSpawn = 10f;    // Distancia a la que salen
    public List<DataOleada> oleadas;  // Lista de archivos de oleadas

    private void Start()
    {
        if (player != null && oleadas.Count > 0)
        {
            StartCoroutine(RutinaDeOleadas());
        }
        else
        {
            Debug.LogError("¡Falta asignar el Player o las Oleadas en el Inspector!");
        }
    }

    // --- LÓGICA PRINCIPAL ---

    private IEnumerator RutinaDeOleadas()
    {
        foreach (var oleadaActual in oleadas)
        {
            Debug.Log($"--- OLEADA: {oleadaActual.name} ---");

            List<Coroutine> rutinasActivas = new List<Coroutine>();

            // Usamos 'gruposDeEnemigos' (minúscula) para coincidir con tu DataOleada
            foreach (var grupo in oleadaActual.gruposDeEnemigos)
            {
                rutinasActivas.Add(StartCoroutine(GenerarEnemigosDelGrupo(grupo)));
            }

            // Esperamos a que todos los grupos terminen de salir
            foreach (var rutina in rutinasActivas)
            {
                yield return rutina;
            }

            Debug.Log($"Oleada terminada. Esperando {oleadaActual.tiempoAlTerminar} segundos.");
            
            // Descanso antes de la siguiente oleada
            yield return new WaitForSeconds(oleadaActual.tiempoAlTerminar);
        }

        Debug.Log("¡VICTORIA! Se acabaron las oleadas.");

        if (GameManager.instancia != null)
        {
            GameManager.instancia.ActivarVictoria(); // Asumiendo que corregiste el nombre en GameManager
        }
    }

    private IEnumerator GenerarEnemigosDelGrupo(DataOleada.GrupoEnemigo grupo)
    {
        
        for (int i = 0; i < grupo.cantidadTotal; i++)
        {
            if (player == null) yield break;

            CrearEnemigo(grupo.enemigoPrefab); 

            yield return new WaitForSeconds(grupo.tiempoEntreSpawns);
        }
    }

    private void CrearEnemigo(GameObject prefabEnemigo)
    {
        // Buscamos un punto aleatorio en el borde del círculo
        // 'normalized' hace que siempre esté exactamente en el borde (radio 1)
        Vector2 puntoBorde = Random.insideUnitCircle.normalized;

        // Lo multiplicamos por la distancia que queremos
        Vector2 puntoLejano = puntoBorde * radioSpawn;

        // Convertimos a 3D (X, 0, Z). Y es 0 porque queremos en el suelo
        Vector3 posicionOffset = new Vector3(puntoLejano.x, 0f, puntoLejano.y);

        // La posición final es: Donde esté el jugador + el offset calculado
        Vector3 posicionFinal = player.position + posicionOffset;

        // 5. Instanciamos el enemigo
        Instantiate(prefabEnemigo, posicionFinal, Quaternion.identity);
    }
}