using Amazon.MissingTypes;
using System;
using System.Collections.Generic;
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
        showBoard();

        Player_1 = new Gracz("Gracz_1", mainBoard, 'x');
        reverseBoard();
        Player_2 = new Gracz("Gracz_2", mainBoard, 'y');
        showBoard();
    }
    public void start()
    {
        DateTime timeStart = DateTime.Now;
        while (!gameEnd)
        {
            reverseBoard();
            gameEnd = Player_1.nextMove();
            //Console.Clear();
            showBoard();
            //System.Threading.Thread.Sleep(200);
            if (gameEnd) break;
            reverseBoard();
            gameEnd = Player_2.nextMove();
            //showBoard();
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
            mainBoard[i] = new Pawn(i, i%7, '-');
        }
    }
    private void showBoard()
    {
        Console.WriteLine("Pokaż plansze");
        for(int i = mainBoard.Length-1; i >= 0; i--)
        {
            Console.Write(mainBoard[i].pawnSymbol + "  ");
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
    public char playerSymbol;
    string playerName = "";
    public Pawn[] playerPawns = new Pawn[7];
    public Pawn[] 
        mainBoard,
        secondBoard;
    public List<dynamic> 
        playerMoves = new List<dynamic>(), 
        pierwszyRuchPiona = new List<dynamic>(),
        drugiRuchPiona = new List<dynamic>();
    bool gameEnd = false;
    public Gracz(string name, Pawn[] p, char symbol)
    {
        this.playerSymbol = symbol;
        this.playerName = name;
        this.mainBoard = p;
        this.secondBoard = p;

        for (int i = 0; i < 7; i++)
        {
            mainBoard[i] = playerPawns[i] = new Pawn(0, i, playerSymbol);
        }
        mainBoard[3] = playerPawns[3] = new Pawn(3, Char.ToUpper(playerSymbol));

        // Przykładowy start gry
        //board[2] = playerPawns[0] = new Pawn(2, playerSymbol);
        //board[4] = playerPawns[1] = new Pawn(4, playerSymbol);
        //board[11] = playerPawns[2] = new Pawn(11, playerSymbol);
        //board[12] = playerPawns[3] = new Pawn(12, Char.ToUpper(playerSymbol));
        //board[23] = playerPawns[4] = new Pawn(23, playerSymbol);
        //board[33] = playerPawns[5] = new Pawn(33, playerSymbol);
        //board[27] = playerPawns[6] = new Pawn(27, playerSymbol);

    }
    public int movesCount()
    {
        return playerMoves.Count;
    }
    public bool nextMove()
    {
        availableMoves();
        makeMove();
        return gameEnd;
    }
    private void availableMoves()
    {
        for(int i = 0; i < playerPawns.Length; i++){
            checkAvailableMoves(mainBoard, playerPawns, pierwszyRuchPiona, true, playerPawns[i].field, 0, 0);
        }
        playerMoves.AddRange(drugiRuchPiona);
        pierwszyRuchPiona.Clear();
        drugiRuchPiona.Clear();
    }
    private void checkAvailableMoves(Pawn[] board, Pawn[] playerPawns, List<dynamic> lista, bool stepOne, int from_2, int from_1, int to_1)
    {
        int[] moveDirection = { 7, -1, 1}; // bez ruchu w tył -7   //int[] moveDirection = { 7, -7, 1, -1 }; // kierunek w którym można ruszyć pionem
        int to_2;  // pole - miejsce w które pionek może się przesunąć, i - miejsce na którym jest
        bool outsideBoard;
        for (int direction = 0; direction < moveDirection.Length; direction++)
        {
            outsideBoard = true;
            to_2 = from_2 + moveDirection[direction];
            if(moveDirection[direction] ==  1 && from_2 % 7 == 6) outsideBoard = false;  //tutaj 2 warunki wychodzenia poza plansze(aby nie przeskoczyć przy polu +1/-1 o całą szerkość planszy będącej jeden rząd wyżej/niżej  
            if(moveDirection[direction] == -1 && from_2 % 7 == 0) outsideBoard = false;
            
            if (to_2 >= 0 && to_2 < 49 && outsideBoard && board[to_2].pawnSymbol == '-')
            {
                if (stepOne)
                {   //Console.WriteLine("Z = " + i + "DO " + pole);
                    //lista.Add(new Ruch(i, pole)); // chwilowo wyłączone krótkie ruchy
                    Pawn[] newBoardForSecoundMove = deepCopyBoard(board);
                    Pawn[] newUserPawnsForSecoundMove = deepCopyPlayerPawns(playerPawns);
                    swapBoardPawns(newBoardForSecoundMove, from_2, to_2);
                    swapPlayerPawns(newUserPawnsForSecoundMove, from_2, to_2);

                    for(int a=0; a<playerPawns.Length; a++)
                    {
                        checkAvailableMoves(newBoardForSecoundMove, newUserPawnsForSecoundMove, drugiRuchPiona, false, playerPawns[a].field, from_2, to_2);
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
        int x = new Random().Next(playerMoves.Count);
        Console.WriteLine(x);
        Move wykonaj = playerMoves[x];

        Console.WriteLine(wykonaj);
        if(wykonaj.to_2 == -1)
        {
            //Console.WriteLine("Krótki ruch");
            Pawn a = mainBoard[wykonaj.to_1];
            mainBoard[wykonaj.to_1] = mainBoard[wykonaj.from_1];
            mainBoard[wykonaj.from_1] = a;
            showPawns();
        }
        else
        {
            //Console.WriteLine("Długi ruch");
            Pawn pion = mainBoard[wykonaj.to_1];
            mainBoard[wykonaj.to_1] = mainBoard[wykonaj.from_1];
            mainBoard[wykonaj.from_1] = pion;



            pion = mainBoard[wykonaj.to_2];
            mainBoard[wykonaj.to_2] = mainBoard[wykonaj.form_2];
            mainBoard[wykonaj.form_2] = pion;


            if (wykonaj.to_1 > 41) gameEnd = true;
            if (wykonaj.to_2 > 41) gameEnd = true;


            for (int a = 0; a < playerPawns.Length; a++)
            {
                if (playerPawns[a].x == wykonaj.from_1 / 7 && playerPawns[a].y == wykonaj.from_1 % 7)
                {
                    playerPawns[a] = new Pawn(wykonaj.to_1, playerPawns[a].pawnSymbol);
                }
                if (playerPawns[a].x == wykonaj.form_2 / 7 && playerPawns[a].y == wykonaj.form_2 % 7)
                {
                    playerPawns[a] = new Pawn(wykonaj.to_2, playerPawns[a].pawnSymbol);
                }
            }
            showPawns();
        }
        Console.Write("Ruchy:" + playerMoves.Count + ",   //");
        playerMoves.Clear();
    }
    public void swapBoardPawns(Pawn[] board, int from, int to)
    {
        Pawn q = board[to];
        board[to] = board[from];
        board[from] = q;
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

class Move
{
    /// <summary>
    /// W związku z tym, że ruchy są 2, tym samym pionkiem albo dwoma i kilka wczesniejszych koncepcji zawiodło,
    /// robię ruch podwójny, z_1, do_1 odpowiadają za 1 ruch, i z_2, do_2 za 2 lub bez ruchu, wtedy są -1
    /// </summary>
    public int 
        from_1, 
        to_1, 
        form_2, 
        to_2;
    public Move(int i, int y)
    {
        this.from_1 = i;
        this.to_1 = y;
        this.form_2 = -1;
        this.to_2 = -1;
    }
    public Move(int i, int y, int j, int h)
    {
        this.from_1 = i;
        this.to_1 = y;
        this.form_2 = j;
        this.to_2 = h;
    }

    public override string ToString()
    {
        return base.ToString() + " " + from_1 + ":" + to_1 + ", " + form_2 + ":" + to_2;
    }
}