using System.Collections;
using UnityEngine;

public class Dispararrayo : ArmaBase
{
    [Header("Configuración")]
    public GameObject proyectilPrefab;
    public float intervaloDisparo = 0.5f;

    [Header("Daño por tick")]
    public int damagePorTick = 5;       
    public int danoExtraPorNivel = 2;     

    void Start()
    {
        // Asignamos nombre para el Debug
        if(string.IsNullOrEmpty(nombreArma)) nombreArma = "Rayo Continuo";

        StartCoroutine(DispararContinuo());
    }

    // Lógica para subir de nivel
    public override void SubirNivel()
    {
        // Lógica base
        base.SubirNivel();

        // Reducimos el intervalo un 2% por nivel, con un límite de 0.1s
        intervaloDisparo = Mathf.Max(0.1f, intervaloDisparo * 0.98f);
    }

    public IEnumerator DispararContinuo()
    {
        while (true)
        {
            // Instanciar
            GameObject nuevoRayo = Instantiate(proyectilPrefab, transform.position, transform.rotation);

            // CALCULAR DAÑO USANDO EL NIVEL HEREDADO
            // Usamos (nivelActual - 1) para que en Nivel 1 el daño sea exactamente la base (5).
            // Nivel 1 = 5 + 0 = 5
            // Nivel 2 = 5 + 2 = 7
            int danoTickCalculado = damagePorTick + ((nivelActual - 1) * danoExtraPorNivel);

            // Pasárselo al script del rayo
            RayoDeEnergia scriptRayo = nuevoRayo.GetComponent<RayoDeEnergia>();

            if (scriptRayo != null)
            {
                scriptRayo.ConfigurarRayo(danoTickCalculado);
            }

            yield return new WaitForSeconds(intervaloDisparo);
        }
    }
}