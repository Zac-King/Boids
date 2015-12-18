using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoidManager : MonoBehaviour {
    
    #region #Variables
    [Header("Boid Setup")]
    public GameObject boid;
    public GameObject predatorBoid;
    public GameObject target;

    public int numberOfBoids = 0;
    public int numberOfPredators = 0;

    public List<GameObject> BoidAgents;
    public List<GameObject> PredatorAgents;
   
    [Header("Rules")]
    [Space(10)]
    public float cohesionMod    = 0;    
    public float seperationMod  = 0;
    public float alignmentMod   = 0;

    public float borderMod      = 50;
    public float velocityLimit  = 0;
    #endregion

    /////////////////////////////////////////////

    void Start()
    {
        CreateBoid(); // Create all the boids 
    }

    void Update()
    {
        UpdateBoids();  // Move 'em
    }

    #region #BoidHandlers
    [ContextMenu("Populate")]
    public void PopBoids()
    {
        foreach(GameObject b in BoidAgents)
        {
            Destroy(b);
        }
        BoidAgents = new List<GameObject>();
        CreateBoid();
    }

    public void BoidNum(UnityEngine.UI.Slider slider)
    {
        numberOfBoids = (int)slider.value;
    }

    void CreateBoid()
    {
        for(int i = 0; i < numberOfBoids; i++)
        {
            GameObject b = Instantiate(boid) as GameObject; // make new boid
            b.transform.parent = gameObject.transform;      // sets new boid as a child of the manager

            b.transform.position = new Vector3( Random.Range(-15.0f, 15.0f),      // Random X Position
                                                Random.Range(-15.0f, 15.0f),      // Random Y Position
                                                Random.Range(-15.0f, 15.0f));     // Random Z Position
            b.GetComponent<Boid>().velocity = new Vector3(  Random.Range(-15.0f, 15.0f),      // Random X Velocity    
                                                            Random.Range(-15.0f, 15.0f),      // Random Y Velocity
                                                            Random.Range(-15.0f, 15.0f));     // Random Z Velocity
            b.GetComponent<Boid>().agressive = false;

            BoidAgents.Add(b);  // Adds newly created boid to the list
        }

        for (int i = 0; i < numberOfPredators; i++)
        {
            GameObject b = Instantiate(predatorBoid) as GameObject; // make new boid
            b.transform.parent = gameObject.transform;      // sets new boid as a child of the manager

            b.transform.position = new Vector3(Random.Range(-15.0f, 15.0f),      // Random X Position
                                                Random.Range(-15.0f, 15.0f),      // Random Y Position
                                                Random.Range(-15.0f, 15.0f));     // Random Z Position
            b.GetComponent<Boid>().velocity = new Vector3(  Random.Range(-15.0f, 15.0f),      // Random X Velocity    
                                                            Random.Range(-15.0f, 15.0f),      // Random Y Velocity
                                                            Random.Range(-15.0f, 15.0f));     // Random Z Velocity
            b.GetComponent<Boid>().agressive = true;

            PredatorAgents.Add(b);  // Adds newly created predator to the list
        }
    }

    private void UpdateBoids()
    {
        Vector3 r1 = Vector3.zero;
        Vector3 r2 = Vector3.zero;
        Vector3 r3 = Vector3.zero;
        Vector3 r4 = Vector3.zero;
        Vector3 r5 = Vector3.zero;

        #region#BoidsUpdatePerameters
        foreach (GameObject g in BoidAgents)
        {
            r1 = Cohesion(g) * cohesionMod;      // Rule 1
            r2 = Seperation(g) * seperationMod;  // Rule 2
            r3 = Alignment(g) * alignmentMod;    // Rule 3
            r4 = BorderPushBack(g);              // Rule 4
            r5 = Velocitylimiter(g);

            g.GetComponent<Boid>().velocity += r1 + r2 + r3 + r4 + r5;
            g.transform.position += g.GetComponent<Boid>().velocity;
            g.transform.up = g.GetComponent<Boid>().velocity;
        }
        #endregion

        #region#PredatorUpdatePerameter
        foreach (GameObject p in PredatorAgents)
        {
            r1 = Cohesion(p);       // Rule 1
            r2 = Seperation(p);     // Rule 2
            r3 = Alignment(p);      // Rule 3
            r4 = BorderPushBack(p); // Rule 4
            r5 = Velocitylimiter(p);

            p.GetComponent<Boid>().velocity += r1 + r2 + r3 + r4 + r5;
            p.transform.position += p.GetComponent<Boid>().velocity;
            p.transform.up = p.GetComponent<Boid>().velocity;
        }
        #endregion
    }
    #endregion

    #region #BoidRules
    private Vector3 Cohesion(GameObject b)
    {
        // r = Return Velocity
        Vector3 r = Vector3.zero;
        int c = 0;

        if (target)
        {
            r =  ((target.transform.position - b.transform.position)/ 100);
        }

        else
        {
            foreach (GameObject g in BoidAgents)
            {
                if (g != b)
                { r += g.GetComponent<Transform>().position; c++; }
            }

            r = r / c; // For Perceived Center of mass
            r = ((r - b.GetComponent<Transform>().position) / 100);
        }
        return r;
    }

    private Vector3 Seperation(GameObject b)
    {
        // r = Return Velocity
        Vector3 r = Vector3.zero;

        foreach (GameObject g in BoidAgents)
        {
            if (g != b)
            {
                if( Vect3Dist(g.transform.position, b.transform.position) < 3 )
                {
                    r += (b.transform.position - g.transform.position)/2;
                }
            }
        }
        return r;
    }

    private Vector3 Alignment(GameObject b)
    {
        // r = Return Velocity
        Vector3 r = Vector3.zero;

        foreach (GameObject g in BoidAgents)
        {
            if (g != b)
            {
                r += g.GetComponent<Boid>().velocity;
            }
        }

        r = r / (BoidAgents.Count - 1);
        return r - ((b.GetComponent<Boid>().velocity) / 200) ;
    }

    private Vector3 BorderPushBack(GameObject b)
    {
        Vector3 r = Vector3.zero;       

        // X border check
        if (b.transform.localPosition.x > borderMod)
        { r.x -= (b.transform.localPosition.x % borderMod); }
        if (b.transform.localPosition.x < (-1 * borderMod))
        { r.x -= (b.transform.localPosition.x % borderMod); }
        
        // Y border check
        if (b.transform.localPosition.y > borderMod)
        { r.y -= (b.transform.localPosition.y % borderMod); }
        if (b.transform.localPosition.y < (-1 * borderMod))
        { r.y -= (b.transform.localPosition.y % borderMod); }

        // Z border check
        if (b.transform.localPosition.z > borderMod)
        { r.z -= (b.transform.localPosition.z % borderMod); }
        if (b.transform.localPosition.z < (-1 * borderMod))
        { r.z -= (b.transform.localPosition.z % borderMod); }


        return r;       // Return velocity away from border

    }

    private Vector3 Velocitylimiter(GameObject b)
    {
        Vector3 r = Vector3.zero;
        // Magintude = (x^2 + y^2 + z^2)
        if (Mathf.Sqrt(b.GetComponent<Boid>().velocity.magnitude) > velocityLimit)
            b.GetComponent<Boid>().velocity = (b.GetComponent<Boid>().velocity / b.GetComponent<Boid>().velocity.magnitude) * velocityLimit;

        return r;       // Return velocity away from border
    }
    #endregion

    #region #RuleSetters
    public void SetRule1(float a)
    {
        cohesionMod = a;
    }

    public void SetRule2(float a)
    {
        seperationMod = a;
    }

    public void SetRule3(float a)
    {
        alignmentMod = a;
    }

    public void SetSpeed(float a)
    {
        velocityLimit = a;
    }

    public void SetBorder(float a)
    {
        borderMod = a;
    }
    #endregion

    
    public float Vect3Dist(Vector3 a, Vector3 b)
    {

        return Mathf.Sqrt(  ((a.x - b.x) * (a.x - b.x)) +           // X^2 
                            ((a.y - b.y) * (a.y - b.y)) +           // Y^2
                            ((a.z - b.z) * (a.z - b.z)));           // Z^2
    }
}