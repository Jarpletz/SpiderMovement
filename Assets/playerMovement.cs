using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class playerMovement : MonoBehaviour
{
   [SerializeField] float movementForce;
   [SerializeField] float downForce;
   [SerializeField] float rotateLerpSpeed;
   Rigidbody2D rb;
   legMovement lM;
   Vector3 normalVector;

    // Start is called before the first frame update
    void Start()
    {
      rb = GetComponent<Rigidbody2D>();
      lM = GetComponent<legMovement>();
   }

   // Update is called once per frame
   void FixedUpdate()
   {

      if (!lM.anthingIsStepping)
      {
         float dx = lM.legs[1].target.transform.position.x - lM.legs[0].target.transform.position.x;
         float dy = lM.legs[1].target.transform.position.y - lM.legs[0].target.transform.position.y;
         Debug.Log("(" + dx + "," + dy + ")");
         Debug.DrawRay(transform.position, new Vector3(dy,-dx)*5,Color.yellow);
         Debug.DrawRay(lM.legs[0].target.transform.position, new Vector3(dx,dy) , Color.magenta);
         normalVector = Vector3.Normalize(new Vector3(dy, -dx));
      }

      rb.transform.up = Vector2.Lerp(rb.transform.up, normalVector,rotateLerpSpeed);

      //rb.transform.position = rb.transform.position + rb.transform.up;

      rb.AddForce(Input.GetAxis("Horizontal") * movementForce * rb.transform.right);
      rb.AddForce(-rb.transform.up * downForce);
      
      //The psition in the direction opposite the norrmal vector must be the point of the hit +1

   }
}
