using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sequential_A_Star : AStar_MonoScript{

    public override void setHCost(Node n)
    {
        //Debug.Log("Inside setHCost");

        List<float> hCosts = new List<float>();


        
        for (int i = 0; i < 7; i++)
        {
            switch (i)
            {
                case 0: // Manhattan Optimized
                    float dstX = Mathf.Abs(n.x - targetDist.x);
                    float dstY = Mathf.Abs(n.y - targetDist.y);

                    if (dstX > dstY)
                    {
                        hCosts.Add(0.25f * Weight * (1.4f * dstY + 1.0f * (dstX - dstY)));
                    }
                    else
                    {
                        hCosts.Add(0.25f * Weight * (1.4f * dstX + 1.0f * (dstY - dstX)));
                    }
                    break;

                case 1:
                    hCosts.Add(Weight * (Mathf.Max(Mathf.Abs(n.x - targetDist.x), Mathf.Abs(n.y - targetDist.y))));
                    break;

                case 2:
                    float dx = Mathf.Abs(n.x - targetDist.x);
                    float dy = Mathf.Abs(n.y - targetDist.y);
                    hCosts.Add((dx + dy) + -2 * Mathf.Min(dx, dy));
                    break;

                case 3:
                    float Dx = Mathf.Abs(n.x - targetDist.x);
                    float Dy = Mathf.Abs(n.y - targetDist.y);
                    hCosts.Add(1 * (Dx + Dy) + (Mathf.Sqrt(2) - 2) * Mathf.Min(Dx, Dy));
                    break;
                case 4:
                    hCosts.Add(Weight * Mathf.Sqrt(((n.x - targetDist.x) * (n.x - targetDist.x) + (n.y - targetDist.y) * (n.y - targetDist.y))));
                    break;
                case 5:
                    hCosts.Add(Weight * ((n.x - targetDist.x) * (n.x - targetDist.x) + (n.y - targetDist.y) * (n.y - targetDist.y)));
                    break;
                case 6:
                    //n.hCost = Weight * (Mathf.Abs(Vector2.Distance(new Vector2(n.x, n.y), targetDist)));
                    hCosts.Add(Weight * (Mathf.Abs(n.x - targetDist.x) + Mathf.Abs(n.y - targetDist.y)));
                    break;

                default:
                    continue;
            }
        }
        n.hCost = hCosts.Max();
    }

}