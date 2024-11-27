using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ListWrapper<T>
{
    public List<T> _itens;

    public ListWrapper(List<T> itens)
    {
        _itens = itens;
    }
}
