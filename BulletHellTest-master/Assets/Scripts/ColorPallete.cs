using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Clase estatica con un par de referencias a colores que se utilizaran en el juego, similar a la clase Color de unity, pero ampliable.

public class ColorPallete : MonoBehaviour {

    public static Color32 color_effectNuclear = new Color32(0, 170, 255, 255);
    public static Color32 color_effectPhoton = new Color32(255, 125, 0, 255);
    public static Color32 color_effectExplosive = new Color32(255, 0, 125, 255);
}
