using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLocations : MonoBehaviour
{
    public enum Team
    {
        Team1,
        Team2
    }


    readonly Dictionary<Team, List<Vector3Int>> _teamEndPositions = new Dictionary<Team, List<Vector3Int>>();

    private void Awake()
    {
        _teamEndPositions[Team.Team1] = new List<Vector3Int>
        {
            new Vector3Int(1,0,0),
            new Vector3Int(1,1,0),
            new Vector3Int(1,2,0),
            new Vector3Int(1,3,0),
            new Vector3Int(1,4,0),
            new Vector3Int(1,5,0),
            new Vector3Int(1,6,0),
            new Vector3Int(1,7,0),
            
        };

        _teamEndPositions[Team.Team2] = new List<Vector3Int>
        {
            new Vector3Int(-6,0,0),
            new Vector3Int(-6,1,0),
            new Vector3Int(-6,2,0),
            new Vector3Int(-6,3,0),
            new Vector3Int(-6,4,0),
            new Vector3Int(-6,5,0),
            new Vector3Int(-6,6,0),
            new Vector3Int(-6,7,0),
        };
    }

    public List<Vector3Int> GetTeamPositions(Team team)
    {
        //see if the key exists and then get the vector (otherwise send an empty list to prevent a crash
        return _teamEndPositions.ContainsKey(team) ? _teamEndPositions[team] : new List<Vector3Int>();
    }


}
