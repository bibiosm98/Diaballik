using Diabiallik;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

namespace Diabiallik
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Diaballik");
            Diaballik diaballik = new Diaballik();
            //diaballik.start();
            diaballik.gameWithPrediction();
        }
    }
}

class Diaballik
{
    bool 
        gameEnd = false;
    public Pawn[]
        gameBoard = new Pawn[49];
    public Gracz 
        Player_1, 
        Player_2;
    public List<dynamic> 
        PredictMoves = new List<dynamic>(60);
    public int
        treeDeep = 0;
    public myTree
        movesTree = new myTree();
    public Diaballik()
    {
        Console.WriteLine("Nowa Gra");
        newBoard();
        Player_1 = new Gracz("Gracz_1", true, gameBoard, 'x', movesTree);
        Player_2 = new Gracz("Gracz_2", false, gameBoard, 'y', movesTree);
        showBoard();
    }
    public void start()
    {
        Console.WriteLine("START_GAME");
        DateTime timeStart = DateTime.Now;
        int i = 0;
        while (!gameEnd)
        {
            i++;
            gameEnd = Player_1.nextMove();
            //showBoardbyNumbers();
            //Console.WriteLine("Diaballik start() player 1");
            //reverseBoard();
            //if(i>10) break;
            //Console.Clear();
            showBoard();
            //System.Threading.Thread.Sleep(200); //706550
            //if (i>22)break;
            if (gameEnd)
            {
                //Console.Clear();
                showBoard();
                Console.WriteLine("X WON");
                break;
            }
            //showBoardbyNumbers();
            gameEnd = Player_2.nextMove();
            if (gameEnd)
            {
                //Console.Clear();
                showBoard();
                Console.WriteLine("Y WON");
                break;
            }
            //showBoardbyNumbers();
            //showBoard();
            //reverseBoard();
        }
        //showBoard();
        DateTime timeEnd = DateTime.Now;
        //Console.WriteLine("Ilość dostepnych ruchów = " + gracz1.iloscRuchow() + ", " + gracz2.iloscRuchow());
        Console.WriteLine("KONIEC: " + timeEnd.Subtract(timeStart));
    }
    public void gameWithPrediction()
    {
        Console.WriteLine("START_GAME WITH PREDICTION");
        DateTime timeStart = DateTime.Now;
        Gracz.firstPredict(Player_1, Player_2);
        int i = 0;
        while (!gameEnd)
        {
            i++;
            if(i>10) break;
            movesTree = Player_1.predict();
            gameEnd = Player_1.nextPredictedMove();
            showBoard();
            if (gameEnd)
            {
                //Console.Clear();
                showBoard();
                Console.WriteLine("X WON");
                break;
            }
            movesTree = Player_1.predict();
            gameEnd = Player_2.nextPredictedMove();
            showBoard();
            if (gameEnd)
            {
                //Console.Clear();
                showBoard();
                Console.WriteLine("Y WON");
                break;
            }
        }
        DateTime timeEnd = DateTime.Now;
        Console.WriteLine("KONIEC: " + timeEnd.Subtract(timeStart));
    }
    public void predict()
    {

    }
    private void newBoard()
    {
        Console.WriteLine("Inicjuj plansze");
        for (int i = 0; i < gameBoard.Length; i++)
        {
            gameBoard[i] = new Pawn(i, '-');
        }
    }
    private void showBoard()
    {
        Console.WriteLine("Pokaż plansze");
        for (int i = gameBoard.Length - 1; i >= 0; i--)
        {
            Console.Write(gameBoard[i].pawnSymbol + "  ");
            if (i % 7 == 0)
            {
                Console.WriteLine("");
            }
        }
        Console.WriteLine("");
    }
    private void showBoardbyNumbers()
    {
        Console.WriteLine("Pokaż plansze");
        for (int i = gameBoard.Length - 1; i >= 0; i--)
        {
            if (gameBoard[i].field < 10)
            {
                Console.Write(" " + gameBoard[i].field + "  ");
            }
            else
            {
                Console.Write(gameBoard[i].field + "  ");
            }
            if (i % 7 == 0)
            {
                Console.WriteLine("");
            }
        }
        Console.WriteLine("");
    }
    private void reverseBoard()
    {
        Array.Reverse(gameBoard);
    }
}

class Gracz
{
    public char 
        playerSymbol, 
        playerSymbolUpper;
    string playerName = "";
    public Pawn
        newBallPawn,
        oldBallPawn;
    public Pawn[]
        mainBoard,
        playerPawns = new Pawn[7],
        playerBoard = new Pawn[49];
    public List<dynamic> 
        playerMoves = new List<dynamic>(), 
        shortMove = new List<dynamic>(), // move one field and pass ball
        longMove = new List<dynamic>(), // move 2 fields(same Pawn or 2 different Pawn) and pass ball
        possiblePassingField = new List<dynamic>(), // Fields where Ball can be throw
        possiblePassingPawns = new List<dynamic>(); // Pawns where Ball can be throw
    public bool
        displayData = false,
        gameEnd = false,
        playerOne;
    public Move 
        lastMove;
    public myTree
        tree;

    public int 
        oldBallField;
    public static myTree firstPredict(Gracz P_1, Gracz P_2)
    {

        return new myTree();
    }
    public Gracz(string name, bool playerOne, Pawn[] gameBoard, char symbol)
    {
        this.playerSymbol = symbol;
        this.playerSymbolUpper = Char.ToUpper(playerSymbol);
        this.playerName = name;
        this.mainBoard = gameBoard;
        this.playerOne = playerOne;

        int start = 0;
        if (!playerOne) start = 42;
        for (int i = start; i < start + 7; i++)
        {
            mainBoard[i] = new Pawn(mainBoard[i].field, playerSymbol);
            playerPawns[i - start] = new Pawn(mainBoard[i].field - start, playerSymbol);
            if (i % 7 == 3)
            {
                newBallPawn = new Pawn(mainBoard[i].field - start, playerSymbolUpper);
                mainBoard[i] = new Pawn(mainBoard[i].field, playerSymbolUpper);
                playerPawns[i - start] = new Pawn(mainBoard[i].field - start, playerSymbolUpper);
            }
        }
    }
    public Gracz(string name, bool playerOne, Pawn[] gameBoard, char symbol, myTree tree)
    {
        this.playerSymbol = symbol;
        this.playerSymbolUpper = Char.ToUpper(playerSymbol);
        this.playerName = name;
        this.mainBoard = gameBoard;
        this.playerOne = playerOne;
        this.tree = tree;

        int start = 0;
        if (!playerOne) start = 42;
        for (int i = start; i < start + 7; i++)
        {
            mainBoard[i] = new Pawn(mainBoard[i].field, playerSymbol);
            playerPawns[i - start] = new Pawn(mainBoard[i].field - start, playerSymbol);
            if (i % 7 == 3)
            {
                newBallPawn = new Pawn(mainBoard[i].field - start, playerSymbolUpper);
                mainBoard[i] = new Pawn(mainBoard[i].field, playerSymbolUpper);
                playerPawns[i - start] = new Pawn(mainBoard[i].field - start, playerSymbolUpper);
            }
        }
    }
    public myTree predict()
    {
        copyCurrentBoard();

        return new myTree();
    }
    public bool nextPredictedMove()
    {
        copyCurrentBoard();

        return gameEnd;
    }
    public bool nextMove()
    {
        copyCurrentBoard();

        availableMoves();
        rateMoves();
        makeMove();
        passBall();

        setBoardAfterMove();
        return gameEnd;
    }
    private void availableMoves()
    {
        playerMoves.Clear();
        for (int i = 0; i < playerPawns.Length; i++)
        {
            checkAvailableMoves(playerBoard, playerPawns, shortMove, true, playerPawns[i].field, 0, 0);
        }
        if (displayData) Console.WriteLine("Drugi ruch piona = " + longMove.Count);
        playerMoves.AddRange(longMove);
        shortMove.Clear();
        longMove.Clear();
    }
    private void checkAvailableMoves(Pawn[] board, Pawn[] playerPawns, List<dynamic> lista, bool stepOne, int from_2, int from_1, int to_1)
    {
        int[] moveDirection = { 7, -1, 1, -7 };// kierunek w którym można ruszyć pionem
        int to_2;  // pole - miejsce w które pionek może się przesunąć, from_2 - miejsce na którym jest
        bool insideBoard;
        for (int direction = 0; direction < moveDirection.Length; direction++)
        {
            insideBoard = true;
            to_2 = from_2 + moveDirection[direction];
            if (moveDirection[direction] == 1 && from_2 % 7 == 6) insideBoard = false;  //tutaj 2 warunki wychodzenia poza plansze(aby nie przeskoczyć przy polu +1/-1 o całą szerkość planszy będącej jeden rząd wyżej/niżej  
            if (moveDirection[direction] == -1 && from_2 % 7 == 0) insideBoard = false;

            //board[from_2].pawnSymbol != playerSymbolUpper  blokuje ruch pionka z piłką
            if (to_2 >= 0 && to_2 < 49 && insideBoard && board[to_2].pawnSymbol == '-' && board[playerBoard[from_2].field].pawnSymbol != playerSymbolUpper)
            {
                if (stepOne)
                {
                    Pawn[] newBoardForSecoundMove = deepCopyBoard(board);
                    Pawn[] newUserPawnsForSecoundMove = deepCopyPlayerPawns(playerPawns);

                    swapBoardPawns(newBoardForSecoundMove, playerBoard[from_2].field, playerBoard[to_2].field);
                    swapPlayerPawns(newUserPawnsForSecoundMove, newBoardForSecoundMove, playerBoard[from_2].field, playerBoard[to_2].field);
                    for (int i = 0; i < playerPawns.Length; i++)
                    {
                        checkAvailableMoves(newBoardForSecoundMove, newUserPawnsForSecoundMove, longMove, false, newUserPawnsForSecoundMove[i].field, from_2, to_2);
                    }
                }
                else
                {
                    lista.Add(new Move(from_1, to_1, from_2, to_2));
                }
            }
        }
    }
    private void rateMoves() // TO NAPEWNO DO POPRAWY I TO SROGIEJ, BRAĆ POD UWAGĘ CZY ! RUCH DAJE KORZYŚCI
    {
        findPossiblePassingField();
        foreach (Move move in playerMoves)
        {
            if (move.from_1 < move.to_1 - 5 || move.from_2 < move.to_2 - 5) { move.score += 1000; } //  Jump 1 field forward / forward across
            if (move.from_1 < move.to_2 - 10) { move.score += 5000; } // Jump 2 fields forward / forward across
            foreach (Pawn passingMove in possiblePassingField) // Move to field where might be throw a ball
            {
                if (move.to_1 == passingMove.field  && move.to_1 != move.from_2 || move.to_2 == passingMove.field) // Make one 2 fields jump to passingField or 2 short jumps
                {
                    move.score += 20_000;
                    if (move.to_1 > 41 || move.to_2 > 41) move.score += 1_000_000; // Game END ^^
                }
            }
            //if (move.to_2 < 41)
            //{
            //    if (playerBoard[move.to_2 + 7].pawnSymbol != '-' && playerBoard[move.to_2 + 7].pawnSymbol != playerSymbol) // blokowanie pionka przeciwnika
            //    {
            //        move.score += 1000; //daleko od startu
            //        if (move.from_1 < 20) move.score += 3000; // w połowie planszy
            //        if (move.from_1 < 0) move.score += 30_000; // blisko swojego startu, zapobiega wygranejprzeciwnika
            //    }
            //}
        }
        if (displayData) Console.WriteLine();
        playerMoves.Sort((x,y)=>y.score.CompareTo(x.score));
    }
    private void findPossiblePassingField()
    {
        possiblePassingField.Clear(); 
        possiblePassingPawns.Clear();
        if(displayData)Console.WriteLine("Ball field and symbol: " + newBallPawn.field + ". " + newBallPawn.pawnSymbol);

        //tu będzie łopatologicznie bo czemu by nie
        int[] start = new int[] {
            newBallPawn.field + 1,
            newBallPawn.field + 7,
            newBallPawn.field + 8,
            newBallPawn.field + 6,
            newBallPawn.field - 1,
            newBallPawn.field - 7,
            newBallPawn.field - 6,
            newBallPawn.field - 8
        };
        //int[] finish = new int[] { 49, 49, 49, 49, 0, 0, 0, 0};
        int[] jump = new int[] { 1, 7, 8, 6, -1, -7, -6, -8};

        bool change = (playerSymbolUpper == 'X') ? true : false; //(change) ? y : 48-y
        for (int i = 0; i < 4; i++)
        {
            for (int y = start[i]; y < 49; y += jump[i])
            {
                if (playerBoard[0].field == 48) y = 48 - y;
                //Console.Write("  Y = " + y);
                if (y % 7 == 6 || y % 7 == 0)
                {
                    if (start[i] == 6) break;
                    if (start[i] == 1) break;
                    if (playerBoard[y].pawnSymbol != '-' && playerBoard[y].pawnSymbol != playerSymbol) break;
                    possiblePassingField.Add(new Pawn(playerBoard[y].field, playerBoard[y].pawnSymbol));
                    break;
                }
                if (playerBoard[y].pawnSymbol != '-' && playerBoard[y].pawnSymbol != playerSymbol)
                {
                    break;
                }
                possiblePassingField.Add(new Pawn(playerBoard[y].field, playerBoard[y].pawnSymbol));
            }
        }
        for (int i = 4; i < 8; i++)
        {
            for (int y = start[i]; y >= 0; y += jump[i])
            {
                if (playerBoard[0].field == 48) y = 48 - y;
                if (y < 0)
                {
                    break;
                }
                if (y % 7 == 6 || y % 7 == 0)
                {
                    if (start[i] == -6) break;
                    if (start[i] == -1) break;
                    possiblePassingField.Add(new Pawn(playerBoard[y].field, playerBoard[y].pawnSymbol));
                    break;
                }
                if (playerBoard[y].pawnSymbol != '-' && playerBoard[y].pawnSymbol != playerSymbol)
                {
                    break;
                }
                possiblePassingField.Add(new Pawn(playerBoard[y].field, playerBoard[y].pawnSymbol));
            }
        }

        possiblePassingPawns.Clear();
        possiblePassingField.Sort((x, y) => y.field.CompareTo(x.field));
        if (displayData) Console.WriteLine("Pssible passing field: ");
        foreach (Pawn p in possiblePassingField)
        {
            if (displayData) Console.Write(p.field + ", ");
            if (p.pawnSymbol == playerSymbol)
            {
                possiblePassingPawns.Add(new Pawn(p));
            }
        }
        if (displayData) Console.WriteLine();
        if (displayData) Console.WriteLine("possible pass field " + possiblePassingField.Count);
        //Console.WriteLine("possible pass pawns " + possiblePassingPawns.Count);
    }
    private void makeMove()
    {
        int a = new Random().Next((playerMoves.Count>10) ? 5 : playerMoves.Count);
        int mov = new Random().Next(playerMoves.Count-1);
        Move wykonaj = lastMove = playerMoves[a];
        if (displayData) Console.WriteLine("Wykonaj ruch: " + wykonaj);
        int x = 0;
        if (playerBoard[0].field > 30) x = 48;
        if (wykonaj.to_2 == -1)
        {//dla jednego krótkiego ruchu, nie wiadomo, czy będą wykonywane
            // Unused code for one short move
            swapBoardPawns(mainBoard, wykonaj.from_1, wykonaj.to_1);
            swapPlayerPawns(playerPawns, wykonaj.from_1, wykonaj.to_1);
        }
        else
        {
            swapBoardPawns(playerBoard, wykonaj.from_1, wykonaj.to_1);
            swapBoardPawns(playerBoard, wykonaj.from_2, wykonaj.to_2);
            swapPlayerPawns(playerPawns, playerBoard[wykonaj.from_1].field, playerBoard[wykonaj.to_1].field);
            swapPlayerPawns(playerPawns, playerBoard[wykonaj.from_2].field, playerBoard[wykonaj.to_2].field);
        }
        possiblePassingPawns.Clear();
        findPossiblePassingField();
    }
    private void passBall()
    {
        ratePassingBall();
        if (displayData) Console.WriteLine("PASSBALL");
        if (displayData) Console.WriteLine("possiblePassingPawns:" + possiblePassingPawns.Count);
        if (displayData) Console.WriteLine();
        int random = new Random().Next(possiblePassingPawns.Count-1);
        random = 0;
        int newBallField = 0;
        if (mainBoard[0].field == 0)
        {
            newBallField = possiblePassingPawns[random].field;
            oldBallField = newBallPawn.field;
        }
        else // Pass ball in main board, for player_2
        { 
            newBallField = 48 - possiblePassingPawns[random].field;
            oldBallField = 48 - newBallPawn.field;
        }
        if (displayData) Console.WriteLine("random = " + random);
        swapBoardPawns(playerBoard, oldBallField, newBallField);
        newBallPawn = (Pawn)playerBoard[newBallField];
        if (newBallField > 41) gameEnd = true;
        if (displayData) Console.WriteLine("ball field = " + newBallPawn.field);
        if (displayData) Console.WriteLine("swap field = " + possiblePassingPawns[random].field);
        //Pawn chosenPawn = null;
        //int localScore = 0;
        //bool go = false;
        //foreach(Pawn pawn in possiblePassingPawns)
        //{
        //    foreach (Move move in playerMoves)
        //    {
        //        if (pawn.pawnSymbol == playerSymbol && move.score > localScore && move.to_2 == pawn.field)
        //        {
        //            Console.WriteLine();
        //            Console.WriteLine("MOVEEE  score" + move.score + ", " + move.to_2 + ", " + pawn.field);
        //            localScore = move.score;
        //            chosenPawn = pawn;
        //        }
        //    }
        //}
        //foreach (Pawn p in playerBoard)
        //{
        //    if (p.field == chosenPawn.field)
        //    {
        //        go = true;
        //        chosenPawn = p;
        //        Console.WriteLine("CHANGING PAWNS " + p.field);
        //        //Console.WriteLine("chosen =  " + chosenPawn.field);
        //    }
        //}
        //Console.WriteLine("Moves " + possiblePassingPawns.Count);
        //Console.WriteLine("score " + localScore);
        ////if (chosenPawn != null) {
        //    if (go)
        //    {
        //        Console.WriteLine("PASSING BALL: from - ");
        //    //chosenPawn.pawnSymbol = playerSymbolUpper;
        //    //playerBoard[ballPawn.field].pawnSymbol = playerSymbol;
        //    //for (int i = 0; i < playerMoves.Count; i++)
        //    //{
        //    //    if (playerMoves[i].to_2 == playerBoard[ballPawn.field].field)
        //    //    {
        //    //        playerMoves[i] = new Pawn(chosenPawn.field, chosenPawn.pawnSymbol);
        //    //    }
        //    //}
        //    //ballPawn.field = chosenPawn.field;

        //        swapBoardPawns(playerBoard, ballPawn.field, chosenPawn.field);
        //    }
        ////chosenPawn.pawnSymbol = playerSymbolUpper;
        ////ballPawn.pawnSymbol = playerSymbol;
        ////playerBoard.ElementAt(chosenPawn.field).pawnSymbol = playerSymbolUpper;
        ////playerBoard.ElementAt(ballPawn.field).pawnSymbol = playerSymbol;
        ////playerBoard.ElementAt(ballPawn.field).field = chosenPawn.field;

        playerMoves.Clear();
    }
    private void ratePassingBall()
    {
        //foreach(Pawn p in possiblePassingPawns)
        //{
        //    Console.Write(p.field + ", ");
        //}
        possiblePassingPawns.Sort((x, y) => y.field.CompareTo(x.field));

        //Console.WriteLine("SORT:");
        //foreach (Pawn p in possiblePassingPawns)
        //{
        //    Console.Write(p.field + ", ");
        //}
    }
    private void swapBoardPawns(Pawn[] board, int from, int to)
    {
        char q = board[to].pawnSymbol;
        board[to].pawnSymbol = board[from].pawnSymbol;
        board[from].pawnSymbol = q;
        //Console.WriteLine("Plansza z: " + board[from].field + ", znak = " + board[from].pawnSymbol);
        //Console.WriteLine("Plansza do: " + board[to].field + ", znak = " + board[to].pawnSymbol);
    }
    private void swapPlayerPawns(Pawn[] pawns, int from, int to)
    {
        //Console.WriteLine("from:" + from + " to:" + to + " pawns[0]:" + pawns[0].field +"");
        for (int i = 0; i < pawns.Length; i++)
        {
            if (pawns[i].field == playerBoard[from].field)
            //if (pawns[i].field == from)
            {
                // Console.WriteLine("CHANGE PAWN " + " field: " + playerBoard[to].field + " is now " + playerBoard[to].pawnSymbol  + " " + pawns[i].field  + " is now " + pawns[i].pawnSymbol + "");
                pawns[i] = playerBoard[to];
                pawns[i].pawnSymbol = playerBoard[to].pawnSymbol;
            }
        }
    }
    private void swapPlayerPawns(Pawn[] pawns, Pawn[] board, int from, int to)
    {
        for (int i = 0; i < pawns.Length; i++)
        {
            if (pawns[i].field == playerBoard[from].field)
            //if (pawns[i].field == from)
            {
                //Console.WriteLine("CHANGE PAWN " + " field: " + playerBoard[to].field + " is now " + pawns[i].field + "" + playerBoard[to].pawnSymbol + " is now " + pawns[i].pawnSymbol + "");
                pawns[i] = board[to];
                //pawns[i].pawnSymbol = playerBoard[to].pawnSymbol;
            }
        }
    }
    private Pawn[] deepCopyBoard(Pawn[] board)
    {
        Pawn[] copiedBoard = new Pawn[board.Length];
        for (int i = 0; i < board.Length; i++)
        {
            copiedBoard[i] = new Pawn(board[i].field, board[i].pawnSymbol); // głębokie kopiowanie planszy
        }
        return copiedBoard;
    }
    private Pawn[] deepCopyPlayerPawns(Pawn[] pawns)
    {
        Pawn[] copiedPawns = new Pawn[7];
        for (int i = 0; i < pawns.Length; i++)
        {
            copiedPawns[i] = new Pawn(pawns[i].field, pawns[i].pawnSymbol); // głębokie kopiowanie pionków gracza
        }
        return copiedPawns;
    }
        public void showPawns()
    {
        if (displayData) Console.WriteLine();
        if (displayData) Console.Write("Pionki, gracza : ");
        for (int i=0; i<playerPawns.Length; i++)
        {
            if (displayData) Console.Write(playerPawns[i].field + ":" + playerPawns[i].pawnSymbol + ", ");
        }
        if (displayData) Console.WriteLine();
    }
    private void showPawns(Pawn[] p)
    {
        Console.WriteLine();
        Console.Write("Pionki, gracza : ");
        for (int i = 0; i < 7; i++)
        {
            Console.Write(p[i].field + ":" + p[i].pawnSymbol + ", ");
        }
        Console.WriteLine();
    }
        public void showBoard()
    {
        Console.WriteLine("\nPokaż plansze: " + playerName);
        for (int i = playerBoard.Length - 1; i >= 0; i--)
        {
            Console.Write(playerBoard[i].pawnSymbol + "  ");
            if (i % 7 == 0)
            {
                Console.WriteLine("");
            }
        }
    }
    private void showBoard(Pawn[] board)
    {
        //showBoardbyNumbers();
        Console.WriteLine("\nPokaż plansze: " + playerName);
        for (int i = board.Length - 1; i >= 0; i--)
        {
            Console.Write(board[i].pawnSymbol + "  ");
            if (i % 7 == 0)
            {
                Console.WriteLine("");
            }
        }
    }
    private void showBoardbyNumbers()
    {
        Console.WriteLine("Pokaż plansze");
        for (int i = mainBoard.Length - 1; i >= 0; i--)
        {
            if (mainBoard[i].field < 10)
            {
                Console.Write(" " + mainBoard[i].field + "  ");
            }
            else
            {
                Console.Write(mainBoard[i].field + "  ");
            }
            if (i % 7 == 0)
            {
                Console.WriteLine("");
            }
        }
        Console.WriteLine("");
    }
        public void showBoardbyNumbers(Pawn[] board)
    {
        Console.WriteLine("Pokaż plansze");
        for (int i = board.Length - 1; i >= 0; i--)
        {
            if (board[i].field < 10)
            {
                Console.Write(" " + board[i].field + "  ");
            }
            else
            {
                Console.Write(board[i].field + "  ");
            }
            if (i % 7 == 0)
            {
                Console.WriteLine("");
            }
        }
        Console.WriteLine("");
    }
    private void copyCurrentBoard()
    {
        if (playerOne)
        {
            playerBoard = deepCopyBoard(mainBoard);
        }
        else
        {
            for(int i = 0; i < mainBoard.Length; i++)
            {
                playerBoard[i] = new Pawn(mainBoard[48-i].field, mainBoard[i].pawnSymbol);
            }
            Array.Reverse(playerBoard);
        }
    }
    private void setBoardAfterMove()
    {
        if (playerOne)
        {
            swapBoardPawns(mainBoard, lastMove.from_1, lastMove.to_1);
            swapBoardPawns(mainBoard, lastMove.from_2, lastMove.to_2);
            swapBoardPawns(mainBoard, oldBallField, newBallPawn.field);
        }
        else
        {
            Pawn[] p = new Pawn[49];
            for (int i = 0; i < mainBoard.Length; i++)
            {
                p[i] = new Pawn(playerBoard[48 - i].field, playerBoard[i].pawnSymbol);
            }
            swapBoardPawns(mainBoard, 48-lastMove.from_1, 48-lastMove.to_1);
            swapBoardPawns(mainBoard, 48-lastMove.from_2, 48-lastMove.to_2);
            swapBoardPawns(mainBoard, 48-oldBallField, 48-newBallPawn.field);
        }
    }
}


class Pawn
{
    public int field = -1;
    public char pawnSymbol;
    public int
        x = -1,
        y = -1;
    public Pawn(int a, int b, char c)
    {
        this.field = a * 7 + b;
        this.x = a;
        this.y = b;
        this.pawnSymbol = c;
    }
    public Pawn(Pawn a)
    {
        this.field = a.field;
        this.x = a.x;
        this.y = a.y;
        this.pawnSymbol = a.pawnSymbol;
    }
    public Pawn(int pole, char token)
    {
        this.pawnSymbol = token;
        this.field = pole;
        this.x = pole / 7;
        this.y = pole % 7;
    }
    public void movePawn(int a, int b)
    {
        this.field = a * 7 + b;
        this.x = a;
        this.y = b;
    }
    public void movePawn(int pole)
    {
        this.field = pole;
        this.x = pole / 7;
        this.y = pole % 7;
    }
}
class Move : IComparable<Move>
{
    /// <summary>
    /// W związku z tym, że ruchy są 2, tym samym pionkiem albo dwoma i kilka wczesniejszych koncepcji zawiodło,
    /// robię ruch podwójny, z_1, do_1 odpowiadają za 1 ruch, i z_2, do_2 za 2 lub bez ruchu, wtedy są -1
    /// </summary>
    public int
        from_1,
        to_1,
        from_2,
        to_2,
        score = 0;
    public Move(int i, int y)
    {
        this.from_1 = i;
        this.to_1 = y;
        this.from_2 = -1;
        this.to_2 = -1;
    }
    public Move(int i, int y, int j, int h)
    {
        this.from_1 = i;
        this.to_1 = y;
        this.from_2 = j;
        this.to_2 = h;
    }

    public int CompareTo([AllowNull] Move move)
    {
        if (move == null)
            return 1;
        else
            return this.score.CompareTo(move.score);
    }

    public override string ToString()
    {
        return base.ToString() + " " + from_1 + ":" + to_1 + ", " + from_2 + ":" + to_2 + " score = " + score;
    }
}
class MoveWithBall
{
    Move move;

    public MoveWithBall()
    {

    }
}
class myTree
{
    public List<dynamic>
        fullRoundMove = new List<dynamic>(360), // All moves * possible passing ball moves
        treeRoude = new List<dynamic>(10); // tree for saving best way... i hope 
    public int treeDeep = 0;
    public myTree deeperMove;
    public myTree() { 
    }
}