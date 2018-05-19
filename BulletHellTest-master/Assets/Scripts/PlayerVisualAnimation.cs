using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Esta clase administra la animacion de giro e inclinacion del jugador, necesita que PlayerShootBehaviour le indique si el jugador se ha girado, y tambien necesita
// que PlayerMovement le indique el input vertical para calcular la inclinacion, tanto la inclinacion como el giro son solo y unicamente visuales, no tienen ningun efecto
// en el angulo en el que se disparan los proyectiles, ni modifican la posicion o rotacion del hitbox del jugador.

public class PlayerVisualAnimation : MonoBehaviour {

    public Transform PlayerVisualParent_turned;                     // Parent de la rotacion de giro
    public Transform PlayerVisualParent_tilted;                     // Parent de la rotacion de inclinacion

    private bool turned = false;                                    // Almacena el boolean que indica si esta girado, que le manda PlayerShootBehaviour
    private float verticalInput = 0;                                // Almacena el input vertical que le manda PlayerMovement

    private const float TILT_SPEED = 200;                           // CONSTANTE que indica la velocidad de inclinacion
    private const float TURN_SPEED = 600;                           // CONSTANTE que indica la velocidad de giro

    // Giramos los parent de rotacion e inclinacion poco a poco hasta la rotacion deseada, utilizamos dos parents diferentes para que no haya problemas a la hora de crear los quaterniones
    // de rotacion objetivo apartir de grados euler.
    private void Update()
    {
        // Dos casos diferentes, si el jugador esta mirando al frente o atras
        if (turned)
        {
            PlayerVisualParent_turned.localRotation = Quaternion.RotateTowards(PlayerVisualParent_turned.localRotation, Quaternion.Euler(0, 180, 0), Time.deltaTime * TURN_SPEED);
            PlayerVisualParent_tilted.localRotation = Quaternion.RotateTowards(PlayerVisualParent_tilted.localRotation, Quaternion.Euler(-45 * verticalInput, 0, 0), Time.deltaTime * TILT_SPEED);
        }
        else
        {
            PlayerVisualParent_turned.localRotation = Quaternion.RotateTowards(PlayerVisualParent_turned.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * TURN_SPEED);
            PlayerVisualParent_tilted.localRotation = Quaternion.RotateTowards(PlayerVisualParent_tilted.localRotation, Quaternion.Euler(45 * verticalInput, 0, 0), Time.deltaTime * TILT_SPEED);
        }
    }

    // Ajusta el parametro Turned, que inicialmente es false. Esta funcion se llama desde PlayerShootBehaviour
    public void SetTurned(bool arg)
    {
        turned = arg;
    }

    // Ajusta el parametro VerticalInput, que incialmente es 0. Esta funcion se llama desde PlayerMovement
    public void SetVerticalInput(float arg)
    {
        verticalInput = arg;
    }

}
