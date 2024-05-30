using System.Collections.Generic;

public class C_StackList<T> where T: class
{
    private List<T> _items;

    public T this[int Index]
    {
        get { return _items[Index]; }
    }

    public int Count
    {
        get { return _items.Count; }
    }

    public C_StackList()
    {
        _items = new List<T>();
    }

    public void Add(T item)
    {
        _items.Add(item);
    }

    public void Remove(T item)
    {
        _items.Remove(item);
    }

    public void RemoveAt(int index)
    {
        _items.RemoveAt(index);
    }

    // Retourne le premier élément de la liste
    public T Pop()
    {
        if (_items.Count == 0) return null;
        int i = 0;

        while (i < _items.Count)
        {
            if (_items[i] == null) _items.RemoveAt(i);
            else return _items[i];
        }
        
        return null;
    }

    // Pousse le premier élément de la liste à la fin de la liste
    public void Push()
    {
        if (_items.Count <= 1) return;

        T item = _items[0];
        _items.RemoveAt(0);
        _items.Add(item);
    }

    // Balaye tous les éléments de la liste et supprime tous les éléments égals à null
    public void Sweep(int startIndex)
    {
        if (startIndex >= _items.Count) return;

        for (int i = startIndex; i < _items.Count; i++)
        {
            if (_items[i] != null) { _items.RemoveAt(i); Sweep(i); }
        }
    }
}