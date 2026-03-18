using System.Collections.Generic;
using UnityEngine;

public class GizObstacleLoader : PropertyLoader
{
    public override string Name => "GizObstacle";

    public override void Load(byte[] bytes, ref int index)
    {
        GizObstacle obstacle = TTObjectManager.Create<GizObstacle>(Name);
        List<TTProperty> props = new();

        _value = obstacle;
    }
}
