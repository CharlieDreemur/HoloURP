using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class HealthCoin : MonoBehaviour
{
    public PlayerBase player;
    public GameObject lifeCoinPrefab; // Reference to the life coin prefab
    private List<GameObject> lifeCoins = new List<GameObject>(); // List to store instantiated life coins
    [SerializeField]
    private float flipDuration = 0.5f; // Duration of the flip animation
    [SerializeField]
    private float vertial_space = 0.5f;
    void Awake()
    {
        player.HurtEvent.AddListener(DecreaseHealth);
    }

    // Start is called before the first frame update
    void Start()
    {
        InstantiateLifeCoins();
    }

    [ContextMenu("InstantiateLifeCoins")]
    private void InstantiateLifeCoins()
    {
        for (int i = 0; i < player.MaxHealth; i++)
        {
            GameObject lifeCoin = Instantiate(lifeCoinPrefab, transform);
            lifeCoin.transform.localPosition = new Vector3(0, 0, i*vertial_space); // Adjust the position as needed
            lifeCoins.Add(lifeCoin);
        }
    }

    // Method to decrease health and flip a life coin
    public void DecreaseHealth()
    {
        FlipCoin(lifeCoins[player.CurrentHealth]); // Flip the next available life coin
    }

    // Flip the life coin using DOTween
    private void FlipCoin(GameObject coin)
    {
        AudioManager.Instance.Play("coinflip");
        coin.transform.DORotate(new Vector3(0, 0, 180), flipDuration, RotateMode.LocalAxisAdd)
            .SetEase(Ease.InOutQuad);
    }
}
