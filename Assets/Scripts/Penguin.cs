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

    private void Start()
    {
        curHealth = baseHealth;
    }

    private void FixedUpdate()
    {
        if (!hasTarget)
        {
            GameObject nearestPenguin = FindNearestPenguin();
            if (nearestPenguin != null)
            {
                SetTarget(nearestPenguin);
            }
        }
        else
        {
            Rotate();
            MoveTowards(targetPenguin);
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
        Debug.Log(nearestPenguin); // hmmm idk
        return nearestPenguin;
    }

    private void SetTarget(GameObject target)
    {
        hasTarget = true;
        targetPenguin = target;
    }

    /*private void UpdateTargetRotation()
    {
        if (targetPenguin != null)
        {
            Vector3 targetDirection = targetPenguin.transform.position - transform.position;
            targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
        }
    }*/

    private void Rotate()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void MoveTowards(GameObject target)
    {
        Vector3 targetPosition = target.transform.position;
        Vector3 direction = targetPosition - transform.position;
        transform.Translate(direction.normalized * moveSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Penguin"))
        {
            Debug.Log("bruh");
            Penguin targetPenguin = other.GetComponent<Penguin>();
            if (targetPenguin != null)
            {
                int damage = Random.Range(10, 26);
                targetPenguin.TakeDamage(damage);

                if (targetPenguin.curHealth <= 0)
                {
                    Destroy(other.gameObject);
                }
            }
        }
    }
    private void TakeDamage(int damage)
    {
        curHealth -= damage;
        Debug.Log(gameObject.name + damage + curHealth);
    }
}
