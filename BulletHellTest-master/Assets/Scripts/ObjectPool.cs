using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{

    public static ObjectPool currentInstance;

    [Header("Prefabs")]
    public GameObject bulletPrefab;
    public GameObject dmgnumPrefab;
    public GameObject partBurstPrefab;
    public GameObject explosionPrefab;
    [Header("Instantiation Parents")]
    public Transform bulletsParent;
    public Transform particleBurstsParent;
    public Transform dmgNumbersParent;
    public Transform explosionsParent;

    private List<GameObject> internal_bulletPool;
    private List<GameObject> internal_dmgnumPool;
    private List<GameObject> internal_partBurstPool;
    private List<ExplosionBehaviour> internal_explosionPool;

    private int initialPoolSize = 20;
    private GameObject lastInstantiatedObject;

    void Awake()
    {
        currentInstance = this;

        internal_bulletPool = new List<GameObject>();
        internal_dmgnumPool = new List<GameObject>();
        internal_partBurstPool = new List<GameObject>();
        internal_explosionPool = new List<ExplosionBehaviour>(); 

        for (int i = 0; i < initialPoolSize; i++)
        {
            lastInstantiatedObject = Instantiate(bulletPrefab, bulletsParent) as GameObject;
            lastInstantiatedObject.gameObject.SetActive(false);
            internal_bulletPool.Add(lastInstantiatedObject);
        }

        for (int i = 0; i < initialPoolSize; i++)
        {
            lastInstantiatedObject = Instantiate(dmgnumPrefab, dmgNumbersParent) as GameObject;
            lastInstantiatedObject.gameObject.SetActive(false);
            internal_dmgnumPool.Add(lastInstantiatedObject);
        }
        for (int i = 0; i < initialPoolSize; i++)
        {
            lastInstantiatedObject = Instantiate(partBurstPrefab, particleBurstsParent) as GameObject;
            lastInstantiatedObject.gameObject.SetActive(false);
            internal_partBurstPool.Add(lastInstantiatedObject);
        }
        for (int i = 0; i < initialPoolSize; i++)
        {
            lastInstantiatedObject = Instantiate(explosionPrefab, explosionsParent) as GameObject;
            lastInstantiatedObject.gameObject.SetActive(false);
            internal_explosionPool.Add(lastInstantiatedObject.GetComponent<ExplosionBehaviour>());
        }
    }
    public GameObject GetFromPool_bullet()
    {
        for (int i = 0; i < internal_bulletPool.Count; i++)
        {
            if (!internal_bulletPool[i].activeInHierarchy)
            {
                return internal_bulletPool[i];
            }
        }

        lastInstantiatedObject = Instantiate(bulletPrefab, bulletsParent) as GameObject;
        lastInstantiatedObject.gameObject.SetActive(false);
        internal_bulletPool.Add(lastInstantiatedObject);
        return lastInstantiatedObject;
    }

    public GameObject GetFromPool_dmgNum()
    {
        for (int i = 0; i < internal_dmgnumPool.Count; i++)
        {
            if (!internal_dmgnumPool[i].activeInHierarchy)
            {
                return internal_dmgnumPool[i];
            }
        }

        lastInstantiatedObject = Instantiate(dmgnumPrefab, dmgNumbersParent) as GameObject;
        lastInstantiatedObject.gameObject.SetActive(false);
        internal_dmgnumPool.Add(lastInstantiatedObject);
        return lastInstantiatedObject;
    }
    public GameObject GetFromPool_partBurst()
    {
        for (int i = 0; i < internal_partBurstPool.Count; i++)
        {
            if (!internal_partBurstPool[i].activeInHierarchy)
            {
                return internal_partBurstPool[i];
            }
        }

        lastInstantiatedObject = Instantiate(partBurstPrefab, particleBurstsParent) as GameObject;
        lastInstantiatedObject.gameObject.SetActive(false);
        internal_partBurstPool.Add(lastInstantiatedObject);
        return lastInstantiatedObject;
    }
    public ExplosionBehaviour GetFromPool_explosion()
    {
        for (int i = 0; i < internal_explosionPool.Count; i++)
        {
            if (!internal_explosionPool[i].gameObject.activeInHierarchy)
            {
                return internal_explosionPool[i];
            }
        }

        lastInstantiatedObject = Instantiate(explosionPrefab, explosionsParent) as GameObject;
        lastInstantiatedObject.gameObject.SetActive(false);
        internal_explosionPool.Add(lastInstantiatedObject.GetComponent<ExplosionBehaviour>());
        return lastInstantiatedObject.GetComponent<ExplosionBehaviour>();
    }
}
