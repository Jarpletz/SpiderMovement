using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class legMovement : MonoBehaviour
{
   [SerializeField] LineRenderer[] lines;
   [SerializeField] Transform[] targets;
   [SerializeField] Transform[] bounds;

   [SerializeField] LayerMask layerMask;

   [SerializeField] float radius;
   [SerializeField] float stepSize;
   [SerializeField] float lerpSpeed;
   [SerializeField] int numRays;
   public bool anthingIsStepping;

   float rayDirectionSign=1;

   [SerializeField] EdgeCollider2D platform;
   List<Vector2> targetPos = new List<Vector2>();


   Rigidbody2D rb;

   public class Leg
   {
      public Transform target;
      public Transform bound;
      public Vector3 lastPos;
      public Vector3 newPos;
      public bool isStepping = false;

      public Leg(Transform t, Transform m)
      {
         target = t; bound = m;
         lastPos = target.position;
      }
   }
   public List<Leg> legs = new List<Leg>();
   

   void Start()
   {
      rb = GetComponent<Rigidbody2D>();

      for (int i = 0; i < targets.Length; i++)
      {
         legs.Add(new Leg(targets[i], bounds[i]));
      }
      targetPos.Add((Vector2)legs[0].target.position);
      targetPos.Add((Vector2)legs[1].target.position);

   }

   private void FixedUpdate()
   {
      //Figure out the direction it draws the raycasts for where to place the feet
      if (rb.velocity.magnitude > 0.001)
      {
         rayDirectionSign = Vector3.Dot(rb.transform.right, rb.velocity);
         rayDirectionSign /= -Mathf.Abs(rayDirectionSign); // 1 or -1
      }
      

      for (int i = 0; i < legs.Count; i++)
      {
         if ( !anthingIsStepping && (Vector3.Distance(legs[i].target.position, legs[i].bound.position) > stepSize || Vector3.Distance(legs[i].target.position, Vector3.zero) < radius))
         {
            legs[i].isStepping = true;
            anthingIsStepping = true;
         }//if no legs are stepping and outside max Step range, start a new step
         

         if (!legs[i].isStepping && legs[i].target.position != legs[i].lastPos){

            keepLegStationary( legs[i]);  

         }//if the leg isn't stepping, keep target still relative to world

         if (legs[i].isStepping){

            moveLeg(legs[i]);

         }//if the leg is stepping, lerp target & update point it is lerping to
         
         if ( Vector3.Distance(legs[i].target.position, legs[i].newPos) < 0.02 && legs[i].isStepping)
         {

            legs[i].isStepping = false;
            anthingIsStepping = false;

         }//if stepping and rly close to newPos, stop stepping


         lines[i].SetPosition(1, legs[i].target.localPosition);
         //getNextPosition(legs[i]);

      }

      targetPos[0] = (Vector2)legs[0].target.position;
      targetPos[1] = (Vector2)legs[1].target.position;
      if (legs[0].isStepping || legs[1].isStepping)
      {
         platform.enabled = false;
      }
      else platform.enabled = true;
      Debug.Log(platform.SetPoints(targetPos));


   }

   private void OnDrawGizmos()
   {
      foreach(Transform b in bounds)
      {
         Gizmos.color = UnityEngine.Color.red;
         Gizmos.DrawWireSphere(b.position, radius);
      }
      
   }

   private void keepLegStationary( Leg leg)
   {
      leg.target.position -= (leg.target.position - leg.lastPos);
      leg.lastPos = leg.target.position;
   }

   private void moveLeg( Leg leg)
   {
      leg.newPos = getNextPosition(leg);// leg.bound.position + (new Vector3(rb.velocity.x, rb.velocity.y, 0)).normalized * stepSize;
      leg.target.position = Vector3.Lerp(leg.target.position, leg.newPos, lerpSpeed);
      leg.lastPos = leg.target.position;
   }

   private Vector3 getNextPosition(Leg leg)
   {

      RaycastHit2D [] hits = new RaycastHit2D[numRays];

      float directionOfCheck = rb.velocity.x / Mathf.Abs(rb.velocity.x);

      for (int i = 0; i < numRays; i++)
      {
         float angle = (transform.rotation.z) + (i * (2 * Mathf.PI) / numRays * rayDirectionSign )+(Mathf.PI);
         Debug.Log(rayDirectionSign);
         Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));


         //Debug.Log(dir);
         hits[i] = Physics2D.Raycast(leg.bound.position, dir, stepSize, layerMask);
         
         if (hits[i].collider != null)
         {
            
            Debug.DrawLine(leg.bound.position, hits[i].point, UnityEngine.Color.green);

            return hits[i].point;

         }
         
      }
      return leg.lastPos;
   }

}
