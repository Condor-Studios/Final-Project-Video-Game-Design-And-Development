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

    /// <summary>
    /// esta funcion sirve para iniciar el script, similar a start. pero tiene que ser llamado de manera externa
    /// </summary>
    public virtual void VS_Initialize() { }

    //Initialize with parameters, similar pero podemos pasar parametros con params
    //despues es importante castearlos explicitamente al type necesario

    /// <summary>
    /// esta funcion sirve para iniciar el script, similar a start. pero tiene que ser llamado de manera externa   
    /// permite el uso de parametros
    /// </summary>
    /// <param name="a"> parametros usados a la hora de iniciar </param>
    /// 
    public virtual void VS_InitializeWithParameters(params object[] a) { }
}
