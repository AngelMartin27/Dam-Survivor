using UnityEngine;
using System.Collections.Generic; 

[CreateAssetMenu(fileName = "NuevaOleada", menuName = "Sistema Oleadas/Crear Oleada")]
public class DataOleada : ScriptableObject
{

    [System.Serializable]
    public class GrupoEnemigo
    {
        [Header("Identificación")]
        [SerializeField] public string nombre; 

        [Header("Spawn Configuration")]
        [SerializeField] public GameObject enemigoPrefab;
        
        [SerializeField] public int cantidadTotal; 
        
        [SerializeField] public float tiempoEntreSpawns; 

        // --- PROPIEDADES PÚBLICAS ---
        // Propiedades que lee el EnemySpawner (Solo lectura)
        public string Nombre => nombre;
        public GameObject EnemigoPrefab => enemigoPrefab;
        public int CantidadTotal => cantidadTotal;
        public float TiempoEntreSpawns => tiempoEntreSpawns;
    }

    // --- CONFIGURACIÓN PRINCIPAL DE LA OLEADA ---

    [Header("Composición")]
    [SerializeField] public List<GrupoEnemigo> gruposDeEnemigos; 

    [Header("Finalización")]
    [SerializeField] public float tiempoAlTerminar = 5f;

    // --- PROPIEDADES PÚBLICAS ---
    public List<GrupoEnemigo> GruposDeEnemigos => gruposDeEnemigos;
    public float TiempoAlTerminar => tiempoAlTerminar;
}