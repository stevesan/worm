using UnityEngine;
using System.Collections.Generic;

public class InputStack : MonoBehaviour
{
    private static InputStack _main = null;

    public static InputStack main {
        get
        {
            if( _main == null )
            {
                var go = new GameObject("__InputStack__");
                _main = go.AddComponent<InputStack>();
            }

            return _main;
        }
    }

    Stack<MonoBehaviour> stack = new Stack<MonoBehaviour>();

    public static void Push( MonoBehaviour comp )
    {
        main.stack.Push(comp);
    }

    public static void Pop( MonoBehaviour comp )
    {
        if( main.stack.Peek() == comp )
        {
            main.stack.Pop();

            // keep popping to get past handlers that have been destroyed
            while( main.stack.Peek() == null )
                main.stack.Pop();
        }
        else
            Debug.LogError("Component tried to pop when it was not the active input handler!");
    }

    public static bool IsActive( MonoBehaviour comp )
    {
        return main.stack.Count > 0 && main.stack.Peek() == comp;
    }
}
