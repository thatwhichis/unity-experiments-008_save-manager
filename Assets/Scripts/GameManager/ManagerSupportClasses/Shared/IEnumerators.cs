using System; // Action
using System.Collections; // IEnumerator
using UnityEngine; // WaitForSeconds

public class IEnumerators
{
    public static IEnumerator DoOnceDelayed(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);

        action.Invoke();
    }
}
