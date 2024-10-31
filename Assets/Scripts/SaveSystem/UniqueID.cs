using System;
using System.Collections.Generic;
using UnityEngine;

public class UniqueID : MonoBehaviour
{

    [SerializeField] private string _id = Guid.NewGuid().ToString();
    public string ID => _id;

    private static Dictionary<string, GameObject> idDatabase = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (idDatabase.ContainsKey(_id)) Generate();
        else idDatabase.Add(_id, this.gameObject);
    }

    private void Generate()
    {
        _id = Guid.NewGuid().ToString();
        idDatabase.Add(_id, this.gameObject);
    }

    private void OnDestroy()
    {
        if(idDatabase.ContainsKey(_id)) idDatabase.Remove(_id);
    }
}
