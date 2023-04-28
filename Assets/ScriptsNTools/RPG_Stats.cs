using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPG_Stats : MonoBehaviour
{
    public int dinero, level, fuerza, agilidad, tecPuño, tecPatada, tecBloqueo;
    public float saludActual, saludTotal, saludRef, aguanteActual, aguanteTotal, aguanteRef, armaduraActual, armaduraTotal, absorcionArmadura;

    public void SetLevel(int niv) { level = niv; }
    public int GetLevel() { return level; }

    public void SetSaludActual(float s) { saludActual = s; }
    public float GetSaludActual() { return saludActual; }
    public void SetSaludTotal(float s) { saludTotal = s; }
    public float GetSaludTotal() { return saludTotal; }
    public void SetSaludRef(float s) { saludRef = s; }
    public float GetSaludRef() { return saludRef; }

    public void SetAguanteActual(float s) { aguanteActual = s; }
    public float GetAguanteActual() { return aguanteActual; }
    public void SetAguanteTotal(float s) { aguanteTotal = s; }
    public float GetAguanteTotal() { return aguanteTotal; }
    public void SetAguanteRef(float s) { aguanteRef = s; }
    public float GetAguanteRef() { return aguanteRef; }


    public void SetArmaduraActual(float s) { armaduraActual = s; }
    public float GetArmaduraActual() { return armaduraActual; }
    public void SetArmaduraTotal(float s) { armaduraTotal = s; }
    public float GetArmaduraTotal() { return armaduraTotal; }
    public void SetAbsorcionArmadura(float s) { absorcionArmadura = s; }
    public float GetAbsorcionArmadura() { return absorcionArmadura; }


    public int GetDinero() { return dinero; }
    public void SetDinero(int f) { dinero = f; }

    public int GetFuerza() { return fuerza; }
    public void SetFuerza(int f) { fuerza = f; }

    public int GetAgilidad() { return agilidad; }
    public void SetAgilidad(int f) { agilidad = f; }

    public int GetTecPuño() { return tecPuño; }
    public void SetTecPuño(int f) { tecPuño = f; }

    public int GetTecPatada() { return tecPatada; }
    public void SetFTecPatada(int f) { tecPatada = f; }

    public int GetTecBloqueo() { return tecBloqueo; }
    public void SetTecBloqueo(int f) { tecBloqueo = f; }
}
