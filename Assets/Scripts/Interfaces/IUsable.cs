using System.Collections;
using UnityEngine;

public interface IUsable
{
    IEnumerator Use();
    bool GetUsed();
}
