using UnityEngine;
using UnityEngine.Rendering;

/*Think the example we follow works as following:
in Fluidsimulator they use commandBuffer to choose in with order rendering is done. 
https://docs.unity3d.com/Manual/GraphicsCommandBuffers.html
https://docs.unity3d.com/Manual/srp-using-scriptable-render-context.html
Seems to first render scene then render the watersimulation. 
Everything is saved in buffers created in fluidGPUResources. The pipelone is then built from row 124 in persianGarden using functions from fluidsimulator.
The pressure buffer and velocity buffer is saved to textures. 
 */
public class Main : MonoBehaviour
{
    public ComputeShader LinearSolver;
    public ComputeShader Advection;
    public ComputeShader Projection;
    public KeyCode ApplyForceKey = KeyCode.Mouse1;

    private RenderTexture visualisation_texture;
    private Camera main_cam;
    //private CommandBuffer sim_command_buffer; //when wanting to create customized pipeline

    public void Initialize()
    {
        visualisation_texture = new RenderTexture(512, 512, 0)
        {
            enableRandomWrite = true,
            useMipMap = false,
        };
        visualisation_texture.Create();

        //UpdateRuntimeKernelParameters();

        //StructuredBufferToTextureShader.SetInt("_Pressure_Results_Resolution", 512);
        //StructuredBufferToTextureShader.SetInt("_Velocity_Results_Resolution", 512);
        //StructuredBufferToTextureShader.SetInt("_Dye_Results_Resolution", 512);
        //StructuredBufferToTextureShader.SetTexture(_handle_pressure_st2tx, "_Results", visulasation_texture);
        //sim_command_buffer = new CommandBuffer()
        //{
        //    name = "Simulation_Command_Buffer",
        //};

        // Global Parameters that are immutable in runtime
        //sim_command_buffer.SetGlobalInt("i_Resolution", 512);
        //sim_command_buffer.SetGlobalFloat("i_timeStep", 1);
        //sim_command_buffer.SetGlobalFloat("i_grid_scale", 1);
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //LinearSolver.SetFloat(Time.time, "i_Time");
        //Projection.SetFloat(Time.time, "i_Time");
        //Advection.SetFloat(Time.time, "i_Time");

        //float forceController = 0;

        //if (Input.GetKey(ApplyForceKey)) forceController = 1.0f;

        //UserInputShader.SetFloat("_force_multiplier", forceController);
        //UserInputShader.SetFloat("_force_effect_radius", 1);
        //UserInputShader.SetFloat("_force_falloff", 2.0f);
    }  


//public bool BindCommandBuffer()
//{
//    if (!IsValid()) return false;

//    main_cam.AddCommandBuffer(CameraEvent.AfterEverything, sim_command_buffer); //afterEverything is a cameraevent that is after everything is rendered. 
//    return true;
//}

}