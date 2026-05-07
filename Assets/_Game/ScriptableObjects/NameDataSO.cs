using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NameDataSO")]
public class NameDataSO : ScriptableObject
{
    [SerializeField] private string[] names;
    public int NameCount => names.Length;

    public string GetName(int index)
    {
        return names[index];
    }

    public void shuffleNames()
    {
        for (int i = 0; i < names.Length; i++)
        {
            int randomIndex = Random.Range(0, names.Length);
            string temp = names[i];
            names[i] = names[randomIndex];
            names[randomIndex] = temp;
        }
    }
}
