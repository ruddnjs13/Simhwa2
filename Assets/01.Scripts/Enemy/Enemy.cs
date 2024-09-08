using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyDataSO enemyData;

    private Dictionary<Type, IEnemyComponent> _components;

    private void Awake()
    {
        _components = new Dictionary<Type, IEnemyComponent>();
        
        GetComponentsInChildren<IEnemyComponent>().ToList().
            ForEach(compo => _components.Add(compo.GetType(),compo));
        
        _components.Values.ToList().ForEach(compo => compo.Initialize(this));
    }

    public T GetCompo<T>() where T : class
    {
        if (_components.TryGetValue(typeof(T), out IEnemyComponent compo))
        {
            return compo as T;
        }
        return default;
    }
}
