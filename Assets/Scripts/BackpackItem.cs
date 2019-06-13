using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackItem
{
    public int Type;
    public int Quantity;

    public BackpackItem(int type, int qty)
    {
        Type = type;
        Quantity = qty;
    }
}
