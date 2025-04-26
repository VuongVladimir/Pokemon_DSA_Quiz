using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassArea : MonoBehaviour
{
    [SerializeField] GrassEncounterList encounterList;
    
    public GrassEncounterList EncounterList => encounterList;
} 