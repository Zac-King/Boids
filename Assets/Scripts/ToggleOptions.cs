using UnityEngine;
using System.Collections;

public class ToggleOptions : MonoBehaviour {

    public GameObject target1;
	public GameObject target2;
	public GameObject target3;


    public void ToggleOffT1()
    {
        if (target1.activeInHierarchy) 
		{
			target1.SetActive(false);
			target2.SetActive(false);
			target3.SetActive(false);
		}
            

        else
		{
			target1.SetActive(true);
			target2.SetActive(true);
			target3.SetActive(true);
		}
    }
}
