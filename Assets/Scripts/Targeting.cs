using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Reports back that a clickable gameobject has been clicked on to select it as a target.
/// </summary>
public class Targeting : MonoBehaviour
{
    //Private Variables
<<<<<<< HEAD
    private GameManager gm;
=======
    //private GameManager gm;
>>>>>>> 286519a468f23cb04c2bd0d9ccf1bca3a38e8b68


    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start ()
    {
<<<<<<< HEAD
        gm = GameManager.Instance;
=======
        //gm = GameManager.Instance;
>>>>>>> 286519a468f23cb04c2bd0d9ccf1bca3a38e8b68
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
