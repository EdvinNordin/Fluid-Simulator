
using System.Collections;
using UnityEngine;

namespace FluidSim2DProject
{

    public class GPUWaterSimulation : MonoBehaviour
    {

        int READ = 0;
        int WRITE = 1;
        float dt = 0.125f;

        public Color fluid_color = Color.blue;
        public Color obstacle_color = Color.white;

        public Material waterSurface_mat, advection_mat, convection_mat, divergence_mat, linearSolve_mat, externalforce_mat, gradient_mat, boundaries_mat;

        RenderTexture surface_texture, divergence_texture, boundaries_texture;
        RenderTexture[] velocity_texture, density_texture, pressure_texture, dye_force_texture;

        float dye_force = 20.0f;
        float force_density = 1.0f;
        float dye_dissipation = 0.99f;
        float velocity_dissipation = 0.99f;
        float density_dissipation = 0.9999f;
        float prevX, prevY, xPos, yPos;
        float cellSize = 1.0f;
        float gradientScale = 1.0f;

        Vector2 resolution;
        int iterations = 50;
        float mouse_force_radius = 0.1f;

        Rect canvas;
        int canvas_width, canvas_height;

        void Start()
        {
            canvas_width = 512;
            canvas_height = 512;

            Vector2 size = new Vector2(canvas_width, canvas_height);
            Vector2 pos = new Vector2(Screen.width / 2, Screen.height / 2) - size * 0.5f;
            canvas = new Rect(pos, size);

            resolution = new Vector2(1.0f / canvas_width, 1.0f / canvas_height);

            velocity_texture = new RenderTexture[2];
            density_texture = new RenderTexture[2];
            dye_force_texture = new RenderTexture[2];
            pressure_texture = new RenderTexture[2];

            InitialiseTexture(velocity_texture, RenderTextureFormat.RGFloat, FilterMode.Bilinear);
            InitialiseTexture(density_texture, RenderTextureFormat.RFloat, FilterMode.Bilinear);
            InitialiseTexture(dye_force_texture, RenderTextureFormat.RFloat, FilterMode.Bilinear);
            InitialiseTexture(pressure_texture, RenderTextureFormat.RFloat, FilterMode.Point);

            surface_texture = new RenderTexture(canvas_width, canvas_height, 0, RenderTextureFormat.ARGB32);
            surface_texture.filterMode = FilterMode.Bilinear;
            surface_texture.wrapMode = TextureWrapMode.Clamp;
            surface_texture.Create();

            divergence_texture = new RenderTexture(canvas_width, canvas_height, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
            divergence_texture.filterMode = FilterMode.Point;
            divergence_texture.wrapMode = TextureWrapMode.Clamp;
            divergence_texture.Create();

            boundaries_texture = new RenderTexture(canvas_width, canvas_height, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
            boundaries_texture.filterMode = FilterMode.Point;
            boundaries_texture.wrapMode = TextureWrapMode.Clamp;
            boundaries_texture.Create();
        }

        void OnGUI()
        {
            GUI.DrawTexture(canvas, surface_texture);
        }

        void InitialiseTexture(RenderTexture[] surface, RenderTextureFormat format, FilterMode filter)
        {
            surface[READ] = new RenderTexture(canvas_width, canvas_height, 0, format, RenderTextureReadWrite.Linear);
            surface[READ].filterMode = filter;
            surface[READ].wrapMode = TextureWrapMode.Clamp;
            surface[READ].Create();

            surface[WRITE] = new RenderTexture(canvas_width, canvas_height, 0, format, RenderTextureReadWrite.Linear);
            surface[WRITE].filterMode = filter;
            surface[WRITE].wrapMode = TextureWrapMode.Clamp;
            surface[WRITE].Create();
        }
        //Advection is the process by which a fluid's velocity transports itself and other quantities in the fluid.
        void Advect(RenderTexture velocity, RenderTexture source, RenderTexture dest, float dissipation, float timeStep)
        {
            advection_mat.SetVector("_GridResolution", resolution);
            advection_mat.SetFloat("_TimeStep", timeStep);
            advection_mat.SetFloat("_Dissipation", dissipation);
            advection_mat.SetTexture("_Velocity", velocity);
            advection_mat.SetTexture("_Source", source);
            advection_mat.SetTexture("_Obstacles", boundaries_texture);

            Graphics.Blit(null, dest, advection_mat);
        }
        //Convection currents are caused by the changes in density associated with temperature changes. 
        void ApplyConvection(RenderTexture velocity, RenderTexture temperature, RenderTexture density, RenderTexture dest, float timeStep)
        {
            convection_mat.SetTexture("_Velocity", velocity);
            convection_mat.SetTexture("_Temperature", temperature);
            convection_mat.SetTexture("_Density", density);
            convection_mat.SetFloat("_TimeStep", timeStep);

            Graphics.Blit(null, dest, convection_mat);
        }
        //external force
        void ApplyForce(RenderTexture source, RenderTexture dest, Vector2 mousepos, float radius, float val)
        {
            externalforce_mat.SetVector("_Point", mousepos);
            externalforce_mat.SetFloat("_Radius", radius);
            externalforce_mat.SetFloat("_Fill", val);
            externalforce_mat.SetTexture("_Source", source);

            Graphics.Blit(null, dest, externalforce_mat);
        }
        //divergence of velocityfield
        void ComputeDivergence(RenderTexture velocity, RenderTexture dest)
        {
            divergence_mat.SetFloat("_HalfGridResolution", 0.5f / cellSize);
            divergence_mat.SetTexture("_Velocity", velocity);
            divergence_mat.SetVector("_GridResolution", resolution);
            divergence_mat.SetTexture("_Obstacles", boundaries_texture);

            Graphics.Blit(null, dest, divergence_mat);
        }
        //Iterative solution to solve possion equation
        void LinearSolver(RenderTexture pressure, RenderTexture divergence, RenderTexture dest)
        {

            linearSolve_mat.SetTexture("_Pressure", pressure);
            linearSolve_mat.SetTexture("_Divergence", divergence);
            linearSolve_mat.SetVector("_GridResolution", resolution);
            linearSolve_mat.SetFloat("_Alpha", -cellSize * cellSize);
            linearSolve_mat.SetFloat("_InverseBeta", 0.25f);
            linearSolve_mat.SetTexture("_Obstacles", boundaries_texture);

            Graphics.Blit(null, dest, linearSolve_mat);
        }
        //subtract gradient from velocity field to be incrompressible
        void SubtractGradient(RenderTexture velocity, RenderTexture pressure, RenderTexture dest)
        {
            gradient_mat.SetTexture("_Velocity", velocity);
            gradient_mat.SetTexture("_Pressure", pressure);
            gradient_mat.SetFloat("_GradientScale", gradientScale);
            gradient_mat.SetVector("_GridResolution", resolution);
            gradient_mat.SetTexture("_Obstacles", boundaries_texture);

            Graphics.Blit(null, dest, gradient_mat);
        }

        void HandleBoundaries()
        {
            boundaries_mat.SetVector("_GridResolution", resolution);
            Graphics.Blit(null, boundaries_texture, boundaries_mat);
        }
        //swap between read and write 
        void Swap(RenderTexture[] texs)
        {
            RenderTexture temp = texs[READ];
            texs[READ] = texs[WRITE];
            texs[WRITE] = temp;
        }
        /*A step of the simulation algorithm can be expressed by
        first advection, followed by diffusion, force application, and projection. */
        void FixedUpdate()
        {
            HandleBoundaries();

            //Set the density field and obstacle color
            waterSurface_mat.SetColor("_FluidColor", fluid_color);
            waterSurface_mat.SetColor("_ObstacleColor", obstacle_color);

            /*********************************** Advection part**************************************************/
            //Advect velocity against its self
            Advect(velocity_texture[READ], velocity_texture[READ], velocity_texture[WRITE], velocity_dissipation, dt);
            //Advect dye against velocity
            Advect(velocity_texture[READ], dye_force_texture[READ], dye_force_texture[WRITE], dye_dissipation, dt);
            //Advect density against velocity
            Advect(velocity_texture[READ], density_texture[READ], density_texture[WRITE], density_dissipation, dt);

            Swap(velocity_texture);
            Swap(dye_force_texture);
            Swap(density_texture);

            /*********************************** Diffusion part**************************************************/
            //Determine how the flow of the fluid changes the velocity (thickness)
            ApplyConvection(velocity_texture[READ], dye_force_texture[READ], density_texture[READ], velocity_texture[WRITE], dt);

            Swap(velocity_texture);

            /*********************************** Force part**************************************************/
            //If left click down add dye
            if (Input.GetMouseButton(0))
            {
                Vector2 pos = Input.mousePosition;

                pos.x -= canvas.xMin;
                pos.y -= canvas.yMin;

                pos.x /= canvas.width;
                pos.y /= canvas.height;
                xPos = pos.x;
                yPos = pos.y;
                float force = (yPos - prevY)*10 + (xPos - prevX)*10;
                ApplyForce(dye_force_texture[READ], dye_force_texture[WRITE], pos, mouse_force_radius, dye_force);
                ApplyForce(density_texture[READ], density_texture[WRITE], pos, mouse_force_radius, force_density);
                Swap(dye_force_texture);
                Swap(density_texture);
            }
            /******************************* Projection part **************************************/
            /* the projection step is divided into two operations: solving the Poisson-pressure equation for p,
             * and subtracting the gradient of p from the intermediate velocity field. 
             * This requires three fragment programs: the linearsolver iteration program, 
             * a program to compute the divergence of the intermediate velocity field, 
             * and a program to subtract the gradient of p from the intermediate velocity field.*/

            ComputeDivergence(velocity_texture[READ], divergence_texture);


            for (int i = 0; i < iterations; ++i)
            {
                LinearSolver(pressure_texture[READ], divergence_texture, pressure_texture[WRITE]);
                Swap(pressure_texture);
            }

            //Use the pressure tex that was last rendered into. This computes divergence free velocity
            SubtractGradient(velocity_texture[READ], pressure_texture[READ], velocity_texture[WRITE]);

            Swap(velocity_texture);

            //Render the tex you want to see into gui tex. Will only use the red channel
            waterSurface_mat.SetTexture("_Obstacles", boundaries_texture);
            Graphics.Blit(density_texture[READ], surface_texture, waterSurface_mat);
            prevX = xPos;
            prevY = yPos;
        }
    }

}
