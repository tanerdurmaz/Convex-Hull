using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera cam;
    float horizontalInput;
    float verticalInput;
    //float RotateHorizontalInput;
    //float RotateVerticalInput;
    float moveSpeed = 25f;
    float rotationSpeed = 0.3f;
    //Vector3 currentPosition;
    //Vector3 inputVector;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        //RotateHorizontalInput = Input.GetAxis("RotateHorizontal");
        //RotateVerticalInput = Input.GetAxis("RotateVertical");



        cam.transform.Translate(new Vector3(horizontalInput, Input.GetAxis("RotateVertical"), verticalInput) * moveSpeed * Time.deltaTime);

        //currentPosition = cam.transform.position;

        //inputVector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);

        cam.transform.Rotate(0, Input.GetAxis("RotateHorizontal") * rotationSpeed, 0.0f);
    }
}
