using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHorde {
    private static ZombieHorde singleton;

    public static ZombieHorde getSingleton()
    {
        if (singleton == null)
        {
            singleton = new ZombieHorde();
        }

        return singleton;
    }
}
