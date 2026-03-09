using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    public GameObject projectile;
    public float cooldown;
    private float cooldownTimer;
    private PlayerMovement movement;
    public bool canAttack;

    public int heal;
    public Transform healParent;
    public GameObject healIcon;

    public int currentHeal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        movement = GetComponent<PlayerMovement>();
        cooldownTimer = cooldown;

        currentHeal = 0;

        for (int i = 0; i < heal; i++)
            CreateHealIcon(i);

        canAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && cooldownTimer >= cooldown && canAttack)
        {
            cooldownTimer = 0;
            GameObject newProj = Instantiate(projectile, transform.position, transform.rotation);
            newProj.GetComponent<PlayerProjectile>().attack = this;

            if (Input.GetAxisRaw("Vertical") < 0 && !movement.grounded)
            {
                newProj.transform.Rotate(0, 0, -90);
            }
            else if (Input.GetAxisRaw("Vertical") > 0)
                newProj.transform.Rotate(0, 0, 90);
        }

        cooldownTimer += Time.deltaTime;
    }

    public void IncreaseHealCount()
    {
        currentHeal++;
        currentHeal = Mathf.Clamp(currentHeal, 0, heal);

        UpdateHealIcons();
    }

    public void IncreaseMaxHealCount()
    {
        heal++;
        CreateHealIcon(heal - 1);
    }

    private void CreateHealIcon(int i)
    {
        GameObject newIcon = Instantiate(healIcon, healParent.transform.position, healIcon.transform.rotation);
        newIcon.transform.SetParent(healParent.transform, false);
        newIcon.transform.GetChild(0).gameObject.SetActive(i < currentHeal);
    }

    public void UpdateHealIcons()
    {
        for (int i = 0; i < healParent.childCount; i++)
        {
            GameObject image = healParent.GetChild(i).GetChild(0).gameObject;
            image.SetActive(i < currentHeal);
            if(currentHeal < 4)
                image.GetComponent<Image>().color = Color.gray;
            else
                image.GetComponent<Image>().color = Color.white;
        }
    }
}
