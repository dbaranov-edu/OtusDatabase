using Dapper;
using Npgsql;
using OtusDatabase.Entities;
using OtusDatabase.Repositories;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;

namespace OtusDatabase
{
    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;
            
            var connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Console.WriteLine($"Завершение работы: незадана строка подключения в App.config");
                Console.ReadKey();
                return;
            }

            try
            {
                using var connection = new NpgsqlConnection(connectionString);
                var now = connection.Query<DateTime>("select now()").ElementAtOrDefault(0);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Завершение работы: не вышло подключиться к БД: {e?.Message}");
                Console.ReadKey();
                return;
            }

            var userRepo = new UserRepository(connectionString);
            var offerRepo = new OfferRepository(connectionString);
            var commentRepo = new CommentRepository(connectionString);

            while (true)
            {
                Console.WriteLine(new string('*', 80));
                Console.WriteLine("Выход из приложения [Q].");
                Console.WriteLine("Вывести: [1] пользователей, [2] предложения/объявления, [3] комментарии.");
                Console.WriteLine("Добавить: [4] пользователя, [5] предложение/объявление, [6] комментарий.");

                Console.Write("Нажмите клавишу: ");
                var key = Console.ReadKey();
                Console.WriteLine();

                switch (Char.ToLower(key.KeyChar))
                {
                    case '1':
                        PrintUsers(userRepo);
                        break;
                    case '2':
                        PrintOffers(offerRepo);
                        break;
                    case '3':
                        PrintComments(commentRepo);
                        break;
                    case '4':
                        CreateUser(userRepo);
                        break;
                    case '5':
                        CreateOffer(offerRepo);
                        break;
                    case '6':
                        CreateComment(commentRepo);
                        break;
                    case 'q':
                        Console.Write("Выход из приложения");
                        return;
                }
            }
        }

        static void PrintUsers(UserRepository repo)
        {
            Console.WriteLine("Вывод пользователей");
            PrintAll(repo.GetAllAsync().Result);
        }

        static void PrintOffers(OfferRepository repo)
        {
            Console.WriteLine("Вывод предложений");
            PrintAll(repo.GetAllAsync().Result);
        }

        static void PrintComments(CommentRepository repo)
        {
            Console.WriteLine("Вывод комментариев");
            PrintAll(repo.GetAllAsync().Result);
        }

        static void CreateUser(UserRepository repo)
        {
            Console.WriteLine("Добавление пользователя");

            var user = new User();
            {
                Console.Write("Введите имя: ");
                var userFirstNameConsole = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(userFirstNameConsole) && Regex.IsMatch(userFirstNameConsole, "^([a-zA-Zа-яА-Я]+)$"))
                {
                    user.FirstName = userFirstNameConsole;
                }
                else
                {
                    Console.WriteLine($"Ошибка: некорректное имя.");
                    return;
                }

                Console.Write("Введите фамилию: ");
                var userLastNameConsole = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(userLastNameConsole) && Regex.IsMatch(userLastNameConsole, "^([a-zA-Zа-яА-Я]+)$"))
                {
                    user.LastName = userLastNameConsole;
                }
                else
                {
                    Console.WriteLine($"Ошибка: некорректная фамилия.");
                    return;
                }

                Console.Write("Введите e-mail: ");
                var userEmailConsole = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(userEmailConsole) && Regex.IsMatch(userEmailConsole, "^([\\w\\-\\.]+)[@]([\\w\\-]+)[.]([a-zA-Z]{2,4})$"))
                {
                    user.Email = userEmailConsole;
                }
                else
                {
                    Console.WriteLine($"Ошибка: некорректный e-mail.");
                    return;
                }    

                Console.Write($"Введите дату рождения (напр. {DateTime.Now.AddYears(-18).ToShortDateString()}) или нажмите [Enter] чтобы пропустить: ");
                var userBirthDateConsole = Console.ReadLine();                
                if (!string.IsNullOrWhiteSpace(userBirthDateConsole))
                {
                    if (DateTime.TryParse(userBirthDateConsole, out DateTime userBirthDate))
                    {
                        user.BirthDate = userBirthDate.Date;
                    }
                    else
                    {
                        Console.WriteLine($"Ошибка: некорректная дата рождения.");
                        return;
                    }
                }
            }

            try
            {
                long id = repo.AddAsync(user).Result;
                Console.WriteLine($"Пользователь добавлен (ID={id})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: не вышло добавить пользователя! {ex?.Message}");
            }
        }

        static void CreateOffer(OfferRepository repo)
        {
            Console.WriteLine("Добавление предложения/объявления");

            var offer = new Offer();
            {
                Console.Write("Введите ID пользователя (владельца объявления): ");
                if (int.TryParse(Console.ReadLine(), out int offerUserId))
                {
                    offer.UserId = offerUserId;
                }
                else
                {
                    Console.WriteLine($"Ошибка: некорректный Id пользователя.");
                    return;
                }

                Console.Write("Введите стоимость: ");
                if (decimal.TryParse(Console.ReadLine(), out decimal offerPrice))
                {
                    offer.Price = offerPrice;
                }
                else
                {
                    Console.WriteLine($"Ошибка: некорректная стоимость.");
                    return;
                }

                Console.Write("Введите заголовок: ");
                var offerTitleConsole = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(offerTitleConsole))
                {
                    offer.Title = offerTitleConsole;
                }
                else
                {
                    Console.WriteLine($"Ошибка: пустой заголовок прдложения/объявления.");
                    return;
                }

                Console.Write("Введите описание: ");
                var offerDescriptionConsole = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(offerDescriptionConsole))
                {
                    offer.Description = offerDescriptionConsole;
                }
                else
                {
                    Console.WriteLine($"Ошибка: пустое описание прдложения/объявления.");
                    return;
                }
            }

            try
            {
                long id = repo.AddAsync(offer).Result;
                Console.WriteLine($"Предложение/объявление добавлено (ID={id})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: не вышло добавить предложение/объявление! {ex?.Message}");
            }
        }

        static void CreateComment(CommentRepository repo)
        {
            Console.WriteLine("Добавление комментария");

            var comment = new Comment();
            {
                Console.Write("Введите ID предложения (объявления): ");
                if (int.TryParse(Console.ReadLine(), out int commentOfferId))
                {
                    comment.OfferId = commentOfferId;
                }
                else
                {
                    Console.WriteLine($"Ошибка: некорректный Id предложения/объявления.");
                    return;
                }

                Console.Write("Введите ID пользователя (автора комментария): ");
                if (int.TryParse(Console.ReadLine(), out int commentUserId))
                {
                    comment.UserId = commentUserId;
                }
                else
                {
                    Console.WriteLine($"Ошибка: некорректный Id пользователя (автора комментария).");
                    return;
                }

                Console.Write("Введите содержание комментария (текст): ");
                var commentContentConsole = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(commentContentConsole))
                {
                    comment.Content = commentContentConsole;
                }
                else
                {
                    Console.WriteLine($"Ошибка: пустой комментарий.");
                    return;
                }
            }

            try
            {
                long id = repo.AddAsync(comment).Result;
                Console.WriteLine($"Комментарий добавлен (ID={id})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: не вышло добавить комментарий! {ex?.Message}");
            }
        }

        static void PrintAll<IEntity>(IEnumerable<IEntity>? entities)
        {
            Console.WriteLine($"Количество записей: {entities?.Count() ?? 0}");
            foreach (var entity in entities ?? Enumerable.Empty<IEntity>())
            {
                Console.WriteLine($"{entity}");
            }
        }
    }
}
