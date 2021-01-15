﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame
{
	class Tank
	{
		readonly TankTypes TankType;
		int Armor;
		readonly float Damage;
		NaturalNumber Ammo;
		public readonly NaturalNumber Health;

		public bool IsAlive => !Health.IsZero;
		

		public void Shoot(Tank enemy)
		{
			// Создание исходного сообщения
			var message = $"Произведён выстрел на {Damage} урона.\n";
			var currentDamage = Damage;

			if (Ammo == 0)
			{
				if (TankType == TankTypes.Player)
				{
					message = "Не хватает патронов, внимательнее, мы пропустили ход!";
				}
				else
				{
					BuyAmmo();
					return;
				}
			}
			else
			{
				Ammo.Subtract(1);
				Random r = new Random();
				// 10%-ый шанс на критический выстрел
				if (0.1 > r.NextDouble())
				{
					currentDamage *= 1.2f;
					message = $"Произведён критический выстрел на {currentDamage} урона.\n";
				}
				// 20%-ый шанс на промах
				else if (0.2 > r.NextDouble())
				{
					currentDamage = 0;
					message = $"Произведён тактический промах с целью запугивания.\n";
				}	
				enemy.TakeDamage(currentDamage);
			}
			Console.WriteLine(message);
		}
		// Метод получения урона с учётом брони
		void TakeDamage(float damage)
		{
			// Броня сокращает урон вдвое
			if (Armor > 0 && damage != 0)
			{
				damage /= 2f;
				Armor--;
				Console.WriteLine("Урон снаряда был погашен наполовину.");
			}
			// Перевод Float -> Int приближает к ЧЁТНОМУ целому числу
			// Для лучшей точности добавлен Math.Round
			Health.Subtract(Convert.ToInt32(Math.Round(damage)));
		}

		public void BuyAmmo(int ammoAmount)
		{
			Ammo.Add(Math.Abs(ammoAmount));
			if (Ammo.IsMax)
				Console.WriteLine("Закуплено патронов до максимума.");
			else
				Console.WriteLine($"Закуплено {ammoAmount} патронов.");
		}
		// Закупка патронов без конкретного объёма закупает четверть от максимума
		// Или как минимум 1 патрон
		public void BuyAmmo()
		{
			int ammoAmount = (Ammo.Max / 4).Clamp(1, Ammo.Max);
			BuyAmmo(ammoAmount);
		}

		public void Repair(int amount)
		{
			Health.Add(amount);
			if (Health.IsMax)
			{
				Console.WriteLine("Произведена 100 %-ная починка.");
			}	
			else
			{
				Console.WriteLine($"Произведена починка на {amount} единиц.");
			}
		}
		//Починка без конкретного количества восстанавливает 1/8 максимального здоровья
		public void Repair()
		{
			int healthAmount = (Health.Max / 8).Clamp(1, Health.Max);
			Repair(healthAmount);
		}

		public void WriteInfo()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append($"Здоровье: {(int)Health}/{Health.Max}\n");
			if (Armor > 0)
				sb.Append($"Броня: {Armor}\n");
			sb.Append($"Количество патронов: {(int)Ammo}/{Ammo.Max}\n");

			Console.WriteLine(sb.ToString());
		}
		public Tank(TankTypes tankType, int armor, int damage, int ammo, int health)
		{
			TankType = tankType;
			Armor = armor;
			Damage = damage;
			Ammo = (NaturalNumber)ammo;
			Health = (NaturalNumber)health;
		}
	}
	enum TankTypes
	{
		Player,
		Enemy
	}
	
	/// <summary>
	/// Класс целочисленного значения, ограниченного нулём и максимумом
	/// </summary>
	class NaturalNumber
	{
		int Num;
		readonly int MaxNum;

		public void Add(int num)
		{
			Num = (Num + num).Clamp(0, MaxNum);
		}
		public void Subtract(int num)
		{
			Num = (Num - num).Clamp(0, MaxNum);
		}
		public int Max => MaxNum;
		public bool IsMax => Num == MaxNum;
		public bool IsZero => Num == 0;

		public NaturalNumber(int num, int maxNum)
		{
			Num = num;
			MaxNum = maxNum;
		}
		public NaturalNumber(int num) : this(num, num)
		{ }

		public static implicit operator int(NaturalNumber number)
		{
			return number.Num;
		}
		public static explicit operator NaturalNumber(int number)
		{
			return new NaturalNumber(number);
		}
	}



	// Расширение функциональности int
	// Добавленная функция Clamp используется в NaturalNumber
	public static partial class ExtensionMethods
	{
		public static int Clamp(this int value, int min, int max)
		{
			return (value < min) ? min : (value > max) ? max : value;
		}
	}

	class Program
	{
		static void ShowInfoAndActions(Tank player, Tank enemy)
		{
			Console.WriteLine("Состояние игрока:");
			player.WriteInfo();
			Console.WriteLine("Состояние врага:");
			enemy.WriteInfo();

			Console.WriteLine("[НАШ ХОД]\n" +
				"Действия (нажмите соответствующую клавишу):\n" +
				"1. Выстрел.\n" +
				"2. Починка.\n" +
				"3. Закупка патронов.\n");
		}
		static void EnemyMove(Tank enemy, Tank player)
		{
			
			if (enemy.Health.IsMax)
			{
				enemy.Shoot(player);
			}
			else
			{
				Random r = new Random();
				if (0.5f > r.NextDouble())
				{
					enemy.Shoot(player);
				}
				else
				{
					enemy.Repair();
				}
			}
		}


		static void Main(string[] args)
		{
			// Создание объектов игрока и врага 
			Tank player = new Tank(TankTypes.Player, 5, 15, 10, 50);
			Tank enemy = new Tank(TankTypes.Enemy, 2, 10, 5, 100);

			Console.WriteLine("Добро пожаловать на танковый полигон, стреляйте первым!");

			// Главный цикл игры
			while (player.IsAlive && enemy.IsAlive)
			{
				ShowInfoAndActions(player, enemy);

				// Считывание нажатой клавиши игрока
				char playerInput = Console.ReadKey(true).KeyChar;
				
				// Недопуск неверного ввода
				while (playerInput < '1' || playerInput > '3')
				{
					Console.WriteLine("Попробуйте нажать те циферки сверху! У вас получится!");
					playerInput = Console.ReadKey(true).KeyChar;
				}
				switch(playerInput)
				{
					case '1':
						player.Shoot(enemy); 
						break;
					case '2':
						player.Repair();
						break;
					case '3':
						player.BuyAmmo();
						break;
				}
				Console.WriteLine("[ХОД СУПОСТАТА]");
				EnemyMove(enemy, player);
				Console.WriteLine("Нажмите любую клавишу, чтобы продолжить.");
				Console.ReadKey(true);
				Console.Clear();
			}
			if (player.Health.IsZero && enemy.Health.IsZero)
			{
				Console.WriteLine("Случай сыграл с Вами в злую шутку, ничья...");
			}
			else if (enemy.Health.IsZero)
			{
				Console.WriteLine("К сожалению, Вы проиграли...");
			}
			else
			{
				Console.WriteLine("Фух, победа! Теперь можете насладиться этим смайликом -> ☺");
			}
			Console.ReadLine();
		}
	}
}
