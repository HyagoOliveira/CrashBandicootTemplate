using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This script makes the camera be pushed against static obstacles (like walls) and make others obstacles
/// between player and camera be transparent
/// Put this script inside the camera.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraObstructionShifter : MonoBehaviour
{
    public bool visualiseInEditor = true;   // toggle for visualising the algorithm through lines for the raycast in the editor
    public float dampingSpeed = 5f;
    public float closestDistance = 0.5f;    // the closest distance the camera can be from the target

    public LayerMask staticObstaclesLayer;        // camera should be pushed into
    public LayerMask transparentObstaclesLayer;   // camera should make them transparent

    public Material transparentMaterial;

    private Dictionary<int, TrespassingObstacle> trespassingObstacles;

    private FollowCamera cameraControll;
    private Ray ray;
    private float distance;
    private float originalDist;
    private Vector3 wantedPosition;
    private bool hitStatic;
    private bool hitTransparent;

    void Start()
    {
        cameraControll = transform.parent.GetComponent<FollowCamera>();
        if (cameraControll == null)
        {
            throw new UnityException("The GO hirarquy must fallow: Camera3DGO -> MainCamera.");
        }

        originalDist = Vector3.Distance(cameraControll.transform.position, transform.position);

        trespassingObstacles = new Dictionary<int, TrespassingObstacle>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        wantedPosition = cameraControll.transform.position - transform.forward * originalDist;

        ray = new Ray(cameraControll.transform.position, -cameraControll.transform.forward);
        distance = Vector3.Distance(cameraControll.transform.position, transform.position);

        UpdateStaticsObstacles();
        UpdateTransparentsObstacles();
        
        transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * dampingSpeed);
    }


    private void UpdateStaticsObstacles()
    {
        RaycastHit hit;
        hitStatic = Physics.Raycast(ray.origin, ray.direction, out hit, 
            distance, staticObstaclesLayer);
        if (hitStatic)
        {
            #if UNITY_EDITOR_WIN
            if (!hit.transform.gameObject.isStatic)
            {
                print(hit.transform.gameObject.name + " was not static. Changed now.");
                hit.transform.gameObject.isStatic = true;
            }
            #endif
            wantedPosition = hit.point;
            if(Vector3.Distance(cameraControll.transform.position, wantedPosition) < closestDistance)
            {
                wantedPosition -= transform.forward * closestDistance;
            }
        }
    }

    private void UpdateTransparentsObstacles()
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray.origin, ray.direction, distance, transparentObstaclesLayer);
        hitTransparent = hits.Length > 0;

        //make all trespassing obstacles be solid
        foreach (int id in trespassingObstacles.Keys)
        {
            trespassingObstacles[id].transparent = false;
        }

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            Renderer renderer = hit.transform.GetComponent<Renderer>();

            if (renderer)
            {
                //if obstacle is alredy in dictionary
                if (trespassingObstacles.ContainsKey(hit.transform.GetInstanceID()))
                {
                    //make sure it continue transparent
                    trespassingObstacles[hit.transform.GetInstanceID()].transparent = true;
                }
                else
                {
                    trespassingObstacles.Add(hit.transform.GetInstanceID(), new TrespassingObstacle(renderer));
                    trespassingObstacles[hit.transform.GetInstanceID()].
                        ChangeToTransparentMaterial(transparentMaterial);
                }
            }
        }

        //check if some obstacle is not transparent
        foreach (int id in trespassingObstacles.Keys)
        {
            if (!trespassingObstacles[id].transparent)
            {
                //change to original material and remove ftom dictonary
                trespassingObstacles[id].ChangeToOriginalMaterial();
                trespassingObstacles.Remove(id);
                break;
            }
        }
    }


#if UNITY_EDITOR_WIN
    void OnDrawGizmos()
    {
        if (visualiseInEditor)
        {
            Gizmos.color = hitStatic || hitTransparent ? Color.red : Color.blue;
            Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * distance);
        }
    }
#endif
}

public class TrespassingObstacle
{
    public bool transparent;
    public Material[] originalsMaterials;
    public Texture[] originalsTextures;
    public Renderer renderer;

    public TrespassingObstacle(Renderer renderer)
    {
        this.renderer = renderer;
        originalsMaterials = renderer.materials;
        originalsTextures = new Texture[originalsMaterials.Length];

        for (int i = 0; i < originalsTextures.Length; i++)
        {
            originalsTextures[i] = originalsMaterials[i].mainTexture;
        }
    }

    public void ChangeToTransparentMaterial(Material otherMaterial)
    {
        Material[] materials = renderer.materials;
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = otherMaterial;
            materials[i].mainTexture = originalsTextures[i];
        }

        renderer.materials = materials;
        transparent = true;
    }

    public void ChangeToOriginalMaterial()
    {
        Material[] materials = renderer.materials;
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = originalsMaterials[i];
            //TODO: testar sem essa linha
            materials[i].mainTexture = originalsTextures[i];
        }
        renderer.materials = materials;

    }
}

