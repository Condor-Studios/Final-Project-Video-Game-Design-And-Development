using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visceral_Script : MonoBehaviour
{
    //patricio malvasio
    // 6/4/2025 21:57
    // hice este Script como componente base de TODOS los scripts del juego
    //la idea detras es poder unificar un poco los scripts y tener eventos universales


    //initialize, usado como start pero tenemos control más exacto de cuando
    public virtual void VS_Initialize() { }

    //Initialize with parameters, similar pero podemos pasar parametros con params
    //despues es importante castearlos explicitamente al type necesario
    public virtual void VS_InitializeWithParameters(params object[] a) { }
}
