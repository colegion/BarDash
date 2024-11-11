using System.Collections;
using System.Collections.Generic;
using Helpers;
using Interfaces;
using UnityEngine;

public class Waitress : BaseTile, ITappable
{
    public void OnTap()
    {
        Move();
    }
    
    public override void Move()
    {
        TryFindPath();
    }

    private void TryFindPath()
    {
        
    }
}
