using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Reports back that a clickable gameobject has been clicked on to select it as a target.
/// </summary>
public class Targeting : MonoBehaviour
{
    //Private Variables
    //private GameManager gm;


    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start ()
    {
        //gm = GameManager.Instance;
	}


    /// <summary>
    /// Tells game manager that I have been targeted.
    /// </summary>
    private void OnMouseDown()
    {
        //Tell game manager that I am the current target.
        //gm.SetTarget(gameObject);
    }

}//end of Targeting
