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

    // IEnumerable<T>�� ����ϸ� �پ��� �÷��� Ÿ���� ���� �� �ִ�.
    // List<T>�Ӹ� �ƴ϶� Array, HashSet<T>, Queue<T> ���� �ٸ� �÷��ǵ� ���� �� �ִ�.
    // IEnumerable<T>�� ����ϸ� "���� ����(lazy evaluation)"�� �����ϴ�. ��, foreach ������ ������ �� �÷��� ��ü�� �������� �ʾ� ���ɻ� ������ �ݸ�, List<T> �� �÷��� ��ü�� �޸𸮿� �ø���.
    public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
    {
        // List�� null�� �� �ְų� ����ִ��� üũ�ϴ� �Լ�
        bool error = false;
        int count = 0;

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
