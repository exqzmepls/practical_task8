using System;

namespace practical_task8
{
    public class Program
    {
        // Вывод меню
        static void PrintMenu(string[] menuItems, int choice, string info)
        {
            Console.Clear();
            Console.WriteLine(info);
            for (int i = 0; i < menuItems.Length; i++)
            {
                if (i == choice) Console.BackgroundColor = ConsoleColor.Blue;
                Console.WriteLine($"{i + 1}. {menuItems[i]}");
                Console.BackgroundColor = ConsoleColor.Black;
            }
        }

        // Выбор пункта из меню
        static int MenuChoice(string[] menuItems, string info = "")
        {
            Console.CursorVisible = false;
            int choice = 0;
            while (true)
            {
                PrintMenu(menuItems, choice, info);
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        if (choice == 0) choice = menuItems.Length;
                        choice--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (choice == menuItems.Length - 1) choice = -1;
                        choice++;
                        break;
                    case ConsoleKey.Enter:
                        Console.CursorVisible = true;
                        return choice;
                }
            }
        }

        // Ввод целого числа
        public static int IntInput(int lBound = int.MinValue, int uBound = int.MaxValue, string info = "")
        {
            bool exit;
            int result;
            Console.Write(info);
            do
            {
                exit = int.TryParse(Console.ReadLine(), out result);
                if (!exit) Console.Write("Введено нецелое число! Повторите ввод: ");
                else if (result <= lBound || result >= uBound)
                {
                    exit = false;
                    Console.Write("Введено недопустимое значение! Повторите ввод: ");
                }
            } while (!exit);
            return result;
        }

        // Проверка ввода матрицы
        public static bool CheckMatrixInput(string[] lines, out bool[,] matrix)
        {
            int n = lines.Length;
            matrix = new bool[n, n];

            for (int i = 0; i < n; i++)
            {
                string[] coll = lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (coll.Length != n) return false;
                for (int j = 0; j < n; j++)
                {
                    if (!int.TryParse(coll[j], out int result)) return false;
                    // Только 0 или 1
                    if (result < 0 || result > 1) return false;
                    matrix[i, j] = result == 1;
                    // Элементы, симментричные относительно главной диагонали должны быть равны
                    if (i > j && matrix[i, j] != matrix[j, i]) return false;
                }
            }
            return true;
        }

        // Обход в глубину
        static void DeepPass(bool[,] matrix, int node, bool[] passed)
        {
            passed[node] = true;
            for (int i = 0; i < matrix.GetLength(1); i++) if (matrix[node, i] && !passed[i]) DeepPass(matrix, i, passed);
        }

        // Проверка является ли граф полуэйлеровым
        // Из графа исключаются все петли
        public static bool IsSemiEuler(bool[,] matrix)
        {
            bool deepPassed = false;
            bool[] passed = new bool[matrix.GetLength(0)];
            int odd = 0;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                // Удаляем петлю, если есть
                matrix[i, i] = false;
                int pow = Pow(matrix, i);
                if (pow > 0 && !passed[i])
                {
                    // Если вершина имеет степень > 0 и 1 компонента связности уже есть, то граф не является связным
                    if (deepPassed) return false;
                    
                    DeepPass(matrix, i, passed);
                    deepPassed = true;
                }
                
                odd += pow % 2;
            }
            // У полуэйлерова графа только 2 вершины имеют нечётную степень или все вершины с четными степенями
            return odd == 2;
        }

        // Степень вершины графа
        static int Pow(bool[,] matrix, int row)
        {
            int result = 0;
            for (int col = 0; col < matrix.GetLength(1); col++) if (matrix[row, col]) result++;
            return result;
        }

        // Поиск эйлерой цепи в полуэйлеровом графе
        // Передавать в функцию следует только полуэйлеров граф без петель
        public static void FindEulerPath(bool[,] matrix)
        {
            // Поиск первой вершины с нечетной степенью
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                if (Pow(matrix, i) % 2 == 1) 
                {
                    // Проход по рёбрам
                    Action(matrix, i);
                    break;
                } 
            } 
        }

        // Рекурсивная функция для прохода по всем рёбрам
        static void Action(bool[,] matrix, int point)
        {
            for (int j = 0; j < matrix.GetLength(1); j++) if (matrix[point, j])
                {
                    matrix[point, j] = matrix[j, point] = false;
                    Action(matrix, j);
                }
            Console.Write(point + 1 + " ");
        }

        static void Main(string[] args)
        {
            // Пункты меню
            string[] MENU_ITEMS = { "Задать матрицу смежности графа", "Выйти из программы" };

            // Индекс пункта - выход из программы
            const int EXIT_CHOICE = 1;

            // Индекс пункта меню, который выбрал пользователь
            int userChoice;

            while (true)
            {
                // Пользователь выбирает действие (выйти или задать матрицу)
                userChoice = MenuChoice(MENU_ITEMS, "Программа для нахождения эйлерой цепи в полуэйлеровом графе\nВыберите действие:");
                if (userChoice == EXIT_CHOICE) break;
                Console.Clear();
                
                // Ввод порядка
                int n = IntInput(lBound: 0, info: "Введите количество вершин в графе: ");

                // Ввод матрицы смежности
                Console.WriteLine($"Введите матрицу смежности для графа из {n} вершин");
                Console.WriteLine("Ввод каждой строки матрицы начинайте с новой строчки,\nразделяя элементы по столбцам пробелами");
                string[] input = new string[n];
                for (int i = 0; i < n; i++) input[i] = Console.ReadLine();

                // Проверка матрицы
                if (CheckMatrixInput(input, out bool[,] matrix)) 
                {
                    // Проверка на то, является ли граф полуэйлеровым
                    if (IsSemiEuler(matrix))
                    {
                        // Поиск эйлеровой цепи
                        Console.WriteLine("Эйлерова цепь:");
                        FindEulerPath(matrix);
                    }
                    // Эйлеровой цепи нет, тк граф не является полуэйлеровым
                    else Console.WriteLine("Эйлеровой цепи нет, т.к. заданный граф не является полуэйлеровым");
                }
                else Console.WriteLine("Неверный формат ввода матрицы!");
                
                Console.WriteLine("Нажмите Enter, чтобы вернуться в меню...");
                Console.ReadLine();                
            }
        }
    }
}
