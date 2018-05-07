using UnityEngine;
using System.Collections;

public struct Square{
   
	public string waypoint;
	public bool isSafe;
	
	public string player;
	public int G;
};

public class Game_Simulator
{
    public struct TestPlayer
    {
        public string color;
        public int startingIndex;
        public int homeIndex;
        public int Available;
        public int Pug;
        public bool canGoHome;
        public int[] Geeti;

        public TestPlayer(Square[] Board, string C, int SI, int HI, int A)
        {

            color = C;
            startingIndex = SI;
            homeIndex = HI;
            Available = A;
            Pug = 0;

            canGoHome = true;
            
            Geeti = new int[Available];

            for (int i = 0; i < Available; i++)
            {
                Geeti[i] = (startingIndex + i) % 76;

                Board[(startingIndex + i) % 76].player = color;
                Board[(startingIndex + i) % 76].G = i;
            }

        }

        public void Random_Move(int dice, Square[] Board, TestPlayer RP, TestPlayer RB, TestPlayer RY, TestPlayer RG, int move = -1)
        {

            int j, prev, s;
            
            if (move == -1)
				move = Random.Range(0, Available);
                
            prev = Geeti[move];
            for (j = 0; j < dice; j++)
            {
                if (Geeti[move] == homeIndex + 6)
                {
                    Pug++;
                    int[] temp = new int[Available - 1];

                    for (int a = 0, b = 0; a < Available; a++)
                    {
                        Board[Geeti[a]].player = "X";
                        Board[Geeti[a]].G = -1;

                        if (a != move)
                            temp[b++] = Geeti[a];
                    }

                    Geeti = null;
                    Available--;
                    Geeti = new int[Available];

                    for (int a = 0; a < Available; a++)
                    {
                        Geeti[a] = temp[a];
                        Board[Geeti[a]].player = color;
                        Board[Geeti[a]].G = a;
                    }

                    temp = null;

                    Board[prev].player = "X";
                    Board[prev].G = -1;
                    return;
                }

                if (Geeti[move] == homeIndex && canGoHome)			//HOME
                    Geeti[move] = (Geeti[move] + 1);
                else if (Geeti[move] % 19 == 6)
                    Geeti[move] = (Geeti[move] + 7);
                else
                    Geeti[move] = (Geeti[move] + 1) % 76;
            }

            while (Board[Geeti[move]].player == color && (Geeti[move] < homeIndex || Geeti[move] > homeIndex + 6))
            {     //Avoid 2 Geetis of same color on same index 
                if (Geeti[move] == homeIndex && canGoHome)
                    Geeti[move] = (Geeti[move] + 1);
                else if (Geeti[move] % 19 == 6)
                    Geeti[move] = (Geeti[move] + 7);
                else
                    Geeti[move] = (Geeti[move] + 1) % 76;
            }

            if (Board[Geeti[move]].player != "X" && Board[Geeti[move]].player != color && !Board[Geeti[move]].isSafe)
            {

                if (Board[Geeti[move]].player == "Red")
                {
                    for (s = RP.startingIndex; Board[s].player != "X"; s = (s + 1) % 76) ;
                    RP.Geeti[Board[Geeti[move]].G] = s;
                }

                else if (Board[Geeti[move]].player == "Blue")
                {
                    for (s = RB.startingIndex; Board[s].player != "X"; s = (s + 1) % 76) ;
                    RB.Geeti[Board[Geeti[move]].G] = s;
                }
                else if (Board[Geeti[move]].player == "Yellow")
                {
                    for (s = RY.startingIndex; Board[s].player != "X"; s = (s + 1) % 76) ;
                    RY.Geeti[Board[Geeti[move]].G] = s;
                }

                else if (Board[Geeti[move]].player == "Green")
                {
                    for (s = RG.startingIndex; Board[s].player != "X"; s = (s + 1) % 76) ;
                    RG.Geeti[Board[Geeti[move]].G] = s;
                }

            }

            Board[prev].player = "X";
            Board[prev].G = -1;

            Board[Geeti[move]].player = color;
            Board[Geeti[move]].G = move;


        }

        public void Genetic_Move(int dice, Square[] Board, TestPlayer RP, TestPlayer RB, TestPlayer RY, TestPlayer RG, Chromosome C)
        {


            float[] Fitness_Score = new float[Available];

            int newIndex, t, i, j;
            for (i = 0; i < Available; i++)
            {
                Fitness_Score[i] = (float)0.0;

                newIndex = Geeti[i];

                for (j = 0; j < dice; j++)
                {
                    if (newIndex == homeIndex + 6){
                     	j++;
						break;
					}

                    if (newIndex == homeIndex && canGoHome)			//HOME
                        newIndex = newIndex + 1;
                    else if (newIndex % 19 == 6)
                        newIndex = newIndex + 7;
                    else
                        newIndex = (newIndex + 1) % 76;
                }

                if (j == dice)
                {

                    /***  1)  Beat_Another_Pawn   ***/
                    if (Board[newIndex].player != "X" && Board[newIndex].player != color && !Board[newIndex].isSafe)
                        Fitness_Score[i] += C.Genes[0];

                    /***   2) Another_Pawn_Betable_In_Next_Round   ***/
                    t = newIndex;
                    for (j = 0; j < 6; j++)
                    {
                        if (t == homeIndex && canGoHome)
                            break;
                        else if (t % 19 == 6)
                            t = t + 7;
                        else
                            t = (t + 1) % 76;

                        if (Board[t].player != "X" && Board[t].player != color && !Board[t].isSafe)
                        {
                            Fitness_Score[i] += C.Genes[1];
                            break;
                        }
                    }

                    /***   3) Pug_The_Pawn   ***/
                    if (newIndex == homeIndex + 6)
                        Fitness_Score[i] += C.Genes[2];


                    /***   4) Get_Pawn_Into_Home   ***/
                    if (newIndex > homeIndex && newIndex < homeIndex + 6)
                        Fitness_Score[i] += C.Genes[3];

                    /***   5)  Get_Pawn_Onto_Safe_Spot   ***/
                    if (newIndex % 19 == 3 || newIndex % 19 == 14 || newIndex % 19 == 7 || newIndex % 19 == 8 || newIndex % 19 == 9 || newIndex % 19 == 10 || newIndex % 19 == 11)
                        Fitness_Score[i] += C.Genes[4];

                    /***   6) Move_Own_Pawn_Into_Potentially_Beatable_Position   ***/
                    t = newIndex;
                    for (j = 0; j < 6; j++)
                    {
                        if (Board[t].isSafe)
                            break;

                        if (t % 19 == 13)
                            t = t - 7;
                        else if (t == 0)
                            t = 75;
                        else
                            t = t - 1;

                        if (Board[t].player != "X" && Board[t].player != color)
                        {
                            Fitness_Score[i] += C.Genes[5];
                            break;
                        }
                    }

                    /***   7) Move_Pawn_On_Begin_Field_Of_Another_Player   ***/
                    if (Mathf.Abs(newIndex - homeIndex) > 15 && (newIndex % 19 == 14 || newIndex % 19 == 15 || newIndex % 19 == 16 || newIndex % 19 == 17 || newIndex % 19 == 18 || newIndex % 19 == 0 || newIndex % 19 == 1))
                        Fitness_Score[i] += C.Genes[6];


                    /***   8   ***/
                }

                int largest = 0;

                for (i = 1; i < Available; i++)
                    if (Fitness_Score[i] > Fitness_Score[largest])
                        largest = i;

                Random_Move(dice, Board, RP, RB, RY, RG, largest);
            }
        }

    };

    public string SIMULATE(Chromosome C)
    {

        Square[] Board = new Square[76];

        for (int i = 0; i < 76; i++)
        {
            Board[i].player = "X";
            Board[i].G = -1;

            if (i % 19 == 3 || i % 19 == 14 || i % 19 == 7 || i % 19 == 8 || i % 19 == 9 || i % 19 == 10 || i % 19 == 11)
                Board[i].isSafe = true;
            else
                Board[i].isSafe = false;
        }

        int totalGeetiyan = 4;

        TestPlayer RP = new TestPlayer(Board, "Red", 14, 6, totalGeetiyan);
        TestPlayer RB = new TestPlayer(Board, "Blue", 33, 25, totalGeetiyan);
        TestPlayer RY = new TestPlayer(Board, "Yellow", 52, 44, totalGeetiyan);
        TestPlayer RG = new TestPlayer(Board, "Green", 71, 63, totalGeetiyan);

        int dice, turn = 0;

        int a = 0;
        while (a < 1)
        {
            a--;                                //to avoid while(true)

            dice = Random.Range(1, 7);

            switch (turn % 4)
            {
                case 0:
                    RP.Genetic_Move(dice, Board, RP, RB, RY, RG, C);
                    if (RP.Pug == totalGeetiyan)
                        return RP.color;
                    break;


                case 1:
                    RB.Random_Move(dice, Board, RP, RB, RY, RG);
                    if (RB.Pug == totalGeetiyan)
                        return RB.color;
                    break;


                case 2:
                    RY.Random_Move(dice, Board, RP, RB, RY, RG);
                    if (RY.Pug == totalGeetiyan)
                        return RY.color;
                    break;
                case 3:
                    RG.Random_Move(dice, Board, RP, RB, RY, RG);
                    if (RG.Pug == totalGeetiyan)
                        return RG.color;
                    break;


            }
            turn++;
        }
        return "X";
    }
};

public class Chromosome
{

    // 1  float Beat_Another_Pawn;
    // 2  float Another_Pawn_Betable_In_Next_Round;
    // 3  float Pug_The_Pawn;
    // 4  float Get_Pawn_Into_Home;	
    // 5  float Get_Pawn_Onto_Safe_Spot;
    // 6  float Move_Own_Pawn_Into_Potentially_Beatable_Position;
    // 7  float Move_Pawn_On_Begin_Field_Of_Another_Player;
    
	//float Get_Out_Of_Position_Where_Own_Pawn_Is_Potentially_Beatable_By_Another_Player;  X
    //float Get_Pawn_On_Own_Begin_Field;	X			
    //float Get_Pawn_Close_To_Home;		    X		
    //float Get_Pawn_Near_An_Enemy;         X

    public int No_Of_Genes;
    public float[] Genes;
    public int Fitness;

    public Chromosome()
    {
        No_Of_Genes = 7;
        Genes = new float[No_Of_Genes];
    
        for (int i = 0; i < No_Of_Genes; i++){            
			Genes[i] = Random.value;			
		}
		Fitness = -1;
    }
};

public class GA
{

    void SORT(Chromosome[] arr, int n)
    {

        Chromosome temp = new Chromosome();
        int largest;
        for (int j = 0; j < n - 1; j++)
        {
            largest = j;
            for (int i = j + 1; i < n; i++)
            {
                if (arr[i].Fitness > arr[largest].Fitness)
                    largest = i;
            }
            temp = arr[j];
            arr[j] = arr[largest];
            arr[largest] = temp;
        }


    }   

    public void CROSS_OVER(Chromosome[] Population, int POPULATION_SIZE, int iter)
    {

        int MOTHER, FATHER, WEAKEST_1 = POPULATION_SIZE - 2, WEAKEST_2 = POPULATION_SIZE - 1, j, breakPoint;

        MOTHER = Random.Range(0, POPULATION_SIZE);
        
		FATHER = Random.Range(1, POPULATION_SIZE);       
        
		while (FATHER == MOTHER)
            FATHER = Random.Range(1, POPULATION_SIZE); ;
        

        Chromosome offspring1 = new Chromosome();
        Chromosome offspring2 = new Chromosome();

        breakPoint = Random.Range(1, offspring1.No_Of_Genes);

        for (j = 0; j < breakPoint; j++)
        {
            offspring1.Genes[j] = Population[MOTHER].Genes[j];
            offspring2.Genes[j] = Population[FATHER].Genes[j];
        }

        for (j = breakPoint; j < offspring1.No_Of_Genes; j++)
        {
            offspring1.Genes[j] = Population[FATHER].Genes[j];
            offspring2.Genes[j] = Population[MOTHER].Genes[j];
        }

        if (iter % 4 == 0)
        {
            MUTATION(offspring1);
            MUTATION(offspring2);
        }
		
		offspring1.Fitness=CALCULATE_FITNESS(offspring1);
        Population[WEAKEST_1] = offspring1;						//Deleting weakest individuals and placing the new offsprings
        
		offspring2.Fitness=CALCULATE_FITNESS(offspring2);
        Population[WEAKEST_2] = offspring2;
        
        Chromosome key = Population[WEAKEST_1];

        for (j = WEAKEST_1 - 1; j >= 0 && Population[j].Fitness < key.Fitness; j--)
            Population[j + 1] = Population[j];

        Population[j+1] = key;

        key = Population[WEAKEST_2];

        for (j = WEAKEST_2 - 1; j >= 0 && Population[j].Fitness < key.Fitness; j--)
            Population[j + 1] = Population[j];

        Population[j+1] = key;


    }

    public void MUTATION(Chromosome C)
    {
        int Mutation_Position = Random.Range(0, C.No_Of_Genes);
        float Mutation_Value = Random.value;
        C.Genes[Mutation_Position] = Mutation_Value;
    }

    public int CALCULATE_FITNESS(Chromosome C)
    {
        int Fitness = 0;
        Game_Simulator G= new Game_Simulator();
        for (int i = 1; i <= 100; i++)
            if (G.SIMULATE(C) == "Red")
                Fitness++;
        
		return Fitness;
    }

    public Chromosome RUN_GENETIC_ALGORITHM()
    {

        const int POPULATION_SIZE = 100;
        const int STOP = 100;
        Chromosome[] Population = new Chromosome[POPULATION_SIZE];

        for (int i = 0; i < POPULATION_SIZE; i++)
        {
            Population[i] = new Chromosome();
            Population[i].Fitness = CALCULATE_FITNESS(Population[i]);
        }

        SORT(Population, POPULATION_SIZE);

       
        for (int i = 0; i < STOP; i++)
        {
            CROSS_OVER(Population, POPULATION_SIZE, i);
        }
      
    	return Population[0];
	}

};

public struct Player{
	
	public string color;
	public int startingIndex;
	public int homeIndex;
	public int Nikli;
	public int Pug;
	public bool canGoHome;
	public SB []Geeti;
	
	public Player(string C,int SI,int HI,string G1,string G2,string G3,string G4,string i1,string i2,string i3,string i4){
	
		color=C;
		startingIndex=SI;
		homeIndex=HI;
		Nikli=0;
		Pug=0;
		canGoHome=false;
		
		Geeti=new SB[4];
		
		
		Geeti[0]=GameObject.Find(G1).collider.gameObject.GetComponent<SB>();
		Geeti[0].initialPos=GameObject.Find(i1).transform.position;
		Geeti[0].pos=Geeti[0].initialPos;
		Geeti[0].gameObj=GameObject.Find(G1);
		Geeti[0].color=color;
		
		Geeti[1]=GameObject.Find(G2).collider.gameObject.GetComponent<SB>();
		Geeti[1].initialPos=GameObject.Find(i2).transform.position;
		Geeti[1].pos=Geeti[1].initialPos;
		Geeti[1].gameObj=GameObject.Find(G2);
		Geeti[1].color=color;
		
		Geeti[2]=GameObject.Find(G3).collider.gameObject.GetComponent<SB>();
		Geeti[2].initialPos=GameObject.Find(i3).transform.position;
		Geeti[2].pos=Geeti[2].initialPos;
		Geeti[2].gameObj=GameObject.Find(G3);
		Geeti[2].color=color;
		
		Geeti[3]=GameObject.Find(G4).collider.gameObject.GetComponent<SB>();
		Geeti[3].initialPos=GameObject.Find(i4).transform.position;
		Geeti[3].pos=Geeti[3].initialPos;
		Geeti[3].gameObj=GameObject.Find(G4);
		Geeti[3].color=color;
	}
	
	public bool MoveGeeti(int dice,Square []Board,int n,ref Player RP,ref Player RB,ref Player RY,ref Player RG){
	
		GameObject.Find("T_Error").guiText.text="";
		
		if(Geeti[n-1].index==-1){
			if(dice==6){
				Geeti[n-1].index=startingIndex;
				Nikli++;	
				Board[Geeti[n-1].index].player=color;
				Board[Geeti[n-1].index].G=n;
				
				Vector3 p=GameObject.Find(Board[Geeti[n-1].index].waypoint).collider.gameObject.transform.position;
				Geeti[n-1].ChangePosition(p);
		
				return true;
			}
			else{
				GameObject.Find("T_Error").guiText.material.color=Color.red;
				GameObject.Find("T_Error").guiText.text="Choose Correct Piece!!!";
				return false;
			}
		}
		else{
			int prev=Geeti[n-1].index;
			int newIndex=Geeti[n-1].index;
			
			bool flag=false;
			int i;
			for(i=1;i<=dice;i++){
				if(newIndex%18==17 && dice==i){
					i++;
					flag=true;
					break;
				}
				else if(newIndex%18==17 && dice-i>1)
					break;
				else if(newIndex%18==12)
					newIndex=(newIndex+6)%72;
				else if(newIndex==homeIndex && canGoHome)			//HOME
					newIndex=(newIndex+7)%72;
				else
					newIndex=(newIndex+1)%72;
			}
			
			
			if(i==dice+1){										//Successful Complete Turn
				
				Board[prev].player="X";
				Board[prev].G=0;
				
				Geeti[n-1].index=newIndex;
				
				if(flag){										//Pug Gi
					
					GameObject.Destroy(Geeti[n-1].gameObj);
					Nikli--;
					Pug++;
					
					Geeti[n-1].index=-1;
					Geeti[n-1].isPug=true;
					return true;
				}
				
				else if(Board[Geeti[n-1].index].player!="X" && Board[Geeti[n-1].index].player!=color && !Board[Geeti[n-1].index].isSafe){
			
					canGoHome=true;
					
					if(Board[Geeti[n-1].index].player=="Red"){
						RP.Nikli--;
						RP.Geeti[Board[Geeti[n-1].index].G-1].ResetPosition();
					}
					
					else if(Board[Geeti[n-1].index].player=="Blue"){
						RB.Nikli--;
						RB.Geeti[Board[Geeti[n-1].index].G-1].ResetPosition();
					}
		
					else if(Board[Geeti[n-1].index].player=="Yellow"){
						RY.Nikli--;
						RY.Geeti[Board[Geeti[n-1].index].G-1].ResetPosition();
					}
					
					else if(Board[Geeti[n-1].index].player=="Green"){
						RG.Nikli=RG.Nikli-1;
						RG.Geeti[Board[Geeti[n-1].index].G-1].ResetPosition();
					}
				}			
				
				Board[Geeti[n-1].index].player=color;
				Board[Geeti[n-1].index].G=n;
				
				Vector3 p=GameObject.Find(Board[Geeti[n-1].index].waypoint).collider.gameObject.transform.position;
				Geeti[n-1].ChangePosition(p);
		
				return true;
	
			}
			else{
			
				GameObject.Find("T_Error").guiText.material.color=Color.red;
				GameObject.Find("T_Error").guiText.text="This Move is not Possible!!!";
				return false;
			
			}
			
		}
		//return true;	}	
			
	public bool Genetic_Move(Chromosome C,int dice,Square []Board,ref Player RP,ref Player RB,ref Player RY,ref Player RG){
		
		float[] Fitness_Score = new float[4];
		
		bool isPug,isNikali;
		int newIndex,i;
			
		for(int a=0;a<4;a++){
			
			Fitness_Score[a] = (float)-100000.0;
			
			if(Geeti[a].isPug)
				Fitness_Score[a] = (float)-100000.0;
			else{
				newIndex=Geeti[a].index;				
				
				isPug=false;
				isNikali=false;
				for(i=1;i<=dice;i++){
					
					if(newIndex==-1 &&dice!=6)
						break;
					else if(newIndex==-1 &&dice==6){
						newIndex=startingIndex;
						isNikali=true;
						i=dice+1;
						break;
					}
						
					else if(newIndex%18==17 && dice==i){
						i++;
						isPug=true;
						break;
					}
					else if(newIndex%18==17 && dice-i>1)
						break;
					else if(newIndex%18==12)
						newIndex=(newIndex+6)%72;
					else if(newIndex==homeIndex && canGoHome)			//HOME
						newIndex=(newIndex+7)%72;
					else
						newIndex=(newIndex+1)%72;
				}
				
				if(i==dice+1){
					
					Fitness_Score[a]=(float)0;
					Fitness_Score[a] += (float)0.0001;
					
					if (isNikali)
                        Fitness_Score[a] += Random.value;

					 /***  1)  Beat_Another_Pawn   ***/
                    if (Board[newIndex].player != "X" && Board[newIndex].player != color && !Board[newIndex].isSafe)
                        Fitness_Score[a] += C.Genes[0];

                    /***   2) Another_Pawn_Betable_In_Next_Round   ***/
//                    t = newIndex;
//                    for (j = 0; j < 6; j++)
//                    {
//                        if (t == homeIndex && canGoHome)
//                            break;
//                        else if (t % 19 == 6)
//                            t = t + 7;
//                        else
//                            t = (t + 1) % 76;
//
//                        if (Board[t].player != "X" && Board[t].player != color && !Board[t].isSafe)
//                        {
//                            Fitness_Score[a] += C.Genes[1];
//                            break;
//                        }
//                    }

                    /***   3) Pug_The_Pawn   ***/
                    if (isPug)
                        Fitness_Score[a] += C.Genes[2];


                    /***   4) Get_Pawn_Into_Home   ***/
                    if (newIndex >=13 && newIndex <=17)
                        Fitness_Score[a] += C.Genes[3];

                    /***   5)  Get_Pawn_Onto_Safe_Spot   ***/
                    if (newIndex % 18 == 3 || newIndex % 18 == 8 || newIndex % 18 == 13 || newIndex % 18 == 14 || newIndex % 18 == 15 || newIndex % 18 == 16 || newIndex % 18 == 17)
                        Fitness_Score[a] += C.Genes[4];

                    /***   6) Move_Own_Pawn_Into_Potentially_Beatable_Position   ***/
//                    t = newIndex;
//                    for (j = 0; j < 6; j++)
//                    {
//                        if (Board[t].isSafe)
//                            break;
//
//                        if (t % 19 == 13)
//                            t = t - 7;
//                        else if (t == 0)
//                            t = 75;
//                        else
//                            t = t - 1;
//
//                        if (Board[t].player != "X" && Board[t].player != color)
//                        {
//                            Fitness_Score[i] += C.Genes[5];
//                            break;
//                        }
//                    }

                    /***   7) Move_Pawn_On_Begin_Field_Of_Another_Player   ***/
                    if (Mathf.Abs(newIndex - homeIndex) > 15 && (newIndex % 18 == 8 || newIndex % 18 == 9 || newIndex % 18 == 10 || newIndex % 18 == 11 || newIndex % 18 == 12 || newIndex % 18 == 0 || newIndex % 18 == 1))
                        Fitness_Score[a] += C.Genes[6];
				}
			}
			
			
		}
		
		int largest = 0;

        for (i = 1; i < 4; i++)
            if (Fitness_Score[i] > Fitness_Score[largest])
                largest = i;

        MoveGeeti(dice,Board,largest+1,ref RP,ref RB,ref RY,ref RG);
		return true;
		
	}	
		
};

public class Gamer : MonoBehaviour {

	private Square []Board=new Square[72];
	
	public Player RP;
	public Player RB;
	public Player RY;
	public Player RG;
	
	int turn;
	bool isWaiting;
	int dice;
	
	bool isError;
	float errorTimeOut;
	
	bool is_Waiting_For_AI_Move_To_Complete;
	float AI_Move_TimeOut;
	
	Chromosome Best;
		
	public Texture2D buttonImage=null; 
	private void OnGUI(){
		
		if (GUI.Button(new Rect(610, 65,buttonImage.width, buttonImage.height), buttonImage) && isWaiting && !isError){			//130,50
            dice=Random.Range(1,7);
			GameObject.Find("T_Dice").guiText.material.color=Color.black;
			GameObject.Find("T_Dice").guiText.text=dice.ToString();
			
			GameObject.Find("T_Error").guiText.text="";
			
			if(dice<6 && ( (turn==1 && RP.Nikli==0) || (turn==2 && RB.Nikli==0) || (turn==3 && RY.Nikli==0)|| (turn==4 && RG.Nikli==0))){
				
				GameObject.Find("T_Error").guiText.material.color=Color.red;
				GameObject.Find("T_Error").guiText.text="No Move Possible!!!";
				
				isError=true;
				errorTimeOut=Time.time+1;							
			}
			else
				isWaiting=false;	
		}
		
	}
	
	void RollTheDice(){
		GameObject g=GameObject.Find("pCube1");
		
	    Vector3 pos=g.rigidbody.transform.position;
		pos.x+=2;
		pos.z+=2;
		g.rigidbody.AddTorque(25,0,25);
		g.rigidbody.AddForce (pos);
	}
	
	void DisplayInfo(){
		
		if(turn==1){
			GameObject.Find("T_Turn").guiText.material.color=Color.red;
			
			GameObject.Find("T_Turn").guiText.text="RED's Turn";
			GameObject.Find("T_Nikli").guiText.text="Nikli: "+RP.Nikli.ToString();
			GameObject.Find("T_Pug").guiText.text="Pug: "+RP.Pug.ToString();
			
			if(RP.canGoHome){
				GameObject.Find("T_canGoHome").guiText.material.color=Color.green;
				GameObject.Find("T_canGoHome").guiText.text="Can enter Home";
			}
			else{
				GameObject.Find("T_canGoHome").guiText.material.color=Color.red;
				GameObject.Find("T_canGoHome").guiText.text="Cannot enter Home";
				
			}
		}
		else if(turn==2){
			GameObject.Find("T_Turn").guiText.material.color=Color.blue;
			
			GameObject.Find("T_Turn").guiText.text="BLUE's Turn (AI)";
			GameObject.Find("T_Nikli").guiText.text="Nikli: "+RB.Nikli.ToString();
			GameObject.Find("T_Pug").guiText.text="Pug: "+RB.Pug.ToString();
			
			if(RB.canGoHome){
				GameObject.Find("T_canGoHome").guiText.material.color=Color.green;
				GameObject.Find("T_canGoHome").guiText.text="Can enter Home";
			}
			else{
				GameObject.Find("T_canGoHome").guiText.material.color=Color.red;
				GameObject.Find("T_canGoHome").guiText.text="Cannot enter Home";
				
			}
		}
		
		else if(turn==3){
			GameObject.Find("T_Turn").guiText.material.color=Color.yellow;
			
			GameObject.Find("T_Turn").guiText.text="YELLOW's Turn";
			GameObject.Find("T_Nikli").guiText.text="Nikli: "+RY.Nikli.ToString();
			GameObject.Find("T_Pug").guiText.text="Pug: "+RY.Pug.ToString();
			
			if(RY.canGoHome){
				GameObject.Find("T_canGoHome").guiText.material.color=Color.green;
				GameObject.Find("T_canGoHome").guiText.text="Can enter Home";
			}
			else{
				GameObject.Find("T_canGoHome").guiText.material.color=Color.red;
				GameObject.Find("T_canGoHome").guiText.text="Cannot enter Home";
				
			}
		}
		
		else if(turn==4){
			GameObject.Find("T_Turn").guiText.material.color=Color.green;
			
			GameObject.Find("T_Turn").guiText.text="GREEN's Turn (AI)";
			GameObject.Find("T_Nikli").guiText.text="Nikli: "+RG.Nikli.ToString();
			GameObject.Find("T_Pug").guiText.text="Pug: "+RG.Pug.ToString();
			
			if(RG.canGoHome){
				GameObject.Find("T_canGoHome").guiText.material.color=Color.green;
				GameObject.Find("T_canGoHome").guiText.text="Can enter Home";
			}
			else{
				GameObject.Find("T_canGoHome").guiText.material.color=Color.red;
				GameObject.Find("T_canGoHome").guiText.text="Cannot enter Home";
				
			}
		}
		
	}
	
	void Start () {
		
		GA ga=new GA();
		Best=new Chromosome();
		Best=ga.RUN_GENETIC_ALGORITHM();
		Debug.Log("Fitness: "+Best.Fitness+"  Chromosome: "+(float)Best.Genes[0] + "  "+(float)Best.Genes[1] + "  "+(float)Best.Genes[2] + "  "+(float)Best.Genes[3] + "  "+(float)Best.Genes[4] + "  "+(float)Best.Genes[5] + "  "+(float)Best.Genes[6] + "  ");
    		
		for(int i=1;i<=18;i++)
			Board[i-1].waypoint="wr"+ i.ToString();

		for(int i=1;i<=18;i++)
			Board[i+17].waypoint="wb"+ i.ToString();

		for(int i=1;i<=18;i++)
			Board[i+35].waypoint="wy"+ i.ToString();

		for(int i=1;i<=18;i++)
			Board[i+53].waypoint="wg"+ i.ToString();
		
		for(int i=0;i<72;i++){
			Board[i].player="X";
			Board[i].G=0;
			
			if(i%18==3 || i%18==8|| i%18==13|| i%18==14|| i%18==15|| i%18==16|| i%18==17)
				Board[i].isSafe=true;
			else 
				Board[i].isSafe=false;
		}
		
		RP=new Player("Red",8,6,"_Red1","_Red2","_Red3","_Red4","ir1","ir2","ir3","ir4");
		RB=new Player("Blue",26,24,"_Blue1","_Blue2","_Blue3","_Blue4","ib1","ib2","ib3","ib4");
		RY=new Player("Yellow",44,42,"_Yellow1","_Yellow2","_Yellow3","_Yellow4","iy1","iy2","iy3","iy4");
		RG=new Player("Green",62,60,"_Green1","_Green2","_Green3","_Green4","ig1","ig2","ig3","ig4");
		
		turn =1;
		isWaiting=true;
		isError=false;		
	}
	
	void Update () {
	
		bool response;
				
		if( isError &&  Time.time > errorTimeOut ){
			
			isError=false;
			isWaiting=true;
			turn=(turn%4)+1;
			GameObject.Find("T_Dice").guiText.text="";
			GameObject.Find("T_Error").guiText.text="";			
		}
		
		DisplayInfo();
		
		if(is_Waiting_For_AI_Move_To_Complete && Time.time > AI_Move_TimeOut){
			is_Waiting_For_AI_Move_To_Complete=false;
			
			isError=false;
			isWaiting=true;
			if(dice!=6)
				turn=(turn%4)+1;
			GameObject.Find("T_Dice").guiText.text="";
			GameObject.Find("T_Error").guiText.text="";
			
		}
		
		if( (turn==2 || turn==4) && !isError && !is_Waiting_For_AI_Move_To_Complete){				//AI Move
			
			AI_Move_TimeOut=Time.time+2;
			is_Waiting_For_AI_Move_To_Complete=true;
			
			isWaiting=false;
			
			dice=Random.Range(1,7);
			GameObject.Find("T_Dice").guiText.material.color=Color.black;
			GameObject.Find("T_Dice").guiText.text=dice.ToString();
			
			GameObject.Find("T_Error").guiText.text="";
			
			if( dice<6 && ( (turn==2 && RB.Nikli==0) || (turn==4 && RG.Nikli==0))){
				
				GameObject.Find("T_Error").guiText.material.color=Color.red;
				GameObject.Find("T_Error").guiText.text="No Move Possible!!!";
				
				goto Finish_AI_Turn;		
			}
			
			if(turn==2)
				response=RB.Genetic_Move(Best,dice,Board,ref RP,ref RB,ref RY,ref RG);
			else if(turn==4)
				response=RG.Genetic_Move(Best,dice,Board,ref RP,ref RB,ref RY,ref RG);
			
		Finish_AI_Turn:
				;		
		}
		
		if(Input.GetMouseButtonDown (0) && !isWaiting  && !isError){
			
			RaycastHit hit = new RaycastHit ();
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			 
			if (Physics.Raycast (ray, out hit)) { 
				
				response=false;
				
				if(hit.collider.gameObject.name.StartsWith("_")){
					
					if(hit.collider.gameObject.name.StartsWith("_R") && turn==1){
				
						isWaiting=true;
						
						if(hit.collider.gameObject.name=="_Red1")
							response=RP.MoveGeeti(dice,Board,1,ref RP,ref RB,ref RY,ref RG);
						
						else if(hit.collider.gameObject.name=="_Red2")
							response=RP.MoveGeeti(dice,Board,2,ref RP,ref RB,ref RY,ref RG);
						
						else if(hit.collider.gameObject.name=="_Red3")
							response=RP.MoveGeeti(dice,Board,3,ref RP,ref RB,ref RY,ref RG);
						
						else if(hit.collider.gameObject.name=="_Red4")
							response=RP.MoveGeeti(dice,Board,4,ref RP,ref RB,ref RY,ref RG);
						
						if(dice==6)
						isWaiting=true;
						
					    if(!response)					//Move not possible. isWaiting is set o that dice is not rolled until a valid MOVE is made
						    isWaiting=false;
						
					    if(dice!=6 && response){
						    turn=(turn%4)+1;
							GameObject.Find("T_Dice").guiText.text="";
							isWaiting=true;
						}
					
					}
					
					else if(hit.collider.gameObject.name.StartsWith("_Y") && turn==3){
					
						isWaiting=true;
											
						if(hit.collider.gameObject.name=="_Yellow1")
							response=RY.MoveGeeti(dice,Board,1,ref RP,ref RB,ref RY,ref RG);
						
						else if(hit.collider.gameObject.name=="_Yellow2")
							response=RY.MoveGeeti(dice,Board,2,ref RP,ref RB,ref RY,ref RG);
						
						else if(hit.collider.gameObject.name=="_Yellow3")
							response=RY.MoveGeeti(dice,Board,3,ref RP,ref RB,ref RY,ref RG);
						
						else if(hit.collider.gameObject.name=="_Yellow4")
							response=RY.MoveGeeti(dice,Board,4,ref RP,ref RB,ref RY,ref RG);
						
						if(dice==6)
						isWaiting=true;
						
						if(!response)
							isWaiting=false;
							
						if(dice!=6 && response){
							turn=(turn%4)+1;
							GameObject.Find("T_Dice").guiText.text="";
							isWaiting=true;
						}						
					}				
					
				}
			}
		}		
	}
}


