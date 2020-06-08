using Amazon.MissingTypes;
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
        //showBoard();

        Player_1 = new Gracz("Gracz_1", mainBoard, 'x');
        reverseBoard();
        Player_2 = new Gracz("Gracz_2", mainBoard, 'y');
        reverseBoard();
        //showBoard();
    }
    public void start()
    {
        DateTime timeStart = DateTime.Now;
        int i = 0;
        while (!gameEnd)
        {
            i++;
            //reverseBoard();
            gameEnd = Player_1.nextMove();
            //Console.Clear();
            if (displayData)
            {
                showBoard();
            }
            showBoardbyNumbers();
            if (i>5)break;
            System.Threading.Thread.Sleep(200);
            //if (gameEnd) break;
            //reverseBoard();
            //gameEnd = Player_2.nextMove();
            ////showBoard();
        }
        showBoard();
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
            Console.Write(mainBoard[i].field + "  ");
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
    bool displayData = false;
    public char playerSymbol, playerSymbolUpper;
    string playerName = "";
    public Pawn ballPawn;
    public Pawn[] playerPawns = new Pawn[7];
    public Pawn[] mainBoard;

    //public Pawn[] possiblePAssingField; 
    public List<dynamic> 
        playerMoves = new List<dynamic>(), 
        pierwszyRuchPiona = new List<dynamic>(),
        drugiRuchPiona = new List<dynamic>(),
        possiblePassingField = new List<dynamic>(), // możliwe miejsce podania piłki
        possiblePassingPawns = new List<dynamic>(); // możliwe miejsce podania piłki gdzie stoją pionki
    bool gameEnd = false;
    public Gracz(string name, Pawn[] p, char symbol)
    {
        this.playerSymbol = symbol;
        this.playerSymbolUpper = Char.ToUpper((char)79);
        this.playerName = name;
        this.mainBoard = p;

        for (int i = 0; i < 7; i++)
        {
            mainBoard[i] = playerPawns[i] = new Pawn(mainBoard[i].field, playerSymbol);
        }

        ballPawn = mainBoard[3] = playerPawns[3] = new Pawn(mainBoard[3].field, playerSymbolUpper);
        //ballPawn = mainBoard[25] = playerPawns[3] = new Pawn(25, Char.ToUpper(playerSymbol));

        // Przykładowy start gry
        //board[2] = playerPawns[0] = new Pawn(2, playerSymbol);
        //board[4] = playerPawns[1] = new Pawn(4, playerSymbol);
        //board[11] = playerPawns[2] = new Pawn(11, playerSymbol);
        //board[12] = playerPawns[3] = new Pawn(12, Char.ToUpper(playerSymbol));
        //board[23] = playerPawns[4] = new Pawn(23, playerSymbol);
        //board[33] = playerPawns[5] = new Pawn(33, playerSymbol);
        //board[27] = playerPawns[6] = new Pawn(27, playerSymbol);

    }
    public bool nextMove()
    {
        availableMoves();
        rateMoves();
        makeMove();
        passBall();
        //Console.WriteLine("Main board " + mainBoard[11].field);
        //Console.WriteLine("player pawns " + playerPawns[0].field);
        return gameEnd;
    }
    public void rateMoves()
    {
        // Matrix3x2 int 2147483647  2_147_483_647
        findPossiblePassingField();
        if(displayData)Console.WriteLine(possiblePassingField.Count);
        //Move move;
        //for(int i = 0; i<playerMoves.Count; i++)
        foreach (Move move in playerMoves)
        {
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
        Console.WriteLine("Size = " + playerMoves.Count);
        playerMoves.Sort((x,y)=>y.score.CompareTo(x.score));
        //foreach (Move move in playerMoves){Console.Write(" : " + move.score);}
    }
    public void findPossiblePassingField()
    {
        possiblePassingField.Clear(); 
        possiblePassingPawns.Clear();
        Console.WriteLine(ballPawn.field + ". " + ballPawn.pawnSymbol);
        //tu będzie łopatologicznie bo czemu by nie
        for (int i = ballPawn.field + 1; i < ((ballPawn.field / 7) * 7 + 7); i++)
        {//przeszukiwanie w lewo
            if (mainBoard[i].pawnSymbol != '-' && mainBoard[i].pawnSymbol != playerSymbol)
            {
                break;
            }
            possiblePassingField.Add(new Pawn(mainBoard[i]));
        }
        for (int i = ballPawn.field - 1; i >= (ballPawn.field / 7); i--)
        {//przeszukiwanie w prawo
            if (mainBoard[i].pawnSymbol != '-' && mainBoard[i].pawnSymbol != playerSymbol)
            {
                break;
            }
            possiblePassingField.Add(new Pawn(mainBoard[i]));
        }
        for (int i = ballPawn.field + 7; i < 49; i += 7)
        {//przeszukiwanie w góre
            if (mainBoard[i].pawnSymbol != '-' && mainBoard[i].pawnSymbol != playerSymbol)
            {
                break;
            }
            possiblePassingField.Add(new Pawn(mainBoard[i]));
        }
        for (int i = ballPawn.field - 7; i > 0; i -= 7)
        {//przeszukiwanie w dół
            if (mainBoard[i].pawnSymbol != '-' && mainBoard[i].pawnSymbol != playerSymbol)
            {
                break;
            }
            possiblePassingField.Add(new Pawn(mainBoard[i]));
        }
        for (int i = ballPawn.field + 8; i < 49; i += 8)
        {//przeszukiwanie w lewo na ukos w góre
            if (mainBoard[i].pawnSymbol != '-' && mainBoard[i].pawnSymbol != playerSymbol)
            {
                break;
            }
            possiblePassingField.Add(new Pawn(mainBoard[i].field, mainBoard[i].pawnSymbol));
            if (i % 7 == 6) break;
        }
        for (int i = ballPawn.field - 6; i > 0; i -= 6)
        {//przeszukiwanie w lewo na ukos w dół
            if (mainBoard[i].pawnSymbol != '-' && mainBoard[i].pawnSymbol != playerSymbol)
            {
                break;
            }
            possiblePassingField.Add(new Pawn(mainBoard[i]));
            if (i % 7 == 6) break;
        }
        for (int i = ballPawn.field + 6; i < 49; i += 6)
        {//przeszukiwanie w prawo na ukos w góre
            if (mainBoard[i].pawnSymbol != '-' && mainBoard[i].pawnSymbol != playerSymbol)
            {
                break;
            }
            possiblePassingField.Add(new Pawn(mainBoard[i]));
            if (i % 7 == 0) break;
        }
        for (int i = ballPawn.field - 8; i > 0; i -= 8)
        {//przeszukiwanie w prawo na ukos w dół
            if (mainBoard[i].pawnSymbol != '-' && mainBoard[i].pawnSymbol != playerSymbol)
            {
                break;
            }
            possiblePassingField.Add(new Pawn(mainBoard[i]));
            if (i % 7 == 0) break;
        }

        //possiblePassingField.Sort((x, y) => y.field.CompareTo(x.field));
        //foreach (Pawn p in possiblePassingField)
        //{
        //    Console.Write(p.field + ", ");
        //    if (p.pawnSymbol == playerSymbol)
        //    {
        //        possiblePassingPawns.Add(new Pawn(p));
        //    }
        //}

        Console.WriteLine("possible pass field " + possiblePassingField.Count);
        Console.WriteLine("possible pass pawns " + possiblePassingPawns.Count);
    }
    private void availableMoves()
    {
        showBoard();
        for(int i = 0; i < playerPawns.Length; i++){
            checkAvailableMoves(mainBoard, playerPawns, pierwszyRuchPiona, true, playerPawns[i].field, 0, 0);
        }
        if(displayData)Console.WriteLine("Drugi ruch piona = " + drugiRuchPiona.Count);
        playerMoves.AddRange(drugiRuchPiona);
        pierwszyRuchPiona.Clear();
        drugiRuchPiona.Clear();
    }
    private void checkAvailableMoves(Pawn[] board, Pawn[] playerPawns, List<dynamic> lista, bool stepOne, int from_2, int from_1, int to_1)
    {
        int[] moveDirection = { 7, -1, 1}; // bez ruchu w tył -7   //int[] moveDirection = { 7, -7, 1, -1 }; // kierunek w którym można ruszyć pionem
        int to_2;  // pole - miejsce w które pionek może się przesunąć, from_2 - miejsce na którym jest
        bool outsideBoard;
        for (int direction = 0; direction < moveDirection.Length; direction++)
        {
            outsideBoard = true;
            to_2 = from_2 + moveDirection[direction];
            if(moveDirection[direction] ==  1 && from_2 % 7 == 6) outsideBoard = false;  //tutaj 2 warunki wychodzenia poza plansze(aby nie przeskoczyć przy polu +1/-1 o całą szerkość planszy będącej jeden rząd wyżej/niżej  
            if(moveDirection[direction] == -1 && from_2 % 7 == 0) outsideBoard = false;

            //board[from_2].pawnSymbol != playerSymbolUpper  blokuje ruch pionka z piłką
            //showBoard(board);
            if (to_2 >= 0 && to_2 < 49 && outsideBoard && board[to_2].pawnSymbol == '-' && board[from_2].pawnSymbol != playerSymbolUpper)
            {
                if (stepOne)
                {   //Console.WriteLine("Z = " + i + "DO " + pole);
                    //lista.Add(new Move(from_2, to_2)); // chwilowo wyłączone krótkie ruchy
                    Pawn[] newBoardForSecoundMove = deepCopyBoard(board);
                    Pawn[] newUserPawnsForSecoundMove = deepCopyPlayerPawns(playerPawns);
                    swapBoardPawns(newBoardForSecoundMove, from_2, to_2);
                    swapPlayerPawns(newUserPawnsForSecoundMove, from_2, to_2);

                    for(int i=0; i<playerPawns.Length; i++)
                    {
                        checkAvailableMoves(newBoardForSecoundMove, newUserPawnsForSecoundMove, drugiRuchPiona, false, newUserPawnsForSecoundMove[i].field, from_2, to_2);
                    }
                }
                else
                {
                    lista.Add(new Move(from_1, to_1, from_2, to_2));
                }
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
    public void makeMove()
    {
        possiblePassingPawns.Clear();
        //int x = new Random().Next(playerMoves.Count);
        int x = new Random().Next(10);
        //Console.WriteLine(x);
        Move wykonaj = playerMoves[x];

        Console.WriteLine(wykonaj);
        if(wykonaj.to_2 == -1)
        {//dla jednego krótkiego ruchu, nie wiadomo, czy będą wykonywane
            swapBoardPawns(mainBoard, wykonaj.from_1, wykonaj.to_1);
            swapPlayerPawns(playerPawns, wykonaj.from_1, wykonaj.to_1);
            if (wykonaj.to_1 > 41) gameEnd = true;
            showPawns();
        }
        else
        {
            swapBoardPawns(mainBoard, wykonaj.from_1, wykonaj.to_1);
            swapBoardPawns(mainBoard, wykonaj.from_2, wykonaj.to_2);
            swapPlayerPawns(playerPawns, wykonaj.from_1, wykonaj.to_1);
            swapPlayerPawns(playerPawns, wykonaj.from_2, wykonaj.to_2);

            //swapBoardPawns(mainBoard, wykonaj.from_1, wykonaj.to_2);
            //swapPlayerPawns(playerPawns, wykonaj.from_1, wykonaj.to_2);
            if (wykonaj.to_1 > 41) gameEnd = true;
            if (wykonaj.to_2 > 41) gameEnd = true;
            if (displayData) showPawns();
        }
        foreach(Pawn p in possiblePassingPawns)
        {
            if (p.field == wykonaj.from_1) possiblePassingPawns.Remove(p);
        }
        //Console.Write("Ruchy:" + playerMoves.Count + ",   //");
        foreach (Pawn p in possiblePassingField)
        {
            if (p.pawnSymbol == playerSymbol)
            {
                possiblePassingPawns.Add(new Pawn(p));
            }
        }
        Console.Write("Possible moves: " + possiblePassingPawns.Count);
        possiblePassingPawns.Sort((x, y) => y.field.CompareTo(x.field));
        Console.WriteLine();
        foreach (Pawn p in possiblePassingPawns)
        {
            Console.Write(p.field + ", ");
        }
        //playerMoves.Clear();
    }
    public void passBall()
    {
        Pawn chosenPawn = null;
        int localScore = 0;
        bool go = false;
        foreach (Pawn pawn in possiblePassingPawns)
        {
            foreach (Move move in playerMoves)
            {
                if (pawn.pawnSymbol == playerSymbol && move.score > localScore && move.to_2 == pawn.field)
                {
                    Console.WriteLine();
                    Console.WriteLine("MOVEEE  score" + move.score + ", " + move.to_2 + ", " + pawn.field);
                    localScore = move.score;
                    chosenPawn = pawn;
                }
            }
        }
        foreach (Pawn p in mainBoard)
        {
            if (p.field == chosenPawn.field)
            {
                go = true;
                chosenPawn = p;
                Console.WriteLine("CHANGING PAWNS " + p.field);
                //Console.WriteLine("chosen =  " + chosenPawn.field);
            }
        }
        Console.WriteLine("Moves " + possiblePassingPawns.Count);
        Console.WriteLine("score " + localScore);
        if (chosenPawn != null)
            if (go)
            {
                Console.WriteLine("PASSING BALL: from - ");
                chosenPawn.pawnSymbol = playerSymbolUpper;
                mainBoard[ballPawn.field].pawnSymbol = playerSymbol;
                for (int i = 0; i < playerMoves.Count; i++)
                {
                    if (playerMoves[i].to_2 == mainBoard[ballPawn.field].field)
                    {
                        playerMoves[i] = new Pawn(chosenPawn.field, chosenPawn.pawnSymbol);
                    }
                }
                ballPawn.field = chosenPawn.field;
            }
        }
        //chosenPawn.pawnSymbol = playerSymbolUpper;
        //ballPawn.pawnSymbol = playerSymbol;
        //mainBoard.ElementAt(chosenPawn.field).pawnSymbol = playerSymbolUpper;
        //mainBoard.ElementAt(ballPawn.field).pawnSymbol = playerSymbol;
        //mainBoard.ElementAt(ballPawn.field).field = chosenPawn.field;

        playerMoves.Clear();
    }
    public void swapBoardPawns(Pawn[] board, int from, int to)
    {
        char q = board[to].pawnSymbol;
        board[to].pawnSymbol = board[from].pawnSymbol;
        board[from].pawnSymbol = q;
    }
    public void swapPlayerPawns(Pawn[] pawns, int from, int to)
    {
        for (int i = 0; i < pawns.Length; i++)
        {
            if (pawns[i].field == from)
            {
                pawns[i] = new Pawn(to, pawns[i].pawnSymbol);
            }
        }
    }
    public void showPawns()
    {
        Console.WriteLine("Pionki, gracza");
        for (int i=0; i<7; i++)
        {
            Console.Write(playerPawns[i].x + ":" + playerPawns[i].y + ", ");
        }
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