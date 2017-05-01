using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the game.
/// </summary>
public class GameManager : MonoBehaviour
{
    //Private Variables
    private static GameManager instance = null;




    //Properties
    public static GameManager Instance
    { get { return instance; } }

    /// <summary>
    /// On awake make an instance of game manager
    /// </summary>
    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}//end of GameManager
