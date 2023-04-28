using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item: MonoBehaviour
{
    public string nombre, descripcion;
    public int cantidad=1;
    public Sprite icono;
   
    public void SetNombre(string n) { nombre = n; }
    public string GetNombre() { return nombre; }

    public void SetDescripcion(string d) { descripcion = d; }
    public string GetDescripcion() { return descripcion; }

    public void SetCantidad(int n) { cantidad = n; }
    public int GetCantidad() { return cantidad; }

    public void SetIcono(Sprite s) { icono = s; }
    public Sprite GetIcono() { return icono; }
}
