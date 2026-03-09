using System.Collections;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    private Enemy controller;

    private void Start()
    {
        controller = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (controller.state)
        {
            case Enemy.State.Idle:
                transform.Translate(controller.walkSpeed * 0.7f * Time.deltaTime * Vector3.right);

                if (Vector3.Distance(transform.position, controller.target) < 0.25f)
                {
                    transform.Rotate(0, 180, 0);
                    controller.idleWalkDistance = -controller.idleWalkDistance;
                    controller.target = new Vector3(controller.initialPos.x + controller.idleWalkDistance, transform.position.y, transform.position.z);
                }

                if(controller.distToPlayer <= controller.chaseDistance && controller.chase)
                    controller.state = Enemy.State.Chase;

                break;
            case Enemy.State.Chase:
                if(controller.player.transform.position.x > transform.position.x)
                    transform.eulerAngles = new Vector3(0, 0, 0);
                else
                    transform.eulerAngles = new Vector3(0, 180, 0);

                transform.Translate(controller.walkSpeed * 1.3f * Time.deltaTime * Vector3.right);

                if (controller.distToPlayer <= controller.attackDistance && controller.attack)
                    controller.state = Enemy.State.Attack;
                break;
            case Enemy.State.Attack:

                if(!controller.attacking && controller.attack)
                    StartCoroutine(Attack());

                break;
        }
    }

    private IEnumerator Attack()
    {
        controller.attacking = true;
        yield return new WaitForSeconds(controller.attackDelay);
        GameObject newProj = Instantiate(controller.projectile, transform.position, transform.rotation);
        int mult = 1;

        if(controller.player.transform.position.x < transform.position.x)
            mult = -1;

        float yDist = Mathf.Abs(controller.player.transform.position.y - transform.position.y);
        newProj.GetComponent<Rigidbody2D>().AddForce(new Vector2(controller.distToPlayer * mult * 1.25f, Mathf.Max(5, yDist * 1.5f)), ForceMode2D.Impulse);

        if (controller.distToPlayer > controller.attackDistance)
            controller.state = Enemy.State.Chase;

        controller.attacking = false;
    }
}
