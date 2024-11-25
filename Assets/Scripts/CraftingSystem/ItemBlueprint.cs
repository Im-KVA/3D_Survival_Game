using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBlueprint
{
    public string itemName;

    public string req1;
    public string req2;

    public int reqAmount1;
    public int reqAmount2;

    public int numOfReq;

    public ItemBlueprint(string name, int reqNum, string r1, int r1Num, string r2, int r2Num)
    {
        itemName = name;

        numOfReq = reqNum;

        req1 = r1;
        req2 = r2;

        reqAmount1 = r1Num;
        reqAmount2 = r2Num;
    }
}
