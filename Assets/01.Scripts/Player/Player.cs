using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private InputReaderSO _inputReader;
    private Dictionary<Type, IPlayerComponent> _components;

    private void Awake()
    {
        _components = new Dictionary<Type, IPlayerComponent>();

        _components.Add(_inputReader.GetType(),_inputReader);

        GetComponentsInChildren<IPlayerComponent>().ToList()
            .ForEach(x => _components.Add(x.GetType(),x));
        _components.Values.ToList().ForEach(compo => compo.Initialize(this));
        
    }

    public T GetCompo<T>() where T : class
    {
        Type type = typeof(T);
        if (_components.TryGetValue(type, out IPlayerComponent compo))
        {
            return compo as T;
        }
        return default;
    }
}
