using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Item : MonoBehaviour
{
    [SerializeField] private int heal = 1;

    [SerializeField] private Transform modelTransform;

    private void Start()
    {
        Debug.Log($"ItemParent : {transform.parent.name} / {modelTransform}");
        for (int i = 0; i < modelTransform.childCount; i++)
        {
            modelTransform.GetChild(i).gameObject.SetActive(false);
        }

        var targetModel = modelTransform.GetChild(Random.Range(0, modelTransform.childCount));
        targetModel.gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.TryGetComponent(out Player player);
            if (player)
            {
                GameManager.Instance.Heal(heal);
                var go = Instantiate(ResourceDataObject.Instance.GumParticle, transform.position, quaternion.identity,
                    transform.parent);
                Destroy(go, 4f);
                Destroy(gameObject);
            }
        }
    }
}