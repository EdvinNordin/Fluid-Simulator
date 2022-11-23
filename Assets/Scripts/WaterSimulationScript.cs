using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSimulationScript : MonoBehaviour
{
    public ComputeShader WaterShader;
    public RenderTexture NState, Nm1State, Np1State;
    public Vector2Int resolution;
    public Vector3 effect; //x coord, y coord, strenght
    public Material waveMat;
    public float dispersion;
    // Start is called before the first frame update
    void Start()
    {
        InitializeTex(ref NState);
        InitializeTex(ref Nm1State);
        InitializeTex(ref Np1State);
        waveMat.mainTexture = NState;
    }
    void InitializeTex(ref RenderTexture tex)
    {
        tex = new RenderTexture(resolution.x, resolution.y, 1, UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_SNorm);
        tex.enableRandomWrite = true;
        tex.Create();
    }
    // Update is called once per frame
    void Update()
    {
        Graphics.CopyTexture(NState, Nm1State);
        Graphics.CopyTexture(Np1State, NState);
        WaterShader.SetTexture(0, "NState", NState);
        WaterShader.SetTexture(0, "Nm1State", Nm1State);
        WaterShader.SetTexture(0, "Np1State", Np1State);
        WaterShader.SetVector("effect", effect);
        WaterShader.SetVector("resolution", new Vector2(resolution.x, resolution.y));
        WaterShader.SetFloat("dispersion",dispersion);
        WaterShader.Dispatch(0, resolution.x / 8, resolution.y / 8, 1); // /8 since 8 threads

    }
}
