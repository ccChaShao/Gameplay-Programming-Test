using System;
using System.Collections;
using System.Collections.Generic;
using Charsiew;
using Sirenix.OdinInspector;
using UnityEngine;

public class BulletControler : MonoBehaviour
{
    public string poolTag;
    public float damge;
    public float bulletSpeed;
    public float aliveTime;
    public LayerMask colliderLayer;
    
    private Coroutine m_aliveCoroutine;

    public void DataInit(string poolTag,float bulletSpeed, float damge, float aliveTime, LayerMask colliderLayer)
    {
        this.poolTag = poolTag;
        this.bulletSpeed = bulletSpeed;
        this.damge = damge;
        this.aliveTime = aliveTime;
        this.colliderLayer = colliderLayer;

        m_aliveCoroutine = StartCoroutine(IEAlive());
    }

    private void Update()
    {
        // 沿子弹自身的Z轴（正前方）移动
        transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);
    }

    private void OnDestroy()
    {
        if (m_aliveCoroutine != null)
            StopCoroutine(m_aliveCoroutine);
    }
    
    

    private IEnumerator IEAlive()
    {
        yield return new WaitForSeconds(aliveTime);
        ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
    }
}
