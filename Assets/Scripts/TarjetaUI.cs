using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TarjetaUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public TMP_Text textoTitulo;
    public TMP_Text textoDescripcion;
    public Image imagenIcono;
    public Button miBoton;

    // Variables internas para guardar la info
    private DataMejora misDatos;
    private LevelUpManager elManager;

    public void Configurar(DataMejora datos, LevelUpManager managerRecibido)
    {
        // 1. Guardamos la información en variables de esta clase para poder usarlas luego cuando hagamos clic.
        misDatos = datos;
        elManager = managerRecibido;

        // 2. Rellenamos lo visual
        if (textoTitulo != null) textoTitulo.text = datos.titulo;
        if (textoDescripcion != null) textoDescripcion.text = datos.descripcion;
        if (imagenIcono != null) imagenIcono.sprite = datos.icono;

        // 3. Configuramos el botón (LA FORMA CLÁSICA)
        if (miBoton != null)
        {
            // Limpiamos por seguridad
            miBoton.onClick.RemoveAllListeners();

            // Le decimos: "Cuando te pulsen, ejecuta la función 'EnviarSeleccion'"
            // Fíjate que aquí NO ponemos paréntesis () al final de EnviarSeleccion
            miBoton.onClick.AddListener(EnviarSeleccion);
        }
    }

    // Esta función se ejecuta cuando el jugador hace CLICK en una carta
    void EnviarSeleccion()
    {
        // Como guardamos 'elManager' y 'misDatos' arriba, podemos usarlos aquí
        if (elManager != null)
        {
            elManager.AlElegirCarta(misDatos);
        }
    }
}