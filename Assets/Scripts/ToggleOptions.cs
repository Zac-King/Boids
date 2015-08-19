using UnityEngine;
using System.Collections;

public class ToggleOptions : MonoBehaviour {

    public GameObject target1;

    public void ToggleOffT1()
    {
        if (target1.activeInHierarchy)
            target1.SetActive(false);

        else
            target1.SetActive(true);
    }
}
