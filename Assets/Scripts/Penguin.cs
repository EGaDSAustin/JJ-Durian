using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Penguin : MonoBehaviour
{
    private GameObject targetPenguin;
    private bool hasTarget = false;
    public float rotationSpeed = 500f;
    public float moveSpeed = 10f;

    public float baseHealth = 100;
    public float curHealth;
    private float targetAngle;

    Rigidbody rb;
    Animator animator;
    PlayerController player;

    bool playerClose = false;

    float sinOffset = 0;

    private void Start()
    {
        sinOffset = Random.Range(0, 100f);
        curHealth = baseHealth;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        player = FindObjectOfType<PlayerController>();
    }

    private void FixedUpdate()
    {
        if (!hasTarget || targetPenguin == null)
        {
            GameObject nearestPenguin = FindNearestPenguin();
            if (nearestPenguin != null)
            {
                SetTarget(nearestPenguin);
            }
        }
        else
        {
            //Attracked by fish
            playerClose = player.hasFish && Vector3.Distance(player.transform.position, transform.position) < 20;

            //Rotate();
            MoveTowards(targetPenguin);
            UpdateTargetRotation();

            //Jumping
            bool onGround = Physics.Raycast(new Ray(transform.position, Vector3.down), 1.5f);

            if (onGround && Random.Range(0, 50f) < .1f)
            {
                rb.velocity += Vector3.up * 10;
                animator.SetTrigger("Jump");
            }
            else
                animator.ResetTrigger("Jump");
            animator.ResetTrigger("Attack");
        }
    }

    private GameObject FindNearestPenguin()
    {
        GameObject[] allPenguins = GameObject.FindGameObjectsWithTag("Penguin");

        if (allPenguins.Length <= 1)
        {
            return null;
            // to be done
        }

        // to not include penguins this scrips attached to
        GameObject nearestPenguin = allPenguins[1];

        float nearestDistance = Vector3.Distance(transform.position, nearestPenguin.transform.position);
        foreach (GameObject curPenguin in allPenguins)
        {
            if (curPenguin != gameObject)
            {
                float distance = Vector3.Distance(transform.position, curPenguin.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPenguin = curPenguin;
                }
            }
        }
        //Debug.Log(nearestPenguin); // hmmm idk
        return nearestPenguin;
    }

    private void SetTarget(GameObject target)
    {
        hasTarget = true;
        targetPenguin = target;
    }

    private void UpdateTargetRotation()
    {
        if (targetPenguin != null)
        {
            Vector3 targetDirection = targetPenguin.transform.position - transform.position;
            if (playerClose)
                targetDirection = player.transform.position - transform.position;

            targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
            targetAngle += Mathf.Sin(Time.time * 4 + sinOffset) * 40;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, targetAngle + 180, 0), 2);
        }
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void MoveTowards(GameObject target)
    {
        Vector3 targetPosition = target.transform.position;

        if (playerClose)
            targetPosition = player.transform.position;

        Vector3 direction = (targetPosition - transform.position).normalized;

        direction += new Vector3(Mathf.Sin(Time.time * 4 + sinOffset), 0, Mathf.Sin(Time.time * 3.5f + sinOffset)) * 1.5f;

        rb.AddForce(direction.normalized * moveSpeed);
        //transform.Translate(direction.normalized * moveSpeed * Time.deltaTime, Space.World);

        //Sometimes resets target
        if (Random.Range(0, 50f) < .05f)
            hasTarget = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Penguin") || other.CompareTag("Player"))
        {
            //Debug.Log("bruh");
            if (Random.Range(0, 100f) < .1)
            {
                int damage = Random.Range(100, 260) * 4;
                //targetPenguin.TakeDamage(damage);
                //Launch Penguin Instead
                Vector3 launchDir = (other.transform.position - transform.position).normalized + Vector3.up * .5f;
                other.attachedRigidbody.AddForce(launchDir * damage);

                animator.SetTrigger("Attack");

                /*if (targetPenguin.curHealth <= 0)
                {
                    Destroy(other.gameObject);
                }*/
            }
        }
    }
    private void TakeDamage(int damage)
    {
        curHealth -= damage;
        //Debug.Log(gameObject.name + damage + curHealth);
    }
}
