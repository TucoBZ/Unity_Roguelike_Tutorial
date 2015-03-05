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

	protected bool Move(int xDir, int yDir, out RaycastHit2D hit){

		Vector2 start = transform.position;					//transform.position é um Vector3, mas quando feito casting para Vector2 ele descarta o eixo z
		Vector2 end = start + new Vector2(xDir, yDir);

		boxCollider.enabled = false; 						//Para não bater no proprio collider
		hit = Physics2D.Linecast (start, end, blockingLayer);	//Verifica em linha reta se há algum Obstaculo entre o ponto start e end
		boxCollider.enabled = true;

		if (hit.transform == null) {						//Se não encontrou objetos no caminho
				
			StartCoroutine(SmoothMovement (end));
			return true; 									//É possivel movimentar
		}

		return false;										//Não é possivel movimentar

	}

	protected IEnumerator SmoothMovement (Vector3 end){

		float sqrRemainingDistance = (transform.position - end).sqrMagnitude; //Computacionalmente melhor que magnitude

		while (sqrRemainingDistance > float.Epsilon){		//Ira verificar até q sqr seja um Numero muito pequeno, quase 0
																												//Time.deltaTime é tempo em segundos, que levou para completar o ultimo frame, usado para mover objetos em m/s em vez de m/frame
			Vector3 newPosition = Vector3.MoveTowards (rb2D.position, end, inverseMoveTime * Time.deltaTime); 	//Move em linha reta em uma direção entre a posição atual e o fim, com uma velocidade baseada no inverseMoveTime e o deltaTime
			rb2D.MovePosition(newPosition);
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			yield return null; 								//Assegura que no próximo frame ele começará deste ponto, rodando frame por frame, em vez de uma vez só

		}
	}

	protected virtual void AttemptMove <T> (int xDir, int yDir)		//o Parametro generico T é especificado para determinar quando bloqueado, 
		where T : Component 											//No caso do jogador são as paredes, no caso dos inimigos é o jogador. Especificando q T será um Component
	{
		RaycastHit2D hit;
		bool canMove = Move (xDir, yDir, out hit);

		if (hit.transform == null)
			return;

		T hitComponent = hit.transform.GetComponent<T> ();

		if (!canMove && hitComponent != null) {					//Quer dizer que o caminho tem um obstaculo
			OnCantMove(hitComponent);
		}
	}

	protected abstract void OnCantMove <T> (T component)		//Pode ser reescrita, e por ser abstrata não precisa de brackets "{}" 
		where T : Component; 
}
