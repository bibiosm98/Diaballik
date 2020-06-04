﻿using Amazon.MissingTypes;
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
            Gra Diaballik = new Gra();
        }
    }
}

class Gra
{
    public Pionek[] 
        plansza = new Pionek[49], 
        odwróconaPlansza = new Pionek[49];
    public Gracz 
        gracz1, 
        gracz2;
    bool koniecGry = false;
    public Gra()
    {
        Console.WriteLine("Nowa Gra");
        inicjujplansze();
        pokazplansze();

        gracz1 = new Gracz("Gracz_1", plansza, 'x');
        odwrocPlansze();
        gracz2 = new Gracz("Gracz_2", plansza, 'y');

        pokazplansze();

        while (!koniecGry)
        {
            odwrocPlansze();
            koniecGry = gracz1.ruch();
            //if(!koniecGry)break;
            //odwrocPlansze();
            //koniecGry = gracz2.ruch();
            break;
        }
        Console.WriteLine("KONIEC");
    }
    private void inicjujplansze()
    {
        Console.WriteLine("Inicjuj plansze");
        for (int i = 0; i < plansza.Length; i++)
        {
            plansza[i] = new Pionek(i, i%7, '-');
        }
    }
    private void pokazplansze()
    {
        Console.WriteLine("Pokaż plansze");
        for(int i = plansza.Length-1; i >= 0; i--)
        {
            Console.Write(plansza[i].znak + "  ");
            if (i % 7 == 0)
            {
                Console.WriteLine("");
            }
        }

        Console.WriteLine("");
        Console.WriteLine("");
        Console.WriteLine("");
    }

    public void ruchGracza1()
    {
        
    }
    public void ruchGracza2()
    {

    }
    private void odwrocPlansze()
    {
        Array.Reverse(plansza);
    }
}


// JAK WRÓCE, zrobić tablice, na niej 4 pole do sprawdzania ruchu, 
// zredukuje to 4 ify do 1
// i zrobić po 1 ruchu znowu pętle na 7 pionow dla 2 ruchu
class Gracz
{
    public char znak;
    string nazwa = "";
    public Pionek[] pionkiGracza = new Pionek[7];
    public Pionek[] 
        plansza,
        plansza2;
    public List<dynamic> 
        ruchyGracza = new List<dynamic>(), 
        pierwszyRuchPiona = new List<dynamic>(), // pierwszy i 2 ruch można zrobić na zwyklej tablicy,   4 dla 1 i 16 dla drugiegi ruchu, po odjeciu powtorzen będzie chyba 12 ruchów + 4 'krótkie'
        drugiRuchPiona = new List<dynamic>();

    public Gracz(string naz, Pionek[] p, char znak)
    {
        this.znak = znak;
        this.nazwa = naz;
        this.plansza = p;
        this.plansza2 = p;

        for(int i=0; i<7; i++)
        {
            plansza[i] = pionkiGracza[i] = new Pionek(0, i, znak);
        }
        plansza[3] = pionkiGracza[3] = new Pionek(0, 3, Char.ToUpper(znak));
        
    }

    public bool ruch()
    {
        dostepneRuchy();
        //najlepszyRuch();
        //Wykonajruch();
        return false;
    }
    private void dostepneRuchy()
    {
        for(int i = 0; i < pionkiGracza.Length; i++){
            sprawdźPierwszyRuch(i, pierwszyRuchPiona, plansza, true);

        }

        Console.WriteLine("Ilośc ruchów 1 = " + pierwszyRuchPiona.Count);
        Console.WriteLine("Ilośc ruchów 2 = " + drugiRuchPiona.Count);
        pokazplansze(plansza);
        //pokazplansze();
        Console.WriteLine("Ilość dostepnych ruchów = " + ruchyGracza.Count);
    }
    private void sprawdźPierwszyRuch(int i, List<dynamic> lista, Pionek[] plansza3, bool ruch1)
    {
        //zmiana planszy dla 2 ruchu
        //char znakk = plansza3[Z].znak;
        //if (tura2)
        //{
        //    plansza3[Z].znak = plansza3[i].znak;
        //    plansza3[i].znak = znakk;
        //    pokazplansze(plansza3);
        //}
        //pokazplansze();
        // sprawdzenie ruchy piona o jedno pole w 4 strony, trzeba sprawdzić czy nie wyjdzie poza plansze
        int pole = i + 7;  // pole - miejsce w które pionek może się przesuąć, i- miejsce na którym jest
        //pole moge na pętli zrobić z tablicą [+7, -7, +1, -1]
        if (pole >= 0 && pole < 49)
        {
            if (plansza3[pole].znak == '-')
            {
                Console.WriteLine("Z = "+i+"DO " + pole);
                lista.Add(new Ruch(i, pole));
                if (ruch1) { sprawdźDrugiRuch(i, pole, drugiRuchPiona, plansza3, false); }
            }
        }
        pole = i - 7;
        if (pole >= 0 && pole < 49)
        {
            if (plansza3[pole].znak == '-')
            {
                Console.WriteLine("Z = " + i + "DO " + pole);
                lista.Add(new Ruch(i, pole));
                if (ruch1) { sprawdźDrugiRuch(i, pole, drugiRuchPiona, plansza3, false); }
            }
        }
        pole = i + 1;//mozna przesunąć i%7!=6 na lewą strone co oszczędzi obliczeń <=0 oraz <49 w przypadku false
        if (pole >= 0 && pole < 49 && i % 7 != 6) // tutaj dodatkowe zabezpieczenie, poniewaz plansza jest liczona 1-49, aby np nie przeskoczyć z pola 6 na 7(co jest szerokością planszy)
        {
            if (plansza3[pole].znak == '-')
            {
                Console.WriteLine("Z = " + i + "DO " + pole);
                lista.Add(new Ruch(i, pole));
                if (ruch1) { sprawdźDrugiRuch(i, pole, drugiRuchPiona, plansza3, false); }
            }
        }
        pole = i - 1;//jak wyzej
        if (pole >= 0 && pole < 49 && i % 7 != 0) // Jak wyżej
        {
            if (plansza3[pole].znak == '-')
            {
                Console.WriteLine("Z = " + i + "DO " + pole);
                lista.Add(new Ruch(i, pole));
                if (ruch1) { sprawdźDrugiRuch(i, pole, drugiRuchPiona, plansza3, false); }
            }
        }

        //if (tura2)
        //{
        //    plansza3[i].znak = plansza3[Z].znak;
        //    plansza3[Z].znak = znakk;
        //}
    }
    private void sprawdźDrugiRuch(int Z, int Do, List<dynamic> lista, Pionek[] plansza3, bool tura2)
    {// tak wiem, że kod się duplikuje, optymalizacja potem

        Pionek[] nowaPlansza = new Pionek[49];
        //Array.ConstrainedCopy(plansza3, 0, nowaPlansza, 0, 49);
        // chuj kurwa, jebać C#, ja chce JAVE!!!, tu sie nie da skopiowac tablicy -,-

        for (int j = 0; j < plansza3.Length; j++)
        {
            nowaPlansza[j] = new Pionek(plansza3[j].x, plansza3[j].y, plansza3[j].znak);
        }
        //zmiana planszy dla 2 ruchu
        char znakk = nowaPlansza[Z].znak;
        if (!tura2)
        {
            nowaPlansza[Z].znak = nowaPlansza[Do].znak;
            nowaPlansza[Do].znak = znakk;
            pokazplansze(nowaPlansza);
        }
        int i = Z;
        int pole = Do + 7;
        if (pole >= 0 && pole < 49)
        {
            if (nowaPlansza[pole].znak == '-')
            {
                Console.WriteLine("Z = " + i + "DO " + pole);
                lista.Add(new Ruch(Z, Do, Do, pole));
            }
        }
        pole = Do - 7;
        if (pole >= 0 && pole < 49)
        {
            if (nowaPlansza[pole].znak == '-')
            {
                Console.WriteLine("Z = " + Do + "DO " + pole);
                lista.Add(new Ruch(Z, Do, Do, pole));
            }
        }
        pole = Do + 1;
        if (pole >= 0 && pole < 49 && i % 7 != 6) 
        {
            if (nowaPlansza[pole].znak == '-')
            {
                Console.WriteLine("Z = " + Do + "DO " + pole);
                lista.Add(new Ruch(Z, Do, Do, pole));
            }
        }
        pole = Do - 1;
        if (pole >= 0 && pole < 49 && i % 7 != 0)
        {
            if (nowaPlansza[pole].znak == '-')
            {
                Console.WriteLine("Z = " + Do + "DO " + pole);
                lista.Add(new Ruch(Z, Do, Do, pole));
            }
        }

    }
    private void pokazplansze()
    {
        Console.WriteLine("\nPokaż plansze: " + nazwa);
        for (int i = plansza.Length - 1; i >= 0; i--)
        {
            Console.Write(plansza[i].znak + "  ");
            if (i % 7 == 0)
            {
                Console.WriteLine("");
            }
        }
    }
    private void pokazplansze(Pionek[] plansza4)
    {
        Console.WriteLine("\nPokaż plansze: " + nazwa);
        for (int i = plansza4.Length - 1; i >= 0; i--)
        {
            Console.Write(plansza4[i].znak + "  ");
            if (i % 7 == 0)
            {
                Console.WriteLine("");
            }
        }
    }
}

class Pionek
{
    public char znak; 
    public int 
        x = -1, 
        y = -1;
    bool piłka = false;
    Gracz gracz = null;
    public Pionek(int a, int b, char c)
    {
        this.x = a;
        this.y = b;
        this.znak = c;
    }
    public Pionek(Pionek a)
    {
        this.x = a.x;
        this.y = a.y;
        this.znak = a.znak;
    }
    public void ruszPionek(int a, int b)
    {
        this.x = a;
        this.y = b;
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
}



//for (int i = 0; i < pierwszyRuchPiona.Count; i++)
//{
//    plansza2 = new Pionek[49];
//    for (int j = 0; j < plansza.Length; j++)
//    {
//        plansza2[j] = new Pionek(plansza[j]);

//    }
//    sprawdźRuchyPiona(pierwszyRuchPiona[i].Do, pierwszyRuchPiona[i].Z, 0, drugiRuchPiona, plansza2, true);

//    Console.WriteLine("Ilośc ruchów 2 = " + drugiRuchPiona.Count);
//    ruchyGracza.Add(drugiRuchPiona);
//    drugiRuchPiona.Clear();
//}

