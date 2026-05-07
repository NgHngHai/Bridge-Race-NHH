using UnityEngine;
using System.Collections.Generic;

public class FinishLine : MonoBehaviour
{
    private Transform tf;
    public Transform TF => tf;

    [SerializeField] private List<Podium> podiums = new List<Podium>();
    public int podiumCount => podiums.Count;

    private void Awake()
    {
        tf = transform;
    }

    public void OnInit()
    {
        // reinit podiums
        for (int i = 0; i < podiumCount; i++)
        {
            podiums[i].OnInit(ColorType.Gray);
        }
    }

    public Podium GetPodium(int index)
    {
        return podiums[index];
    }

    private void OnTriggerEnter(Collider other)
    {
        Character character = other.GetComponent<Character>();

        if (GameManager.Instance.IsState(GameState.Gameplay)) {
            GameManager.Instance.OnFinishGame(character);
        }
    }
}
