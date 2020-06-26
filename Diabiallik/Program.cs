using Diabiallik;
using System;
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
            diaballik.start();
        }
    }
}

class Diaballik
{
    bool displayData = false;
    bool gameEnd = false;
    public Pawn[]
        mainBoard = new Pawn[49];
    public Gracz 
        Player_1, 
        Player_2;
    public Diaballik()
    {
        Console.WriteLine("Nowa Gra");
        newBoard();
        Player_1 = new Gracz("Gracz_1", mainBoard, 'x');
        reverseBoard();
        Player_2 = new Gracz("Gracz_2", mainBoard, 'y');
        reverseBoard();
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
            //gameEnd = Player_1.nextMove();
            //showBoardbyNumbers();
            //showBoard();
            //reverseBoard();
            //Console.Clear();
            if (displayData)
            {
            }
            //System.Threading.Thread.Sleep(400);
            if (i>40)break;
            if (gameEnd) break;
            //showBoardbyNumbers();
            gameEnd = Player_2.nextMove();
            showBoardbyNumbers();
            showBoard();
            //reverseBoard();
        }
        //showBoard();
        DateTime timeEnd = DateTime.Now;
        //Console.WriteLine("Ilość dostepnych ruchów = " + gracz1.iloscRuchow() + ", " + gracz2.iloscRuchow());
        Console.WriteLine("KONIEC: " + timeEnd.Subtract(timeStart));
    }
    private void newBoard()
    {
        Console.WriteLine("Inicjuj plansze");
        for (int i = 0; i < mainBoard.Length; i++)
        {
            mainBoard[i] = new Pawn(i, '-');
        }
    }
    private void showBoard()
    {
        Console.WriteLine("Pokaż plansze");
        for (int i = mainBoard.Length - 1; i >= 0; i--)
        {
            Console.Write(mainBoard[i].pawnSymbol + "  ");
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
    private void reverseBoard()
    {
        Array.Reverse(mainBoard);
    }
}

class Gracz
{
    bool displayData = true;
    public char playerSymbol, playerSymbolUpper;
    string playerName = "";
    public Pawn ballPawn;
    public Pawn[] playerPawns = new Pawn[7];
    public Pawn[] mainBoard;

    //public Pawn[] possiblePAssingField; 
    public List<dynamic> 
        playerMoves = new List<dynamic>(), 
        shortMove = new List<dynamic>(), // move one field
        longMove = new List<dynamic>(), // move 2 fields
        possiblePassingField = new List<dynamic>(), // możliwe miejsce podania piłki
        possiblePassingPawns = new List<dynamic>(); // możliwe miejsce podania piłki gdzie stoją pionki
    bool gameEnd = false;
    public Gracz(string name, Pawn[] p, char symbol)
    {
        this.playerSymbol = symbol;
        this.playerSymbolUpper = Char.ToUpper(playerSymbol);
        this.playerName = name;
        this.mainBoard = p;

        for (int i = 0; i < 7; i++)
        {
            mainBoard[i] = playerPawns[i] = new Pawn(mainBoard[i].field, playerSymbol);
        }
        ballPawn = mainBoard[3] = playerPawns[3] = new Pawn(mainBoard[3].field, playerSymbolUpper);
    }
    public bool nextMove()
    {
        Console.WriteLine("NEW MOVE " + playerSymbolUpper);
        //showBoard();
        showPawns();
        availableMoves();
        rateMoves();
        makeMove();
        showPawns();
        //showBoard();
        passBall();
        //showBoard();
        return gameEnd;
    }
    private void availableMoves()
    {
        playerMoves.Clear();
        for (int i = 0; i < playerPawns.Length; i++)
        {
            checkAvailableMoves(mainBoard, playerPawns, shortMove, true, playerPawns[i].field, 0, 0);
        }
        if (displayData) Console.WriteLine("Drugi ruch piona = " + longMove.Count);
        playerMoves.AddRange(longMove);
        shortMove.Clear();
        longMove.Clear();
    }
    private void checkAvailableMoves(Pawn[] board, Pawn[] playerPawns, List<dynamic> lista, bool stepOne, int from_2, int from_1, int to_1)
    {
        //Console.WriteLine("New TRY: " + from_2 + ", ");
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
            if (to_2 >= 0 && to_2 < 49 && insideBoard && board[to_2].pawnSymbol == '-' && board[mainBoard[from_2].field].pawnSymbol != playerSymbolUpper)
            {
                if (stepOne)
                {   //Console.WriteLine("Z = " + i + "DO " + pole);
                    //lista.Add(new Move(from_2, to_2)); // chwilowo wyłączone krótkie ruchy
                    Pawn[] newBoardForSecoundMove = deepCopyBoard(board);
                    Pawn[] newUserPawnsForSecoundMove = deepCopyPlayerPawns(playerPawns);

                    swapBoardPawns(newBoardForSecoundMove, mainBoard[from_2].field, mainBoard[to_2].field);
                    //swapPlayerPawns(newUserPawnsForSecoundMove, newBoardForSecoundMove, mainBoard[from_2].field, mainBoard[to_2].field);

                    swapPlayerPawns(newUserPawnsForSecoundMove, newBoardForSecoundMove, mainBoard[from_2].field, mainBoard[to_2].field);
                    //swapBoardPawns(newBoardForSecoundMove, from_2, to_2);
                    //showBoard(newBoardForSecoundMove);
                    //showPawns(newUserPawnsForSecoundMove);
                    for (int i = 0; i < playerPawns.Length; i++)
                    {
                        checkAvailableMoves(newBoardForSecoundMove, newUserPawnsForSecoundMove, longMove, false, newUserPawnsForSecoundMove[i].field, from_2, to_2);
                    }
                }
                else
                {
                    //Console.WriteLine("Adding new move: from1- " + from_1 + "  to1- " + to_1 + "  from2- " + from_2 + "  to2- " + to_2);
                    lista.Add(new Move(from_1, to_1, from_2, to_2));
                }
            }
        }
    }
    public void rateMoves()
    {
        findPossiblePassingField();
        foreach (Move move in playerMoves)
        {
            //Console.WriteLine("Move: " + move.from_1 + ":" + move.to_1 + ", " + move.from_2 + ":" + move.to_2);
            if (move.to_2 > move.from_1 + 5) { move.score += 1000; } // skok o linie w przód
            if (move.to_2 > move.from_1 + 10) { move.score += 5000; } // skok o 2 linie w przód
            foreach (Pawn passingMove in possiblePassingField)//sprawdzanie czy da się wejsć na pole na którym może nastapić podanie piłki
            {
                if (move.to_2 == passingMove.field)
                {
                    move.score += 20_000;
                    if (move.to_2 > 41) move.score += 1_000_000; // koniec gry ^^
                }
            }
            //if (move.to_2 < 41)
            //{
            //    if (mainBoard[move.to_2 + 7].pawnSymbol != '-' && mainBoard[move.to_2 + 7].pawnSymbol != playerSymbol) // blokowanie pionka przeciwnika
            //    {
            //        move.score += 1000; //daleko od startu
            //        if (move.from_1 < 20) move.score += 3000; // w połowie planszy
            //        if (move.from_1 < 0) move.score += 30_000; // blisko swojego startu, zapobiega wygranejprzeciwnika
            //    }
            //}
        }
        Console.WriteLine();
        playerMoves.Sort((x,y)=>y.score.CompareTo(x.score));
    }
    public void findPossiblePassingField()
    {
        possiblePassingField.Clear(); 
        possiblePassingPawns.Clear();
        if(displayData)Console.WriteLine("Ball field and symbol: " + ballPawn.field + ". " + ballPawn.pawnSymbol);

        //tu będzie łopatologicznie bo czemu by nie
        int[] start = new int[] {
            ballPawn.field + 1,
            ballPawn.field + 7,
            ballPawn.field + 8,
            ballPawn.field + 6,
            ballPawn.field - 1,
            ballPawn.field - 7,
            ballPawn.field - 6,
            ballPawn.field - 8
        };
        int[] finish = new int[] { 49, 49, 49, 49, 0, 0, 0, 0};
        int[] jump = new int[] { 1, 7, 8, 6, -1, -7, -6, -8};
            
        if(playerSymbolUpper == 'X')
        {
            for (int i = 0; i < 4; i++)
            {
                for (int y = start[i]; y < finish[i]; y += jump[i])
                {
                    if (y % 7 == 6 || y % 7 == 0)
                    {
                        if (start[i] == 6) break;
                        if (start[i] == 1) break;
                        if (mainBoard[y].pawnSymbol != '-' && mainBoard[y].pawnSymbol != playerSymbol) break;
                        possiblePassingField.Add(new Pawn(mainBoard[y]));
                        break;
                    }
                    if (mainBoard[y].pawnSymbol != '-' && mainBoard[y].pawnSymbol != playerSymbol)
                    {
                        break;
                    }
                    possiblePassingField.Add(new Pawn(mainBoard[y]));
                }
            }
            for (int i = 4; i < 8; i++)
            {
                for (int y = start[i]; y >= finish[i]; y += jump[i])
                {
                    if (y < 0)
                    {
                        break;
                    }
                    if (y % 7 == 6 || y % 7 == 0)
                    {
                        if (start[i] == -6) break;
                        if (start[i] == -1) break;
                        possiblePassingField.Add(new Pawn(mainBoard[y]));
                        break;
                    }
                    if (mainBoard[y].pawnSymbol != '-' && mainBoard[y].pawnSymbol != playerSymbol)
                    {
                        break;
                    }
                    possiblePassingField.Add(new Pawn(mainBoard[y]));
                }
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                for (int y = start[i]; y < finish[i]; y += jump[i])
                {
                    if (y % 7 == 6 || y % 7 == 0)
                    {
                        if (start[i] == 6) break;
                        if (start[i] == 1) break;
                        if (mainBoard[y].pawnSymbol != '-' && mainBoard[y].pawnSymbol != playerSymbol) break;
                        possiblePassingField.Add(new Pawn(mainBoard[y]));
                        break;
                    }
                    if (mainBoard[y].pawnSymbol != '-' && mainBoard[y].pawnSymbol != playerSymbol)
                    {
                        break;
                    }
                    possiblePassingField.Add(new Pawn(mainBoard[y]));
                }
            }
            for (int i = 4; i < 8; i++)
            {
                for (int y = start[i]; y >= finish[i]; y += jump[i])
                {
                    if (y < 0)
                    {
                        break;
                    }
                    if (y % 7 == 6 || y % 7 == 0)
                    {
                        if (start[i] == -6) break;
                        if (start[i] == -1) break;
                        possiblePassingField.Add(new Pawn(mainBoard[y]));
                        break;
                    }
                    if (mainBoard[y].pawnSymbol != '-' && mainBoard[y].pawnSymbol != playerSymbol)
                    {
                        break;
                    }
                    possiblePassingField.Add(new Pawn(mainBoard[y]));
                }
            }
        }





        {
            //for (int i = ballPawn.field + 1; i < ((ballPawn.field / 7) * 7 + 7); i++)
            //{//przeszukiwanie w lewo
            //    if (mainBoard[i].pawnSymbol != '-' && mainBoard[i].pawnSymbol != playerSymbol)
            //    {
            //        break;
            //    }
            //    Console.WriteLine(" I = " + i);
            //    possiblePassingField.Add(new Pawn(mainBoard[i].field, mainBoard[i].pawnSymbol));
            //}
            //for (int i = ballPawn.field - 1; i >= (ballPawn.field / 7); i--)
            //{//przeszukiwanie w prawo
            //    if (mainBoard[i].pawnSymbol != '-' && mainBoard[i].pawnSymbol != playerSymbol)
            //    {
            //        break;
            //    }
            //    possiblePassingField.Add(new Pawn(mainBoard[i].field, mainBoard[i].pawnSymbol));
            //}
            //for (int i = ballPawn.field + 7; i < 49; i += 7)
            //{//przeszukiwanie w góre
            //    if (mainBoard[i].pawnSymbol != '-' && mainBoard[i].pawnSymbol != playerSymbol)
            //    {
            //        break;
            //    }
            //    possiblePassingField.Add(new Pawn(mainBoard[i].field, mainBoard[i].pawnSymbol));
            //}
            //for (int i = ballPawn.field - 7; i > 0; i -= 7)
            //{//przeszukiwanie w dół
            //    if (mainBoard[i].pawnSymbol != '-' && mainBoard[i].pawnSymbol != playerSymbol)
            //    {
            //        break;
            //    }
            //    possiblePassingField.Add(new Pawn(mainBoard[i].field, mainBoard[i].pawnSymbol));
            //}
            //for (int i = ballPawn.field + 8; i < 49; i += 8)
            //{//przeszukiwanie w lewo na ukos w góre
            //    if (mainBoard[i].pawnSymbol != '-' && mainBoard[i].pawnSymbol != playerSymbol)
            //    {
            //        break;
            //    }
            //    possiblePassingField.Add(new Pawn(mainBoard[i].field, mainBoard[i].pawnSymbol));
            //    if (i % 7 == 6) break;
            //}
            //for (int i =ballPawn.field - 6; i > 0; i -= 6)
            //{//przeszukiwanie w lewo na ukos w dół
            //    if (mainBoard[i].pawnSymbol != '-' && mainBoard[i].pawnSymbol != playerSymbol)
            //    {
            //        break;
            //    }
            //    possiblePassingField.Add(new Pawn(mainBoard[i].field, mainBoard[i].pawnSymbol));
            //    if (i % 7 == 6) break;
            //}
            //for (int i = ballPawn.field + 6; i < 49; i += 6)
            //{//przeszukiwanie w prawo na ukos w góre
            //    if (mainBoard[i].pawnSymbol != '-' && mainBoard[i].pawnSymbol != playerSymbol)
            //    {
            //        break;
            //    }
            //    possiblePassingField.Add(new Pawn(mainBoard[i].field, mainBoard[i].pawnSymbol));
            //    if (i % 7 == 0) break;
            //}
            //for (int i = ballPawn.field - 8; i > 0; i -= 8)
            //{//przeszukiwanie w prawo na ukos w dół
            //    if (mainBoard[i].pawnSymbol != '-' && mainBoard[i].pawnSymbol != playerSymbol)
            //    {
            //        break;
            //    }
            //    possiblePassingField.Add(new Pawn(mainBoard[i].field, mainBoard[i].pawnSymbol));
            //    if (i % 7 == 0) break;
            //}
        }


        possiblePassingPawns.Clear();
        possiblePassingField.Sort((x, y) => y.field.CompareTo(x.field));
        Console.WriteLine("Pssible passing field: ");
        foreach (Pawn p in possiblePassingField)
        {
            Console.Write(p.field + ", ");
            if (p.pawnSymbol == playerSymbol)
            {
                possiblePassingPawns.Add(new Pawn(p));
            }
        }
        if (displayData) Console.WriteLine();
        if (displayData) Console.WriteLine("possible pass field " + possiblePassingField.Count);
        //Console.WriteLine("possible pass pawns " + possiblePassingPawns.Count);
    }
    public void makeMove()
    {
        //int x = new Random().Next(playerMoves.Count);
        int mov = new Random().Next(playerMoves.Count-1);
        Move wykonaj = playerMoves[mov];
        //foreach(Move m in playerMoves)
        //{
        //    Console.WriteLine("Adding new move: from1- " + m.from_1 + "  to1- " + m.to_1 + "  from2- " + m.from_2 + "  to2- " + m.to_2);
        //}

        if (displayData) Console.WriteLine("Wykonaj ruch: " + wykonaj);
        int x = 0;
        if (mainBoard[0].field > 30) x = 48;
        if(wykonaj.to_2 == -1)
        {//dla jednego krótkiego ruchu, nie wiadomo, czy będą wykonywane
            swapBoardPawns(mainBoard, wykonaj.from_1, wykonaj.to_1);
            swapPlayerPawns(playerPawns, wykonaj.from_1, wykonaj.to_1);
            //if (wykonaj.to_1 > 41) gameEnd = true;
            showPawns();
        }
        else 
        {
            if (x < 30)
            {
                //showPawns();
                swapBoardPawns(mainBoard, wykonaj.from_1, wykonaj.to_1);
                swapBoardPawns(mainBoard, wykonaj.from_2, wykonaj.to_2);
                swapPlayerPawns(playerPawns, mainBoard[wykonaj.from_1].field, mainBoard[wykonaj.to_1].field);
                swapPlayerPawns(playerPawns, mainBoard[wykonaj.from_2].field, mainBoard[wykonaj.to_2].field);
                //showPawns();
                //swapBoardPawns(mainBoard, wykonaj.from_1, wykonaj.to_2);
                //swapPlayerPawns(playerPawns, wykonaj.from_1, wykonaj.to_2);
                //if (wykonaj.to_1 > 41) gameEnd = true;
                //if (wykonaj.to_2 > 41) gameEnd = true;
            }
            else
            {
                //showPawns();
                swapBoardPawns(mainBoard, x - wykonaj.from_1, x - wykonaj.to_1);
                swapBoardPawns(mainBoard, x - wykonaj.from_2, x - wykonaj.to_2);
                swapPlayerPawns(playerPawns, mainBoard[wykonaj.from_1].field, mainBoard[wykonaj.to_1].field);
                swapPlayerPawns(playerPawns, mainBoard[wykonaj.from_2].field, mainBoard[wykonaj.to_2].field);
                showPawns();
                //swapBoardPawns(mainBoard, wykonaj.from_1, wykonaj.to_2);
                //swapPlayerPawns(playerPawns, wykonaj.from_1, wykonaj.to_2);
                if (wykonaj.to_1 < 7) gameEnd = true;
                if (wykonaj.to_2 < 7) gameEnd = true;
                //showPawns();
            }
        }   

        possiblePassingPawns.Clear();
        findPossiblePassingField();
        Console.Write("Possible moves: " + possiblePassingPawns.Count);
        //foreach (Pawn p in possiblePassingField)
        //{
        //    if (p.pawnSymbol == playerSymbol)
        //    {
        //        if (displayData) Console.WriteLine("Adding move to field: " + p.field);
        //        possiblePassingPawns.Add(p);
        //        //possiblePassingPawns.Add(new Pawn(p));
        //    }
        //}
        //Console.Write("Possible moves: " + possiblePassingPawns.Count);

        //possiblePassingPawns.Sort((x, y) => y.field.CompareTo(x.field));
        Console.WriteLine();
        foreach (Pawn p in possiblePassingPawns)
        {
            if (displayData) Console.Write(p.field + ", ");
        }
        Console.WriteLine();
        //playerMoves.Clear();
    }
    public void passBall()
    {
        if (displayData) Console.WriteLine("PASSBALL");
        if (displayData) Console.WriteLine("possiblePassingPawns:" + possiblePassingPawns.Count);
        Console.WriteLine();
        int random = new Random().Next(possiblePassingPawns.Count-1);
        if (displayData) Console.WriteLine("random = " + random);
        swapBoardPawns(mainBoard, ballPawn.field, possiblePassingPawns[random].field);
        ballPawn = (Pawn)mainBoard[possiblePassingPawns[random].field];
        if (displayData) Console.WriteLine("ball field = " + ballPawn.field);
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
        //foreach (Pawn p in mainBoard)
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
        //    //mainBoard[ballPawn.field].pawnSymbol = playerSymbol;
        //    //for (int i = 0; i < playerMoves.Count; i++)
        //    //{
        //    //    if (playerMoves[i].to_2 == mainBoard[ballPawn.field].field)
        //    //    {
        //    //        playerMoves[i] = new Pawn(chosenPawn.field, chosenPawn.pawnSymbol);
        //    //    }
        //    //}
        //    //ballPawn.field = chosenPawn.field;

        //        swapBoardPawns(mainBoard, ballPawn.field, chosenPawn.field);
        //    }
        ////chosenPawn.pawnSymbol = playerSymbolUpper;
        ////ballPawn.pawnSymbol = playerSymbol;
        ////mainBoard.ElementAt(chosenPawn.field).pawnSymbol = playerSymbolUpper;
        ////mainBoard.ElementAt(ballPawn.field).pawnSymbol = playerSymbol;
        ////mainBoard.ElementAt(ballPawn.field).field = chosenPawn.field;

        playerMoves.Clear();
    }
    public void swapBoardPawns(Pawn[] board, int from, int to)
    {
        char q = board[to].pawnSymbol;
        board[to].pawnSymbol = board[from].pawnSymbol;
        board[from].pawnSymbol = q;
        //Console.WriteLine("Plansza z: " + board[from].field + ", znak = " + board[from].pawnSymbol);
        //Console.WriteLine("Plansza do: " + board[to].field + ", znak = " + board[to].pawnSymbol);
    }
    public void swapPlayerPawns(Pawn[] pawns, int from, int to)
    {
        //Console.WriteLine("from:" + from + " to:" + to + " pawns[0]:" + pawns[0].field +"");
        for (int i = 0; i < pawns.Length; i++)
        {
            if (pawns[i].field == mainBoard[from].field)
            //if (pawns[i].field == from)
            {
               // Console.WriteLine("CHANGE PAWN " + " field: " + mainBoard[to].field + " is now " + mainBoard[to].pawnSymbol  + " " + pawns[i].field  + " is now " + pawns[i].pawnSymbol + "");
                pawns[i] = mainBoard[to];
                pawns[i].pawnSymbol = mainBoard[to].pawnSymbol;
            }
        }
    }
    public void swapPlayerPawns(Pawn[] pawns, Pawn[] board, int from, int to)
    {
        for (int i = 0; i < pawns.Length; i++)
        {
            if (pawns[i].field == mainBoard[from].field)
            //if (pawns[i].field == from)
            {
                //Console.WriteLine("CHANGE PAWN " + " field: " + mainBoard[to].field + " is now " + pawns[i].field + "" + mainBoard[to].pawnSymbol + " is now " + pawns[i].pawnSymbol + "");
                pawns[i] = board[to];
                //pawns[i].pawnSymbol = mainBoard[to].pawnSymbol;
            }
        }
    }
    public Pawn[] deepCopyBoard(Pawn[] board)
    {
        Pawn[] copiedBoard = new Pawn[49];
        for (int i = 0; i < board.Length; i++)
        {
            copiedBoard[i] = new Pawn(board[i].field, board[i].pawnSymbol); // głębokie kopiowanie planszy
        }
        return copiedBoard;
    }
    public Pawn[] deepCopyPlayerPawns(Pawn[] pawns)
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
        Console.WriteLine();
        Console.Write("Pionki, gracza : ");
        for (int i=0; i<playerPawns.Length; i++)
        {
            Console.Write(playerPawns[i].field + ":" + playerPawns[i].pawnSymbol + ", ");
        }
        Console.WriteLine();
    }
    public void showPawns(Pawn[] p)
    {
        Console.WriteLine();
        Console.Write("Pionki, gracza : ");
        for (int i = 0; i < 7; i++)
        {
            Console.Write(p[i].field + ":" + p[i].pawnSymbol + ", ");
        }
        Console.WriteLine();
    }
    private void showBoard()
    {
        Console.WriteLine("\nPokaż plansze: " + playerName);
        for (int i = mainBoard.Length - 1; i >= 0; i--)
        {
            Console.Write(mainBoard[i].pawnSymbol + "  ");
            if (i % 7 == 0)
            {
                Console.WriteLine("");
            }
        }
    }
    private void showBoard(Pawn[] plansza4)
    {
        //showBoardbyNumbers();
        Console.WriteLine("\nPokaż plansze: " + playerName);
        for (int i = plansza4.Length - 1; i >= 0; i--)
        {
            Console.Write(plansza4[i].pawnSymbol + "  ");
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