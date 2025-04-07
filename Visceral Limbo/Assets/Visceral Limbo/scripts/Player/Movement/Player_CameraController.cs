using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InputStruct //struct usado para recibir datos de inputs
{
    public Vector2 LookDelta;
}


public class Player_CameraController : Visceral_Script
{
    private Vector3 _EulerAngles;

    [Range(0f, 1f)]
    public float sensitivity = 0.4f;

    [SerializeField] private Transform _CameraAnchor;
    public override void VS_InitializeWithParameters(params object[] a)
    {
        if(a == null || a.Length == 0)
        {
            Debug.LogWarning(this.name + " initialized with no parameters");
        }

        _CameraAnchor= (Transform)a[0]; // casteo explicito - pato

        transform.position = _CameraAnchor.position;
        transform.eulerAngles = _EulerAngles = _CameraAnchor.eulerAngles; //doble set

    }

    public void UpdatePosition(Transform Target)
    {
        this.transform.position = Target.position;
    }

    public void UpdateRotation(InputStruct inputData)
    {
        _EulerAngles += new Vector3(-inputData.LookDelta.y, inputData.LookDelta.x) * sensitivity;
        _EulerAngles.x = Mathf.Clamp(_EulerAngles.x, -80, 80);
        transform.eulerAngles = _EulerAngles;
    }
}
