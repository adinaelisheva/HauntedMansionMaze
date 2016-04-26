using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Setup : MonoBehaviour {

    public class Cell {
        public GameObject rightWall;
        public GameObject topWall;

        public GameObject footprints;

        public bool visited = false;

        public int x;
        public int y;

        private Quaternion horizontal = Quaternion.identity;
        private Quaternion vertical = Quaternion.AngleAxis(90, Vector3.up);

        //takes in an array of walls in order of decreasing popularity
        Transform fetchWall(Transform[] walls) {
            int r = Random.Range(0, 10);
            if(r < 5) {
                return walls[0];
            }
            if (r < 8) {
                return walls[1];
            }
            return walls[2];
        }

        public Cell(int x, int y, Transform[] walls, Transform Footprints, int size) {
            this.x = x;
            this.y = y;

            //create my top and right walls
            int bx = -(size-3) + x * 5; //shift the walls by 2 to line up with the grid
            int bz = -(size-5) + y * 5;
            var bw = Instantiate(fetchWall(walls), new Vector3(bx, 1, bz), horizontal) as Transform;
            topWall = bw.gameObject;

            int rx = -(size-5) + x * 5; //shift the walls by 2 to line up with the grid
            int rz = -(size-3) + y * 5;
            var rw = Instantiate(fetchWall(walls), new Vector3(rx, 1, rz-0.5F), vertical) as Transform;
            rw.localScale += new Vector3(1F, 0, 0);
            rightWall = rw.gameObject;

            int fx = -(size-2) + x * 5;
            int fz = -(size-1) + y * 5;
            var f = Instantiate(Footprints, new Vector3(fx, 0, fz), Quaternion.identity) as Transform;
            footprints = f.gameObject;
            footprints.SetActive(false);

        }
    }

    public class Point<T>{
        public T x;
        public T y;

        public Point(T x, T y) {
            this.x = x;
            this.y = y;
        }
    }

    public int ghostCount;
    public int zombieCount;
    public int flowerCount;
    public int mirrorCount;

    public Text score;
    public Text winText;

    //note: this is dynamically set up here, but if this changes, the outer walls/player
    //location/etc have to manually updated to match
    private int size = 16; //this is the number of cells on a side of the grid
    private int halfGridSize; //in units. the maze will go from -this to +this

    public Transform plainWall;
    public Transform windowWall;
    public Transform candleWall;
    public Transform Ghost;
    public Transform Zombie;
    public Transform Mirror;
    public Transform Flower;
    public Transform Footprints;

    public GameObject exitDoor;

    private Queue<Point<int>> deadEnds = new Queue<Point<int>>();

    private bool won = false;
    private float winTime;

    //this goes: ghost, zombie, mirror, flower 
    private int[] numItems;

    Cell[,] maze;

    //helper function to deal with all the possibilities
    Transform fetchObject() {
        int r = Random.Range(0, 4);

        while(numItems[r] == 0) {
            //none left of this thing
            r = (r+1)%4;
        }

        numItems[r]--;
        if(r == 0){
            ghostCount++;
            return Ghost;
        } else if(r == 1){
            zombieCount++;
            return Zombie;
        } else if(r == 2){
            return Mirror;
        } else {
            return Flower;
        }
        
    }

    bool isMonster(Transform obj){
        return obj.gameObject.CompareTag("Ghost") || obj.gameObject.CompareTag("Zombie");
    }

    void addDeadEnd(Cell curr, int halfGridSize){
        //we're at a dead end! add a monster or item
        //but don't do it if we're at the start, which is at size-1,0
        if(curr.x != size-1 || curr.y != 0) {
            int ox = -(halfGridSize-3) + curr.x * 5;
            int oz = -(halfGridSize-3) + curr.y * 5;

            Point<int> p = new Point<int>(ox, oz);

            deadEnds.Enqueue(p);
        }
    }

    void Start() {
        //winText should start off
        winText.gameObject.SetActive(false);

        ghostCount = zombieCount = flowerCount = mirrorCount = 0;

        halfGridSize = (size*5)/2; //each cell is 5 units across

        //set up the initial grid which will become a maze

        int unvisited = 0;
        maze = new Cell[size,size];

        Transform[] walls = new Transform[] { plainWall, candleWall, windowWall };

        for(int i = 0; i < size; i++){
            for(int j = 0; j < size; j++) {
                maze[i,j] = new Cell(i, j, walls, Footprints, halfGridSize);
                unvisited++;

                if(i == 0 && j == size-1){
                    maze[i, j].topWall.SetActive(false);
                }
            }
        }

        //now use DFS with backtracking to create a maze

        bool backtracking = false;
        Stack<Cell> visited = new Stack<Cell>();
        Cell curr;

        curr = maze[0,0];
        while(unvisited > 0) {

            List<Cell> neighbors = new List<Cell>();

            //find all unvisited neighbors
            if(curr.x > 0 && !maze[curr.x - 1, curr.y].visited) {
                neighbors.Add(maze[curr.x - 1, curr.y]);
            }
            if(curr.x < size-1 && !maze[curr.x + 1, curr.y].visited) {
                neighbors.Add(maze[curr.x + 1, curr.y]);
            }
            if(curr.y > 0 && !maze[curr.x, curr.y - 1].visited) {
                neighbors.Add(maze[curr.x, curr.y - 1]);
            }
            if(curr.y < size-1 && !maze[curr.x, curr.y + 1].visited) {
                neighbors.Add(maze[curr.x, curr.y + 1]);
            }

            if(neighbors.Count > 0) {
                backtracking = false;
                var neighbor = neighbors[Random.Range(0, neighbors.Count)];

                //remove the wall between curr and neighbor
                if(curr.x - 1 == neighbor.x) {
                    neighbor.rightWall.SetActive(false);
                }
                if(curr.x + 1 == neighbor.x) {
                    curr.rightWall.SetActive(false);
                }
                if(curr.y - 1 == neighbor.y) {
                    neighbor.topWall.SetActive(false);
                }
                if(curr.y + 1 == neighbor.y) {
                    curr.topWall.SetActive(false);
                }

                visited.Push(curr);

                unvisited--;
                neighbor.visited = true;
                curr = neighbor;

            } else {
                if(!backtracking) {
                    backtracking = true;
                    addDeadEnd(curr, halfGridSize);
                }
                curr = visited.Pop();
            }
        }
        //we reached the end. add curr as a dead end
        addDeadEnd(curr, halfGridSize);

        //now add objects to every dead end
        int numEnds = deadEnds.Count;
        int numMonsters = numEnds / 2;
        int numGhosts = Random.Range(1, numMonsters);
        int numZombies = numMonsters - numGhosts;
        int numMirrors = numGhosts;
        int numFlowers = numZombies + (numEnds % 2); //add one if there's an extra spot (odd # of dead ends)

        //this goes: ghost, zombie, mirror, flower 
        numItems = new int[4] {numGhosts, numZombies, numMirrors, numFlowers};
        Point<int> end;

        while(deadEnds.Count > 0){
            end = deadEnds.Dequeue();

            Transform obj = fetchObject();
            Quaternion q = Quaternion.identity;
            if(isMonster(obj)){
                int rot = Random.Range(0,4) * 90;
                q = Quaternion.AngleAxis(rot, Vector3.up);
            } else {
                q = Quaternion.AngleAxis(60, Vector3.right);
            }
             
            Instantiate(obj, new Vector3(end.x, 1, end.y-0.5f), q);
        }

    }

    void Update() {
        if(!won) {
            //do normal game things
            int totalEnemies = ghostCount + zombieCount;
            score.text = "Flowers: " + flowerCount + "  Mirrors: " + mirrorCount + "  Monsters remaining: " + totalEnemies;

            if(totalEnemies == 0) {
                won = true;
                winTime = Time.time;
                exitDoor.SetActive(false);
                score.gameObject.SetActive(false);
                winText.gameObject.SetActive(true);
            }
        } else {
            //we won! keep track of time and hide the win text after 10s
            if(Time.time - winTime >= 10) {
                winText.gameObject.SetActive(false);
            }

        }
    }

    public void showFootprints(int x,int y){
        int cx = (x + halfGridSize) / 5;
        int cy = (y + halfGridSize) / 5;
        if(cx < 0 || cy < 0 || cx >= size || cy >= size) {
            return; //out of bounds
        }

        //make sure we're in the center bit
        if( (x+halfGridSize) % 5 < 1 || (x+halfGridSize) % 5 > 4 || (y+halfGridSize) % 5 < 1 || (y+halfGridSize) % 5 > 4) {
            return;
        }

        maze[cx, cy].footprints.SetActive(true);
    }
        	
}