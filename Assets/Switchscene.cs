using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Switchscene : MonoBehaviour
{
     public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(0);
    }
}
