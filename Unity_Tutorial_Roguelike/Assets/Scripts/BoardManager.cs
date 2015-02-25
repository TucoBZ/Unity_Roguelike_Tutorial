using UnityEngine;
using System; //Serializable Atribute: The Serializable attribute lets you embed a class with sub properties in the inspector.
using System.Collections.Generic; //Para usar listas
using Random = UnityEngine.Random; //Especificar porq as duas tem o mesmo nome

public class BoardManager : MonoBehaviour {

	[Serializable]
	public class Count 																			//Classe interna para a contagem de minimos e máximos de um objeto
	{
		public int minimum;
		public int maximum;

		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;

		}

	}
	
	public int columns = 8; 																	//Tamanho do board do jogo
	public int rows = 8;
	public Count wallCount = new Count (5,9); 													// Quantas paredes no max serão spawnadas, min de 5 e max de 9
	public Count foodCount = new Count (1,5); 													// Quantas comidas no max serão spawnadas, min de 1 e max de 5
	public GameObject exit; 																	// só há uma saída

	//Todos os objetos serão populados pelos Prefabs e serão spawnados randomicamente
	public GameObject[] floorTiles; 
	public GameObject[] wallTiles;
	public GameObject[] foodTiles;
	public GameObject[] enemyTiles;
	public GameObject[] outerWallTiles;


	private Transform boardHolder; 																//Para deixar o gameBoard limpo, todos os objetos serão spwanados como filho deste
	private List <Vector3> gridPositions = new List<Vector3>(); 								//Ajuda a trackiar um dos tiles se há algo lá ou não

	//Cria uma lista de pontos no mapa
	void InitialiseList()
	{
		gridPositions.Clear(); 																	//Limpa a lista de posições que temos

		for (int x=1; x < columns -1; x++) 														// de 1,1 a 6,1
		{
			for (int y=1; y < rows -1; y++) 													// de x,1 a x,6
			{
				gridPositions.Add(new Vector3(x,y,0f)); 										//Popula a nossa lista com todas as posições de 1,1 a 6,6
			}	
		}
	}

	//Cria a base do chão e as paredes externas do mapa
	void BoardSetup()
	{
		boardHolder = new GameObject ("Board").transform;

		for (int x= -1; x < columns +1; x++) 													//a board vai de -1,-1 a 8,8, porq terá uma volta a mais no board que sera a outerWall
		{
			for (int y= -1; y < rows +1; y++) 
			{
				GameObject toInstantiate = floorTiles[Random.Range (0, floorTiles.Length)]; 	//setará um tile de chão naquele lugar, escolido randomicamente entre os tiles disponiveis

				if (x== -1 || x == columns || y == -1 || y == rows) 							//Mas se estiver em algum dos locais de outerWall, ali será um outerWall, escolido randomicamente entre os tiles disponiveis
					toInstantiate = outerWallTiles[Random.Range (0, outerWallTiles.Length)];

				GameObject instance = Instantiate(toInstantiate, new Vector3(x,y,0f), Quaternion.identity) as GameObject; // cast do nosso objeto criado para um Objeto Concreto e colocado-o como filho da boardHolder

				instance.transform.SetParent(boardHolder);
			}	
		}
	}

	//Devolve uma posição randomica do mapa ente 1,1 e 6,6
	Vector3 RandomPosition()
	{
		int randomIndex = Random.Range (0, gridPositions.Count); 							//pega uma posição aleatória na lista de posições
		Vector3 randomPosition = gridPositions [randomIndex]; 								//Pega o vetor baseado nesta posição
		gridPositions.RemoveAt (randomIndex); 												//Retira este ponto da lista, para não ser criado outro objeto no mesmo local
		return randomPosition; 																//Devolve o ponto 
	}

	//Coloca objeto randomicamente no mapa
	void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
	{
		int objectCount = Random.Range (minimum, maximum + 1); 								//O numeros de objetos a serem gerados depende da quant. min e max +1  passadas por parâmetro

		for (int i = 0; i < objectCount; i++)												
		{
			Vector3 randomPosition = RandomPosition(); 										// pega uma posição aleatória no board
			GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)]; 			// escole qual dos tiles do array será selecionado
			Instantiate (tileChoice, randomPosition, Quaternion.identity);					// Cria uma instancia daquele tile no local aleatório
		}
	}

	//Inicializa uma fase
	public void SetupScene(int level)
	{
		BoardSetup(); 																			//Cria a base do board
		InitialiseList(); 																		//Inicializa a lista de pontos 
		LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);					//Coloca de 5 a 9 paredes no mapa aleatoriamente
		LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);					//Coloca de 1 a 5 comidas no mapa aleatoriamente
		int enemyCount = (int)Mathf.Log (level, 2f);											//A quantidade de inimigos depende de log de 2 dependendo do de qual level está
		LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);								//Coloca inimigos no mapa aleatóriamente 
		Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0F), Quaternion.identity);		//Instancia a saída
	}
}
