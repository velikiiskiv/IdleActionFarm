using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public int wheatBlockCount;
    public float speed;
    public float rotationSpeed;
    public float throwSpeed;
    public Joystick joystick;
    public Canvas joystickCanvas;
    public Transform ladderTarget;
    public Slider wheatBlockSlider;
    public TMP_Text wheatCountText;
    public TMP_Text coinCountText;
    public GameObject backPanel;
    public GameObject wheatPrefab;
    public GameObject wheatBlockPrefab;
    public GameObject sickle;
    public GameObject basket;
    public GameObject arrow;
    public GameObject roofHole;
    public Animator animator;
    private int coinCount;
    private bool IsMove;
    private bool IsDestroy;
    private CharacterController controller;
    private GameObject wheat;
    private GameObject wheatBlock;
    private Rigidbody rb;
    private Vector3 pos;
    private Vector3 pos1;
    private Vector3 pos2;
    private Vector3 pos3;

    void Start()
    {
        for (float z = 8.5f; z < 11; z++)
        {
            for (float x = 2.5f; x < 7; x+=1.3f)
            {
                wheat = Instantiate(wheatPrefab, new Vector3(x, 0.0f, z), Quaternion.identity);
            }
        }
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        IsDestroy = true;
    }
    void Update()
    {
       
        if (IsMove == false)
        {
            animator.SetBool("SadIdle", true);
            transform.Rotate(0, joystick.Horizontal * rotationSpeed, 0);
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            float curspeed = speed * joystick.Vertical;
            if (joystick.Vertical > 0)
            {
                controller.SimpleMove(forward * curspeed);
                animator.SetBool("Idle", false);
                animator.SetBool("Run", true);
            }
            else
            {
                animator.SetBool("Run", false);
                animator.SetBool("Idle", true);
            }
        }
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1f))
        {
            if (hit.collider.tag == "Wheat")
            {
                IsMove = true;
                wheat = hit.collider.gameObject;
                pos = wheat.transform.position;
                sickle.SetActive(true);
                animator.SetBool("WheatCut", true);
                Destroy(wheat.transform.GetChild(0).gameObject, 1.1f);
                Destroy(wheat, 3.5f);
                IsDestroy = false;
            }
        }
                
        if (IsDestroy == false && wheat == null)
        {
            wheatBlock = Instantiate(wheatBlockPrefab, new Vector3(pos.x, 1.5f, pos.z), Quaternion.identity);
            rb = wheatBlock.GetComponent<Rigidbody>();
            rb.AddForce(Vector3.left, ForceMode.Impulse);
            if(wheatBlockCount < 40)
            {
                StartCoroutine("BlockCoroutine");
            }
            
            IsMove = false;
            pos1 = pos;
            StartCoroutine("InstCoroutine");
            IsDestroy = true;
            animator.SetBool("WheatCut", false);
            animator.SetTrigger("Idle2");
            sickle.SetActive(false);
        }
    }
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Arrow")
        {
            if(arrow.activeSelf == true)
            {
                arrow.SetActive(false);
            }
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, ladderTarget.position - transform.position, 5, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            IsMove = true;
            joystickCanvas.enabled = false;
            animator.SetBool("Idle", false);
            animator.SetBool("UpStairs", true);
            animator.SetFloat("Stairs", 1);
            StartCoroutine("StairsCoroutine");
        }
    }
    IEnumerator StairsCoroutine()
    {
        yield return new WaitForSeconds(2.5f);
        animator.SetBool("UpStairs", true);
        animator.SetFloat("Stairs", -1);
        StartCoroutine("ThrowCoroutine");
        wheatBlockCount = 0;
        wheatBlockSlider.value = 0;
        wheatCountText.SetText("0/40");
        yield return new WaitForSeconds(2.3f);
        IsMove = false;
        joystickCanvas.enabled = true;
        if(coinCount > 699)
        {
            SceneManager.LoadScene("ExitMenu");
        }
    }
    IEnumerator BlockCoroutine()
    {
        yield return new WaitForSeconds(1f);
        wheatBlock.transform.position = new Vector3(basket.transform.position.x, basket.transform.position.y + 0.15f, basket.transform.position.z);
        wheatBlock.transform.parent = basket.transform;
        rb.isKinematic = true;
        arrow.SetActive(true);
        wheatBlockCount = basket.transform.childCount;
        wheatBlockSlider.value = wheatBlockCount;
        wheatCountText.SetText(wheatBlockCount.ToString() + "/40");
    }
    IEnumerator InstCoroutine()
    {
        yield return new WaitForSeconds(3.5f);
        pos2 = pos1;
        yield return new WaitForSeconds(3.5f);
        pos3 = pos2;
        yield return new WaitForSeconds(3.5f);
        wheat = Instantiate(wheatPrefab, pos3, Quaternion.identity);
    }
    IEnumerator ThrowCoroutine()
    {
        yield return new WaitForSeconds(0.015f);
        Vector3 wheatThrowDirection = roofHole.transform.position - basket.transform.position;
        Rigidbody rb1 = basket.transform.GetChild(0).GetComponent<Rigidbody>();
        rb1.isKinematic = false;
        rb1.transform.parent = null;
        rb1.AddForce(wheatThrowDirection * throwSpeed, ForceMode.Impulse);
        Destroy(rb1.gameObject, 2f);
        yield return new WaitForSeconds(0.015f);
        StartCoroutine("ThrowCoroutine");
        yield return new WaitForSeconds(0.2f);
        coinCount += 15;
        coinCountText.SetText(coinCount.ToString());
    }
   public void StartLevel()
    {
        backPanel.SetActive(false);
    }
}
