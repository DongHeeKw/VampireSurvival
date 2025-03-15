using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerNew : MonoBehaviour
{
    public static GameManagerNew instance;
    public Player player;

    void Awake()
    {
        instance = this;
    }
}
