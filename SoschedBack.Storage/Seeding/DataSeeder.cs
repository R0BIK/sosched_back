using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoschedBack.Storage.Seeding;

// Замените на ваш DbContext
// using YourProject.Data; 

public static class DataSeeder
{
    /// <summary>
    /// Заполняет базу данных из JSON-файла, если она пуста.
    /// </summary>
    /// <param name="context">Контекст базы данных (ваш ApplicationDbContext)</param>
    /// <param name="seedFilePath">Путь к файлу seed.json</param>
    public static async Task SeedAsync(SoschedBackDbContext context, string seedFilePath)
    {
        // 1. Проверяем, есть ли уже данные
        // Мы проверяем "Spaces", так как это одна из первых таблиц.
        if (await context.Spaces.AnyAsync())
        {
            Console.WriteLine("База данных уже заполнена. Сидинг отменен.");
            return;
        }

        Console.WriteLine("База данных пуста. Начинаем сидинг...");

        // 2. Читаем и десериализуем JSON
        var jsonData = await File.ReadAllTextAsync(seedFilePath);
        
        var options = new JsonSerializerOptions
        {
            // Используем, если регистр в JSON (camelCase) не совпадает с C# (PascalCase)
            PropertyNameCaseInsensitive = true 
        };
        
        var seedData = JsonSerializer.Deserialize<SeedDataContainer>(jsonData, options);

        if (seedData == null)
        {
            throw new Exception("Не удалось десериализовать seed.json.");
        }

        // 3. Используем транзакцию для безопасности
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            // 4. Вставляем данные в ПРАВИЛЬНОМ ПОРЯДКЕ
            
            // Группа 1: Сущности без внешних ключей
            if (seedData.Spaces != null) await context.Spaces.AddRangeAsync(seedData.Spaces);
            if (seedData.Permissions != null) await context.Permissions.AddRangeAsync(seedData.Permissions);
            if (seedData.EventTypes != null) await context.EventTypes.AddRangeAsync(seedData.EventTypes);
            await context.SaveChangesAsync();

            // Группа 2: Зависят от Группы 1
            if (seedData.Roles != null) await context.Roles.AddRangeAsync(seedData.Roles);
            if (seedData.TagTypes != null) await context.TagTypes.AddRangeAsync(seedData.TagTypes);
            await context.SaveChangesAsync();

            // Группа 3: Зависят от Групп 1 и 2
            // ВАЖНО: ХЭШИРОВАНИЕ ПАРОЛЕЙ
            // ... (ваш код хэширования)
            if (seedData.Users != null) await context.Users.AddRangeAsync(seedData.Users);
            if (seedData.Tags != null) await context.Tags.AddRangeAsync(seedData.Tags);
            await context.SaveChangesAsync();

            // Группа 4: Связующие таблицы (Junction tables)
            if (seedData.PermissionToRoles != null) await context.PermissionToRoles.AddRangeAsync(seedData.PermissionToRoles);
            if (seedData.SpaceUsers != null) await context.SpaceUsers.AddRangeAsync(seedData.SpaceUsers);
            await context.SaveChangesAsync();
            
            // Группа 5: События (Events)
            if (seedData.Events != null) await context.Events.AddRangeAsync(seedData.Events);
            await context.SaveChangesAsync();

            // Группа 6: Оставшиеся связующие таблицы
            // <<< ИСПРАВЛЕНО: Должно быть TagToUsers, а не TagToSpaceUsers
            if (seedData.TagToSpaceUsers != null) await context.TagToSpaceUsers.AddRangeAsync(seedData.TagToSpaceUsers); 
            if (seedData.EventToSpaceUsers != null) await context.EventToSpaceUsers.AddRangeAsync(seedData.EventToSpaceUsers);
            await context.SaveChangesAsync();

            // 5. СБРОС SEQUENCE-ГЕНЕРАТОРОВ (Только для PostgreSQL)
            // Это самая важная часть.
            await ResetAllSequences(context);

            // 6. Фиксируем транзакцию
            await transaction.CommitAsync();
            Console.WriteLine("Сидинг успешно завершен.");
        }
        catch (Exception ex)
        {
            // 7. В случае ошибки откатываем изменения
            await transaction.RollbackAsync();
            Console.WriteLine($"Ошибка во время сидинга: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// (Только для PostgreSQL)
    /// Сбрасывает генераторы ID (sequences) для всех таблиц.
    /// Это необходимо после ручной вставки ID, чтобы следующие INSERT не вызывали ошибок.
    /// </summary>
    private static async Task ResetAllSequences(SoschedBackDbContext context)
    {
        // Список всех таблиц, у которых есть 'Id' c SERIAL/IDENTITY
        // Имена должны ТОЧНО соответствовать вашей БД (включая кавычки, т.к. у вас camelCase)
        var tables = new[]
        {
            "Permissions", "EventTypes", "Spaces", "Roles", "PermissionToRoles",
            "Users", "Events", 
            "EventToSpaceUsers", // <<< ИСПРАВЛЕНО: Было "EventToUsers"
            "TagTypes", "Tags", "TagToSpaceUsers", "SpaceUsers"
        };

        Console.WriteLine("Сброс sequence-генераторов PostgreSQL...");

        foreach (var table in tables)
        {
            // Используем кавычки, так как ваши имена таблиц "смешанногоРегистра"
            var sql = $"SELECT setval(pg_get_serial_sequence('\"{table}\"', 'Id'), (SELECT MAX(\"Id\") FROM \"{table}\"));";
            
            try
            {
                await context.Database.ExecuteSqlRawAsync(sql);
            }
            catch (Exception ex)
            {
                // Может возникнуть ошибка, если таблица пуста (MAX(Id) вернет null)
                // Важно: если здесь произойдет ошибка, она вызовет откат транзакции.
                Console.WriteLine($"Предупреждение при сбросе sequence для '{table}': {ex.Message}");
                // Выбрасываем ошибку, чтобы транзакция откатилась
                throw; 
            }
        }
    }
}