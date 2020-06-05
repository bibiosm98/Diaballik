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
    public Pionek[]
        mainBoard = new Pionek[49];
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
            gameEnd = Player_1.ruch();
            //Console.Clear();
            //showBoard();
            //System.Threading.Thread.Sleep(200);
            if (gameEnd) break;
            reverseBoard();
            gameEnd = Player_2.ruch();
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
            mainBoard[i] = new Pionek(i, i%7, '-');
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
    public Pionek[] pionkiGracza = new Pionek[7];
    public Pionek[] 
        board,
        plansza2;
    public List<dynamic> 
        playerMoves = new List<dynamic>(), 
        pierwszyRuchPiona = new List<dynamic>(), // pierwszy i 2 ruch można zrobić na zwyklej tablicy,   4 dla 1 i 16 dla drugiegi ruchu, po odjeciu powtorzen będzie chyba 12 ruchów + 4 'krótkie'
        drugiRuchPiona = new List<dynamic>();
    bool koniec = false;
    public Gracz(string name, Pionek[] p, char symbol)
    {
        this.playerSymbol = symbol;
        this.playerName = name;
        this.board = p;
        this.plansza2 = p;

        for(int i=0; i<7; i++)
        {
            board[i] = pionkiGracza[i] = new Pionek(0, i, playerSymbol);
        }
        board[3] = pionkiGracza[3] = new Pionek(0, 3, Char.ToUpper(playerSymbol));
        
    }
    public int iloscRuchow()
    {
        return playerMoves.Count;
    }
    public bool ruch()
    {
        dostepneRuchy();
        //najlepszyRuch();
        wykonajRuch();
        return koniec;
    }
    private void dostepneRuchy()
    {
        int pole = 0;
        for(int i = 0; i < pionkiGracza.Length; i++){
            pole = pionkiGracza[i].x * 7 + pionkiGracza[i].y;
            sprawdźPierwszyRuch(pole, 0, pierwszyRuchPiona, board, true, 0);
        }

        playerMoves.AddRange(drugiRuchPiona);
        pierwszyRuchPiona.Clear();
        drugiRuchPiona.Clear();

        //Console.WriteLine("Ilość dostepnych ruchów = " + ruchyGracza.Count);
    }
    private void sprawdźPierwszyRuch(int i, int j, List<dynamic> lista, Pionek[] plansza3, bool ruch1, int pole11)
    {
        //Console.WriteLine("Ruch: " + ((ruch1) ? "pierwszy" : "drugi"));
        // sprawdzenie ruchy piona o jedno pole w 4 strony, trzeba sprawdzić czy nie wyjdzie poza plansze
        //pole moge na pętli zrobić z tablicą [+7, -7, +1, -1]

        int[] ruchPiona = { 7, -7, 1, -1 }; // kierunek w którym można ruszyć pionem
        int pole;  // pole - miejsce w które pionek może się przesuąć, i- miejsce na którym jest
        bool dodatkowy = true;
        for (int kierunek = 0; kierunek<ruchPiona.Length; kierunek++)
        {
            dodatkowy = true;
            //Console.Write(", " + kierunek + ": ");
            pole = i + ruchPiona[kierunek];
            //tutaj 2 warunki wychodzenia poza plansze( aby nie przeskoczyć przy polu +1/-1 i całą szerkość planszy i jeden rząd wyzej/niżej 
            if(ruchPiona[kierunek] == 1 && i % 7 == 6) dodatkowy = false;
            
            if(ruchPiona[kierunek] == -1 && i % 7 == 0) dodatkowy = false;
            
            if (pole >= 0 && pole < 49 && dodatkowy)
            {
                if (plansza3[pole].pawnSymbol == '-')
                {
                    //Console.WriteLine("Z = " + i + "DO " + pole);
                    if (ruch1)
                    {
                        // chwilowo wyłączone krótkie ruchy
                        //lista.Add(new Ruch(i, pole));
                        Pionek[] nowaPlansza = new Pionek[49];
                        for (int a = 0; a < plansza3.Length; a++)
                        {
                            nowaPlansza[a] = new Pionek(plansza3[a].x, plansza3[a].y, plansza3[a].pawnSymbol);
                        }

                        Pionek q = nowaPlansza[pole];
                        nowaPlansza[pole] = nowaPlansza[i];
                        nowaPlansza[i] = q;

                        Pionek nieRuszony = null;
                        Pionek ruszony = null;
                        //Console.WriteLine("Pionki gracza: ");
                        for (int a=0; a<pionkiGracza.Length; a++)
                        {
                            //Console.Write(pionkiGracza[a].x + " : " + pionkiGracza[a].y + ",     ");
                            if (pionkiGracza[a].x == i / 7 && pionkiGracza[a].y == i % 7)
                            {
                                //Console.Write(" | ");
                                nieRuszony = new Pionek(pionkiGracza[a].x, pionkiGracza[a].y, pionkiGracza[a].pawnSymbol);
                                ruszony = pionkiGracza[a];
                            }
                        }

                        if (ruszony!=null) ruszony.ruszPionek(pole);
                        //pokazplansze(nowaPlansza);

                        for(int a=0; a<pionkiGracza.Length; a++)
                        {
                            int pole2 = pionkiGracza[a].x * 7 + pionkiGracza[a].y;
                            //sprawdźPierwszyRuch(i, pole, drugiRuchPiona, nowaPlansza, false);
                            sprawdźPierwszyRuch(pole2, i, drugiRuchPiona, nowaPlansza, false, pole);
                        }
                        ruszony.ruszPionek(nieRuszony.x, nieRuszony.y);
                    }
                    else
                    {
                        lista.Add(new Ruch(j, pole11, i, pole)); // ruch(z, do, z2, do2)
                    }
                }
            }
        }
    }
    public void wykonajRuch()
    {
        int x = new Random().Next(playerMoves.Count);
        Console.WriteLine(x);
        Ruch wykonaj = playerMoves[x];

        Console.WriteLine(wykonaj);
        pokażPionki();
        if(wykonaj.do_2 == -1)
        {
            //Console.WriteLine("Krótki ruch");
            Pionek a = board[wykonaj.do_1];
            board[wykonaj.do_1] = board[wykonaj.z_1];
            board[wykonaj.z_1] = a;
        }
        else
        {
            //Console.WriteLine("Długi ruch");
            Pionek pion = board[wykonaj.do_1];
            board[wykonaj.do_1] = board[wykonaj.z_1];
            board[wykonaj.z_1] = pion;



            pion = board[wykonaj.do_2];
            board[wykonaj.do_2] = board[wykonaj.z_2];
            board[wykonaj.z_2] = pion;


            if (wykonaj.do_1 > 41) koniec = true;
            if (wykonaj.do_2 > 41) koniec = true;

            for (int a = 0; a < pionkiGracza.Length; a++)
            {
                if (pionkiGracza[a].x == wykonaj.z_1 / 7 && pionkiGracza[a].y == wykonaj.z_1 % 7)
                {
                    pionkiGracza[a] = new Pionek(wykonaj.do_1, pionkiGracza[a].pawnSymbol);
                }
                if (pionkiGracza[a].x == wykonaj.z_2 / 7 && pionkiGracza[a].y == wykonaj.z_2 % 7)
                {
                    pionkiGracza[a] = new Pionek(wykonaj.do_2, pionkiGracza[a].pawnSymbol);
                }
            }
        }
        Console.Write("Ruchy:" + playerMoves.Count + ",   //");
        playerMoves.Clear();
    }
    public void pokażPionki()
    {
        Console.WriteLine("Pionki, gracza");
        for (int i=0; i<7; i++)
        {
            //Console.Write(pionkiGracza[i].x + ":" + pionkiGracza[i].y + ", ");
        }
    }
    private void pokazplansze()
    {
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
    private void pokazplansze(Pionek[] plansza4)
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

class Pionek
{
    public int pole = -1;
    public char pawnSymbol; 
    public int 
        x = -1, 
        y = -1;
    //bool piłka = false; // CO do XD
    //Gracz gracz = null;
    public Pionek(int a, int b, char c)
    {
        this.pole = a * 7 + b;
        this.x = a;
        this.y = b;
        this.pawnSymbol = c;
    }
    public Pionek(Pionek a)
    {
        this.pole = a.pole;
        this.x = a.x;
        this.y = a.y;
        this.pawnSymbol = a.pawnSymbol;
    }
    public Pionek(int pole, char token)
    {
        this.pawnSymbol = token;
        this.pole = pole;
        this.x = pole / 7;
        this.y = pole % 7;
    }
    public void ruszPionek(int a, int b)
    {
        this.pole = a * 7 + b;
        this.x = a;
        this.y = b;
    }
    public void ruszPionek(int pole)
    {
        this.pole = pole;
        this.x = pole/7;
        this.y = pole%7;
    }
}

class Pole
{
    public int x = -1, y = -1;
    public Pole()
    {

    }
    public Pole(int a, int b)
    {
        this.x = a;
        this.x = b;
    }
}

class Ruch
{
    /// <summary>
    /// W związku z tym, że ruchy są 2, tym samym pionkiem albo dwoma i kilka wczesniejszych koncepcji zawiodło,
    /// robię ruch podwójny, z_1, do_1 odpowiadają za 1 ruch, i z_2, do_2 za 2 lub bez ruchu, wtedy są -1
    /// </summary>
    public int 
        z_1, 
        do_1, 
        z_2, 
        do_2;
    public Ruch(int i, int y)
    {
        this.z_1 = i;
        this.do_1 = y;
        this.z_2 = -1;
        this.do_2 = -1;
    }
    public Ruch(int i, int y, int j, int h)
    {
        this.z_1 = i;
        this.do_1 = y;
        this.z_2 = j;
        this.do_2 = h;
    }

    public override string ToString()
    {
        return base.ToString() + " " + z_1 + ":" + do_1 + ", " + z_2 + ":" + do_2;
    }
}