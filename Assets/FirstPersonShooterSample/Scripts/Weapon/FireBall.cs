using UnityEngine;

public class FireBall : MonoBehaviour
{
    public GameObject hitParticlePrefab;
    public float damage = 2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
//    void  
    void OnTriggerEnter(Collider hit) {
        
        //何かのTagがEnemyだったとき
        if(hit.gameObject.CompareTag("OtherPlayer"))
        {
            //敵の体力を管理しているものがあるか
            PlayerLocalModel model = hit.transform.GetComponent<PlayerLocalModel>();
            if(model)
            {
                model.TryDamage(damage);
                Destroy(gameObject);
            }
        }
        if(hit.gameObject.CompareTag("Core"))
        {
            //敵の体力を管理しているものがあるか
            CoreLocalModel coreModel = hit.transform.GetComponent<CoreLocalModel>();
            if(coreModel && !coreModel.owned && !coreModel.transporting)
            {
                coreModel.TryDamage(damage);
                Destroy(gameObject);
            }
        }
        //パーティクルを出現させる
        if(hitParticlePrefab)
        {
            Vector3 triggerPoint = hit.ClosestPoint(transform.position);
            GameObject obj = Instantiate(hitParticlePrefab);
            obj.transform.position = triggerPoint;
            //obj.transform.rotation = Quaternion.LookRotation(-hit.normal);
        }
        
    }
}
