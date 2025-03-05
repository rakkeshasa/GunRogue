using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperUtilities
{
    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
    {
        if (stringToCheck == "")
        {
            Debug.Log(fieldName + "is empty and must contain a value in object " + thisObject.name.ToString());
            return true;
        }
        return false;
    }

    // IEnumerable<T>를 사용하면 다양한 컬렉션 타입을 받을 수 있다.
    // List<T>뿐만 아니라 Array, HashSet<T>, Queue<T> 같은 다른 컬렉션도 받을 수 있다.
    // IEnumerable<T>를 사용하면 "지연 실행(lazy evaluation)"이 가능하다. 즉, foreach 루프를 실행할 때 컬렉션 전체를 복사하지 않아 성능상 유리한 반면, List<T> 는 컬렉션 전체를 메모리에 올린다.
    public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
    {
        // List에 null이 들어가 있거나 비어있는지 체크하는 함수
        bool error = false;
        int count = 0;

        if(enumerableObjectToCheck == null)
        {
            Debug.Log(fieldName + " is null in object " + thisObject.name.ToString());
            return true;
        }

        foreach(var item in enumerableObjectToCheck)
        {
            if(item == null)
            {
                Debug.Log(fieldName + " has null values in object " + thisObject.name.ToString());
                error = true;
            }
            else
            {
                count++;
            }
        }

        if (count == 0)
        {
            Debug.Log(fieldName + " has no value in object " + thisObject.name.ToString());
            error = true;
        }
        return error;
    }
}
