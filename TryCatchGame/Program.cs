using System;
using System.Collections.Generic;
using System.Text;

/* Задание на 11.03.2021
 *Работа Вержбицкого Максима TA-20V
 если считать начальную точку движения за 0,0 то в конце игры 
 добавить к статистике сколько шагов игрок прошёл вперёд и влево/вправо.

Задание 18.03.2021
Добавил интеракцию с Гангстером(если нет оружия) => то
Гангстер забирает все монеты и перемещается на любое свободное место
Если есть оружие(то убиваю гангстера)

Добавил общее колличество монеток, сколько осталось собрать!
Так же счетчик шагов
При сборе всех монеток игра завершается



 */


namespace TryCatchGame
{ /* Ходилка по пустому пространству, но с вероятностью нахождения монеток и магазина для покупок.
     * Каждый шаг увеличивает счётчик шагов, найденные монетки собираются в кошель.
     * В магазине за 1 монетку можно купить 1 предмет: одежда, еду или оружие
     ↑(w) = Forward
     ←(a) = left
     →(d) = right
     ↓(s) = down
     space = take/use
     Esc = выйти из игры
     * q - Опиши что я вижу, не реализовано
     * Legend:
     * M - money
     * T - Thief/ Гангстер
     * S - Shop
     * x - block
     * E - enemy
    */
    class Program
    {
        // Статические определения
        static int status = 20; // Номер строки статуса
        static int msgrow = 21; // номер строки сообщения
        static int warnrow = 22; // номер строки препреждения
        static int statusUp = 3;
        // Выводим сообщения в строке msgrow
        static void Message(string msg)
        {

            Warning("");
            Console.SetCursorPosition(0, msgrow);
            for (int i = 0; i < 79; i++) Console.Write(' ');
            Console.SetCursorPosition(0, msgrow);
            Console.ForegroundColor = System.ConsoleColor.Green;
            Console.Write(msg);
        }
        // Вывод предупреждения в строке warnrow
        static void Warning(string msg)
        {
            Console.SetCursorPosition(0, warnrow);
            for (int i = 0; i < 79; i++) Console.Write(' ');
            Console.SetCursorPosition(0, warnrow);
            Console.ForegroundColor = System.ConsoleColor.Red;
            Console.Write(msg);
        }
        // Вывод статуса в строке status 
        static void Status(int score, int eat, int dress, int weapon, int health)
        {
            // стереть строку на экране в позиции вывода
            Console.SetCursorPosition(0, status); for (int i = 0; i < 79; i++) Console.Write(' ');
            Console.SetCursorPosition(0, status);
            Console.ForegroundColor = System.ConsoleColor.White;
            Console.Write($"| Score:{score} |");
            Console.ForegroundColor = System.ConsoleColor.Yellow;
            Console.Write($" Food:{eat} |");
            Console.ForegroundColor = System.ConsoleColor.Cyan;
            Console.Write($" Armory:{dress} |");
            Console.ForegroundColor = System.ConsoleColor.DarkGray;
            Console.Write($" Weaponary:{weapon} |");
            Console.ForegroundColor = System.ConsoleColor.Green;
            Console.Write($" Health:{health} |");

        }
        static void StatusUp(int eCount, int steps, int coinsLeft) 
        {
            Console.SetCursorPosition(0, statusUp); for (int i = 0; i < 79; i++) Console.Write(' ');
            Console.SetCursorPosition(0, statusUp);
            Console.ForegroundColor = System.ConsoleColor.Red;
            Console.Write($"| Enemys:{eCount} |");
            Console.ForegroundColor = System.ConsoleColor.DarkYellow;  // аналогично добавил темно желтый цвет 
            Console.Write($" Total Steps: {steps} |"); // счетчик шагов в общем
            Console.ForegroundColor = System.ConsoleColor.DarkMagenta; // тут добавил  темно розовый цвет
            Console.Write($" Coins Left:{coinsLeft} |"); // для сколько монет осталось всего
        }
        // Вывод раскашенного символа на цветном заднике, задник по умолчанию черный
        static void ColorCharacter(char s, ConsoleColor c, ConsoleColor b = System.ConsoleColor.Black)
        {
            Console.ForegroundColor = c;
            Console.BackgroundColor = b;
            Console.Write(s);
            Console.BackgroundColor = System.ConsoleColor.Black;
        }
        static void Main(string[] args)
        {
            // Определим тип кодировки для ввода и вывода через консоль
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            Random rnd = new Random(); // включаем генератор случайных чисел
            ConsoleKeyInfo action;  // обьект нажатой клавиши(действия)
            // Макет карты игрового поля
            // массив строк !! содержимое нельзя поменять
            string[] premap = {
                    "░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░ S ░░░░░░░░░░░░░░░░░░░░░░░░░░░",
                    "░       ░          ET       ░    ░  ░ ░    ░       ░  ET   ░",
                    "░░ ░ ░░  ░░  ░ ░░  ░░  ░ ░░  ░ ░   ░  ░ ░░  ░░  ░ ░░  ░░   ░",
                    "░  ░       T░░     MS  ░       T░░    ░       T░░     MS  ░░",
                    "░░ ░░  ░S░░░ ░░  ░S░░░ ░░  ░S░░░ ░░   ░░  ░S░░░ ░░  ░S░░░ ░░",
                    "░   ░  ░  E   ░  M  S      ░  E   ░    ░  ░  E   ░  M  S  T░",
                    "░ ░ ░  M░░  ░ ░   ░░  ░ ░   ░░  ░ ░  ░ ░   ░░  ░ ░   ░░  ░ ░",
                    "░   ░░  ░░  ░ E  ░░░M       ░░░ ░ E    ░░  ░░░ ░ E  ░░░M  ░░",
                    "░   ░░    M░░     S ░   ░░     ░░      ░░     ░░     S ░   ░",
                    "░  ░  M░   T░░   ░  S      ░   T░░    ░   ░   T░░   ░  S  ░░",
                    "░░ ░░  ░S░░░ ░░ E ░░░░ ░░  ░S░░░ ░░ E ░░  ░S░░░ ░░ E ░░░░  ░",
                    "░     ░   M   ░  T  E   ░     E   ░      ░   M   ░  T  E  M░",
                    "░░S ░    ░░░░    ░░░░░ M ░ ░░   ░    S ░    ░░░░    ░░░░░░░░",
                    "░ ░  M     T░░     MM  ░       T░░    ░       T░░     MS  ░░",
                    "░   ░  ░S░░░ ░░  ░S░░░ ░░  ░S░░░ ░░   ░░  ░S░░░ ░░  ░S░░░ ░░",
                    " M M░  ░  E      M  S      ░  E   ░    ░  ░  E   ░  M  S  T░"
            };
            int ymax = premap.Length; // вычисляем размер макета по высоте 
            int xmax = premap[0].Length; // и по ширине 
            int y = ymax - 1; int x = 0; //определяем начальную позицию игрока
            int mCount = 0;         // переменная для счетчика монет
            int eCount = 0;
            int health = 200;
            // Создаем 2-х мерный массив символов нашей карты с возможностью удаления элементов на карте
            // с размерами макета
            char[,] map = new char[ymax, xmax];

            // Копируем из макета карты содержимое на карту посимвольно
            for (int i = 0; i < ymax; i++)
                for (int j = 0; j < xmax; j++)
                {
                    map[i, j] = premap[i][j];
                    if (map[i, j] == 'M') {
                        mCount++;
                    }
                    if (map[i, j] == 'T')
                    {
                        eCount++;
                    }
                    if (map[i, j] == 'E')
                    {
                        eCount++;
                    }
                }
           


            int map_offset = 4; // Смещение поля игры по вертикали
            int weapon_price = 4; // цена в монетах за единицу оружия
            int dress_price = 2; // цена в монетах за одежду
            int steps = 0;  // счётчик шагов
            int kills = 0;  // убийства гангстера или врага(нереализовано!)
            //Шаги вперёд-назад-лево-право
            int forward_step = 0;
            int left_step = 0;
            int right_step = 0;
            int backwards_step = 0;


            int score = 0;  // кошелёк для монеток

            int dress = 0;  // количество купленной одежды
            int food = 0;   // количество купленной еды
            int weapon = 0; // количество купленного оружия

            bool noway; // флажок "ход невозможен"
            Console.CursorVisible = false; // Убрать курсор

            Console.WriteLine("Бесконечная пошаговая бродилка с поиском монеток и покупкой вещей. Клавишы:\n" +
                    "↑ вперёд, ← влево, → вправо, ↓ назад, пробел взять/использовать, " +
                    "Esc выход ");
            //for (action = Console.ReadKey(true); action.Key != ConsoleKey.Escape;)

            /*for(int i = 0; i < 10; i++)
            {
                Console.WriteLine(premap[i]);
            }*/
            // считываем с клавиатуры символ очередного хода без отображения символа на экране
            // проверяем символ и итерируем до тех пор пока не нажата клавиша Escape
            while ((action = Console.ReadKey(true)).Key != ConsoleKey.Escape || mCount != 0 || health > 0) // игра продолжается в случае, если не нажата клавиша Escape и оставшихся монет не равно нулю
            {
                noway = false;
                switch (action.Key)
                {
                    case ConsoleKey.UpArrow: Message($"Forward {steps}");
                        if (y > 0)
                        {
                            if (map[y - 1, x] != '░') { y = y - 1; forward_step++; steps++;health--; }
                            else noway = true;
                        }
                        else noway = true;
                        break;
                    case ConsoleKey.DownArrow: Message($"Backwards {steps}");
                        if (y + 1 < ymax)
                        {
                            if (map[y + 1, x] != '░') { y = y + 1; backwards_step++; steps++; health--; }
                            else noway = true;
                        }
                        else noway = true;
                        break;
                    case ConsoleKey.LeftArrow: Message($"Left {steps}");
                        if (x > 0)
                        {
                            if (map[y, x - 1] != '░') { x = x - 1; left_step++; steps++; health--; }
                            else noway = true;
                        }
                        else noway = true;
                        break;
                    case ConsoleKey.RightArrow: Message($"Right {steps}");
                        if (x + 1 < xmax)
                        {
                            if (map[y, x + 1] != '░') { x = x + 1; right_step++; steps++; health--; }
                            else noway = true;
                        }
                        else noway = true;
                        break;
                    default:
                        noway = true;
                        break;
                }

                if(noway)
                {
                    Warning("You have hit the wall"); noway = false;
                    Console.Beep();
                }
               
                for (int i = 0; i < ymax; i++)
                {
                    Console.SetCursorPosition(0, i + map_offset);
                    Console.ForegroundColor = System.ConsoleColor.Cyan;
                    for( int j = 0; j < xmax; j++)
                    {
                        switch(map[i, j])
                        {
                            case '░': ColorCharacter('█', System.ConsoleColor.Gray); break;
                            case 'T': ColorCharacter(map[i, j], System.ConsoleColor.Blue); break; // цвет покраски для Гангстера
                            case 'S': ColorCharacter(map[i, j], System.ConsoleColor.Green); break;
                            case 'E': ColorCharacter(map[i, j], System.ConsoleColor.Red); break;
                            case 'M': ColorCharacter(map[i, j], System.ConsoleColor.Yellow); break;
                            case 'R': // этот кейс равен сбежавшему Гангстеру
                                {
                                    map[i, j] = 'T'; // у которого свой цвет темно розовый, пока я не сделал шаг
                                    ColorCharacter(map[i, j], System.ConsoleColor.DarkMagenta); break;
                                }
                            case 'V':
                                {
                                    map[i, j] = 'E';
                                    ColorCharacter(map[i, j], System.ConsoleColor.Magenta); break;
                                }
                            default: Console.Write(map[i, j]); break;
                        }
                    }
                }
                Status(score, food, dress, weapon, health);
                Console.SetCursorPosition(x, y + map_offset);

                StatusUp(eCount, steps, mCount);
                Console.SetCursorPosition(x, y + map_offset);


                
                ColorCharacter('☻', System.ConsoleColor.Cyan); // показываем лицо на чорном фоне

                if (map[y, x] == 'M') // если найдена монета, вы можете её забрать, нажав пробел
                {
                    Console.SetCursorPosition(x, y + map_offset);
                    ColorCharacter('☻', System.ConsoleColor.Cyan, System.ConsoleColor.Yellow); // лицо на жёлтом фоне(на монетке)

                    Message(" Ураа! Вы можете забрать монетку!!! нажмите пробел");
                    action = Console.ReadKey(true); // вводим клавишу
                    if (action.Key == ConsoleKey.Spacebar) // если нажат пробел
                    {
                        Console.SetCursorPosition(x, y + map_offset);
                        ColorCharacter('☻', System.ConsoleColor.Cyan); // монетку забрали, показываем лицо на чорном фоне

                        Message("Взяли монетку");
                        map[y, x] = ' '; // удаляем эту монетку с карты
                        score++; // монеток на одну больше
                        mCount--;
                    }
                    else
                    {
                        Message("Не взяли монетку");
                        continue; // переходим к следующему циклу
                    }
                }
                else if (map[y, x] == 'T') // если гангстер, он забирает все монетки и убегает
                {

                    Console.SetCursorPosition(x, y + map_offset);
                    ColorCharacter('☻', System.ConsoleColor.Cyan, System.ConsoleColor.Blue); // лицо на синем фоне(на гангстере)

                    // звуковой SOS
                    Console.Beep(); Console.Beep(); Console.Beep();
                    System.Threading.Thread.Sleep(500);
                    Console.Beep(800, 500); Console.Beep(800, 500); Console.Beep(800, 500);
                    Console.Beep(); Console.Beep(); Console.Beep();

                    Console.SetCursorPosition(x, y + map_offset);
                    ColorCharacter('☻', System.ConsoleColor.Cyan); // Гангстер убежал, показываем лицо на чорном фоне
                    map[y, x] = ' '; // удаляем гангстера с карты
                    if (weapon > 0)  // если куплено оружие больше нуля
                    {
                        Message(" Ой! Ты прогнал Гангстера!!!");
                        kills++; // инкрементируем убийство 
                        weapon--; // при убийстве заберается оружие
                        eCount--;
                    }
                    else {
                        Message(" Ой! Гангстер забрал монетки и убежал!!!");
                        score = 0; // монеток нет
                    }
                  
                    while (true) { // тут вся изюминка для перемещения врага влучае(если у меня не было оружия)
                        int newYPos = rnd.Next(0, ymax); // создаю локальную переменную внутри while по вертикали
                        int newXPos = rnd.Next(0, xmax); // создаю локальную переменную внутри while по горизонтали
                        Char item = map[newYPos, newXPos]; // смотрим что находится на потенциальном месте гангстера обзовем item
                        bool foundPlace = false; // переменная найдено место по default false
                        switch(item){           // пройдемся по switch     
                        case ' ':               // если пустое место
                           map[newYPos, newXPos] = 'R'; // то присвоим пустому месту char 'R'
                           foundPlace = true;  // тут переменная при находке пустого места становится True
                                break;         // выходим из цикла
                        }
                        if (foundPlace) // тут переменная при находке пустого места становится True
                            break;      // выходим из цикла
                    }
                    steps++; // шагов на один больше
                    action = Console.ReadKey(true); // вводим клавишу
                }
                else if (map[y, x] == 'E') // Если я наткнулся на врага
                {
                    Console.SetCursorPosition(x, y + map_offset);
                    ColorCharacter('☻', System.ConsoleColor.Cyan, System.ConsoleColor.Red); // лицо на красном фоне(на гангстере)
                    // звуковой SOS
                    Console.Beep(); Console.Beep(); Console.Beep();
                    System.Threading.Thread.Sleep(500);
                    //Console.Beep(800, 500); Console.Beep(800, 500); Console.Beep(800, 500);
                    Console.Beep(); Console.Beep(); Console.Beep();
                    Console.SetCursorPosition(x, y + map_offset);
                    ColorCharacter('☻', System.ConsoleColor.Cyan); // Враг убежал, показываем лицо на чорном фоне
                    map[y, x] = ' '; // удаляем врага с карты
                    if (weapon > 0) // удаляем гангстера с карты
                    {
                        Message("Да ты опасный!!Ты прогнал врага!!!");
                        kills++; // инкрементируем убийство 
                        weapon--; // при убийстве заберается оружие
                        eCount--;
                    }
                    else 
                    {
                        Message(" Ой! Враг забрал монетки(ты спрятал что-то в носок)");
                        score = 0;
                    }
                    while(true)
                    {
                        int newYPos = rnd.Next(0, ymax);
                        int newXPos = rnd.Next(0, xmax);
                        Char item = map[newYPos, newXPos];
                        bool foundPlace = false;
                        switch (item)
                        {
                            case ' ':
                                map[newYPos, newXPos] = 'V';
                                foundPlace = true;
                                break;
                        }
                        if (foundPlace)
                            break;
                    }
                }
                else if (map[y, x] == 'S') // если найден магазин, то можно за монету купить вещь
                {
                    Message($"Пришли в магазин. у вас {score} монеток !!! ");
                    if (score > 0) // если кошелёк не пуст
                    {
                        Console.SetCursorPosition(x, y + map_offset);
                        ColorCharacter('☻', System.ConsoleColor.Black, System.ConsoleColor.Green);
                        Message($"Можете купить нажав 1-одежда, 2-еда, 3-оружие");
                        action = Console.ReadKey(true); // считываем символ определяющий покупку без отображения символа на экране
                        switch (action.KeyChar)
                        {
                            case '1':
                                if (score >= dress_price)
                                {
                                    Message("Купили одежду"); score -= dress_price; dress++;
                                }
                                else Warning("К сожалению у вас не хватает монеток!");
                                break;
                            case '2': Message("Купили еду"); score--; food++; break;
                            case '3':
                                if (score >= weapon_price)
                                {
                                    Message("Купили оружие"); score -= weapon_price; weapon++;
                                }
                                else Warning("К сожалению у вас не хватает монеток!");
                                break;
                            default: Warning("Нет такого предмета!"); break;
                        }
                        Message($"После покупки у вас осталось монеток {score}");
                    }  // если же кошелёк пуст, предупредить
                    else Warning("К сожалениюу у вас нет монеток!");
                }
                

            }

            if(mCount == 0)
                Console.WriteLine($"Congrats - you won");
            else
                Console.WriteLine($"{mCount} coins left on map");

            Console.WriteLine($"You made {steps} steps " + $"\n from them forward: {forward_step}, backwards: {backwards_step}, left: {left_step}, right: {right_step} \n  + and you made {score} scores");
            Console.WriteLine($"\nYou ate food {food} pcs, managed to get clothes {dress} pcs and weaponary {weapon} pcs\n");
            Console.WriteLine("Press any key(on your keyboard)!");
        }

    }
}
