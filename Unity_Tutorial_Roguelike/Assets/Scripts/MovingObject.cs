using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour { 		//Abstract permite criar classes que são incompletas e devem ser herdadas de seus filhos

	public float moveTime = 0.1f;
	public LayerMask blockingLayer;							//Uma das layer criadas no jogo

	private BoxCollider2D boxCollider;
	private Rigidbody2D rb2D;
	private float inverseMoveTime;							//Para deixar o movimento mais fluido

	// Use this for initialization
	protected virtual void Start () {						//Pode ser reescritas pelas as classes que herdam desta
	
		boxCollider = GetComponent<BoxCollider2D>();
		rb2D = GetComponent<Rigidbody2D>();

		inverseMoveTime = 1f / moveTime;
	}

	protected IEnumerator SmoothMovement (Vector3 end){

		float sqrRemainingDistance = (transform.position - end).sqrMagnitude; //Computacionalmente melhor que magnitude

		while (sqrRemainingDistance > float.Epsilon){		//Numero muito pequeno, quase 0
																												//Time.deltaTime é tempo em segundos, que levou para completar o ultimo frame, usado para mover objetos em m/s em vez de m/frame
			Vector3 newPosition = Vector3.MoveTowards (rb2D.position, end, inverseMoveTime * Time.deltaTime); 	//Move em linha reta em uma direção entre a posição atual e o fim, com uma velocidade baseada no inverseMoveTime e o deltaTime
			rb2D.MovePosition(newPosition);
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			yield return null; 								//Assegura que no próximo frame ele começará deste ponto, rodando frame por frame, em vez de uma vez só

		}
	}
	
	protected abstract void OnCanMove <T> (T component)		//Pode ser reescrita, e por ser abstrata não precisa de brackets "{}" 
		where T : Component; 
}
